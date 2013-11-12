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
    public class SynchStorageReader
    {
        private int synchBusinessId;

        public SynchStorageReader(int bid)
        {
            this.synchBusinessId = bid;
        }

        public QuickBooksCredentialEntity getCredentialFromSynchServer()
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("quickbooksinfo");
            table.CreateIfNotExists();

            TableOperation retrieveOperation = TableOperation.Retrieve<QuickBooksCredentialEntity>("qbd", synchBusinessId.ToString());
            TableResult retrievedResult = table.Execute(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                QuickBooksCredentialEntity retrievedCredential = (QuickBooksCredentialEntity)retrievedResult.Result;
                return retrievedCredential;
            }
            else
                return null;
        }

        public Dictionary<string, string> getUpcToItemIdMap(string productMappingStorage)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(productMappingStorage);
            table.CreateIfNotExists();

            TableQuery<Utilities.ERPProductMapEntity> query = new TableQuery<Utilities.ERPProductMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            foreach (Utilities.ERPProductMapEntity entity in table.ExecuteQuery(query))
            {
                result.Add(entity.upc, entity.RowKey);
            }
            return result;
        }

        public Dictionary<string, string> getItemIdToUpcMap(string productMappingStorage)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(productMappingStorage);
            table.CreateIfNotExists();

            TableQuery<Utilities.ERPProductMapEntity> query = new TableQuery<Utilities.ERPProductMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            foreach (Utilities.ERPProductMapEntity entity in table.ExecuteQuery(query))
            {
                result.Add(entity.RowKey, entity.upc);
            }
            return result;
        }

        public Dictionary<int, string> getSynchCidToCustomerIdMap(string businessMappingStorage)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(businessMappingStorage);
            table.CreateIfNotExists();

            TableQuery<Utilities.ERPBusinessMapEntity> query = new TableQuery<Utilities.ERPBusinessMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            foreach (Utilities.ERPBusinessMapEntity entity in table.ExecuteQuery(query))
            {
                result.Add(entity.idFromSynch, entity.RowKey);
            }
            return result;
        }

        public Dictionary<string, int> getCustomerIdToSynchCidMap(string businessMappingStorage)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(businessMappingStorage);
            table.CreateIfNotExists();

            TableQuery<Utilities.ERPBusinessMapEntity> query = new TableQuery<Utilities.ERPBusinessMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            foreach (Utilities.ERPBusinessMapEntity entity in table.ExecuteQuery(query))
            {
                result.Add(entity.RowKey, entity.idFromSynch);
            }
            return result;
        }

        public Dictionary<string, Utilities.ERPRecordMapEntity> getTransactionIdToEntityMap(string recordMappingStorage)
        {
            Dictionary<string, Utilities.ERPRecordMapEntity> result = new Dictionary<string, Utilities.ERPRecordMapEntity>();
            // make Table Storage Connection
            // Retrieve the storage account from the connection string.
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference(recordMappingStorage);
            table.CreateIfNotExists();

            TableQuery<Utilities.ERPRecordMapEntity> query = new TableQuery<Utilities.ERPRecordMapEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, synchBusinessId.ToString()));

            foreach (Utilities.ERPRecordMapEntity entity in table.ExecuteQuery(query))
            {
                result.Add(entity.RowKey, entity);
            }
            return result;
        }
    }
}
