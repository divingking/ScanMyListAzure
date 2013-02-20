using SendGridMail;
using SendGridMail.Transport;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ScanMyListWebRole
{
    public class MailHelper
    {
        //private const string username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
        //private const string password = "i6dvglzv";

        public static bool SendOrderBackup(Order order)
        {
            SendGrid testMessage = SendGrid.GenerateInstance();
            testMessage.From = new MailAddress("ScanMyList Order Tracking Service <ordertracking@scanmylist.com>");
            testMessage.AddTo("Sully Liu <zelinliu@me.com>");
            testMessage.Subject = "Order confirmation For System";
            StringBuilder text = new StringBuilder();
            text.AppendLine(FormatOrder(order));
            text.AppendLine();
            text.AppendLine();
            text.AppendLine("This email was auto-generated. Please do not reply! ");
            testMessage.Text = text.ToString();

            var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
            var password = "i6dvglzv";
            var credentials = new NetworkCredential(username, password);

            var transportSMTP = SMTP.GenerateInstance(credentials);
            try
            {
                transportSMTP.Deliver(testMessage);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static bool SendOrder(Order order, int customer_id)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var result = context.GetCustomer(customer_id);
            IEnumerator<GetCustomerResult> enumerator = result.GetEnumerator();
            if (enumerator.MoveNext())
            {
                GetCustomerResult customer = enumerator.Current;
                SendGrid testMessage = SendGrid.GenerateInstance();
                testMessage.From = new MailAddress("ScanMyList Order Tracking Service <ordertracking@scanmylist.com>");
                testMessage.AddTo(string.Format("{0} {1} <{2}>", customer.fname, customer.lname, customer.email));
                testMessage.Subject = "Order confirmation";
                StringBuilder text = new StringBuilder();
                text.AppendLine(FormatOrder(order));
                text.AppendLine();
                text.AppendLine();
                text.AppendLine("This email was auto-generated. Please do not reply! ");
                testMessage.Text = text.ToString();

                var username = "azure_bf33e57baacbfaae4ebfe0814f1d8a5d@azure.com";
                var password = "i6dvglzv";
                var credentials = new NetworkCredential(username, password);

                var transportSMTP = SMTP.GenerateInstance(credentials);
                transportSMTP.Deliver(testMessage);

                try
                {
                    transportSMTP.Deliver(testMessage);
                    return true;
                }
                catch (System.Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static string FormatOrder(Order order)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(string.Format("Date: {0}", order.date));
            
            if (order.scanIn)
                builder.AppendLine("Scan in");
            else
                builder.AppendLine("Scan out");

            int productCount = 0;
            double totalPrice = 0;
            builder.AppendLine("Products: ");
            foreach (Product product in order.products)
            {
                builder.AppendLine(string.Format("UPC: {0}", product.upc));
                builder.AppendLine(string.Format("Name: {0}", product.name));
                builder.AppendLine(string.Format("Detail: {0}", product.detail));
                builder.AppendLine(string.Format("Lead Time: {0}", product.leadTime));
                builder.AppendLine(string.Format("Quantity: {0}", product.quantity));
                if (product.supplier != null)
                {
                    builder.AppendLine(string.Format("Unit price: {0}", product.supplier.price));
                    builder.AppendLine(string.Format("Total price: {0}", product.quantity * product.supplier.price));
                    builder.AppendLine(string.Format("Supplier: {0}", product.supplier.name));
                    builder.AppendLine(string.Format("Supplier's address: {0}", product.supplier.address));
                    totalPrice += product.quantity * product.supplier.price;
                }
                builder.AppendLine();
                productCount++;
            }
            builder.AppendLine(string.Format("Total price: {0}", totalPrice));

            return builder.ToString();
        }
    }
}