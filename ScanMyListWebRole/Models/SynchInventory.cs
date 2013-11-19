using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SynchWebRole.Models
{
    [DataContract]
    public class SynchInventory
    {

        [DataMember]
        public int businessId { get; set; }

        [DataMember]
        public string upc { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public decimal defaultPrice { get; set; }

        [DataMember]
        public string detail { get; set; }

        [DataMember]
        public int leadTime { get; set; }

        [DataMember]
        public int quantityAvailable { get; set; }

        [DataMember]
        public int category { get; set; }

        [DataMember]
        public string location { get; set; }
    }
}