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
    
    public partial class SUBJECT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SUBJECT()
        {
            this.EMPLOYEES_SUBJECT = new HashSet<EMPLOYEES_SUBJECT>();
            this.EXAMs = new HashSet<EXAM>();
            this.PERIOD_ENTRIES = new HashSet<PERIOD_ENTRIES>();
            this.STUDENT_PREVIOUS_SUBJECT_MARK = new HashSet<STUDENT_PREVIOUS_SUBJECT_MARK>();
            this.STUDENT_SUBJECT = new HashSet<STUDENT_SUBJECT>();
            this.SUBJECT_LEAVE = new HashSet<SUBJECT_LEAVE>();
            this.TIMETABLE_ENTRY = new HashSet<TIMETABLE_ENTRY>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string CODE { get; set; }
        public bool NO_EXAMS { get; set; }
        public Nullable<int> MAX_WKILY_CLSES { get; set; }
        public Nullable<int> ELECTIVE_GRP_ID { get; set; }
        public bool IS_DEL { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<decimal> CR_HRS { get; set; }
        public bool PREF_CNSC { get; set; }
        public Nullable<decimal> AMT { get; set; }
        public Nullable<int> BTCH_ID { get; set; }
        public bool LANG { get; set; }
    
        public virtual BATCH BATCH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPLOYEES_SUBJECT> EMPLOYEES_SUBJECT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXAM> EXAMs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERIOD_ENTRIES> PERIOD_ENTRIES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_PREVIOUS_SUBJECT_MARK> STUDENT_PREVIOUS_SUBJECT_MARK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_SUBJECT> STUDENT_SUBJECT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBJECT_LEAVE> SUBJECT_LEAVE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMETABLE_ENTRY> TIMETABLE_ENTRY { get; set; }
    }
}
