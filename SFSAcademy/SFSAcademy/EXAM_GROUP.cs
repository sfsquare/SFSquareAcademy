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
    
    public partial class EXAM_GROUP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EXAM_GROUP()
        {
            this.EXAMs = new HashSet<EXAM>();
            this.GROUPED_EXAM = new HashSet<GROUPED_EXAM>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public Nullable<int> BTCH_ID { get; set; }
        public string EXAM_TYPE { get; set; }
        public bool IS_PUB { get; set; }
        public bool RSULT_PUB { get; set; }
        public Nullable<System.DateTime> EXAM_DATE { get; set; }
        public bool IS_FINAL_EXAM { get; set; }
        public Nullable<int> CCE_EXAM_CAT_ID { get; set; }
    
        public virtual CCE_EXAM_CATEGORY CCE_EXAM_CATEGORY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXAM> EXAMs { get; set; }
        public virtual BATCH BATCH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUPED_EXAM> GROUPED_EXAM { get; set; }
    }
}
