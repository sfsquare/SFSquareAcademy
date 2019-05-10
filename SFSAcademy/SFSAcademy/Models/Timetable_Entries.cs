using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFSAcademy
{
    public class Timetable_Entries
    {
        public CLASS_TIMING ClassTimingData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public WEEKDAY WeekdayData { get; set; }
        public TIMETABLE_ENTRY TimetableEntryData { get; set; }
        public TIMETABLE TimetableData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
        public SUBJECT SubjectData { get; set; }
    }

    public class Timetable_Entries_Select
    {
        public CLASS_TIMING ClassTimingData { get; set; }
        public BATCH BatchData { get; set; }
        public WEEKDAY WeekdayData { get; set; }
        public TIMETABLE_ENTRY TimetableEntryData { get; set; }
        public TIMETABLE TimetableData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
        public SUBJECT SubjectData { get; set; }
        public bool Select { get; set; }
    }
}