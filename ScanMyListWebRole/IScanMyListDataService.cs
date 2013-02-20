using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ScanMyListWebRole
{
    [ServiceContract]
    public interface IScanMyListDataService
    {
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_upc?upc={upc}&cid={cid}"
        )]
        Product GetProductByUPC(string upc, int cid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_name?name={name}&cid={cid}"
        )]
        List<Product> GetProductByName(string name, int cid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_lead_time?upc={upc}&cid={cid}&lead_time={lead_time}"
        )]
        string UpdateLeadTime(string upc, int cid, int lead_time);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_inventory_quantity?upc={upc}&cid={cid}&quantity={quantity}"
        )]
        string UpdateInventoryQuantity(string upc, int cid, int quantity);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_product_location?upc={upc}&cid={cid}&location={location}"
        )]
        string UpdateProductLocation(string upc, int cid, string location);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "new_product"
        )]
        string NewProduct(Product newProduct);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "inventory?cid={cid}"
        )]
        List<Product> GetInventory(int cid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "suppliers?upc={upc}&cid={cid}"
        )]
        List<Supplier> GetSuppliers(string upc, int cid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "all_suppliers?cid={cid}"
        )]
        List<Supplier> GetAllSuppliers(int cid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_count?upc={upc}"
        )]
        int ProductCount(string upc);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders?cid={cid}"
        )]
        List<Order> GetOrders(int cid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders_by_date?cid={cid}&start={start}&end={end}"
        )]
        List<Order> GetOrdersByDate(int cid, long start, long end);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders_by_range?cid={cid}&start={start}&end={end}"
        )]
        List<Order> GetOrdersByRange(int cid, int start, int end);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "n_orders_from_last?cid={cid}&last={last}&n={n}"
        )]
        List<Order> GetNOrdersFromLast(int cid, int last, int n);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "products_for_order?cid={cid}&oid={oid}"
        )]
        List<Product> GetProductsForOrder(int cid, int oid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "summary?cid={cid}&upc={upc}"
        )]
        string GetProductSummary(int cid, string upc);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "create"
        )]
        string CreateOrder(Order NewOrder);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_title?cid={cid}&oid={oid}&title={title}"
        )]
        string UpdateOrderTitle(int cid, int oid, string title);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "send_order?cid={cid}&oid={oid}"
        )]
        string SendOrder(int cid, int oid);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "login"
        )]
        int Login(User user);
    }
}
