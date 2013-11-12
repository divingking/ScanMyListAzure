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

using ERPIntegrationWorkerRole.SynchIntegration;
using ERPIntegrationWorkerRole.Utilities;

namespace ERPIntegrationWorkerRole.QuickBookIntegration
{
    class QBDDataUpdater
    {
        DataServices qbdDataService;

        public QBDDataUpdater(QuickBooksCredentialEntity qbCredential)
        {
            OAuthRequestValidator oauthValidator = Utils.Initializer.InitializeOAuthValidator(qbCredential.accessToken, qbCredential.accessTokenSecret,
                qbCredential.consumerKey, qbCredential.consumerSecret);
            ServiceContext context = Utils.Initializer.InitializeServiceContext(oauthValidator, qbCredential.realmId, string.Empty, string.Empty, qbCredential.dataSourcetype);
            qbdDataService = new DataServices(context);
        }

        public Intuit.Ipp.Data.Qbd.Invoice createInvoice(SynchRecord recordFromSynch, SynchBusiness customerInSynch,
            Dictionary<string, string> upcToItemIdMap, Dictionary<int, string> synchCidToCustomerIdMap)
        {
            // creates actual invoice
            // add all the items in the record into inovice lines
            decimal balance = Decimal.Zero;
            List<Intuit.Ipp.Data.Qbd.InvoiceLine> listLine = new List<Intuit.Ipp.Data.Qbd.InvoiceLine>();
            foreach (SynchRecordProduct curProduct in recordFromSynch.products)
            {
                // QBD uses an array pair to map attributes to their values.
                // The first array keeps track of what elements are in the second array.
                Intuit.Ipp.Data.Qbd.ItemsChoiceType2[] invoiceItemAttributes =
                { 
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.ItemId,
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.UnitPrice,
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.Qty 
                };
                // Now the second array
                object[] invoiceItemValues =
                {
                    new Intuit.Ipp.Data.Qbd.IdType() 
                    {
                        idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB,
                        Value = upcToItemIdMap[curProduct.upc]
                    },
                    new decimal(curProduct.price),
                    new decimal(curProduct.quantity) 
                };

                var invoiceLine = new Intuit.Ipp.Data.Qbd.InvoiceLine();
                invoiceLine.Amount = (Decimal)curProduct.price * curProduct.quantity;
                invoiceLine.AmountSpecified = true;
                invoiceLine.Desc = curProduct.detail;
                invoiceLine.ItemsElementName = invoiceItemAttributes;
                invoiceLine.Items = invoiceItemValues;
                //invoiceLine.ServiceDate = DateTime.Now;
                //invoiceLine.ServiceDateSpecified = true;
                listLine.Add(invoiceLine);

                balance += invoiceLine.Amount;
            }

            Intuit.Ipp.Data.Qbd.InvoiceHeader invoiceHeader = new Intuit.Ipp.Data.Qbd.InvoiceHeader();
            invoiceHeader.CustomerId = new Intuit.Ipp.Data.Qbd.IdType()
            {
                idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB,
                Value = synchCidToCustomerIdMap[customerInSynch.id]
            };

            invoiceHeader.Balance = (decimal)0.0;
            //invoiceHeader.BillAddr = physicalAddress;
            invoiceHeader.BillEmail = customerInSynch.email;
            invoiceHeader.DueDate = DateTime.Now;
            //invoiceHeader.ShipAddr = physicalAddress;
            invoiceHeader.ShipDate = DateTime.Now;
            invoiceHeader.TaxRate = (decimal).09;
            invoiceHeader.TaxAmt = (decimal)balance * invoiceHeader.TaxRate;
            invoiceHeader.ToBeEmailed = false;
            invoiceHeader.TotalAmt = invoiceHeader.TaxAmt + invoiceHeader.Balance;
            invoiceHeader.Msg = recordFromSynch.comment;
            invoiceHeader.Note = recordFromSynch.comment;
            Intuit.Ipp.Data.Qbd.Invoice invoice = new Intuit.Ipp.Data.Qbd.Invoice();
            invoice.Header = invoiceHeader;
            invoice.Line = listLine.ToArray();

            return qbdDataService.Add(invoice);
        }

        public Intuit.Ipp.Data.Qbd.SalesOrder createSalesOrder(SynchRecord recordFromSynch, SynchBusiness customerInSynch,
            Dictionary<string, string> upcToItemIdMap, Dictionary<int, string> synchCidToCustomerIdMap)
        {
            decimal balance = Decimal.Zero;
            List<Intuit.Ipp.Data.Qbd.SalesOrderLine> listLine = new List<Intuit.Ipp.Data.Qbd.SalesOrderLine>();
            foreach (SynchRecordProduct curProduct in recordFromSynch.products)
            {
                // QBD uses an array pair to map attributes to their values.
                // The first array keeps track of what elements are in the second array.
                Intuit.Ipp.Data.Qbd.ItemsChoiceType2[] salesOrderItemAttributes =
                { 
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.ItemId,
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.UnitPrice,
                    Intuit.Ipp.Data.Qbd.ItemsChoiceType2.Qty 
                };
                // Now the second array
                object[] salesOrderItemValues =
                {
                    new Intuit.Ipp.Data.Qbd.IdType() 
                    {
                        idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB,
                        Value = upcToItemIdMap[curProduct.upc]
                    },
                    new decimal(curProduct.price),
                    new decimal(curProduct.quantity) 
                };

                var salesOrderLine = new Intuit.Ipp.Data.Qbd.SalesOrderLine();
                salesOrderLine.Amount = (Decimal)curProduct.price * curProduct.quantity;
                salesOrderLine.AmountSpecified = true;
                salesOrderLine.Desc = curProduct.detail;
                salesOrderLine.ItemsElementName = salesOrderItemAttributes;
                salesOrderLine.Items = salesOrderItemValues;
                //invoiceLine.ServiceDate = DateTime.Now;
                //invoiceLine.ServiceDateSpecified = true;
                listLine.Add(salesOrderLine);

                balance += salesOrderLine.Amount;
            }

            Intuit.Ipp.Data.Qbd.SalesOrderHeader salesOrderHeader = new Intuit.Ipp.Data.Qbd.SalesOrderHeader();
            salesOrderHeader.CustomerId = new Intuit.Ipp.Data.Qbd.IdType()
            {
                idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB,
                Value = synchCidToCustomerIdMap[customerInSynch.id]
            };

            salesOrderHeader.Balance = (decimal)0.0;
            //salesOrderHeader.BillAddr = physicalAddress;
            salesOrderHeader.BillEmail = customerInSynch.email;
            salesOrderHeader.DueDate = DateTime.Now;
            //salesOrderHeader.ShipAddr = physicalAddress;
            salesOrderHeader.ShipDate = DateTime.Now;
            salesOrderHeader.TaxRate = (decimal).09;
            salesOrderHeader.TaxAmt = (decimal)balance * salesOrderHeader.TaxRate;
            salesOrderHeader.ToBeEmailed = false;
            salesOrderHeader.TotalAmt = salesOrderHeader.TaxAmt + salesOrderHeader.Balance;
            salesOrderHeader.Msg = recordFromSynch.comment;
            salesOrderHeader.Note = recordFromSynch.comment;

            Intuit.Ipp.Data.Qbd.SalesOrder salesOrder = new Intuit.Ipp.Data.Qbd.SalesOrder();
            salesOrder.Header = salesOrderHeader;
            salesOrder.Line = listLine.ToArray();

            return qbdDataService.Add(salesOrder);
        }

    }
}
