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
    
    public partial class FINANCE_FEE_PARTICULAR
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string DESCR { get; set; }
        public Nullable<decimal> AMT { get; set; }
        public Nullable<int> FIN_FEE_CAT_ID { get; set; }
        public Nullable<int> STDNT_CAT_ID { get; set; }
        public string ADMSN_NO { get; set; }
        public Nullable<int> STDNT_ID { get; set; }
        public string IS_DEL { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
    
        public virtual FINANCE_FEE_CATGEORY FINANCE_FEE_CATGEORY { get; set; }
        public virtual STUDENT_CATGEORY STUDENT_CATGEORY { get; set; }
        public virtual STUDENT STUDENT { get; set; }
    }
}
