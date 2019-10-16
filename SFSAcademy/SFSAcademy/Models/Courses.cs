using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace SFSAcademy
{
    public class CoursesBatch
    {
        public COURSE CourseData { get; set; }
        public BATCH BatchData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
        public SUBJECT Elective_Batch_Subject { get; set; }
        public int? Total_Time { get; set; }
    }

    public class SelectCourseBatch
    {
        public COURSE CourseData { get; set; }
        public BATCH BatchData { get; set; }
        public FINANCE_FEE_CATGEORY FeeCategoryData { get; set; }
        public bool Selected { get; set; }
    }

    public enum GradingTypes
    {
        Normal = 0,
        GPA = 1,
        CWA = 2,
        CCE = 3
    }
    [MetadataType(typeof(CourseMetadata))]
    public partial class COURSE : IValidatableObject, IHasTimeStamp, IHasBeforeSave
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class CourseMetadata
        {
            [Required]
            public string CRS_NAME { get; set; }

            [Remote("IsCodeExist", "Courses", ErrorMessage = "Course code already exist in database. Please enter a different code.")]
            [Required]
            public string CODE { get; set; }

            [Remote("InitialBatch", "Courses", AdditionalFields = "CODE", ErrorMessage = "Inital Batch must be added for this course.")]
            public string INIT_BATCH_NAME { get; set; }
        }
        public string INIT_BATCH_NAME { get; set; }

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
            if (string.IsNullOrEmpty(CRS_NAME) || string.IsNullOrEmpty(CODE))
            {
                string ErrorMessage = "Course Name and Course Code is mandatory.";
                //yield return new ValidationResult($"Classic movies must have a release year earlier than {_classicYear}.", new[] { "ReleaseDate" });
                yield return new ValidationResult($"*", new[] { ErrorMessage });
            }
        }
        public void Before_Save()
        {

        }
        public IEnumerable<COURSE> ACTIVE()
        {
            var ActiveCourse = db.COURSEs.Where(x => x.IS_DEL == false).OrderBy(x => x.CRS_NAME);
            return (IEnumerable<COURSE>)ActiveCourse;
        }

        public IEnumerable<COURSE> DELETED()
        {
            var DeletedCourse = db.COURSEs.Where(x => x.IS_DEL == true).OrderBy(x => x.CRS_NAME);
            return (IEnumerable<COURSE>)DeletedCourse;
        }

        public IEnumerable<COURSE> CCE()
        {
            var CCECourse = db.COURSEs.Where(x => x.GRADING_TYPE == 3).OrderBy(x => x.CRS_NAME);
            return (IEnumerable<COURSE>)CCECourse;
        }
        public void Inactivate()
        {
            COURSE cs = db.COURSEs.Find(ID);
            cs.IS_DEL = true;
            db.Entry(cs).State = EntityState.Modified;
            try { db.SaveChanges(); }
            catch (Exception e) { Console.WriteLine(e);}
        }
        public string Full_Name
        {
            get { return SECTN_NAME != null ? CRS_NAME + "-" + SECTN_NAME : CRS_NAME; }        
        }
        public IEnumerable<BATCH> Active_Batches()
        {
            return db.BATCHes.Where(x=>x.CRS_ID == ID && x.IS_ACT == true && x.IS_DEL == false).ToList();
        }
        public bool Has_batch_groups_with_active_batches()
        {
            var batch_groups = db.BATCH_GROUP.Include(x => x.GROUPED_BATCH).Where(x => x.CRS_ID == ID).ToList();
            if(batch_groups == null || batch_groups.Count() == 0)
            {
                return false;
            }
            else
            {
                foreach(var b in batch_groups)
                {
                    if(b.COURSE.Active_Batches() != null && b.COURSE.Active_Batches().Count() != 0)
                    {
                        return true;
                    }
                }
            }
            return false;         
        }
        public DataTable Find_Course_Rank(List<int?> batch_ids, string sort_order)
        {
            var batches = db.BATCHes.Where(x => batch_ids.Contains(x.ID)).ToList();
            var students = db.STUDENTs.Where(x => batch_ids.Contains(x.BTCH_ID)).ToList();
            var grouped_exams = db.EXAM_GROUP.Where(x => batch_ids.Contains(x.BTCH_ID)).ToList();
            List<decimal?> ordered_scores = new List<decimal?>();
            DataTable student_scores = new DataTable();
            student_scores.Columns.Add("Student_Id", typeof(int?));
            student_scores.Columns.Add("Marks", typeof(decimal?));
            DataTable ranked_students = new DataTable();
            student_scores.Columns.Add("Rank", typeof(int?));
            student_scores.Columns.Add("Marks", typeof(decimal?));
            student_scores.Columns.Add("Student_Id", typeof(int?));
            student_scores.Columns.Add("Student_FullName", typeof(string));

            foreach (var student in students)
            {
                GROUPED_EXAM_REPORT score = db.GROUPED_EXAM_REPORT.Where(x => x.STDNT_ID == student.ID && x.BTCH_ID == student.BTCH_ID && x.SCORE_TYPE == "C").FirstOrDefault();
                decimal? marks = 0;
                if(score != null)
                {
                    marks = score.MARKS;
                }
                ordered_scores.Add(marks);
                var row = student_scores.NewRow();
                row["Student_Id"] = student.ID;
                row["Marks"] = marks;
                student_scores.Rows.Add(row);

            }
            //ViewBag.student_scores = student_scores.AsEnumerable();
            ordered_scores = ordered_scores.Distinct().ToList();
            ordered_scores.Sort();
            ordered_scores.Reverse();
            foreach (var student in students)
            {               
                decimal? m = 0;
                foreach(var student_score in student_scores.AsEnumerable())
                {
                    if ((int)student_score["Student_Id"] == student.ID)
                    {
                        m = (decimal)student_score["Marks"];
                    }
                }
                if (sort_order == "" || sort_order == "rank-ascend" || sort_order == "rank-descend")
                {
                    var row = ranked_students.NewRow();
                    row["Rank"] = ordered_scores.IndexOf(m)+1;
                    row["Marks"] = m;
                    row["Student_Id"] = student.ID;
                    ranked_students.Rows.Add(row);
                }
                else
                {
                    var row = ranked_students.NewRow();
                    row["Student_FullName"] = student.Full_Name;
                    row["Rank"] = ordered_scores.IndexOf(m)+1;
                    row["Marks"] = m;
                    row["Student_Id"] = student.ID;
                    ranked_students.Rows.Add(row);
                }
            }
            if (sort_order == "" || sort_order == "rank-ascend" || sort_order == "name-ascend")
            {
                ranked_students.DefaultView.Sort = "Rank asc";
                ranked_students = ranked_students.DefaultView.ToTable();
            }
            else
            {
                ranked_students.DefaultView.Sort = "Rank desc";
                ranked_students = ranked_students.DefaultView.ToTable();
            }

            return ranked_students;
        }
        public bool CCE_Enabled()
        {
            var config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CCE").FirstOrDefault();
            if(config.CONFIG_VAL == "1" && GRADING_TYPE == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool GPA_Enabled()
        {
            var config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "GPA").FirstOrDefault();
            if (config.CONFIG_VAL == "1" && GRADING_TYPE == 1)
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
            var config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CWA").FirstOrDefault();
            if (config.CONFIG_VAL == "1" && GRADING_TYPE == 2)
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
            if (GRADING_TYPE == 0 && GRADING_TYPE == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IEnumerable<CCE_WEIGHTAGE> CCE_Weightages_for_Exam_Category(int? cce_exam_cateogry_id)
        {
            return db.CCE_WEIGHTAGE.Where(x => x.CCE_EXAM_CAT_ID == cce_exam_cateogry_id).ToList();
        }
    }

}
