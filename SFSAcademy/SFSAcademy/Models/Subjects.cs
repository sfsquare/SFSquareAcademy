using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;

namespace SFSAcademy
{
    public class ElectiveGroups
    {
        public ELECTIVE_GROUP ElectiveGroupData { get; set; }
        public SUBJECT SubjectData { get; set; }
        public BATCH BatchData { get; set; }
    }

    public class TimetableEntry
    {
        public TIMETABLE_ENTRY TimeTableEntryData { get; set; }
        public STUDENT_SUBJECT StudentSubjectData { get; set; }
        public SUBJECT SubjectData { get; set; }
        public BATCH BatchData { get; set; }
    }

    public class subject
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        public EMPLOYEE Lower_Week_Grade(SUBJECT sub)
        {
            EMPLOYEE selected_employee = null;
            if (sub.ELECTIVE_GRP_ID != null)
            {
                var subjects = db.SUBJECTs.Where(x => x.ELECTIVE_GRP_ID == sub.ELECTIVE_GRP_ID).ToList();
                foreach (var item in subjects)
                {
                    var employees = (from emp in db.EMPLOYEEs
                                     join emp_sub in db.EMPLOYEES_SUBJECT on emp.ID equals emp_sub.EMP_ID
                                     where emp_sub.SUBJ_ID == item.ID
                                     select new { emp }).ToList();
                    foreach(var item2 in employees)
                    {
                        if(selected_employee == null)
                        {
                            selected_employee = item2.emp;
                        }
                        else
                        {
                            if(item2.emp.EMPLOYEE_GRADE.MAX_WKILY_HRS < selected_employee.EMPLOYEE_GRADE.MAX_WKILY_HRS)
                            {
                                selected_employee = item2.emp;
                            }
                        }
                    }
                }
            }            
           return selected_employee;
        }

        public EMPLOYEE Lower_Day_Grade(SUBJECT sub)
        {
            EMPLOYEE selected_employee = null;
            if (sub.ELECTIVE_GRP_ID != null)
            {
                var subjects = db.SUBJECTs.Where(x => x.ELECTIVE_GRP_ID == sub.ELECTIVE_GRP_ID).ToList();
                foreach (var item in subjects)
                {
                    var employees = (from emp in db.EMPLOYEEs
                                     join emp_sub in db.EMPLOYEES_SUBJECT on emp.ID equals emp_sub.EMP_ID
                                     where emp_sub.SUBJ_ID == item.ID
                                     select new { emp }).ToList();
                    foreach (var item2 in employees)
                    {
                        if (selected_employee == null)
                        {
                            selected_employee = item2.emp;
                        }
                        else
                        {
                            if (item2.emp.EMPLOYEE_GRADE.MAX_DILY_HRS < selected_employee.EMPLOYEE_GRADE.MAX_DILY_HRS)
                            {
                                selected_employee = item2.emp;
                            }
                        }
                    }
                }
            }
            return selected_employee;
        }

    }

    [MetadataType(typeof(SubjectMetadata))]
    public partial class SUBJECT : IValidatableObject, IHasTimeStamp, IHasBeforeSave
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class SubjectMetadata
        {
            [Required]
            public string NAME { get; set; }

            [Remote("WklyClNumeric", "Subjects", ErrorMessage = "Max Weekly Classes must be numerical.")]
            [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Max Weekly Class value.")]
            [Display(Name = "Max Weekly Classes")]
            [Required]
            public int? MAX_WKILY_CLSES { get; set; }

            [Remote("UniqueCode", "Subjects", ErrorMessage = "Subject Code already in use. Please choose another Code.")]
            [Required]
            public string CODE { get; set; }

            [Required]
            public int? BTCH_ID { get; set; }

            [Remote("CreditHoursCheck", "Subjects", AdditionalFields = "BTCH_ID", ErrorMessage = "Credit Hours canot be null.")]
            public decimal? CR_HRS { get; set; }

            [Remote("AmtNumeric", "Subjects", ErrorMessage = "Amount should either be null or numeric.")]
            public decimal? AMT { get; set; }
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(NAME))
            {
                string ErrorMessage = "Name is mandatory.";
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
        public void Before_Save()
        {
            ValidationContext validationContext = new ValidationContext(this);
            this.FA_Group_Valid(validationContext);
        }
        public IEnumerable<SUBJECT> For_Batch(int? batch_id)
        {
            return db.SUBJECTs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).AsEnumerable();
        }
        public IEnumerable<SUBJECT> Without_Exams()
        {
            return db.SUBJECTs.Where(x => x.NO_EXAMS == true && x.IS_DEL == false).AsEnumerable();
        }
        public IEnumerable<SUBJECT> Active()
        {
            return db.SUBJECTs.Where(x => x.IS_DEL == false).AsEnumerable();
        }
        public bool Check_Grade_Type()
        {
            bool flag = false;
            if(this.BTCH_ID != null)
            {
                BATCH batch = db.BATCHes.Find(this.BTCH_ID);
                if(batch.GPA_Enabled() || batch.CWA_Enabled())
                {
                    flag = true;
                }
            }
            return flag;
        }
        public void Inactivate()
        {
            var SubToUpdate = db.SUBJECTs.Find(this.ID);
            SubToUpdate.IS_DEL = true;
            db.Entry(SubToUpdate).State = EntityState.Modified;
            var EmpSubToUpdate = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == this.ID);
            foreach(var es in EmpSubToUpdate)
            {
                db.EMPLOYEES_SUBJECT.Remove(es);
            }
            db.SaveChanges();
        }
        public EMPLOYEE Lower_Day_Grade()
        {
            var subjects = db.SUBJECTs.Where(x => x.ID == -1).DefaultIfEmpty().AsEnumerable();
            if (this.ELECTIVE_GRP_ID != null)
            {
                subjects = db.SUBJECTs.Include(x => x.EMPLOYEES_SUBJECT).Where(x => x.ELECTIVE_GRP_ID == this.ELECTIVE_GRP_ID).AsEnumerable();
            }
            EMPLOYEE selected_employee = db.EMPLOYEEs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            foreach(var subject in subjects)
            {
                var empSub = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == subject.ID).Select(x=>x.EMP_ID).ToList();
                var employees = db.EMPLOYEEs.Where(x => empSub.Contains(x.ID)).ToList();
                foreach(var employee in employees)
                {
                    if(selected_employee == null)
                    {
                        selected_employee = employee;
                    }
                    else
                    {
                        if(Convert.ToInt32(employee.Max_Hours_Per_Day()) < Convert.ToInt32(selected_employee.Max_Hours_Per_Day()))
                        {
                            selected_employee = employee;
                        }
                               
                    }
                }
            }
            return selected_employee;
        }
        public EMPLOYEE Lower_Week_Grade()
        {
            var subjects = db.SUBJECTs.Where(x => x.ID == -1).DefaultIfEmpty().AsEnumerable();
            if (this.ELECTIVE_GRP_ID != null)
            {
                subjects = db.SUBJECTs.Include(x => x.EMPLOYEES_SUBJECT).Where(x => x.ELECTIVE_GRP_ID == this.ELECTIVE_GRP_ID).AsEnumerable();
            }
            EMPLOYEE selected_employee = db.EMPLOYEEs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            foreach (var subject in subjects)
            {
                var empSub = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == subject.ID).Select(x => x.EMP_ID).ToList();
                var employees = db.EMPLOYEEs.Where(x => empSub.Contains(x.ID)).ToList();
                foreach (var employee in employees)
                {
                    if (selected_employee == null)
                    {
                        selected_employee = employee;
                    }
                    else
                    {
                        if (Convert.ToInt32(employee.Max_Hours_Per_Week()) < Convert.ToInt32(selected_employee.Max_Hours_Per_Week()))
                        {
                            selected_employee = employee;
                        }

                    }
                }
            }
            return selected_employee;
        }
        public bool Exam_Not_Created(List<int?> exam_group_ids)
        {
            var exams = db.EXAMs.Where(x => exam_group_ids.Contains((int)x.EXAM_GRP_ID) && x.SUBJ_ID == this.ID).ToList();
            if(exams == null && exams.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool No_Exam_For_Batch(int? batch_id)
        {
            List<int?> grouped_exams = db.GROUPED_EXAM.Where(x => x.BTCH_ID == batch_id).Select(x => x.EXAM_GROUP_ID).ToList();
            return Exam_Not_Created(grouped_exams);
        }
        private IEnumerable<ValidationResult> FA_Group_Valid(ValidationContext validationContext)
        {
            var FAGroups = db.FA_GROUP.GroupBy(x => x.CCE_EXAM_CAT_ID)
               .Select(g => new { membername = g.Key, length = g.Count() });

            foreach (var fg in FAGroups)
            {
                if(fg.length > 2)
                {
                    yield return new ValidationResult($"* Cannot have more than 2 fa group under a single exam category.", new[] { "FA_GROUP" });
                }
            }          
        }
    }
}
    