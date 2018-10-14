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
        public ActionResult Index()
        {
            var config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "AvailableModules").ToList();
            ViewData["config"] = config;
            var Config_Val = new Models.Configuration();
            ViewBag.StudentAttendanceType = Config_Val.find_by_config_key("StudentAttendanceType");
            return View();
        }

        // GET: Timetable/Create
        public ActionResult Work_Allotment()
        {
            var admin_ids = (from emp in db.EMPLOYEEs
                             join empcat in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals empcat.ID
                             where empcat.NAME.Contains("%Admin%")
                             select emp).Distinct().ToList();
            ViewData["admin_ids"] = admin_ids;
            /*var EmployeeSubject = (from empsub in db.EMPLOYEES_SUBJECT
                             where !(from aid in admin_ids
                                     select aid.ID).Contains(Convert.ToInt32(empsub.EMP_ID))
                             select empsub).ToList();
            ViewData["EmployeeSubject"] = EmployeeSubject;*/
            var employees = (from emp in db.EMPLOYEEs
                              where !(from aid in admin_ids
                                      select aid.ID).Contains(emp.ID)
                              select new SFSAcademy.Models.EmployeeWorkAllotment { EmployeeData = emp, Total_Time = 0}).ToList();
            ViewData["employees"] = employees;
            var batches = (from cs in db.COURSEs
                           join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
                           select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs, Total_Time = 0 }).Distinct().ToList();
            ViewData["batches"] = batches;
            /*var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == "N"
                                    select bt).Include(x=>x.COURSE).Include(x=>x.SUBJECTs)
                         .OrderBy(x => x.ID).ToList();
            ViewData["batches"] = queryCourceBatch;*/

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Work_Allotment(IEnumerable<SFSAcademy.Models.CoursesBatch> batches, IEnumerable<SFSAcademy.Models.EmployeeWorkAllotment> employees)
        {
            /*var admin_ids = (from emp in db.EMPLOYEEs
                             join empcat in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals empcat.ID
                             where empcat.NAME.Contains("%Admin%")
                             select emp).Distinct().ToList();
            ViewData["admin_ids"] = admin_ids;
            var employees = (from emp in db.EMPLOYEEs
                             where !(from aid in admin_ids
                                     select aid.ID).Contains(emp.ID)
                             select new SFSAcademy.Models.EmployeeWorkAllotment { EmployeeData = emp, Total_Time = 0 }).Distinct().ToList();
            ViewData["employees"] = employees;
            var Sub = db.SUBJECTs.Include(x => x.EMPLOYEES_SUBJECT).ToList();
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == "N"
                                    select bt).Include(x => x.COURSE).Include(x => x.SUBJECTs)
                         .OrderBy(x => x.ID).ToList();
            ViewData["batches"] = queryCourceBatch;
            var EmployeeSubject = db.EMPLOYEES_SUBJECT.ToList();
            ViewData["EmployeeSubject"] = EmployeeSubject;*/

            var admin_ids = (from emp in db.EMPLOYEEs
                             join empcat in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals empcat.ID
                             where empcat.NAME.Contains("%Admin%")
                             select emp).Distinct().ToList();
            ViewData["admin_ids"] = admin_ids;

            var employees_Inner = (from emp in db.EMPLOYEEs
                             where !(from aid in admin_ids
                                     select aid.ID).Contains(emp.ID)
                             select new SFSAcademy.Models.EmployeeWorkAllotment { EmployeeData = emp, Total_Time = 0 }).ToList();
            ViewData["employees"] = employees_Inner;
            var batches_Inner = (from cs in db.COURSEs
                           join bt in db.BATCHes.Include(x => x.SUBJECTs.Select(c => c.EMPLOYEES_SUBJECT)) on cs.ID equals bt.CRS_ID
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
                        ViewBag.Notice = string.Concat("timetable_created_from ", tIMETABLE.START_DATE.Value.ToShortDateString(), " - ", tIMETABLE.END_DATE.Value.ToShortDateString());
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
