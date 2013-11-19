namespace SynchWebRole.ServiceManager
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;


    [ServiceContract]
    public interface ISynchDataService : IAccountManager, IBusinessManager, IInventoryManager, IRecordManager
    {

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "count_item?bid={bid}&aid={aid}&session={sessionId}&item={item}"
        )]
        int CountItemForBusiness(int bid, int aid, string sessionId, string item);

        /*
       // Administration Part
        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "register_business"
        )]
        int RegisterBusiness(Business business);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "register_account"
        )]
        User RegisterAccount(User user);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "business?name={name}&aid={aid}&session={sessionId}"
        )]
        List<Business> SearchBusinessByName(string name, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "link?bid={bid}&aid={aid}&session={sessionId}"
        )]
        string LinkAccountToBusiness(int bid, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "login"
        )]
        User Login(LoginUser user);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "logout?aid={aid}&session={sessionId}"
        )]
        string Logout(int aid, string sessionId);

        // End of Administration Part

        // Business Management Part
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "suppliers?upc={upc}&bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<Business> GetSuppliers(string upc, int bid, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "all_suppliers?bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<Business> GetAllSuppliers(int bid, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "all_customers?bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<Business> GetAllCustomers(int bid, int aid, string sessionId);

        // End of Business Management Part


        // Inventory Management Part
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_upc?upc={upc}&bid={bid}&aid={aid}&session={sessionId}"
        )]
        Product GetProductByUPC(string upc, int bid, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_name?name={name}&bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<Product> GetProductByName(string name, int bid, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_lead_time?upc={upc}&bid={bid}&lead_time={lead_time}&aid={aid}&session={sessionId}"
        )]
        string UpdateLeadTime(string upc, int bid, int lead_time, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "update_product_location?upc={upc}&bid={bid}&location={location}&aid={aid}&session={sessionId}"
        )]
        string UpdateProductLocation(string upc, int bid, string location, int aid, string sessionId);

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
            UriTemplate = "inventory?bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<Product> GetInventory(int bid, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "summary_for_business?bid={bid}&upc={upc}&aid={aid}&other_bid={other_bid}&session={sessionId}"
        )]
        string GetProductSummaryForBusiness(int bid, string upc, int aid, int other_bid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "summary?bid={bid}&upc={upc}&aid={aid}&session={sessionId}"
        )]
        string GetProductSummary(int bid, string upc, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "summary_data?bid={bid}&upc={upc}&aid={aid}&session={sessionId}"
        )]
        List<RecordProduct> GetProductSummaryData(int bid, string upc, int aid, string sessionId);

        // End of Inventory Management Part

        // Record Management Part
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders?bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<Record> GetOrders(int bid, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "receipts?bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<Record> GetReceipts(int bid, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "records_by_date?bid={bid}&start={start}&end={end}&aid={aid}&session={sessionId}"
        )]
        List<Record> GetRecordsByDate(int bid, long start, long end, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders_by_range?bid={bid}&start={start}&end={end}&aid={aid}&session={sessionId}"
        )]
        List<Order> GetOrdersByRange(int bid, int start, int end, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "n_records_from_last?bid={bid}&last={last}&n={n}&aid={aid}&session={sessionId}"
        )]
        List<Record> GetNRecordsFromLast(int bid, int last, int n, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "products_for_Record?bid={bid}&rid={rid}&aid={aid}&session={sessionId}"
        )]
        List<RecordProduct> GetRecordDetails(int bid, int rid, int aid, string sessionId);

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
            UriTemplate = "update_title?bid={bid}&rid={rid}&title={title}&aid={aid}&session={sessionId}"
        )]
        string UpdateRecordTitle(int bid, int rid, string title, int aid, string sessionId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "send_record?bid={bid}&oid={oid}&aid={aid}&session={sessionId}"
        )]
        string SendRecord(int bid, int oid, int aid, string sessionId);

        // End of Record Management Part

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "product_count?upc={upc}&aid={aid}&session={sessionId}"
        )]
        int ProductCount(string upc, int aid, string sessionId);
        */
        
    }
}
