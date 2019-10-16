using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
namespace SFSAcademy
{

    public class CreateParticularOption
    {
        public int Id { set; get; }
        public string Name { set; get; }
    }

    public class FeeParticular
    {
        public FINANCE_FEE_PARTICULAR FeeParticularData { get; set; }
        public FINANCE_FEE_CATGEORY FeeCategoryData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
    }

    [MetadataType(typeof(FFPartMetadata))]
    public partial class FINANCE_FEE_PARTICULAR : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class FFPartMetadata
        {
            [Required]
            public string NAME { get; set; }

            [Remote("AmtIsNumeric", "Finance", ErrorMessage = "Amount must be positive.")]
            [Required]
            public decimal? AMT { get; set; }
        }
        private string ErrorMessage { get; set; }
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
        public IEnumerable<FINANCE_FEE_PARTICULAR> Active()
        {
            return db.FINANCE_FEE_PARTICULAR.Where(x => x.IS_DEL == "0").AsEnumerable();

        }
        public bool Deleted_Category()
        {
            bool flag = false;
            STUDENT_CATGEORY category = db.STUDENT_CATGEORY.Find(STDNT_CAT_ID);
            if (category != null)
            {
                if(category.IS_DEL == true)
                {
                    flag = true;
                }
            }
            return flag;
        }
    }
}