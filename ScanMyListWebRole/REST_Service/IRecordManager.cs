using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SynchWebRole.REST_Service
{
    [ServiceContract]
    public interface IRecordManager
    {
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
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "record_category_list?bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<string> GetRecordCategoryList(int bid, int aid, string sessionId);

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

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "search_record_by_title?bid={bid}&aid={aid}&session={sessionId}&query={query}"
        )]
        List<Record> SearchRecordByTitle(int bid, int aid, string sessionId, string query);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "page_record?bid={bid}&aid={aid}&session={sessionId}&page_size={pageSize}&offset={offset}"
        )]
        List<Record> PageRecord(int bid, int aid, string sessionId, int pageSize, int offset);
    }
}
