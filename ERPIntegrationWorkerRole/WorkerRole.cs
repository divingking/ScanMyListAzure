using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

using ERPIntegrationWorkerRole.SynchIntegration;

namespace ERPIntegrationWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            while (true)
            {
                Thread.Sleep(10000);
                DataflowWorkers.IntegrationInitializer initializer = new DataflowWorkers.IntegrationInitializer();

                /*
                 * What is missing:
                 * 1. does not delete inactive inventory on Synch
                 * 2. does not create new inventory on Synch
                 * 3. does not fully check if product is legitimate (not dummy product)
                 * 4. does not use barcode/upc as primary key to reference products; uses product names instead, which have duplicates
                */

                initializer.updateERPFromSynch();
                initializer.updateSynchFromERP();
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
