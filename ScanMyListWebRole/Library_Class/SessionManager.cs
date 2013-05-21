using SynchWebRole.Library_Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web.Script.Serialization;

namespace SynchWebRole.Library_Class
{
    public class SessionManager
    {
        public static void CheckSession(int aid, string sessionId)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            string retrievedSessionId = null;
            context.GetSessionId(aid, ref retrievedSessionId);

            if (!sessionId.Equals(retrievedSessionId))
            {
                throw new FaultException("Session Expired! Should be " + retrievedSessionId + "; instead we get " + sessionId);
            }
        }
    }
}