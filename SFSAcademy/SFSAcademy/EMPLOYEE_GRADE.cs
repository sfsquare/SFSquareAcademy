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
    
    public partial class EMPLOYEE_GRADE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMPLOYEE_GRADE()
        {
            this.EMPLOYEEs = new HashSet<EMPLOYEE>();
            this.ARCHIVED_EMPLOYEE = new HashSet<ARCHIVED_EMPLOYEE>();
        }
    
        public int ID { get; set; }
        public string GRADE_CODE { get; set; }
        public string GRADE_NAME { get; set; }
        public string DESCR { get; set; }
        public bool IS_ACT { get; set; }
        public Nullable<int> PRIR { get; set; }
        public Nullable<decimal> MAX_DILY_HRS { get; set; }
        public Nullable<decimal> MAX_WKILY_HRS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPLOYEE> EMPLOYEEs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_EMPLOYEE> ARCHIVED_EMPLOYEE { get; set; }
    }
}
