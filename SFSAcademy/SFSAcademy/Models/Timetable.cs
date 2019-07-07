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

        public List<DateTime> Register_Range(BATCH batch, DateTime date)
        {
            List<DateTime> start = new List<DateTime>();
            start.Add((DateTime)batch.START_DATE);
            DateTime BeginDate = new DateTime(date.Year, date.Month, 1);
            start.Add(BeginDate);
            DateTime ttSTartDate = (DateTime)db.TIMETABLEs.Select(x => x.START_DATE).ToList().Min();
            start.Add(ttSTartDate);

            List<DateTime> stop = new List<DateTime>();
            stop.Add((DateTime)batch.END_DATE);
            DateTime EndDate = BeginDate.AddMonths(1).AddDays(-1);
            stop.Add(EndDate);
            DateTime ttEndDate = (DateTime)db.TIMETABLEs.Select(x => x.END_DATE).ToList().Max();
            stop.Add(ttEndDate);

            List<DateTime> holidays = batch.Holiday_Event_Dates();
            DateTime MaxDate = (DateTime)start.Max();
            DateTime MinDate = (DateTime)stop.Min();

            List<DateTime> range = new List<DateTime>();
            for (var dt = MaxDate; dt <= MinDate; dt = dt.AddDays(1))
            {
                bool Select = true;
                foreach (var hld in holidays)
                {
                    if (dt == hld.Date)
                    {
                        Select = false;
                        break; ;
                    }
                }
                if (Select == true)
                {
                    range.Add(dt);
                }
            }
            return range;
        }
        public IEnumerable<TIMETABLE_ENTRY> tte_for_the_day(BATCH batch, DateTime? date)
        {
            var entries = db.TIMETABLE_ENTRY.Include(x => x.TIMETABLE).Include(x => x.CLASS_TIMING).Include(x => x.WEEKDAY).Include(x => x.SUBJECT).Include(x => x.SUBJECT.ELECTIVE_GROUP).Include(x => x.EMPLOYEE).Where(x => x.TIMETABLE.START_DATE <= date && x.TIMETABLE.END_DATE >= date && x.BTCH_ID == batch.ID).OrderBy(x => x.CLASS_TIMING.START_TIME).ToList();
            var today = db.TIMETABLE_ENTRY.Where(x => x.ID == -1).ToList().DefaultIfEmpty();
            if(entries != null || entries.Count() != 0)
            {
                today = entries.Where(x => x.WEEKDAY.DAY_OF_WK == (int)date.Value.DayOfWeek).ToList();
            }
            return (IEnumerable<TIMETABLE_ENTRY>)today;
        }

        //this is Partially done as currently Timetable Entries in our system is not stored at day level so removing holidays from the list needs to be implemented later on.
        public List<DateTime> tte_for_range(BATCH batch, DateTime date, SUBJECT subject)
        {
            if(subject.ELECTIVE_GRP_ID != null)
            {
                subject = subject.ELECTIVE_GROUP.SUBJECTs.FirstOrDefault();
            }
            var range = Register_Range(batch, date);
            DateTime? RangeFirst = range.First();
            DateTime? RangeLast = range.Last();
            var holidays = batch.Holiday_Event_Dates();

            var entries = db.TIMETABLE_ENTRY.Include(x => x.TIMETABLE).Include(x => x.CLASS_TIMING).Include(x => x.WEEKDAY).Include(x => x.SUBJECT).Include(x => x.SUBJECT.ELECTIVE_GROUP).Include(x => x.EMPLOYEE).Where(x => ((x.TIMETABLE.START_DATE <= RangeFirst && x.TIMETABLE.END_DATE >= RangeFirst) || (x.TIMETABLE.START_DATE <= RangeLast && x.TIMETABLE.END_DATE >= RangeLast) || (RangeFirst <= x.TIMETABLE.START_DATE && RangeLast >= x.TIMETABLE.START_DATE) || (RangeFirst <= x.TIMETABLE.END_DATE && RangeLast >= x.TIMETABLE.END_DATE)) && x.SUBJ_ID == subject.ID && x.BTCH_ID == batch.ID && x.CLASS_TIMING.IS_DEL == false && x.WEEKDAY.IS_DEL == false).OrderBy(x => x.TIMT_ID).ToList();
            List<int?> timetable_ids = new List<int?>();
            timetable_ids = entries.Select(x => x.TIMT_ID).Distinct().ToList();
            var timetables = db.TIMETABLEs.Where(x => timetable_ids.Contains(x.ID)).ToList();
            List<DateTime> hsh2 = new List<DateTime>();
            foreach (var tt in timetables)
            {
                List<DateTime> MaxDate = new List<DateTime>();
                MaxDate.Add((DateTime)tt.START_DATE);
                MaxDate.Add((DateTime)range.First());

                List<DateTime> MinDate = new List<DateTime>();
                MinDate.Add((DateTime)tt.END_DATE);
                MinDate.Add((DateTime)range.Last());

                for (var dt = MaxDate.Max(); dt <= MinDate.Min(); dt = dt.AddDays(1))
                {
                    bool Select = false;
                    foreach (var WkD in entries.Where(x=>x.TIMT_ID == tt.ID))
                    {
                        if ((int)dt.DayOfWeek == WkD.WEEKDAY.DAY_OF_WK)
                        {
                            Select = true;
                            break; ;
                        }
                    }
                    foreach (var hld in holidays)
                    {
                        if (dt == hld.Date)
                        {
                            Select = false;
                            break; ;
                        }
                    }
                    if (Select == true)
                    {
                        hsh2.Add(dt);
                    }
                }
            }
            return hsh2;
        }
    }

}