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
    public class DCThumbnail
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public string MimeType { get; set; }
        [DataMember]
        public DCFile File { get; set; }
    }
}