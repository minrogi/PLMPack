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
    public class DCFile
    {
        [DataMember]
        public Guid Guid { get; set; }
        [DataMember]
        public string Extension { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
    }
}