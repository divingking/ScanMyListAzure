using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace ScanMyListWebRole
{
    public class ScanMyListDataService : IScanMyListDataService
    {
        public Product GetProductByUPC(string upc, int cid)
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

        public List<Product> GetProductByName(string name, int cid)
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

        public string UpdateLeadTime(string upc, int cid, int lead_time)
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

        public string UpdateInventoryQuantity(string upc, int cid, int quantity)
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

        public string UpdateProductLocation(string upc, int cid, string location)
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

        public string NewProduct(Product newProduct)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            context.AddProduct(newProduct.upc, newProduct.name, newProduct.detail);
            context.InitializeInventory(newProduct.upc, newProduct.owner, newProduct.leadTime, newProduct.quantity, newProduct.location);
            return "New product created. ";
        }

        public List<Product> GetInventory(int cid)
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

        public List<Supplier> GetSuppliers(string upc, int cid)
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

        public List<Supplier> GetAllSuppliers(int cid)
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

        public int ProductCount(string upc)
        {
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

        public List<Order> GetOrders(int cid)
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

        public List<Order> GetOrdersByDate(int cid, long start, long end)
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

        public List<Order> GetOrdersByRange(int cid, int start, int end)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetOrdersByRange(cid, start, end);
            List<Order> orders = new List<Order>();
            foreach (var result in results)
            {
                orders.Add(new Order()
                {
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

        public List<Order> GetNOrdersFromLast(int cid, int last, int n)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetNOrdersFromLast(cid, last, n);
            List<Order> orders = new List<Order>();
            foreach (var result in results)
            {
                orders.Add(new Order()
                {
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

        public List<Product> GetProductsForOrder(int cid, int oid)
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

        public string GetProductSummary(int cid, string upc)
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

        public string CreateOrder(Order NewOrder)
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
                    return SendOrder(NewOrder.cid, NewOrder.oid);
                }
                else if (NewOrder.sent)
                {
                    return "Order without supplier specified cannot be sent! Stored as unsent instead";
                }

                return "New Order created! ";
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
                        return "Sent order cannot be modified!";
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
                        if (NewOrder.products != null || NewOrder.products.Count != 0)
                        {
                            context.ClearProductFromOrder(NewOrder.oid);
                            
                            foreach (Product current in NewOrder.products)
                            {
                                if (current.supplier == null)
                                {
                                    context.AddProductToOrder(NewOrder.oid, current.upc, null, current.quantity);
                                }
                                else
                                {
                                    context.AddProductToOrder(NewOrder.oid, current.upc, current.supplier.id, current.quantity);
                                }
                            }
                        }

                        return "Order updated! ";
                    }
                }
                else
                {
                    return "The order you requested for update does not exist! ";
                }
            }
        }

        public string UpdateOrderTitle(int cid, int oid, string title)
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

        public string SendOrder(int cid, int oid)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetOrder(cid, oid);
            Order order = new Order()
            {
                cid = cid,
                oid = oid,
                date = 0,
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
                context.SendOrder(cid, oid);
                if (!MailHelper.SendOrderBackup(order))
                    return "Fail to send system confirmation mail!";
                if (!MailHelper.SendOrder(order, cid))
                    return "Fail to send confirmation mail!";
                return "Order sent!";
            }
        }

        public int Login(User user)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.Login(user.login, user.pass);
            IEnumerator<LoginResult> loggedIn = results.GetEnumerator();
            if (loggedIn.MoveNext())
            {
                return loggedIn.Current.id;
            }
            else
            {
                return -1;
            }
        }
    }
}
