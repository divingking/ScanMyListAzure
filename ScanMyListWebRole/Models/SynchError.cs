using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SynchWebRole.Models
{
    [DataContract]
    public class SynchError
    {
        public class SynchErrorCode
        {
            public const int ACTION_POST = 0;
            public const int ACTION_GET = 1;
            public const int ACTION_PUT = 2;
            public const int ACTION_DELETE = 3;

            public const int SERVICE_ACCOUNT = 0;
            public const int SERVICE_BUSINESS = 1;
            public const int SERVICE_CUSTOMER = 2;
            public const int SERVICE_SUPPLIER = 3;
            public const int SERVICE_INVENTORY = 4;
            public const int SERVICE_RECORD = 5;

            // account manager errors

            // business manager errors

            // customer manager errors

            // supplier manager errors

            // inventory manager errors

            // record manager errors

        }

        public string errorCode { get; set; }
        public string errorMessage { get; set; }

        public SynchError(int action, int service, string message)
        {
            errorCode = action.ToString() + service.ToString();
            errorMessage = message;
        }
    }
}