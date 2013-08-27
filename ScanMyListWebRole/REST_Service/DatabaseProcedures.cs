using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SynchWebRole
{
    public class DatabaseProcedures
    {
        public static GetProductByUpcResult GetProductByUPC(string upc, int cid)
        {
            /*ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            try
            {
                var result = context.GetProductByUPC(cid, upc);
                IEnumerator<GetProductByUPCResult> enumerator = result.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                
                throw;
            }*/

            throw new NotImplementedException();
        }
    }
}