#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
#endregion

namespace PLMPack
{
    [DataContract]
    public class DCCardboardProfile
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public double Thickness { get; set; }
    }
}