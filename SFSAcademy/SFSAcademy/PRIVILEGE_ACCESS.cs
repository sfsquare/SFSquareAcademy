//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SFSAcademy
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRIVILEGE_ACCESS
    {
        public int ID { get; set; }
        public Nullable<int> PRIVILEGE_ID { get; set; }
        public Nullable<int> USERS_ACCESS_ID { get; set; }
    
        public virtual USERS_ACCESS USERS_ACCESS { get; set; }
        public virtual PRIVILEGE PRIVILEGE { get; set; }
    }
}
