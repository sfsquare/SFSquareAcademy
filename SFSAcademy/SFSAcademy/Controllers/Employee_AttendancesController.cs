using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFSAcademy;
using System.Data.Entity.Validation;

namespace SFSAcademy.Controllers
{
    public class Employee_AttendancesController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Employee_Attendances
        public ActionResult Index(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == "Y").OrderBy(x => x.NAMES), "ID", "NAMES").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.EMP_DEPT_ID = options;
            return View();
        }

        // GET: Employee_Attendances
        public ActionResult Show(int? dept_id, string next)
        {
            EMPLOYEE_DEPARTMENT dept = db.EMPLOYEE_DEPARTMENT.Find(dept_id);
            ViewData["dept"] = dept;
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == "Y") on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == "Y") on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == "Y") on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == "Y") on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.EMP_DEPT_ID == dept_id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg)}).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();
            ViewData["employees"] = EmployeeDetail;
            DateTime today = System.DateTime.Today;
            if (next != "null")
            {
                today = Convert.ToDateTime(next);
            }
            ViewBag.today = today.ToShortDateString();
            ViewBag.today_Plus_1Month = today.AddMonths(1).ToShortDateString();
            ViewBag.today_Minus_1Month = today.AddMonths(-1).ToShortDateString();
            var start_date = new DateTime(today.Year, today.Month, 1);
            ViewBag.start_date = start_date;
            var end_date = start_date.AddMonths(1).AddDays(-1);
            ViewBag.end_date = end_date;
            ViewData["EmployeeAttendance"] = db.EMPLOYEE_ATTENDENCES.ToList();
            return PartialView("_Register");
        }

        // GET: Employee_Attendances/Create
        public ActionResult New(string Sel_date, int? id2)
        {
            EMPLOYEE employee = db.EMPLOYEEs.Find(id2);
            ViewData["employee"] = employee;
            DateTime date = Convert.ToDateTime(Sel_date);
            ViewBag.date = date.ToShortDateString();
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "Y").OrderBy(x => x.NAME), "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Leave Type" });
            ViewBag.EMP_LEAVE_TYPE_ID = options;
            return PartialView("_New");
        }

        // POST: Employee_Attendances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "ID,ATNDENCE_DATE,EMP_ID,EMP_LEAVE_TYPE_ID,RSN,IS_HALF_DAY")] EMPLOYEE_ATTENDENCES eMPLOYEE_ATTENDENCES)
        {
            if (ModelState.IsValid)
            {
                eMPLOYEE_ATTENDENCES.ATNDENCE_DATE = Convert.ToDateTime(eMPLOYEE_ATTENDENCES.ATNDENCE_DATE);
                db.EMPLOYEE_ATTENDENCES.Add(eMPLOYEE_ATTENDENCES);
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e) {foreach (var eve in e.EntityValidationErrors){foreach (var ve in eve.ValidationErrors){ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);}}
                    return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage});
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage});
                }
                ViewBag.Notice = "Leave details added sucessfully.";
                return RedirectToAction("Index", new { Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with model state.";
            return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage });
        }


        // GET: Employee_Attendances/Edit/5
        public ActionResult Edit(int? Abs_Id, int? id2)
        {
            if (Abs_Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE_ATTENDENCES attendance = db.EMPLOYEE_ATTENDENCES.Find(Abs_Id);
            if (attendance == null)
            {
                return HttpNotFound();
            }
            ViewData["attendance"] = attendance;
            EMPLOYEE employee = db.EMPLOYEEs.Find(id2);
            ViewData["employee"] = employee;
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "Y").OrderBy(x => x.NAME), "ID", "NAME", attendance.EMP_LEAVE_TYPE_ID).ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Leave Type" });
            ViewBag.EMP_LEAVE_TYPE_ID = options;
            return PartialView("_Edit", attendance);
        }

        // POST: Employee_Attendances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ATNDENCE_DATE,EMP_ID,EMP_LEAVE_TYPE_ID,RSN,IS_HALF_DAY")] EMPLOYEE_ATTENDENCES eMPLOYEE_ATTENDENCES)
        {
            if (ModelState.IsValid)
            {
                EMPLOYEE_ATTENDENCES eMPLOYEE_ATTENDENCES_upd = db.EMPLOYEE_ATTENDENCES.Find(eMPLOYEE_ATTENDENCES.ID);
                eMPLOYEE_ATTENDENCES_upd.EMP_LEAVE_TYPE_ID = eMPLOYEE_ATTENDENCES.EMP_LEAVE_TYPE_ID;
                eMPLOYEE_ATTENDENCES_upd.RSN = eMPLOYEE_ATTENDENCES.RSN;
                eMPLOYEE_ATTENDENCES_upd.IS_HALF_DAY = eMPLOYEE_ATTENDENCES.IS_HALF_DAY;
                db.Entry(eMPLOYEE_ATTENDENCES_upd).State = EntityState.Modified;
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                ViewBag.Notice = "Leave details updated sucessfully.";
                return RedirectToAction("Index", new { Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with model state.";
            return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage });
        }

        // GET: Employee_Attendances/Delete/5
        public ActionResult Delete(int? id)
        {
            EMPLOYEE_ATTENDENCES eMPLOYEE_ATTENDENCES = db.EMPLOYEE_ATTENDENCES.Find(id);
            db.EMPLOYEE_ATTENDENCES.Remove(eMPLOYEE_ATTENDENCES);
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage });
            }
            ViewBag.Notice = "Leave details deleted sucessfully.";
            return RedirectToAction("Index", new { Notice = ViewBag.Notice });
        }
        // GET: Employee_Attendances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE_ATTENDENCES eMPLOYEE_ATTENDENCES = db.EMPLOYEE_ATTENDENCES.Find(id);
            if (eMPLOYEE_ATTENDENCES == null)
            {
                return HttpNotFound();
            }
            return View(eMPLOYEE_ATTENDENCES);
        }

        // GET: Employee_Attendances/Create
        public ActionResult Create()
        {
            ViewBag.EMP_LEAVE_TYPE_ID = new SelectList(db.EMPLOYEE_LEAVE_TYPE, "ID", "NAME");
            ViewBag.EMP_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM");
            return View();
        }

        // POST: Employee_Attendances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ATNDENCE_DATE,EMP_ID,EMP_LEAVE_TYPE_ID,RSN,IS_HALF_DAY")] EMPLOYEE_ATTENDENCES eMPLOYEE_ATTENDENCES)
        {
            if (ModelState.IsValid)
            {
                db.EMPLOYEE_ATTENDENCES.Add(eMPLOYEE_ATTENDENCES);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EMP_LEAVE_TYPE_ID = new SelectList(db.EMPLOYEE_LEAVE_TYPE, "ID", "NAME", eMPLOYEE_ATTENDENCES.EMP_LEAVE_TYPE_ID);
            ViewBag.EMP_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", eMPLOYEE_ATTENDENCES.EMP_ID);
            return View(eMPLOYEE_ATTENDENCES);
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
