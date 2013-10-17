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
    class ERPRecordMapEntity : TableEntity
    {
        public ERPRecordMapEntity(int bid, int rid)
        {
            this.PartitionKey = bid.ToString();
            this.RowKey = rid.ToString();
        }

        public ERPRecordMapEntity() { }

        public string uniqueIdFromERP { get; set; }

        public bool synchronized { get; set; }
    }
}
