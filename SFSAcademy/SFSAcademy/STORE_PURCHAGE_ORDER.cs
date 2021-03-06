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
    
    public partial class STORE_PURCHAGE_ORDER
    {
        public int ID { get; set; }
        public string PO_NUMBER { get; set; }
        public Nullable<int> PRODUCT_ID { get; set; }
        public int REVISION_NUMBER { get; set; }
        public Nullable<int> STATUS_ID { get; set; }
        public Nullable<int> EMPLOYEE_ID { get; set; }
        public Nullable<int> VENDOR_ID { get; set; }
        public Nullable<int> SHIP_METHOD_ID { get; set; }
        public Nullable<System.DateTime> ORDER_DATE { get; set; }
        public Nullable<int> ORDER_QUANTITY { get; set; }
        public Nullable<System.DateTime> SHIP_DATE { get; set; }
        public Nullable<decimal> SUB_TOTAL { get; set; }
        public Nullable<decimal> TAX_AMT { get; set; }
        public Nullable<decimal> FREIGHT { get; set; }
        public Nullable<decimal> TOTAL_DUE { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<int> BRAND_ID { get; set; }
    
        public virtual EMPLOYEE EMPLOYEE { get; set; }
        public virtual STORE_BRAND STORE_BRAND { get; set; }
        public virtual STORE_PRODUCTS STORE_PRODUCTS { get; set; }
        public virtual STORE_PURCHAGE_STATUS STORE_PURCHAGE_STATUS { get; set; }
        public virtual STORE_PURCHAGE_VENDOR STORE_PURCHAGE_VENDOR { get; set; }
    }
}
