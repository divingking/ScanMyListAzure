using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SynchWebRole.Models
{
    [DataContract]
    public class SynchRecordLine
    {

        [DataMember]
        public int recordId { get; set; }

        [DataMember]
        public string upc { get; set; }

        [DataMember]
        public int quantity { get; set; }

        [DataMember]
        public decimal price { get; set; }

        [DataMember]
        public string note { get; set; }
    }
}