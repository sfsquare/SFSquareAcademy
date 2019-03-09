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

    public class subject
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        public EMPLOYEE Lower_Week_Grade(SUBJECT sub)
        {
            EMPLOYEE selected_employee = null;
            if (sub.ELECTIVE_GRP_ID != null)
            {
                var subjects = db.SUBJECTs.Where(x => x.ELECTIVE_GRP_ID == sub.ELECTIVE_GRP_ID).ToList();
                foreach (var item in subjects)
                {
                    var employees = (from emp in db.EMPLOYEEs
                                     join emp_sub in db.EMPLOYEES_SUBJECT on emp.ID equals emp_sub.EMP_ID
                                     where emp_sub.SUBJ_ID == item.ID
                                     select new { emp }).ToList();
                    foreach(var item2 in employees)
                    {
                        if(selected_employee == null)
                        {
                            selected_employee = item2.emp;
                        }
                        else
                        {
                            if(item2.emp.EMPLOYEE_GRADE.MAX_WKILY_HRS < selected_employee.EMPLOYEE_GRADE.MAX_WKILY_HRS)
                            {
                                selected_employee = item2.emp;
                            }
                        }
                    }
                }
            }            
           return selected_employee;
        }

        public EMPLOYEE Lower_Day_Grade(SUBJECT sub)
        {
            EMPLOYEE selected_employee = null;
            if (sub.ELECTIVE_GRP_ID != null)
            {
                var subjects = db.SUBJECTs.Where(x => x.ELECTIVE_GRP_ID == sub.ELECTIVE_GRP_ID).ToList();
                foreach (var item in subjects)
                {
                    var employees = (from emp in db.EMPLOYEEs
                                     join emp_sub in db.EMPLOYEES_SUBJECT on emp.ID equals emp_sub.EMP_ID
                                     where emp_sub.SUBJ_ID == item.ID
                                     select new { emp }).ToList();
                    foreach (var item2 in employees)
                    {
                        if (selected_employee == null)
                        {
                            selected_employee = item2.emp;
                        }
                        else
                        {
                            if (item2.emp.EMPLOYEE_GRADE.MAX_DILY_HRS < selected_employee.EMPLOYEE_GRADE.MAX_DILY_HRS)
                            {
                                selected_employee = item2.emp;
                            }
                        }
                    }
                }
            }
            return selected_employee;
        }

    }
}
    