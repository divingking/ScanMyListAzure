using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net.Http;

using SynchWebRole.Models;

namespace SynchWebRole.ServiceManager
{
    [ServiceContract]
    public interface ISupplierManager
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
        HttpResponseMessage CreateSupplier(int aid, string sessionId, SynchSupplier newSupplier);
        #endregion

        #region GET Request
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "suppliers?accountId={accountId}&sessionId={sessionId}&businessId={businessId}"
        )]
        HttpResponseMessage GetSuppliers(int accountId, string sessionId, int businessId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "suppliers?accountId={accountId}&sessionId={sessionId}&businessId={businessId}&query={query}"
        )]
        HttpResponseMessage GetSuppliers(int accountId, string sessionId, int businessId, string query);

        #endregion
    }
}
