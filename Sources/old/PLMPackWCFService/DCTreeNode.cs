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
    public class DCTreeNode
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid ParentNodeID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DCNodeTypeEnum NodeType { get; set; }
        [DataMember]
        public bool HasChildrens { get; set; }
        [DataMember]
        public DCThumbnail Thumbnail { get; set; }
        [DataMember]
        public DCFile Document { get; set; }
        [DataMember]
        public DCComponent Component { get; set; }
    }

    [DataContract(Name = "NodeType")]
    public enum DCNodeTypeEnum
    {
        [EnumMember]
        NTBranch,
        [EnumMember]
        NTDocument,
        [EnumMember]
        NTComponent   
    }
}