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
    
    public partial class TreeNodeGroupShare
    {
        public string TreeNodeId { get; set; }
        public string GroupId { get; set; }
    
        public virtual Group Group { get; set; }
        public virtual TreeNode TreeNode { get; set; }
    }
}
