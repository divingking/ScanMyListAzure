namespace SynchWebRole.ServiceManager
{
    using System;
    using SynchWebRole.Utility;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.Web.Script.Serialization;


    public partial class SynchDataService : ISynchDataService
    {
        public HttpRequestMessage Request { get; set; }

        public int CountItemForBusiness(int bid, int aid, string sessionId, string item)
        {
            SessionManager.checkSession(aid, sessionId);            

            int count = TierController.countItemForBusinessWithAccount(bid, aid, item);
            return count;
        }

    }
}
