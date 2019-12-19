using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SFSAcademy
{
    [MetadataType(typeof(EventMetadata))]
    public partial class EVENT : IValidatableObject, IHasTimeStamp
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class EventMetadata
        {
            [Required]
            public string TTIL { get; set; }

            [Required]
            public string DESCR { get; set; }
            [Required]
            public DateTime START_DATE { get; set; }

            [Required]
            public DateTime END_DATE { get; set; }
        }
        private string ErrorMessage { get; set; }
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
            if (!(this.START_DATE == null || this.END_DATE == null))
            {
                if (this.END_DATE < this.START_DATE)
                {
                    ErrorMessage = "End time cannot be before the start time";
                    yield return new ValidationResult($"* {ErrorMessage}.", new[] { "END_TIME" });
                }
            }
        }

        public IEnumerable<EVENT> Holidays
        {
            get
            {                
                return db.EVENTs.Where(x=>x.IS_HOL == true).ToList();
            }
        }
        public IEnumerable<EVENT> Exams
        {
            get
            {
                return db.EVENTs.Where(x => x.IS_EXAM == true).ToList();
            }
        }
        public bool Is_Student_Event(STUDENT student)
        {
            bool flag = false;
            var baseval = this.ORIGIN_TYPE;
            if(baseval != null)
            {
                if(baseval == "Finance_Fee_Collection")
                {
                    FINANCE_FEE_COLLECTION fcol = db.FINANCE_FEE_COLLECTION.Find(this.ORIGIN_ID);
                    if(fcol.BTCH_ID == student.BTCH_ID)
                    {
                        var finance = db.FINANCE_FEE.Where(x => x.FEE_CLCT_ID == this.ORIGIN_ID).ToList();
                        if(finance != null && finance.Count() != 0)
                        {
                            if(finance.Select(x=>x.STDNT_ID).Contains(student.ID))
                            {
                                flag = true;
                            }
                        }
                    }
                }
            }
            var user_events = db.USERS_EVENT.Where(x => x.EV_ID == this.ID).ToList();
            if(user_events != null && user_events.Count() != 0)
            {
                if(user_events.Select(x=>x.USRID).Contains(student.USRID))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public bool Is_Employee_Event(USER user)
        {
            bool flag = false;
            var user_events = db.USERS_EVENT.Where(x => x.EV_ID == this.ID).ToList();
            if (user_events != null && user_events.Count() != 0)
            {
                if (user_events.Select(x => x.USRID).Contains(user.ID))
                {
                    flag = true;
                }
            }
            return flag;
        }
        public bool Is_Active_Event()
        {
            bool flag = false;
            var baseval = this.ORIGIN_TYPE;
            if (baseval != null)
            {
                if (baseval == "Finance_Fee_Collection")
                {
                    FINANCE_FEE_COLLECTION fcol = db.FINANCE_FEE_COLLECTION.Find(this.ORIGIN_ID);
                    if (fcol.IS_DEL != true)
                    {
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            return flag;
        }
        public List<DateTime?> Dates
        {
            get {
                List<DateTime?> datesval = new List<DateTime?>();
                for(DateTime? d = this.START_DATE; d<= this.END_DATE; d.Value.AddDays(1) )
                {
                    datesval.Add(d);
                }               
                return datesval; }
        }
        public bool Is_A_Holiday(DateTime day)
        {
            
            if(db.EVENTs.FirstOrDefault().Holidays.Where(x=>x.START_DATE<= day && x.END_DATE >= day).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}