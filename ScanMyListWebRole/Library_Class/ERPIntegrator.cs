using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SynchWebRole.Library_Class
{
    public class ERPIntegrator
    {

        public static void relayRecord(int bid, int rid)
        {
            // check which ERP system to integrate first
            string queueName = "";
            if (bid != 76)
                return;
            else
                queueName = "invoiceqbd";
            CloudStorageAccount synchStorageAccount = CloudStorageAccount.Parse(
            Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            // Create the queue client
            CloudQueueClient queueClient = synchStorageAccount.CreateCloudQueueClient();
            
            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Create a message and add it to the queue
            string messageString = "rid:" + rid + ":" + bid;
            CloudQueueMessage message = new CloudQueueMessage(messageString);
            // queue.AddMessage(message);
        }
    }
}