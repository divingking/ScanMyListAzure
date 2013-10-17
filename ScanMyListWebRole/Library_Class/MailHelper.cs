namespace SynchWebRole.Library_Class
{
    using SynchWebRole.REST_Service;
    using SendGridMail;
    using SendGridMail.Transport;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public class MailHelper
    {
        //private const string username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
        //private const string password = "i6dvglzv";

        // send to the account email that has logged into this business user.
        public static bool SendRecordBackup(string accountEmail, int bid, Record record, IDictionary<int, Business> involved)
        {
            SendGrid message = SendGrid.GenerateInstance();
            message.From = new MailAddress("Synch Order Tracking Service <ordertracking@synchbi.com>");
            message.AddTo(accountEmail);
            message.Subject = "Order confirmation";
            StringBuilder text = new StringBuilder();
            text.AppendLine(record.title);
            text.AppendLine(FormatRecord(record, involved, bid));
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

        public static string FormatRecord(Record record, IDictionary<int, Business> involved, int bid)
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
           
            builder.AppendLine(string.Format("Date: {0}", orderDate.ToString()));

            switch (record.category)
            {
                case (int)RecordCategory.Order:
                    builder.AppendLine(string.Format("Order from {0}", involved[bid].name));
                    break;
                case (int)RecordCategory.Receipt:
                    builder.AppendLine(string.Format("Receipt for {0}", involved[bid].name));
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
            builder.AppendLine(string.Format("{0,-60}{1,-20}{2,-60}{3,-8}", "Customer", "UPC/Product#", "Product Name", "Quantity"));

            foreach (RecordProduct product in record.products)
            {
                switch (record.category)
                {
                    case (int)RecordCategory.Order:
                        if (involved.ContainsKey(product.customer))
                            builder.AppendLine(string.Format("{0,-60}{1,-20}{2,-60}{3,-8}", involved[product.customer].name,
                                                                                            product.upc,
                                                                                            product.name,
                                                                                            product.quantity));
                        break;
                    case (int)RecordCategory.Receipt:
                        if (involved.ContainsKey(product.supplier))
                            builder.AppendLine(string.Format("{0,-60}{1,-20}{2,-60}{3,-8}", involved[product.supplier].name,
                                                                                            product.upc,
                                                                                            product.name,
                                                                                            product.quantity));
                        break;
                    default:
                        break;
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}