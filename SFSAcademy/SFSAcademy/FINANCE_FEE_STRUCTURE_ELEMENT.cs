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
    
    public partial class FINANCE_FEE_STRUCTURE_ELEMENT
    {
        public int ID { get; set; }
        public Nullable<decimal> AMT { get; set; }
        public string LBL { get; set; }
        public Nullable<int> BTCH_ID { get; set; }
        public Nullable<int> STDNT_CAT_ID { get; set; }
        public Nullable<int> STDNT_ID { get; set; }
        public Nullable<int> PARNT_ID { get; set; }
        public Nullable<int> FEE_CLCT_ID { get; set; }
        public string DEL { get; set; }
    
        public virtual FINANCE_FEE_COLLECTION FINANCE_FEE_COLLECTION { get; set; }
        public virtual USER USER { get; set; }
        public virtual STUDENT_CATGEORY STUDENT_CATGEORY { get; set; }
        public virtual STUDENT STUDENT { get; set; }
        public virtual BATCH BATCH { get; set; }
    }
}
