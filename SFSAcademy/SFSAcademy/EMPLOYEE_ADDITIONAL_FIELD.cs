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
    
    public partial class EMPLOYEE_ADDITIONAL_FIELD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EMPLOYEE_ADDITIONAL_FIELD()
        {
            this.EMPLOYEE_ADDITIONAL_DETAIL = new HashSet<EMPLOYEE_ADDITIONAL_DETAIL>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string STAT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPLOYEE_ADDITIONAL_DETAIL> EMPLOYEE_ADDITIONAL_DETAIL { get; set; }
    }
}
