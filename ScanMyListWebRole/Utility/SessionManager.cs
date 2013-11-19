using SynchWebRole.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.Web.Script.Serialization;
using System.ServiceModel.Web;


namespace SynchWebRole.Utility
{
    public class SessionManager
    {
        public static void checkSession(SynchDatabaseDataContext context, int accountId, string sessionId)
        {
            var results = context.GetAccountById(accountId);
            IEnumerator<GetAccountByIdResult> accountEnum = results.GetEnumerator();
            if (accountEnum.MoveNext())
            {
                if (!sessionId.Equals(accountEnum.Current.sessionId))
                {
                    throw new WebFaultException<string>("session expired", HttpStatusCode.Unauthorized);
                }
            }
            else
                throw new WebFaultException<string>("account does not exist", HttpStatusCode.NotFound);

        }
    }
}