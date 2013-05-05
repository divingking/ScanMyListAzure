namespace ScanMyListWebRole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.Web.Script.Serialization;

    public enum RecordCategory
    {
        Order,
        Receipt,
        Change
    }

    public enum RecordStatus
    {
        saved,
        sent,
        closed
    }

    public class ScanMyListDataService : IScanMyListDataService
    {
        public Product GetProductByUPC(string upc, int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetProductByUPC(bid, upc);
            IEnumerator<GetProductByUPCResult> productEnumerator = results.GetEnumerator();
            if (productEnumerator.MoveNext())
            {
                GetProductByUPCResult target = productEnumerator.Current;
                return new Product()
                {
                    upc = target.upc,
                    name = target.name,
                    detail = target.detail,
                    quantity = (int)target.quantity,
                    location = target.location,
                    owner = bid,
                    leadTime = (int)target.lead_time
                };
            }
            else
            {
                throw new FaultException("Product with given UPC not found in your Inventory! ");
            }

            // Ask for search upc to find the info of the product scanned
            // Deprecated for now. 
            /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.searchupc.com/handlers/upcsearch.ashx?request_type=3&access_token=B7BE388C-87B1-435B-B096-01B727953FF4&upc=" + upc);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Dictionary<string, Dictionary<string, string>> values = jss.Deserialize<Dictionary<string, Dictionary<string, string>>>(reader.ReadToEnd());
            if (values.ContainsKey("0"))
            {
                Product newProduct = new Product()
                {
                    upc = upc,
                    name = values["0"]["productname"],
                    detail = null,
                    quantity = 0,
                    supplier = null, 
                    owner = bid, 
                    leadTime = 0
                };
                if (context.HasInventory(upc, bid) == 0)
                {
                    context.InitializeInventory(upc, bid, 0, 0);
                }
                return newProduct;
            }
            else
            {
                return null;
            }*/
        }

        public List<Product> GetProductByName(string name, int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetProductByName(bid, name);
            List<Product> products = new List<Product>();
            foreach (var result in results)
            {
                products.Add(
                    new Product()
                    {
                        upc = result.upc,
                        name = result.name,
                        detail = result.detail,
                        quantity = (int)result.quantity,
                        location = result.location,
                        owner = bid,
                        leadTime = (int)result.lead_time
                    }
                );
            }
            return products;
        }

        public string UpdateLeadTime(string upc, int bid, int lead_time, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            if (context.HasInventory(bid, upc) == 0)
            {
                throw new FaultException(
                    string.Format("This item {0} is not in your inventory!", upc));
            }
            else
            {
                context.UpdateInventoryLeadTime(bid, upc, lead_time);
            }

            return string.Format("Lead time for {0} of {1} updated", bid, upc);
        }

        public string UpdateProductLocation(string upc, int bid, string location, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            if (context.HasInventory(bid, upc) == 0)
            {
                throw new FaultException(
                    string.Format("This item {0} is not in your inventory!", upc));
            }
            else
            {
                context.UpdateInventoryLocation(bid, upc, location);
            }
            return string.Format("Product location for {0} of {1} updated", bid, upc);
        }

        // Not in use for now. 
        // Need to check session if this function is in used in the future
        public string NewProduct(Product newProduct)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            context.CreateProduct(newProduct.upc, newProduct.name, newProduct.detail);
            context.AddProductToInventory(newProduct.owner, newProduct.upc, newProduct.location, newProduct.quantity, newProduct.leadTime);
            return "New product created. ";
        }

        public List<Product> GetInventory(int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetAllInventory(bid);
            List<Product> inventory = new List<Product>();

            foreach (GetAllInventoryResult p in results)
            {
                Product product = new Product()
                {
                    upc = p.upc,
                    name = p.name,
                    detail = p.detail,
                    leadTime = (int)p.lead_time,
                    quantity = (int)p.quantity,
                    location = p.location,
                    owner = bid
                };

                inventory.Add(product);
            }

            return inventory;
        }

        public List<Business> GetSuppliers(string upc, int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetSuppliers(upc, bid);
            List<Business> suppliers = new List<Business>();
            foreach (var result in results)
            {
                suppliers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email,
                        price = (double)result.price
                    }
                );
            }
            return suppliers;
        }

        public List<Business> GetAllSuppliers(int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetAllSuppliers(bid);
            List<Business> suppliers = new List<Business>();
            foreach (var result in results)
            {
                suppliers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email
                    }
                );
            }
            return suppliers;
        }

        public List<Business> GetAllCustomers(int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetAllCustomers(bid);
            List<Business> customers = new List<Business>();
            foreach (var result in results)
            {
                customers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email
                    }
                );
            }
            return customers;
        }

        public int ProductCount(string upc, int aid, string sessionId)
        {
            // Not in use for now. 
            // For the future. 
            // Adding session check in the future.
            /*ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.ProductCount(upc);
            IEnumerator<ProductCountResult> counts = results.GetEnumerator();
            if (counts.MoveNext())
            {
                return (int)counts.Current.count;
            }
            else
            {
                return 0;
            }*/

            throw new NotImplementedException();
        }

        public List<Record> GetOrders(int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

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
                        price = (double)result.product_price
                    });
            }

            orders.Add(current);

            return orders;
        }

        public List<Record> GetReceipts(int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

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
                        price = (double)result.product_price
                    });
            }

            receipts.Add(current);

            return receipts;
        }

        public List<Record> GetRecordsByDate(int bid, long start, long end, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

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
            this.CheckSession(aid, sessionId);

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
            this.CheckSession(aid, sessionId);

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
                        price = (double)product.product_price
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
                        price = (double)product.product_price
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

        public string GetProductSummary(int bid, string upc, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            long start = -1;
            long end = -1;
            int orderCount = 0;
            int productCount = 0;
            int lastProductCount = 0;
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetProductSummary(bid, upc);
            IEnumerator<GetProductSummaryResult> enumerator = results.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (start == -1)
                {
                    start = (long)(enumerator.Current.date);
                }
                end = (long)(enumerator.Current.date);

                orderCount++;
                productCount += (int)(enumerator.Current.quantity);
                lastProductCount = (int)(enumerator.Current.quantity);
            }

            double days = GetDays(start, end);

            return string.Format("{0} {1}", (double)(productCount - lastProductCount) / days, days / (double)orderCount);
        }

        public List<RecordProduct> GetProductSummaryData(int bid, string upc, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            List<RecordProduct> products = new List<RecordProduct>();

            var retrievedProducts = context.GetProductSummary(bid, upc);

            foreach (var product in retrievedProducts)
            {
                products.Add(
                    new RecordProduct()
                    {
                        upc = upc,
                        supplier = (int)product.supplier,
                        customer = (int)product.customer,
                        quantity = (int)product.quantity
                    });
            }

            return products;
        }

        private double GetDays(long start, long end)
        {
            String startStr = start.ToString();
            String endStr = end.ToString();
            DateTime startDate = new DateTime(
                Convert.ToInt32(startStr.Substring(0, 4)),          // year
                Convert.ToInt32(startStr.Substring(4, 2)),          // month   
                Convert.ToInt32(startStr.Substring(6, 2)),          // day
                Convert.ToInt32(startStr.Substring(8, 2)),          // hour
                Convert.ToInt32(startStr.Substring(10, 2)),         // minute
                Convert.ToInt32(startStr.Substring(12, 2)));        // second
            DateTime endDate = new DateTime(
                Convert.ToInt32(endStr.Substring(0, 4)),          // year
                Convert.ToInt32(endStr.Substring(4, 2)),          // month   
                Convert.ToInt32(endStr.Substring(6, 2)),          // day
                Convert.ToInt32(endStr.Substring(8, 2)),          // hour
                Convert.ToInt32(endStr.Substring(10, 2)),         // minute
                Convert.ToInt32(endStr.Substring(12, 2)));        // second

            TimeSpan dateDiff = endDate.Subtract(startDate);
            return Convert.ToDouble(dateDiff.Days);
        }

        public string CreateRecord(Record newRecord)
        {
            this.CheckSession(newRecord.account, newRecord.sessionId);

            this.ValidateRecord(newRecord);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            if (newRecord.id == -1)
            {
                // Create a new record
                int rid = context.CreateRecord(
                        newRecord.business, newRecord.title, newRecord.category, newRecord.status);

                foreach (RecordProduct product in newRecord.products)
                {
                    context.AddProductToRecord(
                        rid, product.upc, product.supplier, product.customer, product.quantity);
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
                        return string.Format(
                            "{0} id=_{1}", this.SendRecord(newRecord.business, rid, newRecord.account, newRecord.sessionId), rid);
                    }
                    else
                    {
                        return string.Format("New Record saved! id=_{0}", newRecord.id);
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
                                    newRecord.id, product.upc, product.supplier, product.customer, product.quantity);
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
            this.CheckSession(aid, sessionId);

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
            this.CheckSession(aid, sessionId);

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
                            category = (int)RecordCategory.Receipt, 
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

            if (MailHelper.SendRecord(bid, overallOrder, customers))
                throw new FaultException("Failed to send confirmation email! ");
            if (MailHelper.SendRecordBackup(bid, overallOrder, customers))
                throw new FaultException("Failed to send system backup confirmatoin email! ");

            foreach (int customer in orders.Keys)
            {
                if (MailHelper.SendRecord(customer, orders[customer], customers))
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
                            category = (int)RecordCategory.Order,
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

            if (MailHelper.SendRecord(bid, overallReceipt, suppliers))
                throw new FaultException("Failed to send confirmation email! ");
            if (MailHelper.SendRecordBackup(bid, overallReceipt, suppliers))
                throw new FaultException("Failed to send system backup confirmatoin email! ");

            foreach (int supplier in receipts.Keys)
            {
                if (MailHelper.SendRecord(supplier, receipts[supplier], suppliers))
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
                    change.category = (int)RecordCategory.Receipt;
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

            if (MailHelper.SendRecord(bid, change, self))
                throw new FaultException("Failed to send confirmation email! ");
            if (MailHelper.SendRecordBackup(bid, change, self))
                throw new FaultException("Failed to send system backup confirmatoin email! ");

            this.IncrementInventories(change.products, bid);

            return string.Format("Inventory change with id {0} sent! ", rid);
        }

        private Business FetchBusiness(ScanMyListDatabaseDataContext context, int bid)
        {
            var result = context.GetBusiness(bid);
            IEnumerator<GetBusinessResult> businessEnumerator = result.GetEnumerator();

            if (businessEnumerator.MoveNext())
            {
                GetBusinessResult retrievedBusiness = businessEnumerator.Current;

                return new Business()
                {
                    id = retrievedBusiness.id, 
                    name = retrievedBusiness.name, 
                    address = retrievedBusiness.address, 
                    zip = (int)retrievedBusiness.zip, 
                    category = retrievedBusiness.category, 
                    email = retrievedBusiness.email
                };
            }
            else
            {
                throw new FaultException(
                    string.Format("Failed to fetch information for Business {0}", bid));
            }
        }

        private string FetchAccountEmail(ScanMyListDatabaseDataContext context, int aid)
        {
            var result = context.GetAccountEmail(aid);
            IEnumerator<GetAccountEmailResult> enumerator = result.GetEnumerator();

            if (enumerator.MoveNext())
            {
                return enumerator.Current.email;
            }
            else
            {
                throw new FaultException(
                    string.Format("Account with id {0} not found! ", aid));
            }
        }

        public int RegisterBusiness(Business business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            int bid = context.RegisterBusiness(
                business.name, business.address, business.zip, business.email, business.category);

            return bid;
        }

        public int RegisterAccount(User user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string passwordHash = Encryptor.Generate512Hash(user.pass);

            int bid = context.RegisterAccount(
                user.login, 
                passwordHash,
                user.email, 
                user.business, 
                user.tier);

            return bid;
        }

        public List<Business> SearchBusinessByName(string name, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.SearchBusinessByName(name);

            List<Business> retrievedBusiness = new List<Business>();

            foreach (var business in results)
            {
                retrievedBusiness.Add(
                    new Business()
                    {
                        id = business.id, 
                        name = business.name, 
                        address = business.address, 
                        zip = (int)business.zip, 
                        email = business.email, 
                        category = business.category
                    });
            }

            return retrievedBusiness;
        }

        public string LinkAccountToBusiness(int bid, int aid, string sessionId)
        {
            this.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var acountBusinessResult = context.GetAccountBusiness(aid);
            IEnumerator<GetAccountBusinessResult> accountBusinessEnumerator = acountBusinessResult.GetEnumerator();

            if (accountBusinessEnumerator.MoveNext())
            {
                if (accountBusinessEnumerator.Current.business == 0)
                {
                    var requestBusinessResult = context.GetBusiness(bid);
                    IEnumerator<GetBusinessResult> requestBusinessEnumerator = requestBusinessResult.GetEnumerator();

                    if (requestBusinessEnumerator.MoveNext())
                    {
                        context.LinkAccountToBusiness(aid, bid);
                        return string.Format("Account {0} successfully linked to Business {1}", aid, bid);
                    }
                    else
                    {
                        throw new FaultException("Business requested to link does not exist! ");
                    }
                }
                else
                {
                    throw new FaultException("This account is already linked to a business! ");
                }
            }
            else
            {
                throw new FaultException("Account does not exist! ");
            }
        }

        public User Login(LoginUser user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string passwordHash = Encryptor.Generate512Hash(user.pass);
            
            var results = context.Login(user.login, passwordHash);

            IEnumerator<LoginResult> loggedIn = results.GetEnumerator();
            if (loggedIn.MoveNext())
            {
                /*
                if (string.IsNullOrEmpty(loggedIn.Current.session_id))
                {
                    Random rand = new Random();
                    string sessionValue = string.Format("{0}", rand.Next());
                    string sessionId = Encryptor.GenerateHash(sessionValue);

                    context.UpdateSessionId(loggedIn.Current.id, sessionId);

                    return new User()
                    {
                        id = loggedIn.Current.id, 
                        business = (int)loggedIn.Current.business,
                        tier = (int)loggedIn.Current.tier,
                        sessionId = sessionId
                    };
                }
                else
                {
                    throw new FaultException("The account has already logged in! ");
                }*/
                // Changed logic to allow multiple logins into same account (not Singleton anymore)
                // by CH
                Random rand = new Random();
                string sessionValue = string.Format("{0}", rand.Next());
                string sessionId = Encryptor.GenerateSimpleHash(sessionValue);

                context.UpdateSessionId(loggedIn.Current.id, sessionId);

                return new User()
                {
                    id = loggedIn.Current.id,
                    business = (int)loggedIn.Current.business,
                    tier = (int)loggedIn.Current.tier,
                    sessionId = sessionId
                };

            }
            else
            {
                throw new FaultException("Invalid username or password. ");
            }
        }

        public string Logout(int aid, string sessionId)
        {
            try
            {
                this.CheckSession(aid, sessionId);
            }
            catch (FaultException)
            {

                throw new FaultException("The user is not logged in yet! ");
            }

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            context.Logout(aid);

            return string.Format("User {0} logged out. ", aid);
        }

        private void CheckSession(int aid, string sessionId)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string retrievedSessionId = null;
            context.GetSessionId(aid, ref retrievedSessionId);

            if (!sessionId.Equals(retrievedSessionId))
            {
                throw new FaultException("Session Expired! ");
            }
        }

        private void IncrementInventories(IList<RecordProduct> products, int business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            foreach (RecordProduct product in products)
            {
                context.IncrementInventory(business, product.upc, product.quantity);
            }
        }

        private void DecrementInventories(IList<RecordProduct> products, int business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            foreach (RecordProduct product in products)
            {
                context.IncrementInventory(business, product.upc, -1 * product.quantity);
            }
        }
    }
}
