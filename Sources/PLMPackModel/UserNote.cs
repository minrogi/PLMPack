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
    
    public partial class UserNote
    {
        public UserNote()
        {
            this.Issues = new HashSet<Issue>();
            this.ParentNote = new HashSet<UserNote>();
        }
    
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ParentNoteId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string Text { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
        public virtual UserNote Childrens { get; set; }
        public virtual ICollection<UserNote> ParentNote { get; set; }
    }
}