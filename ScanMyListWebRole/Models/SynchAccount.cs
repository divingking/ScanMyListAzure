using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace SynchWebRole.Models
{
    [DataContract]
    public class SynchAccount
    {
        [DataMember]
        public int id { get; set; }
        
        [DataMember]
        public int businessId { get; set; }
        
        [DataMember]        
        public string login { get; set; }

        [DataMember]
        public string password { get; set; }

        [DataMember]
        public int tier { get; set; }

        [DataMember]
        public string firstName { get; set; }

        [DataMember]
        public string lastName { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string phoneNumber { get; set; }

        [DataMember]
        public string sessionId { get; set; }

        [DataMember]
        public string deviceId { get; set; }
    }
}