using SynchWebRole.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;

using SynchWebRole.Models;

namespace SynchWebRole.ServiceManager
{
    public partial class SynchDataService : ICustomerManager
    {

        public SynchDataService()
        {
        }

        public HttpResponseMessage CreateCustomer(int accountId, string sessionId, int businessId, SynchCustomer newCustomer)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);

                int customerId = context.CreateBusiness(newCustomer.name, 0, 0, newCustomer.address, newCustomer.postalCode, newCustomer.email, newCustomer.phoneNumber);
                context.CreateCustomer(businessId, customerId, newCustomer.address, newCustomer.email, newCustomer.phoneNumber, newCustomer.category);
                newCustomer.customerId = customerId;
                synchResponse.data = newCustomer;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        public HttpResponseMessage GetCustomers(int accountId, string sessionId, int businessId)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);
                var results = context.GetCustomers(businessId);
                
                List<SynchCustomer> customers = new List<SynchCustomer>();
                foreach (var result in results)
                {
                    customers.Add(
                        new SynchCustomer()
                        {
                            businessId = businessId,
                            customerId = result.customerId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.data = customers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        public HttpResponseMessage GetCustomers(int accountId, string sessionId, int businessId, string query)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);
                query = "%" + query + "%";
                var results = context.GetCustomersLikeName(businessId, query);

                List<SynchCustomer> customers = new List<SynchCustomer>();
                foreach (var result in results)
                {
                    customers.Add(
                        new SynchCustomer()
                        {
                            businessId = businessId,
                            customerId = result.customerId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.data = customers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_CUSTOMER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

    }
}