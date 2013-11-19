using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net.Http;

using SynchWebRole.Models;

namespace SynchWebRole.ServiceManager
{
    [ServiceContract]
    public interface IAccountManager
    {
        #region POST Requests

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "create"
        )]
        HttpResponseMessage CreateAccount(SynchAccount newAccount);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "login"
        )]
        HttpResponseMessage Login(SynchAccount account, int deviceType);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "logout"
        )]
        HttpResponseMessage Logout(SynchAccount account);
        #endregion

        #region PUT Requests
        
        #endregion

        #region GET Requests
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "account?aid={aid}&bid={bid}&session={sessionId}"
        )]
        HttpResponseMessage GetAccount(int aid, int bid, string sessionId);
        #endregion

        #region DELETE Requests
        
        #endregion
    }
}
