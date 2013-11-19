using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net.Http;

using SynchWebRole.Models;

namespace SynchWebRole.ServiceManager
{
    [ServiceContract]
    public interface ICustomerManager
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
        HttpResponseMessage CreateCustomer(int accountId, string sessionId, int businessId, SynchCustomer newCustomer);
        #endregion

        #region GET Request
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "customers?accountId={accountId}&sessionId={sessionId}&businessId={businessId}"
        )]
        HttpResponseMessage GetCustomers(int accountId, string sessionId, int businessId);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "customers?accountId={accountId}&sessionId={sessionId}&businessId={businessId}&query={query}"
        )]
        HttpResponseMessage GetCustomers(int accountId, string sessionId, int businessId, string query);

        #endregion
    }
}
