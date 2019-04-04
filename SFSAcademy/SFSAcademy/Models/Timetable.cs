using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SFSAcademy.Models
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

}