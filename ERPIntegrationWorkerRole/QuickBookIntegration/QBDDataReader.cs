using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Web;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.Services;
using Intuit.Ipp.Data;
using Intuit.Ipp.AsyncServices;
using Intuit.Ipp.Diagnostics;
using Intuit.Ipp.Exception;
using Intuit.Ipp.Retry;
using Intuit.Ipp.Utility;

using ERPIntegrationWorkerRole.Utilities;
using ERPIntegrationWorkerRole.SynchIntegration;

namespace ERPIntegrationWorkerRole.QuickBookIntegration
{
    class QBDDataReader
    {
        ServiceContext qbServiceContext;
        DataServices qbdDataService;

        public QBDDataReader(QuickBooksCredentialEntity qbCredential)
        {
            OAuthRequestValidator oauthValidator = Utils.Initializer.InitializeOAuthValidator(qbCredential.accessToken, qbCredential.accessTokenSecret,
                qbCredential.consumerKey, qbCredential.consumerSecret);
            qbServiceContext = Utils.Initializer.InitializeServiceContext(oauthValidator, qbCredential.realmId, string.Empty, string.Empty, qbCredential.dataSourcetype);
            qbdDataService = new DataServices(qbServiceContext);
        }

        public List<Intuit.Ipp.Data.Qbd.Invoice> getInvoicesFromDate(DateTime startDate)
        {
            // uses InvoiceQuery to repeatedly get invoice information
            List<Intuit.Ipp.Data.Qbd.Invoice> result = new List<Intuit.Ipp.Data.Qbd.Invoice>();
            int pageNumber = 1;
            int chunkSize = 500;
            Intuit.Ipp.Data.Qbd.InvoiceQuery qbdInvoiceQuery = new Intuit.Ipp.Data.Qbd.InvoiceQuery();
            qbdInvoiceQuery.ItemElementName = Intuit.Ipp.Data.Qbd.ItemChoiceType4.StartPage;
            qbdInvoiceQuery.Item = pageNumber.ToString();
            qbdInvoiceQuery.ChunkSize = chunkSize.ToString();
            qbdInvoiceQuery.StartCreatedTMS = startDate;
            qbdInvoiceQuery.StartCreatedTMSSpecified = true;
            IEnumerable<Intuit.Ipp.Data.Qbd.Invoice> invoicesFromQBD = qbdInvoiceQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Invoice>
            (qbServiceContext) as IEnumerable<Intuit.Ipp.Data.Qbd.Invoice>;
            result.AddRange(invoicesFromQBD.ToArray());
            int curItemCount = invoicesFromQBD.ToArray().Length;
            invoicesFromQBD.ToArray();
            while (curItemCount > 0)
            {
                pageNumber++;
                qbdInvoiceQuery.Item = pageNumber.ToString();
                invoicesFromQBD = qbdInvoiceQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Invoice>
                                     (qbServiceContext) as IEnumerable<Intuit.Ipp.Data.Qbd.Invoice>;
                result.AddRange(invoicesFromQBD.ToArray());
                curItemCount = invoicesFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<Intuit.Ipp.Data.Qbd.SalesOrder> getSalesOrdersFromDate(DateTime startDate)
        {
            // uses salesOrderQuery to repeatedly get salesOrder information
            List<Intuit.Ipp.Data.Qbd.SalesOrder> result = new List<Intuit.Ipp.Data.Qbd.SalesOrder>();
            int pageNumber = 1;
            int chunkSize = 500;
            Intuit.Ipp.Data.Qbd.SalesOrderQuery qbdSalesOrderQuery = new Intuit.Ipp.Data.Qbd.SalesOrderQuery();
            qbdSalesOrderQuery.ItemElementName = Intuit.Ipp.Data.Qbd.ItemChoiceType4.StartPage;
            qbdSalesOrderQuery.Item = pageNumber.ToString();
            qbdSalesOrderQuery.ChunkSize = chunkSize.ToString();
            qbdSalesOrderQuery.StartCreatedTMS = startDate;
            qbdSalesOrderQuery.StartCreatedTMSSpecified = true;
            qbdSalesOrderQuery.StatusFilter = Intuit.Ipp.Data.Qbd.SalesOrderStatusFilterEnumType.Open;
            qbdSalesOrderQuery.StatusFilterSpecified = true;
            IEnumerable<Intuit.Ipp.Data.Qbd.SalesOrder> salesOrdersFromQBD = qbdSalesOrderQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.SalesOrder>
            (qbServiceContext) as IEnumerable<Intuit.Ipp.Data.Qbd.SalesOrder>;
            result.AddRange(salesOrdersFromQBD.ToArray());
            int curItemCount = salesOrdersFromQBD.ToArray().Length;
            salesOrdersFromQBD.ToArray();
            while (curItemCount > 0)
            {
                pageNumber++;
                qbdSalesOrderQuery.Item = pageNumber.ToString();
                salesOrdersFromQBD = qbdSalesOrderQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.SalesOrder>
                                     (qbServiceContext) as IEnumerable<Intuit.Ipp.Data.Qbd.SalesOrder>;
                result.AddRange(salesOrdersFromQBD.ToArray());
                curItemCount = salesOrdersFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<Intuit.Ipp.Data.Qbd.Item> getItems()
        {
            List<Intuit.Ipp.Data.Qbd.Item> result = new List<Intuit.Ipp.Data.Qbd.Item>();

            int pageNumber = 1;
            int chunkSize = 500;
            Intuit.Ipp.Data.Qbd.ItemQuery qbdItemQuery = new Intuit.Ipp.Data.Qbd.ItemQuery();
            qbdItemQuery.ItemElementName = Intuit.Ipp.Data.Qbd.ItemChoiceType4.StartPage;
            qbdItemQuery.Item = pageNumber.ToString();
            qbdItemQuery.ChunkSize = chunkSize.ToString();
            qbdItemQuery.ActiveOnly = true;
            IEnumerable<Intuit.Ipp.Data.Qbd.Item> itemsFromQBD = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>
            (qbServiceContext) as IEnumerable<Intuit.Ipp.Data.Qbd.Item>;
            result.AddRange(itemsFromQBD.ToArray());
            int curItemCount = itemsFromQBD.ToArray().Length;

            while (curItemCount > 0)
            {
                pageNumber++;
                qbdItemQuery.Item = pageNumber.ToString();
                itemsFromQBD = qbdItemQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Item>
                                     (qbServiceContext) as IEnumerable<Intuit.Ipp.Data.Qbd.Item>;
                result.AddRange(itemsFromQBD.ToArray());
                curItemCount = itemsFromQBD.ToArray().Length;
            }

            return result;
        }

        public List<Intuit.Ipp.Data.Qbd.Customer> getCustomers()
        {
            List<Intuit.Ipp.Data.Qbd.Customer> result = new List<Intuit.Ipp.Data.Qbd.Customer>();

            int pageNumber = 1;
            int chunkSize = 500;
            Intuit.Ipp.Data.Qbd.CustomerQuery qbdCustomerQuery = new Intuit.Ipp.Data.Qbd.CustomerQuery();
            qbdCustomerQuery.ItemElementName = Intuit.Ipp.Data.Qbd.ItemChoiceType4.StartPage;
            qbdCustomerQuery.Item = pageNumber.ToString();
            qbdCustomerQuery.ChunkSize = chunkSize.ToString();
            qbdCustomerQuery.ActiveOnly = true;
            IEnumerable<Intuit.Ipp.Data.Qbd.Customer> customersFromQBD = qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>
            (qbServiceContext) as IEnumerable<Intuit.Ipp.Data.Qbd.Customer>;
            result.AddRange(customersFromQBD.ToArray());
            int curCustomerCount = customersFromQBD.ToArray().Length;

            while (curCustomerCount > 0)
            {
                pageNumber++;
                qbdCustomerQuery.Item = pageNumber.ToString();
                customersFromQBD = qbdCustomerQuery.ExecuteQuery<Intuit.Ipp.Data.Qbd.Customer>
                                     (qbServiceContext) as IEnumerable<Intuit.Ipp.Data.Qbd.Customer>;
                result.AddRange(customersFromQBD.ToArray());
                curCustomerCount = customersFromQBD.ToArray().Length;
            }

            return result;
        }
    }
}
