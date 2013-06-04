namespace SynchWebRole.REST_Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.Web.Script.Serialization;


    public partial class SynchDataService : ISynchDataService
    {
        public int ProductCount(string upc, int aid, string sessionId)
        {
            // Not in use for now. 
            // For the future. 
            // Adding session check in the future.
            /*ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            var results = context.ProductCount(upc);
            IEnumerator<ProductCountResult> counts = results.GetEnumerator();
            if (counts.MoveNext())
            {
                return (int)counts.Current.count;
            }
            else
            {
                return 0;
            }*/

            throw new NotImplementedException();
        }
    }
}
