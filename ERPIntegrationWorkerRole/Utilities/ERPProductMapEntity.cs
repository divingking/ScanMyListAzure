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
    public class ERPProductMapEntity : TableEntity
    {
        public ERPProductMapEntity(int bid, string erpUniqueProductId)
        {
            this.PartitionKey = bid.ToString();
            this.RowKey = erpUniqueProductId;
        }

        public ERPProductMapEntity() { }

        public string upc { get; set; }

    }
}
