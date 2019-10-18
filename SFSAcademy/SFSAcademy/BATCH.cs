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
    
    public partial class BATCH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BATCH()
        {
            this.ASSESSMENT_SCORE = new HashSet<ASSESSMENT_SCORE>();
            this.ATTENDENCEs = new HashSet<ATTENDENCE>();
            this.CCE_REPORTS = new HashSet<CCE_REPORTS>();
            this.CLASS_TIMING = new HashSet<CLASS_TIMING>();
            this.EXAM_GROUP = new HashSet<EXAM_GROUP>();
            this.FINANCE_FEE_CATGEORY = new HashSet<FINANCE_FEE_CATGEORY>();
            this.FINANCE_FEE_COLLECTION = new HashSet<FINANCE_FEE_COLLECTION>();
            this.FINANCE_FEE_STRUCTURE_ELEMENT = new HashSet<FINANCE_FEE_STRUCTURE_ELEMENT>();
            this.GRADING_LEVEL = new HashSet<GRADING_LEVEL>();
            this.PERIOD_ENTRIES = new HashSet<PERIOD_ENTRIES>();
            this.STUDENT_SUBJECT = new HashSet<STUDENT_SUBJECT>();
            this.SUBJECTs = new HashSet<SUBJECT>();
            this.SUBJECT_LEAVE = new HashSet<SUBJECT_LEAVE>();
            this.TIMETABLE_ENTRY = new HashSet<TIMETABLE_ENTRY>();
            this.WEEKDAYs = new HashSet<WEEKDAY>();
            this.ELECTIVE_GROUP = new HashSet<ELECTIVE_GROUP>();
            this.BATCH_EVENT = new HashSet<BATCH_EVENT>();
            this.BATCH_SEAT = new HashSet<BATCH_SEAT>();
            this.BATCH_STUDENT = new HashSet<BATCH_STUDENT>();
            this.GROUPED_BATCH = new HashSet<GROUPED_BATCH>();
            this.GROUPED_EXAM_REPORT = new HashSet<GROUPED_EXAM_REPORT>();
            this.GROUPED_EXAM = new HashSet<GROUPED_EXAM>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public Nullable<int> CRS_ID { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public bool IS_DEL { get; set; }
        public string EMP_ID { get; set; }
        public bool IS_ACT { get; set; }
        public Nullable<int> GRADING_TYPE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSESSMENT_SCORE> ASSESSMENT_SCORE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTENDENCE> ATTENDENCEs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCE_REPORTS> CCE_REPORTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLASS_TIMING> CLASS_TIMING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXAM_GROUP> EXAM_GROUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE_CATGEORY> FINANCE_FEE_CATGEORY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE_COLLECTION> FINANCE_FEE_COLLECTION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE_STRUCTURE_ELEMENT> FINANCE_FEE_STRUCTURE_ELEMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GRADING_LEVEL> GRADING_LEVEL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERIOD_ENTRIES> PERIOD_ENTRIES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_SUBJECT> STUDENT_SUBJECT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBJECT> SUBJECTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBJECT_LEAVE> SUBJECT_LEAVE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TIMETABLE_ENTRY> TIMETABLE_ENTRY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WEEKDAY> WEEKDAYs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ELECTIVE_GROUP> ELECTIVE_GROUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BATCH_EVENT> BATCH_EVENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BATCH_SEAT> BATCH_SEAT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BATCH_STUDENT> BATCH_STUDENT { get; set; }
        public virtual COURSE COURSE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUPED_BATCH> GROUPED_BATCH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUPED_EXAM_REPORT> GROUPED_EXAM_REPORT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUPED_EXAM> GROUPED_EXAM { get; set; }
    }
}
