using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

using SynchWebRole.Utilities;
namespace SynchWebRole.Utility
{
    public class ERPIntegrator
    {

        public static void relayRecordManagement(int bid, int rid)
        {
            // check which ERP system to integrate first
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetBusinessesWithIntegration(1);

            bool isIntegratedWithERP = false;
            foreach (var business in results)
            {
                if (bid == business.id)
                {
                    isIntegratedWithERP = true;
                    break;
                }
            }

            if (isIntegratedWithERP)
            {
                string queueName = ApplicationConstants.ERP_QBD_QUEUE;
                CloudStorageAccount synchStorageAccount = CloudStorageAccount.Parse(
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

                // Create the queue client
                CloudQueueClient queueClient = synchStorageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue
                CloudQueue queue = queueClient.GetQueueReference(queueName);

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                // Create a message and add it to the queue
                string messageString = "03-00:bid:" + bid + ":rid:" + rid;
                CloudQueueMessage message = new CloudQueueMessage(messageString);
                queue.AddMessage(message);
            }
        }

        public static void relayBusinessManagement(int bid, int otherBid, bool isCustomer)
        {
            // check which ERP system to integrate first
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetBusinessesWithIntegration(1);

            bool isIntegratedWithERP = false;
            foreach (var business in results)
            {
                if (bid == business.id)
                {
                    isIntegratedWithERP = true;
                    break;
                }
            }

            if (isIntegratedWithERP)
            {
                string queueName = ApplicationConstants.ERP_QBD_QUEUE;
                CloudStorageAccount synchStorageAccount = CloudStorageAccount.Parse(
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

                // Create the queue client
                CloudQueueClient queueClient = synchStorageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue
                CloudQueue queue = queueClient.GetQueueReference(queueName);

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                // Create a message and add it to the queue
                string messageString = "01-00:bid:" + bid + ":otherbid:" + otherBid;
                if (isCustomer)
                    messageString += ":customer";
                else
                    messageString += "supplier";
                CloudQueueMessage message = new CloudQueueMessage(messageString);
                queue.AddMessage(message);
            }
        }

        public static void relayInventoryManagement(int bid, string upc, string operationCode)
        {
            // check which ERP system to integrate first
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.GetBusinessesWithIntegration(1);

            bool isIntegratedWithERP = false;
            foreach (var business in results)
            {
                if (bid == business.id)
                {
                    isIntegratedWithERP = true;
                    break;
                }
            }

            if (isIntegratedWithERP)
            {
                string queueName = ApplicationConstants.ERP_QBD_QUEUE;
                CloudStorageAccount synchStorageAccount = CloudStorageAccount.Parse(
                Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

                // Create the queue client
                CloudQueueClient queueClient = synchStorageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue
                CloudQueue queue = queueClient.GetQueueReference(queueName);

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                // Create a message and add it to the queue
                string messageString = "02-" + operationCode + ":bid:" + bid + ":upc:" + upc;
                CloudQueueMessage message = new CloudQueueMessage(messageString);
                queue.AddMessage(message);
            }
        }
    }
}