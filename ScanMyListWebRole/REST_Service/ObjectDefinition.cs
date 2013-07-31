namespace SynchWebRole
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class Business
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
    }

    [DataContract]
    public class Product
    {
        [DataMember]
        public string upc { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string detail { get; set; }
        [DataMember]
        public Business supplier { get; set; }
        [DataMember]
        public Business customer { get; set; }
        [DataMember]
        public int quantity { get; set; }
        [DataMember]
        public int owner { get; set; }
        [DataMember]
        public int leadTime { get; set; }
        [DataMember]
        public string location { get; set; }
    }

    [DataContract]
    public class RecordProduct
    {
        [DataMember]
        public string upc { get; set; }
        [DataMember]
        public string name { get; set; }
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
    public class Record
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
        public List<RecordProduct> products { get; set; }
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
    public class Order
    {
        [DataMember]
        public int oid { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public int bid { get; set; }
        [DataMember]
        public long date { get; set; }
        [DataMember]
        public List<Product> products { get; set; }
        [DataMember]
        public bool sent { get; set; }
        [DataMember]
        public bool scanIn { get; set; }
        [DataMember]
        public string sessionId { get; set; }
    }

    [DataContract]
    public class User
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
    }

    [DataContract]
    public class LoginUser
    {
        [DataMember]
        public string login { get; set; }
        [DataMember]
        public string pass { get; set; }
        [DataMember]
        public int device { get; set; }
    }
}