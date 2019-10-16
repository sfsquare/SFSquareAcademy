using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;
using System.IO;
using System.Data.Entity.Validation;

namespace SFSAcademy.Controllers
{
    public class BatchesController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: BATCH
        public ActionResult Index()
        {
            var bATCHes = db.BATCHes.Include(b => b.COURSE);
            return View(bATCHes.ToList());
        }

        public ActionResult Show(int id)
        {
            BATCH batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            var students = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).ToList();
            ViewData["students"] = students;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            USER current_user = db.USERS.Find(UserId);
            ViewData["current_user"] = current_user;
            return View();
        }
        public ActionResult Assign_Tutor(int id)
        {
            BATCH batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            if (batch.EMP_ID != null)
            {
                ViewData["assigned_employee"] = HtmlHelpers.ApplicationHelper.SplitCommaString(batch.EMP_ID);
            }
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x=>x.STAT == true).OrderBy(x => x.NAMES), "ID", "NAMES").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Department" });
            ViewBag.DEPT_ID = options;

            return View();
        }
        public ActionResult Update_Employees(int? department_id, int? batch_id)
        {
            ViewBag.department_id = department_id;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == department_id).ToList();
            ViewData["employees"] = employees;
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewData["batch"] = batch;
            return PartialView("_Employee_List");
        }
        public JsonResult Assign_Employee(int? id, int? department_id, int? batch_id)
        {
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewBag.department_id = department_id;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == department_id).ToList();
            ViewData["employees"] = employees;
            List<string> assigned_emps = new List<string>();
            if (batch.EMP_ID != null)
            {
                assigned_emps = HtmlHelpers.ApplicationHelper.SplitCommaString(batch.EMP_ID).ToList();
            }
            assigned_emps.Add(id.ToString());
            batch.EMP_ID = string.Join(",", assigned_emps);
            db.Entry(batch).State = EntityState.Modified;
            db.SaveChanges();
            ViewData["batch"] = batch;
            ViewData["assigned_employee"] = HtmlHelpers.ApplicationHelper.SplitCommaString(batch.EMP_ID);

            var TutorPartialView = RenderRazorViewToString(this.ControllerContext, "_Assigned_Tutor_List", null);        
            var EmployeePartialView = RenderRazorViewToString(this.ControllerContext, "_Employee_List", null);

            return Json(new { TutorPartialView, EmployeePartialView }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Remove_Employee(int? id, int? department_id, int? batch_id)
        {
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewBag.department_id = department_id;
            var employees = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == department_id).ToList();
            ViewData["employees"] = employees;
            List<string> assigned_emps = new List<string>();
            if (batch.EMP_ID != null)
            {
                assigned_emps = HtmlHelpers.ApplicationHelper.SplitCommaString(batch.EMP_ID).ToList();
            }
            assigned_emps.Remove(id.ToString());
            batch.EMP_ID = string.Join(",", assigned_emps);
            db.Entry(batch).State = EntityState.Modified;
            db.SaveChanges();
            ViewData["batch"] = batch;
            ViewData["assigned_employee"] = HtmlHelpers.ApplicationHelper.SplitCommaString(batch.EMP_ID);

            var TutorPartialView = RenderRazorViewToString(this.ControllerContext, "_Assigned_Tutor_List", null);
            var EmployeePartialView = RenderRazorViewToString(this.ControllerContext, "_Employee_List", null);

            return Json(new { TutorPartialView, EmployeePartialView }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Update_Batch(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null && !searchString.Equals("-1"))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var CourseBatchS = (from cs in db.COURSEs
                                join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                where cs.IS_DEL == false && bt.IS_DEL == false
                                orderby cs.CODE
                                select new CoursesBatch { CourseData = cs, BatchData = bt}).Distinct();

            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("ALL"))
            {
                if(searchString == "Active")
                {
                    CourseBatchS = CourseBatchS.Where(s => s.BatchData.IS_ACT == true);
                }
                else if (searchString == "Inactive")
                {
                    CourseBatchS = CourseBatchS.Where(s => s.BatchData.IS_ACT == false);
                }
            }
            switch (sortOrder)
            {
                case "name_desc":
                    CourseBatchS = CourseBatchS.OrderByDescending(s => s.BatchData.NAME);
                    break;
                case "Date":
                    CourseBatchS = CourseBatchS.OrderBy(s => s.BatchData.START_DATE);
                    break;
                case "date_desc":
                    CourseBatchS = CourseBatchS.OrderByDescending(s => s.BatchData.START_DATE);
                    break;
                default:  // Name ascending 
                    CourseBatchS = CourseBatchS.OrderBy(s => s.BatchData.NAME);
                    break;
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(CourseBatchS.ToPagedList(pageNumber, pageSize));
        }

        // GET: BATCH/Create
        public ActionResult New(int? course_id)
        {
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;        
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,CRS_ID,START_DATE,END_DATE,IS_DEL,IS_ACT,EMP_ID,GRADING_TYPE,IMPORT_FEES,IMPORT_SUBJECTS")] BATCH batch)
        {
            COURSE course = db.COURSEs.Find(batch.CRS_ID);
            ViewData["course"] = course;
            if (ModelState.IsValid)
            {
                batch.IS_DEL = false;
                batch.IS_ACT = true;
                db.BATCHes.Add(batch);
                List<string> msg = new List<string>();
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = "New Batch created successfully.";
                    if(batch.IMPORT_SUBJECTS != false)
                    {
                        msg.Add("<ol>");
                        int? course_id = batch.CRS_ID;
                        //BATCH previous_batch = db.BATCHes.Include(x => x.SUBJECTs.Where(p => p.IS_DEL == false)).Where(x => x.ID < batch.ID && x.IS_DEL == false && x.CRS_ID == course_id).OrderByDescending(x => x.ID).ToList().FirstOrDefault();
                        BATCH previous_batch = (from bt in db.BATCHes
                                                join sub in db.SUBJECTs.Where(x => x.IS_DEL == false) on bt.ID equals sub.BTCH_ID
                                                where bt.ID < batch.ID && bt.IS_DEL == false && bt.CRS_ID == course_id
                                                select bt).OrderByDescending(x => x.ID).FirstOrDefault();
                        if (previous_batch != null)
                        {
                            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == previous_batch.ID && x.IS_DEL == false).ToList();
                            foreach(var subject in subjects)
                            {
                                if(subject.ELECTIVE_GRP_ID == null)
                                {
                                    SUBJECT NewSub = new SUBJECT() { NAME = subject.NAME, CODE = subject.CODE, BTCH_ID = batch.ID, NO_EXAMS = subject.NO_EXAMS, MAX_WKILY_CLSES = subject.MAX_WKILY_CLSES, ELECTIVE_GRP_ID = subject.ELECTIVE_GRP_ID, CR_HRS = subject.CR_HRS, IS_DEL = false };
                                    db.SUBJECTs.Add(NewSub);
                                }
                                else
                                {
                                    ELECTIVE_GROUP elect_group_exists = db.ELECTIVE_GROUP.Where(x => x.ELECTIVE_GRP_NAME == db.ELECTIVE_GROUP.Find(subject.ELECTIVE_GRP_ID).ELECTIVE_GRP_NAME && x.BTCH_ID == batch.ID).FirstOrDefault();
                                    if(elect_group_exists == null)
                                    {
                                        ELECTIVE_GROUP elect_group = new ELECTIVE_GROUP() { ELECTIVE_GRP_NAME = db.ELECTIVE_GROUP.Find(subject.ELECTIVE_GRP_ID).ELECTIVE_GRP_NAME, BTCH_ID = batch.ID, IS_DELETED = false };
                                        db.ELECTIVE_GROUP.Add(elect_group);
                                        SUBJECT NewSub = new SUBJECT() { NAME = subject.NAME, CODE = subject.CODE, BTCH_ID = batch.ID, NO_EXAMS = subject.NO_EXAMS, MAX_WKILY_CLSES = subject.MAX_WKILY_CLSES, ELECTIVE_GRP_ID = elect_group.ID, CR_HRS = subject.CR_HRS, IS_DEL = false };
                                        db.SUBJECTs.Add(NewSub);
                                    }
                                    else
                                    {
                                        SUBJECT NewSub = new SUBJECT() { NAME = subject.NAME, CODE = subject.CODE, BTCH_ID = batch.ID, NO_EXAMS = subject.NO_EXAMS, MAX_WKILY_CLSES = subject.MAX_WKILY_CLSES, ELECTIVE_GRP_ID = elect_group_exists.ID, CR_HRS = subject.CR_HRS, IS_DEL = false };
                                        db.SUBJECTs.Add(NewSub);                                       
                                    }
                                }
                            }
                            db.SaveChanges();
                            msg.Add("</ol>");
                        }
                        else
                        {
                            msg = null;
                            ViewBag.no_subject_error = "No subjects found in previous batches.";
                        }
                    }
                    ViewBag.subject_import = string.Join("", msg);
                    ViewBag.Notice = string.Concat(ViewBag.Notice, ViewBag.subject_import);
                    string err = "";
                    string err1 = "<span style = 'margin-left:15px;font-size:15px,margin-bottom:20px;'><b>#{t('following_pblm_occured_while_saving_the_batch')}</b></span>";
                    List<string> fee_msg = new List<string>();
                    if (batch.IMPORT_FEES != false)
                    {
                        int? course_id = batch.CRS_ID;
                        //BATCH previous_batch = db.BATCHes.Include(x => x.FINANCE_FEE_CATGEORY.Where(x => x.IS_DEL == false && x.IS_MSTR == true)).Where(x => x.ID < batch.ID && x.IS_DEL == false && x.CRS_ID == course_id).OrderByDescending(x => x.ID).FirstOrDefault();
                        BATCH previous_batch = (from bt in db.BATCHes
                                                join fcat in db.FINANCE_FEE_CATGEORY.Where(x => x.IS_DEL == false && x.IS_MSTR == false) on bt.ID equals fcat.BTCH_ID
                                                where bt.ID < batch.ID && bt.IS_DEL == false && bt.CRS_ID == course_id
                                                select bt).OrderByDescending(x => x.ID).FirstOrDefault();
                        if (previous_batch != null)
                        {
                            fee_msg.Add("<ol>");
                            var categories = db.FINANCE_FEE_CATGEORY.Include(x=>x.FINANCE_FEE_PARTICULAR).Where(x => x.BTCH_ID == previous_batch.ID && x.IS_DEL == false && x.IS_MSTR == false).ToList();
                            foreach(var c in categories)
                            {
                                var particulars = c.FINANCE_FEE_PARTICULAR.Where(x => x.ADMSN_NO == null && x.STDNT_ID == null && x.IS_DEL == "N").ToList();
                                particulars = particulars.Where(x => x.Deleted_Category() == false).ToList();
                                var batch_discounts = db.FEE_DISCOUNT.Where(x => x.FIN_FEE_CAT_ID == c.ID && x.TYPE == "Batch").ToList();
                                var category_discounts = db.FEE_DISCOUNT.Where(x => x.FIN_FEE_CAT_ID == c.ID && x.TYPE == "Student Category").ToList();
                                if(!(particulars == null && particulars.Count() == 0) || !(batch_discounts == null && batch_discounts.Count() == 0) || !(category_discounts == null && category_discounts.Count() == 0))
                                {
                                    FINANCE_FEE_CATGEORY new_category = new FINANCE_FEE_CATGEORY() { NAME = c.NAME, DESCR = c.DESCR, BTCH_ID = batch.ID, IS_DEL = false, IS_MSTR = false, MSTR_CATGRY_ID = c.MSTR_CATGRY_ID, FEE_FREQ = c.FEE_FREQ };
                                    db.FINANCE_FEE_CATGEORY.Add(new_category);
                                    try
                                    {
                                        db.SaveChanges();
                                        fee_msg.Add("<li>"+ c.NAME +"</li>");
                                        foreach(var p in particulars)
                                        {
                                            FINANCE_FEE_PARTICULAR new_particular = new FINANCE_FEE_PARTICULAR() { NAME = p.NAME, DESCR = p.DESCR, AMT = p.AMT, STDNT_CAT_ID = p.STDNT_CAT_ID, ADMSN_NO = p.ADMSN_NO, STDNT_ID = p.STDNT_ID,IS_DEL = "N" };
                                            new_particular.FIN_FEE_CAT_ID = new_category.ID;
                                            db.FINANCE_FEE_PARTICULAR.Add(new_particular);
                                            try
                                            {
                                                db.SaveChanges();
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e);
                                                err += "<li> #{t('particular')} #{p.name} #{t('import_failed')}.</li>";
                                            }
                                        }
                                        foreach(var disc in batch_discounts)
                                        {
                                            FEE_DISCOUNT new_disc = new FEE_DISCOUNT() { NAME = disc.NAME, RCVR_ID = batch.ID, FIN_FEE_CAT_ID = new_category.ID, DISC = disc.DISC, IS_AMT = disc.IS_AMT, FEE_CLCT_ID = disc.FEE_CLCT_ID, DESCR = disc.DESCR, DISC_DATE = disc.DISC_DATE,TYPE = disc.TYPE };
                                            db.FEE_DISCOUNT.Add(new_disc);
                                            try
                                            {
                                                db.SaveChanges();
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e);
                                                err += "<li> #{t('discount ')} #{disc.name} #{t('import_failed')}.</li>";
                                            }
                                        }
                                        foreach (var disc in category_discounts)
                                        {
                                            FEE_DISCOUNT new_disc = new FEE_DISCOUNT() { NAME = disc.NAME, RCVR_ID = disc.RCVR_ID, FIN_FEE_CAT_ID = new_category.ID, DISC = disc.DISC, IS_AMT = disc.IS_AMT, FEE_CLCT_ID = disc.FEE_CLCT_ID, DESCR = disc.DESCR, DISC_DATE = disc.DISC_DATE, TYPE = disc.TYPE };
                                            db.FEE_DISCOUNT.Add(new_disc);
                                            try
                                            {
                                                db.SaveChanges();
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e);
                                                err += "<li>  #{t(' discount ')} #{disc.name} #{t(' import_failed')}.</li><br/>";
                                            }
                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                        err += "<li>  #{t('category')} #{c.name}1 #{t('import_failed')}.</li>";
                                    }

                                }
                                else
                                {
                                    err += "<li>  #{t('category')} #{c.name}2 #{t('import_failed')}.</li>";
                                }

                            }
                            fee_msg.Add("</ol>");
                            ViewBag.fee_import_error = false;
                        }
                        else
                        {
                            ViewBag.fee_import_error = true;
                        }
                    }
                    if(err != "")
                    {
                        ViewBag.Warn_Notice = err1 + err;
                    }
                    if (fee_msg != null && fee_msg.Count() != 0)
                    {
                        ViewBag.fees_import = string.Join("", fee_msg);
                    }
                    ViewBag.Notice = "Batch created successfully.";
                    return RedirectToAction("Show", "Courses", new { id = batch.CRS_ID, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    List<string> grade_types = new List<string>();
                    string gpa = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "GPA").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
                    if(gpa == "1")
                    {
                        grade_types.Add(gpa);
                    }
                    string cwa = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CWA").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
                    if (cwa == "1")
                    {
                        grade_types.Add(cwa);
                    }
                    ViewData["grade_types"] = grade_types;
                    ViewBag.ErrorMessage = "Batch creation failed with exception: " + string.Concat(e.GetType().FullName, ":", e.Message);
                    return View(batch);
                }
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State. PLease contact administrator.";
            return View(batch);
        }

        // GET: BATCH/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BATCH bATCH = db.BATCHes.Include(x=>x.COURSE).Where(x=>x.ID == id).FirstOrDefault();
            if (bATCH == null)
            {
                return HttpNotFound();
            }
            COURSE course = db.COURSEs.Find(bATCH.CRS_ID);
            ViewData["course"] = course;

            return View(bATCH);
        }

        // POST: BATCH/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,CRS_ID,START_DATE,END_DATE,IS_DEL,EMP_ID,GRADING_TYPE")] BATCH bATCH)
        {
            if (ModelState.IsValid)
            {
                BATCH BatchToUpdate = db.BATCHes.Find(bATCH.ID);
                BatchToUpdate.NAME = bATCH.NAME;
                BatchToUpdate.CRS_ID = bATCH.CRS_ID;
                BatchToUpdate.START_DATE = bATCH.START_DATE;
                BatchToUpdate.END_DATE = bATCH.END_DATE;
                BatchToUpdate.EMP_ID = bATCH.EMP_ID;
                BatchToUpdate.GRADING_TYPE = bATCH.GRADING_TYPE;
                db.Entry(BatchToUpdate).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = string.Concat(ViewBag.Notice, "Batch updated successfully.");
                    return RedirectToAction("Show", "Courses", new { id = bATCH.CRS_ID, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Error Occured. ", string.Concat(e.GetType().FullName, ":", e.Message));
                    return RedirectToAction("Show", "Courses", new { id = bATCH.CRS_ID, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            ViewBag.Notice = "There seems to be some issue with Model State";
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", bATCH.CRS_ID);
            return View(bATCH);
        }

        // GET: BATCH/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BATCH batch = db.BATCHes.Find(id);
            if (batch == null)
            {
                return HttpNotFound();
            }
            if((db.STUDENTs.Where(x=>x.BTCH_ID == id) == null && db.STUDENTs.Where(x => x.BTCH_ID == id).Count() ==0) && (db.SUBJECTs.Where(x=>x.BTCH_ID == id) == null && db.SUBJECTs.Where(x => x.BTCH_ID == id).Count() == 0))
            {
                batch.Inactivate();
                ViewBag.Notice = string.Concat(ViewBag.Notice, "Batch is deleted successfully");
                return RedirectToAction("Show", "Courses", new { id = batch.CRS_ID, Notice = ViewBag.Notice });
            }
            else
            {
                if(batch.All_Students() != null && batch.All_Students().Count() != 0)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Unable to delete Batch.Please delete all Students. ");
                }
                if (db.SUBJECTs.Where(x => x.BTCH_ID == id) != null && db.SUBJECTs.Where(x => x.BTCH_ID == id).Count() != 0)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Unable to delete Batch.Please delete all Subjects. ");
                }
                return RedirectToAction("Show", "Courses", new { id = batch.CRS_ID, ErrorMessage = ViewBag.ErrorMessage });
            }
        }

        public ActionResult Activate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BATCH bATCH = db.BATCHes.Find(id);
            if (bATCH == null)
            {
                return HttpNotFound();
            }
            bATCH.IS_ACT = true;
            db.Entry(bATCH).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                ViewBag.Notice = string.Concat(ViewBag.Notice, "Batch is activated successfully");
                return RedirectToAction("Show", "Courses", new { id = bATCH.CRS_ID, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Error Occured. ", string.Concat(e.GetType().FullName, ":", e.Message));
                return RedirectToAction("Show", "Courses", new { id = bATCH.CRS_ID, ErrorMessage = ViewBag.ErrorMessage });
            }
        }

        public ActionResult Deactivate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BATCH bATCH = db.BATCHes.Find(id);
            if (bATCH == null)
            {
                return HttpNotFound();
            }
            bATCH.IS_ACT = false;
            db.Entry(bATCH).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                ViewBag.Notice = string.Concat(ViewBag.Notice, "Batch is Deactivated successfully");
                return RedirectToAction("Show", "Courses", new { id = bATCH.CRS_ID, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Error Occured. ", string.Concat(e.GetType().FullName, ":", e.Message));
                return RedirectToAction("Show", "Courses", new { id = bATCH.CRS_ID, ErrorMessage = ViewBag.ErrorMessage });
            }
        }

        [AllowAnonymous]
        public JsonResult StGraterThenEnd(DateTime? START_DATE, DateTime? END_DATE)
        {
            return Json(START_DATE < END_DATE, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            if (model != null)
            {
                controllerContext.Controller.ViewData.Model = model;
            }

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
