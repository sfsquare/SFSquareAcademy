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
    
    public partial class FINANCE_TRANSACTION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FINANCE_TRANSACTION()
        {
            this.FINANCE_DONATION = new HashSet<FINANCE_DONATION>();
        }
    
        public int ID { get; set; }
        public Nullable<int> MSTRTRAN_ID { get; set; }
        public string TIL { get; set; }
        public string DESCR { get; set; }
        public Nullable<decimal> AMT { get; set; }
        public string FINE_INCLD { get; set; }
        public Nullable<int> CAT_ID { get; set; }
        public Nullable<int> STDNT_ID { get; set; }
        public Nullable<int> FIN_FE_ID { get; set; }
        public Nullable<System.DateTime> CRETAED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<System.DateTime> TRAN_DATE { get; set; }
        public Nullable<decimal> FINE_AMT { get; set; }
        public Nullable<int> FIN_ID { get; set; }
        public string FIN_TYPE { get; set; }
        public Nullable<int> PAYEE_ID { get; set; }
        public string PAYEE_TYPE { get; set; }
        public string RCPT_NO { get; set; }
        public string VCHR_NO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_DONATION> FINANCE_DONATION { get; set; }
        public virtual FINANCE_FEE FINANCE_FEE { get; set; }
        public virtual FINANCE_TRANSACTION_CATEGORY FINANCE_TRANSACTION_CATEGORY { get; set; }
        public virtual STUDENT STUDENT { get; set; }
    }
}
