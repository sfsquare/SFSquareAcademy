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
    public enum BatchStatus
    {
        [Display(Name = "Active")]
        Active,
        [Display(Name = "Inactive")]
        Inactive
    }
    public interface IBATCHMetaData
    {
/*
        [Display(Name = "Full Name")]
        string course_full_name { get; set; }
*/
    }

    public enum BatchGradingTypes
    {
        Normal = 0,
        GPA = 1,
        CWA = 2,
        CCE = 3
    }
    public class BatchSelect
    {
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public bool Select { get; set; }
    }

    [MetadataType(typeof(BatchMetadata))]
    public partial class BATCH : IValidatableObject
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class BatchMetadata
        {
            [Required]
            public string NAME { get; set; }

            [Remote("StGraterThenEnd", "BATCH", AdditionalFields = "END_DATE", ErrorMessage = "Start Date should be before End Date")]
            [Required]
            public DateTime? START_DATE { get; set; }
            [Required]
            public DateTime? END_DATE { get; set; }

        }
        public bool IMPORT_FEES { get; set; }
        public bool IMPORT_SUBJECTS { get; set; }
        public IEnumerable<BATCH> ACTIVE()
        {
            var ActiveBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.COURSE.CODE).OrderBy(x => x.NAME);
            return (IEnumerable<BATCH>)ActiveBatch;
        }

        public IEnumerable<BATCH> INACTIVE()
        {
            var InactiveBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.IS_DEL == false && x.IS_ACT == false).OrderBy(x => x.COURSE.CODE).OrderBy(x => x.NAME);
            return (IEnumerable<BATCH>)InactiveBatch;
        }

        public IEnumerable<BATCH> DELETED()
        {
            var DeletedBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.IS_DEL == true).OrderBy(x => x.COURSE.CODE).OrderBy(x => x.NAME);
            return (IEnumerable<BATCH>)DeletedBatch;
        }

        public IEnumerable<BATCH> CCE()
        {
            var CCEBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.COURSE.GRADING_TYPE == 3).OrderBy(x => x.COURSE.CODE).OrderBy(x => x.NAME);
            return (IEnumerable<BATCH>)CCEBatch;
        }
        public string Course_full_name
        {
            get { return COURSE.CODE + "-" + NAME; }
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(NAME))
            {
                //yield return new ValidationResult($"Classic movies must have a release year earlier than {_classicYear}.", new[] { "ReleaseDate" });
                yield return new ValidationResult($"Batch Name is mandatory.", new[] { "NAME" });
            }
            if (START_DATE == null || END_DATE == null)
            {             
                yield return new ValidationResult($"Start Date or End Date canot be null.", new[] { "START_DATE" });
            }
            if (START_DATE > END_DATE)
            {
                yield return new ValidationResult($"Start Date should be before End Date", new[] { "END_DATE" });
            }
        }

        public List<DateTime> Holiday_Event_Dates()
        {
            var common_holidays = db.EVENTs.Where(x => x.IS_HOL == true && x.IS_COMN == true).ToList();
            var batch_holidays = db.EVENTs.Where(x => x.IS_HOL == true && x.IS_COMN == false).ToList();
            var all_holiday_events = common_holidays.Union(batch_holidays).Distinct();
            List<DateTime> event_holidays = new List<DateTime>();
            foreach(var eve in all_holiday_events)
            {
                for (var dt = (DateTime)eve.START_DATE; dt <= eve.END_DATE; dt = dt.AddDays(1))
                {
                    event_holidays.Add(dt);
                }
            }
            return event_holidays;
        }
        public List<DateTime> Academic_Days()
        {
            List<DateTime> all_days = new List<DateTime>();
            for (var dt = (DateTime)START_DATE; dt <= DateTime.Now; dt = dt.AddDays(1))
            {
                all_days.Add(dt);
            }
            var weekdays = db.WEEKDAYs.FirstOrDefault().Weekday_By_Day(ID);
            var holidays = Holiday_Event_Dates();
            var non_holidays = all_days.Except(holidays).ToList();
            var range = non_holidays.Where(x => weekdays.Any(s => s.DAY_OF_WK == (int)x.DayOfWeek)).ToList();
            return range;
        }

        public List<SelectListItem> Subject_Hours(DateTime? starting_date, DateTime? ending_date, int? subject_id)
        {
            var entries = db.TIMETABLE_ENTRY.Where(x => x.ID == -1).DefaultIfEmpty().ToList();
            if (subject_id != 0)
            {
                SUBJECT subject = db.SUBJECTs.Find(subject_id);
                int? ElectiveGroupId = subject.ELECTIVE_GRP_ID;
                if(subject.ELECTIVE_GRP_ID != null)
                {
                    subject = db.SUBJECTs.Where(x => x.ELECTIVE_GRP_ID == ElectiveGroupId).FirstOrDefault();
                }
                entries = db.TIMETABLE_ENTRY.Include(x => x.TIMETABLE).Include(x => x.WEEKDAY).Where(x => ((x.TIMETABLE.START_DATE <= starting_date && x.TIMETABLE.END_DATE >= starting_date) || (x.TIMETABLE.START_DATE <= ending_date && x.TIMETABLE.END_DATE >= ending_date) || (starting_date <= x.TIMETABLE.START_DATE && ending_date >= x.TIMETABLE.START_DATE) || (starting_date <= x.TIMETABLE.END_DATE && ending_date >= x.TIMETABLE.END_DATE)) && x.SUBJ_ID == subject_id && x.BTCH_ID == ID).ToList();
            }
            else
            {
                entries = db.TIMETABLE_ENTRY.Include(x => x.TIMETABLE).Include(x => x.WEEKDAY).Where(x => ((x.TIMETABLE.START_DATE <= starting_date && x.TIMETABLE.END_DATE >= starting_date) || (x.TIMETABLE.START_DATE <= ending_date && x.TIMETABLE.END_DATE >= ending_date) || (starting_date <= x.TIMETABLE.START_DATE && ending_date >= x.TIMETABLE.START_DATE) || (starting_date <= x.TIMETABLE.END_DATE && ending_date >= x.TIMETABLE.END_DATE)) && x.BTCH_ID == ID).ToList();
            }
            List<int?> timetable_ids = new List<int?>();
            foreach(var ent in entries.Select(x => x.TIMT_ID).Distinct().ToList())
            {
                timetable_ids.Add(ent);
            }
            var holidays = Holiday_Event_Dates();
            List<SelectListItem> hsh2 = new List<SelectListItem>();
            if (timetable_ids != null)
            {
                var timetables = db.TIMETABLEs.Where(x => timetable_ids.Any(s => x.ID == s)).ToList();
                List<DateTime> Max_date = new List<DateTime>();
                List<DateTime> Min_date = new List<DateTime>();
                //int Index = 1;
                foreach (var tt in timetables)
                {
                    Max_date.Add((DateTime)starting_date); Max_date.Add((DateTime)tt.START_DATE); Max_date.Add((DateTime)START_DATE);
                    Min_date.Add((DateTime)ending_date); Min_date.Add((DateTime)tt.END_DATE); Min_date.Add((DateTime)END_DATE); Min_date.Add(DateTime.Now);
                    for (var dt = Max_date.Max(); dt <= Min_date.Min(); dt = (DateTime)dt.AddDays(1))
                    {
                        foreach (var entr in entries)
                        {
                            if (entr.TIMT_ID == tt.ID && entr.WEEKDAY.DAY_OF_WK == (int)dt.DayOfWeek)
                            {
                                var result = new SelectListItem();
                                result.Text = entr.ID.ToString();
                                result.Value = dt.Date.ToString();
                                hsh2.Add(result);
                                //hsh2.Insert(Index, new SelectListItem() { Value = dt.Date.ToString(), Text = entr.ID.ToString() });
                                //Index += 1;
                            }
                        }
                    }
                }
            }
            foreach (var hld in holidays)
            {
                foreach (var item in hsh2)
                {
                    if(hld == Convert.ToDateTime(item.Value))
                    {
                        hsh2.Remove(item);
                    }
                }
            }
            return hsh2;
        }

        public List<DateTime> Working_Days(DateTime date)
        {
            List<DateTime> start = new List<DateTime>();
            start.Add((DateTime)START_DATE);
            DateTime BeginDate = new DateTime(date.Year, date.Month, 1);
            start.Add(BeginDate);

            List<DateTime> stop = new List<DateTime>();
            stop.Add((DateTime)END_DATE);
            DateTime EndDate = BeginDate.AddMonths(1).AddDays(-1);
            stop.Add(EndDate);

            var weekdays = db.WEEKDAYs.Where(x => x.BTCH_ID == ID).ToList();
            if (weekdays == null)
            {
                weekdays = db.WEEKDAYs.Where(x => x.BTCH_ID == null).ToList();
            }
            List<DateTime> holidays = Holiday_Event_Dates();

            List<DateTime> all_days = new List<DateTime>();
            for (var dt = start.Max(); dt <= stop.Min(); dt = dt.AddDays(1))
            {
                bool Select = false;
                foreach (var WkD in weekdays)
                {
                    if((int)dt.DayOfWeek == WkD.DAY_OF_WK)
                    {
                        Select = true;
                        break;;
                    }
                }
                foreach(var hld in holidays)
                {
                    if(dt == hld.Date)
                    {
                        Select = false;
                        break; ;
                    }
                }
                if(Select == true)
                {
                    all_days.Add(dt);
                }
            }
            return all_days;
        }
        public bool GPA_Enabled()
        {
            var gpa_Enabled = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "GPA").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            if (gpa_Enabled == "1" && GRADING_TYPE == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool CWA_Enabled()
        {
            var cwa_Enabled = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CWA").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            if (cwa_Enabled == "1" && GRADING_TYPE == 2)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool CCE_Enabled()
        {
            var cce_Enabled = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CCE").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            if (cce_Enabled == "1" && GRADING_TYPE == 3)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool Normal_Enabled()
        {
            if (GRADING_TYPE == null || GRADING_TYPE == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool Allow_Exam_Acess(USER user)
        {
            var flag = true;
            if (user.EMP_IND == true && user.GetUserPrivilage(user.ID).Select(x=>x.NAME).ToList().Contains("ExaminationControl"))
            {
                var EmpSubjs = db.EMPLOYEES_SUBJECT.Include(x=>x.SUBJECT).Where(x => x.EMP_ID == user.EMPLOYEEs.FirstOrDefault().ID).ToList();
                if(EmpSubjs.Select(x=>x.SUBJECT.BTCH_ID).ToList().Contains(ID))
                {
                    flag = false;
                }
            }
            return flag;

        }
        public IEnumerable<EMPLOYEE> Employees()
        {
            var Employee = db.EMPLOYEEs.Where(x => x.ID == -1).DefaultIfEmpty().AsEnumerable();
            if(EMP_ID != null)
            {
                var employee_ids = HtmlHelpers.ApplicationHelper.SplitCommaString(EMP_ID);
                Employee = db.EMPLOYEEs.Where(x => employee_ids.Contains(x.ID.ToString()));
            }
            return Employee;
        }
        public void Inactivate()
        {
            this.IS_DEL = true;
            db.Entry(this).State = EntityState.Modified;
            var EmpSubToUpdate = db.EMPLOYEES_SUBJECT.Include(x=>x.SUBJECT).Where(x => x.SUBJECT.BTCH_ID == this.ID);
            foreach (var es in EmpSubToUpdate)
            {
                db.EMPLOYEES_SUBJECT.Remove(es);
            }
            db.SaveChanges();
        }
        public string Full_Name
        {
            get { return COURSE.CODE + "-" + NAME; }
        }
        public string Course_Section_Name
        {
            get { return COURSE.CRS_NAME + "-" + COURSE.SECTN_NAME; }
        }
        public IEnumerable<GRADING_LEVEL> Grading_Level_List()
        {
            return db.GRADING_LEVEL.Where(x => x.BTCH_ID == this.ID).ToList();
        }
        public IEnumerable<FINANCE_FEE_COLLECTION> Fee_Collection_Dates()
        {
            return db.FINANCE_FEE_COLLECTION.Where(x => x.BTCH_ID == this.ID && x.IS_DEL == false).AsEnumerable();
        }
        public IEnumerable<STUDENT> All_Students()
        {
            return db.STUDENTs.Where(x => x.BTCH_ID == this.ID).AsEnumerable();
        }
        public IEnumerable<SUBJECT> Normal_Batch_Subject()
        {
            return db.SUBJECTs.Where(x => x.BTCH_ID == this.ID && x.ELECTIVE_GRP_ID == null && x.IS_DEL == false).AsEnumerable();
        }
        public IEnumerable<SUBJECT> Elective_Batch_Subject(int? elect_group)
        {
            return db.SUBJECTs.Where(x => x.BTCH_ID == this.ID && x.ELECTIVE_GRP_ID == elect_group && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false).AsEnumerable();
        }

    }
}