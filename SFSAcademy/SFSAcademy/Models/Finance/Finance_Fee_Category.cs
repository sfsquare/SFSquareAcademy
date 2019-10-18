using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;

namespace SFSAcademy
{
    public class FeeCategory
    {
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
    }

    public class FeeMasterCategory
    {
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
    }

    public class SelectFeeCategory
    {
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public bool Selected { get; set; }
    }


    [MetadataType(typeof(FFCatMetadata))]
    public partial class FINANCE_FEE_CATGEORY : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class FFCatMetadata
        {
            [Remote("UniqueName", "Finance", ErrorMessage = "Name already in use. Please choose another name.")]
            [Required]
            public string NAME { get; set; }

        }
        private string ErrorMessage { get; set; }
        public bool Select { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            if (string.IsNullOrEmpty(NAME))
            {
                ErrorMessage = "Name is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "NAME" });

            }

        }
        public void DoTimeStamp(string EntityStateVal)
        {

            if (EntityStateVal.Equals("Added"))
            {
                //add creation date_time            
                CREATED_AT = DateTime.Now;
                UPDATED_AT = DateTime.Now;
            }

            if (EntityStateVal.Equals("Modified"))
            {
                //update Updation time            
                UPDATED_AT = DateTime.Now;
            }
        }
        public IEnumerable<FINANCE_FEE_PARTICULAR> Fees(STUDENT student)
        {
            return db.FINANCE_FEE_PARTICULAR.Where(x => x.FIN_FEE_CAT_ID == this.ID && ((x.STDNT_CAT_ID == null && x.ADMSN_NO == null) || (x.STDNT_CAT_ID == student.STDNT_CAT_ID && x.ADMSN_NO == null) ||(x.STDNT_CAT_ID == null && x.ADMSN_NO == student.ADMSN_NO)) && x.IS_DEL == "N").AsEnumerable();
        }
        public bool Check_Fee_Collection()
        {
            var fee_collection = db.FINANCE_FEE_COLLECTION.Where(x => x.FEE_CAT_ID == this.ID && x.IS_DEL == false);
            if(fee_collection == null && fee_collection.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Check_Fee_Collection_For_Additional_Fees()
        {
            bool flag = false;
            var fee_collection = db.FINANCE_FEE_COLLECTION.Where(x => x.FEE_CAT_ID == this.ID);
            foreach(var fee in fee_collection)
            {
                if(fee.Check_Fee_Category() == true)
                {
                    flag = true;
                }
            }
            return flag;
        }
        public void Delete_Particulars()
        {
            foreach(var fees in this.FINANCE_FEE_PARTICULAR)
            {
                fees.IS_DEL = "Y";
                db.Entry(this).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
        public bool Is_Collection_Open()
        {
            var collection = db.FINANCE_FEE_COLLECTION.Where(x => x.FEE_CAT_ID == this.ID && x.START_DATE < DateTime.Today && x.DUE_DATE > DateTime.Today);
            if(collection != null && collection.Count() != 0)
            {
                collection = collection.Where(x => x.No_Transaction_Present() == false);
            }
            if(collection != null && collection.Count() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Have_Common_Particular()
        {
            return this.FINANCE_FEE_PARTICULAR.Where(x => x.STDNT_CAT_ID == null && x.ADMSN_NO == null).Count() > 0 ? true : false;
        }
    }
}