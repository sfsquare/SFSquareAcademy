using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SFSAcademy
{
    public class ExamDetails
    {
        public int? Subject_Id { get; set; }
        public int? Exam_Group_Id { get; set; }
        public int? Exam_Id { get; set; }
        public string SUBJ_NAME { get; set; }
        public DateTime? Start_Time { get; set; }
        public DateTime? End_Time { get; set; }
        public int? Maximum_Marks { get; set; }
        public int? Minimum_Marks { get; set; }
        public bool? Deleted { get; set; }
        public SUBJECT SubjectData { get; set; }
        public EXAM ExamData { get; set; }
    }
    [MetadataType(typeof(ExamMetadata))]
    public partial class EXAM : IValidatableObject, IHasTimeStamp, IHasBeforeSave, IHasBeforeDestroy, IHasAfterCreate, IHasAfterUpdate
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class ExamMetadata
        {
            [Required]
            public DateTime START_TIME { get; set; }

            [Required]
            public DateTime END_TIME { get; set; }
        }
        private string ErrorMessage { get; set; }
        private string EntityStateValue { get; set; }
        public void DoTimeStamp(string EntityStateVal)
        {

            if (EntityStateVal.Equals("Added"))
            {
                CREATED_AT = DateTime.Now;
                UPDATED_AT = DateTime.Now;
            }

            if (EntityStateVal.Equals("Modified"))
            {
                UPDATED_AT = DateTime.Now;
            }
            EntityStateValue = EntityStateVal;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Validation_Should_Present())
            {
                if (EntityStateValue.Equals("Modified"))
                {
                    if (MIN_MKS == null)
                    {
                        ErrorMessage = "Minimum Marks is mandatory.";
                        yield return new ValidationResult($"* {ErrorMessage}.", new[] { "MIN_MKS" });

                    }
                    if (MAX_MKS == null)
                    {
                        ErrorMessage = "Maximum Marks is mandatory.";
                        yield return new ValidationResult($"* {ErrorMessage}.", new[] { "MAX_MKS" });

                    }
                }
            }
            if(MAX_MKS != null && MIN_MKS != null && MIN_MKS > MAX_MKS)
            {
                ErrorMessage = "Minimum marks can't be more than max marks.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "MIN_MKS" });
            }
            if(!(this.START_TIME == null || this.END_TIME == null))
            {
                if(this.END_TIME < this.START_TIME)
                {
                    ErrorMessage = "End time cannot be before the start time";
                    yield return new ValidationResult($"* {ErrorMessage}.", new[] { "END_TIME" });
                }
            }
        }
        public void Before_Save()
        {
            if(this.WTAGE == null)
            {
                this.WTAGE = 0;
            }
            Update_Exam_Group_Date();
        }
        public IEnumerable<ValidationResult> Before_Destroy(ValidationContext validationContext)
        {
            if (Removable() == false)
            {
                ErrorMessage = "Canot remove this exam as marks or grading level are assigned to this.";
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "ID" });

            }
        }
        public void After_Create()
        {
            //if(this.CREATED_AT >= DateTime.Now.AddSeconds(-20))
            //{
              //  Create_Exam_Event();
            //}
        }
        public void After_Update()
        {
            if (this.UPDATED_AT >= DateTime.Now.AddSeconds(-20) && this.CREATED_AT <= DateTime.Now.AddSeconds(-20))
            {
                Update_Exam_Event();
            }
        }
        public bool Validation_Should_Present()
        {
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(this.EXAM_GRP_ID);
            if(exam_group.EXAM_TYPE == "Grades")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public SUBJECT Subject
        {
            get { return this.SUBJECT != null?(this.SUBJECT.IS_DEL == false? this.SUBJECT : null) : null; }
        }
        public bool Removable()
        {
            var ExamScores = db.EXAM_SCORE.Where(x => x.EXAM_ID == this.ID && (x.MKS != null || x.GRADING_LVL_ID != null)).ToList();
            if (ExamScores == null || ExamScores.Count() == 0)
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        public EXAM_SCORE Score_For(int? student_id)
        {
            EXAM_SCORE exam_score = db.EXAM_SCORE.Include(x=>x.GRADING_LEVEL).Where(x => x.EXAM_ID == this.ID && x.STDNT_ID == student_id).FirstOrDefault();
            EXAM_SCORE new_score = new EXAM_SCORE() { EXAM_ID = this.ID, STDNT_ID = student_id, GRADING_LVL_ID = this.GRADING_LVL_ID };
            return exam_score == null ? new_score : exam_score;
        }
        public double Class_Average_Marks()
        {
            var results = db.EXAM_SCORE.Where(x => x.EXAM_ID == this.ID).ToList();
            List<int?> scores = new List<int?>();
            foreach(var x in results)
            {
                if(x.MKS != null)
                {
                    scores.Add(x.MKS);
                }
            }
            return scores.Count() == 0 ? 0 : (double)(scores.Sum() / scores.Count());
        }
        public IEnumerable<FA_GROUP> FA_Groups()
        {
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(this.ID);
            SUBJECT sub = db.SUBJECTs.Find(this.SUBJ_ID);
            //FA_GROUP fa_group = db.FA_GROUP.Where(x=>x.sub)
            return sub.FA_GROUP.Where(x => x.CCE_EXAM_CAT_ID == exam_group.CCE_EXAM_CAT_ID).ToList();
        }

        private void Update_Exam_Group_Date()
        {
            var group = db.EXAM_GROUP.Find(this.EXAM_GRP_ID);
            if (group.EXAM_DATE != null )
            {
                if(this.START_TIME.Value.Date < group.EXAM_DATE)
                {
                    group.EXAM_DATE = this.START_TIME.Value.Date;
                }
            }

        }
        public void Create_Exam_Event()
        {
            EXAM UpdatedExam = db.EXAMs.Find(this.ID);
            EVENT Ev = db.EVENTs.Find(UpdatedExam.EV_ID);
            if(Ev == null)
            {
                SUBJECT sub = db.SUBJECTs.Include(x => x.BATCH).Where(x => x.ID == UpdatedExam.SUBJ_ID).FirstOrDefault();
                BATCH batch = db.SUBJECTs.Include(x => x.BATCH).Where(x => x.ID == UpdatedExam.SUBJ_ID).Select(x => x.BATCH).FirstOrDefault();
                EVENT new_event = new EVENT()
                {
                    TTIL = "Exam",
                    DESCR = string.Concat(UpdatedExam.EXAM_GROUP.NAME, " for ", batch.Full_Name, "-", sub.NAME),
                    START_DATE = this.START_TIME,
                    END_DATE = this.END_TIME,
                    IS_EXAM = true,
                    ORIGIN_ID = this.ID,
                    ORIGIN_TYPE = "Exam"
                };
                db.EVENTs.Add(new_event);
                db.SaveChanges();
                BATCH_EVENT batch_event = new BATCH_EVENT()
                {
                    EV_ID = new_event.ID,
                    BTCH_ID = batch.ID
                };
                db.BATCH_EVENT.Add(batch_event);
                UpdatedExam.EV_ID = new_event.ID;
                db.Entry(UpdatedExam).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public void Update_Exam_Event()
        {
            if(this.EVENT != null)
            {
                EVENT ev = db.EVENTs.Find(this.EV_ID);
                ev.START_DATE = this.START_TIME;
                ev.END_DATE = this.END_TIME;
                db.Entry(ev).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

    }
}