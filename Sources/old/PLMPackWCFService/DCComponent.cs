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
    public class DCComponent
    {
        [DataMember]
        public Guid CGuid { get; set; }
        [DataMember]
        public DCFile File { get; set; }
        [DataMember]
        public DCMajorationSet[] MajoSets { get; set; }
        [DataMember]
        public DCParamDefaultValue[] ParamDefaults { get; set; }
    }
}