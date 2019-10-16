using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace SFSAcademy
{
    public partial class ARCHIVED_STUDENT : IHasTimeStamp, IHasBeforeSave
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
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
        public void Before_Save()
        {
            if(IS_ACT != false)
            {
                IS_ACT = false;
            }
        }
        public string gender_as_text
        {
            get { return this.GNDR == "M"? "Male": "Female"; }
        }

        public string First_And_Last_Name
        {
            get { return String.Concat(this.FIRST_NAME, " ", this.LAST_NAME); }
        }
        public string Full_Name
        {
            get { return String.Concat(this.FIRST_NAME, " ", this.MID_NAME, " ", this.LAST_NAME); }
        }
        public ARCHIVED_GUARDIAN Immediate_Contact()
        {
            if(this.IMMDT_CNTCT_ID != null)
            {
                return db.ARCHIVED_GUARDIAN.Find(this.IMMDT_CNTCT_ID);
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<BATCH> Graduated_Batches()
        {
            //user FormerId to map to the student details
            var GrBat = (from bt in db.BATCHes
                         join btst in db.BATCH_STUDENT on bt.ID equals btst.BTCH_ID
                         where btst.STDNT_ID == this.FRM_ID
                         select bt).OrderBy(x=>x.ID).AsEnumerable();
            return GrBat;
        }
        public IEnumerable<BATCH> All_Batches()
        {
            return db.BATCHes.Where(x => x.ID == this.BTCH_ID).Union(Graduated_Batches());
        }
        public STUDENT_ADDITIONAL_DETAIL Additional_Detail(int? additional_field)
        {
            return db.STUDENT_ADDITIONAL_DETAIL.Where(x=>x.ADDL_FLD_ID == additional_field && x.STDNT_ID == this.FRM_ID).FirstOrDefault();
        }
        public bool Has_retaken_exam(int? subject_id)
        {
            var retaken_exams = db.ARCHIVED_EXAM_SCORE.Where(x => x.STDNT_ID == this.FRM_ID).ToList();
            if(retaken_exams == null && retaken_exams.Count() == 0)
            {
                return false;
            }
            else
            {
                var exams = db.EXAMs.Where(x => retaken_exams.Select(p => p.EXAM_ID).Contains(x.ID)).ToList();
                if(exams.Where(x=>x.SUBJ_ID == subject_id).ToList() != null && exams.Where(x => x.SUBJ_ID == subject_id).ToList().Count() != 0)
                {
                    return true;
                }
                return false;
            }          
        }
    }
}