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
    
    public partial class GROUP_MEMBER
    {
        public int ID { get; set; }
        public Nullable<int> GROUP_ID { get; set; }
        public Nullable<int> USRID { get; set; }
        public bool IS_ADMIN { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
    
        public virtual GROUP GROUP { get; set; }
    }
}
