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
    
    public partial class FINANCE_FEE_CATGEORY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FINANCE_FEE_CATGEORY()
        {
            this.FEE_COLLECTION_DISCOUNT = new HashSet<FEE_COLLECTION_DISCOUNT>();
            this.FEE_DISCOUNT = new HashSet<FEE_DISCOUNT>();
            this.FEE_FINE = new HashSet<FEE_FINE>();
            this.FINANCE_FEE_COLLECTION = new HashSet<FINANCE_FEE_COLLECTION>();
            this.FINANCE_FEE_PARTICULAR = new HashSet<FINANCE_FEE_PARTICULAR>();
        }
    
        public int ID { get; set; }
        public string NAME { get; set; }
        public string DESCR { get; set; }
        public Nullable<int> BTCH_ID { get; set; }
        public bool IS_DEL { get; set; }
        public bool IS_MSTR { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<int> MSTR_CATGRY_ID { get; set; }
    
        public virtual BATCH BATCH { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FEE_COLLECTION_DISCOUNT> FEE_COLLECTION_DISCOUNT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FEE_DISCOUNT> FEE_DISCOUNT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FEE_FINE> FEE_FINE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE_COLLECTION> FINANCE_FEE_COLLECTION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE_PARTICULAR> FINANCE_FEE_PARTICULAR { get; set; }
    }
}
