using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
        public ActionResult Index(int? id, string ename, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            BATCH batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            var exam_groups = db.EXAM_GROUP.Where(x => x.BTCH_ID == batch.ID).ToList();
            ViewData["exam_groups"] = exam_groups;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            ViewBag.ename = ename;
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

        // GET: Exam_Groups/Create
        public ActionResult New(int? id, int? exam_group_id, string ename, string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
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
                return RedirectToAction("Dashboard", "User", new { id = current_user.User.ID, ErrorMessage = ViewBag.ErrorMessage });
            }
            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME");
            if (exam_group_id != null)
            {
                var exam_group = db.EXAM_GROUP.Include(x => x.EXAMs).Where(x => x.ID == exam_group_id).FirstOrDefault();
                ViewData["exam_group"] = exam_group;
                var exams = db.EXAM_GROUP.Where(x => x.ID == exam_group_id).Select(x => x.EXAMs).ToList();
                ViewData["exams"] = exams;
                ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME", exam_group.CCE_EXAM_CAT_ID);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IEnumerable<ExamDetails> examDtList, int? BTCH_ID, string NAME, string EXAM_TYPE, int? CCE_EXAM_CAT_ID, int? maximum_marks, int? minimum_marks)
        {
            EXAM_GROUP exam_group = new EXAM_GROUP() { NAME = NAME, BTCH_ID = BTCH_ID, EXAM_TYPE = EXAM_TYPE, CCE_EXAM_CAT_ID = CCE_EXAM_CAT_ID };
            var type = EXAM_TYPE;
            ViewBag.type = type;
            var error = false;
            BATCH batch = db.BATCHes.Find(BTCH_ID);
            ViewBag.maximum_marks = maximum_marks;
            ViewBag.minimum_marks = minimum_marks;
            if (type != "Grades")
            {
                foreach (var exam in examDtList)
                {
                    SUBJECT sub = db.SUBJECTs.Find(exam.Subject_Id);
                    if (exam.Deleted == false && error == false)
                    {
                        if (exam.Maximum_Marks == null)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, " ", sub.NAME, "-Maximum Marks can't be blank.");
                            error = true;
                        }
                        if (exam.Minimum_Marks == null)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, " ", sub.NAME, "-Minimum Marks can't be blank.");
                            error = true;
                        }
                    }
                }
            }
            if (error == false)
            {
                db.EXAM_GROUP.Add(exam_group);
                db.SaveChanges();               
                foreach (var exam in examDtList)
                {
                    if (exam.Deleted == false)
                    {
                        EXAM NewExam = new EXAM() { EXAM_GRP_ID = exam_group.ID, MAX_MKS = exam.Maximum_Marks, MIN_MKS = exam.Minimum_Marks, START_TIME = exam.Start_Time, END_TIME = exam.End_Time, SUBJ_ID = exam.Subject_Id };
                        db.EXAMs.Add(NewExam);
                    }
                }
                try
                {
                    db.SaveChanges();
                    foreach(var exam in db.EXAMs.Where(x=>x.EXAM_GRP_ID == exam_group.ID).ToList())
                    {
                        exam.Create_Exam_Event();
                    }
                    //db.SaveChanges();
                    ViewBag.Notice = "Exam group created successfully.";
                    return RedirectToAction("Index", new { id = batch.ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return RedirectToAction("New", new { id = batch.ID, ename = exam_group.NAME, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            else
            {
                if (batch.CCE_Enabled())
                {
                    ViewData["cce_exam_categories"] = db.CCE_EXAM_CATEGORY.ToList();
                }
                return RedirectToAction("New", new { id = batch.ID, ename = exam_group.NAME, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
            }
        }

        public ActionResult Show(int? id, string ErrorMesasge, string Notice)
        {
            ViewBag.ErroMessage = ErrorMesasge;
            ViewBag.Notice = Notice;
            EXAM_GROUP exam_group = db.EXAM_GROUP.Include(x => x.EXAMs).Where(x => x.ID == id).FirstOrDefault();
            ViewData["exam_group"] = exam_group;
            var exam_list = (from ex in db.EXAMs
                             join sub in db.SUBJECTs on ex.SUBJ_ID equals sub.ID
                             where ex.EXAM_GRP_ID == id
                             select new ExamDetails { SubjectData = sub, ExamData = ex, Subject_Id = sub.ID, Deleted = false, End_Time = ex.END_TIME, Start_Time = ex.START_TIME, Maximum_Marks = ex.MAX_MKS, Minimum_Marks = ex.MIN_MKS })
                 .OrderBy(x => x.SubjectData.NAME).ToList();
            ViewData["exams"] = exam_list;
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            ViewData["batch"] = batch;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            var user_privileges = current_user.privilage_list;
            ViewData["user_privileges"] = user_privileges;
            if (current_user.User.EMP_IND == true)
            {
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
            EXAM_GROUP exam_group_upd = db.EXAM_GROUP.Find(exam_group.ID);
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            exam_group_upd.NAME = exam_group.NAME;
            db.Entry(exam_group_upd).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                ViewBag.Notice = "Updated exam group successfully.";
                return RedirectToAction("Index", new { id = batch.ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
            }
            catch (Exception e)
            {
                if (batch.CCE_Enabled())
                {
                    ViewData["cce_exam_categories"] = db.CCE_EXAM_CATEGORY.ToList();
                }
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return View();
            }
        }

        // GET: Exam_Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(id);
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            if (exam_group == null)
            {
                return HttpNotFound();
            }
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            if (current_user.User.EMP_IND == true)
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
            foreach (var exams in db.EXAMs.Where(x => x.EXAM_GRP_ID == exam_group.ID).ToList())
            {
                db.EXAMs.Remove(exams);
            }
            db.EXAM_GROUP.Remove(exam_group);
            try
            {
                db.SaveChanges();
                ViewBag.Notice = "Exam group deleted successfully";
                return RedirectToAction("Index", new { id = batch.ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return RedirectToAction("Index", new { id = batch.ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
            }
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
