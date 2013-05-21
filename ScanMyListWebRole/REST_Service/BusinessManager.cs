using SynchWebRole.Library_Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
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
    }
}