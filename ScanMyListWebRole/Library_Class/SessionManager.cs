using SynchWebRole.Library_Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web.Script.Serialization;
using System.ServiceModel.Web;


namespace SynchWebRole.Library_Class
{
    public class SessionManager
    {
        public static void checkSession(int aid, string sessionId)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            GetAccountByIdResult result = (GetAccountByIdResult) context.GetAccountById(aid);

            if (!sessionId.Equals(result.session_id))
            {
                throw new WebFaultException<string>("session expired", HttpStatusCode.Unauthorized);
            }
        }
    }
}