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
    
    public partial class CCE_WEIGHTAGES_COURSE
    {
        public int ID { get; set; }
        public int CCE_WTAGE_ID { get; set; }
        public int CRS_ID { get; set; }
    
        public virtual CCE_WEIGHTAGE CCE_WEIGHTAGE { get; set; }
        public virtual COURSE COURSE { get; set; }
    }
}
