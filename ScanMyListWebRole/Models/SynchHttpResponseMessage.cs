using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace SynchWebRole.Models
{
    [DataContract]
    public class SynchHttpResponseMessage
    {
        public class SynchPagination
        {
            public int pageSize { get; set; }
            public string prevPage { get; set; }
            public string nextPage { get; set; }
        }

        public class SynchResponseMetaData
        {
            public DateTime timestamp {  get; set; }
            public string handler { get; set; }
            public int accountId { get; set; }
            public string sessionId { get; set; }
        }

        [DataMember]
        public SynchResponseMetaData metaData { get; set; }

        [DataMember]
        public HttpStatusCode status { get; set; }

        [DataMember]
        public Object data { get; set; }

        [DataMember]
        public SynchError error { get; set; }

        [DataMember]
        public SynchPagination pagination { get; set; }

        public SynchHttpResponseMessage()
        {
            metaData = null;
            status = HttpStatusCode.BadRequest;
            data = null;
            error = new SynchError(-1, -1, "No Error Message Available");
            pagination = null;
        }
    }
}