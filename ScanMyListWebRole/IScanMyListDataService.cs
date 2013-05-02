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
            UriTemplate = "product_upc?upc={upc}&bid={bid}"
        )]
        Product GetProductByUPC(string upc, int bid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_name?name={name}&bid={bid}"
        )]
        List<Product> GetProductByName(string name, int bid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_lead_time?upc={upc}&bid={bid}&lead_time={lead_time}"
        )]
        string UpdateLeadTime(string upc, int bid, int lead_time, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_inventory_quantity?upc={upc}&bid={bid}&quantity={quantity}"
        )]
        string UpdateInventoryQuantity(string upc, int bid, int quantity, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_product_location?upc={upc}&bid={bid}&location={location}"
        )]
        string UpdateProductLocation(string upc, int bid, string location, string sessionId);

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
            UriTemplate = "inventory?bid={bid}"
        )]
        List<Product> GetInventory(int bid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "suppliers?upc={upc}&bid={bid}"
        )]
        List<Business> GetSuppliers(string upc, int bid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "all_suppliers?bid={bid}"
        )]
        List<Business> GetAllSuppliers(int bid, string sessionId);

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
            UriTemplate = "orders?bid={bid}"
        )]
        List<Order> GetOrders(int bid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "receipts?bid={bid}"
        )]
        List<Order> GetReceipts(int bid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders_by_date?bid={bid}&start={start}&end={end}"
        )]
        List<Order> GetOrdersByDate(int bid, long start, long end, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders_by_range?bid={bid}&start={start}&end={end}"
        )]
        List<Order> GetOrdersByRange(int bid, int start, int end, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "n_orders_from_last?bid={bid}&last={last}&n={n}"
        )]
        List<Order> GetNOrdersFromLast(int bid, int last, int n, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "products_for_Record?bid={bid}&rid={rid}&session={sessionId}"
        )]
        List<RecordProduct> GetRecordDetails(int bid, int rid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "summary?bid={bid}&upc={upc}&session={sessionId}"
        )]
        string GetProductSummary(int bid, string upc, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "summary_data?bid={bid}&upc={upc}&session={sessionId}"
        )]
        List<RecordProduct> GetProductSummaryData(int bid, string upc, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "create"
        )]
        string CreateRecord(Record newRecord);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_title?bid={bid}&rid={rid}&title={title}&session={sessionId}"
        )]
        string UpdateRecordTitle(int bid, int rid, string title, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "send_order?bid={bid}&oid={oid}&session={sessionId}"
        )]
        string SendOrder(int bid, int oid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "register_business"
        )]
        int RegisterBusiness(NewUser user);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "register_account"
        )]
        int RegisterAccount(NewUser user);

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
            UriTemplate = "logout?bid={bid}&session={sessionId}"
        )]
        string Logout(int bid, string sessionId);
    }
}
