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
    
    public partial class FINANCE_DONATION
    {
        public int ID { get; set; }
        public string DNR { get; set; }
        public string DESCR { get; set; }
        public Nullable<decimal> AMT { get; set; }
        public Nullable<int> TRAN_ID { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<System.DateTime> TRAN_DATE { get; set; }
    
        public virtual FINANCE_TRANSACTION FINANCE_TRANSACTION { get; set; }
    }
}
