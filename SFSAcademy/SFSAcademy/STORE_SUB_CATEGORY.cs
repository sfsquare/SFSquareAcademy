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
    
    public partial class STORE_SUB_CATEGORY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STORE_SUB_CATEGORY()
        {
            this.STORE_PRODUCTS = new HashSet<STORE_PRODUCTS>();
        }
    
        public int ID { get; set; }
        public int STORE_CATEGORY_ID { get; set; }
        public string NAME { get; set; }
        public bool IS_ACT { get; set; }
        public bool IS_DEL { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
    
        public virtual STORE_CATEGORY STORE_CATEGORY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PRODUCTS> STORE_PRODUCTS { get; set; }
    }
}
