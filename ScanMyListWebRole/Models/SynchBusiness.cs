using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SynchWebRole.Models
{

    [DataContract]
    public class SynchBusiness
    {

        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public int integration { get; set; }

        [DataMember]
        public int tier { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public string postalCode { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string phoneNumber { get; set; }
    }
}