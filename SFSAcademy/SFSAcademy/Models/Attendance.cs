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
using SFSAcademy;



namespace SFSAcademy
{
    public class AttendanceSubLeave
    {
        public int? Absentee_Id { get; set; }
        public int? Student_Id { get; set; }
        public string AttendanceDate { get; set; }
        public int? Batch_Id { get; set; }
        public int? Subject_Id { get; set; }
        public string Reason { get; set; }
        public bool Forenoon { get; set; }
        public bool Afternoon { get; set; }
        public string Next { get; set; }
    }
    public enum RANGE
    {
        Below,
        Above,
        Equals
    }
    public partial class ATTENDENCE : IValidatableObject
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        private string ErrorMessage { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var Batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == BTCH_ID).FirstOrDefault();
            string BatchName = string.Concat(Batch.COURSE.CODE, "-", Batch.NAME);
            STUDENT std = db.STUDENTs.Find(STDNT_ID);
            if(STDNT_ID != null)
            {
                if(std.BTCH_ID != BTCH_ID)
                {
                    yield return new ValidationResult($"Attendance is not marked for present batch {BatchName}.", new[] { BatchName });
                }

            }
        }

        public IEnumerable<ValidationResult> After_Validate(ValidationContext validationContext)
        {
            if(MONTH_DATE != null)
            {
                if(STDNT_ID != null && MONTH_DATE < STUDENT.ADMSN_DATE)
                {
                    yield return new ValidationResult($"Attendance before the date of admission {STUDENT.ADMSN_DATE}.", new[] { ((DateTime)STUDENT.ADMSN_DATE).ToShortDateString() });
                }
            }
            else
            {
                ErrorMessage = "Month Date can not be blank";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { ErrorMessage });
            }
        }
        public bool Is_Full_Day()
        {
            if(FORENOON == true && PM == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Is_Half_Day()
        {
            if (FORENOON == true || PM == true)
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