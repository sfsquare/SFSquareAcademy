using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SFSAcademy
{
    public class FeeCollectionDiscount
    {
        public FEE_COLLECTION_DISCOUNT FeeCollectionDiscountData { get; set; }
        public FINANCE_FEE_COLLECTION FinanceFeeCollectionData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public FINANCE_FEE_PARTICULAR FinanceFeeParticularData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
    }

    public class FeeCollection
    {
        public FINANCE_FEE_COLLECTION FinanceFeeCollectionData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
    }

    public class FeeSubmission
    {
        public FINANCE_FEE_COLLECTION FinanceFeeCollectionData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public STUDENT StudentData { get; set; }
        public FINANCE_TRANSACTION FinanceTransactionData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public FEE_DISCOUNT FeeDiscountData { get; set; }
        public FINANCE_FEE FinanceFeeData { get; set; }
        public FEE_COLLECTION_PARTICULAR FeeCollectionParticularData { get; set; }
        public FEE_COLLECTION_DISCOUNT FeeCollectionDiscountData { get; set; }
    }

    public class FeeCollectionFee
    {
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
        public FINANCE_FEE FinanceFeeData { get; set; }
    }
    public class FeeCollectionTransactions
    {
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public decimal? TRANS_AMNT { get; set; }
    }
    public partial class FINANCE_FEE_COLLECTION : IValidatableObject, IHasBeforeSave
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //admission_no, :admission_date, :first_name, :batch_id, :date_of_birth
            if (string.IsNullOrEmpty(NAME))
            {
                string ErrorMessage = "Collection Name is mandatory.";
                //yield return new ValidationResult($"Classic movies must have a release year earlier than {_classicYear}.", new[] { "ReleaseDate" });
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }
            if (START_DATE == null)
            {
                string ErrorMessage = "Start Date is mandatory.";
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }
            if (FEE_CAT_ID == null)
            {
                string ErrorMessage = "Fee Category Id is mandatory.";
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }
            if (END_DATE == null)
            {
                string ErrorMessage = "End Date is mandatory.";
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }
            if (DUE_DATE == null)
            {
                string ErrorMessage = "Due Date is mandatory.";
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }
            if (START_DATE > END_DATE)
            {
                string ErrorMessage = "Start date cant be after end date.";
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }
            if (START_DATE > DUE_DATE)
            {
                string ErrorMessage = "Start date cant be after due date.";
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }
            if (END_DATE > END_DATE)
            {
                string ErrorMessage = "End date cant be after end date.";
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }

        }
        public void Before_Save()
        {
            var particlulars = db.FINANCE_FEE_PARTICULAR.Where(x => x.FIN_FEE_CAT_ID == FEE_CAT_ID && x.IS_DEL == "N");
            foreach(var p in particlulars)
            {
                FEE_COLLECTION_PARTICULAR particlulars_attributes = new FEE_COLLECTION_PARTICULAR() { NAME = p.NAME, DESCR = p.DESCR, STDNT_CAT_ID = p.STDNT_CAT_ID, STDNT_ID = p.STDNT_ID, FIN_FEE_CLCT_ID = ID, ADMSN_NO = p.ADMSN_NO, AMT = p.AMT, IS_DEL = false, CREATED_AT = DateTime.Today, UPDATED_AT = DateTime.Today };
                db.FEE_COLLECTION_PARTICULAR.Add(particlulars_attributes);
                try { db.SaveChanges(); }                
                catch (Exception e) { Console.WriteLine(e); }
            }
        }
        public string Full_Name
        {
            get { return string.Concat(NAME, "-", START_DATE.Value.ToShortDateString()); }
        }
        public IEnumerable<FINANCE_FEE> Fee_Transactions(int? student_id)
        {
            return db.FINANCE_FEE.Where(x => x.FEE_CLCT_ID == ID && x.STDNT_ID == student_id).ToList();
        }
        public bool Check_Transaction(FINANCE_TRANSACTION transactions)
        {
            if(transactions.FIN_FE_ID != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IEnumerable<FINANCE_FEE> Fee_Table()
        {
            return db.FINANCE_FEE.Where(x => x.FEE_CLCT_ID == ID && x.IS_PD == false).ToList();
        }

        public IEnumerable<FEE_COLLECTION_PARTICULAR> Fees_Particulars(STUDENT student)
        {
            return db.FEE_COLLECTION_PARTICULAR.Where(x => x.FIN_FEE_CLCT_ID == ID && ((x.STDNT_CAT_ID == null && x.ADMSN_NO == null) || (x.STDNT_CAT_ID == student.STDNT_CAT_ID && x.ADMSN_NO == null) || (x.STDNT_CAT_ID == null && x.ADMSN_NO == student.ADMSN_NO)) && x.IS_DEL == false).ToList();
        }
        public bool Check_Fee_Category()
        {
            bool flag = true;
            var finance_fees = db.FINANCE_FEE.Where(x => x.FEE_CLCT_ID == this.ID);
            foreach(var f in finance_fees)
            {
                if(f.TRAN_ID != null)
                {
                    flag = false;
                }
            }
            return flag;
        }
        public bool No_Transaction_Present()
        {
            var f = db.FINANCE_FEE.Where(x => x.FEE_CLCT_ID == this.ID);
            if(f != null && f.Count() != 0)
            {
                f = f.Where(x => x.TRAN_ID == null);
            }
            if (f == null || f.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}