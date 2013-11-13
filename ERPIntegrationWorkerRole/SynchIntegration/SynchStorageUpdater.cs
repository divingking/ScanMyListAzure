using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// for table storage
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

using ERPIntegrationWorkerRole.Utilities;

namespace ERPIntegrationWorkerRole.SynchIntegration
{
    class SynchStorageUpdater
    {
        private int synchBusinessId;

        public SynchStorageUpdater(int bid)
        {
            synchBusinessId = bid;
        }

        public void createBusinessMapping(string businessMappingStorage, int cid, string erpBusinessId)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(businessMappingStorage);
            table.CreateIfNotExists();

            Utilities.ERPBusinessMapEntity newBusinessMapping = new Utilities.ERPBusinessMapEntity(synchBusinessId, erpBusinessId);
            newBusinessMapping.idFromSynch = cid;

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newBusinessMapping);
            table.Execute(insertOrReplaceOperation);
        }

        public void createProductMapping(string productMappingStorage, string upc, string itemId)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
               Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(productMappingStorage);
            table.CreateIfNotExists();

            Utilities.ERPProductMapEntity newProductMapping = new Utilities.ERPProductMapEntity(synchBusinessId, itemId);
            newProductMapping.upc = upc;

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newProductMapping);
            table.Execute(insertOrReplaceOperation);
        }

        public void createRecordMapping(string recordMappingStorage, int rid, string transactionIdFromQBD)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
               Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(recordMappingStorage);
            table.CreateIfNotExists();

            Utilities.ERPRecordMapEntity newRecordMapping = new Utilities.ERPRecordMapEntity(synchBusinessId, transactionIdFromQBD);
            newRecordMapping.rid = rid;

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(newRecordMapping);

            // Execute the insert operation.
            table.Execute(insertOrReplaceOperation);
        }

        public void deleteBusinessMapping(string businessMappingStorage, string customerId)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                                       Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(businessMappingStorage);
            table.CreateIfNotExists();

            TableOperation retrieveOperation = TableOperation.Retrieve<Utilities.ERPBusinessMapEntity>(synchBusinessId.ToString(), customerId);
            TableResult retrievedResult = table.Execute(retrieveOperation);
            Utilities.ERPBusinessMapEntity deleteEntity = (Utilities.ERPBusinessMapEntity)retrievedResult.Result;

            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                table.Execute(deleteOperation);
            }
        }

        internal void deleteProductMapping(string productMappingStorage, string itemId)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                                                   Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(productMappingStorage);
            table.CreateIfNotExists();

            TableOperation retrieveOperation = TableOperation.Retrieve<Utilities.ERPBusinessMapEntity>(synchBusinessId.ToString(), itemId);
            TableResult retrievedResult = table.Execute(retrieveOperation);
            Utilities.ERPBusinessMapEntity deleteEntity = (Utilities.ERPBusinessMapEntity)retrievedResult.Result;

            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                table.Execute(deleteOperation);
            }
        }
    }
}
