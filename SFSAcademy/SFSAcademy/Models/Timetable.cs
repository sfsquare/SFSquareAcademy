using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFSAcademy.Models
{
    public class WeekdaysSelect
    {
        public string Day { get; set; }
        public int Id { set; get; }
        public bool Select { get; set; }
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

    public class EmployeeWorkAllotment
    {
        public EMPLOYEE EmployeeData { get; set; }
        public decimal? Total_Time { get; set; }
    }

}