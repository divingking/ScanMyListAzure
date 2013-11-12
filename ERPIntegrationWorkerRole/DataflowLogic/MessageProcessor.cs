    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace ERPIntegrationWorkerRole.DataflowLogic
{
    class MessageProcessor
    {
        public static string retrieveMessageFromSynchStorage(string queueName)
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            // Create the queue client
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            CloudQueueMessage retrievedMessage = queue.GetMessage();

            if (retrievedMessage != null)
            {
                //Process the message in less than 30 seconds, and then delete the message
                string message = retrievedMessage.AsString;
                queue.DeleteMessage(retrievedMessage);
                return message;
            }
            else
                return null;
        }

    }
}
