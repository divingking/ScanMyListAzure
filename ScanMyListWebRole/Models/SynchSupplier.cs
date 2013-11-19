using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SynchWebRole.Models
{
    [DataContract]
    public class SynchSupplier
    {

        [DataMember]
        public int businessId { get; set; }

        [DataMember]
        public int supplierId { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string phoneNumber { get; set; }

        [DataMember]
        public Nullable<int> category { get; set; }

        // from Business table
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string postalCode { get; set; }
    }
}