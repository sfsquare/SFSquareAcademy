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
    
    public partial class OBSERVATION_GROUP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OBSERVATION_GROUP()
        {
            this.OBSERVATIONs = new HashSet<OBSERVATION>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string HDR_NAME { get; set; }
        public string DESCR { get; set; }
        public Nullable<int> CCE_GRADE_SET_ID { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public string OBSV_KIND { get; set; }
        public Nullable<double> MAX_MKS { get; set; }
        public string IS_DEL { get; set; }
    
        public virtual CCE_GRADE_SET CCE_GRADE_SET { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OBSERVATION> OBSERVATIONs { get; set; }
    }
}
