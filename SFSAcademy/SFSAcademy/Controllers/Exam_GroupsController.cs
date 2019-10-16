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
    public class Exam_GroupsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Exam_Groups
        public ActionResult Index(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            BATCH batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            var exam_groups = db.EXAM_GROUP.Where(x => x.BTCH_ID == batch.ID).ToList();
            ViewData["exam_groups"] = exam_groups;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            if (current_user.User.EMP_IND == true)
            {
                var user_privileges = current_user.privilage_list;
                ViewData["user_privileges"] = user_privileges;
                var employee_subjects = db.EMPLOYEES_SUBJECT.Include(x=>x.SUBJECT).Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).ToList();
                ViewData["employee_subjects"] = employee_subjects;
                if((employee_subjects == null || employee_subjects.Count() == 0) && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl") && !user_privileges.Select(x => x.NAME).Contains("EnterResults"))
                {
                    ViewBag.ErrorMessage = "Sorry, you are not allowed to access that page.";
                    return RedirectToAction("Dashboard", "User",new { id  = current_user.User.ID, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            return View();
        }

        // GET: Exam_Groups/Create
        public ActionResult New(int? id, int? exam_group_id, string ename)
        {
            BATCH batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            COURSE course = db.COURSEs.Find(batch.CRS_ID);
            ViewData["course"] = course;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            var user_privileges = current_user.privilage_list;
            ViewData["user_privileges"] = user_privileges;
            ViewBag.ename = ename;
            if (batch.CCE_Enabled())
            {
                ViewData["cce_exam_categories"] = db.CCE_EXAM_CATEGORY.ToList();
            }
            if (current_user.User.ADMIN_IND == true && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl") && !user_privileges.Select(x => x.NAME).Contains("EnterResults"))
            {
                ViewBag.ErrorMessage = "Sorry, you are not allowed to access that page.";
                return RedirectToAction("Dashboard", "User", new { id = current_user.User.ID });
            }
            if(exam_group_id != null)
            {

                ViewData["exam_group"] = db.EXAM_GROUP.Find(exam_group_id);
            }
            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME");
            return View();
        }

        // POST: Exam_Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,BTCH_ID,EXAM_TYPE,IS_PUB,RSULT_PUB,EXAM_DATE,IS_FINAL_EXAM,CCE_EXAM_CAT_ID")] EXAM_GROUP eXAM_GROUP)
        {
            if (ModelState.IsValid)
            {
                db.EXAM_GROUP.Add(eXAM_GROUP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME", eXAM_GROUP.CCE_EXAM_CAT_ID);
            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME", eXAM_GROUP.BTCH_ID);
            return View(eXAM_GROUP);
        }

        public ActionResult Show(int? id)
        {
            EXAM_GROUP exam_group = db.EXAM_GROUP.Include(x=>x.EXAMs).Where(x=>x.ID == id).FirstOrDefault();
            ViewData["exam_group"] = exam_group;

            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            ViewData["batch"] = batch;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            if (current_user.User.EMP_IND == true)
            {
                var user_privileges = current_user.privilage_list;
                ViewData["user_privileges"] = user_privileges;
                var employee_subjects = db.EMPLOYEES_SUBJECT.Include(x => x.SUBJECT).Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).ToList();
                ViewData["employee_subjects"] = employee_subjects;
                if ((employee_subjects == null || employee_subjects.Count() == 0) && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl") && !user_privileges.Select(x => x.NAME).Contains("EnterResults"))
                {
                    ViewBag.ErrorMessage = "Sorry, you are not allowed to access that page.";
                    return RedirectToAction("Dashboard", "User", new { id = current_user.User.ID, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            return View();
        }

        // GET: Exam_Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(id);
            ViewData["exam_group"] = exam_group;
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            ViewData["batch"] = batch;
            COURSE course = db.COURSEs.Find(batch.CRS_ID);
            ViewData["course"] = course;
            if (exam_group == null)
            {
                return HttpNotFound();
            }
            if (batch.CCE_Enabled())
            {
                ViewData["cce_exam_categories"] = db.CCE_EXAM_CATEGORY.ToList();
            }

            return View(exam_group);
        }

        // POST: Exam_Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,BTCH_ID,EXAM_TYPE,IS_PUB,RSULT_PUB,EXAM_DATE,IS_FINAL_EXAM,CCE_EXAM_CAT_ID")] EXAM_GROUP exam_group)
        {
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            EXAM_GROUP exam_group_upd = db.EXAM_GROUP.Find(exam_group.ID);
            if (ModelState.IsValid)
            {               
                exam_group_upd.NAME = exam_group.NAME;
                db.Entry(exam_group_upd).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Notice = "Updated exam group successfully.";
                return RedirectToAction("Index", new { id = batch.ID, Notice = ViewBag.Notice });
            }
            if (batch.CCE_Enabled())
            {
                ViewData["cce_exam_categories"] = db.CCE_EXAM_CATEGORY.ToList();
            }
            return View(exam_group_upd);
        }

        // GET: Exam_Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXAM_GROUP eXAM_GROUP = db.EXAM_GROUP.Find(id);
            if (eXAM_GROUP == null)
            {
                return HttpNotFound();
            }
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            if(current_user.User.EMP_IND == true)
            {
                var user_privileges = current_user.privilage_list;
                var employee_subjects = db.EMPLOYEES_SUBJECT.Include(x => x.SUBJECT).Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).ToList();
                ViewData["employee_subjects"] = employee_subjects;
                if ((employee_subjects == null || employee_subjects.Count() == 0) && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl") && !user_privileges.Select(x => x.NAME).Contains("EnterResults"))
                {
                    ViewBag.ErrorMessage = "Sorry, you are not allowed to access that page.";
                    return RedirectToAction("Dashboard", "User", new { id = current_user.User.ID, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            ViewBag.Notice = "Exam group deleted successfully";
            db.EXAM_GROUP.Remove(eXAM_GROUP);
            db.SaveChanges();
            return RedirectToAction("Index", new { Notice = ViewBag.Notice});
        }

        [AllowAnonymous]
        public JsonResult UniqueCCECat([Bind(Prefix = "CCE_EXAM_CAT_ID")] int? CCE_EXAM_CAT_ID, [Bind(Prefix = "BTCH_ID")] int? BTCH_ID)
        {
            EXAM_GROUP CCECatExists = db.EXAM_GROUP.Where(x => x.BTCH_ID == BTCH_ID && x.CCE_EXAM_CAT_ID == CCE_EXAM_CAT_ID).FirstOrDefault();
            return Json(CCECatExists == null, JsonRequestBehavior.AllowGet);
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
