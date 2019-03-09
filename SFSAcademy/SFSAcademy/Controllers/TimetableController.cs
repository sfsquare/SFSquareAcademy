using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        public ActionResult Work_Allotment()
        {
            var admin = db.EMPLOYEE_CATEGORY.Where(x => !x.PRFX.Contains("admin")).ToList();
            ViewData["admin"] = admin;

            var employees = (from emp in db.EMPLOYEEs.Include(x=>x.EMPLOYEE_GRADE).Include(x=>x.EMPLOYEES_SUBJECT)
                             join empcat in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals empcat.ID
                             where !empcat.PRFX.Contains("admin")
                              select new SFSAcademy.Models.EmployeeWorkAllotment { EmployeeData = emp, Total_Time = emp.EMPLOYEE_GRADE.MAX_WKILY_HRS}).ToList();
            ViewData["employees"] = employees;

            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var batches = (from cs in db.COURSEs
                           join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
                           where bt.END_DATE >= StartDate
                           select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs, Total_Time = 0 }).Distinct().ToList();
            ViewData["batches"] = batches;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Work_Allotment(IEnumerable<SFSAcademy.Models.CoursesBatch> batches, IEnumerable<SFSAcademy.Models.EmployeeWorkAllotment> employees)
        {
            var admin = db.EMPLOYEE_CATEGORY.Where(x => !x.PRFX.Contains("admin")).ToList();
            ViewData["admin"] = admin;

            var employees_Inner = (from emp in db.EMPLOYEEs.Include(x => x.EMPLOYEE_GRADE).Include(x => x.EMPLOYEES_SUBJECT)
                             join empcat in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals empcat.ID
                             where !empcat.PRFX.Contains("admin")
                             select new SFSAcademy.Models.EmployeeWorkAllotment { EmployeeData = emp, Total_Time = emp.EMPLOYEE_GRADE.MAX_WKILY_HRS }).ToList();
            ViewData["employees"] = employees_Inner;

            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var batches_Inner = (from cs in db.COURSEs
                           join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
                           where bt.END_DATE >= StartDate
                                 select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs, Total_Time = 0 }).Distinct().ToList();
            ViewData["batches"] = batches_Inner;

            return View();
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
            if (timetable.START_DATE <= System.DateTime.Today && timetable.END_DATE >= System.DateTime.Today)
            {
                current = true;
            }
            if (timetable.START_DATE > System.DateTime.Today && timetable.END_DATE > System.DateTime.Today)
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
            var end_conflicts = db.TIMETABLEs.Where(x => DateTime.Compare((DateTime)x.START_DATE,new_end) <= 0 && DateTime.Compare((DateTime)x.END_DATE, new_start) >= 0  && x.ID != tt.ID).ToList();
            if(end_conflicts != null && end_conflicts.Count() != 0)
            {
                error = true;
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "End time is within the range of another timetable. ");
            }
            var fully_overlapping = db.TIMETABLEs.Where(x => DateTime.Compare((DateTime)x.END_DATE , (DateTime)timetable.END_DATE) <= 0 && DateTime.Compare((DateTime)x.START_DATE , (DateTime)timetable.START_DATE) >= 0 && x.ID != timetable.ID).ToList();
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
            TIMETABLE current = timetables.Where(x => DateTime.Compare((DateTime)x.START_DATE, DateTime.Now) <= 0 && DateTime.Compare((DateTime)x.END_DATE, DateTime.Now) >= 0).FirstOrDefault();
            int? current_id = current != null ? current.ID : timetables.FirstOrDefault().ID;
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
