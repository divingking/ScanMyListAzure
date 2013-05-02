using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ScanMyListWebRole
{
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
        [DataMember]
        public double price;
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
    public class NewUser
    {
        [DataMember]
        public string login { get; set; }
        [DataMember]
        public string pass { get; set; }
        [DataMember]
        public string fname { get; set; }
        [DataMember]
        public string mname { get; set; }
        [DataMember]
        public string lname { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string business { get; set; }
        [DataMember]
        public int tier { get; set; }
    }

    [DataContract]
    public class User
    {
        [DataMember]
        public string login { get; set; }
        [DataMember]
        public string pass { get; set; }
    }
}