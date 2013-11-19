using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

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


using ERPIntegrationWorkerRole.QuickBookIntegration;
using ERPIntegrationWorkerRole.SynchIntegration;
using ERPIntegrationWorkerRole.Utilities;

namespace ERPIntegrationWorkerRole.DataflowLogic
{
    class DataIntegratorQBD
    {
        // QBD side of fields
        QBDDataReader qbdDataReader;
        QBDDataUpdater qbdDataUpdater;

        DateTime transactionStartDateFilter;
        Dictionary<string, Utilities.ERPRecordMapEntity> transactionIdToEntityMap;

        // Synch side of fields
        int synchBusinessId;
        int autoUpcCounter;
        string autoUpcPrefix;

        SynchDatabaseReader synchDatabaseReader;
        SynchDatabaseUpdater synchDatabaseUpdater;

        SynchStorageReader synchStorageReader;
        SynchStorageUpdater synchStorageUpdater;

        public DataIntegratorQBD(int bid)
        {
            this.synchBusinessId = bid;
            this.synchDatabaseReader = new SynchDatabaseReader(bid);
            this.synchDatabaseUpdater = new SynchDatabaseUpdater(bid);

            this.synchStorageReader = new SynchStorageReader(bid);
            this.synchStorageUpdater = new SynchStorageUpdater(bid);

            this.qbdDataReader = new QBDDataReader(synchStorageReader.getCredentialFromSynchServer());
            this.qbdDataUpdater = new QBDDataUpdater(synchStorageReader.getCredentialFromSynchServer());

            this.autoUpcPrefix = bid + "AUTO";

            transactionIdToEntityMap = synchStorageReader.getTransactionIdToEntityMap(ApplicationConstants.ERP_QBD_TABLE_RECORD);
            if (transactionIdToEntityMap.Count == 0)
                transactionStartDateFilter = new DateTime(2013, 9, 1);
            else
                transactionStartDateFilter = DateTime.Now.AddDays(-7);

        }

        #region Update QuickBooks Desktop from Synch

        public void createInvoiceInQBD(int rid)
        {
            // get invoice information from Synch database
            SynchRecord recordFromSynch = synchDatabaseReader.getCompleteOrder(rid);
            if (recordFromSynch.products == null)
                return;

            SynchBusiness customerInSynch = synchDatabaseReader.getBusiness(recordFromSynch.products[0].customer);

            if (recordFromSynch == null)
                return;
            if (customerInSynch == null)
                return;

            Dictionary<string, string> upcToItemIdMap = synchStorageReader.getUpcToItemIdMap(ApplicationConstants.ERP_QBD_TABLE_PRODUCT);
            Dictionary<int, string> synchCidToCustomerIdMap = synchStorageReader.getSynchCidToCustomerIdMap(ApplicationConstants.ERP_QBD_TABLE_BUSINESS);

            Intuit.Ipp.Data.Qbd.Invoice newInvoice = qbdDataUpdater.createInvoice(recordFromSynch, customerInSynch, upcToItemIdMap, synchCidToCustomerIdMap);

            // create a mapping for this invoice in storage so that we won't unnecessarily sync it back
            synchStorageUpdater.createRecordMapping(ApplicationConstants.ERP_QBD_TABLE_RECORD, rid, newInvoice.Id.Value);
        }

        public void createSalesOrderInQBD(int rid)
        {
            // get invoice information from Synch database
            SynchRecord recordFromSynch = synchDatabaseReader.getCompleteOrder(rid);
            if (recordFromSynch.products == null)
                return;
            SynchBusiness customerInSynch = synchDatabaseReader.getBusiness(recordFromSynch.products[0].customer);

            if (recordFromSynch == null)
                return;
            if (customerInSynch == null)
                return;

            Dictionary<string, string> upcToItemIdMap = synchStorageReader.getUpcToItemIdMap(ApplicationConstants.ERP_QBD_TABLE_PRODUCT);
            Dictionary<int, string> synchCidToCustomerIdMap = synchStorageReader.getSynchCidToCustomerIdMap(ApplicationConstants.ERP_QBD_TABLE_BUSINESS);

            Intuit.Ipp.Data.Qbd.SalesOrder newSalesOrder = qbdDataUpdater.createSalesOrder(recordFromSynch, customerInSynch, upcToItemIdMap, synchCidToCustomerIdMap);


            // create a mapping for this invoice in storage so that we won't unnecessarily sync it back
            synchStorageUpdater.createRecordMapping(ApplicationConstants.ERP_QBD_TABLE_RECORD, rid, newSalesOrder.Id.Value);
        }

        public void createBusinessInQBD(int otherBid, bool isCustomer)
        {
            /*
            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetBusiness(otherBid);
            IEnumerator<GetBusinessResult> businessEnumerator = result.GetEnumerator();
            SynchBusiness businessInSynch = null;
            if (businessEnumerator.MoveNext())
            {
                GetBusinessResult retrievedBusiness = businessEnumerator.Current;

                businessInSynch = new SynchBusiness()
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
             */
        }

        public void createItemInQbd(string upc)
        {
            /*
            // get invoice information from Synch database
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetInventoryByUpc(synchBusinessId, upc);
            IEnumerator<GetInventoryByUpcResult> productEnumerator = result.GetEnumerator();
            SynchProduct newItemInSynch = null;
            if (productEnumerator.MoveNext())
            {
                GetInventoryByUpcResult target = productEnumerator.Current;
                newItemInSynch = new SynchProduct()
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
             */
        }

        public void updateItemInQbd(string upc)
        {
            /*
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var result = synchDataContext.GetInventoryByUpc(synchBusinessId, upc);
            IEnumerator<GetInventoryByUpcResult> productEnumerator = result.GetEnumerator();
            SynchProduct itemInSynch = null;
            if (productEnumerator.MoveNext())
            {
                GetInventoryByUpcResult target = productEnumerator.Current;
                itemInSynch = new SynchProduct()
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

            if (itemInSynch != null)
            {
                createItemNameToItemMap();
                Intuit.Ipp.Data.Qbd.Item currentItem = itemNameToItemMap[itemInSynch.name];

            }*/
        }

        #endregion

        #region Update Synch from QuickBooks Desktop

        
        public void updateInvoicesFromQBD()
        {
            // get product mapping information from QBD
            Dictionary<string, string> itemIdToUpcMap = synchStorageReader.getItemIdToUpcMap(ApplicationConstants.ERP_QBD_TABLE_PRODUCT);
            Dictionary<string, int> customerIdToSynchCidMap = synchStorageReader.getCustomerIdToSynchCidMap(ApplicationConstants.ERP_QBD_TABLE_BUSINESS);

            if (transactionStartDateFilter == null)
                transactionStartDateFilter = new DateTime(2013, 1, 1);

            List<Intuit.Ipp.Data.Qbd.Invoice> invoicesFromQBD = qbdDataReader.getInvoicesFromDate(transactionStartDateFilter);

            int successCount = 0;
            int noCustomerCount = 0;
            int missingInfoCount = 0;
            int noUpcCount = 0;

            for (int invoiceIndex = 0; invoiceIndex < invoicesFromQBD.Count; invoiceIndex++)
            {
                Intuit.Ipp.Data.Qbd.Invoice curInvoice = invoicesFromQBD[invoiceIndex];

                if (transactionIdToEntityMap.ContainsKey(curInvoice.Id.Value))
                {
                    // this invoice exists;
                    // check if updates needed.
                }
                else
                {
                    string customerIdFromQBD = curInvoice.Header.CustomerId.Value;
                    string invoiceTitle = "Invoiced: " + curInvoice.Header.CustomerName;
                    string invoiceComment = "Invoiced From QuickBooks: " + curInvoice.Header.Msg;
                    string transactionDateString = String.Empty;
                    if (curInvoice.Header.TxnDateSpecified)
                    {
                        transactionDateString = curInvoice.Header.TxnDate.Year.ToString();
                        if (curInvoice.Header.TxnDate.Month > 9)
                            transactionDateString += curInvoice.Header.TxnDate.Month.ToString();
                        else
                            transactionDateString += "0" + curInvoice.Header.TxnDate.Month.ToString();

                        if (curInvoice.Header.TxnDate.Day > 9)
                            transactionDateString += curInvoice.Header.TxnDate.Day.ToString();
                        else
                            transactionDateString += "0" + curInvoice.Header.TxnDate.Day.ToString();

                        transactionDateString += "000000";
                    }
                    long transactionDateLong = (transactionDateString == String.Empty) ? 20111231000000 : long.Parse(transactionDateString);

                    if (customerIdToSynchCidMap.ContainsKey(customerIdFromQBD))
                    {
                        // this customer exists
                        int customerIdFromSynch = customerIdToSynchCidMap[customerIdFromQBD];

                        List<string> upcList = new List<string>();
                        List<int> quantityList = new List<int>();
                        List<double> priceList = new List<double>();
                        foreach (Intuit.Ipp.Data.Qbd.InvoiceLine curLine in curInvoice.Line)
                        {
                            string upc = null;
                            int quantity = 0;
                            double price = 0.0;
                            if (curLine.ItemsElementName != null && curLine.Items != null)
                            {
                                for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                                {
                                    if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                    {
                                        string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                        if (itemIdToUpcMap.ContainsKey(itemId))
                                            upc = itemIdToUpcMap[itemId];
                                        else
                                            noUpcCount++;
                                    }
                                    if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                        price = Double.Parse(curLine.Items[i].ToString());

                                    if (curLine.ItemsElementName[i].ToString() == "Qty")
                                        quantity = Int32.Parse(curLine.Items[i].ToString());
                                }

                                if (upc != null && quantity != 0 && price != 0.0)
                                {
                                    // now create this line item in database
                                    upcList.Add(upc);
                                    quantityList.Add(quantity);
                                    priceList.Add(price);
                                }
                                else
                                {
                                    missingInfoCount++;
                                }
                            }   // if item information exists
                            else
                            {
                                missingInfoCount++;
                            }
                        }   // end foreach line item

                        if (upcList.Count > 0)
                        {
                            int rid = synchDatabaseUpdater.createNewRecord(invoiceTitle, 0, (int)RecordStatus.closed, invoiceComment, 42, transactionDateLong, customerIdFromSynch,
                                                                                upcList, quantityList, priceList);
                            if (rid > 0)
                            {
                                synchStorageUpdater.createRecordMapping(ApplicationConstants.ERP_QBD_TABLE_RECORD, rid, curInvoice.Id.Value);
                            }
                            successCount++;

                        }
                    }   // if this customer exists
                    else
                        noCustomerCount++;

                }   // end new invoice
            }

        }

        public void updateSalesOrdersFromQBD()
        {
            // get product mapping information from QBD
            Dictionary<string, string> itemIdToUpcMap = synchStorageReader.getItemIdToUpcMap(ApplicationConstants.ERP_QBD_TABLE_PRODUCT);
            Dictionary<string, int> customerIdToSynchCidMap = synchStorageReader.getCustomerIdToSynchCidMap(ApplicationConstants.ERP_QBD_TABLE_BUSINESS);

            if (transactionStartDateFilter == null)
                transactionStartDateFilter = new DateTime(2013, 1, 1);
            
            List<Intuit.Ipp.Data.Qbd.SalesOrder> salesOrdersFromQBD = qbdDataReader.getSalesOrdersFromDate(transactionStartDateFilter);

            int successCount = 0;
            int noCustomerCount = 0;
            int missingInfoCount = 0;
            int noUpcCount = 0;

            for (int salesOrderIndex = 0; salesOrderIndex < salesOrdersFromQBD.Count; salesOrderIndex++)
            {
                Intuit.Ipp.Data.Qbd.SalesOrder curSalesOrder = salesOrdersFromQBD[salesOrderIndex];

                if (transactionIdToEntityMap.ContainsKey(curSalesOrder.Id.Value))
                {
                    // this salesOrder exists;
                    // check if updates needed.
                }
                else
                {
                    string customerIdFromQBD = curSalesOrder.Header.CustomerId.Value;
                    string salesOrderTitle = "From QuickBooks: " + curSalesOrder.Header.CustomerName;
                    string salesOrderComment = "From QuickBooks: " + curSalesOrder.Header.Msg;
                    string transactionDateString = String.Empty;
                    if (curSalesOrder.Header.TxnDateSpecified)
                    {
                        transactionDateString = curSalesOrder.Header.TxnDate.Year.ToString();
                        if (curSalesOrder.Header.TxnDate.Month > 9)
                            transactionDateString += curSalesOrder.Header.TxnDate.Month.ToString();
                        else
                            transactionDateString += "0" + curSalesOrder.Header.TxnDate.Month.ToString();

                        if (curSalesOrder.Header.TxnDate.Day > 9)
                            transactionDateString += curSalesOrder.Header.TxnDate.Day.ToString();
                        else
                            transactionDateString += "0" + curSalesOrder.Header.TxnDate.Day.ToString();

                        transactionDateString += "000000";
                    }
                    long transactionDateLong = (transactionDateString == String.Empty) ? 20111231000000 : long.Parse(transactionDateString);

                    if (customerIdToSynchCidMap.ContainsKey(customerIdFromQBD))
                    {
                        // this customer exists
                        int customerIdFromSynch = customerIdToSynchCidMap[customerIdFromQBD];

                        List<string> upcList = new List<string>();
                        List<int> quantityList = new List<int>();
                        List<double> priceList = new List<double>();
                        foreach (Intuit.Ipp.Data.Qbd.SalesOrderLine curLine in curSalesOrder.Line)
                        {
                            string upc = null;
                            int quantity = 0;
                            double price = 0.0;
                            if (curLine.ItemsElementName != null && curLine.Items != null)
                            {
                                for (int i = 0; i < curLine.ItemsElementName.Length; i++)
                                {
                                    if (curLine.ItemsElementName[i].ToString() == "ItemId")
                                    {
                                        string itemId = ((Intuit.Ipp.Data.Qbd.IdType)curLine.Items[i]).Value;
                                        if (itemIdToUpcMap.ContainsKey(itemId))
                                            upc = itemIdToUpcMap[itemId];
                                        else
                                            noUpcCount++;
                                    }
                                    if (curLine.ItemsElementName[i].ToString() == "UnitPrice")
                                        price = Double.Parse(curLine.Items[i].ToString());

                                    if (curLine.ItemsElementName[i].ToString() == "Qty")
                                        quantity = Int32.Parse(curLine.Items[i].ToString());
                                }

                                if (upc != null && quantity != 0 && price != 0.0)
                                {
                                    // now create this line item in database
                                    //context.CreateProductInRecord(rid, upc, synchBusinessId, customerIdFromSynch, quantity, note, price);
                                    upcList.Add(upc);
                                    quantityList.Add(quantity);
                                    priceList.Add(price);
                                }
                                else
                                {
                                    missingInfoCount++;
                                }
                            }   // if item information exists
                            else
                            {
                                missingInfoCount++;
                            }
                        }   // end foreach line item

                        if (upcList.Count > 0)
                        {
                            int rid = synchDatabaseUpdater.createNewRecord(salesOrderTitle, 0, (int)RecordStatus.sent, salesOrderComment, 42, transactionDateLong, customerIdFromSynch,
                                                                                upcList, quantityList, priceList);
                            if (rid > 0)
                            {
                                synchStorageUpdater.createRecordMapping(ApplicationConstants.ERP_QBD_TABLE_RECORD, rid, curSalesOrder.Id.Value);
                            }
                            successCount++;
                        }
                    }   // if this customer exists
                    else
                        noCustomerCount++;

                }   // end new salesOrder
            }
        }

        
        public void updateItemsFromQBD()
        {
            // 1: get current inventory list
            Dictionary<string, SynchProduct> upcToInventoryMap = synchDatabaseReader.getUpcToInventoryMap();
            getAutoUpcCounter(upcToInventoryMap.Keys);
            Dictionary<string, string> itemIdToUpcMap = synchStorageReader.getItemIdToUpcMap(ApplicationConstants.ERP_QBD_TABLE_PRODUCT);

            // 2: get updated information from QBD side
            List<Intuit.Ipp.Data.Qbd.Item> itemsFromQBD = qbdDataReader.getItems();

            // logic of matching item information
            for (int i = 0; i < itemsFromQBD.Count(); i++)
            {
                Intuit.Ipp.Data.Qbd.Item curItem = itemsFromQBD[i];

                // checks if this is a legitimate product we want to sync
                if (String.IsNullOrEmpty(curItem.Name))
                    continue;
                if (!curItem.Active)
                    continue;
                if (String.IsNullOrEmpty(curItem.Desc))
                    continue;
                if (curItem.Type != Intuit.Ipp.Data.Qbd.ItemTypeEnum.Product && curItem.Type != Intuit.Ipp.Data.Qbd.ItemTypeEnum.Inventory)
                    continue;
                if (!curItem.QtyOnHandSpecified)
                    continue;

                string nameFromQBD = curItem.Name;
                string itemId = curItem.Id.Value;
                string detailFromQBD = curItem.Desc;
                double priceFromQBD = 0.99;         // default price
                int quantityFromQBD = Convert.ToInt32(curItem.QtyOnHand);

                // takes into account the quantity on sales order, which includes
                // orders generated from Synch as well as orders generated from QuickBooks directly
                if (curItem.QtyOnSalesOrderSpecified)
                    quantityFromQBD -= Convert.ToInt32(curItem.QtyOnSalesOrder);

                Intuit.Ipp.Data.Qbd.Money costFromQBD = (Intuit.Ipp.Data.Qbd.Money)curItem.Item1;
                if (costFromQBD != null)
                    priceFromQBD = Convert.ToDouble(costFromQBD.Amount);

                // now get current product linking information from Table Storage mapping,
                // or create a new mapping if no mapping exists.
                if (!itemIdToUpcMap.ContainsKey(itemId))
                {
                    string upc = null;
                    upc = matchNameAndDetailWithInventory(nameFromQBD, detailFromQBD, upcToInventoryMap.Values);

                    if (upc == null)
                    {
                        // when no mapping exist and no product with same name/detail exist in our database,
                        // we create new one for them
                        autoUpcCounter++;
                        string autoUpc = autoUpcPrefix + autoUpcCounter;
                        synchDatabaseUpdater.createNewInventory(autoUpc, nameFromQBD, detailFromQBD, "temporary location", quantityFromQBD, 7, priceFromQBD, 0);
                        synchStorageUpdater.createProductMapping(ApplicationConstants.ERP_QBD_TABLE_PRODUCT, autoUpc, itemId);
                    }
                    else
                    {
                        // when we have the same product with missing/incorrect mapping information in storage
                        upcToInventoryMap.Remove(upc);
                        synchStorageUpdater.createProductMapping(ApplicationConstants.ERP_QBD_TABLE_PRODUCT, upc, itemId);
                    }
                }
                else
                {
                    string upc = itemIdToUpcMap[itemId];
                    itemIdToUpcMap.Remove(itemId);

                    if (upcToInventoryMap.ContainsKey(upc))
                    {
                        // this product with correct upc exists in Synch, update if needed
                        SynchProduct itemFromSynch = upcToInventoryMap[upc];
                        upcToInventoryMap.Remove(upc);
                        if (detailFromQBD != itemFromSynch.detail ||
                            priceFromQBD != itemFromSynch.price ||
                            quantityFromQBD != itemFromSynch.quantity ||
                            nameFromQBD != itemFromSynch.name)
                        {
                            synchDatabaseUpdater.updateInventory(itemFromSynch.upc, detailFromQBD, quantityFromQBD, priceFromQBD, synchBusinessId, nameFromQBD);

                        }
                    }
                    else
                    {
                        // this upc does not exist in Synch, create new one
                        synchDatabaseUpdater.createNewInventory(upc, nameFromQBD, detailFromQBD, "Unassigned", quantityFromQBD, 7, priceFromQBD, 0);
                    }
                }
            }

            // 3. After matching all the products from QBD, we delete excessive/inactive products in Synch
            foreach (string upc in upcToInventoryMap.Keys)
                synchDatabaseUpdater.deleteInventory(upc);

            foreach (string itemId in itemIdToUpcMap.Keys)
                synchStorageUpdater.deleteProductMapping(ApplicationConstants.ERP_QBD_TABLE_PRODUCT, itemId);
        }

        private void getAutoUpcCounter(Dictionary<string, SynchProduct>.KeyCollection keyCollection)
        {
            string[] upcs = keyCollection.ToArray<string>();
            int maxCurrentCount = 0;
            foreach (string upc in upcs)
            {
                if (upc.StartsWith(autoUpcPrefix))
                {
                    int curCount = 0;
                    Int32.TryParse(upc.Split('O')[1], out curCount);
                    if (curCount > maxCurrentCount)
                        maxCurrentCount = curCount;
                }
            }

            autoUpcCounter = maxCurrentCount;
        }

        public void updateCustomersFromQBD()
        {
            // 1: get current customer list
            Dictionary<int, SynchBusiness> bidToSynchBusinessMap = synchDatabaseReader.getBidToCustomerMap();

            // 2: get information from table storage
            Dictionary<string, int> customerIdToSynchBidMap = synchStorageReader.getCustomerIdToSynchCidMap(ApplicationConstants.ERP_QBD_TABLE_BUSINESS); 

            // 3: get updated information from QBD side
            List<Intuit.Ipp.Data.Qbd.Customer> customersFromQBD = qbdDataReader.getCustomers();

            // logic of matching customer info
            for (int i = 0; i < customersFromQBD.Count(); i++)
            {
                Intuit.Ipp.Data.Qbd.Customer curCustomer = customersFromQBD[i];

                if (String.IsNullOrEmpty(curCustomer.Name))
                    continue;

                string nameFromQBD = curCustomer.Name;
                string idFromQBD = curCustomer.Id.Value;

                string addressFromQBD = "empty address";
                string zipFromQBD = "98105";
                if (curCustomer.Address != null)
                {
                    zipFromQBD = (curCustomer.Address[0].PostalCode == null) ? "98105" :
                                    curCustomer.Address[0].PostalCode.Split('-')[0];

                    if (curCustomer.Address[0].Line1 == null)
                        addressFromQBD = curCustomer.Address[0].City + ", " + curCustomer.Address[0].CountrySubDivisionCode;
                    else
                    {
                        addressFromQBD = curCustomer.Address[0].Line1 + ", ";
                        if (curCustomer.Address[0].Line2 == null)
                            addressFromQBD += curCustomer.Address[0].City + ", " + curCustomer.Address[0].CountrySubDivisionCode;
                        else
                            addressFromQBD += curCustomer.Address[0].Line2
                                                + ", " + curCustomer.Address[0].City + ", "
                                                + curCustomer.Address[0].CountrySubDivisionCode;

                    }
                }

                int intZipFromQBD = -1;
                if (!Int32.TryParse(zipFromQBD, out intZipFromQBD))
                    intZipFromQBD = 98105;
                string emailFromQBD = (curCustomer.Email == null) ? "changhao.han@gmail.com" : curCustomer.Email[0].Address;
                string categoryFromQBD = (curCustomer.Category == null) ? "empty_category" : curCustomer.Category;
                string phoneNumFromQBD = (curCustomer.Phone == null) ? "206-407-9494" : curCustomer.Phone[0].FreeFormNumber;

                // compare information now
                // 1. try to get cid from table storage mapping
                if (!customerIdToSynchBidMap.ContainsKey(idFromQBD))
                {
                    // not in table storage right now, and considered not in Synch database
                    // create new business in Synch and a new mapping
                    int newCustomerId = synchDatabaseUpdater.createCustomer(nameFromQBD, addressFromQBD, intZipFromQBD, emailFromQBD, categoryFromQBD, 0, 0, phoneNumFromQBD);
                    if (newCustomerId != -1)
                        synchStorageUpdater.createBusinessMapping(ApplicationConstants.ERP_QBD_TABLE_BUSINESS, newCustomerId, idFromQBD);
                }
                else
                {
                    // in table storage; get business info from Synch
                    int idFromSynch = customerIdToSynchBidMap[idFromQBD];
                    SynchBusiness currentBusinessFromSynch = null;

                    if (!bidToSynchBusinessMap.ContainsKey(idFromSynch))
                    {
                        // business mapping exists, but business id in Synch is outdated;
                        // create new business in Synch and a new mapping; later on delete outdated ones
                        int newCustomerId = synchDatabaseUpdater.createCustomer(nameFromQBD, addressFromQBD, intZipFromQBD, emailFromQBD, categoryFromQBD, 0, 0, phoneNumFromQBD);
                        if (newCustomerId != -1)
                            synchStorageUpdater.createBusinessMapping(ApplicationConstants.ERP_QBD_TABLE_BUSINESS, newCustomerId, idFromQBD);
                    }
                    else
                    {
                        // business mapping exist and business id is update;
                        // check and update business information
                        currentBusinessFromSynch = bidToSynchBusinessMap[idFromSynch];
                        bidToSynchBusinessMap.Remove(idFromSynch);
                        customerIdToSynchBidMap.Remove(idFromQBD);

                        if (addressFromQBD != currentBusinessFromSynch.address
                            || intZipFromQBD != currentBusinessFromSynch.zip
                            || emailFromQBD != currentBusinessFromSynch.email
                            || phoneNumFromQBD != currentBusinessFromSynch.phoneNumber)
                        {
                            // update new info into Synch's database
                            synchDatabaseUpdater.updateBusinessById(currentBusinessFromSynch.id, addressFromQBD, intZipFromQBD, emailFromQBD, categoryFromQBD, phoneNumFromQBD);
                        }
                    }
                }   // end if mapping in storage

            }

            // 3. After matching all the customers from QBD, we delete excessive/inactive customers in Synch
            foreach (int cid in bidToSynchBusinessMap.Keys)
            {
                synchDatabaseUpdater.deleteCustomer(cid);
            }

            foreach (string customerId in customerIdToSynchBidMap.Keys)
            {
                synchStorageUpdater.deleteBusinessMapping(ApplicationConstants.ERP_QBD_TABLE_BUSINESS, customerId);
            }            
        }
        #endregion


        #region private helper methods that are not always used in the service

        private string matchNameAndDetailWithInventory(string nameFromQBD, string detailFromQBD, Dictionary<string, SynchProduct>.ValueCollection synchProducts)
        {
            foreach (SynchProduct p in synchProducts)
            {
                if (nameFromQBD == p.name || detailFromQBD == p.detail)
                    return p.upc;
            }

            return null;
        }

        #endregion

    }
}
