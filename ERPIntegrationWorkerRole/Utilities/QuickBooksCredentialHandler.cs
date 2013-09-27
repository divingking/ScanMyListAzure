using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;

// for table storage
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace ERPIntegrationWorkerRole.Utilities
{
    class QuickBooksCredentialHandler : ICredentialHandler
    {
        public int bid;
        /*
        public string realmId;
        string accessToken;
        string accessTokenSecret;
        string consumerKey;
        string consumerSecret;
        string dataSourcetype;
        */

        public QuickBooksCredentialHandler(int bid)
        {
            this.bid = bid;
            /*
            realmId = null;
            accessToken = null;
            accessTokenSecret = null;
            consumerKey = null;
            consumerSecret = null;
            dataSourcetype = null;
             * */
        }

        public void verifyCredentialForUser() { }

        public void updateCredentialOnServer()
        {

        }

        public TableEntity getCredentialFromSynchServer()
        {
            // Retrieve the storage account from the connection string.
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                           Microsoft.WindowsAzure.CloudConfigurationManager.GetSetting("SynchStorageConnection"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("quickbooksinfo");

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<QuickBooksCredentialEntity>("qbd", bid.ToString());

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                QuickBooksCredentialEntity retrievedCredential = (QuickBooksCredentialEntity)retrievedResult.Result;
                return retrievedCredential;
            }
            else
                return null;
        }

        public void getCredentialFromErpServer()
        {

        }
    }
}
