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
    public partial class SynchDataService : ISupplierManager
    {

        public SynchDataService()
        {
        }

        public HttpResponseMessage CreateSupplier(int accountId, string sessionId, int businessId, SynchSupplier newSupplier)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);

                int supplierId = context.CreateBusiness(newSupplier.name, 0, 0, newSupplier.address, newSupplier.postalCode, newSupplier.email, newSupplier.phoneNumber);
                context.CreateSupplier(businessId, supplierId, newSupplier.address, newSupplier.email, newSupplier.phoneNumber, newSupplier.category);
                newSupplier.supplierId = supplierId;
                synchResponse.data = newSupplier;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        public HttpResponseMessage GetSuppliers(int accountId, string sessionId, int businessId)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);
                var results = context.GetSuppliers(businessId);

                List<SynchSupplier> suppliers = new List<SynchSupplier>();
                foreach (var result in results)
                {
                    suppliers.Add(
                        new SynchSupplier()
                        {
                            businessId = businessId,
                            supplierId = result.supplierId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.data = suppliers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        public HttpResponseMessage GetSuppliers(int accountId, string sessionId, int businessId, string query)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);
                query = "%" + query + "%";
                var results = context.GetSuppliersLikeName(businessId, query);

                List<SynchSupplier> suppliers = new List<SynchSupplier>();
                foreach (var result in results)
                {
                    suppliers.Add(
                        new SynchSupplier()
                        {
                            businessId = businessId,
                            supplierId = result.supplierId,
                            name = result.name,
                            address = result.address,
                            email = result.email,
                            postalCode = result.postalCode,
                            phoneNumber = result.phoneNumber,
                            category = result.category
                        }
                    );
                }

                synchResponse.data = suppliers;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_SUPPLIER, e.Message);
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