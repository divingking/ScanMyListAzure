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
    public partial class SynchDataService : IRecordManager
    {
        public HttpResponseMessage CreateRecord(int accountId, string sessionId, SynchRecord newRecord)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);

                // ignore client-sent data; server uses superior logic to overwrite
                // transaction date and order status to prevent data corruption
                newRecord.transactionDate = DateTimeOffset.Now;
                newRecord.status = (int)RecordStatus.created;

                int recordId = context.CreateRecord(
                    newRecord.category,
                    newRecord.accountId,
                    newRecord.ownerId,
                    newRecord.clientId,
                    newRecord.status,
                    newRecord.title,
                    newRecord.comment,
                    newRecord.transactionDate,
                    newRecord.deliveryDate);

                if (recordId < 0)
                    throw new WebFaultException<string>("unable to create record", HttpStatusCode.BadRequest);

                foreach (SynchRecordLine recordLine in newRecord.recordLines)
                {
                    context.CreateRecordLine(recordId, recordLine.upc, recordLine.quantity, recordLine.price, recordLine.note);
                }

                newRecord.id = recordId;
                synchResponse.data = newRecord;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_POST, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        public string SendRecord(int bid, int rid, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetRecordById(bid, rid);
            IEnumerator<GetRecordByIdResult> enumerator = result.GetEnumerator();

            if (enumerator.MoveNext())
            {
                TierController.validateAccessToRecord((int)enumerator.Current.account, (int)enumerator.Current.business, aid, bid);
                string res = "";
                switch (enumerator.Current.category)
                {
                    case (int)RecordCategory.Order:
                        res = this.SendOrder(context, bid, rid, aid);
                        // check with database to see if we need to relay this new record to ERP system
                        ERPIntegrator.relayRecordManagement(bid, rid);
                        return res;
                    case (int)RecordCategory.Receipt:
                        res = this.SendReceipt(context, bid, rid, aid);
                        // check with database to see if we need to relay this new record to ERP system
                        ERPIntegrator.relayRecordManagement(bid, rid);
                        return res;
                    default:
                        res = this.SendChange(context, bid, rid, aid);
                        // check with database to see if we need to relay this new record to ERP system
                        ERPIntegrator.relayRecordManagement(bid, rid);
                        return res;
                }
            }
            else
            {
                throw new FaultException(
                    string.Format("Failed to find Record with id {0}! ", rid));
            }
        }

        private string SendOrder(ScanMyListDatabaseDataContext context, int bid, int rid, int aid)
        {
            var result = context.GetCompleteOrder(bid, rid);

            Record overallOrder = new Record() { date = -1, products = new List<RecordProduct>() };
            IDictionary<int, Record> orders = new Dictionary<int, Record>();
            IDictionary<int, Business> customers = new Dictionary<int, Business>();

            foreach (var product in result)
            {
                if (overallOrder.date == -1)
                {
                    overallOrder.id = product.record_id;
                    overallOrder.title = product.record_title;
                    overallOrder.date = (long)product.record_date;
                    overallOrder.category = (int)RecordCategory.Order;
                    overallOrder.comment = product.record_comment;
                }

                if (!customers.ContainsKey(product.customer_id))
                {
                    customers.Add(
                        product.customer_id,
                        new Business()
                        {
                            id = product.customer_id,
                            name = product.customer_name,
                            email = product.customer_email
                        });

                    orders.Add(
                        product.customer_id,
                        new Record()
                        {
                            id = product.record_id,
                            title = product.record_title,
                            date = (long)product.record_date,
                            category = (int)RecordCategory.Order,
                            products = new List<RecordProduct>()
                        });
                }

                RecordProduct p = new RecordProduct()
                {
                    upc = product.product_upc,
                    name = product.product_name,
                    quantity = (int)product.product_quantity,
                    customer = product.customer_id
                };

                overallOrder.products.Add(p);
                orders[product.customer_id].products.Add(p);
            }

            User account = null;
            var results = context.GetAccountById(aid);
            IEnumerator<GetAccountByIdResult> accountEnumerator = results.GetEnumerator();
            if (accountEnumerator.MoveNext())
            {
                GetAccountByIdResult getAccountResult = accountEnumerator.Current;
                account = new User()
                {
                    email = getAccountResult.email,
                    login = getAccountResult.login
                };
            }

            Business business = this.FetchBusiness(context, bid);

            customers.Add(business.id, business);

            /*
            if (!MailHelper.SendRecord(bid, overallOrder, customers))
                throw new FaultException("Failed to send confirmation email! ");
            if (!MailHelper.SendRecordBackup(accountEmail, bid, overallOrder, customers))
                throw new FaultException("Failed to send system backup confirmatoin email! ");
            foreach (int customer in orders.Keys)
            {
                if (!MailHelper.SendRecord(customer, orders[customer], customers))
                    throw new FaultException(
                        string.Format("Failed to send confirmation email to Customer {0}! ", customers[customer].name));
            }
            */

            // silent fail for now
            MailHelper.SendRecord(bid, overallOrder, customers);
            MailHelper.SendRecordBackup(account, bid, overallOrder, customers);

            this.DecrementInventories(overallOrder.products, bid);

            return string.Format("Order with id {0} sent! ", rid);
        }

        private string SendReceipt(ScanMyListDatabaseDataContext context, int bid, int rid, int aid)
        {
            var result = context.GetCompleteReceipt(bid, rid);

            Record overallReceipt = new Record() { date = -1, products = new List<RecordProduct>() };
            IDictionary<int, Record> receipts = new Dictionary<int, Record>();
            IDictionary<int, Business> suppliers = new Dictionary<int, Business>();

            foreach (var product in result)
            {
                if (overallReceipt.date == -1)
                {
                    overallReceipt.id = product.record_id;
                    overallReceipt.title = product.record_title;
                    overallReceipt.date = (long)product.record_date;
                    overallReceipt.category = (int)RecordCategory.Receipt;
                    overallReceipt.comment = product.record_comment;
                }

                if (!suppliers.ContainsKey(product.supplier_id))
                {
                    suppliers.Add(
                        product.supplier_id,
                        new Business()
                        {
                            id = product.supplier_id,
                            name = product.supplier_name,
                            email = product.supplier_email
                        });

                    receipts.Add(
                        product.supplier_id,
                        new Record()
                        {
                            id = product.record_id,
                            title = product.record_title,
                            date = (long)product.record_date,
                            category = (int)RecordCategory.Receipt,
                            products = new List<RecordProduct>()
                        });
                }

                RecordProduct p = new RecordProduct()
                {
                    upc = product.product_upc,
                    name = product.product_name,
                    quantity = (int)product.product_quantity,
                    supplier = product.supplier_id
                };

                overallReceipt.products.Add(p);
                receipts[product.supplier_id].products.Add(p);
            }

            User account = null;
            var results = context.GetAccountById(aid);
            IEnumerator<GetAccountByIdResult> accountEnumerator = results.GetEnumerator();
            if (accountEnumerator.MoveNext())
            {
                GetAccountByIdResult getAccountResult = accountEnumerator.Current;
                account = new User()
                {
                    email = getAccountResult.email,
                    login = getAccountResult.login
                };
            }

            Business business = this.FetchBusiness(context, bid);

            suppliers.Add(business.id, business);

            if (!MailHelper.SendRecord(bid, overallReceipt, suppliers))
                throw new FaultException("Failed to send confirmation email! ");
            if (!MailHelper.SendRecordBackup(account, bid, overallReceipt, suppliers))
                throw new FaultException("Failed to send system backup confirmatoin email! ");

            foreach (int supplier in receipts.Keys)
            {
                if (!MailHelper.SendRecord(supplier, receipts[supplier], suppliers))
                    throw new FaultException(
                        string.Format("Failed to send confirmation email to Supplier {0}! ", suppliers[supplier].name));
            }

            this.IncrementInventories(overallReceipt.products, bid);

            return string.Format("Receipt with id {0} sent! ", rid);
        }

        private string SendChange(ScanMyListDatabaseDataContext context, int bid, int rid, int aid)
        {
            var result = context.GetCompleteChange(bid, rid);

            Record change = new Record() { date = -1, products = new List<RecordProduct>() };
            IDictionary<int, Business> self = new Dictionary<int, Business>();

            foreach (var product in result)
            {
                if (change.date == -1)
                {
                    change.id = product.record_id;
                    change.title = product.record_title;
                    change.date = (long)product.record_date;
                    change.category = (int)product.record_category;
                }

                RecordProduct p = new RecordProduct()
                {
                    upc = product.product_upc,
                    name = product.product_name,
                    quantity = (int)product.product_quantity
                };

                change.products.Add(p);
            }

            string accountEmail = this.FetchAccountEmail(context, aid);
            Business business = this.FetchBusiness(context, bid);

            self.Add(business.id, business);

            if (!MailHelper.SendRecord(bid, change, self))
                throw new FaultException("Failed to send confirmation email! ");
            /*  we don't need to send backup for change
            if (MailHelper.SendRecordBackup(bid, change, self))
                throw new FaultException("Failed to send system backup confirmatoin email! ");
            */
            this.IncrementInventories(change.products, bid);

            return string.Format("Inventory change with id {0} sent! ", rid);
        }

        public HttpResponseMessage GetOrders(int accountId, string sessionId, int businessId)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);
                List<SynchRecord> orders = new List<SynchRecord>();
                var results = context.GetRecords(businessId);

                foreach (var result in results)
                {
                    if (result.category == (int)RecordCategory.Order)
                    {
                        orders.Add(
                            new SynchRecord()
                            {
                                id = result.id,
                                accountId = result.accountId,
                                category = result.category,
                                ownerId = result.ownerId,
                                clientId = result.clientId,
                                comment = result.comment,
                                status = result.status,
                                title = result.title,
                                transactionDate = result.transactionDate,
                                deliveryDate = result.deliveryDate,
                                recordLines = null
                            }
                        );
                    }
                }

                synchResponse.data = TierController.filterRecordWithAccountTier(context, orders, accountId);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        public HttpResponseMessage GetReceipts(int accountId, string sessionId, int businessId)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);
                List<SynchRecord> receipts = new List<SynchRecord>();
                var results = context.GetRecords(businessId);

                foreach (var result in results)
                {
                    if (result.category == (int)RecordCategory.Receipt)
                    {
                        receipts.Add(
                            new SynchRecord()
                            {
                                id = result.id,
                                accountId = result.accountId,
                                category = result.category,
                                ownerId = result.ownerId,
                                clientId = result.clientId,
                                comment = result.comment,
                                status = result.status,
                                title = result.title,
                                transactionDate = result.transactionDate,
                                deliveryDate = result.deliveryDate,
                                recordLines = null
                            }
                        );
                    }
                }

                synchResponse.data = TierController.filterRecordWithAccountTier(context, receipts, accountId);
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        /*
        public List<Record> PageRecord(int bid, int aid, string sessionId, int pageSize, int offset)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            List<Record> records = TierController.pageRecordForBusinessWithAccount(bid, aid, offset, pageSize);
            return records;
        }

       
        public List<RecordProduct> GetRecordDetails(int bid, int rid, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var record = context.GetRecordById(bid, rid);
            IEnumerator<GetRecordByIdResult> recordEnumerator = record.GetEnumerator();

            if (recordEnumerator.MoveNext())
            {
                TierController.validateAccessToRecord((int)recordEnumerator.Current.account, (int)recordEnumerator.Current.business, aid, bid);
                int category = (int)recordEnumerator.Current.category;

                switch (category)
                {
                    case (int)RecordCategory.Order:
                        return GetOrderDetails(context, bid, rid);
                    case (int)RecordCategory.Receipt:
                        return GetReceiptDetails(context, bid, rid);
                    default:
                        return GetChangeDetails(context, bid, rid);

                }
            }
            else
            {
                throw new WebFaultException<string>(string.Format("Record with id {0} is not found! ", rid), HttpStatusCode.NotFound);
            }
        }

        private List<RecordProduct> GetOrderDetails(ScanMyListDatabaseDataContext context, int bid, int rid)
        {
            List<RecordProduct> products = new List<RecordProduct>();

            var results = context.GetCompleteOrder(bid, rid);

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        upc = product.product_upc,
                        customer = product.customer_id,
                        quantity = (int)product.product_quantity,
                        price = (double)product.product_price,
                        note = product.product_note
                    });
            }

            return products;
        }

        private List<RecordProduct> GetReceiptDetails(ScanMyListDatabaseDataContext context, int bid, int rid)
        {
            List<RecordProduct> products = new List<RecordProduct>();

            var results = context.GetCompleteReceipt(bid, rid);

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        upc = product.product_upc,
                        supplier = product.supplier_id,
                        quantity = (int)product.product_quantity,
                        price = (double)product.product_price,
                        note = product.product_note
                    });
            }

            return products;
        }

        private List<RecordProduct> GetChangeDetails(ScanMyListDatabaseDataContext context, int bid, int rid)
        {
            List<RecordProduct> products = new List<RecordProduct>();

            var results = context.GetCompleteChange(bid, rid);

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        upc = product.product_upc,
                        quantity = (int)product.product_quantity
                    });
            }

            return products;
        }
        */

        public HttpResponseMessage GetRecordCategoryList(int accountId, string sessionId, int businessId)
        {
            HttpResponseMessage response;
            SynchHttpResponseMessage synchResponse = new SynchHttpResponseMessage();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                SessionManager.checkSession(context, accountId, sessionId);
                List<string> categories = new List<string>();
                foreach (string currentCategory in Enum.GetNames(typeof(RecordCategory)))
                {
                    categories.Add(currentCategory);
                }

                synchResponse.data = categories;
                synchResponse.status = HttpStatusCode.OK;
            }
            catch (WebFaultException e)
            {
                synchResponse.status = e.StatusCode;
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            catch (Exception e)
            {
                synchResponse.error = new SynchError(SynchError.SynchErrorCode.ACTION_GET, SynchError.SynchErrorCode.SERVICE_RECORD, e.Message);
            }
            finally
            {
                response = Request.CreateResponse<SynchHttpResponseMessage>(synchResponse.status, synchResponse);
                context.Dispose();
            }

            return response;
        }

        /*
        public string CreateRecord(Record newRecord)
        {
            SessionManager.checkSession(newRecord.account, newRecord.sessionId);

            this.ValidateRecord(newRecord);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            if (newRecord.id == -1)
            {
                // Create a new record
                int rid = context.CreateRecord(
                        newRecord.business, newRecord.title, newRecord.category, newRecord.status, newRecord.comment, newRecord.account);

                foreach (RecordProduct product in newRecord.products)
                {
                    context.CreateProductInRecord(
                        rid, product.upc, product.supplier, product.customer, product.quantity, product.note, product.price);
                }

                if (newRecord.category != (int)RecordCategory.Order && newRecord.category != (int)RecordCategory.Receipt)
                {
                    this.IncrementInventories(newRecord.products, newRecord.business);
                    return string.Format("Inventory changed by Record {0}", newRecord.id);
                }
                else
                {
                    if (newRecord.status == (int)RecordStatus.sent)
                    {
                        string result = string.Format(
                            "{0} id=_{1}", this.SendRecord(newRecord.business, rid, newRecord.account, newRecord.sessionId), rid);
                        return result;
                    }
                    else
                    {
                        return string.Format("New Record saved! id=_{0}", rid);
                    }
                }

            }
            else
            {
                // Update an existing record
                var result = context.GetRecordById(newRecord.business, newRecord.id);
                IEnumerator<GetRecordByIdResult> recordEnumerator = result.GetEnumerator();

                if (recordEnumerator.MoveNext())
                {
                    GetRecordByIdResult retrievedRecord = recordEnumerator.Current;

                    TierController.validateAccessToRecord((int)retrievedRecord.account, (int)retrievedRecord.business, newRecord.account, newRecord.business);

                    if (retrievedRecord.status != (int)RecordStatus.saved)
                    {
                        throw new FaultException("Sent or Closed Record cannot be modified! ");
                    }
                    else
                    {
                        // Update Record's title
                        if (!newRecord.title.Equals(retrievedRecord.title))
                        {
                            context.UpdateRecordTitle(newRecord.business, newRecord.id, newRecord.title);
                        }

                        // Update Record's products
                        if (newRecord.products != null || newRecord.products.Count != 0)
                        {
                            context.DeleteProductsInRecord(newRecord.id);

                            foreach (RecordProduct product in newRecord.products)
                            {
                                context.CreateProductInRecord(
                                    newRecord.id, product.upc, product.supplier, product.customer, product.quantity, product.note, product.price);
                            }
                        }

                        if (newRecord.status == (int)RecordStatus.sent)
                        {
                            return string.Format(
                                "{0} id=_{1}",
                                this.SendRecord(newRecord.business, newRecord.id, newRecord.account, newRecord.sessionId), newRecord.id);
                        }
                        else
                        {
                            return string.Format("New Record saved! id=_{0}", newRecord.id);
                        }
                    }
                }
                else
                {
                    throw new FaultException("The record you requested for update does not exist! ");
                }
            }
        }


        public string UpdateRecordTitle(int bid, int rid, string title, int aid, string sessionId)
        {
            SessionManager.checkSession(aid, sessionId);

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetRecordById(bid, rid);
            IEnumerator<GetRecordByIdResult> enumerator = result.GetEnumerator();
            if (enumerator.MoveNext())
            {
                GetRecordByIdResult record = enumerator.Current;
                TierController.validateAccessToRecord((int)record.account, (int)record.business, aid, bid);

                if (record.status == (int)RecordStatus.sent || record.status == (int)RecordStatus.closed)
                {
                    throw new FaultException("Sent or Closed Record's title cannot be updated! ");
                }
                else
                {
                    context.UpdateRecordTitle(bid, rid, title);
                    return string.Format("Record {0}'s title has been updated to {1}.", rid, title);
                }
            }
            else
            {
                throw new FaultException(
                    string.Format("Record with given id {0} not found! ", rid));
            }
        }
        */

    }
}