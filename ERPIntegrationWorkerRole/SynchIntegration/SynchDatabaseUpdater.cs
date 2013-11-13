using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPIntegrationWorkerRole.SynchIntegration
{
    public class SynchDatabaseUpdater
    {
        private int synchBusinessId;

        public SynchDatabaseUpdater(int synchBusinessId)
        {
            this.synchBusinessId = synchBusinessId;
        }


        public void createNewInventory(string upc, string name, string detail, string location, int quantity, int leadTime, double price, int category)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.CreateProduct(upc, name, detail);
                context.CreateInventory(synchBusinessId, upc, location, quantity, leadTime, price, category);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }

        public int createNewRecord(string invoiceTitle, int category, int status, string invoiceComment, int accountId, long transactionDateLong,
                                    int customerId, List<string> upcList, List<int> quantityList, List<double> priceList)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            int rid = 0;

            try
            {
                rid = context.CreateHistoryRecord(synchBusinessId, invoiceTitle, category, status, invoiceComment, accountId, transactionDateLong);
                for (int i = 0; i < upcList.Count; i++)
                {
                    context.CreateProductInRecord(rid, upcList[i], 1, customerId, quantityList[i], "", priceList[i]);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

            return rid;
        }


        internal void updateInventory(string upc, string detailFromQBD, int quantityFromQBD, double priceFromQBD, int synchBusinessId, string nameFromQBD)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.UpdateInventoryByUpc(upc, detailFromQBD, quantityFromQBD, priceFromQBD, synchBusinessId, nameFromQBD);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }

        public int createCustomer(string name, string address, int zip, string email, string category, int integration, int tier, string phoneNumber)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            int newCustomerId = -1;

            try
            {
                newCustomerId = context.CreateBusiness(name, address, zip, email, category, integration, tier, phoneNumber);
                context.CreateSupplies(synchBusinessId, newCustomerId, synchBusinessId);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

            return newCustomerId;

        }

        public void updateBusinessById(int otherBid, string address, int zip, string email, string category, string phoneNumber)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.UpdateBusinessById(otherBid, address, zip, email, category, phoneNumber);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }

        public void deleteCustomer(int cid)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.DeleteSupplies(synchBusinessId, cid, synchBusinessId);
                context.DeleteBusinessById(cid);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }

        }

        public void deleteInventory(string upc)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                context.DeleteInventoryByUpc(upc, synchBusinessId);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                context.Dispose();
            }
        }
    }

}
