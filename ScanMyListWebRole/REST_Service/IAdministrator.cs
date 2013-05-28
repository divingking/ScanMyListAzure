using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SynchWebRole.REST_Service
{
    [ServiceContract]
    public interface IAdministrator
    {
        #region POST Requests

        /// <summary>
        /// Registers a new business in the database.
        /// </summary>
        /// <param name="business"></param>
        /// <returns></returns>
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
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "login"
        )]
        User Login(LoginUser user);
        #endregion

        #region PUT Requests
        
        #endregion

        #region GET Requests
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
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "logout?aid={aid}&session={sessionId}"
        )]
        string Logout(int aid, string sessionId);
        #endregion

        #region DELETE Requests
        
        #endregion
    }
}
