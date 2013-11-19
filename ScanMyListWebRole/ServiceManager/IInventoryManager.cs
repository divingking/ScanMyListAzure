using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SynchWebRole.ServiceManager
{
    [ServiceContract]
    public interface IInventoryManager
    {
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
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "new_product"
        )]
        string NewProduct(Product newProduct, int aid, string sessionId);

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

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "search_inventory_by_upc?bid={bid}&aid={aid}&session={sessionId}&query={query}"
        )]
        List<Product> SearchInventoryByUpc(int bid, int aid, string sessionId, string query);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "search_inventory_by_name?bid={bid}&aid={aid}&session={sessionId}&query={query}"
        )]
        List<Product> SearchInventoryByName(int bid, int aid, string sessionId, string query);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "page_inventory?bid={bid}&aid={aid}&session={sessionId}&page_size={pageSize}&offset={offset}"
        )]
        List<Product> PageInventory(int bid, int aid, string sessionId, int pageSize, int offset);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "update_product_upc?bid={bid}&aid={aid}&session={sessionId}&old_upc={old_upc}&new_upc={new_upc}"
        )]
        void UpdateProductUpc(int bid, int aid, string sessionId, string old_upc, string new_upc);
    }
}
