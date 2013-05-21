using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SynchWebRole.REST_Service
{
    [ServiceContract]
    public interface IBusinessManager
    {
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
    }
}
