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
    class ERPBusinessMapEntity : TableEntity
    {
        public ERPBusinessMapEntity(int bid, string erpUniqueBusinessId)
        {
            this.PartitionKey = bid.ToString();
            this.RowKey = erpUniqueBusinessId;
        }

        public ERPBusinessMapEntity() { }

        public string nameFromSynch { get; set; }

        public string nameFromERP { get; set; }
    }
}
