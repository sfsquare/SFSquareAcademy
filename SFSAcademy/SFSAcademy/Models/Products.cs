using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFSAcademy
{
    public class Products
    {
        public STORE_PRODUCTS ProductData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
        public STORE_SUB_CATEGORY SubCategoryData { get; set; }
        public STORE_BRAND BrandData { get; set; }
    }

    public class SubCategory
    {
        public STORE_SUB_CATEGORY SubCategoryData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
    }
    public class Purchase
    {
        public STORE_PURCHAGE PurchaseData { get; set; }
        public STORE_PRODUCTS ProductData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public STORE_PURCHAGE_BACKUP PurchaseBackupData { get; set; }
        public USER UserData { get; set; }
        public int? ID { get; set; }
        public int? UNIT_SOLD { get; set; }
        public decimal? SOLD_PRICE { get; set; }
        public int? SOLD_BY_ID { get; set; }
        public bool? IS_DEPOSITED { get; set; }
        public DateTime? SOLD_ON { get; set; }
        public bool? IS_BACKUP { get; set; }
        public long? STUDENT_CONTACT_NO { get; set; }
        public int? MONEY_RECEIVED_BY_ID { get; set; }
        public int? STUDENT_ID { get; set; }
    }

    public class PurchaseCart
    {
        public STORE_PURCHAGE_CART PurchaseCartData { get; set; }
        public STORE_PRODUCTS ProductData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
        public int? UNIT_SOLD { get; set; }
        public decimal? SOLD_AMNT { get; set; }
        public DateTime? PUR_DATE { get; set; }
        public int? SOLD_BY_ID { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public DateTime? UPDATED_AT { get; set; }
        public int? MONEY_RECEIVED_BY_ID { get; set; }
    }

    public class PurchageOrder
    {
        public STORE_PURCHAGE_ORDER PurchaseOrderData { get; set; }
        public STORE_PRODUCTS ProductData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
        public STORE_SUB_CATEGORY SubCategoryData { get; set; }
        public USER EmployeeData { get; set; }
        public STORE_PURCHAGE_STATUS PurchageStatusData { get; set; }
        public STORE_PURCHAGE_VENDOR PurchageVendorData { get; set; }

    }
}