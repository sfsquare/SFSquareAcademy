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
    
    public partial class OBSERVATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OBSERVATION()
        {
            this.CCE_REPORTS = new HashSet<CCE_REPORTS>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string DESCR { get; set; }
        public string IS_ACT { get; set; }
        public Nullable<int> OBSV_GRP_ID { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<int> SRT_ORD { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCE_REPORTS> CCE_REPORTS { get; set; }
        public virtual OBSERVATION_GROUP OBSERVATION_GROUP { get; set; }
    }
}