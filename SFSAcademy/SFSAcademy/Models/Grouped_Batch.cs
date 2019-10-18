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

    [MetadataType(typeof(GrBatchMetadata))]
    public partial class GROUPED_BATCH : IValidatableObject, IHasTimeStamp
    {
        internal sealed class GrBatchMetadata
        {
            [Required]
            public int? BTCH_GROUP_ID { get; set; }

            [Required]
            public int? BTCH_ID { get; set; }

        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BTCH_ID == null)
            {
                string ErrorMessage = "Batch Id is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "CRS_ID" });

            }
            if (BTCH_GROUP_ID == null)
            {
                string ErrorMessage = "Group Id is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "CRS_ID" });

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