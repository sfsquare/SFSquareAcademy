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
    [MetadataType(typeof(GuardianMetadata))]
    public partial class GUARDIAN : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class GuardianMetadata
        {
            [Required]
            public string FIRST_NAME { get; set; }

            [Required]
            public string REL { get; set; }
            [Required]
            public int? WARD_ID { get; set; }

            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string EML { get; set; }

            [Remote("GurdianDOBFutureDate", "Student", ErrorMessage = "Date of birth cannot be a future date.")]
            public DateTime? DOB { get; set; }
        }
        private string ErrorMessage { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(FIRST_NAME))
            {
                ErrorMessage = "First Name is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "FIRST_NAME" });

            }
            if(DOB != null)
            {
                if(DOB> DateTime.Today)
                {
                    ErrorMessage = "Date of Birth cannot be future date.";
                    yield return new ValidationResult($"* {ErrorMessage}.", new[] { "DOB" });
                }
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
        public bool Is_Immediate_Contact()
        {
            return db.STUDENTs.Find(this.WARD_ID).IMMDT_CNTCT_ID == this.ID ? true : false;

        }
        public string Full_Name
        {
            get { return string.Concat(FIRST_NAME, " ", LAST_NAME); }
        }
        public bool Archive_Guardian(int? archived_student)
        {
            ARCHIVED_GUARDIAN guardian_attributes = new ARCHIVED_GUARDIAN()
            {
                WARD_ID = archived_student,
                FIRST_NAME = this.FIRST_NAME,
                LAST_NAME = this.LAST_NAME,
                REL = this.REL,
                EML = this.EML,
                OFF_PH1 = this.OFF_PH1,
                OFF_PH2 = this.OFF_PH2,
                MOBL_PH = this.MOBL_PH,
                OFF_ADDR_LINE1 = this.OFF_ADDR_LINE1,
                OFF_ADDR_LINE2 = this.OFF_ADDR_LINE2,
                CITY = this.CITY,
                STATE = this.STATE,
                CTRY_ID = this.CTRY_ID,
                DOB = this.DOB,
                OCCP = this.OCCP,
                INCM = this.INCM,
                ED = this.ED
            };
            db.ARCHIVED_GUARDIAN.Add(guardian_attributes);
            try
            {
                db.SaveChanges();
                if(this.USRID != null)
                {
                    USER Olduser = db.USERS.Find(this.USRID);
                    Olduser.IS_DEL = true;
                    db.Entry(Olduser).State = EntityState.Modified;
                }
                db.GUARDIANs.Remove(this);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Create_Guardian_User(STUDENT student)
        {
            USER user = new USER() { FIRST_NAME = this.FIRST_NAME, LAST_NAME = this.LAST_NAME, USRNAME = "P" + student.ADMSN_NO, HASHED_PSWRD = "P" + student.ADMSN_NO + "123", ROLE = "Parent", EML = (this.EML == "" || db.USERS.FirstOrDefault().Active().Where(x => x.EML == this.EML).Count() != 0) ? "" : this.EML };
            try
            {
                db.SaveChanges();
                this.USRID = user.ID;
                db.Entry(this).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void Shift_User(STUDENT student)
        {
           foreach(var g in db.GUARDIANs.Where(x=>x.WARD_ID == student.ID))
            {
                USER parent_user = g.USER;
                if(parent_user != null && parent_user.IS_DEL == false)
                {
                    parent_user.IS_DEL = true;
                    db.Entry(this).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            GUARDIAN current_guardian = db.GUARDIANs.Find(student.IMMDT_CNTCT_ID);
            if(current_guardian != null)
            {
                current_guardian.Create_Guardian_User(student);
            }
        }
        public void Immediate_Contact_Nil()
        {
            STUDENT student = db.STUDENTs.Find(this.WARD_ID);
            if(student != null && student.IMMDT_CNTCT_ID == this.ID)
            {
                student.IMMDT_CNTCT_ID = null;
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}