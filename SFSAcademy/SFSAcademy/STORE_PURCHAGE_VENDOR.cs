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
    
    public partial class STORE_PURCHAGE_VENDOR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STORE_PURCHAGE_VENDOR()
        {
            this.STORE_PROCUREMENT = new HashSet<STORE_PROCUREMENT>();
            this.STORE_PRODUCTS = new HashSet<STORE_PRODUCTS>();
            this.STORE_PURCHAGE_ORDER = new HashSet<STORE_PURCHAGE_ORDER>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public bool IS_DEL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PROCUREMENT> STORE_PROCUREMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PRODUCTS> STORE_PRODUCTS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PURCHAGE_ORDER> STORE_PURCHAGE_ORDER { get; set; }
    }
}
