using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace ScanMyListWebRole
{
    public class ScanMyListDataService : IScanMyListDataService
    {
        public Product GetProductByUPC(string upc, int cid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetProductByUPC(upc, cid);
                IEnumerator<GetProductByUPCResult> productEnumerator = results.GetEnumerator();
                if (productEnumerator.MoveNext())
                {
                    GetProductByUPCResult target = productEnumerator.Current;
                    return new Product()
                    {
                        upc = upc,
                        name = target.name,
                        detail = target.detail,
                        quantity = (int)target.quantity,
                        location = target.location,
                        supplier = null,
                        owner = cid,
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
                        owner = cid, 
                        leadTime = 0
                    };
                    if (context.HasInventory(upc, cid) == 0)
                    {
                        context.InitializeInventory(upc, cid, 0, 0);
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

        public List<Product> GetProductByName(string name, int cid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetProductByName(name, cid);
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
                            owner = cid,
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

        public string UpdateLeadTime(string upc, int cid, int lead_time, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (context.HasInventory(upc, cid) == 0)
                {
                    return string.Format("This item {0} is not in your inventory!", upc);
                }
                else
                {
                    context.UpdateLeadTime(upc, cid, lead_time);
                }
                return string.Format("Lead time for {0} of {1} updated", cid, upc);
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public string UpdateInventoryQuantity(string upc, int cid, int quantity, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (context.HasInventory(upc, cid) == 0)
                {
                    return string.Format("This item {0} is not in your inventory!", upc);
                }
                else
                {
                    context.UpdateInventoryQuantity(upc, cid, quantity);
                }
                return string.Format("Inventory quantity for {0} of {1} updated", cid, upc);
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public string UpdateProductLocation(string upc, int cid, string location, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (context.HasInventory(upc, cid) == 0)
                {
                    return string.Format("This item {0} is not in your inventory!", upc);
                }
                else
                {
                    context.UpdateProductLocation(upc, cid, location);
                }
                return string.Format("Product location for {0} of {1} updated", cid, upc);
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
            context.AddProduct(newProduct.upc, newProduct.name, newProduct.detail);
            context.InitializeInventory(newProduct.upc, newProduct.owner, newProduct.leadTime, newProduct.quantity, newProduct.location);
            return "New product created. ";
        }

        public List<Product> GetInventory(int cid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetInventory(cid);
                List<Product> inventory = new List<Product>();

                foreach (GetInventoryResult p in results)
                {
                    Product product = new Product()
                    {
                        upc = p.upc,
                        name = p.name,
                        detail = p.detail,
                        leadTime = (int)p.lead_time,
                        quantity = (int)p.quantity,
                        location = p.location,
                        owner = cid
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

        public List<Supplier> GetSuppliers(string upc, int cid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetSuppliers(upc, cid);
                List<Supplier> suppliers = new List<Supplier>();
                foreach (var result in results)
                {
                    suppliers.Add(
                        new Supplier()
                        {
                            id = result.id,
                            name = result.name,
                            address = result.addr,
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

        public List<Supplier> GetAllSuppliers(int cid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetAllSuppliers(cid);
                List<Supplier> suppliers = new List<Supplier>();
                foreach (var result in results)
                {
                    suppliers.Add(
                        new Supplier()
                        {
                            id = result.id,
                            name = result.name,
                            address = result.addr,
                            price = 0
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
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.ProductCount(upc);
            IEnumerator<ProductCountResult> counts = results.GetEnumerator();
            if (counts.MoveNext())
            {
                return (int)counts.Current.count;
            }
            else
            {
                return 0;
            }
        }

        public List<Order> GetOrders(int cid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetOrders(cid);
                List<Order> orders = new List<Order>();
                Order current = null;
                foreach (var result in results)
                {
                    if (current == null)
                    {
                        current = new Order()
                        {
                            cid = (int)result.cid,
                            title = result.title,
                            oid = result.oid,
                            date = (long)result.date,
                            scanIn = (bool)result.scan_in,
                            sent = (bool)result.sent,
                            products = new List<Product>()
                        };
                    }
                    else if (current.oid != result.oid)
                    {
                        orders.Add(current);
                        current = new Order()
                        {
                            cid = (int)result.cid,
                            title = result.title,
                            oid = result.oid,
                            date = (long)result.date,
                            scanIn = (bool)result.scan_in,
                            sent = (bool)result.sent,
                            products = new List<Product>()
                        };
                    }
                    current.products.Add(
                        new Product()
                        {
                            upc = result.upc,
                            name = result.pname,
                            detail = result.detail,
                            supplier = new Supplier()
                            {
                                id = result.sid,
                                name = result.sname,
                                address = result.saddr,
                                price = (double)result.price
                            },
                            quantity = (int)result.quantity,
                            owner = cid,
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

        public List<Order> GetOrdersByDate(int cid, long start, long end, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetOrdersByDate(cid, start, end);
                List<Order> orders = new List<Order>();
                foreach (GetOrdersByDateResult order in results)
                {
                    orders.Add(new Order()
                    {
                        cid = (int)order.cid,
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

        public List<Order> GetOrdersByRange(int cid, int start, int end, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetOrdersByRange(cid, start, end);
                List<Order> orders = new List<Order>();
                foreach (var result in results)
                {
                    orders.Add(new Order()
                    {
                        cid = cid,
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

        public List<Order> GetNOrdersFromLast(int cid, int last, int n, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetNOrdersFromLast(cid, last, n);
                List<Order> orders = new List<Order>();
                foreach (var result in results)
                {
                    orders.Add(new Order()
                    {
                        cid = cid,
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

        public List<Product> GetProductsForOrder(int cid, int oid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetProductsForOrder(oid, cid);
                List<Product> products = new List<Product>();
                foreach (var result in results)
                {
                    products.Add(new Product()
                    {
                        upc = result.upc,
                        name = result.product_name,
                        detail = result.product_detail,
                        owner = cid,
                        leadTime = (int)result.lead_time,
                        quantity = (int)result.quantity,
                        location = result.location,
                        supplier = new Supplier()
                        {
                            id = result.supplier_id,
                            name = result.supplier_name,
                            address = result.supplier_addr,
                            business = result.business,
                            price = (double)result.price
                        }
                    });
                }
                return products;
            }
            else
            {
                return null;
            }
        }

        public string GetProductSummary(int cid, string upc, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                long start = -1;
                long end = -1;
                int orderCount = 0;
                int productCount = 0;
                int lastProductCount = 0;
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var results = context.GetProductSummary(cid, upc);
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
                double days = Math.Ceiling((end - start) / 86400.0);
                return string.Format("{0} {1}", (double)(productCount - lastProductCount) / days, days / (double)orderCount);
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public string CreateOrder(Order NewOrder)
        {
            if (CheckSession(NewOrder.cid, NewOrder.sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                if (NewOrder.oid == -1)
                {
                    int oid = context.CreateOrder(NewOrder.title, NewOrder.cid, false, NewOrder.scanIn);
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
                        return string.Format("{0} id=_{1}", SendOrder(NewOrder.cid, oid, NewOrder.sessionId), oid);
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
                    var result = context.GetOrderOverview(NewOrder.cid, NewOrder.oid);
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
                                context.UpdateOrderTitle(NewOrder.cid, NewOrder.oid, NewOrder.title);
                            }
                            if (retrievedOrder.scan_in != NewOrder.scanIn)
                            {
                                context.UpdateOrderScanIn(NewOrder.cid, NewOrder.oid, NewOrder.scanIn);
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
                                return string.Format("{0} id=_{1}", SendOrder(NewOrder.cid, NewOrder.oid), NewOrder.oid);
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

        public string UpdateOrderTitle(int cid, int oid, string title, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var result = context.GetOrderOverview(cid, oid);
                IEnumerator<GetOrderOverviewResult> enumerator = result.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    GetOrderOverviewResult order = enumerator.Current;
                    if ((bool)order.sent)
                    {
                        return "Sent order's title cannot be updated! ";
                    }
                    else
                    {
                        context.UpdateOrderTitle(cid, oid, title);
                        return string.Format("Order {0}'s title has been updated to {1}.", oid, title);
                    }
                }
                else
                {
                    return "No such order! ";
                }
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public string SendOrder(int cid, int oid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                var result = context.GetOrder(cid, oid);
                Order order = new Order()
                {
                    cid = cid,
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
                        owner = cid,
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
                        IncrementInventories(order.products, order.cid);
                    }
                    else
                    {
                        DecrementInventories(order.products, order.cid);
                    }
                    context.SendOrder(cid, oid);
                    if (!MailHelper.SendOrderBackup(order))
                        return "Fail to send system confirmation mail!";
                    if (!MailHelper.SendOrder(order, cid))
                        return "Fail to send confirmation mail!";
                    return "Order sent!";
                }
            }
            else
            {
                return "Session Expired! ";
            }
        }

        public int Register(NewUser user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string passwordHash = Encryptor.GenerateHash(user.pass);

            int cid = context.CreateNewCustomer(
                user.login, 
                passwordHash, 
                user.fname, 
                user.mname, 
                user.lname, 
                user.address, 
                user.email, 
                user.email, 
                user.tier);

            return cid;
        }

        public string Login(User user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string passwordHash = Encryptor.GenerateHash(user.pass);
            var results = context.Login(user.login, passwordHash);
            IEnumerator<LoginResult> loggedIn = results.GetEnumerator();
            if (loggedIn.MoveNext())
            {
                int cid = loggedIn.Current.id;
                Random rand = new Random();
                string sessionValue = string.Format("{0}", rand.Next());
                string sessionId = Encryptor.GenerateHash(sessionValue);

                context.SetSessionId(cid, sessionId);

                return sessionId;
            }
            else
            {
                return "Invalid username or password. ";
            }
        }

        public string Logout(int cid, string sessionId)
        {
            if (CheckSession(cid, sessionId))
            {
                ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
                context.Logout(cid);

                return string.Format("User {0} logged out. ", cid);
            }
            else
            {
                return "Session Expired or the user is not logged in yet! ";
            }
        }

        private bool CheckSession(int cid, string sessionId)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string retrievedSessionId = null;
            context.GetSessionId(cid, ref retrievedSessionId);

            return sessionId.Equals(retrievedSessionId);
        }

        private void IncrementInventories(IList<Product> products, int customerId)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            foreach (Product product in products)
            {
                context.IncrementInventory(product.upc, customerId, product.quantity);
            }
        }

        private void DecrementInventories(IList<Product> products, int customerId)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            foreach (Product product in products)
            {
                context.IncrementInventory(product.upc, customerId, -1 * product.quantity);
            }
        }
    }
}
