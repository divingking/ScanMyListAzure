using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScanMyListWebRole
{
    public class DatabaseProcedures
    {
        public static GetProductByUPCResult GetProductByUPC(string upc, int cid)
        {
            ScanMyListDatabaseDataContext context = new ScanMyListDatabaseDataContext();
            try
            {
                var result = context.GetProductByUPC(upc, cid);
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
            }
        }
    }
}