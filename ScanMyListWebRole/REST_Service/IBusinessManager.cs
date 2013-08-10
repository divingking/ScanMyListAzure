using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SynchWebRole.REST_Service
{
    [ServiceContract]
    public interface IBusinessManager
    {
        /* deprecated for now
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "suppliers?upc={upc}&bid={bid}&aid={aid}&session={sessionId}"
        )]
        List<Business> GetSuppliers(string upc, int bid, int aid, string sessionId);
        */

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

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "search_customer?bid={bid}&aid={aid}&session={sessionId}&query={query}"
        )]
        List<Business> SearchCustomer(int bid, int aid, string sessionId, string query);
        
        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "search_supplier?bid={bid}&aid={aid}&session={sessionId}&query={query}"
        )]
        List<Business> SearchSupplier(int bid, int aid, string sessionId, string query);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "page_customer?bid={bid}&aid={aid}&session={sessionId}&page_size={pageSize}&offset={offset}"
        )]
        List<Business> PageCustomer(int bid, int aid, string sessionId, int pageSize, int offset);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "page_supplier?bid={bid}&aid={aid}&session={sessionId}&page_size={pageSize}&offset={offset}"
        )]
        List<Business> PageSupplier(int bid, int aid, string sessionId, int pageSize, int offset);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "get_customer_by_id?bid={bid}&aid={aid}&session={sessionId}&cid={cid}"
        )]
        Business GetCustomerById(int bid, int aid, string sessionId, int cid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "get_supplier_by_id?bid={bid}&aid={aid}&session={sessionId}&sid={sid}"
        )]
        Business GetSupplierById(int bid, int aid, string sessionId, int sid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "add_customer?bid={bid}&aid={aid}&session={sessionId}&cid={cid}"
        )]
        void AddCustomer(int bid, int aid, string sessionId, int cid);

        [OperationContract]
        [WebInvoke(
            Method = "GET",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "add_supplier?bid={bid}&aid={aid}&session={sessionId}&sid={sid}"
        )]
        void AddSupplier(int bid, int aid, string sessionId, int sid);
    }
}
