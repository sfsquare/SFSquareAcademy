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
    
    public partial class CCE_REPORTS
    {
        public int ID { get; set; }
        public Nullable<int> OBSERVABLE_ID { get; set; }
        public string OBSERVABLE_TYPE { get; set; }
        public Nullable<int> STDNT_ID { get; set; }
        public Nullable<int> BTCH_ID { get; set; }
        public string GRADE_STRNG { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<int> EXAM_ID { get; set; }
    
        public virtual BATCH BATCH { get; set; }
        public virtual EXAM EXAM { get; set; }
        public virtual OBSERVATION OBSERVATION { get; set; }
        public virtual STUDENT STUDENT { get; set; }
    }
}
