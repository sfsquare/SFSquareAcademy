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
    public class BatchGroupSelect
    {
        public BATCH_GROUP BatchGroupData { get; set; }
        public BATCH BatchData { get; set; }
        public GROUPED_BATCH GroupedBatchData { get; set; }
        public bool Select { get; set; }
    }

    [MetadataType(typeof(BtGroupMetadata))]
    public partial class BATCH_GROUP : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class BtGroupMetadata
        {
            [Required]
            public string NAME { get; set; }

            [Required]
            public decimal? CRS_ID { get; set; }

        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {           
            if (string.IsNullOrEmpty(NAME))
            {
                string ErrorMessage = "Name is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "NAME" });

            }
            if (CRS_ID == null)
            {
                string ErrorMessage = "Course must be attached to a Batch Group.";
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
        public bool Has_Active_Batches()
        {
            bool ActiveBtExists = false;
            var Batches = GROUPED_BATCH.Where(x => x.BTCH_GROUP_ID == ID).Select(x => x.BATCH).ToList();
            foreach(var item in Batches)
            {
                if(item.IS_ACT == true && item.IS_DEL == false)
                {
                    ActiveBtExists = true;
                    return true;
                }
            }
            if(ActiveBtExists == true)
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