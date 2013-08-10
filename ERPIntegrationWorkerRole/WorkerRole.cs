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
            while (true)
            {
                Thread.Sleep(10000);
                //updateERPFromSynch();
            }
        }

        private void updateSynchFromERP()
        {
            // try to do the auto-sync
            string realmId, accessToken, accessTokenSecret, consumerKey, consumerSecret, dataSourcetype;
            accessToken = "lvprdsHdmwhxqxhwReuLgYSJyUtpUTTDBuMPS3frqLKRE5og";
            accessTokenSecret = "t9m89H4myvEvVq3oi3uac91jwV4r8sjSeWJZ3HFh";
            consumerKey = "qyprdChIG6ax7TK3OWyp6ZIygWNJwj";
            consumerSecret = "gFNdGdTaye35jSd9AYEeqqHY68KXdyEFD7p5x352";
            dataSourcetype = "QBD";
            realmId = "738592490";

        }

        private void updateERPFromSynch()
        {
            string message = MessageProcessor.RetrieveMessageFromSynchStorage();
            if (message != null)
            {
                string[] elements = message.Split(':');
                int rid = Convert.ToInt32(elements[1]);
                int bid = Convert.ToInt32(elements[2]);
                string realmId, accessToken, accessTokenSecret, consumerKey, consumerSecret, dataSourcetype;
                accessToken = "lvprdsHdmwhxqxhwReuLgYSJyUtpUTTDBuMPS3frqLKRE5og";
                accessTokenSecret = "t9m89H4myvEvVq3oi3uac91jwV4r8sjSeWJZ3HFh";
                consumerKey = "qyprdChIG6ax7TK3OWyp6ZIygWNJwj";
                consumerSecret = "gFNdGdTaye35jSd9AYEeqqHY68KXdyEFD7p5x352";
                dataSourcetype = "QBD";
                realmId = "738592490";

                QuickBookIntegration.QBDIntegrator qbdIntegrator = new QuickBookIntegration.QBDIntegrator(bid, realmId, accessToken, accessTokenSecret, consumerKey, consumerSecret, dataSourcetype);

                qbdIntegrator.createInvoiceInQBD(rid);
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
