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
            this.ARCHIVED_GUARDIAN = new HashSet<ARCHIVED_GUARDIAN>();
            this.EMPLOYEEs = new HashSet<EMPLOYEE>();
            this.FINANCE_FEE_STRUCTURE_ELEMENT = new HashSet<FINANCE_FEE_STRUCTURE_ELEMENT>();
            this.GUARDIANs = new HashSet<GUARDIAN>();
            this.PRIVILEGES_USERS = new HashSet<PRIVILEGES_USERS>();
            this.USERS_ACCESS = new HashSet<USERS_ACCESS>();
            this.STORE_PURCHAGE_CART = new HashSet<STORE_PURCHAGE_CART>();
            this.STORE_PURCHAGE_CART1 = new HashSet<STORE_PURCHAGE_CART>();
            this.STORE_PURCHAGE = new HashSet<STORE_PURCHAGE>();
            this.STORE_PURCHAGE1 = new HashSet<STORE_PURCHAGE>();
            this.STORE_PURCHAGE_BACKUP = new HashSet<STORE_PURCHAGE_BACKUP>();
            this.STORE_PURCHAGE_BACKUP1 = new HashSet<STORE_PURCHAGE_BACKUP>();
            this.STUDENTs = new HashSet<STUDENT>();
            this.ARCHIVED_EMPLOYEE = new HashSet<ARCHIVED_EMPLOYEE>();
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
        public virtual ICollection<USERS_ACCESS> USERS_ACCESS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PURCHAGE_CART> STORE_PURCHAGE_CART { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PURCHAGE_CART> STORE_PURCHAGE_CART1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PURCHAGE> STORE_PURCHAGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PURCHAGE> STORE_PURCHAGE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PURCHAGE_BACKUP> STORE_PURCHAGE_BACKUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_PURCHAGE_BACKUP> STORE_PURCHAGE_BACKUP1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT> STUDENTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_EMPLOYEE> ARCHIVED_EMPLOYEE { get; set; }
    }
}
