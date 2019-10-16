using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SFSAcademy
{

    public class FeeDiscount
    {
        public FEE_DISCOUNT FeeDiscountData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public FINANCE_FEE_PARTICULAR FinanceFeeParticularData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
    }

    public class FeeFine
    {
        public FEE_FINE FeeFineData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public FINANCE_FEE_PARTICULAR FinanceFeeParticularData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
    }

    public class BATCHList
    {
        public int ID { set; get; }
        public string NAME { set; get; }
    }
    public class StundentFee
    {
        public STUDENT StudentData { get; set; }
        public FINANCE_FEE FinanceFeeData { get; set; }
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
        public COURSE CourseData { get; set; }
        public BATCH BatchData { get; set; }
    }
    public enum Gender
    {
        Male,
        Female
    }
    public enum PaymentMode
    {
        Cash,
        Check,
        BankDeposite,
        Others
    }

    public enum FinCategoryIncomeTypes
    {
        Y,
        N
    }

    public enum FeeFrequency
    {
        [Display(Name = "Monthly")]
        Monthly,
        [Display(Name = "Yearly")]
        Yearly,
        [Display(Name = "One Time")]
        One_Time,
        [Display(Name = "Quarterly")]
        Quarterly
    }

    public class CategoryTransactions
    {
        public FINANCE_TRANSACTION_CATEGORY TransactionCategoryData { get; set; }
        public decimal? TRANS_AMNT { get; set; }
    }

    public class SubmitFees : NoResubmitAbstract // << Inherit from NoResubmitAbstract
    {
        public string PAYMENT_MODE { get; set; }
        [Required(ErrorMessage = "Reference Receipt Number is required. Add a note if it is not available.")]
        public string PAYMENT_NOTE { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Payment amount should be bigger than 0.")]
        [Required(ErrorMessage = "Payment Amount is required.")]
        public decimal? PAYMENT_AMOUNT { get; set; }
        [Required(ErrorMessage = "Payment Date is required.")]
        public string PAYMENT_DATE { get; set; }
        public int? StudentID { get; set; }
        public int? Batch_id { get; set; }
        public int? Date { get; set; }
    }

    public class SubmitFeeFine
    {
        [Required(ErrorMessage = "Fine amount is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Fine amount should be bigger than 0.")]
        public decimal? Fine { get; set; }
        [Required(ErrorMessage = "Fine description is required.")]
        public string Fine_Desc { get; set; }
        public int? StudentID { get; set; }
        public int? Batch_id { get; set; }
        public int? Date { get; set; }
    }

    public class SubmitFeeDiscounts
    {
        [Required(ErrorMessage = "Discount amount is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Discount amount should be bigger than 0.")]
        public decimal? Discount { get; set; }
        [Required(ErrorMessage = "Discount description is required.")]
        public string Discount_Desc { get; set; }
        public int? StudentID { get; set; }
        public int? Batch_id { get; set; }
        public int? Date { get; set; }
    }
}