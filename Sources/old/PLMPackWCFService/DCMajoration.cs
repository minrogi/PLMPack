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
    public class DCMajoration
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Value { get; set; }
    }

    [DataContract]
    public class DCMajorationSet
    {
        [DataMember]
        public DCCardboardProfile Profile { get; set; }
        [DataMember]
        public DCMajoration[] Majorations { get; set; }
    }
}