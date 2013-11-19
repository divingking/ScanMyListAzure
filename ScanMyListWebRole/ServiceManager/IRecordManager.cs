using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net.Http;

using SynchWebRole.Models;

namespace SynchWebRole.ServiceManager
{
    [ServiceContract]
    public interface IRecordManager
    {
        #region POST Request
        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "create"
        )]
        HttpResponseMessage CreateRecord(int accountId, string sessionId, SynchRecord newRecord);
        #endregion

        #region PUT Request
        [OperationContract]
        [WebInvoke(
            Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "present"
        )]
        HttpResponseMessage PresentRecord(int accountId, string sessionId, int recordId);

        [OperationContract]
        [WebInvoke(
            Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "send"
        )]
        HttpResponseMessage SendRecord(int accountId, string sessionId, int recordId);

        [OperationContract]
        [WebInvoke(
            Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "close"
        )]
        HttpResponseMessage CloseRecord(int accountId, string sessionId, int recordId);

        [OperationContract]
        [WebInvoke(
            Method = "PUT",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "update"
        )]
        HttpResponseMessage UpdateRecord(int accountId, string sessionId, SynchRecord updatedRecord);
        #endregion

        #region GET Request
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "orders?accountid={accountId}&sessionid={sessionId}&businessid={businessId}"
        )]
        HttpResponseMessage GetOrders(int accountId, string sessionId, int businessId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "receipts?accountid={accountId}&sessionid={sessionId}&businessid={businessId}"
        )]
        HttpResponseMessage GetReceipts(int accountId, string sessionId, int businessId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "categories?accountid={accountId}&sessionid={sessionId}&businessid={businessId}"
        )]
        HttpResponseMessage GetRecordCategoryList(int accountId, string sessionId, int businessId);

        #endregion
    }
}
