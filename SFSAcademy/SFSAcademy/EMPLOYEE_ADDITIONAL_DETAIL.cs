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
    
    public partial class EMPLOYEE_ADDITIONAL_DETAIL
    {
        public int ID { get; set; }
        public Nullable<int> EMP_ID { get; set; }
        public Nullable<int> ADDL_FLD_ID { get; set; }
        public string ADDL_INFO { get; set; }
    
        public virtual EMPLOYEE EMPLOYEE { get; set; }
        public virtual STUDENT_ADDITIONAL_FIELD STUDENT_ADDITIONAL_FIELD { get; set; }
    }
}
