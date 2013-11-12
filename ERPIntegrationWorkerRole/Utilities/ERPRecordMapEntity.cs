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
    public class ERPRecordMapEntity : TableEntity
    {
        public ERPRecordMapEntity(int bid, string invoiceId)
        {
            this.PartitionKey = bid.ToString();
            this.RowKey = invoiceId;
        }

        public ERPRecordMapEntity() { }

        public int rid { get; set; }

    }
}
