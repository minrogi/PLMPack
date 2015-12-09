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
    public class DCCardboardQuality
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public double SurfacicMass { get; set; }
        [DataMember]
        public double[] Rigidity { get; set; }
        [DataMember]
        public double YoungModulus { get; set; }
        [DataMember]
        public double ECT { get; set; }
    }
}