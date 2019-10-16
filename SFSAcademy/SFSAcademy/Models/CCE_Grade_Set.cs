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
    [MetadataType(typeof(CCEGrSetMetadata))]
    public partial class CCE_GRADE_SET : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class CCEGrSetMetadata
        {
            [Required]
            public string NAME { get; set; }
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
        public string Grade_String_For(float? point)
        {
            CCE_GRADE grade_obj = db.CCE_GRADE.Where(x => x.GRADE_PT == point).FirstOrDefault();
            return grade_obj == null ? "No Grade" : grade_obj.NAME;
        }
        public float Max_Grade_Point()
        {
            float? Point = (float)db.CCE_GRADE.Select(x => x.GRADE_PT).Max();
            if(Point == 0)
            {
                Point = 1;
            }
            return (float)Point;
        }
    }
}