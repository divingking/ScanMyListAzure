using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SynchWebRole.Models
{
    [DataContract]
    public class SynchRecord
    {
        
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public int category { get; set; }

        [DataMember]
        public int accountId { get; set; }

        [DataMember]
        public int ownerId { get; set; }

        [DataMember]
        public int clientId { get; set; }

        [DataMember]
        public int status { get; set; }

        [DataMember]
        public string title { get; set; }

        [DataMember]
        public string comment { get; set; }

        [DataMember]
        public System.DateTimeOffset transactionDate { get; set; }

        [DataMember]
        public Nullable<System.DateTimeOffset> deliveryDate { get; set; }

        [DataMember]
        public List<SynchRecordLine> recordLines { get; set; }
    }
}