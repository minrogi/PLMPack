//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PLMPackModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Thumbnail
    {
        public Thumbnail()
        {
            this.TreeNodes = new HashSet<TreeNode>();
        }
    
        public int Id { get; set; }
        public string FileGuid { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string MimeType { get; set; }
    
        public virtual File File { get; set; }
        public virtual ICollection<TreeNode> TreeNodes { get; set; }
    }
}
