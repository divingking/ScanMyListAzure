using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Web;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.Services;
using Intuit.Ipp.Data;
using Intuit.Ipp.AsyncServices;
using Intuit.Ipp.Diagnostics;
using Intuit.Ipp.Exception;
using Intuit.Ipp.Retry;
using Intuit.Ipp.Utility;

using ERPIntegrationWorkerRole.QuickBookIntegration.Utils;
using ERPIntegrationWorkerRole.SynchIntegration;

namespace ERPIntegrationWorkerRole.QuickBookIntegration
{
    class QBDIntegrator
    {
        // QBD side of fields
        string realmId;
        string accessToken;
        string accessTokenSecret;
        string consumerKey;
        string consumerSecret;
        string dataSourcetype;

        Dictionary<String, Intuit.Ipp.Data.Qbd.Item> itemNameToItemMap;

        // Synch side of fields
        int synchBusinessId;

        public QBDIntegrator(int bid, string realmId, string accessToken, string accessTokenSecret, string consumerKey, string consumerSecret, string dataSourcetype)
        {
            this.synchBusinessId = bid;

            this.realmId = realmId;
            this.accessToken = accessToken;
            this.accessTokenSecret = accessTokenSecret;
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.dataSourcetype = dataSourcetype.ToUpper();

            itemNameToItemMap = new Dictionary<string, Intuit.Ipp.Data.Qbd.Item>();
        }


        #region Update QuickBooks Desktop from Synch
		 
        public void createInvoiceInQBD(int rid)
        {
            // validating
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);
            DataServices commonService = new DataServices(context);

            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var results = synchDataContext.GetCompleteOrder(synchBusinessId, rid);

            List<RecordProduct> products = new List<RecordProduct>();
            
            double balance = 0.0;

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        name = product.product_name,
                        upc = product.product_upc,
                        customer = product.customer_id,
                        quantity = (int)product.product_quantity,
                        price = (double)product.product_price,
                        note = product.product_note
                    });
                balance += (double)product.product_price * (int)product.product_quantity;
            }
            
            var result = synchDataContext.GetCustomerById(synchBusinessId, products[0].customer);
            IEnumerator<GetCustomerByIdResult> customerEnumerator = result.GetEnumerator();
            Business customerInSynch = null;
            if (customerEnumerator.MoveNext())
            {
                GetCustomerByIdResult retrievedBusiness = customerEnumerator.Current;

                customerInSynch = new Business()
                {
                    id = retrievedBusiness.id,
                    name = retrievedBusiness.name,
                    address = retrievedBusiness.address,
                    zip = (int)retrievedBusiness.zip,
                    email = retrievedBusiness.email
                };
            }
            else
                return;     // no customer found

            string customerName = customerInSynch.name;
            string[] customerAddress = customerInSynch.address.Split(',');
            string customerEmail = customerInSynch.email;
            string zipCode = customerInSynch.zip.ToString();
            string stateCode = customerAddress[customerAddress.Length - 1].Split(' ')[1];

            // creates actual invoice
            Intuit.Ipp.Data.Qbd.PhysicalAddress physicalAddress = new Intuit.Ipp.Data.Qbd.PhysicalAddress();
            physicalAddress.Line1 = customerAddress[0];
            if (customerAddress.Length >= 4)
                physicalAddress.Line2 = customerAddress[1];

            physicalAddress.City = customerAddress[customerAddress.Length - 2];
            physicalAddress.CountrySubDivisionCode = stateCode;
            physicalAddress.Country = "USA";
            physicalAddress.PostalCode = zipCode;
            physicalAddress.Tag = new string[] { "Billing" };

            Intuit.Ipp.Data.Qbd.InvoiceHeader invoiceHeader = new Intuit.Ipp.Data.Qbd.InvoiceHeader();
            //invoiceHeader.ARAccountId = new Intuit.Ipp.Data.Qbd.IdType() { idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB, Value = "37" };
            //invoiceHeader.ARAccountName = "Accounts Receivable";
            // original code : invoiceHeader.CustomerId = new IdType() { idDomain = idDomainEnum.NG, Value = "3291253" };
            invoiceHeader.CustomerName = customerName;
            //invoiceHeader.CustomerId = new Intuit.Ipp.Data.Qbd.IdType() { idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB, Value = "2" };

            invoiceHeader.Balance = (decimal)balance;
            invoiceHeader.BillAddr = physicalAddress;
            invoiceHeader.BillEmail = customerEmail;
            invoiceHeader.DocNumber = rid.ToString();
            invoiceHeader.DueDate = DateTime.Now;
            invoiceHeader.ShipAddr = physicalAddress;
            invoiceHeader.ShipDate = DateTime.Now;
            invoiceHeader.TaxRate = (decimal).09;
            invoiceHeader.TaxAmt = (decimal)balance * invoiceHeader.TaxRate;
            invoiceHeader.ToBeEmailed = false;
            invoiceHeader.TotalAmt = invoiceHeader.TaxAmt + invoiceHeader.Balance;

            List<Intuit.Ipp.Data.Qbd.InvoiceLine> listLine = new List<Intuit.Ipp.Data.Qbd.InvoiceLine>();

            createItemNameToItemMap();
            // add all the items in the record into inovice lines
            foreach (RecordProduct curProduct in products)
            {
                // get item id by querying QBD
                string itemIdString = "-1";
                if (itemNameToItemMap.ContainsKey(curProduct.name))
                {
                    Intuit.Ipp.Data.Qbd.Item item = itemNameToItemMap[curProduct.name];
                    itemIdString = item.Id.Value;
                }
                Intuit.Ipp.Data.Qbd.ItemsChoiceType2[] invoiceItemAttributes =
                { 
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.ItemId,
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.UnitPrice,
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.Qty 
                };
                object[] invoiceItemValues =
                {
                    new Intuit.Ipp.Data.Qbd.IdType() 
                    {
                        idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB,
                        Value = itemIdString
                    },
                    new decimal(curProduct.price),
                    new decimal(curProduct.quantity) 
                };

                var invoiceLine = new Intuit.Ipp.Data.Qbd.InvoiceLine();
                invoiceLine.Amount = (Decimal)curProduct.price * curProduct.quantity;
                invoiceLine.AmountSpecified = true;
                invoiceLine.Desc = curProduct.note;
                invoiceLine.ItemsElementName = invoiceItemAttributes;
                invoiceLine.Items = invoiceItemValues;
                invoiceLine.ServiceDate = DateTime.Now;
                invoiceLine.ServiceDateSpecified = true;
                listLine.Add(invoiceLine);
            }

            Intuit.Ipp.Data.Qbd.Invoice invoice = new Intuit.Ipp.Data.Qbd.Invoice();
            invoice.Header = invoiceHeader;
            invoice.Line = listLine.ToArray();

            Intuit.Ipp.Data.Qbd.Invoice addedInvoice = commonService.Add(invoice);
        }

        public void createSalesOrderInQBD(int rid)
        {
            // validating
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);
            DataServices commonService = new DataServices(context);

            // get sales order information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var results = synchDataContext.GetCompleteOrder(synchBusinessId, rid);

            List<RecordProduct> products = new List<RecordProduct>();

            double balance = 0.0;

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        name = product.product_name,
                        upc = product.product_upc,
                        customer = product.customer_id,
                        quantity = (int)product.product_quantity,
                        price = (double)product.product_price,
                        note = product.product_note
                    });
                balance += (double)product.product_price * (int)product.product_quantity;
            }

            var result = synchDataContext.GetCustomerById(synchBusinessId, products[0].customer);
            IEnumerator<GetCustomerByIdResult> customerEnumerator = result.GetEnumerator();
            Business customerInSynch = null;
            if (customerEnumerator.MoveNext())
            {
                GetCustomerByIdResult retrievedBusiness = customerEnumerator.Current;

                customerInSynch = new Business()
                {
                    id = retrievedBusiness.id,
                    name = retrievedBusiness.name,
                    address = retrievedBusiness.address,
                    zip = (int)retrievedBusiness.zip,
                    email = retrievedBusiness.email
                };
            }
            else
                return;     // no customer found

            string customerName = customerInSynch.name;
            string[] customerAddress = customerInSynch.address.Split(',');
            string customerEmail = customerInSynch.email;
            string zipCode = customerInSynch.zip.ToString();
            string stateCode = customerAddress[customerAddress.Length - 1].Split(' ')[1];

            // creates actual invoice
            Intuit.Ipp.Data.Qbd.PhysicalAddress physicalAddress = new Intuit.Ipp.Data.Qbd.PhysicalAddress();
            physicalAddress.Line1 = customerAddress[0];
            if (customerAddress.Length >= 4)
                physicalAddress.Line2 = customerAddress[1];

            physicalAddress.City = customerAddress[customerAddress.Length - 2];
            physicalAddress.CountrySubDivisionCode = stateCode;
            physicalAddress.Country = "USA";
            physicalAddress.PostalCode = zipCode;
            physicalAddress.Tag = new string[] { "Billing" };

            Intuit.Ipp.Data.Qbd.SalesOrderHeader salesOrderHeader = new Intuit.Ipp.Data.Qbd.SalesOrderHeader();
            //invoiceHeader.ARAccountId = new Intuit.Ipp.Data.Qbd.IdType() { idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB, Value = "37" };
            //invoiceHeader.ARAccountName = "Accounts Receivable";
            // original code : invoiceHeader.CustomerId = new IdType() { idDomain = idDomainEnum.NG, Value = "3291253" };
            salesOrderHeader.CustomerName = customerName;
            //invoiceHeader.CustomerId = new Intuit.Ipp.Data.Qbd.IdType() { idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB, Value = "2" };

            salesOrderHeader.Balance = (decimal)balance;
            salesOrderHeader.BillAddr = physicalAddress;
            salesOrderHeader.BillEmail = customerEmail;
            salesOrderHeader.DocNumber = rid.ToString();
            salesOrderHeader.DueDate = DateTime.Now;
            salesOrderHeader.ShipAddr = physicalAddress;
            salesOrderHeader.ShipDate = DateTime.Now;
            salesOrderHeader.TaxRate = (decimal).09;
            salesOrderHeader.TaxAmt = (decimal)balance * salesOrderHeader.TaxRate;
            salesOrderHeader.ToBeEmailed = false;
            salesOrderHeader.TotalAmt = salesOrderHeader.TaxAmt + salesOrderHeader.Balance;

            List<Intuit.Ipp.Data.Qbd.SalesOrderLine> listLine = new List<Intuit.Ipp.Data.Qbd.SalesOrderLine>();

            createItemNameToItemMap();
            // add all the items in the record into inovice lines
            foreach (RecordProduct curProduct in products)
            {
                // get item id by querying QBD
                string itemIdString = "-1";
                if (itemNameToItemMap.ContainsKey(curProduct.name))
                {
                    Intuit.Ipp.Data.Qbd.Item item = itemNameToItemMap[curProduct.name];
                    itemIdString = item.Id.Value;
                }
                Intuit.Ipp.Data.Qbd.ItemsChoiceType2[] salesOrderItemAttributes =
                { 
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.ItemId,
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.UnitPrice,
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.Qty 
                };
                object[] salesOrderItemValues =
                {
                    new Intuit.Ipp.Data.Qbd.IdType() 
                    {
                        idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB,
                        Value = itemIdString
                    },
                    new decimal(curProduct.price),
                    new decimal(curProduct.quantity) 
                };

                var salesOrderLine = new Intuit.Ipp.Data.Qbd.SalesOrderLine();
                salesOrderLine.Amount = (Decimal)curProduct.price * curProduct.quantity;
                salesOrderLine.AmountSpecified = true;
                salesOrderLine.Desc = curProduct.note;
                salesOrderLine.ItemsElementName = salesOrderItemAttributes;
                salesOrderLine.Items = salesOrderItemValues;
                //salesOrderLine.ServiceDate = DateTime.Now;
                //salesOrderLine.ServiceDateSpecified = true;
                listLine.Add(salesOrderLine);
            }

            Intuit.Ipp.Data.Qbd.SalesOrder salesOrder = new Intuit.Ipp.Data.Qbd.SalesOrder();
            salesOrder.Header = salesOrderHeader;
            salesOrder.Line = listLine.ToArray();

            Intuit.Ipp.Data.Qbd.SalesOrder addedSalesOrder = commonService.Add(salesOrder);
        }

        public void createBusinessInQBD(int otherBid, bool isCustomer)
        {
            // validating
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);
            DataServices commonService = new DataServices(context);

            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetBusiness(otherBid);
            IEnumerator<GetBusinessResult> businessEnumerator = result.GetEnumerator();
            Business businessInSynch = null;
            if (businessEnumerator.MoveNext())
            {
                GetBusinessResult retrievedBusiness = businessEnumerator.Current;

                businessInSynch = new Business()
                {
                    id = retrievedBusiness.id,
                    name = retrievedBusiness.name,
                    address = retrievedBusiness.address,
                    zip = (int)retrievedBusiness.zip,
                    email = retrievedBusiness.email,
                    phoneNumber = retrievedBusiness.phone_number
                };
            }
            else
                return;     // no business found

            string name = businessInSynch.name;
            string[] address = businessInSynch.address.Split(',');
            string email = businessInSynch.email;
            string zipCode = businessInSynch.zip.ToString();
            string stateCode = address[address.Length - 1].Trim();
            string phoneNumber = businessInSynch.phoneNumber;
            if (isCustomer)
            {
                // create a customer in QBD
                Intuit.Ipp.Data.Qbd.Customer newCustomer = new Intuit.Ipp.Data.Qbd.Customer();
                newCustomer.Name = name;
                
                // add address
                Intuit.Ipp.Data.Qbd.PhysicalAddress ippAddress = new Intuit.Ipp.Data.Qbd.PhysicalAddress();
                ippAddress.Line1 = address[0];
                if (address.Length >= 4)
                    ippAddress.Line2 = address[1];
                ippAddress.City = address[address.Length - 2];
                ippAddress.CountrySubDivisionCode = stateCode;
                ippAddress.Country = "USA";
                ippAddress.PostalCode = zipCode;
                ippAddress.Tag = new string[] { "Billing" };
                newCustomer.Address = new Intuit.Ipp.Data.Qbd.PhysicalAddress[] { ippAddress };

                // add phone number
                Intuit.Ipp.Data.Qbd.TelephoneNumber ippPhoneNumber = new Intuit.Ipp.Data.Qbd.TelephoneNumber();
                ippPhoneNumber.FreeFormNumber = phoneNumber;
                ippPhoneNumber.Tag = new string[] { "Business" };
                newCustomer.Phone = new Intuit.Ipp.Data.Qbd.TelephoneNumber[] { ippPhoneNumber };

                // add email address
                Intuit.Ipp.Data.Qbd.EmailAddress ippEmail = new Intuit.Ipp.Data.Qbd.EmailAddress();
                ippEmail.Address = email;
                ippEmail.Tag = new string[] { "Business" };
                newCustomer.Email = new Intuit.Ipp.Data.Qbd.EmailAddress[] { ippEmail };

                Intuit.Ipp.Data.Qbd.Customer addedCustomer = commonService.Add(newCustomer);
            }
            else
            {
                // create a vendor in QBD
                Intuit.Ipp.Data.Qbd.Vendor newVendor = new Intuit.Ipp.Data.Qbd.Vendor();
                newVendor.Name = name;

                // add address
                Intuit.Ipp.Data.Qbd.PhysicalAddress ippAddress = new Intuit.Ipp.Data.Qbd.PhysicalAddress();
                ippAddress.Line1 = address[0];
                if (address.Length >= 4)
                    ippAddress.Line2 = address[1];
                ippAddress.City = address[address.Length - 2];
                ippAddress.CountrySubDivisionCode = stateCode;
                ippAddress.Country = "USA";
                ippAddress.PostalCode = zipCode;
                ippAddress.Tag = new string[] { "Billing" };
                newVendor.Address = new Intuit.Ipp.Data.Qbd.PhysicalAddress[] { ippAddress };

                // add phone number
                Intuit.Ipp.Data.Qbd.TelephoneNumber ippPhoneNumber = new Intuit.Ipp.Data.Qbd.TelephoneNumber();
                ippPhoneNumber.FreeFormNumber = phoneNumber;
                newVendor.Phone = new Intuit.Ipp.Data.Qbd.TelephoneNumber[] { ippPhoneNumber };

                // add email address
                Intuit.Ipp.Data.Qbd.EmailAddress ippEmail = new Intuit.Ipp.Data.Qbd.EmailAddress();
                ippEmail.Address = email;
                newVendor.Email = new Intuit.Ipp.Data.Qbd.EmailAddress[] { ippEmail };

                Intuit.Ipp.Data.Qbd.Vendor addedVendor = commonService.Add(newVendor);
            }
        }

        public void createItemInQbd(string upc)
        {
            // validating
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);
            DataServices commonService = new DataServices(context);

            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetInventoryByUpc(synchBusinessId, upc);
            IEnumerator<GetInventoryByUpcResult> productEnumerator = result.GetEnumerator();
            Product newItemInSynch = null;
            if (productEnumerator.MoveNext())
            {
                GetInventoryByUpcResult target = productEnumerator.Current;
                newItemInSynch = new Product()
                {
                    upc = target.upc,
                    name = target.name,
                    detail = target.detail,
                    quantity = (int)target.quantity,
                    location = target.location,
                    owner = synchBusinessId,
                    leadTime = (int)target.lead_time,
                    price = (double)target.default_price
                };
            }

            if (newItemInSynch != null)
            {
                Intuit.Ipp.Data.Qbd.Item newItem = new Intuit.Ipp.Data.Qbd.Item();
                newItem.Active = true;
                newItem.Name = newItemInSynch.name;
                newItem.Desc = newItemInSynch.detail;
                newItem.QtyOnHand = newItemInSynch.quantity;
                newItem.QtyOnHandSpecified = true;
                newItem.Type = Intuit.Ipp.Data.Qbd.ItemTypeEnum.Inventory;
                newItem.TypeSpecified = true;
                
                // assign income account
                Intuit.Ipp.Data.Qbd.AccountRef incomeAccountRef = new Intuit.Ipp.Data.Qbd.AccountRef();
                incomeAccountRef.AccountName = "AB Sales-Wholesale:Wines Sold:Importer Wines:Hand of God";
                newItem.IncomeAccountRef = incomeAccountRef;

                // assign COGS account
                Intuit.Ipp.Data.Qbd.AccountRef cogsAccountRef = new Intuit.Ipp.Data.Qbd.AccountRef();
                cogsAccountRef.AccountName = "Cost of Inventory Sold:Cost of Wine:Importer Wines:Hand of God";
                newItem.COGSAccountRef = cogsAccountRef;

                // assign asset account
                Intuit.Ipp.Data.Qbd.AccountRef assetAccountRef = new Intuit.Ipp.Data.Qbd.AccountRef();
                assetAccountRef.AccountName = "Inventory Asset";
                newItem.AssetAccountRef = assetAccountRef;

                // assign money
                Intuit.Ipp.Data.Qbd.Money ippMoney = new Intuit.Ipp.Data.Qbd.Money();
                ippMoney.Amount = (decimal)newItemInSynch.price;
                ippMoney.CurrencyCode = Intuit.Ipp.Data.Qbd.currencyCode.USD;
                newItem.AvgCost = ippMoney;

                newItem.Name = newItem.Name.Substring(0, 31);

                Intuit.Ipp.Data.Qbd.Item addedItem = commonService.Add(newItem);
            }
        }

        public void updateItemInQbd(string upc)
        {
            // validating
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);
            DataServices commonService = new DataServices(context);

            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetInventoryByUpc(synchBusinessId, upc);
            IEnumerator<GetInventoryByUpcResult> productEnumerator = result.GetEnumerator();
            Product newItemInSynch = null;
            if (productEnumerator.MoveNext())
            {
                GetInventoryByUpcResult target = productEnumerator.Current;
                newItemInSynch = new Product()
                {
                    upc = target.upc,
                    name = target.name,
                    detail = target.detail,
                    quantity = (int)target.quantity,
                    location = target.location,
                    owner = synchBusinessId,
                    leadTime = (int)target.lead_time,
                    price = (double)target.default_price
                };
            }

            if (newItemInSynch != null)
            {
                createItemNameToItemMap();
                Intuit.Ipp.Data.Qbd.Item currentItem = itemNameToItemMap[newItemInSynch.name];
                Intuit.Ipp.Data.Qbd.InventoryTransfer inventoryTransfer = new Intuit.Ipp.Data.Qbd.InventoryTransfer();
                


                Intuit.Ipp.Data.Qbd.Item addedItem = commonService.Update(currentItem);
            }
        }

        private void createItemNameToItemMap()
        {
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);

            int pageNumber = 1;
            int chunkSize = 500;
            Intuit.Ipp.Data.Qbd.ItemQuery qbdItemQuery = new Intuit.Ipp.Data.Qbd.ItemQuery();
            qbdItemQuery.ItemElementName = Intuit.Ipp.Data.Qbd.ItemChoiceType4.StartPage;
            qbdItemQuery.Item = pageNumber.ToString();
            qbdItemQuery.ChunkSize = chunkSize.ToString();

            // use a while loop to page all the items from QBD
            int curItemCount = 1;
            while (curItemCount > 0)
            {
                qbdItemQuery.Item = pageNumber.ToString();
                IEnumerable<Intuit.Ipp.Data.Qbd.Item> items = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>
                                    (context) as IEnumerable<Intuit.Ipp.Data.Qbd.Item>;
                IEnumerator<Intuit.Ipp.Data.Qbd.Item> itemEnum = items.GetEnumerator();
                while (itemEnum.MoveNext())
                {
                    if ((itemEnum.Current.Type == Intuit.Ipp.Data.Qbd.ItemTypeEnum.Inventory
                        || itemEnum.Current.Type == Intuit.Ipp.Data.Qbd.ItemTypeEnum.Product)
                        && itemEnum.Current.Name != null)
                    {
                        itemNameToItemMap.Add(itemEnum.Current.Name, itemEnum.Current);
                    }

                }
                curItemCount = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>(context).Count;
                pageNumber++;
            }
        }

        #endregion
        
        #region Update Synch from QuickBooks Desktop

        public void updateInvoicesFromQBD() { }

        public void updateSalesOrdersFromQBD()
        {
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);

            // 2: get updated information from QBD side
            int pageNumber = 1;
            int chunkSize = 100;
            int totalItemCount = 0;
            Intuit.Ipp.Data.Qbd.SalesOrderQuery qbdSalesOrderQuery = new Intuit.Ipp.Data.Qbd.SalesOrderQuery();
            qbdSalesOrderQuery.ItemElementName = Intuit.Ipp.Data.Qbd.ItemChoiceType4.StartPage;
            qbdSalesOrderQuery.Item = pageNumber.ToString();
            qbdSalesOrderQuery.ChunkSize = chunkSize.ToString();
            //List<Intuit.Ipp.Data.Qbd.Customer> customers = (qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>
            //(context) as IEnumerable<Intuit.Ipp.Data.Qbd.Customer>).ToList();
            //IEnumerable<Intuit.Ipp.Data.Qbd.SalesOrderQuery> salesOrdersFromQBD = qbdSalesOrderQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.SalesOrderQuery>
            //(context) as IEnumerable<Intuit.Ipp.Data.Qbd.SalesOrderQuery>;
            int curItemCount = qbdSalesOrderQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>(context).Count;
        }

        public void updateItemsFromQBD()
        {
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);

            // we store upc, name, detail/description, location, quantity, lead time, default price
            // so we only need to update these information

            // 1: get current inventory list
            SynchDatabaseDataContext synchDatabaseContext = new SynchDatabaseDataContext();
            var results = synchDatabaseContext.GetAllInventory(synchBusinessId);
            Dictionary<string, Product> itemsFromSynch = new Dictionary<string, Product>();
            foreach (var result in results)
            {
                if (!itemsFromSynch.ContainsKey(result.name))
                {

                    itemsFromSynch.Add(result.name,
                        new Product()
                        {
                            name = result.name,
                            upc = result.upc,
                            detail = result.detail,
                            location = result.location,
                            quantity = (int)result.quantity,
                            leadTime = (int)result.lead_time,
                            price = (double)result.default_price
                        }
                    );
                }
                /*
                if (!itemsFromSynch.ContainsKey(result.name))
                {
                    
                    itemsFromSynch.Add(result.name,
                        new Product()
                        {
                            name = result.name,
                            upc = result.upc,
                            detail = result.detail,
                            location = result.location,
                            quantity = (int)result.quantity,
                            leadTime = (int)result.lead_time,
                            price = (double)result.default_price
                        }
                    );
                }*/
            }

            // 2: get updated information from QBD side
            int pageNumber = 1;
            int chunkSize = 100;
            int totalItemCount = 0;
            Intuit.Ipp.Data.Qbd.ItemQuery qbdItemQuery = new Intuit.Ipp.Data.Qbd.ItemQuery();
            qbdItemQuery.ItemElementName = Intuit.Ipp.Data.Qbd.ItemChoiceType4.StartPage;
            qbdItemQuery.Item = pageNumber.ToString();
            qbdItemQuery.ChunkSize = chunkSize.ToString();
            //List<Intuit.Ipp.Data.Qbd.Customer> customers = (qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>
            //(context) as IEnumerable<Intuit.Ipp.Data.Qbd.Customer>).ToList();
            IEnumerable<Intuit.Ipp.Data.Qbd.Item> itemsFromQBD = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>
            (context) as IEnumerable<Intuit.Ipp.Data.Qbd.Item>;
            int curItemCount = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>(context).Count;
            matchItemInformation(synchDatabaseContext, ref itemsFromSynch, itemsFromQBD);

            while (curItemCount > 0)
            {
                pageNumber++;
                qbdItemQuery.Item = pageNumber.ToString();
                itemsFromQBD = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>
                                     (context) as IEnumerable<Intuit.Ipp.Data.Qbd.Item>;
                matchItemInformation(synchDatabaseContext, ref itemsFromSynch, itemsFromQBD);
                totalItemCount += curItemCount;
                curItemCount = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>(context).Count;
            }


            // 3. After matching all the products from QBD, we delete excessive/inactive customers in Synch
            foreach (string bname in itemsFromSynch.Keys)
            {
                Product curProduct = itemsFromSynch[bname];
                // synchDatabaseContext.DeleteInventoryByUpc(curProduct.upc, synchBusinessId);
            }
        }

        /// <summary>
        /// Matches item information from Synch to item information from QBD.
        /// Create new product and inventory when it does not exist on Synch;
        /// modify product and inventory information when it does not match Synch's record;
        /// modify the reference passed as Synch's inventory
        /// </summary>
        /// <param name="synchDatabaseContext"></param>
        /// <param name="itemsFromSynch"></param>
        /// <param name="itemsFromQBD"></param>
        private void matchItemInformation(SynchDatabaseDataContext synchDatabaseContext, ref Dictionary<string, Product> itemsFromSynch, IEnumerable<Intuit.Ipp.Data.Qbd.Item> itemsFromQBD)
        {
            IEnumerator<Intuit.Ipp.Data.Qbd.Item> itemEnum = itemsFromQBD.GetEnumerator();
            while (itemEnum.MoveNext())
            {
                string nameFromQBD = itemEnum.Current.Name;
                // checks if this is a legitimate product we want to sync
                if (nameFromQBD != null && itemEnum.Current.Active && itemEnum.Current.Desc != null &&
                    (itemEnum.Current.Type == Intuit.Ipp.Data.Qbd.ItemTypeEnum.Product ||
                    itemEnum.Current.Type == Intuit.Ipp.Data.Qbd.ItemTypeEnum.Inventory))
                {   // this is a legitimate product we want to sync
                    // we will only sync
                    // 1. QtyOnHand
                    // 2. Description
                    // 3. sales price
                    // for now.

                    //itemEnum.Current.i

                    string detailFromQBD = itemEnum.Current.Desc;
                    double priceFromQBD = 0.99;         // default price
                    int quantityFromQBD = Int32.MinValue;       // initialized to be an impossible value
                    if (itemEnum.Current.QtyOnHandSpecified)
                        quantityFromQBD = Convert.ToInt32(itemEnum.Current.QtyOnHand);
                    Intuit.Ipp.Data.Qbd.Money costFromQBD = (Intuit.Ipp.Data.Qbd.Money)itemEnum.Current.Item1;
                    if (costFromQBD != null)
                        priceFromQBD = Convert.ToDouble(costFromQBD.Amount);


                    // now get current product from Synch or create new one
                    Product itemFromSynch = null;
                    if (!itemsFromSynch.TryGetValue(nameFromQBD, out itemFromSynch))
                    {
                        // this product does not exist in Synch, create one
                        // synchDatabaseContext.CreateProduct("aa", nameFromQBD, detailFromQBD);
                        // synchDatabaseContext.CreateInventory(synchBusinessId, "aa", "sample warehouse", quantityFromQBD,
                        //                                        7, priceFromQBD, 0);
                    }
                    else
                    {
                        // this product exists in Synch, update if needed
                        itemsFromSynch.Remove(nameFromQBD);
                        //createItemInQbd(itemFromSynch.upc);
                        //tempUpdateCurrentItemInQBD(itemFromSynch, itemEnum.Current);
                        if (detailFromQBD != itemFromSynch.detail ||
                            priceFromQBD != itemFromSynch.price ||
                            quantityFromQBD != itemFromSynch.quantity)
                        {
                            synchDatabaseContext.UpdateInventoryByUpc(itemFromSynch.upc, detailFromQBD, quantityFromQBD, priceFromQBD, synchBusinessId);

                        }
                    }
                }
            }
        }

        private void tempUpdateCurrentItemInQBD(Product itemInSynch, Intuit.Ipp.Data.Qbd.Item itemInQBD)
        {
            // validating
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);
            DataServices commonService = new DataServices(context);

            string[] separator = {"---"};
            string newName = itemInSynch.name.Split(separator, StringSplitOptions.None)[0];
            itemInQBD.Name = "testNewName002";
            itemInQBD.Desc = "test new description";
            Intuit.Ipp.Data.Qbd.Item addedItem = commonService.Update(itemInQBD);
        }

        public void updateBusinessesFromQBD()
        {
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);

            // we store business name, phone number, address, and email for our user's customers and vendors,
            // so we only need to update these information
            // WE IGNORE VENDORS for now

            // 1: get current customer list
            SynchDatabaseDataContext synchDatabaseContext = new SynchDatabaseDataContext();
            var results = synchDatabaseContext.GetAllCustomers(synchBusinessId);
            Dictionary<string, Business> customersFromSynch = new Dictionary<string, Business>();
            foreach (var result in results)
            {
                customersFromSynch.Add(result.name, 
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email,
                        phoneNumber = result.phone_number,
                        integration = (int)result.integration,
                        tier = (int)result.tier,
                        category = result.category

                    }
                );
            }

            // 2: get updated information from QBD side
            int pageNumber = 1;
            int chunkSize = 100;
            int totalCustomerCount = 0;
            Intuit.Ipp.Data.Qbd.CustomerQuery qbdCustomerQuery = new Intuit.Ipp.Data.Qbd.CustomerQuery();
            qbdCustomerQuery.ItemElementName = Intuit.Ipp.Data.Qbd.ItemChoiceType4.StartPage;
            qbdCustomerQuery.Item = pageNumber.ToString();
            qbdCustomerQuery.ChunkSize = chunkSize.ToString();
            //List<Intuit.Ipp.Data.Qbd.Customer> customers = (qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>
            //(context) as IEnumerable<Intuit.Ipp.Data.Qbd.Customer>).ToList();
            IEnumerable<Intuit.Ipp.Data.Qbd.Customer> customersFromQBD = qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>
            (context) as IEnumerable<Intuit.Ipp.Data.Qbd.Customer>;
            int curCustomerCount = qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>(context).Count;
            matchBusinessInformation(synchDatabaseContext, ref customersFromSynch, customersFromQBD);

            while (curCustomerCount > 0)
            {
                pageNumber++;
                qbdCustomerQuery.Item = pageNumber.ToString();

                /*
                customers.Concat(
                    (qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>
                                     (context) as IEnumerable<Intuit.Ipp.Data.Qbd.Customer>).ToList());
                */
                customersFromQBD = qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>
                                     (context) as IEnumerable<Intuit.Ipp.Data.Qbd.Customer>;
                matchBusinessInformation(synchDatabaseContext, ref customersFromSynch, customersFromQBD);
                totalCustomerCount += curCustomerCount;
                curCustomerCount = qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>(context).Count;
            }

            // 3. After matching all the customers from QBD, we delete excessive/inactive customers in Synch
            foreach (string bname in customersFromSynch.Keys)
            {
                Business curBusiness = customersFromSynch[bname];
                synchDatabaseContext.DeleteSupplies(synchBusinessId, curBusiness.id, synchBusinessId);
                synchDatabaseContext.DeleteBusinessById(curBusiness.id);
            }
        }

        // check each one of the customers to see if any updates exist
        private void matchBusinessInformation(SynchDatabaseDataContext context, ref Dictionary<string, Business> businessesFromSynch, IEnumerable<Intuit.Ipp.Data.Qbd.Customer> businessesFromQBD)
        {
            IEnumerator<Intuit.Ipp.Data.Qbd.Customer> businessEnum = businessesFromQBD.GetEnumerator();
            while (businessEnum.MoveNext())
            {
                string nameFromQBD = businessEnum.Current.Name;
                if (nameFromQBD != null)
                {
                    string addressFromQBD = "";
                    string zipFromQBD = "98105";
                    if (businessEnum.Current.Address == null)
                    {
                        addressFromQBD = "empty_address";
                    }
                    else
                    {
                        zipFromQBD = (businessEnum.Current.Address[0].PostalCode == null) ? "98105" :
                                        businessEnum.Current.Address[0].PostalCode.Split('-')[0];

                        if (businessEnum.Current.Address[0].Line1 == null)
                            addressFromQBD += businessEnum.Current.Address[0].City + ", " + businessEnum.Current.Address[0].CountrySubDivisionCode;
                        else
                        {
                            addressFromQBD += businessEnum.Current.Address[0].Line1 + ", ";
                            if (businessEnum.Current.Address[0].Line2 == null)
                                addressFromQBD += businessEnum.Current.Address[0].City + ", " + businessEnum.Current.Address[0].CountrySubDivisionCode;
                            else
                                addressFromQBD += businessEnum.Current.Address[0].Line2
                                                    + ", " + businessEnum.Current.Address[0].City + ", "
                                                    + businessEnum.Current.Address[0].CountrySubDivisionCode;

                        }
                    }

                    int intZipFromQBD = -1;
                    if (!Int32.TryParse(zipFromQBD, out intZipFromQBD))
                        intZipFromQBD = 98105;
                    string emailFromQBD = (businessEnum.Current.Email == null) ? "changhao.han@gmail.com" : businessEnum.Current.Email[0].Address;
                    string categoryFromQBD = (businessEnum.Current.Category == null) ? "empty_category" : businessEnum.Current.Category;
                    string phoneNumFromQBD = (businessEnum.Current.Phone == null) ? "206-407-9494" : businessEnum.Current.Phone[0].FreeFormNumber;

                    // compare information now
                    Business currentBusinessFromSynch = null;
                    if (!businessesFromSynch.TryGetValue(nameFromQBD, out currentBusinessFromSynch))
                    {
                        // this business does not exist, create new one in Synch
                        int newCustomerId = context.CreateBusiness(nameFromQBD, addressFromQBD, intZipFromQBD, emailFromQBD, categoryFromQBD, 0, 0, phoneNumFromQBD);
                        context.CreateSupplies(synchBusinessId, newCustomerId, synchBusinessId);
                    }
                    else
                    {
                        businessesFromSynch.Remove(nameFromQBD);
                        if (addressFromQBD != currentBusinessFromSynch.address
                            || intZipFromQBD != currentBusinessFromSynch.zip
                            || emailFromQBD != currentBusinessFromSynch.email
                            || phoneNumFromQBD != currentBusinessFromSynch.phoneNumber)
                        {
                            // update new info into Synch's database
                            context.UpdateBusinessById(currentBusinessFromSynch.id, addressFromQBD, intZipFromQBD, emailFromQBD, categoryFromQBD, phoneNumFromQBD);

                        }
                    }
                }
            }   // end while loop
        }

        #endregion

    }
}
