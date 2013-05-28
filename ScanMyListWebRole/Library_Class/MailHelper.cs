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
            message.From = new MailAddress("ScanMyList Order Tracking Service <ordertracking@scanmylist.com>");
            message.AddTo(accountEmail);
            message.Subject = "Order confirmation";
            StringBuilder text = new StringBuilder();
            text.AppendLine(record.title);
            text.AppendLine(FormatRecord(record, involved));
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
            SendGrid message = SendGrid.GenerateInstance();
            message.From = new MailAddress("ScanMyList Order Tracking Service <ordertracking@scanmylist.com>");
            message.AddTo(string.Format("{0} <{1}>", involved[bid].name, involved[bid].email));
            message.Subject = "Order confirmation";
            StringBuilder text = new StringBuilder();
            text.AppendLine(FormatRecord(record, involved));
            text.AppendLine();
            text.AppendLine();
            text.AppendLine(
                string.Format(
                "This email was auto-generated and auto-sent to {0}. Please do not reply! ", 
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
            catch (Exception)
            {
                return false;
            }
        }

        public static string FormatRecord(Record record, IDictionary<int, Business> involved)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(string.Format("Date: {0}", record.date));

            switch (record.category)
            {
                case (int)RecordCategory.Order:
                    builder.AppendLine("Order: ");
                    break;
                case (int)RecordCategory.Receipt:
                    builder.AppendLine("Receipt: ");
                    break;
                case (int)RecordCategory.Change:
                    builder.AppendLine("Inventory change: ");
                    break;
                default:
                    break;
            }

            builder.AppendLine("Products: ");

            foreach (RecordProduct product in record.products)
            {
                builder.AppendLine(string.Format("Name: {0} ", product.name));
                builder.AppendLine(string.Format("Quantity: {0} ", product.quantity));

                switch (record.category)
                {
                    case (int)RecordCategory.Order:
                        if (involved.ContainsKey(product.customer))
                            builder.AppendLine(string.Format("Customer: {0}",involved[product.customer].name));
                        break;
                    case (int)RecordCategory.Receipt:
                        if (involved.ContainsKey(product.supplier))
                            builder.AppendLine(string.Format("Supplier: {0}", involved[product.supplier].name));
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