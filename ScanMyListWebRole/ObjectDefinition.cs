﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ScanMyListWebRole
{
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
        public Supplier supplier { get; set; }
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
        public int cid { get; set; }
        [DataMember]
        public long date { get; set; }
        [DataMember]
        public List<Product> products { get; set; }
        [DataMember]
        public bool sent { get; set; }
        [DataMember]
        public bool scanIn { get; set; }
    }

    [DataContract]
    public class Supplier
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string business { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public double price { get; set; }
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