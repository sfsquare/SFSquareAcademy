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
    [MetadataType(typeof(ClDesignationMetadata))]
    public partial class CLASS_DESIGNATION : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class ClDesignationMetadata
        {
            [Required]
            public string NAME { get; set; }

            [Remote("CGPAIsNumeric", "Class_Designations", AdditionalFields = "CRS_ID", ErrorMessage = "CGPA must be numerical.")]
            [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid CGPA value.")]
            [Display(Name = "CGPA")]
            public decimal? CGPA { get; set; }

            [Remote("MarksIsNumeric", "Class_Designations", AdditionalFields = "CRS_ID", ErrorMessage = "Marks must be numerical.")]
            [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Marks.")]
            public decimal? MKS { get; set; }
            public int? CRS_ID { get; set; }
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
        public void Inactivate()
        {
            var GradetoUpdate = db.GRADING_LEVEL.Find(ID);
            GradetoUpdate.IS_DEL = true;
            db.Entry(GradetoUpdate).State = EntityState.Modified;
            db.SaveChanges();

        }
        public bool Has_Gpa()
        {
            if(CRS_ID != null && COURSE.GPA_Enabled() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public bool Has_Cwa()
        {
            if (CRS_ID != null && (COURSE.CWA_Enabled() == true || COURSE.Normal_Enabled()))
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