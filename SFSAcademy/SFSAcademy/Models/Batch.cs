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
    public partial class BATCH
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        public IEnumerable<BATCH> ACTIVE()
        {
            var ActiveBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.COURSE.CODE).OrderBy(x => x.NAME);
            return (IEnumerable<BATCH>)ActiveBatch;
        }

        public IEnumerable<BATCH> INACTIVE()
        {
            var ActiveBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.IS_DEL == false && x.IS_ACT == false).OrderBy(x => x.COURSE.CODE).OrderBy(x => x.NAME);
            return (IEnumerable<BATCH>)ActiveBatch;
        }

        public IEnumerable<BATCH> DELETED()
        {
            var ActiveBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.IS_DEL == true).OrderBy(x => x.COURSE.CODE).OrderBy(x => x.NAME);
            return (IEnumerable<BATCH>)ActiveBatch;
        }

        public IEnumerable<BATCH> CCE()
        {
            var ActiveBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.COURSE.GRADING_TYPE.Contains("CCE")).OrderBy(x => x.COURSE.CODE).OrderBy(x => x.NAME);
            return (IEnumerable<BATCH>)ActiveBatch;
        }
        public string Course_full_name
        {
            get { return COURSE.CODE + "-" + NAME; }
        }
        public string Validate()
        {
            string Message = "";
            if(START_DATE == null || END_DATE == null)
            {
                Message = "Start Date or End Date canot be null. ";
            }
            if (START_DATE > END_DATE)
            {
                Message = string.Concat(Message, START_DATE, " should be before End Date");
            }
            return Message;
        }

    }
}