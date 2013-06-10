using SynchWebRole.Library_Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;

namespace SynchWebRole.REST_Service
{
    public partial class SynchDataService : IBusinessManager
    {
        public List<Business> GetSuppliers(string upc, int bid, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetSuppliers(upc, bid);
            List<Business> suppliers = new List<Business>();
            foreach (var result in results)
            {
                suppliers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email,
                        price = (double)result.price
                    }
                );
            }
            return suppliers;
        }

        public List<Business> GetAllSuppliers(int bid, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetAllSuppliers(bid);
            List<Business> suppliers = new List<Business>();
            foreach (var result in results)
            {
                suppliers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email
                    }
                );
            }
            return suppliers;
        }

        public List<Business> GetAllCustomers(int bid, int aid, string sessionId)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetAllCustomers(bid);
            List<Business> customers = new List<Business>();
            foreach (var result in results)
            {
                customers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email
                    }
                );
            }
            return customers;
        }


        public List<Business> SearchCustomer(int bid, int aid, string sessionId, string query)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            query = "%" + query + "%";
            var results = context.SearchCustomerOrSupplier(bid, "customer", query);
            List<Business> customers = new List<Business>();
            foreach (SearchCustomerOrSupplierResult result in results)
            {
                customers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email
                    }
                );
            }
            return customers;
        }

        public List<Business> SearchSupplier(int bid, int aid, string sessionId, string query)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            query = "%" + query + "%";
            var results = context.SearchCustomerOrSupplier(bid, "supplier", query);
            List<Business> suppliers = new List<Business>();
            foreach (SearchCustomerOrSupplierResult result in results)
            {
                suppliers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email
                    }
                );
            }
            return suppliers;
        }

        public List<Business> PageCustomer(int bid, int aid, string sessionId, int pageSize, int offset)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.PageBusinessForBusiness(bid, pageSize, offset, "customer");
            List<Business> customers = new List<Business>();
            foreach (PageBusinessForBusinessResult result in results)
            {
                customers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email
                    }
                );
            }
            return customers;
        }

        public List<Business> PageSupplier(int bid, int aid, string sessionId, int pageSize, int offset)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.PageBusinessForBusiness(bid, pageSize, offset, "supplier");
            List<Business> suppliers = new List<Business>();
            foreach (PageBusinessForBusinessResult result in results)
            {
                suppliers.Add(
                    new Business()
                    {
                        id = result.id,
                        name = result.name,
                        address = result.address,
                        zip = (int)result.zip,
                        email = result.email
                    }
                );
            }
            return suppliers;
        }

        public Business GetCustomerById(int bid, int aid, string sessionId, int cid)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetCustomerById(bid, cid);

            IEnumerator<GetCustomerByIdResult> businessEnumerator = result.GetEnumerator();

            if (businessEnumerator.MoveNext())
            {
                GetCustomerByIdResult retrievedBusiness = businessEnumerator.Current;

                return new Business()
                {
                    id = retrievedBusiness.id,
                    name = retrievedBusiness.name,
                    address = retrievedBusiness.address,
                    zip = (int)retrievedBusiness.zip,
                    email = retrievedBusiness.email
                };
            }
            else
            {
                throw new WebFaultException(HttpStatusCode.NoContent);
            }
        }

        public Business GetSupplierById(int bid, int aid, string sessionId, int sid)
        {
            SessionManager.CheckSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetSupplierById(bid, sid);

            IEnumerator<GetSupplierByIdResult> businessEnumerator = result.GetEnumerator();

            if (businessEnumerator.MoveNext())
            {
                GetSupplierByIdResult retrievedBusiness = businessEnumerator.Current;

                return new Business()
                {
                    id = retrievedBusiness.id,
                    name = retrievedBusiness.name,
                    address = retrievedBusiness.address,
                    zip = (int)retrievedBusiness.zip,
                    email = retrievedBusiness.email
                };
            }
            else
            {
                throw new WebFaultException(HttpStatusCode.NoContent);
            }
        }
    
    }
}