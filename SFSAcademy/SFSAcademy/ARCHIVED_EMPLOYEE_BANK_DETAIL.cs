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
    
    public partial class ARCHIVED_EMPLOYEE_BANK_DETAIL
    {
        public int ID { get; set; }
        public Nullable<int> EMP_ID { get; set; }
        public string BANK_INFO { get; set; }
        public Nullable<int> BANK_FLD_ID { get; set; }
    
        public virtual BANK_FIELD BANK_FIELD { get; set; }
        public virtual ARCHIVED_EMPLOYEE ARCHIVED_EMPLOYEE { get; set; }
    }
}
