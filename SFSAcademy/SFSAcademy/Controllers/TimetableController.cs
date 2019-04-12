using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFSAcademy;

namespace SFSAcademy.Controllers
{
    public class TimetableController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Timetable
        public ActionResult Index(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            var config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "AvailableModules").ToList();
            ViewData["config"] = config;
            var Config_Val = new Models.Configuration();
            ViewBag.StudentAttendanceType = Config_Val.find_by_config_key("StudentAttendanceType");
            return View();
        }

        // GET: Timetable/Create
        public ActionResult Work_Allotment(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var admin = db.EMPLOYEE_CATEGORY.Where(x => !x.PRFX.Contains("admin")).ToList();
            ViewData["admin"] = admin;

            var employees = (from emp in db.EMPLOYEEs.Include(x=>x.EMPLOYEE_GRADE).Include(x=>x.EMPLOYEES_SUBJECT).Include(x=>x.EMPLOYEE_DEPARTMENT)
                             join empcat in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals empcat.ID
                             where !empcat.PRFX.Contains("admin")
                              select new EmployeeWorkAllotment { EmployeeData = emp, Total_Time = emp.EMPLOYEE_GRADE.MAX_WKILY_HRS}).ToList();
            ViewData["employees"] = employees;

            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var batches = (from cs in db.COURSEs
                           join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
                           where bt.END_DATE >= StartDate
                           select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs, Total_Time = 0 }).Distinct().ToList();
            ViewData["batches"] = batches;

            return View();
        }

        public ActionResult Manage_Allotment(int? batch_id, int? sub_id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.END_DATE >= StartDate
                                    select new Models.CoursesBatch { CourseData = cs, BatchData = bt })
                        .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID == batch_id? true: false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.BTCH_ID = options;
            BATCH batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == batch_id).FirstOrDefault();
            ViewData["batch"] = batch;

            List<SelectListItem> options2 = new SelectList(db.SUBJECTs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).OrderBy(c => c.NAME), "ID", "NAME", sub_id).ToList();
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "Select a Subject" });
            ViewBag.SUB_ID = options2;

            SUBJECT subject = db.SUBJECTs.Find(sub_id);
            ViewData["subject"] = subject;
            var assigned_employee = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == sub_id).ToList();
            ViewData["assigned_employee"] = assigned_employee;
            var departments = db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).ToList();
            ViewData["departments"] = departments;
            var Employee = db.EMPLOYEEs.ToList();
            ViewData["Employee"] = Employee;
            List<SelectListItem> options3 = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).OrderBy(c => c.NAMES), "ID", "NAMES").ToList();
            // add the 'ALL' option
            options3.Insert(0, new SelectListItem() { Value = "-1", Text = "Select a Department" });
            ViewBag.DEPT_ID = options3;

            return View();
        }

        public ActionResult Subject_Select(int? batch_id)
        {
            BATCH batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == batch_id).FirstOrDefault();
            ViewData["batch"] = batch;
            List<SelectListItem> options = new SelectList(db.SUBJECTs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).OrderBy(c => c.NAME), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select a Subject" });
            ViewBag.SUB_ID = options;

            return PartialView("_Subject_Select");
        }

        public ActionResult Department_Select(int? subject_id, int? batch_id)
        {
            SUBJECT subject = db.SUBJECTs.Find(subject_id);
            ViewData["subject"] = subject;
            var assigned_employee = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == subject_id).ToList();
            ViewData["assigned_employee"] = assigned_employee;
            var departments = db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).ToList();
            ViewData["departments"] = departments;
            var Employee = db.EMPLOYEEs.ToList();
            ViewData["Employee"] = Employee;
            BATCH batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == batch_id).FirstOrDefault();
            ViewData["batch"] = batch;
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).OrderBy(c => c.NAMES), "ID", "NAMES").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select a Department" });
            ViewBag.DEPT_ID = options;

            return PartialView("_Department_Select");
        }

        public ActionResult Update_Employees(int? department_id, int? subject_id, int? batch_id)
        {
            SUBJECT subject = db.SUBJECTs.Find(subject_id);
            ViewData["subject"] = subject;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == department_id && x.STAT == true).ToList();
            ViewData["employees"] = employees;
            var EmployeesSubject = db.EMPLOYEES_SUBJECT.ToList();
            ViewData["EmployeesSubject"] = EmployeesSubject;
            BATCH batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == batch_id).FirstOrDefault();
            ViewData["batch"] = batch;
            return PartialView("_Employee_List");
        }

        public ActionResult Assign_Employee(int? id, int? id1, int? batch_id)
        {
            var departments = db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).ToList();
            ViewData["departments"] = departments;
            SUBJECT subject = db.SUBJECTs.Find(id1);
            ViewData["subject"] = subject;
            var employee_department_id = db.EMPLOYEEs.Find(id).EMP_DEPT_ID;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == employee_department_id && x.STAT == true).ToList();
            ViewData["employees"] = employees;
            EMPLOYEES_SUBJECT EmployeesSubject = new EMPLOYEES_SUBJECT { EMP_ID = id, SUBJ_ID = id1 };
            db.EMPLOYEES_SUBJECT.Add(EmployeesSubject);
            db.SaveChanges();
            var assigned_employee = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == subject.ID).ToList();
            ViewData["assigned_employee"] = assigned_employee;
            ViewBag.Notice = "Employee Successfully assigned.";
            return RedirectToAction("Manage_Allotment", new { batch_id = batch_id, sub_id = id1, Notice = ViewBag.Notice });
        }

        public ActionResult Remove_Employee(int? id, int? id1, int? batch_id)
        {
            var departments = db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).ToList();
            ViewData["departments"] = departments;
            SUBJECT subject = db.SUBJECTs.Find(id1);
            ViewData["subject"] = subject;
            var employee_department_id = db.EMPLOYEEs.Find(id).EMP_DEPT_ID;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == employee_department_id && x.STAT == true).ToList();
            ViewData["employees"] = employees;
            var TimetableEntry = db.TIMETABLE_ENTRY.Where(x => x.SUBJ_ID == subject.ID && x.EMP_ID == id).ToList();
            if (TimetableEntry == null || TimetableEntry.Count() == 0)
            {
                EMPLOYEES_SUBJECT EmployeesSubject = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == id && x.SUBJ_ID == id1).FirstOrDefault();
                db.EMPLOYEES_SUBJECT.Remove(EmployeesSubject);
                db.SaveChanges();
                ViewBag.Notice = "Employee sucessfully removed.";
            }
            else
            {
                ViewBag.Notice = "<p>The employee is currently assigned to same subject in timetable</p> <p>Please assign another employee in timetable inorder to remove this association</p>";
            }

            var assigned_employee = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == subject.ID).ToList();
            ViewData["assigned_employee"] = assigned_employee;

            return RedirectToAction("Manage_Allotment", new { batch_id = batch_id, sub_id = id1, Notice = ViewBag.Notice });
        }

        public ActionResult Allocate_Work(int? batch_id, int? sub_id, int? emp_id)
        {
            var admin = db.EMPLOYEE_CATEGORY.Where(x => !x.PRFX.Contains("admin")).ToList();
            ViewData["admin"] = admin;

            var employees_Inner = (from emp in db.EMPLOYEEs.Include(x => x.EMPLOYEE_GRADE).Include(x => x.EMPLOYEES_SUBJECT)
                                   join empcat in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals empcat.ID
                                   where !empcat.PRFX.Contains("admin")
                                   select new EmployeeWorkAllotment { EmployeeData = emp, Total_Time = emp.EMPLOYEE_GRADE.MAX_WKILY_HRS }).ToList();
            ViewData["employees"] = employees_Inner;

            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var batches_Inner = (from cs in db.COURSEs
                                 join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
                                 where bt.END_DATE >= StartDate
                                 select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs, Total_Time = 0 }).Distinct().ToList();
            ViewData["batches"] = batches_Inner;

            var employee_subject = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == emp_id && x.SUBJ_ID == sub_id).ToList();
            if(employee_subject == null || employee_subject.Count() == 0)
            {
                var employee_sub = new EMPLOYEES_SUBJECT() { EMP_ID = emp_id, SUBJ_ID = sub_id };
                db.EMPLOYEES_SUBJECT.Add(employee_sub);
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return RedirectToAction("Work_Allotment", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Work_Allotment", new { ErrorMessage = ViewBag.ErrorMessage });
                }

                for (int i = 0; i < batches_Inner.Count(); i++)
                {
                    for (int j = 0; j < batches_Inner.ElementAt(i).BatchData.SUBJECTs.Count(); j++)
                    {
                        for (int k = 0; k < batches_Inner.ElementAt(i).BatchData.SUBJECTs.ElementAt(j).EMPLOYEES_SUBJECT.Count(); k++)
                        {
                            var ep = employees_Inner.Where(x => x.EmployeeData.ID == batches_Inner.ElementAt(i).BatchData.SUBJECTs.ElementAt(j).EMPLOYEES_SUBJECT.ElementAt(k).EMP_ID).FirstOrDefault();
                            if (ep != null && ep.EmployeeData.ID != 0)
                            {
                                ep.Total_Time = ep.Total_Time - batches_Inner.ElementAt(i).BatchData.SUBJECTs.ElementAt(j).MAX_WKILY_CLSES;
                                batches_Inner.ElementAt(i).Total_Time = batches_Inner.ElementAt(i).Total_Time - batches_Inner.ElementAt(i).BatchData.SUBJECTs.ElementAt(j).MAX_WKILY_CLSES;
                            }
                        }

                    }

                }
                var Employee = employees_Inner.Where(x => x.EmployeeData.ID == emp_id).FirstOrDefault();
                string Message = "";
                if (Employee.Total_Time < 0)
                {
                    Message = string.Concat("Employee has a deficit time of ", Convert.ToInt32(Employee.Total_Time), " Hours. Please revisit your allocation.") ;
                }
                else if (Employee.Total_Time == 0)
                {
                    Message = string.Concat("Employee is OK to be allocated and has finiashed all hours.");
                }
                else
                {
                    Message = string.Concat("Employee is OK to be allocated and has ", Employee.Total_Time, " hours remaining");
                }
                ViewBag.Notice = Message;
                return RedirectToAction("Work_Allotment", new { Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Notice = "Employee already assigned to this subject.";
                return RedirectToAction("Work_Allotment", new { Notice = ViewBag.Notice });
            }

        }


        // GET: Timetable/Create
        public ActionResult New_Timetable(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            return View();
        }

        // POST: Timetable/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New_Timetable([Bind(Include = "ID,START_DATE,END_DATE,IS_ACT,CREATED_AT,UPDATED_AT")] TIMETABLE tIMETABLE)
        {
            if (ModelState.IsValid)
            {
                tIMETABLE.IS_ACT = true;
                tIMETABLE.CREATED_AT = System.DateTime.Now;
                tIMETABLE.UPDATED_AT = System.DateTime.Now;
                db.TIMETABLEs.Add(tIMETABLE);
                bool error = false;
                var previous = db.TIMETABLEs.Where(x => x.END_DATE >= tIMETABLE.START_DATE && x.START_DATE <= tIMETABLE.START_DATE).ToList();
                if(previous != null && previous.Count() != 0)
                {
                    error = true;
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Start time is within the range of another timetable. ");
                }
                var conflicts = db.TIMETABLEs.Where(x => x.END_DATE >= tIMETABLE.END_DATE && x.START_DATE <= tIMETABLE.END_DATE).ToList();
                if (conflicts != null && conflicts.Count() != 0)
                {
                    error = true;
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "End time is within the range of another timetable");
                }
                var fully_overlapping = db.TIMETABLEs.Where(x => x.END_DATE <= tIMETABLE.END_DATE && x.START_DATE >= tIMETABLE.START_DATE).ToList();
                if (fully_overlapping != null && fully_overlapping.Count() != 0)
                {
                    error = true;
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Another timetable exists in between the given dates. ");
                }

                if (tIMETABLE.START_DATE > tIMETABLE.END_DATE)
                {
                    error = true;
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "End date is lower than start date. ");
                }

                if(!error)
                {
                    try {                
                        db.SaveChanges();
                        ViewBag.Notice = string.Concat("Time table structure created from ", tIMETABLE.START_DATE.Value.ToShortDateString(), " - ", tIMETABLE.END_DATE.Value.ToShortDateString());
                        return RedirectToAction("New", "Timetable_Entries", new { timetable_id = tIMETABLE.ID, Notice = ViewBag.Notice });
                    }
                    catch (Exception e) {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage,"Error Occured. ", e.InnerException.InnerException.Message);
                        return View(tIMETABLE);
                    }
                }
                else
                {
                    return View(tIMETABLE);
                }
            }
            else
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "There seems to be some issue with Model State.");
                return View(tIMETABLE);
            }
        }
        // GET: Timetable/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIMETABLE tIMETABLE = db.TIMETABLEs.Find(id);
            if (tIMETABLE == null)
            {
                return HttpNotFound();
            }
            return View(tIMETABLE);
        }

        // POST: Timetable/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,START_DATE,END_DATE,IS_ACT,CREATED_AT,UPDATED_AT")] TIMETABLE tIMETABLE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tIMETABLE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tIMETABLE);
        }

        // GET: Timetable/Edit/5
        public ActionResult Edit_Master(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var batches = (from cs in db.COURSEs
                           join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
                           where bt.END_DATE >= StartDate
                           select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs, Total_Time = 0 }).Distinct().ToList();
            ViewData["batches"] = batches;
            var tIMETABLE = db.TIMETABLEs.Where(x => (x.END_DATE ?? System.DateTime.Now) >= System.DateTime.Now);
            return View(tIMETABLE);
        }

        public ActionResult Update_Timetable(int? id)
        {
            TIMETABLE timetable = db.TIMETABLEs.Find(id);
            bool current = false;
            bool removable = false;
            if (timetable.START_DATE.Value.Date <= DateTime.Today && timetable.END_DATE.Value.Date >= DateTime.Today)
            {
                current = true;
            }
            if (timetable.START_DATE.Value.Date > DateTime.Today && timetable.END_DATE.Value.Date > DateTime.Today)
            {
                removable = true;
            }
            ViewBag.current = current;
            ViewBag.removable = removable;
            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var batches = (from cs in db.COURSEs
                           join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
                           where bt.END_DATE >= StartDate
                           select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs, Total_Time = 0 }).Distinct().ToList();
            ViewData["batches"] = batches;
            return View(timetable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Timetable([Bind(Include = "ID,START_DATE,END_DATE,IS_ACT,CREATED_AT,UPDATED_AT")] TIMETABLE timetable, bool current, bool removable)
        {
            bool? error = false;
            TIMETABLE tt = db.TIMETABLEs.Find(timetable.ID);
            ViewBag.current = current;
            ViewBag.removable = removable;
            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var batches = (from cs in db.COURSEs
                           join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
                           where bt.END_DATE >= StartDate
                           select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs, Total_Time = 0 }).Distinct().ToList();
            ViewData["batches"] = batches;

            DateTime new_start = DateTime.Now;
            DateTime new_end = DateTime.Now;

            if (timetable.START_DATE != null)
            {
                new_start = (DateTime)timetable.START_DATE;
            }
            else
            {
                new_start = (DateTime) tt.START_DATE;
            }
            if (timetable.END_DATE != null)
            {
                new_end = (DateTime)timetable.END_DATE;
            }
            else
            {
                new_end = (DateTime)tt.END_DATE;
            }
            if(new_end < new_start)
            {
                error = true;
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Start date is lower than end date. ");
            }
            if(new_end < DateTime.Now)
            {
                error = true;
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "End date is lower than today's date. ");
            }
            var end_conflicts = db.TIMETABLEs.Where(x => DbFunctions.TruncateTime(x.START_DATE) <= new_end && DbFunctions.TruncateTime(x.END_DATE) >= new_start  && x.ID != tt.ID).ToList();
            if(end_conflicts != null && end_conflicts.Count() != 0)
            {
                error = true;
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "End time is within the range of another timetable. ");
            }
            var fully_overlapping = db.TIMETABLEs.Where(x => DbFunctions.TruncateTime(x.END_DATE) <= DbFunctions.TruncateTime(timetable.END_DATE) && DbFunctions.TruncateTime(x.START_DATE) >= DbFunctions.TruncateTime(timetable.START_DATE) && x.ID != timetable.ID).ToList();
            if (fully_overlapping != null && fully_overlapping.Count() != 0)
            {
                error = true;
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Another timetable exists in between the given dates. ");
            }
            if(current == false)
            {
                if(new_start <= DateTime.Today)
                {
                    error = true;
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Start date is lower than today's date. ");
                }
            }
            if(error == false)
            {
                if (tt.START_DATE <= DateTime.Today && tt.END_DATE >= DateTime.Today)
                {
                    tt.END_DATE = DateTime.Today;
                    tt.UPDATED_AT = DateTime.Now;
                    db.Entry(tt).State = EntityState.Modified;
                    try {
                        db.SaveChanges();
                        if( new_end > DateTime.Today)
                        {
                            var tt2 = new TIMETABLE()
                            {
                                START_DATE = DateTime.Now.AddDays(1),
                                END_DATE = new_end,
                                IS_ACT = true,
                                CREATED_AT = DateTime.Now,
                                UPDATED_AT = DateTime.Now
                            };
                            db.TIMETABLEs.Add(tt2);
                            try {
                                db.SaveChanges();
                                var entries = db.TIMETABLE_ENTRY.Where(x => x.TIMT_ID == tt.ID).ToList();
                                foreach(var item in entries)
                                {
                                    var entry2 = new TIMETABLE_ENTRY() { BTCH_ID = item.BTCH_ID, WK_DAY_ID = item.WK_DAY_ID, EMP_ID = item.EMP_ID, TIMT_ID= item.TIMT_ID, CLS_TMNG_ID= item.CLS_TMNG_ID, SUBJ_ID=item.SUBJ_ID };
                                    entry2.TIMT_ID = tt2.ID;
                                    db.TIMETABLE_ENTRY.Add(entry2);
                                }
                                db.SaveChanges();
                            }
                            catch (Exception e) {
                                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage,"|", e.InnerException.InnerException.Message);
                            }
                            ViewBag.Notice = string.Concat(ViewBag.Notice, "Timetable updated successfully. ");
                            return RedirectToAction("New", "Timetable_Entries", new { timetable_id = tt2.ID, ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                        }
                        else
                        {
                            ViewBag.Notice = string.Concat(ViewBag.Notice, "Timetable updated successfully. ");
                            return RedirectToAction("Edit_Master", new {ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                        }
                    }
                    catch (Exception e) {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                        return RedirectToAction("New_Timetable", new { ErrorMessage  = ViewBag.ErrorMessage, Notice = ViewBag.Notice});
                    }
                }
                else
                {
                    tt.START_DATE = timetable.START_DATE;
                    tt.END_DATE = timetable.END_DATE;
                    db.Entry(tt).State = EntityState.Modified;
                    try {
                        db.SaveChanges();
                        ViewBag.Notice = "Timetable updated successfully. ";
                        return RedirectToAction("Edit_Master", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice});
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", "Timetable update failed", "|", e.InnerException.InnerException.Message);
                        error = true;
                        return View(timetable);
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Please correct all errors above and try again. ");
                return View(timetable);
            }
        }

        // GET: Timetable/Edit/5
        public ActionResult Destroy(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIMETABLE timetable = db.TIMETABLEs.Find(id);
            if (timetable == null)
            {
                return HttpNotFound();
            }

            var timetable_entries = db.TIMETABLE_ENTRY.Where(x => x.TIMT_ID == id).ToList();
            foreach(var item in timetable_entries)
            {
                db.TIMETABLE_ENTRY.Remove(item);
            }
            db.TIMETABLEs.Remove(timetable);
            try
            {
                db.SaveChanges();
                ViewBag.Notice = "Timetable deleted successfully";
                return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", "Timetable deletion failed", "|", e.InnerException.InnerException.Message);
                return View(timetable);
            }
        }


        public ActionResult ViewTimetable(int? course_id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;

            var timetables = db.TIMETABLEs.ToList();
            var current = db.TIMETABLEs.Where(x => DbFunctions.TruncateTime(x.START_DATE) <= DateTime.Today && DbFunctions.TruncateTime(x.END_DATE) >= DateTime.Today);
            int? current_id = current != null ? current.FirstOrDefault().ID : timetables.FirstOrDefault().ID;
            List <SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in timetables)
            {
                string TimetableFullName = string.Concat(item.START_DATE.Value.ToShortDateString(), " To ", item.END_DATE.Value.ToShortDateString());
                var result = new SelectListItem();
                result.Text = TimetableFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == current_id ? true : false;
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select a Batch" });
            ViewBag.TIMT_ID = options2;

            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.END_DATE >= StartDate
                                    select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                         .OrderBy(x => x.BatchData.ID).ToList();

            if(course_id == null)
            {
                course_id = queryCourceBatch.FirstOrDefault().BatchData.ID;
            }

            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID == course_id ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select a Batch" });
            ViewBag.BTCH_ID = options;

            return View(timetables);
        }

        public ActionResult Update_Timetable_View(int? course_id, int? timetable_id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            if(course_id == null || timetable_id == null)
            {
                return PartialView("_View_Timetable");
            }
            BATCH batch = db.BATCHes.Include(x=>x.COURSE).Where(x=>x.ID == course_id).FirstOrDefault();
            ViewData["batch"] = batch;
            TIMETABLE tt = db.TIMETABLEs.Find(timetable_id);
            ViewData["tt"] = tt;

            var timetable = db.TIMETABLE_ENTRY.Include(x=>x.SUBJECT).Include(x=>x.SUBJECT.ELECTIVE_GROUP).Include(x => x.SUBJECT.ELECTIVE_GROUP.SUBJECTs).Include(x => x.EMPLOYEE).Where(x => x.BTCH_ID == batch.ID && x.TIMT_ID == tt.ID).ToList();
            ViewData["timetable"] = timetable;
            if (timetable == null || timetable.Count() == 0)
            {
                return PartialView("_View_Timetable");
            }

            string[] weekday = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            ViewBag.weekday = weekday;
            var class_timing = db.CLASS_TIMING.Where(x => x.BTCH_ID == batch.ID).ToList().DefaultIfEmpty();
            ViewData["class_timing"] = class_timing;

            var day = db.WEEKDAYs.Where(x => x.BTCH_ID == batch.ID).ToList().DefaultIfEmpty();
            ViewData["day"] = day;
            var timetable_entries = db.TIMETABLE_ENTRY.Include(x => x.SUBJECT).Include(x => x.EMPLOYEE).Where(x => x.BTCH_ID == batch.ID && x.TIMT_ID == tt.ID).ToList();
            ViewData["timetable_entries"] = timetable_entries;
            var employees_subject_list = db.EMPLOYEES_SUBJECT.Include(x => x.EMPLOYEE).Include(x => x.SUBJECT).ToList();
            ViewData["EmployeesSubject"] = employees_subject_list;

            var subject_list = db.SUBJECTs.Include(x => x.ELECTIVE_GROUP).ToList();
            ViewData["Subject"] = subject_list;

            return PartialView("_View_Timetable");
        }

        public ActionResult Teachers_Timetable(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;

            var timetables = db.TIMETABLEs.ToList();
            TIMETABLE current = db.TIMETABLEs.Where(x => DbFunctions.TruncateTime(x.START_DATE) <= DateTime.Today && DbFunctions.TruncateTime(x.END_DATE) >= DateTime.Today).FirstOrDefault();
            ViewData["current"] = current;
            int? current_id = current != null ? current.ID : timetables.FirstOrDefault().ID;
            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in timetables)
            {
                string TimetableFullName = string.Concat(item.START_DATE.Value.ToShortDateString(), " To ", item.END_DATE.Value.ToShortDateString());
                var result = new SelectListItem();
                result.Text = TimetableFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == current_id ? true : false;
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select a Batch" });
            ViewBag.TIMT_ID = options2;
            if(current != null)
            {
                var all_timetable_entries = db.TIMETABLE_ENTRY.Where(x => x.TIMT_ID == current.ID && x.BATCH.IS_ACT == true && x.CLASS_TIMING.IS_DEL == false && x.WEEKDAY.IS_DEL == false).ToList();
                ViewData["all_timetable_entries"] = all_timetable_entries;
                var all_batches = all_timetable_entries.AsEnumerable().Select(x => x.BATCH).GroupBy(BATCH => BATCH.ID).Select(g => g.FirstOrDefault()).Distinct();
                ViewData["all_batches"] = all_batches;
                var all_weekdays = all_timetable_entries.AsEnumerable().Select(x => x.WEEKDAY).GroupBy(WEEKDAY => new { WEEKDAY.NAME, WEEKDAY.WKDAY, WEEKDAY.SRT_ORD, WEEKDAY.DAY_OF_WK }).Select(g => g.FirstOrDefault()).Distinct();
                ViewData["all_weekdays"] = all_weekdays;
                var all_classtimings = all_timetable_entries.AsEnumerable().Select(x => x.CLASS_TIMING).GroupBy(CLASS_TIMING => new { CLASS_TIMING.NAME, CLASS_TIMING.START_TIME, CLASS_TIMING.END_TIME}).Select(g => g.FirstOrDefault()).Distinct();              
                ViewData["all_classtimings"] = all_classtimings;
                var all_subjects = all_timetable_entries.Where(x=>x.SUBJ_ID != null).AsEnumerable().Select(x => x.SUBJECT).GroupBy(SUBJECT => SUBJECT.ID).Select(g => g.FirstOrDefault()).Distinct();
                ViewData["all_subjects"] = all_subjects;
                var all_teachers = all_timetable_entries.Where(x => x.EMP_ID != null).AsEnumerable().Select(x => x.EMPLOYEE).GroupBy(EMPLOYEE => EMPLOYEE.ID).Select(g => g.FirstOrDefault()).Distinct();                
                foreach(var sub in all_subjects)
                {
                    if(sub.ELECTIVE_GRP_ID != null)
                    {
                        var electiveGroupEmpSub = sub.ELECTIVE_GROUP.SUBJECTs.Select(x => x.EMPLOYEES_SUBJECT).ToList();
                        var elective_teachers = db.EMPLOYEEs.Where(x=>x.ID == -1).DefaultIfEmpty();
                        foreach (var item in electiveGroupEmpSub)
                        {
                            var employee = db.EMPLOYEEs.Where(x=>x.ID == item.FirstOrDefault().EMP_ID).ToList() ;
                            all_teachers = all_teachers.Union(employee);
                            elective_teachers = elective_teachers.Union(employee);
                        }
                        ViewData["elective_teachers"] = elective_teachers;
                    }
                }
                all_teachers = all_teachers.Distinct();
                ViewData["all_teachers"] = all_teachers;
            }
            else
            {
                ViewData["all_timetable_entries"] = null;
            }

            var timetable_entries = db.TIMETABLE_ENTRY.Where(x=>x.TIMT_ID == current.ID).Include(x=>x.EMPLOYEE).Include(x=>x.SUBJECT).Include(x=>x.BATCH).Include(x=>x.BATCH.COURSE).Include(x => x.CLASS_TIMING).Include(x => x.WEEKDAY).ToList();
            ViewData["timetable_entries"] = timetable_entries;

            return View();
        }

        public ActionResult Update_Teacher_TT(int? timetable_id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;

            var timetables = db.TIMETABLEs.ToList();
            TIMETABLE current = db.TIMETABLEs.Where(x => x.ID == -1).FirstOrDefault();
            if(timetable_id == null)
            {
                current = db.TIMETABLEs.Where(x => DbFunctions.TruncateTime(x.START_DATE) <= DateTime.Today && DbFunctions.TruncateTime(x.END_DATE) >= DateTime.Today).FirstOrDefault();
            }
            else
            {
                current = db.TIMETABLEs.Find(timetable_id);
            }
            ViewData["current"] = current;
            int? current_id = current != null ? current.ID : timetables.FirstOrDefault().ID;
            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in timetables)
            {
                string TimetableFullName = string.Concat(item.START_DATE.Value.ToShortDateString(), " To ", item.END_DATE.Value.ToShortDateString());
                var result = new SelectListItem();
                result.Text = TimetableFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == current_id ? true : false;
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select a Batch" });
            ViewBag.TIMT_ID = options2;
            if (current != null)
            {
                var all_timetable_entries = db.TIMETABLE_ENTRY.Where(x => x.TIMT_ID == current.ID && x.BATCH.IS_ACT == true && x.CLASS_TIMING.IS_DEL == false && x.WEEKDAY.IS_DEL == false).ToList();
                ViewData["all_timetable_entries"] = all_timetable_entries;
                var all_batches = all_timetable_entries.AsEnumerable().Select(x => x.BATCH).GroupBy(BATCH => BATCH.ID).Select(g => g.FirstOrDefault()).Distinct();
                ViewData["all_batches"] = all_batches;
                var all_weekdays = all_timetable_entries.AsEnumerable().Select(x => x.WEEKDAY).GroupBy(WEEKDAY => new { WEEKDAY.NAME, WEEKDAY.WKDAY, WEEKDAY.SRT_ORD, WEEKDAY.DAY_OF_WK }).Select(g => g.FirstOrDefault()).Distinct();
                ViewData["all_weekdays"] = all_weekdays;
                var all_classtimings = all_timetable_entries.AsEnumerable().Select(x => x.CLASS_TIMING).GroupBy(CLASS_TIMING => new { CLASS_TIMING.NAME, CLASS_TIMING.START_TIME, CLASS_TIMING.END_TIME, CLASS_TIMING.IS_BRK }).Select(g => g.FirstOrDefault()).Distinct();
                ViewData["all_classtimings"] = all_classtimings;
                var all_subjects = all_timetable_entries.Where(x => x.SUBJ_ID != null).AsEnumerable().Select(x => x.SUBJECT).GroupBy(SUBJECT => SUBJECT.ID).Select(g => g.FirstOrDefault()).Distinct();
                ViewData["all_subjects"] = all_subjects;
                var all_teachers = all_timetable_entries.Where(x => x.EMP_ID != null).AsEnumerable().Select(x => x.EMPLOYEE).GroupBy(EMPLOYEE => EMPLOYEE.ID).Select(g => g.FirstOrDefault()).Distinct();
                foreach (var sub in all_subjects)
                {
                    if (sub.ELECTIVE_GRP_ID != null)
                    {
                        var electiveGroupEmpSub = sub.ELECTIVE_GROUP.SUBJECTs.Select(x => x.EMPLOYEES_SUBJECT).ToList();
                        var elective_teachers = db.EMPLOYEEs.Where(x => x.ID == -1).DefaultIfEmpty();
                        foreach (var item in electiveGroupEmpSub)
                        {
                            var employee = db.EMPLOYEEs.Where(x => x.ID == item.FirstOrDefault().EMP_ID).ToList();
                            all_teachers = all_teachers.Union(employee);
                            elective_teachers = elective_teachers.Union(employee);
                        }
                        ViewData["elective_teachers"] = elective_teachers;
                    }
                }
                all_teachers = all_teachers.Where(x=>x.ID != 0).Distinct();
                ViewData["all_teachers"] = all_teachers;
            }
            else
            {
                ViewData["all_timetable_entries"] = null;
            }

            var timetable_entries = db.TIMETABLE_ENTRY.Where(x => x.TIMT_ID == current.ID).Include(x => x.EMPLOYEE).Include(x => x.SUBJECT).Include(x => x.BATCH).Include(x => x.BATCH.COURSE).Include(x => x.CLASS_TIMING).Include(x => x.WEEKDAY).ToList();
            ViewData["timetable_entries"] = timetable_entries;

            return PartialView("_Teacher_Timetable");
        }


        public ActionResult Timetable(string Notice, string ErrorMessage, DateTime? next)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;

            var config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "AvailableModules").ToList();
            ViewData["config"] = config;
            var batches = db.BATCHes.Include(x => x.COURSE).Where(x => x.IS_ACT == true).ToList();
            ViewData["batches"] = batches;
            var Weekday = db.WEEKDAYs.Where(x => x.IS_DEL == false).ToList();
            ViewData["Weekday"] = Weekday;
            var Timetable = db.TIMETABLEs.ToList();
            ViewData["Timetable"] = Timetable;
            var TimetableEntry = db.TIMETABLE_ENTRY.Include(x => x.SUBJECT).Include(x => x.SUBJECT.ELECTIVE_GROUP).Include(x => x.EMPLOYEE).ToList();
            ViewData["TimetableEntry"] = TimetableEntry;
            var ClassTiming = db.CLASS_TIMING.ToList();
            ViewData["ClassTiming"] = ClassTiming;
            DateTime? today = DateTime.Today;
            ViewBag.today = today;
            if (next != null)
            {
                today = Convert.ToDateTime(next);
                ViewBag.today = today;
                return PartialView("_Table");
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
