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
            var results = context.GetAccountById(aid);
            IEnumerator<GetAccountByIdResult> accountEnumerator = results.GetEnumerator();
            if (accountEnumerator.MoveNext())
            {
                GetAccountByIdResult result = accountEnumerator.Current;
                if (!sessionId.Equals(result.session_id))
                {
                    throw new WebFaultException<string>("session expired", HttpStatusCode.Unauthorized);
                }
            }
            else
                throw new WebFaultException<string>("account does not exist", HttpStatusCode.Unauthorized);

        }
    }
}