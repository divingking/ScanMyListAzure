using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web.Script.Serialization;

namespace ScanMyListWebRole
{
    public class ScanMyListDataService : IScanMyListDataService
    {
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

        public Product GetProductByUPC(string upc, int bid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
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
                        supplier = null,
                        owner = bid,
                        leadTime = (int)target.lead_time
                    };
                }
                else
                {
                    return null;
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
            else
            {
                return null;
            }
        }

        public List<Product> GetProductByName(string name, int bid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
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
                            supplier = null,
                            quantity = (int)result.quantity,
                            location = result.location,
                            owner = bid,
                            leadTime = (int)result.lead_time
                        }
                    );
                }
                return products;
            }
            else
            {
                return null;
            }
        }

        public string UpdateLeadTime(string upc, int bid, int lead_time, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (context.HasInventory(bid, upc) == 0)
                {
                    return string.Format("This item {0} is not in your inventory!", upc);
                }
                else
                {
                    context.UpdateInventoryLeadTime(bid, upc, lead_time);
                }
                return string.Format("Lead time for {0} of {1} updated", bid, upc);
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public string UpdateInventoryQuantity(string upc, int bid, int quantity, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (context.HasInventory(bid, upc) == 0)
                {
                    return string.Format("This item {0} is not in your inventory!", upc);
                }
                else
                {
                    context.UpdateInventoryQuantity(bid, upc, quantity);
                }
                return string.Format("Inventory quantity for {0} of {1} updated", bid, upc);
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public string UpdateProductLocation(string upc, int bid, string location, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (context.HasInventory(bid, upc) == 0)
                {
                    return string.Format("This item {0} is not in your inventory!", upc);
                }
                else
                {
                    context.UpdateInventoryLocation(bid, upc, location);
                }
                return string.Format("Product location for {0} of {1} updated", bid, upc);
            }
            else
            {
                return "Session Expired! ";
            }
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

        public List<Product> GetInventory(int bid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
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
            else
            {
                return null;
            }
            
        }

        public List<Business> GetSuppliers(string upc, int bid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
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
            else
            {
                return null;
            }
        }

        public List<Business> GetAllSuppliers(int bid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
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
            else
            {
                return null;
            }
        }

        public int ProductCount(string upc, string sessionId)
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

        public List<Order> GetOrders(int bid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetOrders(bid);
                List<Order> orders = new List<Order>();
                Order current = null;
                foreach (var result in results)
                {
                    if (current == null)
                    {
                        current = new Order()
                        {
                            bid = bid,
                            title = result.title,
                            oid = result.oid,
                            date = (long)result.date,
                            scanIn = false,
                            sent = true,
                            products = new List<Product>()
                        };
                    }
                    else if (current.oid != result.oid)
                    {
                        orders.Add(current);
                        current = new Order()
                        {
                            bid = bid,
                            title = result.title,
                            oid = result.oid,
                            date = (long)result.date,
                            scanIn = false,
                            sent = true,
                            products = new List<Product>()
                        };
                    }
                    current.products.Add(
                        new Product()
                        {
                            upc = result.upc,
                            name = result.product_name,
                            detail = result.detail,
                            supplier = new Business() {
                                id = result.customer_id, 
                                name = result.customer_name, 
                                address = result.address, 
                                zip = (int)result.zip, 
                                email = result.email, 
                                price = (double)result.price
                            },
                            quantity = (int)result.quantity,
                            owner = bid,
                            leadTime = (int)result.lead_time,
                            location = result.location
                        }
                    );
                }
                orders.Add(current);
                return orders;
            }
            else
            {
                return null;
            }
        }

        public List<Order> GetReceipts(int bid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetRecepts(bid);
                List<Order> receipts = new List<Order>();
                Order current = null;
                foreach (var result in results)
                {
                    if (current == null)
                    {
                        current = new Order()
                        {
                            bid = bid,
                            title = result.title,
                            oid = result.oid,
                            date = (long)result.date,
                            scanIn = false,
                            sent = true,
                            products = new List<Product>()
                        };
                    }
                    else if (current.oid != result.oid)
                    {
                        receipts.Add(current);
                        current = new Order()
                        {
                            bid = bid,
                            title = result.title,
                            oid = result.oid,
                            date = (long)result.date,
                            scanIn = false,
                            sent = true,
                            products = new List<Product>()
                        };
                    }
                    current.products.Add(
                        new Product()
                        {
                            upc = result.upc,
                            name = result.product_name,
                            detail = result.detail,
                            supplier = new Business()
                            {
                                id = result.supplier_id,
                                name = result.supplier_name,
                                address = result.address,
                                zip = (int)result.zip,
                                email = result.email,
                                price = (double)result.price
                            },
                            quantity = (int)result.quantity,
                            owner = result.supplier_id,
                            leadTime = (int)result.lead_time,
                            location = result.location
                        }
                    );
                }
                receipts.Add(current);
                return receipts;
            }
            else
            {
                return null;
            }
        }

        public List<Order> GetOrdersByDate(int bid, long start, long end, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetOrdersByTimeRange(bid, start, end);
                List<Order> orders = new List<Order>();
                foreach (GetOrdersByTimeRangeResult order in results)
                {
                    orders.Add(new Order()
                    {
                        bid = bid,
                        oid = order.id,
                        title = order.title,
                        scanIn = (bool)order.scan_in,
                        sent = (bool)order.sent,
                        date = (long)order.date,
                        products = null
                    });
                }
                return orders;
            }
            else
            {
                return null;
            }
        }

        public List<Order> GetOrdersByRange(int bid, int start, int end, string sessionId)
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

        public List<Order> GetNOrdersFromLast(int bid, int last, int n, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetNOrdersFromLast(bid, last, n);
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
            }
        }

        public List<RecordProduct> GetRecordDetails(int bid, int rid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
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
                    return null;
                }
            }
            else
            {
                return null;
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

        public string GetProductSummary(int bid, string upc, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
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
            else
            {
                return "Session Expired! ";
            }
        }

        public List<RecordProduct> GetProductSummaryData(int bid, string upc, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

                List<RecordProduct> products = new List<RecordProduct>();

                var retrievedProducts = context.GetProductSummary(bid, upc, sessionId);

                foreach (var product in retrievedProducts)
                {
                    products.Add(
                        new RecordProduct()
                        {
                            upc = upc, 
                            supplier = product.supplier, 
                            customer = product.customer, 
                            quantity = product.quantity
                        });
                }

                return products;
            }
            else
            {
                return null;
            }
        }

        private double GetDays(long start, long end)
        {
            String startStr = start.ToString();
            String endStr = end.ToString();
            DateTime startDate = new DateTime(Convert.ToInt32(startStr.Substring(0, 4)),          // year
                                                Convert.ToInt32(startStr.Substring(4, 2)),          // month   
                                                Convert.ToInt32(startStr.Substring(6, 2)),          // day
                                                Convert.ToInt32(startStr.Substring(8, 2)),          // hour
                                                Convert.ToInt32(startStr.Substring(10, 2)),         // minute
                                                Convert.ToInt32(startStr.Substring(12, 2)));        // second
            DateTime endDate = new DateTime(Convert.ToInt32(endStr.Substring(0, 4)),          // year
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
            if (CheckSession(newRecord.business, newRecord.sessionId))
            {
                this.ValidateRecord(newRecord);

                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (newRecord.id == -1)
                {
                    // Create a new record
                    int rid = context.CreateRecord(
                            newRecord.business, newRecord.title, newRecord.category, newRecord.status); ;
                    if (newRecord.category == (int)RecordCategory.Change)
                    {
                        newRecord.status = (int)RecordStatus.closed;
                    }

                    foreach (RecordProduct product in newRecord.products)
                    {
                        context.AddProductToRecord(
                            rid, product.upc, product.supplier, product.customer, product.quantity);
                    }

                    if (newRecord.status == (int)RecordStatus.sent)
                    {
                        return string.Format("{0} id=_{1}", SendOrder(newRecord.business, rid, newRecord.sessionId), rid);
                    }
                    else
                    {
                        return string.Format("New Record saved! id=_{0}", newRecord.id);
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
                            if (!newRecord.title.Equals(retrievedRecord.title))
                            {
                                context.UpdateRecordTitle(newRecord.business, newRecord.id, newRecord.title);
                            }
                            
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
                                    SendOrder(newRecord.business, newRecord.id, newRecord.sessionId), newRecord.id);
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
            else
            {
                throw new FaultException("Session Expired! ");
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
                    throw new FaultException("Cannot create a closed Record! ");
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

        public string CreateOrder(Order NewOrder)
        {
            if (CheckSession(NewOrder.bid, NewOrder.sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (NewOrder.oid == -1)
                {
                    int oid = context.CreateOrder(NewOrder.title, NewOrder.bid, false, NewOrder.scanIn);
                    bool allHasSupplier = true;
                    foreach (Product current in NewOrder.products)
                    {
                        if (current.supplier == null)
                        {
                            context.AddProductToOrder(oid, current.upc, null, current.quantity);
                            allHasSupplier = false;
                        }
                        else
                        {
                            context.AddProductToOrder(oid, current.upc, current.supplier.id, current.quantity);
                        }
                    }

                    if (NewOrder.sent && allHasSupplier)
                    {
                        return string.Format("{0} id=_{1}", SendOrder(NewOrder.bid, oid, NewOrder.sessionId), oid);
                    }
                    else if (NewOrder.sent)
                    {
                        return string.Format(
                            "Order without supplier specified cannot be sent! Stored as unsent instead. id=_{0}",
                            NewOrder.oid);
                    }

                    return string.Format("New Order created! id=_{0}", NewOrder.oid);
                }
                else
                {
                    var result = context.GetOrderOverview(NewOrder.bid, NewOrder.oid);
                    IEnumerator<GetOrderOverviewResult> enumerator = result.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        GetOrderOverviewResult retrievedOrder = enumerator.Current;
                        if ((bool)retrievedOrder.sent)
                        {
                            return string.Format("Sent order cannot be modified! id=_{0}", NewOrder.oid);
                        }
                        else
                        {
                            if (!NewOrder.title.Equals(retrievedOrder.title))
                            {
                                context.UpdateOrderTitle(NewOrder.bid, NewOrder.oid, NewOrder.title);
                            }
                            if (retrievedOrder.scan_in != NewOrder.scanIn)
                            {
                                context.UpdateOrderScanIn(NewOrder.bid, NewOrder.oid, NewOrder.scanIn);
                            }
                            bool allHasSupplier = true;
                            if (NewOrder.products != null || NewOrder.products.Count != 0)
                            {
                                context.ClearProductFromOrder(NewOrder.oid);

                                foreach (Product current in NewOrder.products)
                                {
                                    if (current.supplier == null)
                                    {
                                        context.AddProductToOrder(NewOrder.oid, current.upc, null, current.quantity);
                                        allHasSupplier = false;
                                    }
                                    else
                                    {
                                        context.AddProductToOrder(NewOrder.oid, current.upc, current.supplier.id, current.quantity);
                                    }
                                }
                            }

                            if (NewOrder.sent && allHasSupplier)
                            {
                                return string.Format("{0} id=_{1}", SendOrder(NewOrder.bid, NewOrder.oid), NewOrder.oid);
                            }
                            else if (NewOrder.sent)
                            {
                                return string.Format(
                                    "Order without supplier specified cannot be sent! Stored as unsent instead. id=_{0}",
                                    NewOrder.oid);
                            }

                            return string.Format("Order updated! id=_{0}", NewOrder.oid);
                        }
                    }
                    else
                    {
                        return "The order you requested for update does not exist! ";
                    }
                }
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public string UpdateRecordTitle(int bid, int rid, string title, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
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
                    return "No such record! ";
                }
            }
            else
            {
                throw new FaultException("Session Expired! ");
            }
        }

        public string SendOrder(int bid, int oid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var result = context.GetOrder(bid, oid);
                Order order = new Order()
                {
                    bid = bid,
                    oid = oid,
                    date = 0,
                    title = null,
                    scanIn = false,
                    sent = false,
                    products = null
                };
                foreach (var product in result)
                {
                    if (order.products == null)
                    {
                        order.title = product.title;
                        order.date = (long)product.date;
                        order.scanIn = (bool)product.scan_in;
                        order.sent = (bool)product.sent;
                        order.products = new List<Product>();
                    }

                    order.products.Add(new Product()
                    {
                        upc = product.upc,
                        name = product.pname,
                        detail = product.detail,
                        owner = bid,
                        leadTime = (int)product.lead_time,
                        quantity = (int)product.quantity,
                        supplier = new Supplier()
                        {
                            id = product.sid,
                            name = product.sname,
                            address = product.saddr,
                            business = product.business,
                            price = (double)product.price
                        }
                    });
                }

                if (order.date == 0)
                {
                    return "No such order!";
                }
                else if (order.sent)
                {
                    return "The order cannot be sent twice!";
                }
                else
                {
                    if (order.scanIn)
                    {
                        IncrementInventories(order.products, order.bid);
                    }
                    else
                    {
                        DecrementInventories(order.products, order.bid);
                    }
                    context.SendOrder(bid, oid);
                    if (!MailHelper.SendOrderBackup(order))
                        return "Fail to send system confirmation mail!";
                    if (!MailHelper.SendOrder(order, bid))
                        return "Fail to send confirmation mail!";
                    return "Order sent!";
                }
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public int RegisterBusiness(Business business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            int bid = context.RegisterBusiness(
                business.name, business.address, business.zip, business.email, business.category);

            return bid;
        }

        public int RegisterAccount(NewUser user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string passwordHash = Encryptor.GenerateHash(user.pass);

            int bid = context.RegisterAccount(
                user.login, 
                passwordHash, 
                user.business, 
                user.tier);

            return bid;
        }

        public string Login(User user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string passwordHash = Encryptor.GenerateHash(user.pass);

            var results = context.Login(user.login, passwordHash);
            IEnumerator<LoginResult> loggedIn = results.GetEnumerator();
            if (loggedIn.MoveNext())
            {
                int bid = loggedIn.Current.id;
                Random rand = new Random();
                string sessionValue = string.Format("{0}", rand.Next());
                string sessionId = Encryptor.GenerateHash(sessionValue);

                context.UpdateSessionId(user.login, sessionId);

                return sessionId;
            }
            else
            {
                throw new FaultException("Invalid username or password. ");
            }
        }

        public string Logout(int bid, string sessionId)
        {
            if (CheckSession(bid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                context.Logout(bid);

                return string.Format("User {0} logged out. ", bid);
            }
            else
            {
                return "Session Expired or the user is not logged in yet! ";
            }
        }

        private bool CheckSession(int bid, string sessionId)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string retrievedSessionId = null;
            context.GetSessionId(bid, ref retrievedSessionId);

            return sessionId.Equals(retrievedSessionId);
        }

        private void IncrementInventories(IList<Product> products, int business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            foreach (Product product in products)
            {
                context.IncrementInventory(business, product.upc, product.quantity);
            }
        }

        private void DecrementInventories(IList<Product> products, int business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            foreach (Product product in products)
            {
                context.IncrementInventory(business, product.upc, -1 * product.quantity);
            }
        }
    }
}
