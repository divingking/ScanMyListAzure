using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SynchWebRole.REST_Service;
using System.ServiceModel.Web;
using System.Net;

namespace SynchWebRole.Library_Class
{
    public class TierController
    {

        public static List<Record> filterRecordWithAccountTier(List<Record> originalList, int aid)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            GetAccountByIdResult result = (GetAccountByIdResult) context.GetAccountById(aid);
            switch ((int)result.tier)
            {
                case (int) AccountTier.sales:
                    List<Record> resultList = new List<Record>();
                    foreach (Record r in originalList)
                    {
                        if (r.account == aid)
                            resultList.Add(r);
                    }
                    return resultList;
                case (int)AccountTier.manager:      // a placeholder for now
                    return originalList;
                case (int)AccountTier.ceo:
                    return originalList;
                default:
                    return null;
            }
        }

        public static void validateAccessToRecord(int recordAid, int recordBid, int aid, int bid)
        {
            // validate business access
            if (recordBid != bid)
                throw new WebFaultException<string>("record does not belong to business", HttpStatusCode.Unauthorized);

            // validate account access
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            GetAccountByIdResult result = (GetAccountByIdResult)context.GetAccountById(aid);

            if (recordAid != aid)
            {
                switch ((int)result.tier)
                {
                    case (int)AccountTier.sales:
                        throw new WebFaultException<string>("record does not belong to account", HttpStatusCode.Unauthorized);
                    case (int)AccountTier.manager:      // a placeholder for now
                        return;
                    case (int)AccountTier.ceo:
                        return;
                    default:
                        return;
                }
            }
        }
    }
}