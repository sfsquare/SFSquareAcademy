using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace SFSAcademy
{
    public class Student
    {
        public STUDENT StudentData { get; set; }
        public BATCH BatcheData { get; set; }
        public COURSE CourseData { get; set; }
        public COUNTRY CountryData { get; set; }
        public STUDENT_CATGEORY CategoryData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
        public GRADING_LEVEL GradeData { get; set; }
        public IMAGE_DOCUMENTS ImageData { get; set; }
        public GUARDIAN GuardianData { get; set; }
    }

    public class StudentsBatch
    {
        public IEnumerable<BATCH> BatchList { get; set; }
        public string SearchString { get; set; }
    }

    public class RadioCourseBatch
    {
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public string BatchId { get; set; }
    }

    public class StudentsGuardians
    {
        public GUARDIAN GuardianData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
    }
    public class SelectGuardian
    {
        public GUARDIAN GuardianList { get; set; }
        public bool Selected { get; set; }
    }
    public class StudentTC
    {
        public STUDENT StundentData { get; set; }
        public string Status_Descrition { get; set; }
    }

    public enum MissingDetl
    {
        DateOfBirth,
        PhoneNumber,
        ParentDetails,
        StundetsPicture,
        SchoolBook,
        SchoolDress
    }

    public enum HadPdFees
    {
        Y,
        N
    }
    public enum BloodGroup
    {
        [Display(Name = "O+")]
        O_Positive,
        [Display(Name = "A+")]
        A_Positive,
        [Display(Name = "B+")]
        B_Positive,
        [Display(Name = "AB+")]
        AB_Positive,
        [Display(Name = "AB-")]
        AB_Negative,
        [Display(Name = "A-")]
        A_Negative,
        [Display(Name = "B-")]
        B_Negative,
        [Display(Name = "O-")]
        O_Negative

    }
    public enum Relationship
    {
        [Display(Name = "Father")]
        Father,
        [Display(Name = "Mother")]
        Mother,
        [Display(Name = "Uncle")]
        Uncle,
        [Display(Name = "Auntie")]
        Auntie,
        [Display(Name = "Other")]
        Other

    }

    [MetadataType(typeof(StudentMetadata))]
    public partial class STUDENT : IValidatableObject, IHasTimeStamp, IHasBeforeSave
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class StudentMetadata
        {
            [Required]
            public string ADMSN_NO { get; set; }
            [Required]
            public string FIRST_NAME { get; set; }
            [Required]
            public string GNDR { get; set; }
            [Required]
            public int? BTCH_ID { get; set; }

            [Required]
            public DateTime? ADMSN_DATE { get; set; }
            [Required]
            public DateTime? DOB { get; set; }

            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string EML { get; set; }

        }
        public bool Select { get; set; }
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
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB >= System.DateTime.Today)
            {
                string ErrorMessage = "Date of Birth canot be a future date.";
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }

        }
        public void Before_Save()
        {
            IS_ACT = true;
        }
        public void Is_Active_True()
        {
            if(IS_ACT != true)
            {
                IS_ACT = true;
            }
        }
        public string First_And_Last_Name()
        {
            return string.Concat(FIRST_NAME, " ", LAST_NAME);
        }
        public string Full_Name
        {
            get { return string.Concat(FIRST_NAME, " ", MID_NAME, " ", LAST_NAME); }
        }
        public string Gender_As_Text()
        {
            if(GNDR == "M" || GNDR == "m")
            {
                return "Male";
            }
            else if (GNDR == "F" || GNDR == "f")
            {
                return "Female";
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<BATCH> Graduated_Batches()
        {
            var BatchStd = db.BATCH_STUDENT.Where(x => x.STDNT_ID == ID).ToList();
            var GrBatch = db.BATCHes.Include(x => x.BATCH_STUDENT).Where(x => BatchStd.Select(p => p.BTCH_ID).Contains(x.ID)).ToList();
            return GrBatch;
        }
        public IEnumerable<BATCH> All_Batches()
        {
            var BatchStd = db.BATCH_STUDENT.Where(x => x.STDNT_ID == ID).ToList();
            var GrBatch = db.BATCHes.Where(x => BatchStd.Select(p => p.BTCH_ID).Contains(x.ID)).ToList();
            var AllBatches = db.BATCHes.Where(x => x.ID == BTCH_ID).Union(GrBatch).ToList();
            return AllBatches;
        }
        public GUARDIAN Immediate_Contact()
        {
            if(IMMDT_CNTCT_ID != null)
            {
                return db.GUARDIANs.Find(IMMDT_CNTCT_ID);
            }
            else
            {
                return null;
            }
        }
        public STUDENT Next_Student()
        {
            if (ID != -1)
            {
                return db.STUDENTs.Where(x=>x.BTCH_ID == BTCH_ID && x.ID > ID).OrderBy(x=>x.ID).FirstOrDefault();
            }
            else
            {
                return db.STUDENTs.Where(x => x.BTCH_ID == BTCH_ID).OrderBy(x => x.ID).FirstOrDefault(); ;
            }
        }
        public STUDENT Previous_Student()
        {
            if (ID != -1)
            {
                return db.STUDENTs.Where(x => x.BTCH_ID == BTCH_ID && x.ID < ID).OrderByDescending(x => x.ID).FirstOrDefault();
            }
            else
            {
                return db.STUDENTs.Where(x => x.BTCH_ID == BTCH_ID).OrderByDescending(x => x.ID).FirstOrDefault(); ;
            }
        }
        public STUDENT Previous_Fee_Student(int? Fee_Collection_Id)
        {
            if (ID != -1)
            {
                var fee = db.FINANCE_FEE.Include(x => x.STUDENT).Where(x => x.STDNT_ID < ID && x.FEE_CLCT_ID == Fee_Collection_Id).OrderByDescending(x => x.STDNT_ID);
                if (fee != null && fee.Count() != 0)
                {
                    STUDENT prev_st = fee.FirstOrDefault().STUDENT;
                    return prev_st;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var fee = db.FINANCE_FEE.Include(x => x.STUDENT).Where(x => x.FEE_CLCT_ID == Fee_Collection_Id).OrderByDescending(x => x.STDNT_ID);
                if (fee != null && fee.Count() != 0)
                {
                    STUDENT prev_st = fee.FirstOrDefault().STUDENT;
                    return prev_st;
                }
                else
                {
                    return null;
                }
            }
        }
        public STUDENT Next_Fee_Student(int? Fee_Collection_Id)
        {
            if (ID != -1)
            {
                var fee = db.FINANCE_FEE.Include(x => x.STUDENT).Where(x => x.STDNT_ID > ID && x.FEE_CLCT_ID == Fee_Collection_Id).OrderBy(x => x.STDNT_ID);
                if (fee != null && fee.Count() != 0)
                {
                    STUDENT prev_st = fee.FirstOrDefault().STUDENT;
                    return prev_st;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var fee = db.FINANCE_FEE.Include(x => x.STUDENT).Where(x => x.FEE_CLCT_ID == Fee_Collection_Id).OrderBy(x => x.STDNT_ID);
                if (fee != null && fee.Count() != 0)
                {
                    STUDENT prev_st = fee.FirstOrDefault().STUDENT;
                    return prev_st;
                }
                else
                {
                    return null;
                }
            }
        }
        public IEnumerable<FINANCE_FEE> Finance_Fee_by_Date(int? Fee_Collection_Id)
        {
            return db.FINANCE_FEE.Where(x => x.STDNT_ID == ID && x.FEE_CLCT_ID == Fee_Collection_Id).ToList();
        }

        public bool Archive_Student(string status)
        {
            ARCHIVED_STUDENT archived_student = new ARCHIVED_STUDENT()
            {
                ADMSN_NO = this.ADMSN_NO,
                CLS_ROLL_NO = this.CLS_ROLL_NO,
                ADMSN_DATE = this.ADMSN_DATE,
                FIRST_NAME = this.FIRST_NAME,
                MID_NAME = this.MID_NAME,
                LAST_NAME = this.LAST_NAME,
                BTCH_ID = this.BTCH_ID,
                DOB = this.DOB,
                GNDR = this.GNDR,
                BLOOD_GRP = this.BLOOD_GRP,
                BIRTH_PLACE = this.BIRTH_PLACE,
                LANG = this.LANG,
                RLGN = this.RLGN,
                ADDR_LINE1 = this.ADDR_LINE1,
                ADDR_LINE2 = this.ADDR_LINE2,
                CITY = this.CITY,
                STATE = this.STATE,
                PIN_CODE = this.PIN_CODE,
                CTRY_ID = this.CTRY_ID,
                PH1 = this.PH1.ToString(),
                PH2 = this.PH2.ToString(),
                EML = this.EML,
                IMMDT_CNTCT_ID = this.IMMDT_CNTCT_ID,
                IS_SMS_ENABL = this.IS_SMS_ENABL,
                PHTO_FILENAME = this.PHTO_FILENAME,
                PHTO_CNTNT_TYPE = this.PHTO_CNTNT_TYPE,
                PHTO_DATA = this.IMAGE_DOCUMENTS_ID.ToString(),
                PHTO_FILE_SIZE = this.PHTO_FILE_SIZE,
                STDNT_CAT_ID = this.STDNT_CAT_ID,
                NTLTY_ID = this.NTLTY_ID,
                IS_DEL = this.IS_DEL,
                FRM_ID = this.ID,
                STAT_DESCR = status
            };
            db.ARCHIVED_STUDENT.Add(archived_student);
            try
            {
                db.SaveChanges();
                var guardians = db.GUARDIANs.Where(x => x.WARD_ID == this.ID);
                var user = db.USERS.Find(this.USRID);
                user.IS_DEL = true;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                foreach (var g in guardians)
                {
                    g.Archive_Guardian(archived_student.ID);
                }
                var student_exam_scores = db.EXAM_SCORE.Where(x => x.STDNT_ID == this.ID);
                foreach (var s in student_exam_scores)
                {
                    ARCHIVED_EXAM_SCORE ArchivedExamScore = new ARCHIVED_EXAM_SCORE() { STDNT_ID = archived_student.ID, EXAM_ID = s.EXAM_ID, GRADING_LVL_ID = s.GRADING_LVL_ID, IS_FAIL = s.IS_FAIL, MKS = s.MKS, RMK = s.RMK };
                    db.ARCHIVED_EXAM_SCORE.Add(ArchivedExamScore);
                    db.EXAM_SCORE.Remove(s);
                }
                db.STUDENTs.Remove(this);
                db.SaveChanges();
                return true;
            }
            catch 
            {
                return false;
            }
        }

    }
}