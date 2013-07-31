using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

using ERPIntegrationWorkerRole.SynchIntegration;

namespace ERPIntegrationWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("ERPIntegrationWorkerRole entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                int rid = MessageProcessor.RetrieveMessageFromSynchStorage();

                // QuickBookIntegration.QBDIntegrator.createInvoiceInQBD(rid);

                if (rid != -1)
                {
                    //QuickBookIntegration.QBDIntegrator.createInvoiceInQBD(rid);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            return base.OnStart();
        }
    }
}
