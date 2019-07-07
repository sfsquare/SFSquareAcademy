using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFSAcademy
{
    public partial class SUBJECT_LEAVE : IValidatableObject, IHasTimeStamp
    {
        private string ErrorMessage { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ErrorMessage = "Attendance before the date of admission";
            if (MONTH_DATE < STUDENT.ADMSN_DATE)
            {
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { ErrorMessage });

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