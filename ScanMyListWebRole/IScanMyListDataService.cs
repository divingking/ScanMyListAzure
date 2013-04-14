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
        Product GetProductByUPC(string upc, int cid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_name?name={name}&cid={cid}"
        )]
        List<Product> GetProductByName(string name, int cid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_lead_time?upc={upc}&cid={cid}&lead_time={lead_time}"
        )]
        string UpdateLeadTime(string upc, int cid, int lead_time, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_inventory_quantity?upc={upc}&cid={cid}&quantity={quantity}"
        )]
        string UpdateInventoryQuantity(string upc, int cid, int quantity, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_product_location?upc={upc}&cid={cid}&location={location}"
        )]
        string UpdateProductLocation(string upc, int cid, string location, string sessionId);

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
        List<Product> GetInventory(int cid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "suppliers?upc={upc}&cid={cid}"
        )]
        List<Supplier> GetSuppliers(string upc, int cid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "all_suppliers?cid={cid}"
        )]
        List<Supplier> GetAllSuppliers(int cid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_count?upc={upc}"
        )]
        int ProductCount(string upc, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders?cid={cid}"
        )]
        List<Order> GetOrders(int cid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders_by_date?cid={cid}&start={start}&end={end}"
        )]
        List<Order> GetOrdersByDate(int cid, long start, long end, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders_by_range?cid={cid}&start={start}&end={end}"
        )]
        List<Order> GetOrdersByRange(int cid, int start, int end, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "n_orders_from_last?cid={cid}&last={last}&n={n}"
        )]
        List<Order> GetNOrdersFromLast(int cid, int last, int n, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "products_for_order?cid={cid}&oid={oid}"
        )]
        List<Product> GetProductsForOrder(int cid, int oid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "summary?cid={cid}&upc={upc}"
        )]
        string GetProductSummary(int cid, string upc, string sessionId);

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
        string UpdateOrderTitle(int cid, int oid, string title, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "send_order?cid={cid}&oid={oid}"
        )]
        string SendOrder(int cid, int oid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "register"
        )]
        int Register(NewUser user);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "login"
        )]
        string Login(User user);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "logout?cid={cid}&session_id={sessionId}"
        )]
        string Logout(int cid, string sessionId);
    }
}
