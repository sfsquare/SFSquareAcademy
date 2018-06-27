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

        public ActionResult HR()
        {
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var EmployeeData = (from emp in db.EMPLOYEEs
                                join ec in db.EMPLOYEE_CATEGORY on emp.EMP_CAT_ID equals ec.ID
                                where emp.USRID == UserId
                                select new Models.Employee { EmployeeData = emp, CategoryData = ec}).Distinct();
            //EMPLOYEE Employee = db.EMPLOYEEs.Find(UserId);

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
                             where ec.STAT == "Y"
                             select new Models.EmployeeCategory { CategoryData = ec })
                            .OrderBy(x => x.CategoryData.NAME).ToList();
            ViewData["categories"] = categories;

            var inactive_categories = (from ec in db.EMPLOYEE_CATEGORY
                              where ec.STAT == "N"
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
                                  where ec.STAT == "Y"
                                  select new Models.EmployeeCategory { CategoryData = ec })
                           .OrderBy(x => x.CategoryData.NAME).ToList();
                ViewData["categories"] = categories;

                var inactive_categories = (from ec in db.EMPLOYEE_CATEGORY
                                           where ec.STAT == "N"
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
                                where emp.EMP_CAT_ID == eMPLOYEEcTAGORY.ID && emp.STAT == "Y"
                                select emp).Distinct();
                if((eMPLOYEEcTAGORY.STAT == "N" && Employee.Count() == 0) || (eMPLOYEEcTAGORY.STAT == "Y"))
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
                    if(eMPLOYEEcTAGORY.STAT == "N")
                    {
                        var Position = (from pos in db.EMPLOYEE_POSITION
                                        join ecat in db.EMPLOYEE_CATEGORY on pos.EMP_CAT_ID equals ecat.ID
                                        where ecat.ID == eMPLOYEEcTAGORY.ID
                                        select pos).Distinct();
                        foreach(var item in Position)
                        {
                            EMPLOYEE_POSITION EmpPos = db.EMPLOYEE_POSITION.Find(item.ID);
                            EmpPos.IS_ACT = "N";
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
                             where emp.EMP_CAT_ID == id && emp.STAT == "Y"
                             select emp).Distinct();
            if(employees == null || employees.Count() == 0)
            {
                employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_CAT_ID == id && emp.STAT == "N"
                             select emp).Distinct();
            }
            var category_position = (from pos in db.EMPLOYEE_POSITION
                                     join ecat in db.EMPLOYEE_CATEGORY on pos.EMP_CAT_ID equals ecat.ID
                                     where ecat.ID == id && pos.IS_ACT == "Y"
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
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x => x.STAT == "Y").OrderBy(x=>x.NAME), "ID", "NAME");
            var positions = (from ep in db.EMPLOYEE_POSITION
                             join ecat in db.EMPLOYEE_CATEGORY on ep.EMP_CAT_ID equals ecat.ID
                              where ep.IS_ACT == "Y"
                              select new Models.EmployeePosition { PositionData = ep, CategoryData = ecat })
                            .OrderBy(x => x.PositionData.POS_NAME).ToList();
            ViewData["positions"] = positions;

            var inactive_positions = (from ep in db.EMPLOYEE_POSITION
                                      join ecat in db.EMPLOYEE_CATEGORY on ep.EMP_CAT_ID equals ecat.ID
                                      where ep.IS_ACT == "N"
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
                ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x=>x.STAT=="Y").OrderBy(x => x.NAME), "ID", "NAME");
                var positions = (from ep in db.EMPLOYEE_POSITION
                                 join ecat in db.EMPLOYEE_CATEGORY on ep.EMP_CAT_ID equals ecat.ID
                                 where ep.IS_ACT == "Y"
                                 select new Models.EmployeePosition { PositionData = ep, CategoryData = ecat })
                                .OrderBy(x => x.PositionData.POS_NAME).ToList();
                ViewData["positions"] = positions;

                var inactive_positions = (from ep in db.EMPLOYEE_POSITION
                                          join ecat in db.EMPLOYEE_CATEGORY on ep.EMP_CAT_ID equals ecat.ID
                                          where ep.IS_ACT == "N"
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
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY.Where(x => x.STAT == "Y").OrderBy(x => x.NAME), "ID", "NAME", EmployeePos.EMP_CAT_ID);

            return View(EmployeePos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Position([Bind(Include = "ID,POS_NAME,POS_DESCR,IS_ACT,EMP_CAT_ID")] EMPLOYEE_POSITION eMPLOYEEpOSITION)
        {
            if (ModelState.IsValid)
            {
                var Employee = (from emp in db.EMPLOYEEs
                                where emp.EMP_POS_ID == eMPLOYEEpOSITION.ID && emp.STAT == "Y"
                                select emp).Distinct();
                if ((eMPLOYEEpOSITION.IS_ACT == "N" && Employee.Count() == 0) || (eMPLOYEEpOSITION.IS_ACT == "Y"))
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
                             where emp.EMP_POS_ID == id && emp.STAT == "Y"
                             select emp).Distinct();
            if (employees == null || employees.Count() == 0)
            {
                employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_POS_ID == id && emp.STAT == "N"
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
                             where dp.STAT == "Y"
                             select new Models.EmployeeDepartment { DepartmentData = dp})
                            .OrderBy(x => x.DepartmentData.NAMES).ToList();
            ViewData["departments"] = departments;

            var inactive_departments = (from dp in db.EMPLOYEE_DEPARTMENT
                                        where dp.STAT == "N"
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
                                   where dp.STAT == "Y"
                                   select new Models.EmployeeDepartment { DepartmentData = dp })
                            .OrderBy(x => x.DepartmentData.NAMES).ToList();
                ViewData["departments"] = departments;

                var inactive_departments = (from dp in db.EMPLOYEE_DEPARTMENT
                                            where dp.STAT == "N"
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
                                where emp.EMP_DEPT_ID == eMPLOYEEdEPARTMENT.ID && emp.STAT == "Y"
                                select emp).Distinct();
                if ((eMPLOYEEdEPARTMENT.STAT == "N" && Employee.Count() == 0) || (eMPLOYEEdEPARTMENT.STAT == "Y"))
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
                             where emp.EMP_DEPT_ID == id && emp.STAT == "Y"
                             select emp).Distinct();
            if (employees == null || employees.Count() == 0)
            {
                employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_DEPT_ID == id && emp.STAT == "N"
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
                               where gr.IS_ACT == "Y"
                               select new Models.EmployeeGrade { GradeData = gr })
                            .OrderBy(x => x.GradeData.GRADE_NAME).ToList();
            ViewData["grades"] = grades;

            var inactive_grades = (from gr in db.EMPLOYEE_GRADE
                                   where gr.IS_ACT == "N"
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
                              where gr.IS_ACT == "Y"
                              select new Models.EmployeeGrade { GradeData = gr })
                  .OrderBy(x => x.GradeData.GRADE_NAME).ToList();
                ViewData["grades"] = grades;

                var inactive_grades = (from gr in db.EMPLOYEE_GRADE
                                       where gr.IS_ACT == "N"
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
                                where emp.EMP_GRADE_ID == eMPLOYEEgRADE.ID && emp.STAT == "Y"
                                select emp).Distinct();
                if ((eMPLOYEEgRADE.IS_ACT == "N" && Employee.Count() == 0) || (eMPLOYEEgRADE.IS_ACT == "Y"))
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
                             where emp.EMP_GRADE_ID == id && emp.STAT == "Y"
                             select emp).Distinct();
            if (employees == null || employees.Count() == 0)
            {
                employees = (from emp in db.EMPLOYEEs
                             where emp.EMP_GRADE_ID == id && emp.STAT == "N"
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
                          where bf.STAT == "Y"
                          select new Models.EmployeeBankDetail { BankFieldData = bf })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
            ViewData["bank_details"] = bank_details;

            var inactive_bank_details = (from bf in db.BANK_FIELD
                                         where bf.STAT == "N"
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
                                    where bf.STAT == "Y"
                                    select new Models.EmployeeBankDetail { BankFieldData = bf })
                            .OrderBy(x => x.BankFieldData.NAME).ToList();
                ViewData["bank_details"] = bank_details;

                var inactive_bank_details = (from bf in db.BANK_FIELD
                                             where bf.STAT == "N"
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
                             where bf.ID == id && emp.STAT == "Y"
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
                                where af.STAT == "Y"
                                select new Models.EmployeeAdditionalDetail { AdditionalFieldData = af })
                            .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
            ViewData["additional_details"] = additional_details;

            var inactive_additional_details = (from af in db.EMPLOYEE_ADDITIONAL_FIELD
                                               where af.STAT == "N"
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
                                          where af.STAT == "Y"
                                          select new Models.EmployeeAdditionalDetail { AdditionalFieldData = af })
                            .OrderBy(x => x.AdditionalFieldData.NAME).ToList();
                ViewData["additional_details"] = additional_details;

                var inactive_additional_details = (from af in db.EMPLOYEE_ADDITIONAL_FIELD
                                                   where af.STAT == "N"
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
                             where af.ID == id && emp.STAT == "Y"
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
    }
}
