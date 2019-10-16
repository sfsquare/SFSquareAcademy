using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFSAcademy
{
    public enum LIMIT_TYPES
    {
        [Display(Name = "Upper")]
        Upper,
        [Display(Name = "Lower")]
        Lower,
        [Display(Name = "Exact")]
        Exact

    }
    public partial class RANKING_LEVEL : IValidatableObject, IHasTimeStamp
    {
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
        public bool Has_GPA()
        {
            return COURSE.GPA_Enabled();
        }
        public bool Has_CWA()
        {
            if(COURSE.CWA_Enabled() || COURSE.Normal_Enabled())
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