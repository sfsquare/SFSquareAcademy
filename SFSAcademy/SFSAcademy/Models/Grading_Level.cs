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
    public class GradingTypesSelect
    {
        public int? GRADING_TYPE { get; set; }
        public bool Select { get; set; }

    }

    [MetadataType(typeof(GradingMetadata))]
    public partial class GRADING_LEVEL : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class GradingMetadata
        {
            [Remote("GradeExists", "Grading_Levels", ErrorMessage = "Grading Level Name already in use.")]
            public string NAME { get; set; }

            [Remote("CreditPointRequired", "Grading_Levels", AdditionalFields = "BTCH_ID", ErrorMessage = "Credit Point is required field.")]
            public decimal? CRED_PT { get; set; }
            public int? BTCH_ID { get; set; }

        }
        private string ErrorMessage { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {           
            if (string.IsNullOrEmpty(NAME))
            {
                ErrorMessage = "Name is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "NAME" });

            }
            if (MIN_SCORE == null)
            {
                ErrorMessage = "Minimum Score is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "MIN_SCORE" });

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
        public IEnumerable<GRADING_LEVEL> Default()
        {
            return db.GRADING_LEVEL.Where(x => x.BTCH_ID == null && x.IS_DEL == false).ToList();
        }
        public IEnumerable<GRADING_LEVEL> For_Batch(int b)
        {
            return db.GRADING_LEVEL.Where(x => x.BTCH_ID == b && x.IS_DEL == false).ToList();
        }
        public void Inactivate()
        {
            var GradetoUpdate = db.GRADING_LEVEL.Find(ID);
            GradetoUpdate.IS_DEL = true;
            db.Entry(GradetoUpdate).State = EntityState.Modified;
            db.SaveChanges();

        }
        public bool Batch_Has_GPA()
        {
            if(BTCH_ID != null && BATCH.GPA_Enabled() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public string to_s()
        {
            return NAME;
        }
        public bool Exists_For_Batch(int batch_id)
        {
            var batch_grades = db.GRADING_LEVEL.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).ToList();
            var default_grade = Default();
            if (batch_grades == null && batch_grades.Count() == 0 && default_grade == null && default_grade.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public GRADING_LEVEL Percentage_To_Grade(int percent_score, int batch_id)
        {
            var batch_grades = For_Batch(batch_id);
            GRADING_LEVEL grade = db.GRADING_LEVEL.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            if (batch_grades == null && batch_grades.Count() == 0)
            {
                grade = Default().Where(x => x.MIN_SCORE <= percent_score).OrderByDescending(x => x.MIN_SCORE).FirstOrDefault();
            }
            else
            {
                grade = batch_grades.Where(x => x.MIN_SCORE <= percent_score).OrderByDescending(x => x.MIN_SCORE).FirstOrDefault();
            }
            return grade;
        }
    }
}