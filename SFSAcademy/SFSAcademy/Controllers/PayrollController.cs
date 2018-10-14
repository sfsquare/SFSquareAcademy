using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFSAcademy;
using SFSAcademy.Models;
using System.Data.Entity.Validation;

namespace SFSAcademy.Controllers
{
    public class PayrollController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Payroll
        public ActionResult Index()
        {
            return View(db.PAYROLL_CATEGORY.ToList());
        }

        // GET: Payroll/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYROLL_CATEGORY pAYROLL_CATEGORY = db.PAYROLL_CATEGORY.Find(id);
            if (pAYROLL_CATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(pAYROLL_CATEGORY);
        }

        // GET: Payroll/Create
        public ActionResult Add_Category(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            List<SelectListItem> options = new SelectList(db.PAYROLL_CATEGORY.Where(x => x.STAT == true).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Parent Category" });
            ViewBag.PYRL_CAT_ID = options;
            var categories = (from prc in db.PAYROLL_CATEGORY
                          where prc.IS_DED == false
                          select new Models.PayrollCategory { PayrollCatData = prc })
                            .OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["categories"] = categories;

            var deductionable_categories = (from prc in db.PAYROLL_CATEGORY
                                   where prc.IS_DED == true
                                   select new Models.PayrollCategory { PayrollCatData = prc })
                            .OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["deductionable_categories"] = deductionable_categories;

            return View();
        }

        // POST: Payroll/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Category([Bind(Include = "ID,NAME,PCT,PYRL_CAT_ID,IS_DED,STAT")] PAYROLL_CATEGORY pAYROLL_CATEGORY)
        {
            if (ModelState.IsValid)
            {
                List<SelectListItem> options = new SelectList(db.PAYROLL_CATEGORY.Where(x => x.STAT == true).OrderBy(x => x.NAME), "ID", "NAME", pAYROLL_CATEGORY.PYRL_CAT_ID).ToList();
                options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Parent Category" });
                ViewBag.PYRL_CAT_ID = options;
                var categories = (from prc in db.PAYROLL_CATEGORY
                                  where prc.IS_DED == false
                                  select new Models.PayrollCategory { PayrollCatData = prc })
                            .OrderBy(x => x.PayrollCatData.NAME).ToList();
                ViewData["categories"] = categories;

                var deductionable_categories = (from prc in db.PAYROLL_CATEGORY
                                                where prc.IS_DED == true
                                                select new Models.PayrollCategory { PayrollCatData = prc })
                                .OrderBy(x => x.PayrollCatData.NAME).ToList();
                ViewData["deductionable_categories"] = deductionable_categories;

                if(pAYROLL_CATEGORY.PYRL_CAT_ID == -1)
                {
                    pAYROLL_CATEGORY.PYRL_CAT_ID = null;
                }
                db.PAYROLL_CATEGORY.Add(pAYROLL_CATEGORY);
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
                    return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Payroll Category added in system successfully!";
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Payroll/Edit/5
        public ActionResult Edit_Category(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYROLL_CATEGORY pAYROLL_CATEGORY = db.PAYROLL_CATEGORY.Find(id);
            List<SelectListItem> options = new SelectList(db.PAYROLL_CATEGORY.Where(x => x.STAT == true).OrderBy(x => x.NAME), "ID", "NAME", pAYROLL_CATEGORY.PYRL_CAT_ID).ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Parent Category" });
            ViewBag.PYRL_CAT_ID = options;
            if (pAYROLL_CATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(pAYROLL_CATEGORY);
        }

        // POST: Payroll/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Category([Bind(Include = "ID,NAME,PCT,PYRL_CAT_ID,IS_DED,STAT")] PAYROLL_CATEGORY pAYROLL_CATEGORY)
        {
            if (ModelState.IsValid)
            {
                if (pAYROLL_CATEGORY.PYRL_CAT_ID == -1)
                {
                    pAYROLL_CATEGORY.PYRL_CAT_ID = null;
                }
                db.Entry(pAYROLL_CATEGORY).State = EntityState.Modified;
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
                    return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Payroll Category updated in system successfully!";
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Payroll/Delete/5
        public ActionResult Delete_Category(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYROLL_CATEGORY pAYROLL_CATEGORY = db.PAYROLL_CATEGORY.Find(id);
            if (pAYROLL_CATEGORY == null)
            {
                return HttpNotFound();
            }
            db.PAYROLL_CATEGORY.Remove(pAYROLL_CATEGORY);
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
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.Notice = "Payroll Category deleted fom system successfully!";
            return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Payroll/Delete/5
        public ActionResult Inactivate_Category(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYROLL_CATEGORY pAYROLL_CATEGORY = db.PAYROLL_CATEGORY.Find(id);
            if (pAYROLL_CATEGORY == null)
            {
                return HttpNotFound();
            }
            pAYROLL_CATEGORY.STAT = false;
            db.Entry(pAYROLL_CATEGORY).State = EntityState.Modified;
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
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.Notice = "Payroll Category Deactivated successfully!";
            return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Payroll/Delete/5
        public ActionResult Activate_Category(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYROLL_CATEGORY pAYROLL_CATEGORY = db.PAYROLL_CATEGORY.Find(id);
            if (pAYROLL_CATEGORY == null)
            {
                return HttpNotFound();
            }
            pAYROLL_CATEGORY.STAT = true;
            db.Entry(pAYROLL_CATEGORY).State = EntityState.Modified;
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
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.Notice = "Payroll Category Activated successfully!";
            return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Payroll/Delete/5
        public ActionResult Manage_Payroll(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            ViewData["Employee"] = Employee;
            var independent_categories = (from pc in db.PAYROLL_CATEGORY
                                          where pc.PYRL_CAT_ID == null && pc.STAT == true
                                          select new SFSAcademy.Models.EmployeePayroll { PayrollCatData = pc, EmployeeId = Employee.ID}).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["independent_categories"] = independent_categories;
            var dependent_categories = (from pc in db.PAYROLL_CATEGORY
                                        join dpc in db.PAYROLL_CATEGORY on pc.ID equals dpc.PYRL_CAT_ID
                                        where dpc.PYRL_CAT_ID != null && pc.STAT == true
                                        select new SFSAcademy.Models.EmployeeDependentPayroll { PayrollCatData = pc,DependentPayrollCatData = dpc, DependentEmployeeId = Employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["dependent_categories"] = dependent_categories;

            return View(Employee);
        }

        // POST: Payroll/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage_Payroll(IEnumerable<SFSAcademy.Models.EmployeePayroll> independent_categories, IEnumerable<SFSAcademy.Models.EmployeeDependentPayroll> dependent_categories)
        {
            int EmpId = independent_categories == null ? dependent_categories.FirstOrDefault().DependentEmployeeId : independent_categories.FirstOrDefault().EmployeeId;

            EMPLOYEE Employee = db.EMPLOYEEs.Find(EmpId);
            ViewData["Employee"] = Employee;
            var independent_categories_inner = (from pc in db.PAYROLL_CATEGORY
                                          where pc.PYRL_CAT_ID == null && pc.STAT == true
                                          select new SFSAcademy.Models.EmployeePayroll { PayrollCatData = pc, EmployeeId = Employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["independent_categories"] = independent_categories_inner;
            var dependent_categories_ineer = (from pc in db.PAYROLL_CATEGORY
                                        join dpc in db.PAYROLL_CATEGORY on pc.ID equals dpc.PYRL_CAT_ID
                                        where dpc.PYRL_CAT_ID != null && pc.STAT == true
                                        select new SFSAcademy.Models.EmployeeDependentPayroll { PayrollCatData = pc, DependentPayrollCatData = dpc, DependentEmployeeId = Employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["dependent_categories"] = dependent_categories_ineer;

            if (ModelState.IsValid)
            {
                if (independent_categories != null || dependent_categories != null)
                {
                    var payroll_created = (from ess in db.EMPLOYEE_SALARY_STRUCTURE
                                           where ess.EMP_ID == EmpId
                                           select ess).ToList();
                    if (payroll_created == null || payroll_created.Count() == 0)
                    {
                        foreach (var item in independent_categories)
                        {
                            if(item.PayrollCategoryId != 0)
                            {
                                var EmpSalStr = new EMPLOYEE_SALARY_STRUCTURE() { EMP_ID = item.EmployeeId, PYRL_CAT_ID = item.PayrollCategoryId, AMT = item.Amount };
                                db.EMPLOYEE_SALARY_STRUCTURE.Add(EmpSalStr);
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
                                    return View(Employee);
                                }
                                catch (Exception e)
                                {
                                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                                    return View(Employee);
                                }
                            }                            
                        }

                        foreach (var item in dependent_categories)
                        {
                            if(item.DependentPayrollCategoryId != 0)
                            {
                                var EmpSalStr = new EMPLOYEE_SALARY_STRUCTURE() { EMP_ID = item.DependentEmployeeId, PYRL_CAT_ID = item.DependentPayrollCategoryId, AMT = item.DependentAmount };
                                db.EMPLOYEE_SALARY_STRUCTURE.Add(EmpSalStr);
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
                                    return View(Employee);
                                }
                                catch (Exception e)
                                {
                                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                                    return View(Employee);
                                }
                            }
                            
                        }

                    }
                }
                return RedirectToAction("Profiles", "Employee", new { id = EmpId });
            }

            return View(Employee);
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
