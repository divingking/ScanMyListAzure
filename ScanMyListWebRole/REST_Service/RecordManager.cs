using SynchWebRole.Library_Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web.Script.Serialization;

namespace SynchWebRole.REST_Service
{
    public partial class SynchDataService : IRecordManager
    {
        
        public List<Record> GetOrders(int bid, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetOrders(bid);

            List<Record> orders = new List<Record>();
            Record current = null;
            foreach (var result in results)
            {
                if (current == null)
                {
                    current = new Record()
                    {
                        id = result.record_id,
                        title = result.record_title,
                        date = (long)result.record_date,
                        products = new List<RecordProduct>()
                    };
                }
                else if (current.id != result.record_id)
                {
                    orders.Add(current);
                    current = new Record()
                    {
                        id = result.record_id,
                        title = result.record_title,
                        date = (long)result.record_date,
                        products = new List<RecordProduct>()
                    };
                }
                current.products.Add(
                    new RecordProduct()
                    {
                        upc = result.product_upc,
                        name = result.product_name,
                        quantity = (int)result.product_quantity,
                        customer = result.customer_id,
                        note = result.item_note
                        //price = (double)result.product_price
                    });
            }

            orders.Add(current);

            return orders;
        }

        public List<Record> SearchRecordByTitle(int bid, int aid, string sessionId, string query)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            query = "%" + query + "%";
            var results = context.SearchRecordByTitle(bid, query);

            List<Record> records = new List<Record>();

            foreach (SearchRecordByTitleResult record in results)
            {
                records.Add(
                    new Record()
                    {
                        id = record.id,
                        title = record.title,
                        date = (long)record.date,
                        business = (int)record.business,
                        category = (int)record.category,
                        status = (int)record.status
                    });
            }

            return records;
        }

        public List<Record> PageRecord(int bid, int aid, string sessionId, int pageSize, int offset)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.PageRecordForBusiness(bid, pageSize, offset);

            List<Record> records = new List<Record>();
            foreach (PageRecordForBusinessResult record in results)
            {
                records.Add(
                    new Record()
                    {
                        id = record.id,
                        title = record.title,
                        date = (long)record.date,
                        business = (int)record.business,
                        category = (int)record.category,
                        status = (int)record.status
                    });
            }

            return records;
        }

        public List<Record> GetReceipts(int bid, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetReceipts(bid);

            List<Record> receipts = new List<Record>();
            Record current = null;
            foreach (var result in results)
            {
                if (current == null)
                {
                    current = new Record()
                    {
                        id = result.record_id,
                        title = result.record_title,
                        date = (long)result.record_date,
                        products = new List<RecordProduct>()
                    };
                }
                else if (current.id != result.record_id)
                {
                    receipts.Add(current);
                    current = new Record()
                    {
                        id = result.record_id,
                        title = result.record_title,
                        date = (long)result.record_date,
                        products = new List<RecordProduct>()
                    };
                }
                current.products.Add(
                    new RecordProduct()
                    {
                        upc = result.product_upc,
                        name = result.product_name,
                        quantity = (int)result.product_quantity,
                        supplier = result.supplier_id,
                        price = (double)result.product_price,
                        note = result.item_note
                    });
            }

            receipts.Add(current);

            return receipts;
        }

        public List<Record> GetRecordsByDate(int bid, long start, long end, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetRecordsByTimeRange(bid, start, end);

            List<Record> records = new List<Record>();

            foreach (GetRecordsByTimeRangeResult record in results)
            {
                records.Add(
                    new Record()
                    {
                        id = record.id,
                        title = record.title,
                        date = (long)record.date,
                        business = (int)record.business,
                        category = (int)record.category,
                        status = (int)record.status
                    });
            }

            return records;
        }

        public List<Order> GetOrdersByRange(int bid, int start, int end, int aid, string sessionId)
        {
            /*if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetOrdersByRange(bid, start, end);
                List<Order> orders = new List<Order>();
                foreach (var result in results)
                {
                    orders.Add(new Order()
                    {
                        bid = bid,
                        oid = result.id,
                        title = result.title,
                        date = (long)result.date,
                        scanIn = (bool)result.scan_in,
                        sent = (bool)result.sent,
                        products = null
                    });
                }
                return orders;
            }
            else
            {
                return null;
            }*/

            throw new NotImplementedException();
        }

        public List<Record> GetNRecordsFromLast(int bid, int last, int n, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetNRecordsFromLast(bid, last, n);

            List<Record> records = new List<Record>();

            foreach (var result in results)
            {
                records.Add(
                    new Record()
                    {
                        id = result.id,
                        title = result.title,
                        date = (long)result.date,
                        business = (int)result.business,
                        category = (int)result.category,
                        status = (int)result.status
                    });
            }

            return records;
        }

        public List<RecordProduct> GetRecordDetails(int bid, int rid, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var record = context.GetRecord(bid, rid);
            IEnumerator<GetRecordResult> recordEnumerator = record.GetEnumerator();

            if (recordEnumerator.MoveNext())
            {
                int category = (int)recordEnumerator.Current.category;

                switch (category)
                {
                    case (int)RecordCategory.Order:
                        return GetOrderDetails(context, bid, rid);
                    case (int)RecordCategory.Receipt:
                        return GetReceiptDetails(context, bid, rid);
                    case (int)RecordCategory.Change:
                        return GetChangeDetails(context, bid, rid);
                    default:
                        return null;
                }
            }
            else
            {
                throw new FaultException(string.Format("Record with id {0} is not found! ", rid));
            }
        }

        private List<RecordProduct> GetOrderDetails(ScanMyListDatabaseDataContext context, int bid, int rid)
        {
            List<RecordProduct> products = new List<RecordProduct>();

            var results = context.GetOrderDetails(bid, rid);

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        upc = product.product_upc,
                        customer = product.customer_id,
                        quantity = (int)product.product_quantity,
                        price = (double)product.product_price,
                        note = product.item_note
                    });
            }

            return products;
        }

        private List<RecordProduct> GetReceiptDetails(ScanMyListDatabaseDataContext context, int bid, int rid)
        {
            List<RecordProduct> products = new List<RecordProduct>();

            var results = context.GetReceiptDetails(bid, rid);

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        upc = product.product_upc,
                        supplier = product.supplier_id,
                        quantity = (int)product.product_quantity,
                        price = (double)product.product_price,
                        note = product.item_note
                    });
            }

            return products;
        }

        private List<RecordProduct> GetChangeDetails(ScanMyListDatabaseDataContext context, int bid, int rid)
        {
            List<RecordProduct> products = new List<RecordProduct>();

            var results = context.GetChangeDetails(bid, rid);

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        upc = product.product_upc,
                        quantity = (int)product.product_quantity
                    });
            }

            return products;
        }

        public string CreateRecord(Record newRecord)
        {
            SessionManager.CheckSession(newRecord.account, newRecord.sessionId);

            this.ValidateRecord(newRecord);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            if (newRecord.id == -1)
            {
                // Create a new record
                int rid = context.CreateRecord(
                        newRecord.business, newRecord.title, newRecord.category, newRecord.status, newRecord.comment);

                foreach (RecordProduct product in newRecord.products)
                {
                    context.AddProductToRecord(
                        rid, product.upc, product.supplier, product.customer, product.quantity, product.note);
                }

                if (newRecord.category == (int)RecordCategory.Change)
                {
                    this.IncrementInventories(newRecord.products, newRecord.business);
                    return string.Format("Inventory changed by Record {0}", newRecord.id);
                }
                else
                {
                    if (newRecord.status == (int)RecordStatus.sent)
                    {
                        string result = string.Format(
                            "{0} id=_{1}", this.SendRecord(newRecord.business, rid, newRecord.account, newRecord.sessionId), rid);

                        // check with database to see if we need to relay this new record to ERP system
                        // TO-DO
                        //ERPIntegrator.testAzureStorageQueue(rid);

                        return result;
                    }
                    else
                    {
                        return string.Format("New Record saved! id=_{0}", rid);
                    }
                }

            }
            else
            {
                // Update an existing record
                var result = context.GetRecord(newRecord.business, newRecord.id);
                IEnumerator<GetRecordResult> recordEnumerator = result.GetEnumerator();

                if (recordEnumerator.MoveNext())
                {
                    GetRecordResult retrievedRecord = recordEnumerator.Current;

                    if (retrievedRecord.status != (int)RecordStatus.saved)
                    {
                        throw new FaultException("Sent or Closed Record cannot be modified! ");
                    }
                    else
                    {
                        // Update Record's title
                        if (!newRecord.title.Equals(retrievedRecord.title))
                        {
                            context.UpdateRecordTitle(newRecord.business, newRecord.id, newRecord.title);
                        }

                        // Update Record's products
                        if (newRecord.products != null || newRecord.products.Count != 0)
                        {
                            context.ClearProductsFromRecord(newRecord.id);

                            foreach (RecordProduct product in newRecord.products)
                            {
                                context.AddProductToRecord(
                                    newRecord.id, product.upc, product.supplier, product.customer, product.quantity, product.note);
                            }
                        }

                        if (newRecord.status == (int)RecordStatus.sent)
                        {
                            return string.Format(
                                "{0} id=_{1}",
                                this.SendRecord(newRecord.business, newRecord.id, newRecord.account, newRecord.sessionId), newRecord.id);
                        }
                        else
                        {
                            return string.Format("New Record saved! id=_{0}", newRecord.id);
                        }
                    }
                }
                else
                {
                    throw new FaultException("The record you requested for update does not exist! ");
                }
            }
        }

        private void ValidateRecord(Record record)
        {
            if (record.category == (int)RecordCategory.Change)
            {
                if (record.id != -1)
                {
                    throw new FaultException("Cannot modify an Inventory change! ");
                }

                record.status = (int)RecordStatus.closed;
                foreach (RecordProduct product in record.products)
                {
                    product.supplier = 1;
                    product.customer = 1;
                    product.price = 0;
                }
            }
            else
            {
                if (record.status == (int)RecordStatus.closed)
                {
                    throw new FaultException(
                        "Cannot modify or create a Closed Record!\nUse \"CloseRecord\" to close a Record. ");
                }

                bool allHasSupplier = true;
                bool allHasCustomer = true;

                foreach (RecordProduct product in record.products)
                {
                    if (product.supplier == 1)
                    {
                        allHasSupplier = false;
                    }
                    if (product.customer == 1)
                    {
                        allHasCustomer = false;
                    }
                }

                if (record.status == (int)RecordStatus.sent)
                {
                    if (record.category == (int)RecordCategory.Order && !allHasCustomer)
                    {
                        throw new FaultException("Cannot send an Order without specifying all customers! ");
                    }
                    else if (record.category == (int)RecordCategory.Receipt && !allHasSupplier)
                    {
                        throw new FaultException("Cannot send a Receipt without specifying all suppliers! ");
                    }
                }
            }
        }

        public string UpdateRecordTitle(int bid, int rid, string title, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetRecord(bid, rid);
            IEnumerator<GetRecordResult> enumerator = result.GetEnumerator();
            if (enumerator.MoveNext())
            {
                GetRecordResult record = enumerator.Current;
                if (record.status == (int)RecordStatus.sent || record.status == (int)RecordStatus.closed)
                {
                    throw new FaultException("Sent or Closed Record's title cannot be updated! ");
                }
                else
                {
                    context.UpdateRecordTitle(bid, rid, title);
                    return string.Format("Record {0}'s title has been updated to {1}.", rid, title);
                }
            }
            else
            {
                throw new FaultException(
                    string.Format("Record with given id {0} not found! ", rid));
            }
        }

        public string SendRecord(int bid, int rid, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetRecord(bid, rid);
            IEnumerator<GetRecordResult> enumerator = result.GetEnumerator();

            if (enumerator.MoveNext())
            {
                switch (enumerator.Current.category)
                {
                    case (int)RecordCategory.Order:
                        return this.SendOrder(context, bid, rid, aid);
                    case (int)RecordCategory.Receipt:
                        return this.SendReceipt(context, bid, rid, aid);
                    case (int)RecordCategory.Change:
                        return this.SendChange(context, bid, rid, aid);
                    default:
                        throw new FaultException("Invalid Record type! ");
                }
            }
            else
            {
                throw new FaultException(
                    string.Format("Failed to find Record with id {0}! ", rid));
            }
        }

        private string SendOrder(ScanMyListDatabaseDataContext context, int bid, int rid, int aid)
        {
            var result = context.GetCompleteOrder(bid, rid);

            Record overallOrder = new Record() { date = -1, products = new List<RecordProduct>() };
            IDictionary<int, Record> orders = new Dictionary<int, Record>();
            IDictionary<int, Business> customers = new Dictionary<int, Business>();

            foreach (var product in result)
            {
                if (overallOrder.date == -1)
                {
                    overallOrder.id = product.record_id;
                    overallOrder.title = product.record_title;
                    overallOrder.date = (long)product.record_date;
                    overallOrder.category = (int)RecordCategory.Order;
                }

                if (!customers.ContainsKey(product.customer_id))
                {
                    customers.Add(
                        product.customer_id,
                        new Business()
                        {
                            id = product.customer_id,
                            name = product.customer_name,
                            email = product.customer_email
                        });

                    orders.Add(
                        product.customer_id,
                        new Record()
                        {
                            id = product.record_id,
                            title = product.record_title,
                            date = (long)product.record_date,
                            category = (int)RecordCategory.Order,
                            products = new List<RecordProduct>()
                        });
                }

                RecordProduct p = new RecordProduct()
                {
                    upc = product.product_upc,
                    name = product.product_name,
                    quantity = (int)product.product_quantity,
                    customer = product.customer_id
                };

                overallOrder.products.Add(p);
                orders[product.customer_id].products.Add(p);
            }

            string accountEmail = this.FetchAccountEmail(context, aid);
            Business business = this.FetchBusiness(context, bid);

            customers.Add(business.id, business);

            if (!MailHelper.SendRecord(bid, overallOrder, customers))
                throw new FaultException("Failed to send confirmation email! ");
            if (!MailHelper.SendRecordBackup(accountEmail, bid, overallOrder, customers))
                throw new FaultException("Failed to send system backup confirmatoin email! ");

            foreach (int customer in orders.Keys)
            {
                if (!MailHelper.SendRecord(customer, orders[customer], customers))
                    throw new FaultException(
                        string.Format("Failed to send confirmation email to Customer {0}! ", customers[customer].name));
            }

            this.DecrementInventories(overallOrder.products, bid);

            return string.Format("Order with id {0} sent! ", rid);
        }

        private string SendReceipt(ScanMyListDatabaseDataContext context, int bid, int rid, int aid)
        {
            var result = context.GetCompleteReceipt(bid, rid);

            Record overallReceipt = new Record() { date = -1, products = new List<RecordProduct>() };
            IDictionary<int, Record> receipts = new Dictionary<int, Record>();
            IDictionary<int, Business> suppliers = new Dictionary<int, Business>();

            foreach (var product in result)
            {
                if (overallReceipt.date == -1)
                {
                    overallReceipt.id = product.record_id;
                    overallReceipt.title = product.record_title;
                    overallReceipt.date = (long)product.record_date;
                    overallReceipt.category = (int)RecordCategory.Receipt;
                }

                if (!suppliers.ContainsKey(product.supplier_id))
                {
                    suppliers.Add(
                        product.supplier_id,
                        new Business()
                        {
                            id = product.supplier_id,
                            name = product.supplier_name,
                            email = product.supplier_email
                        });

                    receipts.Add(
                        product.supplier_id,
                        new Record()
                        {
                            id = product.record_id,
                            title = product.record_title,
                            date = (long)product.record_date,
                            category = (int)RecordCategory.Receipt,
                            products = new List<RecordProduct>()
                        });
                }

                RecordProduct p = new RecordProduct()
                {
                    upc = product.product_upc,
                    name = product.product_name,
                    quantity = (int)product.product_quantity,
                    supplier = product.supplier_id
                };

                overallReceipt.products.Add(p);
                receipts[product.supplier_id].products.Add(p);
            }

            string accountEmail = this.FetchAccountEmail(context, aid);
            Business business = this.FetchBusiness(context, bid);

            suppliers.Add(business.id, business);

            if (!MailHelper.SendRecord(bid, overallReceipt, suppliers))
                throw new FaultException("Failed to send confirmation email! ");
            if (!MailHelper.SendRecordBackup(accountEmail, bid, overallReceipt, suppliers))
                throw new FaultException("Failed to send system backup confirmatoin email! ");

            foreach (int supplier in receipts.Keys)
            {
                if (!MailHelper.SendRecord(supplier, receipts[supplier], suppliers))
                    throw new FaultException(
                        string.Format("Failed to send confirmation email to Supplier {0}! ", suppliers[supplier].name));
            }

            this.IncrementInventories(overallReceipt.products, bid);

            return string.Format("Receipt with id {0} sent! ", rid);
        }

        private string SendChange(ScanMyListDatabaseDataContext context, int bid, int rid, int aid)
        {
            var result = context.GetCompleteChange(bid, rid);

            Record change = new Record() { date = -1, products = new List<RecordProduct>() };
            IDictionary<int, Business> self = new Dictionary<int, Business>();

            foreach (var product in result)
            {
                if (change.date == -1)
                {
                    change.id = product.record_id;
                    change.title = product.record_title;
                    change.date = (long)product.record_date;
                    change.category = (int)RecordCategory.Change;
                }

                RecordProduct p = new RecordProduct()
                {
                    upc = product.product_upc,
                    name = product.product_name,
                    quantity = (int)product.product_quantity
                };

                change.products.Add(p);
            }

            string accountEmail = this.FetchAccountEmail(context, aid);
            Business business = this.FetchBusiness(context, bid);

            self.Add(business.id, business);

            if (!MailHelper.SendRecord(bid, change, self))
                throw new FaultException("Failed to send confirmation email! ");
            /*  we don't need to send backup for change
            if (MailHelper.SendRecordBackup(bid, change, self))
                throw new FaultException("Failed to send system backup confirmatoin email! ");
            */
            this.IncrementInventories(change.products, bid);

            return string.Format("Inventory change with id {0} sent! ", rid);
        }
    }
}