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
    public class Employee_AttendanceController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Employee_Attendance
        public ActionResult Index()
        {
            var eMPLOYEE_ATTENDENCES = db.EMPLOYEE_ATTENDENCES.Include(e => e.EMPLOYEE_LEAVE_TYPE).Include(e => e.EMPLOYEE);
            return View(eMPLOYEE_ATTENDENCES.ToList());
        }

        // GET: Employee_Attendance/Details/5
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

        // GET: Employee_Attendance/Create
        public ActionResult Add_Leave_Types(string ErrorMessage, string Notice)
        {
            var leave_types = db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "Y").OrderBy(x => x.NAME).ToList();
            ViewData["leave_types"] = leave_types;
            var inactive_leave_types = db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "N").OrderBy(x => x.NAME).ToList();
            ViewData["inactive_leave_types"] = inactive_leave_types;
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            return View();
        }

        // POST: Employee_Attendance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Leave_Types([Bind(Include = "ID,NAME,CODE,STAT,MAX_LEAVE_CNT,CARR_FRWD")] EMPLOYEE_LEAVE_TYPE eMPLOYEE_lEAVE_tYPE)
        {
            var leave_types = db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "Y").OrderBy(x => x.NAME).ToList();
            ViewData["leave_types"] = leave_types;
            var inactive_leave_types = db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "N").OrderBy(x => x.NAME).ToList();
            ViewData["inactive_leave_types"] = inactive_leave_types;
            if (ModelState.IsValid)
            {
                db.EMPLOYEE_LEAVE_TYPE.Add(eMPLOYEE_lEAVE_tYPE);
                foreach(var item in db.EMPLOYEEs.ToList())
                {
                    var EmployeeLeave = new EMPLOYEE_LEAVE() { EMP_ID = item.ID, EMP_LEAVE_TYPE_ID = eMPLOYEE_lEAVE_tYPE.ID, LEAVE_CNT = eMPLOYEE_lEAVE_tYPE.MAX_LEAVE_CNT, CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now };
                    db.EMPLOYEE_LEAVE.Add(EmployeeLeave);
                }
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View();
                }
                ViewBag.Notice = "Leave Type added successfully!!";
                return RedirectToAction("Add_Leave_Types", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State is not Valid";
            return View();
        }

        // GET: Employee_Attendance/Edit/5
        public ActionResult Edit_Leave_Types(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE_LEAVE_TYPE leave_type = db.EMPLOYEE_LEAVE_TYPE.Find(id);
            if (leave_type == null)
            {
                return HttpNotFound();
            }
            return View(leave_type);
        }

        // POST: Employee_Attendance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Leave_Types([Bind(Include = "ID,NAME,CODE,STAT,MAX_LEAVE_CNT,CARR_FRWD")] EMPLOYEE_LEAVE_TYPE eMPLOYEE_lEAVE_tYPE)
        {
            var leave_types = db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "Y").OrderBy(x => x.NAME).ToList();
            ViewData["leave_types"] = leave_types;
            var inactive_leave_types = db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "N").OrderBy(x => x.NAME).ToList();
            ViewData["inactive_leave_types"] = inactive_leave_types;
            if (ModelState.IsValid)
            {
                EMPLOYEE_LEAVE_TYPE eMPLOYEE_lEAVE_tYPE_upd = db.EMPLOYEE_LEAVE_TYPE.Find(eMPLOYEE_lEAVE_tYPE.ID);
                eMPLOYEE_lEAVE_tYPE_upd.NAME = eMPLOYEE_lEAVE_tYPE.NAME;
                eMPLOYEE_lEAVE_tYPE_upd.CODE = eMPLOYEE_lEAVE_tYPE.CODE;
                eMPLOYEE_lEAVE_tYPE_upd.STAT = eMPLOYEE_lEAVE_tYPE.STAT;
                eMPLOYEE_lEAVE_tYPE_upd.MAX_LEAVE_CNT = eMPLOYEE_lEAVE_tYPE.MAX_LEAVE_CNT;
                eMPLOYEE_lEAVE_tYPE_upd.CARR_FRWD = eMPLOYEE_lEAVE_tYPE.CARR_FRWD;

                db.Entry(eMPLOYEE_lEAVE_tYPE_upd).State = EntityState.Modified;
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View();
                }
                ViewBag.Notice = "Leave Type edited successfully!!";
                return RedirectToAction("Add_Leave_Types", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State is not Valid";
            return View();
        }

        // GET: Employee_Attendance/Delete/5
        public ActionResult Delete_Leave_Types(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE_LEAVE_TYPE leave_type = db.EMPLOYEE_LEAVE_TYPE.Find(id);
            if (leave_type == null)
            {
                return HttpNotFound();
            }
            var attendance = db.EMPLOYEE_ATTENDENCES.Where(x => x.EMP_LEAVE_TYPE_ID == id).ToList();
            var leave_count = db.EMPLOYEE_LEAVE.Where(x => x.EMP_LEAVE_TYPE_ID == id).ToList();
            if(attendance == null || attendance.Count() == 0)
            {
                db.EMPLOYEE_LEAVE_TYPE.Remove(leave_type);
                foreach(var item in leave_count)
                {
                    EMPLOYEE_LEAVE Emp_Leave = db.EMPLOYEE_LEAVE.Find(item.ID);
                    db.EMPLOYEE_LEAVE.Remove(Emp_Leave);
                }
                ViewBag.Notice = "Leave type deleted succesfully";
            }
            else
            {
                ViewBag.Notice = "Leave Reset Successfull";
            }
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return View();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return View();
            }
            return RedirectToAction("Add_Leave_Types", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Report(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == "Y").OrderBy(x => x.NAMES), "ID", "NAMES").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.EMP_DEPT_ID = options;
            return View();
        }

        public ActionResult Update_Attendance_Report(int? department_id)
        {
            var leave_types = db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "Y").ToList();
            ViewData["leave_types"] = leave_types;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == department_id).ToList();
            ViewData["employees"] = employees;
            var EmployeeLeave = db.EMPLOYEE_LEAVE.ToList();
            ViewData["EmployeeLeave"] = EmployeeLeave;
            var EmployeeAttendance = db.EMPLOYEE_ATTENDENCES.ToList();
            ViewData["EmployeeAttendance"] = EmployeeAttendance;
            return PartialView("_Attendance_Report");
        }
        public ActionResult Manual_Reset()
        {
            return View();
        }
        public ActionResult Leave_Reset_Settings()
        {
            var Config = new Models.Configuration();
            ViewBag.auto_reset = Config.find_by_config_key("AutomaticLeaveReset");
            ViewBag.reset_period = Config.find_by_config_key("LeaveResetPeriod");
            ViewBag.last_reset = Config.find_by_config_key("LastAutoLeaveReset");
            ViewBag.fin_start_date = Config.find_by_config_key("FinancialYearStartDate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Leave_Reset_Settings([Bind(Include = "automatic_leave_reset,leave_reset_period,last_reset_date,financial_year_start_date")] SFSAcademy.Models.LeaveReset LeaveReset)
        {
            var Config1 = db.CONFIGURATIONs.Where(x=>x.CONFIG_KEY == "AutomaticLeaveReset").FirstOrDefault();
            Config1.CONFIG_VAL = LeaveReset.automatic_leave_reset == true ? "1" : "0";
            db.Entry(Config1).State = EntityState.Modified;
            var Config2 = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "LeaveResetPeriod").FirstOrDefault();
            Config2.CONFIG_VAL = LeaveReset.leave_reset_period;
            db.Entry(Config2).State = EntityState.Modified;
            var Config3 = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "LastAutoLeaveReset").FirstOrDefault();
            Config3.CONFIG_VAL = LeaveReset.last_reset_date;
            db.Entry(Config3).State = EntityState.Modified;

            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return View();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return View();
            }
            ViewBag.Notice = "Settings has been saved";
            return View();
        }

        public ActionResult Employee_Leave_Reset_All(string Notice)
        {
            ViewBag.Notice = Notice;
            return View();
        }
        public ActionResult Update_Employee_Leave_Reset_All()
        {
            var leave_count = db.EMPLOYEE_LEAVE.ToList();
            //ViewData["leave_count"] = leave_count;
            decimal default_leave_count = 0;
            decimal leave_taken = 0;
            decimal available_leave = 0;
            decimal balance_leave = 0;
            foreach (var item in leave_count)
            {
                EMPLOYEE_LEAVE_TYPE leave_type = db.EMPLOYEE_LEAVE_TYPE.Find(item.EMP_LEAVE_TYPE_ID);
                if(leave_type.STAT=="Y")
                {
                    default_leave_count = (decimal)leave_type.MAX_LEAVE_CNT;
                    if(leave_type.CARR_FRWD)
                    {
                        leave_taken = item.LEAVE_TAKE != null? (decimal)item.LEAVE_TAKE: 0;
                        available_leave = item.LEAVE_CNT!=null? (decimal)item.LEAVE_CNT: 0;
                        if(leave_taken <= available_leave)
                        {
                            balance_leave = available_leave - leave_taken;
                            available_leave = balance_leave;
                            available_leave += default_leave_count;
                            leave_taken = 0;
                            item.LEAVE_TAKE = leave_taken;
                            item.LEAVE_CNT = available_leave;
                            item.RST_DATE = System.DateTime.Today;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            available_leave = default_leave_count;
                            leave_taken = 0;
                            item.LEAVE_TAKE = leave_taken;
                            item.LEAVE_CNT = available_leave;
                            item.RST_DATE = System.DateTime.Today;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        available_leave = default_leave_count;
                        leave_taken = 0;
                        item.LEAVE_TAKE = leave_taken;
                        item.LEAVE_CNT = available_leave;
                        item.RST_DATE = System.DateTime.Today;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            ViewBag.Notice = "Leave count reset successful for all employees";
            return RedirectToAction("Employee_Leave_Reset_All", new { Notice = ViewBag.Notice });
        }

        public ActionResult Employee_Leave_Reset_By_Department(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == "Y").OrderBy(x => x.NAMES), "ID", "NAMES").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.EMP_DEPT_ID = options;
            return View();
        }

        public ActionResult List_Department_Leave_Reset(int? department_id)
        {
            var leave_types = db.EMPLOYEE_LEAVE_TYPE.Where(x => x.STAT == "Y").OrderBy(x => x.NAME).ToList();
            ViewData["leave_types"] = leave_types;
            //var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == department_id).ToList();
            var employees = (from emp in db.EMPLOYEEs
                               where emp.STAT == "Y"
                               select new SFSAcademy.Models.EmployeeLeaveReset { EmployeeData = emp, Selected = true }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();
            ViewData["employees"] = employees;


            return PartialView("_Department_List", employees);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Department_Leave_Reset(IList<SFSAcademy.Models.EmployeeLeaveReset> model)
        {
            decimal default_leave_count = 0;
            decimal leave_taken = 0;
            decimal available_leave = 0;
            decimal balance_leave = 0;
            foreach (var item in model)
            {
                if(item.Selected)
                {
                    var leave_count = db.EMPLOYEE_LEAVE.Where(x => x.EMP_ID == item.EmployeeData.ID).ToList();
                    foreach (var item2 in leave_count)
                    {
                        EMPLOYEE_LEAVE_TYPE leave_type = db.EMPLOYEE_LEAVE_TYPE.Find(item2.EMP_LEAVE_TYPE_ID);
                        if (leave_type.STAT == "Y")
                        {
                            default_leave_count = (decimal)leave_type.MAX_LEAVE_CNT;
                            if (leave_type.CARR_FRWD)
                            {
                                leave_taken = item2.LEAVE_TAKE != null ? (decimal)item2.LEAVE_TAKE : 0;
                                available_leave = item2.LEAVE_CNT != null ? (decimal)item2.LEAVE_CNT : 0;
                                if (leave_taken <= available_leave)
                                {
                                    balance_leave = available_leave - leave_taken;
                                    available_leave = balance_leave;
                                    available_leave += default_leave_count;
                                    leave_taken = 0;
                                    item2.LEAVE_TAKE = leave_taken;
                                    item2.LEAVE_CNT = available_leave;
                                    item2.RST_DATE = System.DateTime.Today;
                                    db.Entry(item2).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    available_leave = default_leave_count;
                                    leave_taken = 0;
                                    item2.LEAVE_TAKE = leave_taken;
                                    item2.LEAVE_CNT = available_leave;
                                    item2.RST_DATE = System.DateTime.Today;
                                    db.Entry(item2).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                available_leave = default_leave_count;
                                leave_taken = 0;
                                item2.LEAVE_TAKE = leave_taken;
                                item2.LEAVE_CNT = available_leave;
                                item2.RST_DATE = System.DateTime.Today;
                                db.Entry(item2).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }

            }
            ViewBag.Notice = "Department Wise Leave Reset Successfull";
            return RedirectToAction("Employee_Leave_Reset_By_Department", new { Notice = ViewBag.Notice });
        }


        public ActionResult Employee_Leave_Reset_By_Employee(string ErrorMessage, string Notice)
        {
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == "Y") on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == "Y") on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == "Y") on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == "Y") on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.STAT == "Y"
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg) }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            return View(EmployeeDetail);
        }

        public ActionResult Employee_Leave_Details(int? id)
        {
            EMPLOYEE employee = db.EMPLOYEEs.Find(id);
            var leave_count = db.EMPLOYEE_LEAVE.Where(x => x.EMP_ID == id).ToList();
            ViewData["leave_count"] = leave_count;
            var EmployeeLeaveType = db.EMPLOYEE_LEAVE_TYPE.ToList();
            ViewData["EmployeeLeaveType"] = EmployeeLeaveType;

            return View(employee);
        }

        public ActionResult Employee_Wise_Leave_Reset(int? id)
        {
            decimal default_leave_count = 0;
            decimal leave_taken = 0;
            decimal available_leave = 0;
            decimal balance_leave = 0;
            EMPLOYEE employee = db.EMPLOYEEs.Find(id);
            ViewData["Employee"] = employee;
            var leave_count = db.EMPLOYEE_LEAVE.Where(x => x.EMP_ID == id).ToList();
            ViewData["leave_count"] = leave_count;
            var EmployeeLeaveType = db.EMPLOYEE_LEAVE_TYPE.ToList();
            ViewData["EmployeeLeaveType"] = EmployeeLeaveType;
            foreach (var item in leave_count)
            {
                EMPLOYEE_LEAVE_TYPE leave_type = db.EMPLOYEE_LEAVE_TYPE.Find(item.EMP_LEAVE_TYPE_ID);
                if(leave_type.STAT == "Y")
                {
                    default_leave_count = (decimal)leave_type.MAX_LEAVE_CNT;
                    if (leave_type.CARR_FRWD)
                    {
                        leave_taken = item.LEAVE_TAKE != null ? (decimal)item.LEAVE_TAKE : 0;
                        available_leave = item.LEAVE_CNT != null ? (decimal)item.LEAVE_CNT : 0;
                        if (leave_taken <= available_leave)
                        {
                            balance_leave = available_leave - leave_taken;
                            available_leave = balance_leave;
                            available_leave += default_leave_count;
                            leave_taken = 0;
                            item.LEAVE_TAKE = leave_taken;
                            item.LEAVE_CNT = available_leave;
                            item.RST_DATE = System.DateTime.Today;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            available_leave = default_leave_count;
                            leave_taken = 0;
                            item.LEAVE_TAKE = leave_taken;
                            item.LEAVE_CNT = available_leave;
                            item.RST_DATE = System.DateTime.Today;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        available_leave = default_leave_count;
                        leave_taken = 0;
                        item.LEAVE_TAKE = leave_taken;
                        item.LEAVE_CNT = available_leave;
                        item.RST_DATE = System.DateTime.Today;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            ViewBag.Notice = "Leave Reset Successfull";
            return PartialView("_Employee_Reset_Sucess");
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
