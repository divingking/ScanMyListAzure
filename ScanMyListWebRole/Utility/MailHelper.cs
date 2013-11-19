namespace SynchWebRole.Utility
{
    using SynchWebRole.ServiceManager;
    using SendGridMail;
    using SendGridMail.Transport;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Text.RegularExpressions;


    public class MailHelper
    {
        //private const string username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
        //private const string password = "i6dvglzv";

        // send to the account email that has logged into this business user.
        public static bool SendRecordBackup(User account, int bid, Record record, IDictionary<int, Business> involved)
        {
            SendGrid message = SendGrid.GenerateInstance();
            message.From = new MailAddress("Synch Order Tracking Service <ordertracking@synchbi.com>");
            message.AddTo(string.Format("{0} <{1}>", involved[bid].name, involved[bid].email));
            message.AddTo(account.email);
            message.AddTo(" synchbiorder@gmail.com");
            message.Subject = "Order confirmation";
            StringBuilder text = new StringBuilder();
            text.AppendLine(record.title);
            text.AppendLine(FormatRecord(account.login, record, involved, bid));
            text.AppendLine();
            text.AppendLine();
            text.AppendLine(
                string.Format(
                "This email was auto-generated and auto-sent to {0}. Please do not reply! ",
                involved[bid].name));
            message.Text = text.ToString();

            var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
            var password = "i6dvglzv";
            var credentials = new NetworkCredential(username, password);

            var transportSMTP = SMTP.GenerateInstance(credentials);

            try
            {
                transportSMTP.Deliver(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendRecord(int bid, Record record, IDictionary<int, Business> involved)
        {
            return true;
            /*
            SendGrid message = SendGrid.GenerateInstance();
            message.From = new MailAddress("Synch Order Tracking Service <ordertracking@synchbi.com>");
            message.AddTo(string.Format("{0} <{1}>", involved[bid].name, involved[bid].email));
            message.Subject = "Order confirmation";
            StringBuilder text = new StringBuilder();
            text.AppendLine(FormatRecord(record, involved, bid));
            text.AppendLine();
            text.AppendLine();
            text.AppendLine(
                string.Format(
                "This email was auto-generated and auto-sent to {0}. Please do not reply. ", 
                involved[bid].name));
            message.Text = text.ToString();

            // add Excel sheet as attachment
            //string excelFilePath = ExcelGenerator.AutomateExcel();
            //message.AddAttachment(excelFilePath);

            var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
            var password = "i6dvglzv";
            var credentials = new NetworkCredential(username, password);

            var transportSMTP = SMTP.GenerateInstance(credentials);

            try
            {
                transportSMTP.Deliver(message);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            */

        }

        public static string FormatRecord(string login, Record record, IDictionary<int, Business> involved, int bid)
        {
            StringBuilder builder = new StringBuilder();

            // converts long int into Date
            string dateString = record.date.ToString();
            DateTime orderDate = new DateTime(
                Convert.ToInt32(dateString.Substring(0, 4)),          // year
                Convert.ToInt32(dateString.Substring(4, 2)),          // month   
                Convert.ToInt32(dateString.Substring(6, 2)),          // day
                Convert.ToInt32(dateString.Substring(8, 2)),          // hour
                Convert.ToInt32(dateString.Substring(10, 2)),         // minute
                Convert.ToInt32(dateString.Substring(12, 2)));        // second
           
            builder.AppendLine(string.Format("Order Date: {0}", orderDate.ToString()));

            switch (record.category)
            {
                case (int)RecordCategory.Order:
                    builder.AppendLine(string.Format("Order from {0}", login));
                    break;
                case (int)RecordCategory.Receipt:
                    builder.AppendLine(string.Format("Receipt for {0}", login));
                    break;
                case (int)RecordCategory.PhysicalDamage:
                    builder.AppendLine("Inventory Change (Physical Damange)");
                    break;
                case (int)RecordCategory.PhysicalInventory:
                    builder.AppendLine("Inventory Change (Physical Inventory)");
                    break;
                case (int)RecordCategory.QualityIssue:
                    builder.AppendLine("Inventory Change (Quality Issue)");
                    break;
                case (int)RecordCategory.CycleCount:
                    builder.AppendLine("Inventory Change (Cycle Count)");
                    break;
                case (int)RecordCategory.Return:
                    builder.AppendLine("Inventory Change (Return)");
                    break;
                case (int)RecordCategory.SalesSample:
                    builder.AppendLine("Inventory Change (Sales Sample)");
                    break;
                case (int)RecordCategory.Stolen:
                    builder.AppendLine("Inventory Change (Stolen)");
                    break;
                default:
                    break;
            }

            builder.AppendLine();
            builder.AppendLine(string.Format("{0,-60}{1,-20}{2,-60}{3,-20}{4,-20}", "Customer", "UPC/Product#", "Product Name", "Quantity", "Location"));

            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            Product[] sortedProducts = sortProductsByLocation(bid, context, record.products);

            for (int i = 0; i < sortedProducts.Length; i++)
            {
                string location = sortedProducts[i].location;
                string upc = sortedProducts[i].upc;
                RecordProduct product = getRecordProductByUpc(upc, record.products);
                
                switch (record.category)
                {
                    case (int)RecordCategory.Order:
                        if (involved.ContainsKey(product.customer))
                            builder.AppendLine(string.Format("{0,-60}{1,-20}{2,-60}{3,-20}{4,-20}", involved[product.customer].name,
                                                                                            product.upc,
                                                                                            product.name,
                                                                                            product.quantity,
                                                                                            location
                                                                                            ));
                        break;
                    case (int)RecordCategory.Receipt:
                        if (involved.ContainsKey(product.supplier))
                            builder.AppendLine(string.Format("{0,-60}{1,-20}{2,-60}{3,-20}{4,-20}", involved[product.supplier].name,
                                                                                            product.upc,
                                                                                            product.name,
                                                                                            product.quantity,
                                                                                            location));
                        break;
                    default:
                        break;
                }
                builder.AppendLine();
            }

            builder.AppendLine(string.Format("Memo: {0}", record.comment));
            return builder.ToString();
        }

        private static RecordProduct getRecordProductByUpc(string upc, List<RecordProduct> products)
        {
            foreach (RecordProduct curProduct in products)
            {
                if (upc == curProduct.upc)
                    return curProduct;
            }

            return null;
        }

        private static Product[] sortProductsByLocation(int bid, ScanMyListDatabaseDataContext context, List<RecordProduct> products)
        {
            List<string> locationList = new List<string>();
            Product[] sortedProducts = new Product[products.Count];

            string pattern = "\\W+";
            string replacement = String.Empty;
            Regex rgx = new Regex(pattern);
            
            foreach (RecordProduct product in products)
            {
                var results = context.GetInventoryByUpc(bid, product.upc);
                IEnumerator<GetInventoryByUpcResult> productEnumerator = results.GetEnumerator();
                if (productEnumerator.MoveNext())
                {
                    GetInventoryByUpcResult target = productEnumerator.Current;
                    Product curProduct = new Product()
                    {
                        upc = target.upc,
                        name = target.name,
                        detail = target.detail,
                        quantity = (int)target.quantity,
                        location = target.location,
                        owner = bid,
                        leadTime = (int)target.lead_time,
                        price = (double)target.default_price
                    };

                    string comparableLocation = rgx.Replace(curProduct.location, replacement);
                    locationList.Add(comparableLocation);

                }
            }

            locationList.Sort();
            foreach (RecordProduct product in products)
            {
                var results = context.GetInventoryByUpc(bid, product.upc);
                IEnumerator<GetInventoryByUpcResult> productEnumerator = results.GetEnumerator();
                if (productEnumerator.MoveNext())
                {
                    GetInventoryByUpcResult target = productEnumerator.Current;
                    Product curProduct = new Product()
                    {
                        upc = target.upc,
                        name = target.name,
                        detail = target.detail,
                        quantity = product.quantity,
                        location = target.location,
                        owner = bid,
                        leadTime = (int)target.lead_time,
                        price = product.price
                    };

                    string comparableLocation = rgx.Replace(curProduct.location, replacement);
                    for (int i = 0; i < locationList.Count; i++)
                    {
                        if (comparableLocation == locationList[i])
                        {
                            while (sortedProducts[i] != null)
                                i++;
                            sortedProducts[i] = curProduct;
                            break;
                        }
                    }

                }
            }


            return sortedProducts;
        }
    }
}