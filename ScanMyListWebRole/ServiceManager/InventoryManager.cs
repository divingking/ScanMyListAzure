using SynchWebRole.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;

namespace SynchWebRole.ServiceManager
{
    public partial class SynchDataService : IInventoryManager
    {
        public Product GetProductByUPC(string upc, int bid, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetInventoryByUpc(bid, upc);
            IEnumerator<GetInventoryByUpcResult> productEnumerator = results.GetEnumerator();
            if (productEnumerator.MoveNext())
            {
                GetInventoryByUpcResult target = productEnumerator.Current;
                return new Product()
                {
                    upc = target.upc,
                    name = target.name,
                    detail = target.detail,
                    quantity = (int)target.quantity,
                    location = target.location,
                    owner = bid,
                    leadTime = (int)target.lead_time,
                    price = (double)target.default_price
                };
            }
            else
            {
                throw new WebFaultException<string>("Product with given UPC not found in your Inventory", HttpStatusCode.NotFound);
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
            SessionManager.checkSession(aid, sessionId);

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
                        leadTime = (int)result.lead_time,
                        price = (double)result.default_price
                    }
                );
            }
            return products;
        }

        public string UpdateLeadTime(string upc, int bid, int lead_time, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

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
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            if (context.HasInventory(bid, upc) == 0)
            {
                throw new FaultException(
                    string.Format("This item {0} is not in your inventory!", upc));
            }
            else
            {
                context.UpdateInventoryLocation(bid, upc, location);
                // update location in ERP system
                //ERPIntegrator.relayInventoryManagement(bid, upc, "01");
            }
            return string.Format("Product location for {0} of {1} updated", bid, upc);
        }

        public string NewProduct(Product newProduct, int aid, string sessionId)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            SessionManager.checkSession(aid, sessionId);

            var result = context.GetProductByUpc(newProduct.upc);
            if (result.GetEnumerator().MoveNext())
            {
                throw new WebFaultException<string>("a product with the same UPC already exists", HttpStatusCode.Conflict);
            }
            else
            {
                context.CreateProduct(newProduct.upc, newProduct.name, newProduct.detail);
                context.CreateInventory(newProduct.owner, newProduct.upc, newProduct.location, newProduct.quantity, newProduct.leadTime, newProduct.price, newProduct.productCategory);
                ERPIntegrator.relayInventoryManagement(newProduct.owner, newProduct.upc, "00");
                return "New product created.";
            }
        }

        public List<Product> SearchInventoryByUpc(int bid, int aid, string sessionId, string query)
        {
            SessionManager.checkSession(aid, sessionId);
            query = "%" + query + "%";

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.SearchInventory(bid, "upc", query);
            List<Product> inventory = new List<Product>();

            foreach (SearchInventoryResult p in results)
            {
                Product product = new Product()
                {
                    upc = p.upc,
                    name = p.name,
                    detail = p.detail,
                    leadTime = (int)p.lead_time,
                    quantity = (int)p.quantity,
                    location = p.location,
                    owner = bid,
                    price = (double)p.default_price
                };

                inventory.Add(product);
            }

            return inventory;
        }

        public List<Product> SearchInventoryByName(int bid, int aid, string sessionId, string query)
        {
            SessionManager.checkSession(aid, sessionId);
            query = "%" + query + "%";

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.SearchInventory(bid, "name", query);
            List<Product> inventory = new List<Product>();

            foreach (SearchInventoryResult p in results)
            {
                Product product = new Product()
                {
                    upc = p.upc,
                    name = p.name,
                    detail = p.detail,
                    leadTime = (int)p.lead_time,
                    quantity = (int)p.quantity,
                    location = p.location,
                    owner = bid,
                    price = (double)p.default_price
                };

                inventory.Add(product);
            }

            return inventory;
        }

        public List<Product> PageInventory(int bid, int aid, string sessionId, int pageSize, int offset)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.PageInventoryForBusiness(bid, pageSize, offset);
            List<Product> inventory = new List<Product>();

            foreach (PageInventoryForBusinessResult p in results)
            {
                Product product = new Product()
                {
                    upc = p.upc,
                    name = p.name,
                    detail = p.detail,
                    leadTime = (int)p.lead_time,
                    quantity = (int)p.quantity,
                    location = p.location,
                    owner = bid,
                    price = (double)p.default_price
                };

                inventory.Add(product);
            }

            return inventory;
        }

        
        public List<Product> GetInventory(int bid, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

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
                    owner = bid,
                    price = (double)p.default_price
                };

                inventory.Add(product);
            }

            return inventory;
        }

        public string GetProductSummaryForBusiness(int bid, string upc, int aid, int other_bid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

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
                if ((int)enumerator.Current.customer == other_bid || (int)enumerator.Current.supplier == other_bid)
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

            }

            if (orderCount <= 1)
                throw new WebFaultException(HttpStatusCode.NoContent);

            double days = (start == end) ? 1.0 : GetDays(start, end);

            return string.Format("{0} {1}", (double)(productCount - lastProductCount) / days, days / (double)orderCount);
        }

        public string GetProductSummary(int bid, string upc, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

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

            if (start == -1 || end == -1)
                return "This product does not have any summary information.";

            double days = GetDays(start, end);

            return string.Format("{0} {1}", (double)(productCount - lastProductCount) / days, days / (double)orderCount);
        }

        public List<RecordProduct> GetProductSummaryData(int bid, string upc, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

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

        public void UpdateProductUpc(int bid, int aid, string sessionId, string old_upc, string new_upc)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            context.UpdateProductUpc(new_upc, old_upc);
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

        public void IncrementInventories(IList<RecordProduct> products, int business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            foreach (RecordProduct product in products)
            {
                context.IncrementInventory(business, product.upc, product.quantity);
            }
        }

        public void DecrementInventories(IList<RecordProduct> products, int business)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();

            foreach (RecordProduct product in products)
            {
                context.IncrementInventory(business, product.upc, -1 * product.quantity);
            }
        }
    }

}