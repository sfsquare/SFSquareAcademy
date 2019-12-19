using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SFSAcademy
{
    public class Products
    {
        public STORE_PRODUCTS ProductData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
        public STORE_SUB_CATEGORY SubCategoryData { get; set; }
        public STORE_BRAND BrandData { get; set; }
        public STORE_INVENTORY InventoryData { get; set; }
        public STORE_PROCUREMENT ProcurementData { get; set; }
        public DateTime? LAST_PROCURED { get; set; }
    }

    public class Procurement
    {
        public STORE_PROCUREMENT ProcurementData { get; set; }
        public STORE_PRODUCTS ProductData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
        public STORE_SUB_CATEGORY SubCategoryData { get; set; }
        public STORE_BRAND BrandData { get; set; }
        public STORE_PURCHAGE_VENDOR VendorData { get; set; }
    }

    public class SubCategory
    {
        public STORE_SUB_CATEGORY SubCategoryData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
    }

    public class Selling
    {
        public STORE_SELLING SelliingData { get; set; }
        public STORE_PRODUCTS ProductData { get; set; }
        public STORE_CATEGORY CategoryData { get; set; }
        public STORE_SUB_CATEGORY SubCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public STORE_SELLING_BACKUP PurchaseBackupData { get; set; }
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

    public class ConsolidatedSelling
    {
        public int? CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public int? SUB_CATEGORY_ID { get; set; }
        public string SUB_CATEGORY_NAME { get; set; }
        public decimal? transactions_income { get; set; }
        public decimal? transactions_expense { get; set; }
        public decimal? transactions_total { get; set; }
        public decimal? total_unit_sold { get; set; }
        public decimal? purchage_cost_per_Unit { get; set; }
        public decimal? selling_cost_per_unit { get; set; }
    }
    public class DetailedTransactions
    {
        public int? CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public int? SUB_CATEGORY_ID { get; set; }
        public string SUB_CATEGORY_NAME { get; set; }
        public int? PRODUCT_ID { get; set; }
        public string PRODUCT_NAME { get; set; }
        public decimal? transactions_income { get; set; }
        public decimal? transactions_expense { get; set; }
        public decimal? transactions_total { get; set; }
        public decimal? total_unit_sold { get; set; }
        public decimal? purchage_cost_per_Unit { get; set; }
        public decimal? selling_cost_per_unit { get; set; }
    }

    public class StoreIncomeDetail
    {
        public string Sub_Category_Name { get; set; }
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public int? Number_Of_Unit { get; set; }
        public DateTime? Transactions_Date { get; set; }
        public int? Receipt_Number { get; set; }
        public bool Is_Income { get; set; }
    }


    public class SellingCart
    {
        public STORE_SELLING_CART SellingCartData { get; set; }
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
    public enum UnitOfMeasure
    {
        [Display(Name = "m")]
        Meter,
        [Display(Name = "kg")]
        Kilogram,
        [Display(Name = "ea")]
        Each,
        [Display(Name = "pk")]
        Packet, 
        [Display(Name = "100Gr")]
        Hundred_Grams

    }
}