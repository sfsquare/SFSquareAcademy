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
    
    public partial class USER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public USER()
        {
            this.ARCHIVED_EMPLOYEE = new HashSet<ARCHIVED_EMPLOYEE>();
            this.ARCHIVED_GUARDIAN = new HashSet<ARCHIVED_GUARDIAN>();
            this.EMPLOYEEs = new HashSet<EMPLOYEE>();
            this.FINANCE_FEE_STRUCTURE_ELEMENT = new HashSet<FINANCE_FEE_STRUCTURE_ELEMENT>();
            this.GUARDIANs = new HashSet<GUARDIAN>();
            this.PRIVILEGES_USERS = new HashSet<PRIVILEGES_USERS>();
            this.STORE_SELLING = new HashSet<STORE_SELLING>();
            this.STORE_SELLING1 = new HashSet<STORE_SELLING>();
            this.STORE_SELLING_BACKUP = new HashSet<STORE_SELLING_BACKUP>();
            this.STORE_SELLING_BACKUP1 = new HashSet<STORE_SELLING_BACKUP>();
            this.STORE_SELLING_CART = new HashSet<STORE_SELLING_CART>();
            this.STORE_SELLING_CART1 = new HashSet<STORE_SELLING_CART>();
            this.STUDENTs = new HashSet<STUDENT>();
            this.USERS_EVENT = new HashSet<USERS_EVENT>();
        }
    
        public int ID { get; set; }
        public string USRNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EML { get; set; }
        public Nullable<bool> ADMIN_IND { get; set; }
        public Nullable<bool> STDNT_IND { get; set; }
        public Nullable<bool> EMP_IND { get; set; }
        public string HASHED_PSWRD { get; set; }
        public string SALT { get; set; }
        public string RST_PSWRD_CODE { get; set; }
        public Nullable<System.DateTime> RST_PSWRD_CODE_UNTL { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<bool> PARNT_IND { get; set; }
        public bool IS_DEL { get; set; }
        public Nullable<bool> IS_FIRST_LOGIN { get; set; }
        public string ROLE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_EMPLOYEE> ARCHIVED_EMPLOYEE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_GUARDIAN> ARCHIVED_GUARDIAN { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPLOYEE> EMPLOYEEs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE_STRUCTURE_ELEMENT> FINANCE_FEE_STRUCTURE_ELEMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GUARDIAN> GUARDIANs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRIVILEGES_USERS> PRIVILEGES_USERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING> STORE_SELLING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING> STORE_SELLING1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING_BACKUP> STORE_SELLING_BACKUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING_BACKUP> STORE_SELLING_BACKUP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING_CART> STORE_SELLING_CART { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING_CART> STORE_SELLING_CART1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT> STUDENTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USERS_EVENT> USERS_EVENT { get; set; }
    }
}
