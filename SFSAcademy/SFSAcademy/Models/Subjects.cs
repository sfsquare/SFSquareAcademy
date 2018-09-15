using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFSAcademy.Models
{
    public class ElectiveGroups
    {
        public ELECTIVE_GROUP ElectiveGroupData { get; set; }
        public SUBJECT SubjectData { get; set; }
        public BATCH BatchData { get; set; }
    }

    public class TimetableEntry
    {
        public TIMETABLE_ENTRY TimeTableEntryData { get; set; }
        public STUDENT_SUBJECT StudentSubjectData { get; set; }
        public SUBJECT SubjectData { get; set; }
        public BATCH BatchData { get; set; }
    }
}