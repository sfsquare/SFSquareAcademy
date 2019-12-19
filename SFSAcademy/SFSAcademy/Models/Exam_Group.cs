using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SFSAcademy
{
    public enum ExamType
    {
        [Display(Name = "Marks and Grades")]
        MarksAndGrades,
        [Display(Name = "Grades")]
        Grades,
        [Display(Name = "Marks")]
        Marks
    }

    [MetadataType(typeof(ExamGrMetadata))]
    public partial class EXAM_GROUP : IValidatableObject, IHasBeforeSave, IHasBeforeDestroy
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class ExamGrMetadata
        {
            [Required]
            public string NAME { get; set; }

            [Remote("UniqueCCECat", "Exam_Groups", AdditionalFields = "BTCH_ID", ErrorMessage = "Already assigned for another Exam Group.")]
            public int? CCE_EXAM_CAT_ID { get; set; }
        }
        private string ErrorMessage { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(NAME))
            {
                ErrorMessage = "Name is mandatory.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "NAME" });

            }
        }
        public void Before_Save()
        {
            this.EXAM_DATE = this.EXAM_DATE != null ? this.EXAM_DATE : DateTime.Today;
            if(this.EXAM_TYPE.ToLower() == "grades")
            {
                foreach(var ex in db.EXAMs.Where(x=>x.EXAM_GRP_ID == this.ID))
                {
                    ex.MAX_MKS = 0;
                    ex.MIN_MKS = 0;
                    db.Entry(ex).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
        public IEnumerable<ValidationResult> Before_Destroy(ValidationContext validationContext)
        {
            var Exams = db.EXAMs.Where(x => x.EXAM_GRP_ID != this.ID);
            if (Exams == null || Exams.Count() == 0)
            {
                ErrorMessage = "Canot remove this exam group as there is no other Exam Group to assign exams to.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "ID" });

            }
        }
        public bool Removable()
        {
            var Exams = db.EXAMs.Where(x => x.EXAM_GRP_ID != this.ID);
            if (Exams == null || Exams.Count() == 0)
            {
                return false;

            }
            else
            {
                return true;
            }
        }
        public decimal? Batch_Average_Marks(string marks)
        {
            BATCH batch = db.BATCHes.Find(this.BTCH_ID);
            var exams = db.EXAMs.Where(x => x.EXAM_GRP_ID == this.ID);
            var batch_students = batch.All_Students();
            var total_students_marks = 0;
            //var total_max_marks = 0
            var batch_average_marks = 0;
            List<int?> students_attended = new List<int?>();
            foreach(var exam in exams)
            {
                foreach(var student in batch_students)
                {
                    EXAM_SCORE exam_score = db.EXAM_SCORE.Where(x => x.EXAM_ID == exam.ID && x.STDNT_ID == student.ID).FirstOrDefault();
                    if(exam_score != null)
                    {
                        if(exam_score.MKS != null)
                        {
                            total_students_marks = total_students_marks + (int)exam_score.MKS;
                            if(!students_attended.Contains(student.ID))
                            {
                                students_attended.Add(student.ID);
                            }
                        }
                    }
                }
                //total_max_marks = total_max_marks+exam.maximum_marks
            }
            if(students_attended.Count() != 0)
            {
                batch_average_marks = total_students_marks / students_attended.Count();
            }
            if(marks == "marks")
            {
                return batch_average_marks;
            }
            //else if (marks == "percentage")
            //{
            // return total_max_marks;
            // }
            else { return null; }
        }
        public decimal Weightage
        {
            get {
                GROUPED_EXAM grp = db.GROUPED_EXAM.Where(x => x.BTCH_ID == this.BTCH_ID && x.EXAM_GROUP_ID == this.ID).FirstOrDefault();
                decimal weight = 0;
                if (grp != null)
                {
                    weight = (decimal)grp.WTAGE;
                }
                return weight;
            }
        }
        public decimal? Archived_Batch_Average_Marks(string marks)
        {
            BATCH batch = db.BATCHes.Find(this.BTCH_ID);
            var exams = db.EXAMs.Where(x => x.EXAM_GRP_ID == this.ID);
            var batch_students = batch.All_Students();
            var total_students_marks = 0;
            //var total_max_marks = 0
            var batch_average_marks = 0;
            List<int?> students_attended = new List<int?>();
            foreach (var exam in exams)
            {
                foreach (var student in batch_students)
                {
                    ARCHIVED_EXAM_SCORE exam_score = db.ARCHIVED_EXAM_SCORE.Where(x => x.EXAM_ID == exam.ID && x.STDNT_ID == student.ID).FirstOrDefault();
                    if (exam_score != null)
                    {
                        if (exam_score.MKS != null)
                        {
                            total_students_marks = total_students_marks + (int)exam_score.MKS;
                            if (!students_attended.Contains(student.ID))
                            {
                                students_attended.Add(student.ID);
                            }
                        }
                    }
                }
                //total_max_marks = total_max_marks+exam.maximum_marks
            }
            if (students_attended.Count() != 0)
            {
                batch_average_marks = total_students_marks / students_attended.Count();
            }
            if (marks == "marks")
            {
                return batch_average_marks;
            }
            //else if (marks == "percentage")
            //{
            // return total_max_marks;
            // }
            else { return null; }
        }

        public decimal? Subject_Wise_Batch_Average_Marks(int? subject_id)
        {
            BATCH batch = db.BATCHes.Find(this.BTCH_ID);
            SUBJECT subject = db.SUBJECTs.Find(subject_id);
            EXAM exam = db.EXAMs.Where(x => x.EXAM_GRP_ID == this.ID && x.SUBJ_ID == subject.ID).FirstOrDefault();
            var batch_students = batch.All_Students();
            var total_students_marks = 0;
            //var total_max_marks = 0
            var subject_wise_batch_average_marks = 0;
            List<int?> students_attended = new List<int?>();
            foreach (var student in batch_students)
            {
                ARCHIVED_EXAM_SCORE exam_score = db.ARCHIVED_EXAM_SCORE.Where(x => x.EXAM_ID == exam.ID && x.STDNT_ID == student.ID).FirstOrDefault();
                if (exam_score != null)
                {
                    if (exam_score.MKS != null)
                    {
                        total_students_marks = total_students_marks + (int)exam_score.MKS;
                        if (!students_attended.Contains(student.ID))
                        {
                            students_attended.Add(student.ID);
                        }
                    }
                }
            }
            //total_max_marks = total_max_marks+exam.maximum_marks
            if (students_attended.Count() != 0)
            {
                subject_wise_batch_average_marks = total_students_marks / students_attended.Count();
            }
            return subject_wise_batch_average_marks;
            //if (marks == "percentage")
            //{
            // return total_max_marks;
            // }
        }
        public List<int> Total_Marks(STUDENT student)
        {
            var exams = db.EXAMs.Where(x => x.EXAM_GRP_ID == this.ID).ToList();
            int total_marks = 0;
            int max_total = 0;
            foreach(var exam in exams)
            {
                EXAM_SCORE exam_score = db.EXAM_SCORE.Where(x => x.EXAM_ID == exam.ID && x.STDNT_ID == student.ID).FirstOrDefault();
                if(exam_score != null)
                {
                    total_marks = total_marks + (int)(exam_score.MKS == null? 0 : exam_score.MKS);
                    max_total = max_total + (int)exam.MAX_MKS;
                }
            }
            List<int> result = new List<int>();
            result.Add(total_marks);
            result.Add(max_total);
            return result;
        }
        public List<int> Archived_Total_Marks(STUDENT student)
        {
            var exams = db.EXAMs.Where(x => x.EXAM_GRP_ID == this.ID).ToList();
            int total_marks = 0;
            int max_total = 0;
            foreach (var exam in exams)
            {
                ARCHIVED_EXAM_SCORE exam_score = db.ARCHIVED_EXAM_SCORE.Where(x => x.EXAM_ID == exam.ID && x.STDNT_ID == student.ID).FirstOrDefault();
                if (exam_score != null)
                {
                    total_marks = total_marks + (int)(exam_score.MKS == null ? 0 : exam_score.MKS);
                    max_total = max_total + (int)exam.MAX_MKS;
                }
            }
            List<int> result = new List<int>();
            result.Add(total_marks);
            result.Add(max_total);
            return result;
        }
        public COURSE course
        {          
            get {
                BATCH batch = db.BATCHes.Find(this.BTCH_ID);
                COURSE course= db.COURSEs.Find(batch.ID);
                return batch != null ? course : null; 
            }
        }
    }
}