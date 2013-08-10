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

        Dictionary<String, String> itemNameToIdMap;

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
            this.dataSourcetype = dataSourcetype;

            itemNameToIdMap = new Dictionary<string, string>();
        }

        public void updateInvoiceFromQBD()
        {

        }

        public void createInvoiceInQBD(int rid)
        {
            // validating
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);
            DataServices commonService = new DataServices(context);

            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var results = synchDataContext.GetOrderDetails(synchBusinessId, rid);

            List<RecordProduct> products = new List<RecordProduct>();

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
                        note = product.item_note
                    });
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
            invoiceHeader.Balance = (decimal)100.00;
            invoiceHeader.BillAddr = physicalAddress;
            invoiceHeader.BillEmail = customerEmail;
            invoiceHeader.DocNumber = rid.ToString();
            invoiceHeader.DueDate = DateTime.Now;
            invoiceHeader.ShipAddr = physicalAddress;
            invoiceHeader.ShipDate = DateTime.Now;
            invoiceHeader.TaxAmt = (decimal)5;
            invoiceHeader.TaxRate = (decimal).05;
            invoiceHeader.ToBeEmailed = false;
            invoiceHeader.TotalAmt = (decimal)105.00;

            List<Intuit.Ipp.Data.Qbd.InvoiceLine> listLine = new List<Intuit.Ipp.Data.Qbd.InvoiceLine>();

            createItemNameToItemIdMap();
            // add all the items in the record into inovice lines
            foreach (RecordProduct curProduct in products)
            {
                // get item id by querying QBD
                string itemIdString = "-1";
                if (itemNameToIdMap.ContainsKey(curProduct.name))
                    itemIdString = itemNameToIdMap[curProduct.name];

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

        private void createItemNameToItemIdMap()
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
                    if (itemEnum.Current.Name != null)
                    {
                        itemNameToIdMap.Add(itemEnum.Current.Name, itemEnum.Current.Id.Value);
                    }

                }
                curItemCount = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>(context).Count;
                pageNumber++;
            }
        }
    }
}
