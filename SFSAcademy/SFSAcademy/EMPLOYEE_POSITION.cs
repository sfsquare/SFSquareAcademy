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
    
    public partial class EMPLOYEE_POSITION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMPLOYEE_POSITION()
        {
            this.EMPLOYEEs = new HashSet<EMPLOYEE>();
            this.ARCHIVED_EMPLOYEE = new HashSet<ARCHIVED_EMPLOYEE>();
        }
    
        public int ID { get; set; }
        public string POS_NAME { get; set; }
        public string POS_DESCR { get; set; }
        public bool IS_ACT { get; set; }
        public Nullable<int> EMP_CAT_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPLOYEE> EMPLOYEEs { get; set; }
        public virtual EMPLOYEE_CATEGORY EMPLOYEE_CATEGORY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_EMPLOYEE> ARCHIVED_EMPLOYEE { get; set; }
    }
}
