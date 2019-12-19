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
        public bool Select { get; set; }
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
            var levels = this.GRADING_LEVEL.Where(x => x.IS_DEL == false).AsEnumerable();
            if(levels == null || levels.Count() == 0)
            {
                levels = db.GRADING_LEVEL.FirstOrDefault().Default();
            }
            return levels;
        }
        public IEnumerable<FINANCE_FEE_COLLECTION> Fee_Collection_Dates()
        {
            return db.FINANCE_FEE_COLLECTION.Where(x => x.BTCH_ID == this.ID && x.IS_DEL == false).AsEnumerable();
        }
        public IEnumerable<STUDENT> All_Students()
        {
            return db.STUDENTs.Where(x => x.BTCH_ID == this.ID && x.IS_DEL == false).AsEnumerable();
        }
        public IEnumerable<SUBJECT> Normal_Batch_Subject()
        {
            return db.SUBJECTs.Where(x => x.BTCH_ID == this.ID && x.ELECTIVE_GRP_ID == null && x.IS_DEL == false).AsEnumerable();
        }
        public IEnumerable<SUBJECT> Elective_Batch_Subject(int? elect_group)
        {
            return db.SUBJECTs.Where(x => x.BTCH_ID == this.ID && x.ELECTIVE_GRP_ID == elect_group && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false).AsEnumerable();
        }

        //Below 3 functions should be called through scheduled job in future state 
        //They are refered in Exam Controller
        //job_name = "Batch - Generate Reports" if @job_type=="Batch/1" 
        //job_name = "Batch - Generate Previous Reports" if @job_type=="Batch/2" 
        //job_name = "Batch - Generate CCE Reports" if @job_type=="Batch/3"
        public void Generate_Batch_Reports()
        {
            var grading_type = this.GRADING_TYPE;
            var students = this.All_Students();
            var grouped_exam_ids = db.GROUPED_EXAM.Where(x => x.BTCH_ID == this.ID).Select(x => x.EXAM_GROUP_ID).ToList();
            var grouped_exams = db.EXAM_GROUP.Where(x => x.BTCH_ID == this.ID && grouped_exam_ids.Contains(x.ID)).ToList();
            if(grouped_exams != null && grouped_exams.Count() != 0)
            {
                var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == this.ID && x.IS_DEL == false).ToList();
                if(students != null && students.Count() != 0)
                {
                    var std_ids = students.Select(x => x.ID).ToList();
                    var st_scores = db.GROUPED_EXAM_REPORT.Where(x => x.BTCH_ID == this.ID && std_ids.Contains((int)x.STDNT_ID)).ToList();
                    if(st_scores != null && st_scores.Count() != 0)
                    {
                        foreach(var sc in st_scores)
                        {
                            db.GROUPED_EXAM_REPORT.Remove(sc);
                        }
                        db.SaveChanges();
                    }
                    //List<int?> subject_marks = new List<int?>();
                    DataTable subject_marks = new DataTable();
                    subject_marks.Columns.Add("Student_Id", typeof(int));
                    subject_marks.Columns.Add("Subject_Id", typeof(int));
                    subject_marks.Columns.Add("Percentage", typeof(decimal));
                    //List<int?> exam_marks = new List<int?>();
                    DataTable exam_marks = new DataTable();
                    exam_marks.Columns.Add("Student_Id", typeof(int));
                    exam_marks.Columns.Add("Exam_Group_Id", typeof(int));
                    exam_marks.Columns.Add("Marks", typeof(decimal));
                    exam_marks.Columns.Add("Max_Marks", typeof(decimal));
                    foreach (var exam_group in grouped_exams)
                    {
                        foreach(var subject in subjects)
                        {
                            EXAM exam = db.EXAMs.Where(x => x.EXAM_GRP_ID == exam_group.ID && x.SUBJ_ID == subject.ID).FirstOrDefault();
                            if(exam != null)
                            {
                                foreach(var student in students)
                                {
                                    var is_assigned_elective = 1;
                                    if(subject.ELECTIVE_GRP_ID != null)
                                    {
                                        var assigned = db.STUDENT_SUBJECT.Where(x => x.SUBJ_ID == subject.ID && x.STDNT_ID == student.ID).ToList();
                                        if(assigned == null || assigned.Count() == 0)
                                        {
                                            is_assigned_elective = 0;
                                        }
                                    }
                                    if(is_assigned_elective != 0)
                                    {
                                        decimal? percentage = 0;
                                        decimal? marks = 0;
                                        EXAM_SCORE score = db.EXAM_SCORE.Include(x=>x.GRADING_LEVEL).Where(x => x.EXAM_ID == exam.ID && x.STDNT_ID == student.ID).FirstOrDefault();
                                        if(grading_type == null || this.Normal_Enabled())
                                        {
                                            if(score != null && score.MKS != null)
                                            {
                                                percentage = (((score.MKS) / exam.MAX_MKS) * 100) * ((exam_group.Weightage) / 100);
                                                marks = (decimal)score.MKS;
                                            }
                                        }
                                        else if(this.GPA_Enabled())
                                        {
                                            if(score != null && score.GRADING_LVL_ID != null)
                                            {
                                                percentage = (score.GRADING_LEVEL.CRED_PT) * ((exam_group.Weightage) / 100);
                                                marks = (score.GRADING_LEVEL.CRED_PT) * (subject.CR_HRS);
                                            }
                                        }
                                        else if(this.CWA_Enabled())
                                        {
                                            if(score != null && score.MKS != null)
                                            {
                                                percentage = (((score.MKS) / exam.MAX_MKS) * 100) * ((exam_group.Weightage) / 100);
                                                marks = (((score.MKS) / exam.MAX_MKS) * 100) * (subject.CR_HRS);
                                            }
                                        }
                                        var flag = 0;
                                        foreach(var s in subject_marks.AsEnumerable())
                                        {
                                            if((int)s["Student_Id"] == student.ID && (int)s["Subject_Id"] == subject.ID)
                                            {
                                                s["Percentage"] = (decimal)s["Percentage"] + percentage;
                                                flag = 1;
                                            }
                                        }
                                        if(flag != 1)
                                        {
                                            var row = subject_marks.NewRow();
                                            row["Student_Id"] = student.ID;
                                            row["Subject_Id"] = subject.ID;
                                            row["Percentage"] = percentage;
                                            subject_marks.Rows.Add(row);
                                        }
                                        var e_flag = 0;
                                        foreach(var e in exam_marks.AsEnumerable())
                                        {
                                            if((int)e["Student_Id"] == student.ID && (int)e["Exam_Group_Id"] == exam_group.ID)
                                            {
                                                e["Marks"] = (decimal)e["Marks"] + marks;
                                                if(grading_type == null || this.Normal_Enabled())
                                                {
                                                    e["Max_Marks"] = (decimal)e["Max_Marks"] + exam.MAX_MKS;
                                                }
                                                else if (this.GPA_Enabled() || this.CWA_Enabled())
                                                {
                                                    e["Max_Marks"] = (decimal)e["Max_Marks"] + subject.CR_HRS;
                                                }
                                                e_flag = 1;
                                            }
                                        }
                                        if(e_flag != 1)
                                        {
                                            if(grading_type == null || this.Normal_Enabled())
                                            {
                                                var row = exam_marks.NewRow();
                                                row["Student_Id"] = student.ID;
                                                row["Exam_Group_Id"] = exam_group.ID;
                                                row["Marks"] = marks;
                                                row["Max_Marks"] = exam.MAX_MKS;
                                                exam_marks.Rows.Add(row);
                                            }
                                            else if(this.GPA_Enabled() || this.CWA_Enabled())
                                            {
                                                var row = exam_marks.NewRow();
                                                row["Student_Id"] = student.ID;
                                                row["Exam_Group_Id"] = exam_group.ID;
                                                row["Marks"] = marks;
                                                row["Max_Marks"] = subject.CR_HRS;
                                                exam_marks.Rows.Add(row);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    foreach(var subject_mark in subject_marks.AsEnumerable())
                    {
                        var student_id = (int)subject_mark["Student_Id"];
                        var subject_id = (int)subject_mark["Subject_Id"];
                        var marks = (decimal)subject_mark["Percentage"];
                        GROUPED_EXAM_REPORT prev_marks = db.GROUPED_EXAM_REPORT.Where(x => x.STDNT_ID == student_id && x.SUBJ_ID == subject_id && x.BTCH_ID == this.ID && x.SCORE_TYPE == "s").FirstOrDefault();
                        if(prev_marks != null)
                        {
                            prev_marks.MARKS = marks;
                            db.Entry(prev_marks).State = EntityState.Modified;
                        }
                        else
                        {
                            GROUPED_EXAM_REPORT New_Marks = new GROUPED_EXAM_REPORT() { STDNT_ID = student_id, SUBJ_ID = subject_id, BTCH_ID = this.ID, SCORE_TYPE = "s", MARKS = marks };
                            db.GROUPED_EXAM_REPORT.Add(New_Marks);
                        }
                    }
                    DataTable exam_totals = new DataTable();
                    exam_totals.Columns.Add("Student_Id", typeof(int));
                    exam_totals.Columns.Add("Percentage", typeof(decimal));
                    foreach(var exam_mark in exam_marks.AsEnumerable())
                    {
                        var student_id = (int)exam_mark["Student_Id"];
                        var exam_group = db.EXAM_GROUP.Find((int)exam_mark["Exam_Group_Id"]);
                        var score = (decimal)exam_mark["Marks"];
                        var max_marks = (decimal)exam_mark["Max_Marks"];
                        decimal tot_score = 0;
                        decimal percent = 0;
                        if(max_marks != 0)
                        {
                            if(grading_type == null || this.Normal_Enabled())
                            {
                                tot_score = (((score) / max_marks) * 100);
                                percent = (((score) / max_marks) * 100) * ((exam_group.Weightage) / 100);
                            }
                            else if (this.GPA_Enabled() || this.CWA_Enabled())
                            {
                                tot_score = ((score) / max_marks);
                                percent = ((score) / max_marks) * ((exam_group.Weightage) / 100);
                            }
                        }
                        GROUPED_EXAM_REPORT prev_exam_score = db.GROUPED_EXAM_REPORT.Where(x => x.STDNT_ID == student_id && x.EXAM_GROUP_ID == exam_group.ID && x.BTCH_ID == this.ID && x.SCORE_TYPE == "e").FirstOrDefault();
                        if (prev_exam_score != null)
                        {
                            prev_exam_score.MARKS = tot_score;
                            db.Entry(prev_exam_score).State = EntityState.Modified;
                        }
                        else
                        {
                            GROUPED_EXAM_REPORT New_Exam_Score = new GROUPED_EXAM_REPORT() { STDNT_ID = student_id, EXAM_GROUP_ID = exam_group.ID, BTCH_ID = this.ID, SCORE_TYPE = "e", MARKS = tot_score };
                            db.GROUPED_EXAM_REPORT.Add(New_Exam_Score);
                        }
                        var exam_flag = 0;
                        foreach (var total in exam_totals.AsEnumerable())
                        { 
                            if((int)total["Student_Id"] == student_id)
                            {
                                total["Percentage"] = (decimal)total["Percentage"] + percent;
                                exam_flag = 1;
                            }
                        }
                        if(exam_flag != 1)
                        {
                            var row = exam_totals.NewRow();
                            row["Student_Id"] = student_id;
                            row["Percentage"] = percent;
                            exam_totals.Rows.Add(row);
                        }
                    }
                    foreach(var exam_total in exam_totals.AsEnumerable())
                    {
                        var student_id = (int)exam_total["Student_Id"];
                        var total = (decimal)exam_total["Percentage"];
                        GROUPED_EXAM_REPORT prev_total_score = db.GROUPED_EXAM_REPORT.Where(x => x.STDNT_ID == student_id && x.BTCH_ID == this.ID && x.SCORE_TYPE == "c").FirstOrDefault();
                        if(prev_total_score != null)
                        {
                            prev_total_score.MARKS = total;
                            db.Entry(prev_total_score).State = EntityState.Modified;
                        }
                        else
                        {
                            GROUPED_EXAM_REPORT New_Total_Score = new GROUPED_EXAM_REPORT() { STDNT_ID = student_id, BTCH_ID = this.ID, MARKS = total, SCORE_TYPE = "c" };
                            db.GROUPED_EXAM_REPORT.Add(New_Total_Score);
                        }
                    }
                    db.SaveChanges();
                }
            }
        }
        public void Generate_Previous_Batch_Reports()
        {
            var grading_type = this.GRADING_TYPE;
            //var students = this.All_Students();
            var students = db.STUDENTs.Where(x => x.ID == -1).AsEnumerable();
            var batch_students = db.BATCH_STUDENT.Where(x => x.BTCH_ID == this.ID).ToList();
            foreach(var bs in batch_students)
            {
                var stu = db.STUDENTs.Where(x=>x.ID == bs.STDNT_ID);
                if(stu != null)
                {
                    students = students.Union(stu);
                }
            }
            students = students.ToList();
            var grouped_exam_ids = db.GROUPED_EXAM.Where(x => x.BTCH_ID == this.ID).Select(x => x.EXAM_GROUP_ID).ToList();
            var grouped_exams = db.EXAM_GROUP.Where(x => x.BTCH_ID == this.ID && grouped_exam_ids.Contains(x.ID)).ToList();
            if (grouped_exams != null && grouped_exams.Count() != 0)
            {
                var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == this.ID && x.IS_DEL == false).ToList();
                if (students != null && students.Count() != 0)
                {
                    var std_ids = students.Select(x => x.ID).ToList();
                    var st_scores = db.GROUPED_EXAM_REPORT.Where(x => x.BTCH_ID == this.ID && std_ids.Contains((int)x.STDNT_ID)).ToList();
                    if (st_scores != null && st_scores.Count() != 0)
                    {
                        foreach (var sc in st_scores)
                        {
                            db.GROUPED_EXAM_REPORT.Remove(sc);
                        }
                        db.SaveChanges();
                    }
                    //List<int?> subject_marks = new List<int?>();
                    DataTable subject_marks = new DataTable();
                    subject_marks.Columns.Add("Student_Id", typeof(int));
                    subject_marks.Columns.Add("Subject_Id", typeof(int));
                    subject_marks.Columns.Add("Percentage", typeof(decimal));
                    //List<int?> exam_marks = new List<int?>();
                    DataTable exam_marks = new DataTable();
                    exam_marks.Columns.Add("Student_Id", typeof(int));
                    exam_marks.Columns.Add("Exam_Group_Id", typeof(int));
                    exam_marks.Columns.Add("Marks", typeof(decimal));
                    exam_marks.Columns.Add("Max_Marks", typeof(decimal));
                    foreach (var exam_group in grouped_exams)
                    {
                        foreach (var subject in subjects)
                        {
                            EXAM exam = db.EXAMs.Where(x => x.EXAM_GRP_ID == exam_group.ID && x.SUBJ_ID == subject.ID).FirstOrDefault();
                            if (exam != null)
                            {
                                foreach (var student in students)
                                {
                                    var is_assigned_elective = 1;
                                    if (subject.ELECTIVE_GRP_ID != null)
                                    {
                                        var assigned = db.STUDENT_SUBJECT.Where(x => x.SUBJ_ID == subject.ID && x.STDNT_ID == student.ID).ToList();
                                        if (assigned == null || assigned.Count() == 0)
                                        {
                                            is_assigned_elective = 0;
                                        }
                                    }
                                    if (is_assigned_elective != 0)
                                    {
                                        decimal? percentage = 0;
                                        decimal? marks = 0;
                                        EXAM_SCORE score = db.EXAM_SCORE.Include(x => x.GRADING_LEVEL).Where(x => x.EXAM_ID == exam.ID && x.STDNT_ID == student.ID).FirstOrDefault();
                                        if (grading_type == null || this.Normal_Enabled())
                                        {
                                            if (score != null && score.MKS != null)
                                            {
                                                percentage = (((score.MKS) / exam.MAX_MKS) * 100) * ((exam_group.Weightage) / 100);
                                                marks = (decimal)score.MKS;
                                            }
                                        }
                                        else if (this.GPA_Enabled())
                                        {
                                            if (score != null && score.GRADING_LVL_ID != null)
                                            {
                                                percentage = (score.GRADING_LEVEL.CRED_PT) * ((exam_group.Weightage) / 100);
                                                marks = (score.GRADING_LEVEL.CRED_PT) * (subject.CR_HRS);
                                            }
                                        }
                                        else if (this.CWA_Enabled())
                                        {
                                            if (score != null && score.MKS != null)
                                            {
                                                percentage = (((score.MKS) / exam.MAX_MKS) * 100) * ((exam_group.Weightage) / 100);
                                                marks = (((score.MKS) / exam.MAX_MKS) * 100) * (subject.CR_HRS);
                                            }
                                        }
                                        var flag = 0;
                                        foreach (var s in subject_marks.AsEnumerable())
                                        {
                                            if ((int)s["Student_Id"] == student.ID && (int)s["Subject_Id"] == subject.ID)
                                            {
                                                s["Percentage"] = (decimal)s["Percentage"] + percentage;
                                                flag = 1;
                                            }
                                        }
                                        if (flag != 1)
                                        {
                                            var row = subject_marks.NewRow();
                                            row["Student_Id"] = student.ID;
                                            row["Subject_Id"] = subject.ID;
                                            row["Percentage"] = percentage;
                                            subject_marks.Rows.Add(row);
                                        }
                                        var e_flag = 0;
                                        foreach (var e in exam_marks.AsEnumerable())
                                        {
                                            if ((int)e["Student_Id"] == student.ID && (int)e["Exam_Group_Id"] == exam_group.ID)
                                            {
                                                e["Marks"] = (decimal)e["Marks"] + marks;
                                                if (grading_type == null || this.Normal_Enabled())
                                                {
                                                    e["Max_Marks"] = (decimal)e["Max_Marks"] + exam.MAX_MKS;
                                                }
                                                else if (this.GPA_Enabled() || this.CWA_Enabled())
                                                {
                                                    e["Max_Marks"] = (decimal)e["Max_Marks"] + subject.CR_HRS;
                                                }
                                                e_flag = 1;
                                            }
                                        }
                                        if (e_flag != 1)
                                        {
                                            if (grading_type == null || this.Normal_Enabled())
                                            {
                                                var row = exam_marks.NewRow();
                                                row["Student_Id"] = student.ID;
                                                row["Exam_Group_Id"] = exam_group.ID;
                                                row["Marks"] = marks;
                                                row["Max_Marks"] = exam.MAX_MKS;
                                                exam_marks.Rows.Add(row);
                                            }
                                            else if (this.GPA_Enabled() || this.CWA_Enabled())
                                            {
                                                var row = exam_marks.NewRow();
                                                row["Student_Id"] = student.ID;
                                                row["Exam_Group_Id"] = exam_group.ID;
                                                row["Marks"] = marks;
                                                row["Max_Marks"] = subject.CR_HRS;
                                                exam_marks.Rows.Add(row);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    foreach (var subject_mark in subject_marks.AsEnumerable())
                    {
                        var student_id = (int)subject_mark["Student_Id"];
                        var subject_id = (int)subject_mark["Subject_Id"];
                        var marks = (decimal)subject_mark["Percentage"];
                        GROUPED_EXAM_REPORT prev_marks = db.GROUPED_EXAM_REPORT.Where(x => x.STDNT_ID == student_id && x.SUBJ_ID == subject_id && x.BTCH_ID == this.ID && x.SCORE_TYPE == "s").FirstOrDefault();
                        if (prev_marks != null)
                        {
                            prev_marks.MARKS = marks;
                            db.Entry(prev_marks).State = EntityState.Modified;
                        }
                        else
                        {
                            GROUPED_EXAM_REPORT New_Marks = new GROUPED_EXAM_REPORT() { STDNT_ID = student_id, SUBJ_ID = subject_id, BTCH_ID = this.ID, SCORE_TYPE = "s", MARKS = marks };
                            db.GROUPED_EXAM_REPORT.Add(New_Marks);
                        }
                    }
                    DataTable exam_totals = new DataTable();
                    exam_totals.Columns.Add("Student_Id", typeof(int));
                    exam_totals.Columns.Add("Percentage", typeof(decimal));
                    foreach (var exam_mark in exam_marks.AsEnumerable())
                    {
                        var student_id = (int)exam_mark["Student_Id"];
                        var exam_group = db.EXAM_GROUP.Find((int)exam_mark["Exam_Group_Id"]);
                        var score = (decimal)exam_mark["Marks"];
                        var max_marks = (decimal)exam_mark["Max_Marks"];
                        decimal tot_score = 0;
                        decimal percent = 0;
                        if (max_marks != 0)
                        {
                            if (grading_type == null || this.Normal_Enabled())
                            {
                                tot_score = (((score) / max_marks) * 100);
                                percent = (((score) / max_marks) * 100) * ((exam_group.Weightage) / 100);
                            }
                            else if (this.GPA_Enabled() || this.CWA_Enabled())
                            {
                                tot_score = ((score) / max_marks);
                                percent = ((score) / max_marks) * ((exam_group.Weightage) / 100);
                            }
                        }
                        GROUPED_EXAM_REPORT prev_exam_score = db.GROUPED_EXAM_REPORT.Where(x => x.STDNT_ID == student_id && x.EXAM_GROUP_ID == exam_group.ID && x.BTCH_ID == this.ID && x.SCORE_TYPE == "e").FirstOrDefault();
                        if (prev_exam_score != null)
                        {
                            prev_exam_score.MARKS = tot_score;
                            db.Entry(prev_exam_score).State = EntityState.Modified;
                        }
                        else
                        {
                            GROUPED_EXAM_REPORT New_Exam_Score = new GROUPED_EXAM_REPORT() { STDNT_ID = student_id, EXAM_GROUP_ID = exam_group.ID, BTCH_ID = this.ID, SCORE_TYPE = "e", MARKS = tot_score };
                            db.GROUPED_EXAM_REPORT.Add(New_Exam_Score);
                        }
                        var exam_flag = 0;
                        foreach (var total in exam_totals.AsEnumerable())
                        {
                            if ((int)total["Student_Id"] == student_id)
                            {
                                total["Percentage"] = (decimal)total["Percentage"] + percent;
                                exam_flag = 1;
                            }
                        }
                        if (exam_flag != 1)
                        {
                            var row = exam_totals.NewRow();
                            row["Student_Id"] = student_id;
                            row["Percentage"] = percent;
                            exam_totals.Rows.Add(row);
                        }
                    }
                    foreach (var exam_total in exam_totals.AsEnumerable())
                    {
                        var student_id = (int)exam_total["Student_Id"];
                        var total = (decimal)exam_total["Percentage"];
                        GROUPED_EXAM_REPORT prev_total_score = db.GROUPED_EXAM_REPORT.Where(x => x.STDNT_ID == student_id && x.BTCH_ID == this.ID && x.SCORE_TYPE == "c").FirstOrDefault();
                        if (prev_total_score != null)
                        {
                            prev_total_score.MARKS = total;
                            db.Entry(prev_total_score).State = EntityState.Modified;
                        }
                        else
                        {
                            GROUPED_EXAM_REPORT New_Total_Score = new GROUPED_EXAM_REPORT() { STDNT_ID = student_id, BTCH_ID = this.ID, MARKS = total, SCORE_TYPE = "c" };
                            db.GROUPED_EXAM_REPORT.Add(New_Total_Score);
                        }
                    }
                    db.SaveChanges();
                }
            }
        }
        public void Generate_CCE_Reports()
        {
            
        }

    }
}