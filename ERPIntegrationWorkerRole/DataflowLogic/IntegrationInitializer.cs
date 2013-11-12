using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ERPIntegrationWorkerRole.Utilities;
using ERPIntegrationWorkerRole.SynchIntegration;

namespace ERPIntegrationWorkerRole.DataflowLogic
{
    class IntegrationInitializer
    {
        public IntegrationInitializer()
        {
        }

        public void updateSynchFromERP()
        {
            SynchDatabaseDataContext context = new SynchDatabaseDataContext();

            var results = context.GetBusinessesWithIntegration(1);
            List<int> retrievedBusinessIDs = new List<int>();

            foreach (var business in results)
            {
                retrievedBusinessIDs.Add(business.id);
            }

            context.Dispose();

            foreach (int bid in retrievedBusinessIDs)
            {
                // try to do the auto-sync

                DataIntegratorQBD qbdIntegrator = new DataIntegratorQBD(bid);
                qbdIntegrator.updateCustomersFromQBD();
                qbdIntegrator.updateItemsFromQBD();
                //qbdIntegrator.updateSalesRepsFromQBD();
                qbdIntegrator.updateSalesOrdersFromQBD();
                qbdIntegrator.updateInvoicesFromQBD();
            }

        }

        /// <summary>
        /// Retrieves message from message queue and dispatch to responsible managers
        /// </summary>
        public void updateERPFromSynch()
        {
            string message = MessageProcessor.retrieveMessageFromSynchStorage(ApplicationConstants.ERP_QBD_QUEUE);
            if (message != null)
            {
                string[] elements = message.Split('-');
                switch (elements[0])
                {
                    case "00":      // administration
                        break;
                    case "01":      // business
                        manageBusiness(elements[1]);
                        break;
                    case "02":      // inventory
                        manageInventory(elements[1]);
                        break;
                    case "03":      // record
                        manageRecord(elements[1]);
                        break;
                    default:
                        break;
                }
            }
        }

        private void manageRecord(string message)
        {
            /*
                For example, for an user with QuickBooks Desktop as ERP backend, the queue would be:
                ERP-QBD
                and the message to create a new sales receipt would be:
                03-00:bid:76:rid:1517
             */
            string[] elements = message.Split(':');
            string operationCode = elements[0];
            int bid = Convert.ToInt32(elements[2]);
            int rid = Convert.ToInt32(elements[4]);

            DataIntegratorQBD qbdIntegrator = new DataIntegratorQBD(bid);

            switch (operationCode)
            {
                case "00":      // create
                    //qbdIntegrator.createInvoiceInQBD(rid);
                    qbdIntegrator.createSalesOrderInQBD(rid);
                    break;
                case "01":      // update
                    break;
                case "02":      // get
                    break;
                case "03":      // delete
                    break;
                default:
                    break;
            }

        }

        private void manageInventory(string message)
        {
            /*
                For example, for an user with QuickBooks Desktop as ERP backend, the queue would be:
                ERP-QBD-03
                and the message to create a new sales receipt would be:
                "00:bid:76:upc:883311002211";
            */
            string[] elements = message.Split(':');
            string operationCode = elements[0];
            int bid = Convert.ToInt32(elements[2]);
            string upc = elements[4];

            DataIntegratorQBD qbdIntegrator = new DataIntegratorQBD(bid);

            switch (operationCode)
            {
                case "00":      // create
                    qbdIntegrator.createItemInQbd(upc);
                    break;
                case "01":      // update
                    qbdIntegrator.updateItemInQbd(upc);
                    break;
                case "02":      // get
                    break;
                case "03":      // delete
                    break;
                default:
                    break;
            }

        }

        private void manageBusiness(string message)
        {
            /*
                For example, for an user with QuickBooks Desktop as ERP backend, the queue would be:
                ERP-QBD-01
                and the message to create a new customer with bid 1588 would be:
                00:bid:76:otherbid:1588:customer
            */
            string[] elements = message.Split(':');
            string operationCode = elements[0];
            int bid = Convert.ToInt32(elements[2]);
            int otherBid = Convert.ToInt32(elements[4]);
            bool isCustomer = (elements[5] == "customer");

            DataIntegratorQBD qbdIntegrator = new DataIntegratorQBD(bid);

            switch (operationCode)
            {
                case "00":      // create
                    qbdIntegrator.createBusinessInQBD(otherBid, isCustomer);
                    break;
                case "01":      // update
                    break;
                case "02":      // get
                    break;
                case "03":      // delete
                    break;
                default:
                    break;
            }

        }
    }
}
