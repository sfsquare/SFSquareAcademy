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
    public class WeekdaysSelect
    {
        public string Day { get; set; }
        public int Id { set; get; }
        public bool Select { get; set; }
        public string ClassTimingSet { get; set; }
    }

    public class Weekdays
    {
        public int DayId { get; set; }
        public List<WeekdaysSelect> WeekdayIds { get; set; }
        //Other Properties also her

        public Weekdays()
        {
            WeekdayIds = new List<WeekdaysSelect>();
        }

    }

    public class BatchWeekdaysSelect
    {
        public string Day { get; set; }
        public int Id { set; get; }
        public bool Select { get; set; }
        public string ClassTimingSet { get; set; }
    }

    public class EmployeeWorkAllotment
    {
        public EMPLOYEE EmployeeData { get; set; }
        public EMPLOYEE_DEPARTMENT DepartmentData { get; set; }
        public decimal? Total_Time { get; set; }
    }

    public class ClassTimings
    {
        public string NAME { get; set; }
        [Required(ErrorMessage = "Start Time is required")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public Nullable<System.TimeSpan> StartTimeVal { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}")]
        public DateTime? START_TIME { get { return (StartTimeVal.HasValue) ? (DateTime?)DateTime.Today.Add(StartTimeVal.Value) : null; } }

        [Required(ErrorMessage = "End Time is required")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public Nullable<System.TimeSpan> EndTimeVal { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}")]
        public DateTime? END_TIME { get { return (EndTimeVal.HasValue) ? (DateTime?)DateTime.Today.Add(EndTimeVal.Value) : null; } }

        public bool IS_BRK { get; set; }
    }

    public partial class TIMETABLE
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        public IEnumerable<TIMETABLE_ENTRY> tte_for_the_day(BATCH batch, DateTime? date)
        {
            var entries = db.TIMETABLE_ENTRY.Include(x => x.TIMETABLE).Include(x => x.CLASS_TIMING).Include(x => x.WEEKDAY).Include(x => x.SUBJECT).Include(x => x.SUBJECT.ELECTIVE_GROUP).Include(x => x.EMPLOYEE).Where(x => x.TIMETABLE.START_DATE <= date && x.TIMETABLE.END_DATE >= date && x.BTCH_ID == batch.ID).OrderBy(x => x.CLASS_TIMING.START_TIME).ToList();
            var today = db.TIMETABLE_ENTRY.Where(x => x.ID == -1).ToList().DefaultIfEmpty();
            if(entries != null || entries.Count() != 0)
            {
                today = entries.Where(x => x.WEEKDAY.DAY_OF_WK.ToString() == date.Value.DayOfWeek.ToString()).ToList();
            }
            return (IEnumerable<TIMETABLE_ENTRY>)today;
        }
    }

}