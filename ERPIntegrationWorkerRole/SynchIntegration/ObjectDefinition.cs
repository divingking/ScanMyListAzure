using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPIntegrationWorkerRole.SynchIntegration
{
    using System.Runtime.Serialization;

    public class SynchBusiness
    {
        [DataMember]
        public int id;
        [DataMember]
        public string name;
        [DataMember]
        public string address;
        [DataMember]
        public int zip;
        [DataMember]
        public string email;
        [DataMember]
        public string category;
        [DataMember]
        public int integration;
        [DataMember]
        public int tier;
        [DataMember]
        public string phoneNumber;
    }

    [DataContract]
    public class SynchProduct
    {
        [DataMember]
        public string upc { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string detail { get; set; }
        [DataMember]
        public SynchBusiness supplier { get; set; }
        [DataMember]
        public SynchBusiness customer { get; set; }
        [DataMember]
        public int quantity { get; set; }
        [DataMember]
        public int owner { get; set; }
        [DataMember]
        public int leadTime { get; set; }
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public double price { get; set; }
        [DataMember]
        public int productCategory { get; set; }
    }

    [DataContract]
    public class SynchRecordProduct
    {
        [DataMember]
        public string upc { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string detail { get; set; }
        [DataMember]
        public int supplier { get; set; }
        [DataMember]
        public int customer { get; set; }
        [DataMember]
        public int quantity { get; set; }
        [DataMember]
        public double price { get; set; }
        [DataMember]
        public string note { get; set; }
    }

    [DataContract]
    public class SynchRecord
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public int account { get; set; }
        [DataMember]
        public int business { get; set; }
        [DataMember]
        public long date { get; set; }
        [DataMember]
        public List<SynchRecordProduct> products { get; set; }
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public int category { get; set; }
        [DataMember]
        public string sessionId { get; set; }
        [DataMember]
        public string comment { get; set; }
    }


    [DataContract]
    public class SynchUser
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string login { get; set; }
        [DataMember]
        public string pass { get; set; }
        [DataMember]
        public int business { get; set; }
        [DataMember]
        public int tier { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string sessionId { get; set; }
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public string lastName { get; set; }
        [DataMember]
        public string phoneNumber { get; set; }
    }

}