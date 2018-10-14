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
using System.Web.UI.WebControls;
using System.IO;
using SFSAcademy.HtmlHelpers;
using PagedList;
using System.Data.Entity.Core.Objects;

namespace SFSAcademy.Controllers
{
    public class EmployeeController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Employee
        public ActionResult Index()
        {
            var eMPLOYEEs = db.EMPLOYEEs.Include(e => e.COUNTRY).Include(e => e.COUNTRY1).Include(e => e.COUNTRY2).Include(e => e.EMPLOYEE_CATEGORY).Include(e => e.EMPLOYEE_POSITION).Include(e => e.EMPLOYEE_DEPARTMENT).Include(e => e.EMPLOYEE2).Include(e => e.EMPLOYEE_GRADE).Include(e => e.USER);
            return View(eMPLOYEEs.ToList());
        }

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            return View(eMPLOYEE);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME");
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME");
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME");
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME");
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME");
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "CODE");
            ViewBag.RPTG_MGR_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM");
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_CODE");
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME");
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RPTG_MGR_ID,EMP_CAT_ID,EMP_NUM,JOINING_DATE,FIRST_NAME,MID_NAME,LAST_NAME,GNDR,JOB_TIL,EMP_POS_ID,EMP_DEPT_ID,EMP_GRADE_ID,QUAL,EXPNC_DETL,EXPNC_YEAR,EXPNC_MONTH,STAT,STAT_DESCR,DOB,MARITAL_STAT,CHLD_CNT,FTHR_NAME,MTHR_NAME,HUSBND_NAME,BLOOD_GRP,NTLTY_ID,HOME_ADDR_LINE1,HOME_ADDR_LINE2,HOME_CITY,HOME_STATE,HOME_CTRY_ID,HOME_PIN_CODE,OFF_ADDR_LINE1,OFF_ADDR_LINE2,OFF_CITY,OFF_STATE,OFF_CTRY_ID,OFF_PIN_CODE,OFF_PH1,OFF_PH2,MOBL_PH,HOME_PH,EML,FAX,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,CREATED_AT,UPDATED_AT,PHTO_FILE_SIZE,USRID")] EMPLOYEE eMPLOYEE)
        {
            if (ModelState.IsValid)
            {
                db.EMPLOYEEs.Add(eMPLOYEE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.NTLTY_ID);
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.HOME_CTRY_ID);
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.OFF_CTRY_ID);
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME", eMPLOYEE.EMP_CAT_ID);
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME", eMPLOYEE.EMP_POS_ID);
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "CODE", eMPLOYEE.EMP_DEPT_ID);
            ViewBag.RPTG_MGR_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", eMPLOYEE.RPTG_MGR_ID);
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_CODE", eMPLOYEE.EMP_GRADE_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", eMPLOYEE.USRID);
            return View(eMPLOYEE);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.NTLTY_ID);
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.HOME_CTRY_ID);
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.OFF_CTRY_ID);
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME", eMPLOYEE.EMP_CAT_ID);
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME", eMPLOYEE.EMP_POS_ID);
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "CODE", eMPLOYEE.EMP_DEPT_ID);
            ViewBag.RPTG_MGR_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", eMPLOYEE.RPTG_MGR_ID);
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_CODE", eMPLOYEE.EMP_GRADE_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", eMPLOYEE.USRID);
            return View(eMPLOYEE);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RPTG_MGR_ID,EMP_CAT_ID,EMP_NUM,JOINING_DATE,FIRST_NAME,MID_NAME,LAST_NAME,GNDR,JOB_TIL,EMP_POS_ID,EMP_DEPT_ID,EMP_GRADE_ID,QUAL,EXPNC_DETL,EXPNC_YEAR,EXPNC_MONTH,STAT,STAT_DESCR,DOB,MARITAL_STAT,CHLD_CNT,FTHR_NAME,MTHR_NAME,HUSBND_NAME,BLOOD_GRP,NTLTY_ID,HOME_ADDR_LINE1,HOME_ADDR_LINE2,HOME_CITY,HOME_STATE,HOME_CTRY_ID,HOME_PIN_CODE,OFF_ADDR_LINE1,OFF_ADDR_LINE2,OFF_CITY,OFF_STATE,OFF_CTRY_ID,OFF_PIN_CODE,OFF_PH1,OFF_PH2,MOBL_PH,HOME_PH,EML,FAX,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,CREATED_AT,UPDATED_AT,PHTO_FILE_SIZE,USRID")] EMPLOYEE eMPLOYEE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eMPLOYEE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.NTLTY_ID);
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.HOME_CTRY_ID);
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.OFF_CTRY_ID);
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME", eMPLOYEE.EMP_CAT_ID);
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME", eMPLOYEE.EMP_POS_ID);
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "CODE", eMPLOYEE.EMP_DEPT_ID);
            ViewBag.RPTG_MGR_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", eMPLOYEE.RPTG_MGR_ID);
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_CODE", eMPLOYEE.EMP_GRADE_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", eMPLOYEE.USRID);
            return View(eMPLOYEE);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            return View(eMPLOYEE);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            db.EMPLOYEEs.Remove(eMPLOYEE);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult HR(string Notice)
        {
            ViewBag.Notice = Notice;
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var EmployeeData = (from emp in db.EMPLOYEEs
                                join ec in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals ec.ID
                                where emp.USRID == UserId
                                select new Models.Employee { EmployeeData = emp, CategoryData = ec}).Distinct();

            return View(EmployeeData.FirstOrDefault());
        }

        public ActionResult Settings()
        {

            return View();
        }

        public ActionResult Add_Category(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var categories = (from ec in db.EMPLOYEE_CATEGORY
                             where ec.STAT == true
                             select new Models.EmployeeCategory { CategoryData = ec })
                            .OrderBy(x => x.CategoryData.NAME).ToList();
            ViewData["categories"] = categories;

            var inactive_categories = (from ec in db.EMPLOYEE_CATEGORY
                              where ec.STAT == false
                              select new Models.EmployeeCategory { CategoryData = ec })
                            .OrderBy(x => x.CategoryData.NAME).ToList();
            ViewData["inactive_categories"] = inactive_categories;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Category([Bind(Include = "ID,NAME,PRFX,STAT")] EMPLOYEE_CATEGORY eMPLOYEEcTAGORY)
        {
            if (ModelState.IsValid)
            {
                var categories = (from ec in db.EMPLOYEE_CATEGORY
                                  where ec.STAT == true
                                  select new Models.EmployeeCategory { CategoryData = ec })
                           .OrderBy(x => x.CategoryData.NAME).ToList();
                ViewData["categories"] = categories;

                var inactive_categories = (from ec in db.EMPLOYEE_CATEGORY
                                           where ec.STAT == false
                                           select new Models.EmployeeCategory { CategoryData = ec })
                                .OrderBy(x => x.CategoryData.NAME).ToList();
                ViewData["inactive_categories"] = inactive_categories;

                db.EMPLOYEE_CATEGORY.Add(eMPLOYEEcTAGORY);
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
                ViewBag.Notice = "Employee Category added in system successfully!";
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Edit_Category(int? id)
        {
            EMPLOYEE_CATEGORY EmployeeCat = db.EMPLOYEE_CATEGORY.Find(id);

            return View(EmployeeCat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Category([Bind(Include = "ID,NAME,PRFX,STAT")] EMPLOYEE_CATEGORY eMPLOYEEcTAGORY)
        {
            if (ModelState.IsValid)
            {
                var Employee = (from emp in db.EMPLOYEEs
                                where emp.EMP_CAT_ID == eMPLOYEEcTAGORY.ID && emp.STAT == true
                                select emp).Distinct();
                if((eMPLOYEEcTAGORY.STAT == false && Employee.Count() == 0) || (eMPLOYEEcTAGORY.STAT == true))
                {
                    db.Entry(eMPLOYEEcTAGORY).State = EntityState.Modified;
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
                    if(eMPLOYEEcTAGORY.STAT == false)
                    {
                        var Position = (from pos in db.EMPLOYEE_POSITION
                                        join ecat in db.EMPLOYEE_CATEGORY on pos.EMP_CAT_ID equals ecat.ID
                                        where ecat.ID == eMPLOYEEcTAGORY.ID
                                        select pos).Distinct();
                        foreach(var item in Position)
                        {
                            EMPLOYEE_POSITION EmpPos = db.EMPLOYEE_POSITION.Find(item.ID);
                            EmpPos.IS_ACT = false;
                            db.Entry(EmpPos).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    ViewBag.Notice = "Employee Category updated in system successfully!";
                    return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                else
                {
                    ViewBag.ErrorMessage = "Updates canot be done as Active Employees are attached to this Category.";
                    return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Delete_Category(int? id)
        {
            EMPLOYEE_CATEGORY eMPLOYEEcATEGORY = db.EMPLOYEE_CATEGORY.Find(id);
            var employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_CAT_ID == id && emp.STAT == true
                             select emp).Distinct();
            if(employees == null || employees.Count() == 0)
            {
                employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_CAT_ID == id && emp.STAT == false
                             select emp).Distinct();
            }
            var category_position = (from pos in db.EMPLOYEE_POSITION
                                     join ecat in db.EMPLOYEE_CATEGORY on pos.EMP_CAT_ID equals ecat.ID
                                     where ecat.ID == id && pos.IS_ACT == true
                                     select pos).Distinct();
            if((employees == null || employees.Count() == 0) &&(category_position == null || category_position.Count() == 0))
            {
                db.EMPLOYEE_CATEGORY.Remove(eMPLOYEEcATEGORY);
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
                ViewBag.Notice = "Employee Category deleted fom system successfully!";
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Notice = "Employee Category cannot be deleted as employees are attached to this Category!";
                return RedirectToAction("Add_Category", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            
        }

        public ActionResult Add_Position(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true).OrderBy(x=>x.NAME), "ID", "NAME");
            var positions = (from ep in db.EMPLOYEE_POSITION
                             join ecat in db.EMPLOYEE_CATEGORY on ep.EMP_CAT_ID equals ecat.ID
                              where ep.IS_ACT == true
                              select new Models.EmployeePosition { PositionData = ep, CategoryData = ecat })
                            .OrderBy(x => x.PositionData.POS_NAME).ToList();
            ViewData["positions"] = positions;

            var inactive_positions = (from ep in db.EMPLOYEE_POSITION
                                      join ecat in db.EMPLOYEE_CATEGORY on ep.EMP_CAT_ID equals ecat.ID
                                      where ep.IS_ACT == false
                                       select new Models.EmployeePosition { PositionData = ep, CategoryData = ecat })
                            .OrderBy(x => x.PositionData.POS_NAME).ToList();
            ViewData["inactive_positions"] = inactive_positions;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Position([Bind(Include = "ID,POS_NAME,POS_DESCR,IS_ACT,EMP_CAT_ID")] EMPLOYEE_POSITION eMPLOYEEpOSITION)
        {
            if (ModelState.IsValid)
            {
                ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x=>x.STAT==true).OrderBy(x => x.NAME), "ID", "NAME");
                var positions = (from ep in db.EMPLOYEE_POSITION
                                 join ecat in db.EMPLOYEE_CATEGORY on ep.EMP_CAT_ID equals ecat.ID
                                 where ep.IS_ACT == true
                                 select new Models.EmployeePosition { PositionData = ep, CategoryData = ecat })
                                .OrderBy(x => x.PositionData.POS_NAME).ToList();
                ViewData["positions"] = positions;

                var inactive_positions = (from ep in db.EMPLOYEE_POSITION
                                          join ecat in db.EMPLOYEE_CATEGORY on ep.EMP_CAT_ID equals ecat.ID
                                          where ep.IS_ACT == false
                                          select new Models.EmployeePosition { PositionData = ep, CategoryData = ecat })
                                .OrderBy(x => x.PositionData.POS_NAME).ToList();
                ViewData["inactive_positions"] = inactive_positions;

                db.EMPLOYEE_POSITION.Add(eMPLOYEEpOSITION);
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
                    return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Employee Position added in system successfully!";
                return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Edit_Position(int? id)
        {
            EMPLOYEE_POSITION EmployeePos = db.EMPLOYEE_POSITION.Find(id);
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true).OrderBy(x => x.NAME), "ID", "NAME", EmployeePos.EMP_CAT_ID);

            return View(EmployeePos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Position([Bind(Include = "ID,POS_NAME,POS_DESCR,IS_ACT,EMP_CAT_ID")] EMPLOYEE_POSITION eMPLOYEEpOSITION)
        {
            if (ModelState.IsValid)
            {
                var Employee = (from emp in db.EMPLOYEEs
                                where emp.EMP_POS_ID == eMPLOYEEpOSITION.ID && emp.STAT == true
                                select emp).Distinct();
                if ((eMPLOYEEpOSITION.IS_ACT == false && Employee.Count() == 0) || (eMPLOYEEpOSITION.IS_ACT == true))
                {
                    db.Entry(eMPLOYEEpOSITION).State = EntityState.Modified;
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
                        return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                        return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                    }
                    ViewBag.Notice = "Employee Position updated in system successfully!";
                    return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                else
                {
                    ViewBag.ErrorMessage = "Updates canot be done as Active Employees are attached to this Position.";
                    return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }

            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Delete_Position(int? id)
        {
            EMPLOYEE_POSITION eMPLOYEEpOSITION = db.EMPLOYEE_POSITION.Find(id);
            var employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_POS_ID == id && emp.STAT == true
                             select emp).Distinct();
            if (employees == null || employees.Count() == 0)
            {
                employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_POS_ID == id && emp.STAT == false
                             select emp).Distinct();
            }
            if (employees == null || employees.Count() == 0)
            {
                db.EMPLOYEE_POSITION.Remove(eMPLOYEEpOSITION);
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
                    return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Employee Position deleted fom system successfully!";
                return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Notice = "Employee Position cannot be deleted as active employees are attached to this Position!";
                return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }

        }


        public ActionResult Add_Department(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            //ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x => x.STAT == "Y").OrderBy(x => x.NAME), "ID", "NAME");
            var departments = (from dp in db.EMPLOYEE_DEPARTMENT
                             where dp.STAT == true
                             select new Models.EmployeeDepartment { DepartmentData = dp})
                            .OrderBy(x => x.DepartmentData.NAMES).ToList();
            ViewData["departments"] = departments;

            var inactive_departments = (from dp in db.EMPLOYEE_DEPARTMENT
                                        where dp.STAT == false
                                      select new Models.EmployeeDepartment { DepartmentData = dp})
                            .OrderBy(x => x.DepartmentData.NAMES).ToList();
            ViewData["inactive_departments"] = inactive_departments;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Department([Bind(Include = "ID,CODE,NAMES,STAT")] EMPLOYEE_DEPARTMENT eMPLOYEEdEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                var departments = (from dp in db.EMPLOYEE_DEPARTMENT
                                   where dp.STAT == true
                                   select new Models.EmployeeDepartment { DepartmentData = dp })
                            .OrderBy(x => x.DepartmentData.NAMES).ToList();
                ViewData["departments"] = departments;

                var inactive_departments = (from dp in db.EMPLOYEE_DEPARTMENT
                                            where dp.STAT == false
                                            select new Models.EmployeeDepartment { DepartmentData = dp })
                                .OrderBy(x => x.DepartmentData.NAMES).ToList();
                ViewData["inactive_departments"] = inactive_departments;

                db.EMPLOYEE_DEPARTMENT.Add(eMPLOYEEdEPARTMENT);
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
                    return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Employee Department added in system successfully!";
                return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Position", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Edit_Department(int? id)
        {
            EMPLOYEE_DEPARTMENT EmployeeDepart = db.EMPLOYEE_DEPARTMENT.Find(id);
            
            return View(EmployeeDepart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Department([Bind(Include = "ID,CODE,NAMES,STAT")] EMPLOYEE_DEPARTMENT eMPLOYEEdEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                var Employee = (from emp in db.EMPLOYEEs
                                where emp.EMP_DEPT_ID == eMPLOYEEdEPARTMENT.ID && emp.STAT == true
                                select emp).Distinct();
                if ((eMPLOYEEdEPARTMENT.STAT == false && Employee.Count() == 0) || (eMPLOYEEdEPARTMENT.STAT == true))
                {
                    db.Entry(eMPLOYEEdEPARTMENT).State = EntityState.Modified;
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
                        return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                        return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                    }
                    ViewBag.Notice = "Employee Department updated in system successfully!";
                    return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                else
                {
                    ViewBag.ErrorMessage = "Updates canot be done as Active Employees are attached to this Department.";
                    return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }

            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Delete_Department(int? id)
        {
            EMPLOYEE_DEPARTMENT eMPLOYEEdEPARTMENT = db.EMPLOYEE_DEPARTMENT.Find(id);
            var employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_DEPT_ID == id && emp.STAT == true
                             select emp).Distinct();
            if (employees == null || employees.Count() == 0)
            {
                employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_DEPT_ID == id && emp.STAT == false
                             select emp).Distinct();
            }
            if (employees == null || employees.Count() == 0)
            {
                db.EMPLOYEE_DEPARTMENT.Remove(eMPLOYEEdEPARTMENT);
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
                    return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Employee Department deleted fom system successfully!";
                return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Notice = "Employee Department cannot be deleted as active employees are attached to this Department.";
                return RedirectToAction("Add_Department", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }

        }


        public ActionResult Add_Grade(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var grades = (from gr in db.EMPLOYEE_GRADE
                               where gr.IS_ACT == true
                               select new Models.EmployeeGrade { GradeData = gr })
                            .OrderBy(x => x.GradeData.GRADE_NAME).ToList();
            ViewData["grades"] = grades;

            var inactive_grades = (from gr in db.EMPLOYEE_GRADE
                                   where gr.IS_ACT == false
                                   select new Models.EmployeeGrade { GradeData = gr })
                            .OrderBy(x => x.GradeData.GRADE_NAME).ToList();
            ViewData["inactive_grades"] = inactive_grades;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Grade([Bind(Include = "ID,GRADE_CODE,GRADE_NAME,DESCR,IS_ACT,PRIR,MAX_DILY_HRS,MAX_WKILY_HRS")] EMPLOYEE_GRADE eMPLOYEEgRADE)
        {
            if (ModelState.IsValid)
            {
                var grades = (from gr in db.EMPLOYEE_GRADE
                              where gr.IS_ACT == true
                              select new Models.EmployeeGrade { GradeData = gr })
                  .OrderBy(x => x.GradeData.GRADE_NAME).ToList();
                ViewData["grades"] = grades;

                var inactive_grades = (from gr in db.EMPLOYEE_GRADE
                                       where gr.IS_ACT == false
                                       select new Models.EmployeeGrade { GradeData = gr })
                                .OrderBy(x => x.GradeData.GRADE_NAME).ToList();
                ViewData["inactive_grades"] = inactive_grades;

                db.EMPLOYEE_GRADE.Add(eMPLOYEEgRADE);
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
                    return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Employee Grade added in system successfully!";
                return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Edit_Grade(int? id)
        {
            EMPLOYEE_GRADE EmployeeGrade = db.EMPLOYEE_GRADE.Find(id);

            return View(EmployeeGrade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Grade([Bind(Include = "ID,GRADE_CODE,GRADE_NAME,DESCR,IS_ACT,PRIR,MAX_DILY_HRS,MAX_WKILY_HRS")] EMPLOYEE_GRADE eMPLOYEEgRADE)
        {
            if (ModelState.IsValid)
            {
                var Employee = (from emp in db.EMPLOYEEs
                                where emp.EMP_GRADE_ID == eMPLOYEEgRADE.ID && emp.STAT == true
                                select emp).Distinct();
                if ((eMPLOYEEgRADE.IS_ACT == false && Employee.Count() == 0) || (eMPLOYEEgRADE.IS_ACT == true))
                {
                    db.Entry(eMPLOYEEgRADE).State = EntityState.Modified;
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
                        return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                        return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                    }
                    ViewBag.Notice = "Employee Grade updated in system successfully!";
                    return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                else
                {
                    ViewBag.ErrorMessage = "Updates canot be done as Active Employees are attached to this Grade.";
                    return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }

            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Delete_Grade(int? id)
        {
            EMPLOYEE_GRADE eMPLOYEEgRADE = db.EMPLOYEE_GRADE.Find(id);
            var employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_GRADE_ID == id && emp.STAT == true
                             select emp).Distinct();
            if (employees == null || employees.Count() == 0)
            {
                employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_GRADE_ID == id && emp.STAT == false
                             select emp).Distinct();
            }
            if (employees == null || employees.Count() == 0)
            {
                db.EMPLOYEE_GRADE.Remove(eMPLOYEEgRADE);
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
                    return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Employee Grade deleted fom system successfully!";
                return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Notice = "Employee Grade cannot be deleted as active employees are attached to this Grade.";
                return RedirectToAction("Add_Grade", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }

        }

        public ActionResult Add_Bank_Details(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var bank_details = (from bf in db.BANK_FIELD
                          where bf.STAT == true
                          select new Models.EmployeeBankDetail { BankFieldData = bf })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
            ViewData["bank_details"] = bank_details;

            var inactive_bank_details = (from bf in db.BANK_FIELD
                                         where bf.STAT == false
                                         select new Models.EmployeeBankDetail { BankFieldData = bf })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
            ViewData["inactive_bank_details"] = inactive_bank_details;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Bank_Details([Bind(Include = "ID,NAME,STAT")] BANK_FIELD bANKfIELD)
        {
            if (ModelState.IsValid)
            {
                var bank_details = (from bf in db.BANK_FIELD
                                    where bf.STAT == true
                                    select new Models.EmployeeBankDetail { BankFieldData = bf })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
                ViewData["bank_details"] = bank_details;

                var inactive_bank_details = (from bf in db.BANK_FIELD
                                             where bf.STAT == false
                                             select new Models.EmployeeBankDetail { BankFieldData = bf })
                                .OrderBy(x => x.BankFieldData.NAME).ToList();
                ViewData["inactive_bank_details"] = inactive_bank_details;

                db.BANK_FIELD.Add(bANKfIELD);
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
                    return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Bank Field added in system successfully!";
                return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Edit_Bank_Details(int? id)
        {
            BANK_FIELD BankField = db.BANK_FIELD.Find(id);

            return View(BankField);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Bank_Details([Bind(Include = "ID,NAME,STAT")] BANK_FIELD bANKfIELD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bANKfIELD).State = EntityState.Modified;
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
                    return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Bank Field updated in system successfully!";
                return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });

            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Delete_Bank_Details(int? id)
        {
            BANK_FIELD bANKfIELD = db.BANK_FIELD.Find(id);
            var employees = (from emp in db.EMPLOYEEs
                             join bd in db.EMPLOYEE_BANK_DETAIL on emp.ID equals bd.EMP_ID
                             join bf in db.BANK_FIELD on bd.BANK_FLD_ID equals bf.ID
                             where bf.ID == id && emp.STAT == true
                             select emp).Distinct();
            if (employees == null || employees.Count() == 0)
            {
                db.BANK_FIELD.Remove(bANKfIELD);
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
                    return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Bank Field deleted fom system successfully!";
                return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Notice = "Bank Field cannot be deleted as active employees are attached to this Bank Field.";
                return RedirectToAction("Add_Bank_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }

        }

        public ActionResult Add_Additional_Details(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var additional_details = (from af in db.EMPLOYEE_ADDITIONAL_FIELD
                                where af.STAT == true
                                select new Models.EmployeeAdditionalDetail { AdditionalFieldData = af })
                            .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
            ViewData["additional_details"] = additional_details;

            var inactive_additional_details = (from af in db.EMPLOYEE_ADDITIONAL_FIELD
                                               where af.STAT == false
                                               select new Models.EmployeeAdditionalDetail { AdditionalFieldData = af })
                            .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
            ViewData["inactive_additional_details"] = inactive_additional_details;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Additional_Details([Bind(Include = "ID,NAME,STAT")] EMPLOYEE_ADDITIONAL_FIELD aDDITIONALfIELD)
        {
            if (ModelState.IsValid)
            {
                var additional_details = (from af in db.EMPLOYEE_ADDITIONAL_FIELD
                                          where af.STAT == true
                                          select new Models.EmployeeAdditionalDetail { AdditionalFieldData = af })
                            .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
                ViewData["additional_details"] = additional_details;

                var inactive_additional_details = (from af in db.EMPLOYEE_ADDITIONAL_FIELD
                                                   where af.STAT == false
                                                   select new Models.EmployeeAdditionalDetail { AdditionalFieldData = af })
                                .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
                ViewData["inactive_additional_details"] = inactive_additional_details;

                db.EMPLOYEE_ADDITIONAL_FIELD.Add(aDDITIONALfIELD);
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
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Addional Field added in system successfully!";
                return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Edit_Additional_Details(int? id)
        {
            EMPLOYEE_ADDITIONAL_FIELD aDDITIONALfIELD = db.EMPLOYEE_ADDITIONAL_FIELD.Find(id);

            return View(aDDITIONALfIELD);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Additional_Details([Bind(Include = "ID,NAME,STAT")] EMPLOYEE_ADDITIONAL_FIELD aDDITIONALfIELD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aDDITIONALfIELD).State = EntityState.Modified;
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
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Additional Field updated in system successfully!";
                return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });

            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Delete_Additional_Details(int? id)
        {
            EMPLOYEE_ADDITIONAL_FIELD aDDITIONALfIELD = db.EMPLOYEE_ADDITIONAL_FIELD.Find(id);
            var employees = (from emp in db.EMPLOYEEs
                             join ad in db.EMPLOYEE_ADDITIONAL_DETAIL on emp.ID equals ad.EMP_ID
                             join af in db.EMPLOYEE_ADDITIONAL_FIELD on ad.ADDL_FLD_ID equals af.ID
                             where af.ID == id && emp.STAT == true
                             select emp).Distinct();
            if (employees == null || employees.Count() == 0)
            {
                db.EMPLOYEE_ADDITIONAL_FIELD.Remove(aDDITIONALfIELD);
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
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Additional Field deleted fom system successfully!";
                return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Notice = "Addional Field cannot be deleted as active employees are attached to this Additional Field.";
                return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }

        }

        public ActionResult Employee_Management()
        {

            return View();
        }

        public ActionResult Admission1()
        {
            DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
            ViewBag.ReturnDate = PDate.ToShortDateString();
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x=>x.STAT == true).ToList(), "ID", "NAMES");
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true).ToList(), "ID", "NAME");
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true).ToList(), "ID", "POS_NAME");
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true).ToList(), "ID", "GRADE_NAME");

            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", "99");
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies.Where(o => o.NTLTY != " ").ToList(), "ID", "NTLTY", "99");

            var configValue = (from C in db.CONFIGURATIONs
                               where C.CONFIG_KEY == "EmployeeNumberAutoIncrement"
                               select new { CONFIG_VALUE = C.CONFIG_VAL }).FirstOrDefault();
            int NewEmployeeNumberNum = Convert.ToInt32(configValue.CONFIG_VALUE.ToString()) + 1;
            ViewBag.NewEmployeeNumber = NewEmployeeNumberNum.ToString();
            return View();
        }

        // POST: Student/Admission1
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Admission1([Bind(Include = "ID,RPTG_MGR_ID,EMP_CAT_ID,EMP_NUM,JOINING_DATE,FIRST_NAME,MID_NAME,LAST_NAME,GNDR,JOB_TIL,EMP_POS_ID,EMP_DEPT_ID,EMP_GRADE_ID,QUAL,EXPNC_DETL,EXPNC_YEAR,EXPNC_MONTH,STAT,STAT_DESCR,DOB,MARITAL_STAT,CHLD_CNT,FTHR_NAME,MTHR_NAME,HUSBND_NAME,BLOOD_GRP,NTLTY_ID,HOME_ADDR_LINE1,HOME_ADDR_LINE2,HOME_CITY,HOME_STATE,HOME_CTRY_ID,HOME_PIN_CODE,OFF_ADDR_LINE1,OFF_ADDR_LINE2,OFF_CITY,OFF_STATE,OFF_CTRY_ID,OFF_PIN_CODE,OFF_PH1,OFF_PH2,MOBL_PH,HOME_PH,EML,FAX,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,CREATED_AT,UPDATED_AT,PHTO_FILE_SIZE,USRID")] EMPLOYEE eMPLOYEE)
        {
            ViewBag.NewEmployeeNumberNum = eMPLOYEE.EMP_NUM;
            DateTime PDate = Convert.ToDateTime(eMPLOYEE.JOINING_DATE);
            ViewBag.ReturnDate = PDate.ToShortDateString();
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies.Where(o => o.NTLTY != " ").ToList(), "ID", "NTLTY", eMPLOYEE.NTLTY_ID);
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true), "ID", "NAME", eMPLOYEE.EMP_DEPT_ID);
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true), "ID", "NAME", eMPLOYEE.EMP_CAT_ID);
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true), "ID", "POS_NAME", eMPLOYEE.EMP_POS_ID);
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true), "ID", "GRADE_NAME", eMPLOYEE.EMP_GRADE_ID);

            int UserId = Convert.ToInt32(this.Session["UserId"]);
            eMPLOYEE.USRID = UserId;
            int Emp_Num_Int = Convert.ToInt32(eMPLOYEE.EMP_NUM);

            if (ModelState.IsValid)
            {
                /////Picture Upload Code
                string FileName = null;
                SuccessModel viewModel = new SuccessModel();
                if (Request.Files.Count == 1 && Request.Files[0].FileName != "")
                {
                    var name = Request.Files[0].FileName;
                    var size = Request.Files[0].ContentLength;
                    var type = Request.Files[0].ContentType;
                    FileName = name;
                    int PhotoId = 0;
                    PhotoId = HandleUpload(Request.Files[0].InputStream, name, size, type, PhotoId);

                    eMPLOYEE.IMAGE_DOCUMENTS_ID = PhotoId;
                    eMPLOYEE.PHTO_FILENAME = null;
                }
                ////End to Picture Upload Code

                eMPLOYEE.STAT = true;
                eMPLOYEE.STAT_DESCR = "Ative Employee";
                eMPLOYEE.CREATED_AT = System.DateTime.Now;
                eMPLOYEE.UPDATED_AT = System.DateTime.Now;
                eMPLOYEE.EMP_NUM = string.Concat("E", eMPLOYEE.EMP_NUM);
                db.EMPLOYEEs.Add(eMPLOYEE);
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
                    return View(eMPLOYEE);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(eMPLOYEE);
                }

                var result = from u in db.CONFIGURATIONs where (u.CONFIG_KEY == "EmployeeNumberAutoIncrement") select u;
                if (result.Count() != 0)
                {
                    var dbConfig = result.First();

                    dbConfig.CONFIG_VAL = Emp_Num_Int.ToString();
                    db.SaveChanges();
                }

                string FullName = string.Concat(eMPLOYEE.FIRST_NAME, eMPLOYEE.MID_NAME, eMPLOYEE.LAST_NAME);
                var StdUser = new USER() { USRNAME = FullName, FIRST_NAME = eMPLOYEE.FIRST_NAME, LAST_NAME = eMPLOYEE.LAST_NAME, EML = eMPLOYEE.EML, ADMIN_IND = false, STDNT_IND = false, EMP_IND = true, PARNT_IND = false, HASHED_PSWRD = string.Concat(eMPLOYEE.EMP_NUM, 123), SALT = "N", RST_PSWRD_CODE = null, RST_PSWRD_CODE_UNTL = null, CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now };
                db.USERS.Add(StdUser);
                db.SaveChanges();
                foreach (var entity in db.USERS_ACCESS.Select(s => new { s.USRS_ID, s.LIST_ITEM, s.LVL_1_MENU, s.LVL_2_MENU, s.CTL, s.ACTN, s.IS_ACCBLE }).Distinct().Where(a => a.USRS_ID.Equals(2)).ToList())
                {
                    var UserAccess = new USERS_ACCESS() { USRS_ID = StdUser.ID, LIST_ITEM = entity.LIST_ITEM, LVL_1_MENU = entity.LVL_1_MENU, LVL_2_MENU = entity.LVL_2_MENU, CTL = entity.CTL, ACTN = entity.ACTN, IS_ACCBLE = entity.IS_ACCBLE };
                    db.USERS_ACCESS.Add(UserAccess);
                    db.SaveChanges();
                }

                var EmpResult = from u in db.EMPLOYEEs where (u.EMP_NUM == eMPLOYEE.EMP_NUM) select u;
                if (EmpResult.Count() != 0)
                {
                    EmpResult.First().USRID = StdUser.ID;
                    db.SaveChanges();
                }
                // some code 
                TempData["alertMessage"] = string.Concat("Employee Admission Done. Website User ID :", FullName, "     Password :", string.Concat(eMPLOYEE.EMP_NUM, 123), ". Please change passord after login.");
                return RedirectToAction("Admission2", "Employee", new { Emp_id = eMPLOYEE.ID });
            }
            return View(eMPLOYEE);
        }


        public ActionResult Admission2(int? Emp_id)
        {
            DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
            ViewBag.ReturnDate = PDate.ToShortDateString();

            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", "99");
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", "99");
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies.Where(o => o.NTLTY != " ").ToList(), "ID", "NTLTY", "99");
            EMPLOYEE NewEmp = db.EMPLOYEEs.Find(Emp_id);
            
            return View(NewEmp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Admission2([Bind(Include = "ID,RPTG_MGR_ID,EMP_CAT_ID,EMP_NUM,JOINING_DATE,FIRST_NAME,MID_NAME,LAST_NAME,GNDR,JOB_TIL,EMP_POS_ID,EMP_DEPT_ID,EMP_GRADE_ID,QUAL,EXPNC_DETL,EXPNC_YEAR,EXPNC_MONTH,STAT,STAT_DESCR,DOB,MARITAL_STAT,CHLD_CNT,FTHR_NAME,MTHR_NAME,HUSBND_NAME,BLOOD_GRP,NTLTY_ID,HOME_ADDR_LINE1,HOME_ADDR_LINE2,HOME_CITY,HOME_STATE,HOME_CTRY_ID,HOME_PIN_CODE,OFF_ADDR_LINE1,OFF_ADDR_LINE2,OFF_CITY,OFF_STATE,OFF_CTRY_ID,OFF_PIN_CODE,OFF_PH1,OFF_PH2,MOBL_PH,HOME_PH,EML,FAX,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,CREATED_AT,UPDATED_AT,PHTO_FILE_SIZE,USRID")] EMPLOYEE eMPLOYEE)
        {
            EMPLOYEE EmpToUpdate = db.EMPLOYEEs.Find(eMPLOYEE.ID);
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.HOME_CTRY_ID);
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.OFF_CTRY_ID);

            if (ModelState.IsValid)
            {
                EmpToUpdate.UPDATED_AT = System.DateTime.Now;
                EmpToUpdate.HOME_ADDR_LINE1 = eMPLOYEE.HOME_ADDR_LINE1;
                EmpToUpdate.HOME_ADDR_LINE2 = eMPLOYEE.HOME_ADDR_LINE2;
                EmpToUpdate.HOME_CITY = eMPLOYEE.HOME_CITY;
                EmpToUpdate.HOME_STATE = eMPLOYEE.HOME_STATE;
                EmpToUpdate.HOME_CTRY_ID = eMPLOYEE.HOME_CTRY_ID;
                EmpToUpdate.HOME_PIN_CODE = eMPLOYEE.HOME_PIN_CODE;
                EmpToUpdate.OFF_ADDR_LINE1 = eMPLOYEE.OFF_ADDR_LINE1;
                EmpToUpdate.OFF_ADDR_LINE2 = eMPLOYEE.OFF_ADDR_LINE2;
                EmpToUpdate.OFF_CITY = eMPLOYEE.OFF_CITY;
                EmpToUpdate.OFF_STATE = eMPLOYEE.OFF_STATE;
                EmpToUpdate.OFF_CTRY_ID = eMPLOYEE.OFF_CTRY_ID;
                EmpToUpdate.OFF_PIN_CODE = eMPLOYEE.OFF_PIN_CODE;
                EmpToUpdate.OFF_PH1 = eMPLOYEE.OFF_PH1;
                EmpToUpdate.OFF_PH2 = eMPLOYEE.OFF_PH2;
                EmpToUpdate.MOBL_PH = eMPLOYEE.MOBL_PH;
                EmpToUpdate.HOME_PH = eMPLOYEE.HOME_PH;
                EmpToUpdate.FAX = eMPLOYEE.FAX;
                db.Entry(EmpToUpdate).State = EntityState.Modified;
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
                    return View(eMPLOYEE);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(eMPLOYEE);
                } 
                return RedirectToAction("Admission3", "Employee", new { Emp_id = EmpToUpdate.ID });
            }
            return View(eMPLOYEE);
        }

        public ActionResult Admission3(int? Emp_id)
        {
            EMPLOYEE NewEmp = db.EMPLOYEEs.Find(Emp_id);

            var bank_fields = (from bf in db.BANK_FIELD
                               where bf.STAT == true
                               select new Models.EmployeeBankFieldValue { BankFieldData = bf, EMPLOYEE_ID = Emp_id,BANK_FIELD_ID= bf.ID, FIELD_VALUE = "" })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
            ViewData["bank_fields"] = bank_fields;

            if (bank_fields ==null)
            {
                return RedirectToAction("Admission3_1", "Employee", new { Emp_id = Emp_id });
            }

            return View(bank_fields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Admission3(IEnumerable<SFSAcademy.Models.EmployeeBankFieldValue> EmpDet)
        {
            if (ModelState.IsValid)
            {
                foreach(var item in EmpDet)
                {
                    var EmpBankDet = new EMPLOYEE_BANK_DETAIL() { EMP_ID = item.EMPLOYEE_ID, BANK_FLD_ID = item.BANK_FIELD_ID, BANK_INFO = item.FIELD_VALUE };
                    db.EMPLOYEE_BANK_DETAIL.Add(EmpBankDet);
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
                        return View(EmpDet);
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                        return View(EmpDet);
                    }                   
                }
                return RedirectToAction("Admission3_1", "Employee", new { Emp_id = EmpDet.FirstOrDefault().EMPLOYEE_ID });

            }
            return View(EmpDet);
        }

        public ActionResult Admission3_1(int? Emp_id, string edit_request)
        {
            EMPLOYEE NewEmp = db.EMPLOYEEs.Find(Emp_id);

            var employee_additional_details = (from af in db.EMPLOYEE_ADDITIONAL_FIELD
                               where af.STAT == true
                               select new Models.EmployeeAdditionalDetailValue { AdditionalFieldData = af, EMPLOYEE_ID = Emp_id, ADDITIONAL_DETAIL_ID = af.ID, ADDITIONAL_DETAIL_VALUE = "" })
                            .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
            ViewData["employee_additional_details"] = employee_additional_details;

            if (employee_additional_details == null)
            {
                return RedirectToAction("Edit_Privilege", "USERs", new { id = NewEmp.USRID, Calling_Method = "Employee" });
            }
            ViewBag.edit_request = edit_request;
            return View(employee_additional_details);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Admission3_1(IEnumerable<SFSAcademy.Models.EmployeeAdditionalDetailValue> EmpAddDet, string edit_request)
        {
            if (ModelState.IsValid)
            {
                EMPLOYEE NewEmp = db.EMPLOYEEs.Find(EmpAddDet.FirstOrDefault().EMPLOYEE_ID);
                foreach (var item in EmpAddDet)
                {
                    var EmpAddDetNew = new EMPLOYEE_ADDITIONAL_DETAIL() { EMP_ID = item.EMPLOYEE_ID, ADDL_FLD_ID = item.ADDITIONAL_DETAIL_ID, ADDL_INFO = item.ADDITIONAL_DETAIL_VALUE };
                    db.EMPLOYEE_ADDITIONAL_DETAIL.Add(EmpAddDetNew);
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
                        return View(EmpAddDet);
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                        return View(EmpAddDet);
                    }
                }
                if (edit_request == null)
                {
                    return RedirectToAction("Edit_Privilege", "USERs", new { id = NewEmp.USRID, Calling_Method = "Employee" });
                }
                else
                {
                    return RedirectToAction("Profiles", "Employee", new { Emp_id = NewEmp.ID });
                }

            }
            return View(EmpAddDet);
        }

        public ActionResult Admission4(int Emp_id, int? Reporting_Mn_Id)
        {
            EMPLOYEE Employee = db.EMPLOYEEs.Find(Emp_id);
            ViewData["Employee"] = Employee;

            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x=>x.STAT==true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.STAT == true
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg), Employee_Id= Emp_id }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            if(Reporting_Mn_Id != null)
            {
                var Reporting_Manager = (from rm in db.EMPLOYEEs
                                         where rm.ID == Reporting_Mn_Id
                                         select rm).Distinct().ToList();
                ViewData["Reporting_Manager"] = Reporting_Manager;
            }
            else
            {
                var Reporting_Manager = (from rm in db.EMPLOYEEs
                                         join emp in db.EMPLOYEEs on rm.ID equals emp.RPTG_MGR_ID
                                         where emp.ID == Emp_id
                                         select rm).Distinct().ToList();
                ViewData["Reporting_Manager"] = Reporting_Manager;
            }

            return View(EmployeeDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Admission4(IEnumerable<SFSAcademy.Models.Employee> EmpRepManager)
        {
            int EmpId = EmpRepManager.FirstOrDefault().Employee_Id;
            int RpMgr_Id = EmpRepManager.FirstOrDefault().Reporting_Manager_Id;
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.STAT == true
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg), Employee_Id = EmpId, Reporting_Manager_Id = RpMgr_Id }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            var Reporting_Manager = (from rm in db.EMPLOYEEs
                                     where rm.ID == RpMgr_Id
                                     select rm).Distinct().ToList();
            ViewData["Reporting_Manager"] = Reporting_Manager;

            if (ModelState.IsValid)
            {
                EMPLOYEE EmpToUpdate = db.EMPLOYEEs.Find(EmpRepManager.FirstOrDefault().Employee_Id);
                EmpToUpdate.UPDATED_AT = System.DateTime.Now;
                EmpToUpdate.RPTG_MGR_ID = EmpRepManager.FirstOrDefault().Reporting_Manager_Id;
                db.Entry(EmpToUpdate).State = EntityState.Modified;
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
                    return View(EmployeeDetail);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(EmployeeDetail);
                }
                return RedirectToAction("Manage_Payroll", "Payroll", new { id = EmpToUpdate.ID });
            }
            return View(EmployeeDetail);
        }

        public ActionResult Profiles(int? id, string Notice)
        {
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            ViewData["Employee"] = Employee;
            int new_reminder_count = 0;
            ViewBag.Notice = Notice;
            var EventReminder = (from EV in db.EVENTs
                                 join EDE in db.EMPLOYEE_DEPARTMENT_EVENT on EV.ID equals EDE.EV_ID
                                 join ED in db.EMPLOYEE_DEPARTMENT on EDE.EMP_DEPT_ID equals ED.ID
                                 join EP in db.EMPLOYEEs on ED.ID equals EP.EMP_DEPT_ID
                                 where EP.USRID == UserId && EV.IS_DUE == true
                                 select new { EVENT_ID = EV.ID }).ToList();

            foreach (var entity in EventReminder.ToList())
            {
                new_reminder_count = new_reminder_count + 1;
            }
            ViewBag.new_reminder_count = new_reminder_count;
            ViewBag.gender = Employee.GNDR == "M" ? "Male" : "Female";
            ViewBag.status = Employee.STAT == true ? "Active" : "Inactive";
            if(Employee.RPTG_MGR_ID != null)
            {
                EMPLOYEE reporting_manager = db.EMPLOYEEs.Find(Employee.RPTG_MGR_ID);
                ViewBag.reporting_manager = string.Concat(reporting_manager.FIRST_NAME, " ", reporting_manager.MID_NAME, " ", reporting_manager.LAST_NAME);
            }

            int exp_years = Convert.ToInt32(Employee.EXPNC_YEAR);
            int exp_months = Convert.ToInt32(Employee.EXPNC_MONTH);
            DateTime date = System.DateTime.Now;
            int total_current_exp_days = 0;
            if (Employee.JOINING_DATE != null)
            {
                TimeSpan span = date.Subtract(Convert.ToDateTime(Employee.JOINING_DATE));
                total_current_exp_days = (int)span.TotalDays;
            }
            int current_years = (total_current_exp_days / 365);
            int rem = total_current_exp_days % 365;
            int current_months = rem / 30;
            int total_month = exp_months + current_months;
            int year = total_month / 12;
            int month = total_month % 12;
            ViewBag.total_years = exp_years + current_years + year;
            ViewBag.total_months = month; 
            return View(Employee);
        }

        public ActionResult Search()
        {
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.STAT == true
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg)}).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            return View(EmployeeDetail);
        }


        public ActionResult Profile_General(int? id)
        {
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            //EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            //ViewData["Employee"] = Employee;
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.ID == id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg) }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            ViewData["Employee"] = EmployeeDetail;
            int new_reminder_count = 0;
            var EventReminder = (from EV in db.EVENTs
                                 join EDE in db.EMPLOYEE_DEPARTMENT_EVENT on EV.ID equals EDE.EV_ID
                                 join ED in db.EMPLOYEE_DEPARTMENT on EDE.EMP_DEPT_ID equals ED.ID
                                 join EP in db.EMPLOYEEs on ED.ID equals EP.EMP_DEPT_ID
                                 where EP.USRID == UserId && EV.IS_DUE == true
                                 select new { EVENT_ID = EV.ID }).ToList();

            foreach (var entity in EventReminder.ToList())
            {
                new_reminder_count = new_reminder_count + 1;
            }
            ViewBag.new_reminder_count = new_reminder_count;
            ViewBag.gender = EmployeeDetail.FirstOrDefault().EmployeeData.GNDR == "M" ? "Male" : "Female";
            ViewBag.status = EmployeeDetail.FirstOrDefault().EmployeeData.STAT == true ? "Active" : "Inactive";
            if (EmployeeDetail.FirstOrDefault().EmployeeData.RPTG_MGR_ID != null)
            {
                EMPLOYEE reporting_manager_val = db.EMPLOYEEs.Find(EmployeeDetail.FirstOrDefault().EmployeeData.RPTG_MGR_ID);
                //ViewBag.reporting_manager = string.Concat(reporting_manager_val.FIRST_NAME, " ", reporting_manager_val.MID_NAME, " ", reporting_manager_val.LAST_NAME);
                ViewData["Reporting_Manager"] = reporting_manager_val;
            }

            int exp_years = Convert.ToInt32(EmployeeDetail.FirstOrDefault().EmployeeData.EXPNC_YEAR);
            int exp_months = Convert.ToInt32(EmployeeDetail.FirstOrDefault().EmployeeData.EXPNC_MONTH);
            DateTime date = System.DateTime.Now;
            int total_current_exp_days = 0;
            if (EmployeeDetail.FirstOrDefault().EmployeeData.JOINING_DATE != null)
            {
                TimeSpan span = date.Subtract(Convert.ToDateTime(EmployeeDetail.FirstOrDefault().EmployeeData.JOINING_DATE));
                total_current_exp_days = (int)span.TotalDays;
            }
            int current_years = (total_current_exp_days / 365);
            int rem = total_current_exp_days % 365;
            int current_months = rem / 30;
            int total_month = exp_months + current_months;
            int year = total_month / 12;
            int month = total_month % 12;
            ViewBag.total_years = exp_years + current_years + year;
            ViewBag.total_months = month;
            return PartialView("_General");
        }

        public ActionResult Profile_Personal(int? id)
        {
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  join nlty in db.COUNTRies on emp.NTLTY_ID equals nlty.ID into gnlty
                                  from subgnlty in gnlty.DefaultIfEmpty()
                                  where emp.ID == id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg),NationalityData = (subgnlty == null ? null : subgnlty) }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            ViewData["Employee"] = EmployeeDetail;
            return PartialView("_Personal");
        }

        public ActionResult Profile_Address(int? id)
        {
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  join nlty in db.COUNTRies on emp.NTLTY_ID equals nlty.ID into gnlty
                                  from subgnlty in gnlty.DefaultIfEmpty()
                                  where emp.ID == id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg), NationalityData = (subgnlty == null ? null : subgnlty) }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            ViewData["Employee"] = EmployeeDetail;
            COUNTRY home_country = db.COUNTRies.Find(EmployeeDetail.FirstOrDefault().EmployeeData.HOME_CTRY_ID);
            ViewData["home_country"] = home_country;
            COUNTRY office_country = db.COUNTRies.Find(EmployeeDetail.FirstOrDefault().EmployeeData.OFF_CTRY_ID);
            ViewData["office_country"] = office_country;
            return PartialView("_Address");
        }

        public ActionResult Profile_Contact(int? id)
        {
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            ViewData["Employee"] = Employee;
            return PartialView("_Contact");
        }

        public ActionResult Profile_Bank_Details(int? id)
        {
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            ViewData["Employee"] = Employee;
            var bank_details = (from emp in db.EMPLOYEEs
                                join ebd in db.EMPLOYEE_BANK_DETAIL on emp.ID equals ebd.EMP_ID into gebd
                                from subgebd in gebd.DefaultIfEmpty()
                                join bf in db.BANK_FIELD on subgebd.BANK_FLD_ID equals bf.ID into gbf
                                from subgbf in gbf.DefaultIfEmpty()
                               where emp.ID == id
                               select new SFSAcademy.Models.EmployeeBankDetail { EmployeedData = emp, BankDetailData = (subgebd == null ? null : subgebd), BankFieldData = (subgbf == null ? null : subgbf) })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
            ViewData["bank_details"] = bank_details;
            return PartialView("_Bank_Details");
        }

        public ActionResult Profile_Additional_Details(int? id)
        {
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            ViewData["Employee"] = Employee;
            var additional_details = (from emp in db.EMPLOYEEs
                                join ead in db.EMPLOYEE_ADDITIONAL_DETAIL on emp.ID equals ead.EMP_ID into gead
                                from subgead in gead.DefaultIfEmpty()
                                join af in db.EMPLOYEE_ADDITIONAL_FIELD on subgead.ADDL_FLD_ID equals af.ID into gaf
                                from subgaf in gaf.DefaultIfEmpty()
                                where emp.ID == id
                                select new SFSAcademy.Models.EmployeeAdditionalDetail { EmployeedData = emp, AdditionalDetailData = (subgead == null ? null : subgead), AdditionalFieldData = (subgaf == null ? null : subgaf) })
                            .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
            ViewData["additional_details"] = additional_details;
            return PartialView("_Additional_Details");
        }

        public ActionResult Profile_Payroll_Details(int? id)
        {
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            ViewData["Employee"] = Employee;
            var payroll_details = (from emp in db.EMPLOYEEs
                                      join est in db.EMPLOYEE_SALARY_STRUCTURE on emp.ID equals est.EMP_ID into gest
                                      from subgest in gest.DefaultIfEmpty()
                                      join pc in db.PAYROLL_CATEGORY on subgest.PYRL_CAT_ID equals pc.ID into gpc
                                      from subgpc in gpc.DefaultIfEmpty()
                                      where emp.ID == id
                                      select new SFSAcademy.Models.EmployeePayroll { EmployeeData = emp, PayrollCatData = (subgpc == null ? null : subgpc), SalaryStructureData = (subgest == null ? null : subgest) })
                            .OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["payroll_details"] = payroll_details;
            return PartialView("_Payroll_Details");
        }

        public ActionResult View_Payslip(int? id)
        {
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            ViewData["Employee"] = Employee;

            var querysalary_dates = (from emp in db.EMPLOYEEs
                                join mps in db.MONTHLY_PAYSLIP.Where(x => x.IS_APPR == true) on emp.ID equals mps.EMP_ID 
                                where emp.ID == id
                                select new { mps.SAL_DATE }).OrderBy(x => x.SAL_DATE).Distinct().ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in querysalary_dates)
            {
                string SalaryDate = Convert.ToDateTime(item.SAL_DATE).ToString("MMM-yyyy");
                var result = new SelectListItem();
                result.Text = SalaryDate;
                result.Value = item.SAL_DATE.ToString();
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Month and Year" });
            ViewBag.SALARY_DATE = options;
            return PartialView("_Select_Dates");
        }

        public ActionResult Update_Monthly_Payslip(int? emp_id, DateTime salary_date)
        {
            EMPLOYEE Employee = db.EMPLOYEEs.Find(emp_id);
            ViewData["Employee"] = Employee;
            ViewBag.salary_dates = salary_date;
            if(salary_date == null)
            {
                return PartialView("_Payslip_View");
            }
            return PartialView("_Payslip_View");
        }

        public ActionResult Profile_pdf(int? id)
        {
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  join nlty in db.COUNTRies on emp.NTLTY_ID equals nlty.ID into gnlty
                                  from subgnlty in gnlty.DefaultIfEmpty()
                                  where emp.ID == id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg), NationalityData = (subgnlty == null ? null : subgnlty) }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            ViewData["Employee"] = EmployeeDetail;
            int new_reminder_count = 0;
            var EventReminder = (from EV in db.EVENTs
                                 join EDE in db.EMPLOYEE_DEPARTMENT_EVENT on EV.ID equals EDE.EV_ID
                                 join ED in db.EMPLOYEE_DEPARTMENT on EDE.EMP_DEPT_ID equals ED.ID
                                 join EP in db.EMPLOYEEs on ED.ID equals EP.EMP_DEPT_ID
                                 where EP.USRID == UserId && EV.IS_DUE == true
                                 select new { EVENT_ID = EV.ID }).ToList();

            foreach (var entity in EventReminder.ToList())
            {
                new_reminder_count = new_reminder_count + 1;
            }
            ViewBag.new_reminder_count = new_reminder_count;
            ViewBag.gender = EmployeeDetail.FirstOrDefault().EmployeeData.GNDR == "M" ? "Male" : "Female";
            ViewBag.status = EmployeeDetail.FirstOrDefault().EmployeeData.STAT == true ? "Active" : "Inactive";
            if (EmployeeDetail.FirstOrDefault().EmployeeData.RPTG_MGR_ID != null)
            {
                EMPLOYEE reporting_manager_val = db.EMPLOYEEs.Find(EmployeeDetail.FirstOrDefault().EmployeeData.RPTG_MGR_ID);
                ViewData["Reporting_Manager"] = reporting_manager_val;
            }

            int exp_years = Convert.ToInt32(EmployeeDetail.FirstOrDefault().EmployeeData.EXPNC_YEAR);
            int exp_months = Convert.ToInt32(EmployeeDetail.FirstOrDefault().EmployeeData.EXPNC_MONTH);
            DateTime date = System.DateTime.Now;
            int total_current_exp_days = 0;
            if (EmployeeDetail.FirstOrDefault().EmployeeData.JOINING_DATE != null)
            {
                TimeSpan span = date.Subtract(Convert.ToDateTime(EmployeeDetail.FirstOrDefault().EmployeeData.JOINING_DATE));
                total_current_exp_days = (int)span.TotalDays;
            }
            int current_years = (total_current_exp_days / 365);
            int rem = total_current_exp_days % 365;
            int current_months = rem / 30;
            int total_month = exp_months + current_months;
            int year = total_month / 12;
            int month = total_month % 12;
            ViewBag.total_years = exp_years + current_years + year;
            ViewBag.total_months = month;
            COUNTRY home_country = db.COUNTRies.Find(EmployeeDetail.FirstOrDefault().EmployeeData.HOME_CTRY_ID);
            ViewData["home_country"] = home_country;
            COUNTRY office_country = db.COUNTRies.Find(EmployeeDetail.FirstOrDefault().EmployeeData.OFF_CTRY_ID);
            ViewData["office_country"] = office_country;
            var bank_details = (from emp in db.EMPLOYEEs
                                join ebd in db.EMPLOYEE_BANK_DETAIL on emp.ID equals ebd.EMP_ID into gebd
                                from subgebd in gebd.DefaultIfEmpty()
                                join bf in db.BANK_FIELD on subgebd.BANK_FLD_ID equals bf.ID into gbf
                                from subgbf in gbf.DefaultIfEmpty()
                                where emp.ID == id
                                select new SFSAcademy.Models.EmployeeBankDetail { EmployeedData = emp, BankDetailData = (subgebd == null ? null : subgebd), BankFieldData = (subgbf == null ? null : subgbf) })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
            ViewData["bank_details"] = bank_details;
            var additional_details = (from emp in db.EMPLOYEEs
                                      join ead in db.EMPLOYEE_ADDITIONAL_DETAIL on emp.ID equals ead.EMP_ID into gead
                                      from subgead in gead.DefaultIfEmpty()
                                      join af in db.EMPLOYEE_ADDITIONAL_FIELD on subgead.ADDL_FLD_ID equals af.ID into gaf
                                      from subgaf in gaf.DefaultIfEmpty()
                                      where emp.ID == id
                                      select new SFSAcademy.Models.EmployeeAdditionalDetail { EmployeedData = emp, AdditionalDetailData = (subgead == null ? null : subgead), AdditionalFieldData = (subgaf == null ? null : subgaf) })
                            .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
            ViewData["additional_details"] = additional_details;

            return View();
        }


        public ActionResult View_All()
        {
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x=>x.STAT==true), "ID", "NAMES").Distinct().ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Employee Department" });
            ViewBag.departments = options;
            return View();
        }

        public ActionResult Employees_List(int? id)
        {
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  join nlty in db.COUNTRies on emp.NTLTY_ID equals nlty.ID into gnlty
                                  from subgnlty in gnlty.DefaultIfEmpty()
                                  where subged.ID == id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg), NationalityData = (subgnlty == null ? null : subgnlty) }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();

            ViewData["employees"] = EmployeeDetail;
            return PartialView("_employee_view_all_list");
        }

        public ActionResult Advanced_Search(string sortOrder, string currentFilter, string searchString, int? page, string currentFilter2, string EmployeeNumber, string currentFilter3, string MaritalStatus, int? currentFilter4, int? Nationality_Id, int? currentFilter5, int? Employee_Category_Id, string currentFilter6, string EmployeeGender, int? currentFilter7, int? Employee_Department_Id, int? currentFilter8, int? Employee_Grade_Id, int? currentFilter9, int? Employee_Position_Id, string currentFilter10, string BloodGroup, string currentFilter11, string EmployeeJoinFromDate, string currentFilter12, string EmployeeJoinToDate, string currentFilter13, string Status)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.NameSortParm2 = sortOrder == "LName" ? "name_desc_2" : "LName";
            ViewBag.NameSortParm3 = sortOrder == "EmpNum" ? "name_desc_3" : "EmpNum";
            ViewBag.DateSortParm = sortOrder == "DOB" ? "date_desc" : "DOB";
            ViewBag.DateSortParm2 = sortOrder == "DOJ" ? "date_desc2" : "DOJ";

            if (!string.IsNullOrEmpty(searchString))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            if (!string.IsNullOrEmpty(EmployeeNumber)) { page = 1; }
            else { EmployeeNumber = currentFilter2; }
            ViewBag.CurrentFilter2 = EmployeeNumber;
            if (!string.IsNullOrEmpty(MaritalStatus)) { page = 1; }
            else { MaritalStatus = currentFilter3; }
            ViewBag.CurrentFilter3 = MaritalStatus;
            if (!Nationality_Id.Equals(null)) { page = 1; }
            else { Nationality_Id = currentFilter4; }
            ViewBag.CurrentFilter4 = Nationality_Id;
            if (!Employee_Category_Id.Equals(null)) { page = 1; }
            else { Employee_Category_Id = currentFilter5; }
            ViewBag.CurrentFilter5 = Employee_Category_Id;
            if (!string.IsNullOrEmpty(EmployeeGender)) { page = 1; }
            else { EmployeeGender = currentFilter6; }
            ViewBag.CurrentFilter6 = EmployeeGender;
            if (!Employee_Department_Id.Equals(null)) { page = 1; }
            else { Employee_Department_Id = currentFilter7; }
            ViewBag.CurrentFilter7 = Employee_Department_Id;
            if (!Employee_Grade_Id.Equals(null)) { page = 1; }
            else { Employee_Grade_Id = currentFilter8; }
            ViewBag.CurrentFilter8 = Employee_Grade_Id;
            if (!Employee_Position_Id.Equals(null)) { page = 1; }
            else { Employee_Position_Id = currentFilter9; }
            ViewBag.CurrentFilter9 = Employee_Position_Id;
            if (!string.IsNullOrEmpty(BloodGroup)) { page = 1; }
            else { BloodGroup = currentFilter10; }
            ViewBag.CurrentFilter10 = BloodGroup;

            if (!string.IsNullOrEmpty(EmployeeJoinFromDate))
            {
                page = 1;
            }
            else { EmployeeJoinFromDate = currentFilter11; }
            DateTime? dFrom; DateTime dtFrom;
            dFrom = DateTime.TryParse(EmployeeJoinFromDate, out dtFrom) ? dtFrom : (DateTime?)null;
            ViewBag.CurrentFilter11 = EmployeeJoinFromDate;
            if (!string.IsNullOrEmpty(EmployeeJoinToDate))
            {
                page = 1;
            }
            else { EmployeeJoinToDate = currentFilter12; }
            DateTime? dTo; DateTime dtTo;
            dTo = DateTime.TryParse(EmployeeJoinToDate, out dtTo) ? dtTo : (DateTime?)null;
            ViewBag.CurrentFilter12 = EmployeeJoinToDate;

            if (string.IsNullOrEmpty(Status)) { Status = "Y"; page = 1; };
            ViewBag.CurrentFilter13 = Status;

            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  join nlty in db.COUNTRies on emp.NTLTY_ID equals nlty.ID into gnlty
                                  from subgnlty in gnlty.DefaultIfEmpty()
                                  orderby emp.FIRST_NAME, emp.LAST_NAME, subged.NAMES
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg), NationalityData = (subgnlty == null ? null : subgnlty) }).Distinct();

            if (!String.IsNullOrEmpty(searchString))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.LAST_NAME.Contains(searchString)
                                       || s.EmployeeData.FIRST_NAME.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(EmployeeNumber))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.EMP_NUM.Equals(EmployeeNumber));
            }
            if (!String.IsNullOrEmpty(MaritalStatus) && !MaritalStatus.Contains("All"))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.MARITAL_STAT.Equals(MaritalStatus));
            }

            if (!Nationality_Id.Equals(null))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.NTLTY_ID == Nationality_Id);
            }
            if (!Employee_Category_Id.Equals(null))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.EMP_CAT_ID == Nationality_Id);
            }
            if (!String.IsNullOrEmpty(EmployeeGender) && !EmployeeGender.Contains("All"))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.GNDR.Equals(EmployeeGender));
            }
            if (!Employee_Department_Id.Equals(null))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.EMP_DEPT_ID == Employee_Department_Id);
            }
            if (!Employee_Grade_Id.Equals(null))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.EMP_GRADE_ID== Employee_Grade_Id);
            }
            if (!Employee_Position_Id.Equals(null))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.EMP_POS_ID == Employee_Position_Id);
            }
            if (!String.IsNullOrEmpty(BloodGroup))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.BLOOD_GRP.Equals(BloodGroup));
            }
            if (!String.IsNullOrEmpty(Status) && !Status.Contains("All"))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.STAT.Equals(Status));
            }
            if (!String.IsNullOrEmpty(EmployeeJoinFromDate) && !String.IsNullOrEmpty(EmployeeJoinToDate))
            {
                EmployeeDetail = EmployeeDetail.Where(s => s.EmployeeData.JOINING_DATE >= dFrom).Where(s => s.EmployeeData.JOINING_DATE <= dTo);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    EmployeeDetail = EmployeeDetail.OrderByDescending(s => s.EmployeeData.FIRST_NAME);
                    break;
                case "DOB":
                    EmployeeDetail = EmployeeDetail.OrderBy(s => s.EmployeeData.DOB);
                    break;
                case "date_desc":
                    EmployeeDetail = EmployeeDetail.OrderByDescending(s => s.EmployeeData.DOB);
                    break;
                case "LName":
                    EmployeeDetail = EmployeeDetail.OrderBy(s => s.EmployeeData.LAST_NAME);
                    break;
                case "name_desc_2":
                    EmployeeDetail = EmployeeDetail.OrderByDescending(s => s.EmployeeData.LAST_NAME);
                    break;
                case "EmpNum":
                    EmployeeDetail = EmployeeDetail.OrderBy(s => s.EmployeeData.EMP_NUM);
                    break;
                case "name_desc_3":
                    EmployeeDetail = EmployeeDetail.OrderByDescending(s => s.EmployeeData.EMP_NUM);
                    break;
                case "DOJ":
                    EmployeeDetail = EmployeeDetail.OrderBy(s => s.EmployeeData.JOINING_DATE);
                    break;
                case "date_desc2":
                    EmployeeDetail = EmployeeDetail.OrderByDescending(s => s.EmployeeData.JOINING_DATE);
                    break;
                default:  // Name ascending 
                    EmployeeDetail = EmployeeDetail.OrderBy(s => s.EmployeeData.FIRST_NAME);
                    break;
            }
            List<SelectListItem> options = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", Nationality_Id==null ? 99: Nationality_Id).Distinct().ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Nationality" });
            ViewBag.Nationality_Id = options;
            List<SelectListItem> options2 = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME").Distinct().ToList();
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Employee Category" });
            ViewBag.Employee_Category_Id = options2;
            List<SelectListItem> options3 = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "NAMES").Distinct().ToList();
            // add the 'ALL' option
            options3.Insert(0, new SelectListItem() { Value = null, Text = "Select Employee Department" });
            ViewBag.Employee_Department_Id = options3;
            List<SelectListItem> options4 = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME").Distinct().ToList();
            // add the 'ALL' option
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Employee Position" });
            ViewBag.Employee_Position_Id = options4;
            List<SelectListItem> options5 = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_NAME").Distinct().ToList();
            // add the 'ALL' option
            options5.Insert(0, new SelectListItem() { Value = null, Text = "Select Employee Grade" });
            ViewBag.Employee_Grade_Id = options5;

            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(EmployeeDetail.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Payslip()
        {
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true), "ID", "NAMES").Distinct().ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Employee Department" });
            ViewBag.departments = options;
            return View();
        }

        public ActionResult Payslip_Date_Select()
        {
            return PartialView("_One_Click_Payslip_Date");
        }

        public ActionResult One_Click_Payslip_Generation(string SALARY_DATE)
        {
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var finance_manager = (from pv in db.PRIVILEGES
                                   where pv.PRIVILEGE_TAG == "Finance Control"
                                   select pv).ToList();
            var Config = new Models.Configuration();
            string finance = Config.find_by_config_value("Finance");
            DateTime SALARY_DATE2 = Convert.ToDateTime(SALARY_DATE);
            ViewBag.SALARY_DATE = SALARY_DATE2.ToShortDateString();
            DateTime start_date = SALARY_DATE2.AddDays(-SALARY_DATE2.AddDays(-1).Day);
            DateTime end_date = start_date.AddMonths(1);
            var employees = db.EMPLOYEEs.Where(x => x.STAT == true).ToList();
            if(finance_manager != null && finance_manager.Count() != 0 && finance != null)
            {
                var finance_manager_ids = (from pr in db.PRIVILEGES
                                           join upr in db.PRIVILEGES_USERS on pr.ID equals upr.PRIVILEGE_ID
                                           join emp in db.EMPLOYEEs on upr.USER_ID equals emp.USRID
                                           join emp_dep in db.EMPLOYEE_DEPARTMENT on emp.EMP_DEPT_ID equals emp_dep.ID
                                           where pr.PRIVILEGE_TAG == "Finance Control"
                                           select new SFSAcademy.Models.FinanceManager { PrivilegeData = pr, PrivilegeUsersData = upr, EmployeeData = emp, EmpDepartmentData = emp_dep }).ToList();
                var PG_eVENT = new EVENT() { TTIL = "Payslip Generated", DESCR = "Payslip Generated. Approval Pending.", START_DATE = start_date, END_DATE = end_date, IS_DUE = true, ORIGIN_ID = 2, ORIGIN_TYPE = "Payslip Approval" };
                db.EVENTs.Add(PG_eVENT);
                db.SaveChanges();
                foreach(var item in finance_manager_ids)
                {
                    var PG_Dep_eVENT = new EMPLOYEE_DEPARTMENT_EVENT() { EV_ID = PG_eVENT.ID, CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now, EMP_DEPT_ID = item.EmpDepartmentData.ID };
                    db.EMPLOYEE_DEPARTMENT_EVENT.Add(PG_Dep_eVENT);
                    db.SaveChanges();
                }
                foreach (var item2 in db.EMPLOYEEs.Where(x=>x.STAT==true).ToList())
                {
                    var payslip_exists = (from mp in db.MONTHLY_PAYSLIP
                                          where mp.EMP_ID == item2.ID && mp.SAL_DATE >= start_date && mp.SAL_DATE <= end_date
                                          select mp).ToList();
                    if(payslip_exists == null || payslip_exists.Count() == 0)
                    {
                        var salary_structure = db.EMPLOYEE_SALARY_STRUCTURE.Where(x => x.EMP_ID == item2.ID).ToList();
                        if(salary_structure != null && salary_structure.Count() != 0)
                        {
                            foreach(var item3 in salary_structure)
                            {
                                var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = item2.ID, PYRL_CAT_ID = item3.PYRL_CAT_ID, AMT = item3.AMT, IS_APPR = false, APRV_ID = null };
                                db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                                db.SaveChanges();
                            }
                        }
                    }
                }

            }
            else
            {
                foreach (var item4 in db.EMPLOYEEs.Where(x => x.STAT == true).ToList())
                {
                    var payslip_exists = (from mp in db.MONTHLY_PAYSLIP
                                          where mp.EMP_ID == item4.ID && mp.SAL_DATE >= start_date && mp.SAL_DATE <= end_date
                                          select mp).ToList();
                    if (payslip_exists == null || payslip_exists.Count() == 0)
                    {
                        var salary_structure = db.EMPLOYEE_SALARY_STRUCTURE.Where(x => x.EMP_ID == item4.ID).ToList();
                        if (salary_structure != null && salary_structure.Count() != 0)
                        {
                            foreach (var item3 in salary_structure)
                            {
                                var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = item4.ID, PYRL_CAT_ID = item3.PYRL_CAT_ID, AMT = item3.AMT, IS_APPR = true, APRV_ID = UserId };
                                db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

            return PartialView("_One_Click_Payslip_Generation");
        }

        public ActionResult Payslip_Revert_Date_Select()
        {
            var salary_dates_val = (from mp in db.MONTHLY_PAYSLIP
                                select new { mp.SAL_DATE }).Distinct().ToList();
            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in salary_dates_val)
            {
                string SalaryMonth = string.Concat(item.SAL_DATE.Value.Month, "-", item.SAL_DATE.Value.Year);
                var result = new SelectListItem();
                result.Text = SalaryMonth;
                result.Value = item.SAL_DATE.Value.ToShortDateString();
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Salary Month" });
            ViewBag.SALARY_DATE = options;
            return PartialView("_One_Click_Payslip_Revert_Date");
        }

        public ActionResult One_Click_Payslip_Revert(string SALARY_DATE)
        {
            if(SALARY_DATE != null)
            {
                DateTime SALARY_DATE2 = Convert.ToDateTime(SALARY_DATE);
                ViewBag.SALARY_DATE = SALARY_DATE2.ToShortDateString();
                DateTime start_date = SALARY_DATE2.AddDays(-SALARY_DATE2.AddDays(-1).Day);
                DateTime end_date = start_date.AddMonths(1);
                var employees = db.EMPLOYEEs.Where(x => x.STAT == true).ToList();
                foreach (var item in db.EMPLOYEEs.Where(x => x.STAT == true).ToList())
                {
                    var payslip_record = (from mp in db.MONTHLY_PAYSLIP
                                          where mp.EMP_ID == item.ID && mp.SAL_DATE >= start_date && mp.SAL_DATE <= end_date && mp.IS_APPR == false
                                          select mp).ToList();
                    foreach(var item2 in payslip_record)
                    {
                        MONTHLY_PAYSLIP Payslip_to_delete = db.MONTHLY_PAYSLIP.Find(item2.ID);
                        db.MONTHLY_PAYSLIP.Remove(Payslip_to_delete);
                        db.SaveChanges();
                    }
                    var payslip_record_Approved = (from mp in db.MONTHLY_PAYSLIP
                                          where mp.EMP_ID == item.ID && mp.SAL_DATE >= start_date && mp.SAL_DATE <= end_date && mp.IS_APPR == true
                                          select mp).ToList();

                    if (payslip_record_Approved != null && payslip_record_Approved.Count() != 0)
                    {
                        var individual_payslip_record = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.SAL_DATE >= start_date && x.SAL_DATE <= end_date && x.EMP_ID == item.ID).ToList();
                        if (individual_payslip_record != null && individual_payslip_record.Count() != 0)
                        {
                            foreach (var item3 in individual_payslip_record)
                            {
                                INDIVIDUAL_PAYSLIP_CATGEORY ipr = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(item3.ID);
                                db.INDIVIDUAL_PAYSLIP_CATGEORY.Remove(ipr);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                ViewData["Message"] = "Generated";
            }
            else
            {
                ViewData["Message"] = "Not Generated";
            }

            return PartialView("_One_Click_Payslip_Revert", ViewData["Message"]);
        }

        public ActionResult Select_Department_Employee(string Notice)
        {
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).Distinct(), "ID", "NAMES").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.departments = options;
            ViewBag.Notice = Notice;

            return View();
        }

        public ActionResult Update_Employee_Select_List(int? id)
        {
            //var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == id).OrderBy(x => x.FIRST_NAME).ToList();
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.EMP_DEPT_ID == id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg) }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();
            ViewData["employees"] = EmployeeDetail;

            return PartialView("_Employee_Select_List");
        }

        public ActionResult Create_Monthly_Payslip(int? id)
        {
            ViewBag.salary_date = null;
            var employee = db.EMPLOYEEs.Find(id);
            ViewData["employee"] = employee;
            var independent_categories = (from pc in db.PAYROLL_CATEGORY
                                          where pc.PYRL_CAT_ID == null && pc.STAT == true
                                          select new SFSAcademy.Models.EmployeePayroll { PayrollCatData = pc, EmployeeId = employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["independent_categories"] = independent_categories;
            var dependent_categories = (from pc in db.PAYROLL_CATEGORY
                                        join dpc in db.PAYROLL_CATEGORY on pc.ID equals dpc.PYRL_CAT_ID
                                        where dpc.PYRL_CAT_ID != null && pc.STAT == true
                                        select new SFSAcademy.Models.EmployeeDependentPayroll { PayrollCatData = pc, DependentPayrollCatData = dpc, DependentEmployeeId = employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["dependent_categories"] = dependent_categories;
            var employee_additional_categories = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.INCL_EVRY_MONTH == true).ToList();
            ViewData["employee_additional_categories"] = employee_additional_categories;
            var new_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.SAL_DATE == null);
            ViewData["new_payslip_category"] = new_payslip_category;
            var individual = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && System.Data.Entity.DbFunctions.TruncateTime(x.SAL_DATE.Value) == System.Data.Entity.DbFunctions.TruncateTime(System.DateTime.Now));
            ViewData["individual"] = individual;
            var user = this.Session["CurrentUser"] as UserDetails;
            ViewData["user"] = user;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var EmployeeSalaryStructure = db.EMPLOYEE_SALARY_STRUCTURE.ToList();
            ViewData["EmployeeSalaryStructure"] = EmployeeSalaryStructure;
            ViewBag.CallingAction = "Create";
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Monthly_Payslip(IEnumerable<SFSAcademy.Models.EmployeePayroll> independent_categories, IEnumerable<SFSAcademy.Models.EmployeeDependentPayroll> dependent_categories, DateTime SAL_DATE)
        {
            EMPLOYEE employee = db.EMPLOYEEs.Find(independent_categories.Where(X=>X.PayrollCategoryId != 0).FirstOrDefault().EmployeeId);
            ViewData["employee"] = employee;
            DateTime salary_date = SAL_DATE;
            ViewBag.salary_date = salary_date;
            var independent_categories_val = (from pc in db.PAYROLL_CATEGORY
                                          where pc.PYRL_CAT_ID == null && pc.STAT == true
                                          select new SFSAcademy.Models.EmployeePayroll { PayrollCatData = pc, EmployeeId = employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["independent_categories"] = independent_categories_val;
            var dependent_categories_val = (from pc in db.PAYROLL_CATEGORY
                                        join dpc in db.PAYROLL_CATEGORY on pc.ID equals dpc.PYRL_CAT_ID
                                        where dpc.PYRL_CAT_ID != null && pc.STAT == true
                                        select new SFSAcademy.Models.EmployeeDependentPayroll { PayrollCatData = pc, DependentPayrollCatData = dpc, DependentEmployeeId = employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["dependent_categories"] = dependent_categories_val;
            var employee_additional_categories = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.INCL_EVRY_MONTH == true).ToList();
            ViewData["employee_additional_categories"] = employee_additional_categories;
            var new_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == null);
            ViewData["new_payslip_category"] = new_payslip_category;
            var individual = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && System.Data.Entity.DbFunctions.TruncateTime(x.SAL_DATE.Value) == System.Data.Entity.DbFunctions.TruncateTime(System.DateTime.Now));
            ViewData["individual"] = individual;
            var user = this.Session["CurrentUser"] as UserDetails;
            ViewData["user"] = user;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var EmployeeSalaryStructure = db.EMPLOYEE_SALARY_STRUCTURE.ToList();
            ViewData["EmployeeSalaryStructure"] = EmployeeSalaryStructure;
            if (salary_date >= employee.JOINING_DATE)
            {
                DateTime start_date = salary_date.AddDays(-salary_date.AddDays(-1).Day);
                DateTime end_date = start_date.AddMonths(1);
                var payslip_exists = (from mp in db.MONTHLY_PAYSLIP
                                      where mp.EMP_ID == employee.ID && mp.SAL_DATE >= start_date && mp.SAL_DATE <= end_date
                                      select mp).ToList();
                if (payslip_exists == null || payslip_exists.Count() == 0)
                {
                    foreach (var item in independent_categories)
                    {
                        if (item.PayrollCategoryId != 0)
                        {
                            var row_id = db.EMPLOYEE_SALARY_STRUCTURE.Where(x=>x.EMP_ID==employee.ID && x.PYRL_CAT_ID==item.PayrollCategoryId).ToList();
                            string category_name = db.PAYROLL_CATEGORY.Find(item.PayrollCategoryId).NAME;
                            if(row_id != null && row_id.Count() != 0)
                            {
                                var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = employee.ID, PYRL_CAT_ID = item.PayrollCategoryId, AMT = item.Amount, IS_APPR = false, APRV_ID = null };
                                db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                            }
                            else
                            {
                                var Emp_Sal_Struct = new EMPLOYEE_SALARY_STRUCTURE() { EMP_ID = employee.ID, PYRL_CAT_ID = item.PayrollCategoryId, AMT = item.Amount };
                                db.EMPLOYEE_SALARY_STRUCTURE.Add(Emp_Sal_Struct);
                                //db.SaveChanges();
                                var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = employee.ID, PYRL_CAT_ID = item.PayrollCategoryId, AMT = item.Amount, IS_APPR = false, APRV_ID = null };
                                db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                            }
                        }
                    }

                    foreach (var item in dependent_categories)
                    {
                        if (item.DependentPayrollCategoryId != 0)
                        {
                            var row_id = db.EMPLOYEE_SALARY_STRUCTURE.Where(x => x.EMP_ID == employee.ID && x.PYRL_CAT_ID == item.DependentPayrollCategoryId).ToList();
                            string category_name = db.PAYROLL_CATEGORY.Find(item.DependentPayrollCategoryId).NAME;
                            if (row_id != null && row_id.Count() != 0)
                            {
                                var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = employee.ID, PYRL_CAT_ID = item.DependentPayrollCategoryId, AMT = item.DependentAmount, IS_APPR = false, APRV_ID = null };
                                db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                            }
                            else
                            {
                                var Emp_Sal_Struct = new EMPLOYEE_SALARY_STRUCTURE() { EMP_ID = employee.ID, PYRL_CAT_ID = item.DependentPayrollCategoryId, AMT = item.DependentAmount };
                                db.EMPLOYEE_SALARY_STRUCTURE.Add(Emp_Sal_Struct);
                                var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = employee.ID, PYRL_CAT_ID = item.DependentPayrollCategoryId, AMT = item.DependentAmount, IS_APPR = false, APRV_ID = null };
                                db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                            }
                        }
                    }
                    var individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == null);
                    foreach(var item in individual_payslip_category)
                    {
                        var individual_payslip_category_upd = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(item.ID);
                        individual_payslip_category_upd.SAL_DATE = start_date;
                        db.Entry(individual_payslip_category_upd).State = EntityState.Modified;                      
                    }
                    ViewBag.Notice = string.Concat(employee.FIRST_NAME, "'s salary slip generated for: ", salary_date.ToShortDateString());
                }
                else
                {
                    var individual_payslips_generated = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == null).ToList();
                    if(individual_payslips_generated != null && individual_payslips_generated.Count()!=0)
                    {
                        foreach(var item in individual_payslips_generated)
                        {
                            INDIVIDUAL_PAYSLIP_CATGEORY ipc = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(item.ID);
                            db.INDIVIDUAL_PAYSLIP_CATGEORY.Remove(ipc);
                        }
                    }
                    ViewBag.Notice = string.Concat(employee.FIRST_NAME, "'s salary slip  already generated for: ", salary_date.ToShortDateString());
                }
                var finance_manager_ids = (from pr in db.PRIVILEGES
                                           join upr in db.PRIVILEGES_USERS on pr.ID equals upr.PRIVILEGE_ID
                                           join emp in db.EMPLOYEEs on upr.USER_ID equals emp.USRID
                                           join emp_dep in db.EMPLOYEE_DEPARTMENT on emp.EMP_DEPT_ID equals emp_dep.ID
                                           where pr.PRIVILEGE_TAG == "Finance Control"
                                           select new SFSAcademy.Models.FinanceManager { PrivilegeData = pr, PrivilegeUsersData = upr, EmployeeData = emp, EmpDepartmentData = emp_dep }).ToList();
                var PG_eVENT = new EVENT() { TTIL = "Payslip Generated", DESCR = "Payslip Generated. Approval Pending.", START_DATE = start_date, END_DATE = end_date, IS_DUE = true, ORIGIN_ID = 2, ORIGIN_TYPE = "Payslip Approval" };
                db.EVENTs.Add(PG_eVENT);
                foreach (var item in finance_manager_ids)
                {
                    var PG_Dep_eVENT = new EMPLOYEE_DEPARTMENT_EVENT() { EV_ID = PG_eVENT.ID, CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now, EMP_DEPT_ID = item.EmpDepartmentData.ID };
                    db.EMPLOYEE_DEPARTMENT_EVENT.Add(PG_Dep_eVENT);
                }
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.Warn_Notice = string.Concat(ViewBag.Warn_Notice, "|", ve.ErrorMessage); } }
                    return View(employee);
                }
                catch (Exception e)
                {
                    ViewBag.Warn_Notice = string.Concat(ViewBag.Warn_Notice, "|", e.InnerException.InnerException.Message);
                    return View(employee);
                }
                return RedirectToAction("Select_Department_Employee", new { Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Warn_Notice = string.Concat("Payslip Date cant be generated before joining date ", employee.JOINING_DATE.Value.ToShortDateString());
                return View(employee);
            }
        }

        public ActionResult Add_Payslip_Category(int? emp_id, DateTime? salary_date, string CallingAction)
        {
            ViewBag.salary_date = salary_date;
            var employee = db.EMPLOYEEs.Find(emp_id);
            ViewData["employee"] = employee;
            ViewBag.CallingAction = CallingAction;
            return PartialView("_Payslip_Category_Form");
        }

        public ActionResult Create_Payslip_Category(int? employee_id, DateTime? salary_date, string Name, decimal? Amount, string include_every_month, string Is_Deduction, string CallingAction)
        {

            ViewBag.salary_date = salary_date;
            DateTime? salary_date2 = salary_date == null ? System.DateTime.Now : salary_date;
            var employee = db.EMPLOYEEs.Find(employee_id);
            ViewData["employee"] = employee;
            ViewBag.CallingAction = CallingAction;
            var created_category = new INDIVIDUAL_PAYSLIP_CATGEORY() { EMP_ID = employee.ID, NAME = Name, AMT = Amount };
            db.INDIVIDUAL_PAYSLIP_CATGEORY.Add(created_category);
            try
            {
                db.SaveChanges();
                if(Is_Deduction == null)
                {
                    var individual_payslip_category_upd = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(created_category.ID);
                    individual_payslip_category_upd.IS_DED = false;
                    db.Entry(individual_payslip_category_upd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var individual_payslip_category_upd = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(created_category.ID);
                    individual_payslip_category_upd.IS_DED = Is_Deduction == "True"? true : false;
                    db.Entry(individual_payslip_category_upd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (include_every_month == null)
                {
                    var individual_payslip_category_upd = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(created_category.ID);
                    individual_payslip_category_upd.INCL_EVRY_MONTH = false;
                    db.Entry(individual_payslip_category_upd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var individual_payslip_category_upd = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(created_category.ID);
                    individual_payslip_category_upd.INCL_EVRY_MONTH = include_every_month == "Y"? true :false;
                    db.Entry(individual_payslip_category_upd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var new_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == null);
                ViewData["new_payslip_category"] = new_payslip_category;
                var individual = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == salary_date2);
                ViewData["individual"] = individual;
                return PartialView("_Payslip_Category_List");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                    }
                }
                return PartialView("_Payslip_Category_Form"); ;
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return PartialView("_Payslip_Category_Form");
            }
        }

        public ActionResult Remove_New_Paylist_Category(int? id, DateTime? id3, string CallingAction)
        {
            INDIVIDUAL_PAYSLIP_CATGEORY removal_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(id);
            var employee = db.EMPLOYEEs.Find(removal_category.EMP_ID);
            ViewData["employee"] = employee;
            DateTime? salary_date = id3 == null ? System.DateTime.Now : id3;
            ViewBag.salary_date = id3;

            db.INDIVIDUAL_PAYSLIP_CATGEORY.Remove(removal_category);
            db.SaveChanges();
            var new_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == null);
            ViewData["new_payslip_category"] = new_payslip_category;
            var individual = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == salary_date);
            ViewData["individual"] = individual;

            if(CallingAction == "Edit")
            {
                return RedirectToAction("Edit_Rejected_Payslip", new { id = employee.ID, id2 = id3 });
            }
            else if (CallingAction == "Create")
            {
                return RedirectToAction("Create_Monthly_Payslip", new { id = employee.ID });
            }
            else
            {
                return RedirectToAction("Create_Monthly_Payslip", new { id = employee.ID });
            }
            //return PartialView("_List_Payslip_Category");
        }

        public ActionResult Rejected_Payslip(string Notice)
        {
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).Distinct(), "ID", "NAMES").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.departments = options;
            ViewBag.Notice = Notice;

            return View();
        }

        public ActionResult Update_Rejected_Employee_List(int? id)
        {
            var Emp_Payslip = from ps in db.MONTHLY_PAYSLIP.Where(x=>x.IS_RJCT==true)
                           group ps by ps.EMP_ID into g
                           select new
                           {
                               Employee_ID = g.Key,
                               Salary_date = (from ps2 in g select ps2.SAL_DATE).Max()
                           };
            var EmployeeDetail = (from emp in db.EMPLOYEEs
                                  join psl in Emp_Payslip on emp.ID equals psl.Employee_ID
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.EMP_DEPT_ID == id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg), Salary_date = psl.Salary_date }).OrderBy(x => x.EmployeeData.FIRST_NAME).ToList();
            ViewData["employees"] = EmployeeDetail;

            return PartialView("_Rejected_Employee_Select_List");
        }

        public ActionResult View_Rejected_Payslip(int? id, DateTime? id2)
        {
            var PSL_Sal_date = (from psl in db.MONTHLY_PAYSLIP
                                where psl.IS_RJCT == true && psl.EMP_ID == id
                                select new { Salary_date = psl.SAL_DATE }).Distinct()
                        .OrderBy(x => x.Salary_date).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in PSL_Sal_date)
            {
                string sala_Month_Year = string.Concat(((DateTime)item.Salary_date.Value).ToString("MMMM"), "-", item.Salary_date.Value.Year);
                var result = new SelectListItem();
                result.Text = sala_Month_Year;
                result.Value = item.Salary_date.ToString();
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Salary Date" });
            ViewBag.Salary_Date = options;

            var employee = db.EMPLOYEEs.Find(id);
            ViewData["employee"] = employee;

            return View();
        }

        public ActionResult Update_Rejected_Payslip(int? emp_id, string salary_date)
        {
            DateTime? SAL_DATE = DateTime.Parse(salary_date);
            ViewBag.salary_date = SAL_DATE;
            var employee = db.EMPLOYEEs.Find(emp_id);
            ViewData["employee"] = employee;
            var Config = new Models.Configuration();
            ViewBag.currency_type = Config.find_by_config_key("CurrencyType");
            if(salary_date == null)
            {
                return PartialView("_Rejected_Payslip");
            }
            var monthly_payslips = db.MONTHLY_PAYSLIP.Where(x => x.SAL_DATE == SAL_DATE && x.EMP_ID == emp_id).OrderBy(x => x.PYRL_CAT_ID).ToList();
            ViewData["monthly_payslips"] = monthly_payslips;
            var individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == emp_id && x.SAL_DATE == SAL_DATE).OrderBy(x => x.ID).ToList();
            ViewData["individual_payslip_category"] = individual_payslip_category;
            decimal individual_category_non_deductionable = 0;
            decimal individual_category_deductionable = 0;
            foreach(var item in individual_payslip_category)
            {
                if(item.IS_DED == false)
                {
                    individual_category_non_deductionable += (decimal)item.AMT;
                }
            }
            foreach (var item in individual_payslip_category)
            {
                if (item.IS_DED == true)
                {
                    individual_category_deductionable += (decimal)item.AMT;
                }
            }
            ViewBag.individual_category_non_deductionable = individual_category_non_deductionable;
            ViewBag.individual_category_deductionable = individual_category_deductionable;

            decimal non_deductionable_amount = 0;
            decimal deductionable_amount = 0;
            foreach (var item in monthly_payslips)
            {
                var category1 = db.PAYROLL_CATEGORY.Find(item.PYRL_CAT_ID);
                if (category1.IS_DED == false)
                {
                    non_deductionable_amount += (decimal)item.AMT;
                }
            }
            foreach (var item in monthly_payslips)
            {
                var category2 = db.PAYROLL_CATEGORY.Find(item.PYRL_CAT_ID);
                if (category2.IS_DED == true)
                {
                    deductionable_amount += (decimal)item.AMT;
                }
            }
            ViewBag.non_deductionable_amount = non_deductionable_amount;
            ViewBag.deductionable_amount = deductionable_amount;

            decimal net_non_deductionable_amount = individual_category_non_deductionable + non_deductionable_amount;
            decimal net_deductionable_amount = individual_category_deductionable + deductionable_amount;
            ViewBag.net_non_deductionable_amount = net_non_deductionable_amount;
            ViewBag.net_deductionable_amount = net_deductionable_amount;

            decimal net_amount = net_non_deductionable_amount - net_deductionable_amount;
            ViewBag.net_amount = net_amount;
            ViewData["PayrollCategory"] = db.PAYROLL_CATEGORY.ToList();
            return PartialView("_Rejected_Payslip");
        }

        public ActionResult Edit_Rejected_Payslip(int? id, DateTime id2)
        {
            DateTime PDate = Convert.ToDateTime(id2);
            ViewBag.salary_date = PDate.ToShortDateString();

           var employee = db.EMPLOYEEs.Find(id);
            ViewData["employee"] = employee;
            var monthly_payslips = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == id && x.SAL_DATE == PDate).ToList();
            ViewData["monthly_payslips"] = monthly_payslips;
            var individual = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && System.Data.Entity.DbFunctions.TruncateTime(x.SAL_DATE.Value) == System.Data.Entity.DbFunctions.TruncateTime(PDate));
            ViewData["individual"] = individual;
            var independent_categories = (from pc in db.PAYROLL_CATEGORY
                                          where pc.PYRL_CAT_ID == null && pc.STAT == true
                                          select new SFSAcademy.Models.EmployeePayroll { PayrollCatData = pc, EmployeeId = employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["independent_categories"] = independent_categories;
            var dependent_categories = (from pc in db.PAYROLL_CATEGORY
                                        join dpc in db.PAYROLL_CATEGORY on pc.ID equals dpc.PYRL_CAT_ID
                                        where dpc.PYRL_CAT_ID != null && pc.STAT == true
                                        select new SFSAcademy.Models.EmployeeDependentPayroll { PayrollCatData = pc, DependentPayrollCatData = dpc, DependentEmployeeId = employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["dependent_categories"] = dependent_categories;
            var employee_additional_categories = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.INCL_EVRY_MONTH == true).ToList();
            ViewData["employee_additional_categories"] = employee_additional_categories;
            var new_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.SAL_DATE == null);
            ViewData["new_payslip_category"] = new_payslip_category;
            var user = this.Session["CurrentUser"] as UserDetails;
            ViewData["user"] = user;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var MonthlyPayslip = db.MONTHLY_PAYSLIP.ToList();
            ViewData["MonthlyPayslip"] = MonthlyPayslip;
            ViewBag.CallingAction = "Edit";

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Rejected_Payslip(IEnumerable<SFSAcademy.Models.EmployeePayroll> independent_categories, IEnumerable<SFSAcademy.Models.EmployeeDependentPayroll> dependent_categories, DateTime SAL_DATE)
        {
            EMPLOYEE employee = db.EMPLOYEEs.Find(independent_categories.Where(X => X.PayrollCategoryId != 0).FirstOrDefault().EmployeeId);
            ViewData["employee"] = employee;
            DateTime salary_date = SAL_DATE;
            ViewBag.salary_date = salary_date;
            var monthly_payslips = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == salary_date).ToList();
            ViewData["monthly_payslips"] = monthly_payslips;
            var individual = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && System.Data.Entity.DbFunctions.TruncateTime(x.SAL_DATE.Value) == System.Data.Entity.DbFunctions.TruncateTime(salary_date));
            ViewData["individual"] = individual;
            var independent_categories_val = (from pc in db.PAYROLL_CATEGORY
                                          where pc.PYRL_CAT_ID == null && pc.STAT == true
                                          select new SFSAcademy.Models.EmployeePayroll { PayrollCatData = pc, EmployeeId = employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["independent_categories"] = independent_categories;
            var dependent_categories_val = (from pc in db.PAYROLL_CATEGORY
                                        join dpc in db.PAYROLL_CATEGORY on pc.ID equals dpc.PYRL_CAT_ID
                                        where dpc.PYRL_CAT_ID != null && pc.STAT == true
                                        select new SFSAcademy.Models.EmployeeDependentPayroll { PayrollCatData = pc, DependentPayrollCatData = dpc, DependentEmployeeId = employee.ID }).OrderBy(x => x.PayrollCatData.NAME).ToList();
            ViewData["dependent_categories"] = dependent_categories;
            var employee_additional_categories = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.INCL_EVRY_MONTH == true).ToList();
            ViewData["employee_additional_categories"] = employee_additional_categories;
            var new_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == null);
            ViewData["new_payslip_category"] = new_payslip_category;
            var user = this.Session["CurrentUser"] as UserDetails;
            ViewData["user"] = user;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var MonthlyPayslip = db.MONTHLY_PAYSLIP.ToList();
            ViewData["MonthlyPayslip"] = MonthlyPayslip;
            ViewBag.CallingAction = "Edit";

            DateTime start_date = salary_date.AddDays(-salary_date.AddDays(-1).Day);
            DateTime end_date = start_date.AddMonths(1);
            var payslip_exists = (from mp in db.MONTHLY_PAYSLIP
                                  where mp.EMP_ID == employee.ID && mp.SAL_DATE >= start_date && mp.SAL_DATE <= end_date
                                  select mp).ToList();
            foreach(var item in payslip_exists)
            {
                MONTHLY_PAYSLIP mps = db.MONTHLY_PAYSLIP.Find(item.ID);
                db.MONTHLY_PAYSLIP.Remove(mps);
            }
            foreach (var item in independent_categories)
            {
                if (item.PayrollCategoryId != 0)
                {
                    var row_id = db.EMPLOYEE_SALARY_STRUCTURE.Where(x => x.EMP_ID == employee.ID && x.PYRL_CAT_ID == item.PayrollCategoryId).ToList();
                    string category_name = db.PAYROLL_CATEGORY.Find(item.PayrollCategoryId).NAME;
                    if (row_id != null && row_id.Count() != 0)
                    {
                        var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = employee.ID, PYRL_CAT_ID = item.PayrollCategoryId, AMT = item.Amount, IS_APPR =false, APRV_ID = null };
                        db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                    }
                    else
                    {
                        var Emp_Sal_Struct = new EMPLOYEE_SALARY_STRUCTURE() { EMP_ID = employee.ID, PYRL_CAT_ID = item.PayrollCategoryId, AMT = item.Amount };
                        db.EMPLOYEE_SALARY_STRUCTURE.Add(Emp_Sal_Struct);
                        var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = employee.ID, PYRL_CAT_ID = item.PayrollCategoryId, AMT = item.Amount, IS_APPR = false, APRV_ID = null };
                        db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                    }
                }
            }

            foreach (var item in dependent_categories)
            {
                if (item.DependentPayrollCategoryId != 0)
                {
                    var row_id = db.EMPLOYEE_SALARY_STRUCTURE.Where(x => x.EMP_ID == employee.ID && x.PYRL_CAT_ID == item.DependentPayrollCategoryId).ToList();
                    string category_name = db.PAYROLL_CATEGORY.Find(item.DependentPayrollCategoryId).NAME;
                    if (row_id != null && row_id.Count() != 0)
                    {
                        var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = employee.ID, PYRL_CAT_ID = item.DependentPayrollCategoryId, AMT = item.DependentAmount, IS_APPR = false, APRV_ID = null };
                        db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                    }
                    else
                    {
                        var Emp_Sal_Struct = new EMPLOYEE_SALARY_STRUCTURE() { EMP_ID = employee.ID, PYRL_CAT_ID = item.DependentPayrollCategoryId, AMT = item.DependentAmount };
                        db.EMPLOYEE_SALARY_STRUCTURE.Add(Emp_Sal_Struct);
                        var Month_Pay_Slip = new MONTHLY_PAYSLIP() { SAL_DATE = start_date, EMP_ID = employee.ID, PYRL_CAT_ID = item.DependentPayrollCategoryId, AMT = item.DependentAmount, IS_APPR = false, APRV_ID = null };
                        db.MONTHLY_PAYSLIP.Add(Month_Pay_Slip);
                    }
                }
            }
            var individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == null);
            foreach (var item in individual_payslip_category)
            {
                var individual_payslip_category_upd = db.INDIVIDUAL_PAYSLIP_CATGEORY.Find(item.ID);
                individual_payslip_category_upd.SAL_DATE = start_date;
                db.Entry(individual_payslip_category_upd).State = EntityState.Modified;
            }
            ViewBag.Notice = string.Concat(employee.FIRST_NAME, "'s salary slip generated for: ", salary_date.ToShortDateString());
            var finance_manager_ids = (from pr in db.PRIVILEGES
                                       join upr in db.PRIVILEGES_USERS on pr.ID equals upr.PRIVILEGE_ID
                                       join emp in db.EMPLOYEEs on upr.USER_ID equals emp.USRID
                                       join emp_dep in db.EMPLOYEE_DEPARTMENT on emp.EMP_DEPT_ID equals emp_dep.ID
                                       where pr.PRIVILEGE_TAG == "Finance Control"
                                       select new SFSAcademy.Models.FinanceManager { PrivilegeData = pr, PrivilegeUsersData = upr, EmployeeData = emp, EmpDepartmentData = emp_dep }).ToList();
            var PG_eVENT = new EVENT() { TTIL = "Payslip Generated", DESCR = "Payslip Generated. Approval Pending.", START_DATE = start_date, END_DATE = end_date, IS_DUE = true, ORIGIN_ID = 2, ORIGIN_TYPE = "Payslip Approval" };
            db.EVENTs.Add(PG_eVENT);
            foreach (var item in finance_manager_ids)
            {
                var PG_Dep_eVENT = new EMPLOYEE_DEPARTMENT_EVENT() { EV_ID = PG_eVENT.ID, CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now, EMP_DEPT_ID = item.EmpDepartmentData.ID };
                db.EMPLOYEE_DEPARTMENT_EVENT.Add(PG_Dep_eVENT);
            }
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.Notice = string.Concat(ViewBag.Notice, "|", ve.ErrorMessage); } }
                return View(employee);
            }
            catch (Exception e)
            {
                ViewBag.Notice = string.Concat(ViewBag.Notice, "|", e.InnerException.InnerException.Message);
                return View(employee);
            }
            return RedirectToAction("Profiles", new { id = employee.ID, Notice = ViewBag.Notice });
            
        }

        public ActionResult Department_Payslip(string Notice)
        {
            ViewBag.Notice = Notice;

            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).Distinct(), "ID", "NAMES").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.departments = options;
            ViewBag.IsPost = false;
            var PSL_Sal_date = (from psl in db.MONTHLY_PAYSLIP
                                select new { Salary_date = psl.SAL_DATE }).Distinct()
                        .OrderBy(x => x.Salary_date).ToList();


            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in PSL_Sal_date)
            {
                string sala_Month_Year = string.Concat(((DateTime)item.Salary_date.Value).ToString("MMMM"), "-", item.Salary_date.Value.Year);
                var result = new SelectListItem();
                result.Text = sala_Month_Year;
                result.Value = item.Salary_date.ToString();
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Salary Date" });
            ViewBag.Salary_Date = options2;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Department_Payslip(int? departments, DateTime? Salary_Date, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.IsPost = true;
            ViewBag.Selected_Salary_Date = Salary_Date;
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).Distinct(), "ID", "NAMES", departments).ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.departments = options;
            var PSL_Sal_date = (from psl in db.MONTHLY_PAYSLIP
                                select new { Salary_date = psl.SAL_DATE }).Distinct()
                        .OrderBy(x => x.Salary_date).ToList();


            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in PSL_Sal_date)
            {
                string sala_Month_Year = string.Concat(((DateTime)item.Salary_date.Value).ToString("MMMM"), "-", item.Salary_date.Value.Year);
                var result = new SelectListItem();
                result.Text = sala_Month_Year;
                result.Value = item.Salary_date.ToString();
                result.Selected = item.Salary_date == Salary_Date ? true : false;
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Salary Date" });
            ViewBag.Salary_Date = options2;

            if (departments!= null || Salary_Date != null)
            {
                if(departments != null && Salary_Date != null)
                {
                    var monthly_payslips = (from ps in db.MONTHLY_PAYSLIP
                                             join pc in db.PAYROLL_CATEGORY on ps.PYRL_CAT_ID equals pc.ID
                                             where ps.SAL_DATE == Salary_Date
                                             select new Models.MonthyPayslip { MonthlyPayslipData = ps, PayrollCatogaryData = pc });

                    var grouped_monthly_payslips = from mps in monthly_payslips
                                      group mps by mps.MonthlyPayslipData.EMP_ID into g
                                      select new
                                      {
                                          Employee_ID = g.Key,
                                          Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() ==true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true? "Rejected" : null),
                                          Monthy_Payslip_Amount = g.Sum(x=>x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT)
                                      };

                    var approved_grouped_monthly_payslips = from mps in monthly_payslips
                                                            where mps.MonthlyPayslipData.IS_APPR == true
                                                            group mps by mps.MonthlyPayslipData.EMP_ID into g
                                                   select new
                                                   {
                                                       Employee_ID = g.Key,
                                                       Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                                       Aapproved_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED==false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT)
                                                   };

                    var grouped_individual_payslip_categories = from ps in db.INDIVIDUAL_PAYSLIP_CATGEORY
                                                                where ps.SAL_DATE == Salary_Date
                                                                group ps by ps.EMP_ID into g
                                                   select new
                                                   {
                                                       Employee_ID = g.Key,
                                                       Individual_Pyaslip_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : -x.AMT)
                                                   };

                    var payslips = (from emp in db.EMPLOYEEs
                                    join gmp in grouped_monthly_payslips on emp.ID equals gmp.Employee_ID
                                    join gamp in approved_grouped_monthly_payslips on emp.ID equals gamp.Employee_ID into ggamp
                                    from subggamp in ggamp.DefaultIfEmpty()
                                    join gipc in grouped_individual_payslip_categories on emp.ID equals gipc.Employee_ID into ggipc
                                    from subgipc in ggipc.DefaultIfEmpty()
                                    where emp.EMP_DEPT_ID == departments
                                    select new SFSAcademy.Models.Payslip { EmployeeData = emp, Monthy_Payslip_Amount = gmp.Monthy_Payslip_Amount, Status = gmp.Status, Aapproved_Amount = (subggamp == null ? null : subggamp.Aapproved_Amount), Individual_Pyaslip_Amount = (subgipc == null ? null : subgipc.Individual_Pyaslip_Amount), Net_Amount = (subgipc == null ? gmp.Monthy_Payslip_Amount : gmp.Monthy_Payslip_Amount + subgipc.Individual_Pyaslip_Amount) }).Distinct();
                    ViewData["payslips"] = payslips.ToList();

                    return View();
                }
                else
                {
                    ViewBag.Notice = "Select salary date";
                    return RedirectToAction("Department_Payslip", new { Notice = ViewBag.Notice });
                }
            }
            return View();
        }

        public ActionResult View_Employee_Payslip(int? id, DateTime? salary_date)
        {
            ViewBag.Salary_Date = salary_date;
            ViewBag.Selected_Salary_Date = salary_date.Value.ToShortDateString();
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            var Config = new Models.Configuration();
            ViewBag.currency_type = Config.find_by_config_key("CurrencyType");
            var monthly_payslips = (from emp in db.EMPLOYEEs
                                    join mp in db.MONTHLY_PAYSLIP on emp.ID equals mp.EMP_ID
                                    join pc in db.PAYROLL_CATEGORY on mp.PYRL_CAT_ID equals pc.ID
                                    where mp.SAL_DATE == salary_date &&  emp.ID == id
                                    select new Models.MonthyPayslip { EmployeeData = emp, MonthlyPayslipData = mp, PayrollCatogaryData = pc }).OrderBy(x => x.PayrollCatogaryData.NAME).ToList();
            ViewData["monthly_payslips"] = monthly_payslips;
            ViewBag.Status = monthly_payslips.FirstOrDefault().MonthlyPayslipData.IS_APPR == true ? "Approved" : (monthly_payslips.FirstOrDefault().MonthlyPayslipData.IS_RJCT == true ? "Rejected" : null);
            var individual_payslips = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.SAL_DATE == salary_date).ToList();
            ViewData["individual_payslips"] = individual_payslips;

            var monthly_payslips_val = (from ps in db.MONTHLY_PAYSLIP
                                    join pc in db.PAYROLL_CATEGORY on ps.PYRL_CAT_ID equals pc.ID
                                    where ps.SAL_DATE == salary_date
                                        select new Models.MonthyPayslip { MonthlyPayslipData = ps, PayrollCatogaryData = pc });

            var grouped_monthly_payslips = from mps in monthly_payslips_val
                                           group mps by mps.MonthlyPayslipData.EMP_ID into g
                                           select new
                                           {
                                               Employee_ID = g.Key,
                                               Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                               Monthy_Payslip_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT),
                                               Non_Deductionable_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT: 0),
                                               Deductionable_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == true ? x.MonthlyPayslipData.AMT : 0)
                                           };

            var approved_grouped_monthly_payslips = from mps in monthly_payslips_val
                                                    where mps.MonthlyPayslipData.IS_APPR == true
                                                    group mps by mps.MonthlyPayslipData.EMP_ID into g
                                                    select new
                                                    {
                                                        Employee_ID = g.Key,
                                                        Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                                        Aapproved_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT)
                                                    };

            var grouped_individual_payslip_categories = from ps in db.INDIVIDUAL_PAYSLIP_CATGEORY
                                                        where ps.SAL_DATE == salary_date
                                                        group ps by ps.EMP_ID into g
                                                        select new
                                                        {
                                                            Employee_ID = g.Key,
                                                            Individual_Pyaslip_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : -x.AMT),
                                                            Non_Deductionable_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : 0),
                                                            Deductionable_Amount = g.Sum(x => x.IS_DED == true ? x.AMT : 0)
                                                        };

            var payslips = (from emp in db.EMPLOYEEs
                            join gmp in grouped_monthly_payslips on emp.ID equals gmp.Employee_ID
                            join gamp in approved_grouped_monthly_payslips on emp.ID equals gamp.Employee_ID into ggamp
                            from subggamp in ggamp.DefaultIfEmpty()
                            join gipc in grouped_individual_payslip_categories on emp.ID equals gipc.Employee_ID into ggipc
                            from subgipc in ggipc.DefaultIfEmpty()
                            where emp.ID == id
                            select new SFSAcademy.Models.Payslip { EmployeeData = emp, Monthy_Payslip_Amount = gmp.Monthy_Payslip_Amount, Status = gmp.Status, Aapproved_Amount = (subggamp == null ? null : subggamp.Aapproved_Amount), Individual_Pyaslip_Amount = (subgipc == null ? null : subgipc.Individual_Pyaslip_Amount), Net_Amount = (subgipc == null ? gmp.Monthy_Payslip_Amount : gmp.Monthy_Payslip_Amount + subgipc.Individual_Pyaslip_Amount), Net_Non_Deductionable_Amount = (subgipc == null ? gmp.Non_Deductionable_Amount : gmp.Non_Deductionable_Amount + subgipc.Non_Deductionable_Amount), Net_Deductionable_Amount = (subgipc == null ? gmp.Deductionable_Amount : gmp.Deductionable_Amount + subgipc.Deductionable_Amount) }).Distinct();

            ViewData["salary"] = payslips;
            
            return View(Employee);
        }

        public ActionResult Employee_Individual_Payslip_pdf(int? id, DateTime? salary_date)
        {
            ViewBag.Salary_Date = salary_date.Value.ToShortDateString();
            var Employee = (from emp in db.EMPLOYEEs
                                  join ed in db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true) on emp.EMP_DEPT_ID equals ed.ID into ged
                                  from subged in ged.DefaultIfEmpty()
                                  join ec in db.EMPLOYEE_CATEGORY.Where(x => x.STAT == true) on emp.EMP_CAT_ID equals ec.ID into gec
                                  from subgec in gec.DefaultIfEmpty()
                                  join ep in db.EMPLOYEE_POSITION.Where(x => x.IS_ACT == true) on emp.EMP_POS_ID equals ep.ID into gep
                                  from subgep in gep.DefaultIfEmpty()
                                  join eg in db.EMPLOYEE_GRADE.Where(x => x.IS_ACT == true) on emp.EMP_GRADE_ID equals eg.ID into geg
                                  from subgeg in geg.DefaultIfEmpty()
                                  where emp.ID == id
                                  select new SFSAcademy.Models.Employee { EmployeeData = emp, DepartmentData = (subged == null ? null : subged), CategoryData = (subgec == null ? null : subgec), PositionData = (subgep == null ? null : subgep), GradeData = (subgeg == null ? null : subgeg)}).OrderBy(x => x.EmployeeData.FIRST_NAME).FirstOrDefault();
            var Config = new Models.Configuration();
            ViewBag.currency_type = Config.find_by_config_key("CurrencyType");
            var monthly_payslips = (from emp in db.EMPLOYEEs
                                    join mp in db.MONTHLY_PAYSLIP on emp.ID equals mp.EMP_ID
                                    join pc in db.PAYROLL_CATEGORY on mp.PYRL_CAT_ID equals pc.ID
                                    where mp.SAL_DATE == salary_date && emp.ID == id
                                    select new Models.MonthyPayslip { EmployeeData = emp, MonthlyPayslipData = mp, PayrollCatogaryData = pc }).OrderBy(x => x.PayrollCatogaryData.NAME).ToList();
            ViewData["monthly_payslips"] = monthly_payslips;
            ViewBag.Status = monthly_payslips.FirstOrDefault().MonthlyPayslipData.IS_APPR == true ? "Approved" : (monthly_payslips.FirstOrDefault().MonthlyPayslipData.IS_RJCT == true ? "Rejected" : null);
            var individual_payslips = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.SAL_DATE == salary_date).ToList();
            ViewData["individual_payslips"] = individual_payslips;

            var monthly_payslips_val = (from ps in db.MONTHLY_PAYSLIP
                                        join pc in db.PAYROLL_CATEGORY on ps.PYRL_CAT_ID equals pc.ID
                                        where ps.SAL_DATE == salary_date
                                        select new Models.MonthyPayslip { MonthlyPayslipData = ps, PayrollCatogaryData = pc });

            var grouped_monthly_payslips = from mps in monthly_payslips_val
                                           group mps by mps.MonthlyPayslipData.EMP_ID into g
                                           select new
                                           {
                                               Employee_ID = g.Key,
                                               Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                               Monthy_Payslip_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT),
                                               Non_Deductionable_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : 0),
                                               Deductionable_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == true ? x.MonthlyPayslipData.AMT : 0)
                                           };

            var approved_grouped_monthly_payslips = from mps in monthly_payslips_val
                                                    where mps.MonthlyPayslipData.IS_APPR == true
                                                    group mps by mps.MonthlyPayslipData.EMP_ID into g
                                                    select new
                                                    {
                                                        Employee_ID = g.Key,
                                                        Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                                        Aapproved_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT)
                                                    };

            var grouped_individual_payslip_categories = from ps in db.INDIVIDUAL_PAYSLIP_CATGEORY
                                                        where ps.SAL_DATE == salary_date
                                                        group ps by ps.EMP_ID into g
                                                        select new
                                                        {
                                                            Employee_ID = g.Key,
                                                            Individual_Pyaslip_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : -x.AMT),
                                                            Non_Deductionable_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : 0),
                                                            Deductionable_Amount = g.Sum(x => x.IS_DED == true ? x.AMT : 0)
                                                        };

            var payslips = (from emp in db.EMPLOYEEs
                            join gmp in grouped_monthly_payslips on emp.ID equals gmp.Employee_ID
                            join gamp in approved_grouped_monthly_payslips on emp.ID equals gamp.Employee_ID into ggamp
                            from subggamp in ggamp.DefaultIfEmpty()
                            join gipc in grouped_individual_payslip_categories on emp.ID equals gipc.Employee_ID into ggipc
                            from subgipc in ggipc.DefaultIfEmpty()
                            where emp.ID == id
                            select new SFSAcademy.Models.Payslip { EmployeeData = emp, Monthy_Payslip_Amount = gmp.Monthy_Payslip_Amount, Status = gmp.Status, Aapproved_Amount = (subggamp == null ? null : subggamp.Aapproved_Amount), Individual_Pyaslip_Amount = (subgipc == null ? null : subgipc.Individual_Pyaslip_Amount), Net_Amount = (subgipc == null ? gmp.Monthy_Payslip_Amount : gmp.Monthy_Payslip_Amount + subgipc.Individual_Pyaslip_Amount), Net_Non_Deductionable_Amount = (subgipc == null ? gmp.Non_Deductionable_Amount : gmp.Non_Deductionable_Amount + subgipc.Non_Deductionable_Amount), Net_Deductionable_Amount = (subgipc == null ? gmp.Deductionable_Amount : gmp.Deductionable_Amount + subgipc.Deductionable_Amount) }).Distinct();

            ViewData["salary"] = payslips;
            var bank_details = (from emp in db.EMPLOYEEs
                                join ebd in db.EMPLOYEE_BANK_DETAIL on emp.ID equals ebd.EMP_ID into gebd
                                from subgebd in gebd.DefaultIfEmpty()
                                join bf in db.BANK_FIELD on subgebd.BANK_FLD_ID equals bf.ID into gbf
                                from subgbf in gbf.DefaultIfEmpty()
                                where emp.ID == id
                                select new SFSAcademy.Models.EmployeeBankDetail { EmployeedData = emp, BankDetailData = (subgebd == null ? null : subgebd), BankFieldData = (subgbf == null ? null : subgbf) })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
            ViewData["bank_details"] = bank_details;

            return View(Employee);
        }


        public ActionResult Payslip_Approve()
        {
            var PSL_Sal_date = (from psl in db.MONTHLY_PAYSLIP
                                select new { Salary_date = psl.SAL_DATE }).Distinct()
                         .OrderBy(x => x.Salary_date).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in PSL_Sal_date)
            {
                string sala_Month_Year = string.Concat(((DateTime)item.Salary_date.Value).ToString("MMMM"), "-", item.Salary_date.Value.Year);
                var result = new SelectListItem();
                result.Text = sala_Month_Year;
                result.Value = item.Salary_date.ToString();
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select a month" });
            ViewBag.salary_dates = options;

            return View();
        }

        public ActionResult One_Click_Approve(string salary_date)
        {
            DateTime salary_date_val = DateTime.Parse(salary_date);
            ViewBag.salary_date = salary_date_val;
            var monthly_payslips = (from emp in db.EMPLOYEEs
                                    join mp in db.MONTHLY_PAYSLIP on emp.ID equals mp.EMP_ID
                                    join pc in db.PAYROLL_CATEGORY on mp.PYRL_CAT_ID equals pc.ID
                                    where mp.SAL_DATE == salary_date_val && mp.IS_APPR == false
                                    select new Models.MonthyPayslip { EmployeeData = emp, MonthlyPayslipData = mp, PayrollCatogaryData = pc }).OrderBy(x => x.PayrollCatogaryData.NAME).ToList();
            ViewData["dates"] = monthly_payslips;

            return PartialView("_One_Click_Approve");
        }

        public ActionResult One_Click_Approve_Submit(DateTime? date)
        {
            ViewBag.salary_date = date;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var dates = db.MONTHLY_PAYSLIP.Where(x=>x.SAL_DATE == date).ToList();
            foreach(var item in dates)
            {
                item.IS_APPR = true;
                item.APRV_ID = UserId;
                db.Entry(item).State = EntityState.Modified;
            }
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.Notice = string.Concat(ViewBag.Notice, "|", ve.ErrorMessage);
                    }
                }
                return RedirectToAction("HR", new { Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.Notice = string.Concat(ViewBag.Notice, "|", e.InnerException.InnerException.Message);
                return RedirectToAction("HR", new { Notice = ViewBag.Notice });
            }
            ViewBag.Notice = "Payslip has been approved.";
            return RedirectToAction("HR", new { Notice = ViewBag.Notice });
        }

        public ActionResult Employee_Attendance()
        {           
            return View();
        }

        public ActionResult Subject_Assignment(string Notice)
        {
            ViewBag.Notice = Notice;
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false
                                    select new Models.CoursesBatch { CourseData = cs, BatchData = bt})
                        .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.BTCH_ID = options;
            return View();
        }

        public ActionResult Update_Subjects( int? batch_id)
        {
            var batch = db.BATCHes.Where(x=>x.ID == batch_id).Include(x=>x.COURSE);
            ViewData["batch"] = batch;
            List<SelectListItem> options = new SelectList(db.SUBJECTs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).OrderBy(c => c.NAME), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select a Subject" });
            ViewBag.SUB_ID = options;

            return PartialView("_Subjects");
        }
        public ActionResult Select_Department(int? subject_id)
        {
            SUBJECT subject = db.SUBJECTs.Find(subject_id);
            ViewData["subject"] = subject;
            var assigned_employee = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == subject_id).ToList();
            ViewData["assigned_employee"] = assigned_employee;
            var departments = db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).ToList();
            ViewData["departments"] = departments;
            var Employee = db.EMPLOYEEs.ToList();
            ViewData["Employee"] = Employee;
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).OrderBy(c => c.NAMES), "ID", "NAMES").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select a Department" });
            ViewBag.DEPT_ID = options;

            return PartialView("_Select_Department");
        }

        public ActionResult Update_Employees(int? department_id, int? subject_id)
        {
            SUBJECT subject = db.SUBJECTs.Find(subject_id);
            ViewData["subject"] = subject;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == department_id && x.STAT == true).ToList();
            ViewData["employees"] = employees;
            var EmployeesSubject = db.EMPLOYEES_SUBJECT.ToList();
            ViewData["EmployeesSubject"] = EmployeesSubject;

            return PartialView("_Employee_List");
        }

        public ActionResult Assign_Employee(int? id, int? id1)
        {
            var departments = db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).ToList();
            ViewData["departments"] = departments;
            SUBJECT subject = db.SUBJECTs.Find(id1);
            ViewData["subject"] = subject;
            var employee_department_id = db.EMPLOYEEs.Find(id).EMP_DEPT_ID;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == employee_department_id && x.STAT == true).ToList();
            ViewData["employees"] = employees;
            EMPLOYEES_SUBJECT EmployeesSubject = new EMPLOYEES_SUBJECT{ EMP_ID = id, SUBJ_ID = id1};
            db.EMPLOYEES_SUBJECT.Add(EmployeesSubject);
            db.SaveChanges();
            var assigned_employee = db.EMPLOYEES_SUBJECT.Where(x => x.SUBJ_ID == subject.ID).ToList();
            ViewData["assigned_employee"] = assigned_employee;
            ViewBag.Notice = "Employee Successfully assigned.";
            return RedirectToAction("Subject_Assignment", new { Notice = ViewBag.Notice });
        }

        public ActionResult Remove_Employee(int? id, int? id1)
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
                EMPLOYEES_SUBJECT EmployeesSubject = db.EMPLOYEES_SUBJECT.Where(x=>x.EMP_ID == id && x.SUBJ_ID == id1).FirstOrDefault();
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

            return RedirectToAction("Subject_Assignment", new { Notice = ViewBag.Notice});
        }

        /////Document Upload related methods////////////////////////////////////////////////////////////////

        [HttpGet]
        public ActionResult Show(int? id)
        {
            string mime;
            byte[] bytes = LoadImage(id.Value, out mime);
            return File(bytes, mime);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            SuccessModel viewModel = new SuccessModel();
            if (Request.Files.Count == 1)
            {
                var name = Request.Files[0].FileName;
                var size = Request.Files[0].ContentLength;
                var type = Request.Files[0].ContentType;
                int PhotoId = 0;
                //viewModel.Success = HandleUpload(Request.Files[0].InputStream, name, size, type, PhotoId);
                PhotoId = HandleUpload(Request.Files[0].InputStream, name, size, type, PhotoId);
            }
            return Json(viewModel);
        }

        private int HandleUpload(Stream fileStream, string name, int size, string type, int PhotoId)
        {
            bool handled = false;

            try
            {
                // Convert image to buffered stream
                var imageBufferedStream = new BufferedStream(fileStream);
                byte[] documentBytes = new byte[imageBufferedStream.Length];
                imageBufferedStream.Read(documentBytes, 0, documentBytes.Length);


                if (PhotoId == 0 || PhotoId.Equals(null))
                {
                    IMAGE_DOCUMENTS databaseDocument = new IMAGE_DOCUMENTS
                    {
                        CREATEDON = DateTime.Now,
                        FILECONTENT = documentBytes,
                        ISDELETED = false,
                        NAME = name,
                        SIZE = size,
                        TYPE = type
                    };

                    db.IMAGE_DOCUMENTS.Add(databaseDocument);
                    handled = (db.SaveChanges() > 0);
                    PhotoId = databaseDocument.DOCUMENTID;
                }
                else
                {
                    IMAGE_DOCUMENTS ImagetoUpdate = db.IMAGE_DOCUMENTS.Find(PhotoId);
                    ImagetoUpdate.CREATEDON = DateTime.Now;
                    ImagetoUpdate.FILECONTENT = documentBytes;
                    ImagetoUpdate.NAME = name;
                    ImagetoUpdate.SIZE = size;
                    ImagetoUpdate.TYPE = type;

                    db.Entry(ImagetoUpdate).State = EntityState.Modified;
                    handled = (db.SaveChanges() > 0);
                }

            }
            catch (Exception ex)
            {
                throw ex; // Oops, something went wrong, handle the exception
            }

            return PhotoId;
        }

        private byte[] LoadImage(int id, out string type)
        {
            byte[] fileBytes = null;
            string fileType = null;
            var databaseDocument = db.IMAGE_DOCUMENTS.FirstOrDefault(doc => doc.DOCUMENTID == id);
            if (databaseDocument != null)
            {
                fileBytes = databaseDocument.FILECONTENT;
                fileType = databaseDocument.TYPE;
            }
            type = fileType;
            return fileBytes;
        }

        /////End of Document Upload related methods////////////////////////////////////////////////////////////////
    }
}
