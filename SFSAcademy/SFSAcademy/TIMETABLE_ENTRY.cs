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
    
    public partial class TIMETABLE_ENTRY
    {
        public int ID { get; set; }
        public Nullable<int> BTCH_ID { get; set; }
        public Nullable<int> WK_DAY_ID { get; set; }
        public Nullable<int> EMP_ID { get; set; }
        public Nullable<int> TIMT_ID { get; set; }
        public Nullable<int> CLS_TMNG_ID { get; set; }
        public Nullable<int> SUBJ_ID { get; set; }
    
        public virtual BATCH BATCH { get; set; }
        public virtual CLASS_TIMING CLASS_TIMING { get; set; }
        public virtual EMPLOYEE EMPLOYEE { get; set; }
        public virtual SUBJECT SUBJECT { get; set; }
        public virtual TIMETABLE TIMETABLE { get; set; }
        public virtual WEEKDAY WEEKDAY { get; set; }
    }
}
