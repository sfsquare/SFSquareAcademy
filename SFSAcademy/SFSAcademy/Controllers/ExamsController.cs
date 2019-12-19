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
    public class ExamsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Exams/Details/5
        public ActionResult Show(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXAM exam = db.EXAMs.Include(x=>x.EXAM_GROUP).Include(x=>x.SUBJECT).Where(x=>x.ID == id).FirstOrDefault();
            ViewData["exam"] = exam;
            ViewData["exam_group"] = exam.EXAM_GROUP;
            if (exam == null)
            {
                return HttpNotFound();
            }
            BATCH batch = db.BATCHes.Find(exam.EXAM_GROUP.BTCH_ID);
            ViewData["batch"] = batch;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            ViewData["current_user"] = current_user;
            var user_privileges = current_user.privilage_list;
            ViewData["user_privileges"] = user_privileges;
            List<int?> employee_subjects = new List<int?>();
            if(current_user.User.EMP_IND == true)
            {
                var Subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).Select(k => k.SUBJECT).ToList();
                foreach(var item in Subjects)
                {
                    employee_subjects.Add(item.ID);
                }
            }
            ViewData["employee_subjects"] = employee_subjects;
            if (!(employee_subjects.Contains(exam.SUBJ_ID) || current_user.User.ADMIN_IND == true || user_privileges.Select(x => x.NAME).Contains("ExaminationControl") || user_privileges.Select(x => x.NAME).Contains("EnterResults")))
            {
                ViewBag.ErrorMessage = "Access denied!";
                return RedirectToAction("Dashboard", "User", new { id = current_user.User.ID, ErrorMessage = ViewBag.ErrorMessage });
            }
            var exam_subject = db.SUBJECTs.Find(exam.SUBJ_ID);
            var is_elective = exam_subject.ELECTIVE_GRP_ID;
            //var students = db.STUDENTs.Where(x => x.ID == -1).DefaultIfEmpty().AsEnumerable();
            List<int> students = new List<int>();
            if (is_elective == null)
            {
                students = batch.All_Students().Select(x=>x.ID).ToList();
            }
            else
            {
                var assigned_students = db.STUDENT_SUBJECT.Include(x=>x.STUDENT).Where(x => x.SUBJ_ID == exam_subject.ID).OrderBy(x=>x.STUDENT.FIRST_NAME).ToList();
                if(assigned_students != null)
                {
                    students = assigned_students.Select(x => x.STUDENT.ID).ToList();
                }
            }
            ViewData["students"] = students;
            var exam_score = (from ex in db.EXAMs.Include(x=>x.SUBJECT)
                              from std in db.STUDENTs.Where(x=> students.Contains(x.ID))
                              where ex.ID == id
                              select new SFSAcademy.ExamScoreDetails { SubjectData = ex.SUBJECT, ExamData = ex, StudentData = std, Name = string.Concat(std.FIRST_NAME, " ", std.MID_NAME, " ", std.LAST_NAME), Marks = null, Remark = "", Is_Fail = false })
                .OrderBy(x => x.SubjectData.NAME).ToList();
            ViewData["exam_score"] = exam_score;
            var config = db.CONFIGURATIONs.FirstOrDefault().Get_Config_Value("ExamResultType");
            if(config == null)
            {
                config = "Marks";
            }
            ViewBag.config = config;
            var grades = batch.Grading_Level_List();
            ViewData["grades"] = grades;
            List<SelectListItem> options = new SelectList(grades, "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Grade" });
            ViewBag.Grading_Level_Id = options;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_Scores(IEnumerable<SFSAcademy.ExamScoreDetails> exam_score)
        {
            EXAM exam = db.EXAMs.Find(exam_score.FirstOrDefault().Exam_Id);
            bool? error = false;
            foreach(var item in exam_score)
            {
                var examscore = db.EXAM_SCORE.Where(x => x.STDNT_ID == item.Student_Id && x.EXAM_ID == exam.ID).FirstOrDefault();
                if(examscore == null)
                {
                    if ((item.Marks == null ? 0 : item.Marks) <= (exam.MAX_MKS == null ? 0 : exam.MAX_MKS))
                    {
                        EXAM_SCORE new_exam_score = new EXAM_SCORE() { EXAM_ID = exam.ID, STDNT_ID = item.Student_Id, MKS = item.Marks, GRADING_LVL_ID = item.Grading_Level_Id, RMK = item.Remark };
                        db.EXAM_SCORE.Add(new_exam_score);
                        db.SaveChanges();
                    }
                    else
                    {
                        error = true;
                    }
                }
                else
                {
                    if ((item.Marks== null? 0: item.Marks) <= (exam.MAX_MKS == null? 0 : exam.MAX_MKS))
                    {
                        examscore.EXAM_ID = exam.ID; examscore.STDNT_ID = item.Student_Id;
                        examscore.MKS = item.Marks; examscore.GRADING_LVL_ID = item.Grading_Level_Id;
                        examscore.RMK = item.Remark ;
                        db.Entry(examscore).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ViewBag.ErrorMessage = "Garading Levels are not set. Please set Garding Levels.";
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                            error = null;
                            return RedirectToAction("Show", new { id = exam.ID, ErrorMessage = ViewBag.ErrorMessage });
                        }
                    }
                    else
                    {
                        error = true;
                    }
                }
            }
            if(error == true)
            {
                ViewBag.ErrorMessage = "Exam score exceeds Maximum Mark.";
            }
            if (error == false)
            {
                ViewBag.Notice = "Exam scores updated.";
            }
            return RedirectToAction("Show", "Exam_Groups", new { id = exam.EXAM_GRP_ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
        }

        // GET: Exams/Create
        public ActionResult New(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(id);
            ViewData["exam_group"] = exam_group;
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            ViewData["batch"] = batch;
            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID).ToList();
            var current_user = this.Session["CurrentUser"] as UserDetails;
            var user_privileges = current_user.privilage_list;
            if (current_user.User.EMP_IND == true && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl"))
            {
                var employee_sub = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).Select(x => x.SUBJ_ID).ToList();
                subjects = db.SUBJECTs.Where(x => employee_sub.Contains(x.ID) && x.BTCH_ID == batch.ID).ToList();
            }
            List<int?> existing_subs = exam_group.EXAMs.Select(x => x.SUBJ_ID).Distinct().ToList();
            subjects = subjects.Where(x => !existing_subs.Contains(x.ID)).ToList();
            if(subjects == null || subjects.Count() == 0)
            {
                ViewBag.Notice = "Sorry, you are not allowed to access that page.";
                return RedirectToAction("Index", "Exam_Groups", new { id = batch.ID, ErrorMessage = ViewBag.ErrorMessage });
            }

            List<SelectListItem> options = new SelectList(subjects, "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Subject" });
            ViewBag.Subject_Id = options;
            var exam_detail = (from sub in subjects
                               select new ExamDetails { SubjectData = sub, Exam_Group_Id = exam_group.ID, Deleted = false, End_Time = null, Start_Time = null, Maximum_Marks = null, Minimum_Marks = null })
            .FirstOrDefault();

            return View(exam_detail);
        }

        // POST: Exams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(ExamDetails exam)
        {
            EXAM new_exam = new EXAM() { EXAM_GRP_ID = exam.Exam_Group_Id, SUBJ_ID = exam.Subject_Id, START_TIME = exam.Start_Time, END_TIME = exam.End_Time, MAX_MKS = exam.Maximum_Marks, MIN_MKS = exam.Minimum_Marks };
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(exam.Exam_Group_Id);
            ViewData["exam_group"] = exam_group;
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            ViewData["batch"] = batch;
            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID).ToList();
            var current_user = this.Session["CurrentUser"] as UserDetails;
            var user_privileges = current_user.privilage_list;
            if (current_user.User.EMP_IND == true && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl"))
            {
                var employee_sub = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).Select(x => x.SUBJ_ID).ToList();
                subjects = db.SUBJECTs.Where(x => employee_sub.Contains(x.ID) && x.BTCH_ID == batch.ID).ToList();
            }
            List<SelectListItem> options = new SelectList(subjects, "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Subject" });
            ViewBag.Subject_Id = options;
            var exam_detail = (from sub in subjects
                               select new ExamDetails { SubjectData = sub, Exam_Group_Id = exam_group.ID, Deleted = false, End_Time = null, Start_Time = null, Maximum_Marks = null, Minimum_Marks = null })
                               .FirstOrDefault();
            bool error = false;
            if (exam_group.EXAM_TYPE != "Grades")
            {
                if (exam.Maximum_Marks == null)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "-Maximum Marks can't be blank.");
                    error = true;
                }
                if (exam.Minimum_Marks == null)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "-Minimum Marks can't be blank.");
                    error = true;
                }
            }
            if (error == false)
            {
                db.EXAMs.Add(new_exam);
                try
                {
                    db.SaveChanges();
                    new_exam.Create_Exam_Event();
                    db.SaveChanges();
                    ViewBag.Notice = "New exam created successfully.";
                    return RedirectToAction("Index", "Exam_Groups", new { id = batch.ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
                }
                catch (Exception e)
                {

                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(exam_detail);
                }

            }
            return View(exam_detail);
        }

        // GET: Exams/Edit/5
        public ActionResult Edit(int? id, int? MaxMarks, int? MinMarks)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXAM exam = db.EXAMs.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaxMarks = MaxMarks;
            ViewBag.MinMarks = MinMarks;
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(exam.EXAM_GRP_ID);
            ViewData["exam_group"] = exam_group;
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            ViewData["batch"] = batch;
            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == exam_group.BTCH_ID).ToList();
            var current_user = this.Session["CurrentUser"] as UserDetails;
            var user_privileges = current_user.privilage_list;
            if (current_user.User.EMP_IND == true && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl"))
            {
                var employee_sub = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).Select(x => x.SUBJ_ID).ToList();
                subjects = db.SUBJECTs.Where(x => employee_sub.Contains(x.ID) && x.BTCH_ID == batch.ID).ToList();
                if (subjects.Where(x => x.ID == exam.SUBJ_ID).ToList() == null)
                {
                    ViewBag.ErrorMessage = "Sorry, you are not allowed to access that page.";
                    return RedirectToAction("Index", "Exam_Groups", new { id = batch.ID, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            List<SelectListItem> options = new SelectList(subjects, "ID", "NAME", exam.SUBJ_ID).ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Subject" });
            ViewBag.Subject_Id = options;
            var exam_detail = (from ex in db.EXAMs
                             join sub in db.SUBJECTs on ex.SUBJ_ID equals sub.ID
                             where ex.ID == exam.ID
                             select new ExamDetails { SubjectData = sub, Exam_Id = ex.ID, Exam_Group_Id = ex.EXAM_GRP_ID, Subject_Id = sub.ID, Deleted = false, End_Time = ex.END_TIME, Start_Time = ex.START_TIME, Maximum_Marks = ex.MAX_MKS, Minimum_Marks = ex.MIN_MKS })
                 .FirstOrDefault();
            return View(exam_detail);
        }

        // POST: Exams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExamDetails curr_exam)
        {
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(curr_exam.Exam_Group_Id);
            ViewData["exam_group"] = exam_group;
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            ViewData["batch"] = batch;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            var user_privileges = current_user.privilage_list;
            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == exam_group.BTCH_ID).ToList();
            if (current_user.User.EMP_IND == true && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl"))
            {
                var employee_sub = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).Select(x => x.SUBJ_ID).ToList();
                subjects = db.SUBJECTs.Where(x => employee_sub.Contains(x.ID) && x.BTCH_ID == batch.ID).ToList();
            }
            List<SelectListItem> options = new SelectList(subjects, "ID", "NAME", curr_exam.Subject_Id).ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Subject" });
            ViewBag.SUBJ_ID = options;
            if (ModelState.IsValid)
            {
                EXAM exam_to_update = db.EXAMs.Find(curr_exam.Exam_Id);
                exam_to_update.SUBJ_ID = curr_exam.Subject_Id;
                exam_to_update.START_TIME = curr_exam.Start_Time;
                exam_to_update.END_TIME = curr_exam.End_Time;
                exam_to_update.MAX_MKS = curr_exam.Maximum_Marks;
                exam_to_update.MIN_MKS = curr_exam.Minimum_Marks;
                db.Entry(exam_to_update).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    exam_to_update.Update_Exam_Event();
                    ViewBag.Notice = "Updated exam details successfully.";
                    return RedirectToAction("Show", "Exam_Groups", new { id = exam_group.ID, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(curr_exam);
                }
            }
            return View(curr_exam);
        }

        // GET: Exams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXAM exam = db.EXAMs.Find(id);
            if (exam == null)
            {
                return HttpNotFound();
            }
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(exam.EXAM_GRP_ID);
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            var current_user = this.Session["CurrentUser"] as UserDetails;
            var user_privileges = current_user.privilage_list;
            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == exam_group.BTCH_ID).ToList();
            if (current_user.User.EMP_IND == true && !user_privileges.Select(x => x.NAME).Contains("ExaminationControl"))
            {
                var employee_sub = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == current_user.User.ID).FirstOrDefault().ID).Select(x => x.SUBJ_ID).ToList();
                subjects = db.SUBJECTs.Where(x => employee_sub.Contains(x.ID) && x.BTCH_ID == batch.ID).ToList();
                if (subjects.Where(x => x.ID == exam.SUBJ_ID).ToList() == null)
                {
                    ViewBag.ErrorMessage = "Sorry, you are not allowed to access that page.";
                    return RedirectToAction("Index", "Exam_Groups", new { id = batch.ID, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            var event_id = exam.EV_ID;
            var batch_id = exam_group.BTCH_ID;
            db.EXAMs.Remove(exam);
            try
            {
                db.SaveChanges();
                var batch_event = db.BATCH_EVENT.Where(x => x.BTCH_ID == batch_id && x.EV_ID == event_id).FirstOrDefault();
                var curr_event = db.EVENTs.Find(exam.EV_ID);
                if(curr_event != null)
                {
                    db.EVENTs.Remove(curr_event);
                }
                if(batch_event != null)
                {
                    db.BATCH_EVENT.Remove(batch_event);
                }
                db.SaveChanges();
                ViewBag.Notice = "Exam deleted successfully.";
                return RedirectToAction("Show", "Exam_Groups", new { id = exam_group.ID, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return RedirectToAction("Index", "Exam_Groups", new { id = batch.ID, ErrorMessage = ViewBag.ErrorMessage });
            }
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
