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
    public class DCParamDefaultValue
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Value { get; set; }
    }
}