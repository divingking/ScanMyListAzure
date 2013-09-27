using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;

namespace ERPIntegrationWorkerRole.Utilities
{
    public class QuickBooksCredentialEntity : TableEntity
    {
        public QuickBooksCredentialEntity(string businessName, int bid, string dataSourcetype)
        {
            this.PartitionKey = dataSourcetype;
            this.RowKey = bid.ToString();
        }

        public QuickBooksCredentialEntity() { }

        public string realmId { get; set; }

        public string accessToken { get; set; }

        public string accessTokenSecret { get; set; }

        public string consumerKey { get; set; }

        public string consumerSecret { get; set; }

        public string dataSourcetype { get; set; }
    }   
}
