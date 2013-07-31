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

using ERPIntegrationWorkerRole.QuickBookIntegration.Utils;
using ERPIntegrationWorkerRole.SynchIntegration;

namespace ERPIntegrationWorkerRole.QuickBookIntegration
{
    class QBDIntegrator
    {

        public static void createInvoiceInQBD(int rid)
        {
            // credentials for connecting with QuickBooks database
            string realmId, accessToken, accessTokenSecret, consumerKey, consumerSecret, dataSourcetype;
            accessToken = "lvprdsHdmwhxqxhwReuLgYSJyUtpUTTDBuMPS3frqLKRE5og";
            accessTokenSecret = "t9m89H4myvEvVq3oi3uac91jwV4r8sjSeWJZ3HFh";
            consumerKey = "qyprdChIG6ax7TK3OWyp6ZIygWNJwj";
            consumerSecret = "gFNdGdTaye35jSd9AYEeqqHY68KXdyEFD7p5x352";
            dataSourcetype = "QBD";
            realmId = "738592490";

            // validating
            OAuthRequestValidator oauthValidator = Initializer.InitializeOAuthValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret);
            ServiceContext context = Initializer.InitializeServiceContext(oauthValidator, realmId, string.Empty, string.Empty, dataSourcetype);
            DataServices commonService = new DataServices(context);

            // get invoice information from Synch database
            int synchBusinessId = 76;
            SynchDatabaseDataContext synchDataContext = new SynchDatabaseDataContext();
            var results = synchDataContext.GetOrderDetails(synchBusinessId, 1500);

            List<RecordProduct> products = new List<RecordProduct>();

            foreach (var product in results)
            {
                products.Add(
                    new RecordProduct()
                    {
                        upc = product.product_upc,
                        customer = product.customer_id,
                        quantity = (int)product.product_quantity,
                        price = (double)product.product_price,
                        note = product.item_note
                    });
            }
            
            var result = synchDataContext.GetCustomerById(synchBusinessId, products[0].customer);
            IEnumerator<GetCustomerByIdResult> customerEnumerator = result.GetEnumerator();
            Business customerInSynch = null;
            if (customerEnumerator.MoveNext())
            {
                GetCustomerByIdResult retrievedBusiness = customerEnumerator.Current;

                customerInSynch = new Business()
                {
                    id = retrievedBusiness.id,
                    name = retrievedBusiness.name,
                    address = retrievedBusiness.address,
                    zip = (int)retrievedBusiness.zip,
                    email = retrievedBusiness.email
                };
            }
            else
                return;     // no customer found

            string customerName = customerInSynch.name;
            string[] customerAddress = customerInSynch.address.Split(',');
            string customerEmail = customerInSynch.email;
            string zipCode = customerInSynch.zip.ToString();
            string stateCode = customerAddress[customerAddress.Length - 1].Split(' ')[0];

            // creates actual invoice
            Intuit.Ipp.Data.Qbd.PhysicalAddress physicalAddress = new Intuit.Ipp.Data.Qbd.PhysicalAddress();
            physicalAddress.Line1 = customerAddress[0];
            if (customerAddress.Length >= 4)
                physicalAddress.Line2 = customerAddress[1];

            physicalAddress.City = customerAddress[customerAddress.Length - 2];
            physicalAddress.CountrySubDivisionCode = stateCode;
            physicalAddress.Country = "USA";
            physicalAddress.PostalCode = zipCode;
            physicalAddress.Tag = new string[] { "Billing" };

            Intuit.Ipp.Data.Qbd.InvoiceHeader invoiceHeader = new Intuit.Ipp.Data.Qbd.InvoiceHeader();
            //invoiceHeader.ARAccountId = new Intuit.Ipp.Data.Qbd.IdType() { idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB, Value = "37" };
            //invoiceHeader.ARAccountName = "Accounts Receivable";
            // original code : invoiceHeader.CustomerId = new IdType() { idDomain = idDomainEnum.NG, Value = "3291253" };
            invoiceHeader.CustomerName = customerName;
            //invoiceHeader.CustomerId = new Intuit.Ipp.Data.Qbd.IdType() { idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB, Value = "2" };
            invoiceHeader.Balance = (decimal)100.00;
            invoiceHeader.BillAddr = physicalAddress;
            invoiceHeader.BillEmail = customerEmail;
            invoiceHeader.DocNumber = rid.ToString();
            invoiceHeader.DueDate = DateTime.Now;
            invoiceHeader.ShipAddr = physicalAddress;
            invoiceHeader.ShipDate = DateTime.Now;
            invoiceHeader.TaxAmt = (decimal)5;
            invoiceHeader.TaxRate = (decimal).05;
            invoiceHeader.ToBeEmailed = false;
            invoiceHeader.TotalAmt = (decimal)105.00;

            List<Intuit.Ipp.Data.Qbd.InvoiceLine> listLine = new List<Intuit.Ipp.Data.Qbd.InvoiceLine>();

            // add all the items in the record into inovice lines
            foreach (RecordProduct curProduct in products)
            {
                Intuit.Ipp.Data.Qbd.ItemsChoiceType2[] invoiceItemAttributes = { Intuit.Ipp.Data.Qbd.ItemsChoiceType2.ItemId, Intuit.Ipp.Data.Qbd.ItemsChoiceType2.UnitPrice, Intuit.Ipp.Data.Qbd.ItemsChoiceType2.Qty };
                // original code : object[] invoiceItemValues = { new IdType() { idDomain = idDomainEnum.QB, Value = "5" }, new decimal(33), new decimal(2) };
                object[] invoiceItemValues = { new Intuit.Ipp.Data.Qbd.IdType() { idDomain = Intuit.Ipp.Data.Qbd.idDomainEnum.QB, Value = "1" }, new decimal(33), new decimal(2) };
                var invoiceLine = new Intuit.Ipp.Data.Qbd.InvoiceLine();
                invoiceLine.Amount = 66;
                invoiceLine.AmountSpecified = true;
                invoiceLine.Desc = "test " + DateTime.Now.ToShortDateString();
                invoiceLine.ItemsElementName = invoiceItemAttributes;
                invoiceLine.Items = invoiceItemValues;
                invoiceLine.ServiceDate = DateTime.Now;
                invoiceLine.ServiceDateSpecified = true;
                listLine.Add(invoiceLine);
            }

            Intuit.Ipp.Data.Qbd.Invoice invoice = new Intuit.Ipp.Data.Qbd.Invoice();
            invoice.Header = invoiceHeader;
            invoice.Line = listLine.ToArray();

            Intuit.Ipp.Data.Qbd.Invoice addedInvoice = commonService.Add(invoice);
        }
    }
}
