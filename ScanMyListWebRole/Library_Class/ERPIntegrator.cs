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

        public static void testAzureStorageQueue(int rid)
        {
            CloudStorageAccount synchStorageAccount = CloudStorageAccount.Parse(
            Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            // Create the queue client
            CloudQueueClient queueClient = synchStorageAccount.CreateCloudQueueClient();
            
            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("invoice");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage("new record id:4690");
            queue.AddMessage(message);
        }
    }
}