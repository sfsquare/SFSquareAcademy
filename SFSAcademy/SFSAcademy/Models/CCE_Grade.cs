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
    [MetadataType(typeof(CCEGrMetadata))]
    public partial class CCE_GRADE : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class CCEGrMetadata
        {
            [Required]
            public string NAME { get; set; }

            [Required]
            [RegularExpression("([0-9]+)", ErrorMessage = "Grade Point must be numerical.")]
            public float? GRADE_PT { get; set; }
        }
        private string ErrorMessage { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(NAME))
            {
                ErrorMessage = "Name is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "NAME" });

            }
            if (GRADE_PT == null)
            {
                ErrorMessage = "Grade Point is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "GRADE_PT" });

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
    }
}