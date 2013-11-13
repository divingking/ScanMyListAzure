using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPIntegrationWorkerRole.SynchIntegration
{
    class SynchDatabaseReader
    {
        private int synchBusinessId;

        public SynchDatabaseReader(int synchBusinessId)
        {
            this.synchBusinessId = synchBusinessId;
        }


        public SynchRecord getCompleteOrder(int rid)
        {
            SynchRecord recordResult = new SynchRecord();
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            try
            {
                var results = context.GetCompleteOrder(synchBusinessId, rid);
                recordResult.products = new List<SynchRecordProduct>();

                foreach (var product in results)
                {
                    recordResult.account = (int)product.record_account;
                    recordResult.comment = product.record_comment;
                    recordResult.id = product.record_id;
                    recordResult.title = product.record_title;

                    recordResult.products.Add(
                        new SynchRecordProduct()
                        {
                            name = product.product_name,
                            upc = product.product_upc,
                            detail = product.product_detail,
                            customer = product.customer_id,
                            quantity = (int)product.product_quantity,
                            price = (double)product.product_price,
                            note = product.product_note,
                        });
                }
            }
            catch (Exception)
            {
                recordResult = null;
                throw;

            }
            finally
            {
                context.Dispose();
            }
            

            return recordResult;
        }

        public SynchBusiness getBusiness(int otherBid)
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            SynchBusiness business = null;

            try
            {
                var result = context.GetBusiness(otherBid);
                IEnumerator<GetBusinessResult> businessEnumerator = result.GetEnumerator();
                if (businessEnumerator.MoveNext())
                {
                    business = new SynchBusiness()
                    {
                        id = businessEnumerator.Current.id,
                        name = businessEnumerator.Current.name,
                        address = businessEnumerator.Current.address,
                        zip = (int)businessEnumerator.Current.zip,
                        email = businessEnumerator.Current.email
                    };
                }

            }
            catch (Exception)
            {
                business = null;
                throw;
            }
            finally
            {
                context.Dispose();
            }

            return business;
        }

        public List<SynchProduct> getInventories()
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            List<SynchProduct> inventory = new List<SynchProduct>();

            try
            {
                var results = context.GetAllInventory(synchBusinessId);

                foreach (GetAllInventoryResult p in results)
                {
                    SynchProduct product = new SynchProduct()
                    {
                        upc = p.upc,
                        name = p.name,
                        detail = p.detail,
                        leadTime = (int)p.lead_time,
                        quantity = (int)p.quantity,
                        location = p.location,
                        owner = synchBusinessId,
                        price = (double)p.default_price
                    };

                    inventory.Add(product);
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

            return inventory;
        }

        public Dictionary<string, SynchProduct> getUpcToInventoryMap()
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            Dictionary<string, SynchProduct> result = new Dictionary<string, SynchProduct>();
            try
            {
                var results = context.GetAllInventory(synchBusinessId);
                foreach (var inventory in results)
                {
                    result.Add(inventory.upc,
                        new SynchProduct()
                        {
                            name = inventory.name,
                            upc = inventory.upc,
                            detail = inventory.detail,
                            location = inventory.location,
                            quantity = (int)inventory.quantity,
                            leadTime = (int)inventory.lead_time,
                            price = (double)inventory.default_price
                        });
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

            return result;
        }

        public Dictionary<int, SynchBusiness> getBidToCustomerMap()
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();
            Dictionary<int, SynchBusiness> result = new Dictionary<int, SynchBusiness>();
            try
            {
                var results = context.GetAllCustomers(synchBusinessId);
                foreach (var customer in results)
                {
                    result.Add(customer.id,
                        new SynchBusiness()
                        {
                            id = customer.id,
                            name = customer.name,
                            address = customer.address,
                            email = customer.email,
                            zip = (int)customer.zip,
                            phoneNumber = customer.phone_number
                        });
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

            return result;
        }
    }
}
