using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Table;

namespace ERPIntegrationWorkerRole.Utilities
{
    public interface ICredentialHandler
    {
        void verifyCredentialForUser();

        void updateCredentialOnServer();

        TableEntity getCredentialFromSynchServer();

        void getCredentialFromErpServer();
    }
}
