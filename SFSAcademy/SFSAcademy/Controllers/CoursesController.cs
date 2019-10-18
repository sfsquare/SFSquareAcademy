using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.Web.UI.WebControls;
using System;
using SFSAcademy;
using SFSAcademy.HtmlHelpers;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.IO;
using System.Data.Entity.Validation;

namespace SFSAcademy.Controllers
{
    public class CoursesController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Courses
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage_Course(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            return View(db.COURSEs.FirstOrDefault().ACTIVE().OrderBy(x=>x.ID));
        }

        public ActionResult Manage_Batches()
        {
            List<SelectListItem> options = new SelectList(db.COURSEs.FirstOrDefault().ACTIVE().OrderBy(x => x.ID), "ID", "Full_Name").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.CRS_ID = options;
            COURSE course = db.COURSEs.Where(x => x.ID == -1).FirstOrDefault();
            ViewData["course"] = course;

            return View();
        }
        public ActionResult Update_Batch(int? course_id)
        {
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            var batch = db.BATCHes.Where(x => x.CRS_ID == course_id && x.IS_ACT == true && x.IS_DEL == false).ToList();
            ViewData["batch"] = batch;

            return PartialView("_Update_Batch");
        }
        public ActionResult Show(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            COURSE course = db.COURSEs.Find(id);
            ViewData["course"] = course;
            var batches = db.COURSEs.Find(id).BATCHes.Where(x=>x.IS_ACT == true && x.IS_DEL == false);
            ViewData["batches"] = batches;
            return View();
        }
        public ActionResult Grouped_Batches(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            COURSE course = db.COURSEs.Find(id);
            ViewData["course"] = course;
            var batch_groups = db.BATCH_GROUP.Include(x=>x.GROUPED_BATCH).Where(x=>x.CRS_ID == id).ToList();
            ViewData["batch_groups"] = batch_groups;
            var GroupdBatches = db.GROUPED_BATCH.Include(x => x.BATCH_GROUP).Where(x => x.BATCH_GROUP.CRS_ID == id).ToList();
            //var batches = course.Active_Batches().Where( x=> !GroupdBatches.Any(sp => sp.BTCH_ID == x.ID)).ToList();
            var batches = (from bt in course.Active_Batches()
                           join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                           where !GroupdBatches.Any(sp => sp.BTCH_ID == bt.ID)
                           select new BatchSelect { BatchData = bt, CourseData = cs, Select = false }).ToList();
            ViewData["batches"] = batches;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Batch_Group(IList<BatchSelect> batches, int? course_id, string NAME)
        {
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            BATCH_GROUP batch_group = new BATCH_GROUP { NAME = NAME, CRS_ID = course_id };
            bool error = false;
            if (batches.Where(x => x.Select == true) == null || batches.Where(x => x.Select == true).Count() == 0)
            {
                error = true;
            }
            if (!string.IsNullOrEmpty(NAME) && course_id != null && error == false)
            {
                db.BATCH_GROUP.Add(batch_group);
                db.SaveChanges();
                foreach (var batch in batches.Where(x => x.Select == true))
                {
                    BATCH SelBatch = db.BATCHes.Find(batch.BatchData.ID);
                    GROUPED_BATCH GroupedBatch = new GROUPED_BATCH { BTCH_GROUP_ID = batch_group.ID, BTCH_ID = SelBatch.ID };
                    db.GROUPED_BATCH.Add(GroupedBatch);
                }
                db.SaveChanges();
                return RedirectToAction("Grouped_Batches", new { id = course.ID, Notice = ViewBag.Notice });
            }
            else
            {
                if (batches.Where(x => x.Select == true) == null || batches.Where(x => x.Select == true).Count() == 0)
                {
                    ViewBag.ErrorMessage = "Atleast one batch must be selected.";
                    return View();
                }
                ViewBag.ErrorMessage = "Batch group creation failed. Contact Administrator.";
                return View();
            }
        }
        public ActionResult Edit_Batch_Group(int? id)
        {
            BATCH_GROUP batch_group = db.BATCH_GROUP.Include(x=>x.GROUPED_BATCH).Where(x=>x.ID == id).FirstOrDefault();
            ViewData["batch_group"] = batch_group;

            COURSE course = db.COURSEs.Find(batch_group.CRS_ID);
            ViewData["course"] = course;

            var GroupdBatches = db.GROUPED_BATCH.Include(x => x.BATCH_GROUP).Where(x => x.BATCH_GROUP.CRS_ID == course.ID).ToList();
            //var batches = course.Active_Batches().Where( x=> !GroupdBatches.Any(sp => sp.BTCH_ID == x.ID)).ToList();
            var assigned_batches = (from bt in course.Active_Batches()
                           join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                           where GroupdBatches.Any(sp => sp.BTCH_ID == bt.ID)
                           select new BatchSelect { BatchData = bt, CourseData = cs, Select = true }).ToList();
            ViewData["assigned_batches"] = assigned_batches;

            var batches = (from bt in course.Active_Batches()
                           join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                           where !GroupdBatches.Any(sp => sp.BTCH_ID == bt.ID)
                           select new BatchSelect { BatchData = bt, CourseData = cs, Select = false });
            batches = batches.Union(assigned_batches).ToList();
            ViewData["batches"] = batches;

            return PartialView("_Batch_Group_Edit_Form", batch_group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Batch_Group([Bind(Include = "ID,CRS_ID,NAME,CREATED_AT,UPDATED_AT")] BATCH_GROUP batch_group, IList<BatchSelect> batches, int? course_id)
        {
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            if (batches.Where(x => x.Select == true) != null && batches.Where(x => x.Select == true).Count() != 0)
            {
                batch_group.CRS_ID = course.ID;
                db.Entry(batch_group).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    var grouped_batches = db.GROUPED_BATCH.Where(x => x.BTCH_GROUP_ID == batch_group.ID).ToList();
                    foreach (var item in grouped_batches)
                    {
                        db.GROUPED_BATCH.Remove(item);
                    }
                    db.SaveChanges();
                    foreach (var batch in batches.Where(x => x.Select == true))
                    {
                        BATCH SelBatch = db.BATCHes.Find(batch.BatchData.ID);
                        GROUPED_BATCH GroupedBatch = new GROUPED_BATCH { BTCH_GROUP_ID = batch_group.ID, BTCH_ID = SelBatch.ID };
                        db.GROUPED_BATCH.Add(GroupedBatch);
                    }
                    db.SaveChanges();

                    var batch_groups = db.BATCH_GROUP.Include(x => x.GROUPED_BATCH).Where(x => x.CRS_ID == course.ID).ToList();
                    ViewData["batch_groups"] = batch_groups;
                    var GroupdBatches = db.GROUPED_BATCH.Include(x => x.BATCH_GROUP).Where(x => x.BATCH_GROUP.CRS_ID == course.ID).ToList();
                    var Newbatches = (from bt in course.Active_Batches()
                                      join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                      where !GroupdBatches.Any(sp => sp.BTCH_ID == bt.ID)
                                      select new BatchSelect { BatchData = bt, CourseData = cs, Select = false }).ToList();
                    ViewData["batches"] = Newbatches;
                    ViewBag.Notice = "Batch Group updated successfully.";
                    return RedirectToAction("Grouped_Batches", new { id = course.ID, Notice = ViewBag.Notice });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(batch_group);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(batch_group);
                }
            }
            else
            {
                if (batches.Where(x => x.Select == true) == null || batches.Where(x => x.Select == true).Count() == 0)
                {
                    ViewBag.ErrorMessage = "Atleast one batch must be selected.";
                    return View(batch_group);
                }
                ViewBag.ErrorMessage = "Batch group creation failed. Contact Administrator.";
                return View(batch_group);
            }
        }
        public ActionResult Delete_Batch_Group(int? id)
        {
            BATCH_GROUP batch_group = db.BATCH_GROUP.Include(x => x.GROUPED_BATCH).Where(x => x.ID == id).FirstOrDefault();
            COURSE course = db.COURSEs.Find(batch_group.CRS_ID);
            ViewData["course"] = course;
            var grouped_batches = db.GROUPED_BATCH.Where(x => x.BTCH_GROUP_ID == batch_group.ID).ToList();
            foreach (var item in grouped_batches)
            {
                db.GROUPED_BATCH.Remove(item);
            }
            db.BATCH_GROUP.Remove(batch_group);
            try
            {
                db.SaveChanges();
                var batch_groups = db.BATCH_GROUP.Include(x => x.GROUPED_BATCH).Where(x => x.CRS_ID == course.ID).ToList();
                ViewData["batch_groups"] = batch_groups;
                var GroupdBatches = db.GROUPED_BATCH.Include(x => x.BATCH_GROUP).Where(x => x.BATCH_GROUP.CRS_ID == course.ID).ToList();
                var batches = (from bt in course.Active_Batches()
                               join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                               where !GroupdBatches.Any(sp => sp.BTCH_ID == bt.ID)
                               select new BatchSelect { BatchData = bt, CourseData = cs, Select = false }).ToList();
                ViewData["batches"] = batches;
                ViewBag.Notice = "Batch Group deleted successfully.";
                return PartialView("_Batch_Groups");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return View(batch_group);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return View(batch_group);
            }           
        }
        public ActionResult New()
        {

            DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
            ViewBag.ReturnDate = PDate.ToShortDateString();
            DateTime PDate2 = Convert.ToDateTime(System.DateTime.Now.AddYears(1));
            ViewBag.ReturnDate2 = PDate2.ToShortDateString();
            ViewBag.BATCH_NAME = "";

            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "ID,CRS_NAME,CODE,SECTN_NAME,IS_DEL,CREATED_AT,UPDATED_AT,GRADING_TYPE,INIT_BATCH_NAME")] COURSE cOURSE, DateTime ReturnDate, DateTime ReturnDate2)
        {
            DateTime PDate = Convert.ToDateTime(ReturnDate);
            ViewBag.ReturnDate = PDate.ToShortDateString();
            DateTime PDate2 = Convert.ToDateTime(ReturnDate2);
            ViewBag.ReturnDate2 = PDate2.ToShortDateString();
            ViewBag.BATCH_NAME = cOURSE.INIT_BATCH_NAME;
            if (ModelState.IsValid)
            {
                cOURSE.IS_DEL = false;
                db.COURSEs.Add(cOURSE);
                BATCH bATCH = new BATCH() { NAME = cOURSE.INIT_BATCH_NAME, START_DATE = ReturnDate,END_DATE = ReturnDate2,CRS_ID = cOURSE.ID,IS_DEL = false, IS_ACT = true};
                db.BATCHes.Add(bATCH);
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = "Course created successfully!";
                    return RedirectToAction("Manage_Course",new { Notice = ViewBag.Notice });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(cOURSE);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(cOURSE);
                }
            }
            ViewBag.ErrorMessage = "There is some issue adding course.";
            return View(cOURSE);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COURSE cOURSE = db.COURSEs.Find(id);
            if (cOURSE == null)
            {
                return HttpNotFound();
            }
            return View(cOURSE);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CRS_NAME,CODE,SECTN_NAME,IS_DEL,CREATED_AT,UPDATED_AT,GRADING_TYPE")] COURSE cOURSE)
        {
            if (ModelState.IsValid)
            {
                cOURSE.IS_DEL = false;
                db.Entry(cOURSE).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = "Course updated successfully!";
                    return RedirectToAction("Manage_Course", new { Notice = ViewBag.Notice });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(cOURSE);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(cOURSE);
                }
            }
            ViewBag.ErrorMessage = "There seems to eb some issue with Model State. PLease contact administrator.";
            return View(cOURSE);
        }

        public ActionResult Destroy(int id)
        {
            COURSE course = db.COURSEs.Find(id);

            if (course.BATCHes.FirstOrDefault().ACTIVE() == null || course.BATCHes.FirstOrDefault().ACTIVE().Count() == 0)
            {
                course.Inactivate();
                ViewBag.Notice = "Course deleted successfully.";
                return RedirectToAction("Manage_Course", new { Notice = ViewBag.Notice});
            }
            else
            {
                 ViewBag.ErrorMessage = "Unable to Delete. Please remove existing batches and students.";
                return RedirectToAction("Manage_Course", new { ErrorMessage = ViewBag.ErrorMessage });
            }

        }

        [AllowAnonymous]
        public JsonResult IsCodeExist([Bind(Prefix = "CODE")] string CODE)
        {
            COURSE course = db.COURSEs.Where(x => x.CODE == CODE).FirstOrDefault();
            return Json(course == null, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult InitialBatch(string INIT_BATCH_NAME, string CODE)
        {
            return Json(!(!string.IsNullOrEmpty(CODE) && string.IsNullOrEmpty(INIT_BATCH_NAME)), JsonRequestBehavior.AllowGet);
        }
        /*
        public JsonResult CCEWeightage(string CODE)
        {
            COURSE course = db.COURSEs.Where(x => x.CODE == CODE).FirstOrDefault();
            var cce_weightages_group = db.CCE_WEIGHTAGES_COURSE.Where(x => x.CRS_ID == course.ID).Include(x=>x.CCE_WEIGHTAGE).Select(x=>x.CCE_WEIGHTAGE).GroupBy(o => o.CRITA_TYPE).Select(g => new { Criteria = g.Key, Unique_Exam_Cat = g.Max(p => p.CCE_EXAM_CAT_ID), Exam_Cat = g.Sum(x => x.CCE_EXAM_CAT_ID) });
            bool flag = false;
            foreach (var v in cce_weightages_group)
            {
                if (v.Unique_Exam_Cat != v.Exam_Cat)
                {
                    flag = true;
                    break;
                }
            }
            return Json(flag == true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CourseValidation([Bind(Prefix = "CODE")] string CODE, [Bind(Prefix = "INIT_BATCH_NAME")] string INIT_BATCH_NAME)
        {
            COURSE course = db.COURSEs.Where(x => x.CODE == CODE).FirstOrDefault();
            if (course != null)
            {
                return Json("Course code already exist in database. Choose another one.", JsonRequestBehavior.AllowGet);
            }
            //return Json(!db.GRADING_LEVEL.Any(x => x.NAME.ToUpper() == NAME.ToUpper() && x.IS_DEL == false), JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(INIT_BATCH_NAME))
            {
                return Json("Initial Batch must be added for this course.", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        */
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
