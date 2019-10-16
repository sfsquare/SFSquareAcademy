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
    public class Batch_TransfersController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Batch_Transfers
        public ActionResult Index(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            List<SelectListItem> options = new SelectList(db.COURSEs.FirstOrDefault().ACTIVE().OrderBy(x => x.ID), "ID", "Full_Name").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Course" });
            ViewBag.CRS_ID = options;
            var batches = db.BATCHes.FirstOrDefault().ACTIVE();
            ViewData["batches"] = batches;
            return View();
        }
        public ActionResult Update_Batch(int? course_id)
        {
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            var batches = db.BATCHes.Where(x => x.CRS_ID == course_id && x.IS_ACT == true && x.IS_DEL == false).ToList();
            ViewData["batches"] = batches;

            return PartialView("_List_Courses");
        }
        public ActionResult Show(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            BATCH batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            var Students = db.BATCHes.Find(id).All_Students().OrderBy(x=>x.FIRST_NAME);
            ViewData["Students"] = Students.ToList();
            COURSE course = db.COURSEs.Find(batch.CRS_ID);
            ViewData["course"] = course;
            var batches = db.BATCHes.FirstOrDefault().ACTIVE().Where(x=>x.ID != batch.ID);
            List<SelectListItem> options = new SelectList(batches.OrderBy(x => x.ID), "ID", "Full_Name").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Batch" });
            ViewBag.Transfer_To = options;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(int? id, int? Transfer_To, IEnumerable<STUDENT> Students)
        {
            var batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            var Students_Inner = db.BATCHes.Find(id).All_Students().OrderBy(x => x.FIRST_NAME);
            ViewData["Students"] = Students_Inner.ToList();
            COURSE course = db.COURSEs.Find(batch.CRS_ID);
            ViewData["course"] = course;
            var batches = db.BATCHes.FirstOrDefault().ACTIVE().Where(x=>x.ID != batch.ID);
            List<SelectListItem> options = new SelectList(batches.OrderBy(x => x.ID), "ID", "Full_Name").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.Transfer_To = options;
            Students = Students.Where(x => x.Select == true);
            if (Transfer_To != null)
            {
                if(Students != null && Students.Count() != 0)
                {
                    foreach(var s in Students)
                    {
                        STUDENT student = db.STUDENTs.Find(s.ID);
                        BATCH_STUDENT NewBs = new BATCH_STUDENT() { STDNT_ID = student.ID, BTCH_ID = student.BTCH_ID };
                        db.BATCH_STUDENT.Add(NewBs);
                        student.BTCH_ID = Transfer_To;
                        student.HAS_PD_FE = false;
                        db.Entry(student).State = EntityState.Modified;
                    }
                    try { db.SaveChanges(); }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                        return RedirectToAction("Show", new { id = batch.ID, ErrorMessage = ViewBag.ErrorMessage });
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                        return RedirectToAction("Show", new { id = batch.ID, ErrorMessage = ViewBag.ErrorMessage });
                    }

                }
                BATCH CurrBatch = batch;
                var stu = db.STUDENTs.Where(x => x.BTCH_ID == CurrBatch.ID).ToList();
                if(stu == null && stu.Count() == 0)
                {
                    CurrBatch.IS_ACT = false;
                    foreach(var sub in db.SUBJECTs.Include(x=>x.EMPLOYEES_SUBJECT).Where(x=>x.BTCH_ID == CurrBatch.ID).ToList())
                    {
                        foreach(var emp_sub in sub.EMPLOYEES_SUBJECT)
                        {
                            db.EMPLOYEES_SUBJECT.Remove(emp_sub);
                        }
                    }
                }
                ViewBag.Notice = "Trasferred students successfully";
                return RedirectToAction("Index", new { Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.ErrorMessage = "Select a batch to continue";
                return RedirectToAction("Show", new {id=batch.ID, ErrorMessage = ViewBag.ErrorMessage });
            }
        }
        public ActionResult Graduation(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch; 
            var Students = db.BATCHes.Find(id).All_Students().OrderBy(x => x.FIRST_NAME);
            ViewData["Students"] = Students.ToList();

                            List<string> ids = (List<string>)TempData["admission_list"];
            if (ids != null)
            {
                var id_lists = db.ARCHIVED_STUDENT.Where(x => ids.Contains(x.ADMSN_NO)).ToList();
                ViewData["id_lists"] = id_lists;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Graduation(int? id, IEnumerable<STUDENT> students, string Status_Description)
        {
            var batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            var Students = db.BATCHes.Find(id).All_Students().OrderBy(x => x.FIRST_NAME);
            ViewData["Students"] = Students.ToList();

            List<string> admission_list = new List<string>();
            students = students.Where(x => x.Select == true);
            foreach (var s in students)
            {
                admission_list.Add(s.ADMSN_NO);
            }
            foreach (var s in students)
            {
                s.Archive_Student(Status_Description);
            }
            var stu = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).ToList();
            if(stu == null || stu.Count() == 0)
            {
                batch.IS_ACT = false;
                db.Entry(batch).State = EntityState.Modified;
                foreach(var es in db.EMPLOYEES_SUBJECT.Include(x=>x.SUBJECT).Where(x=>x.SUBJECT.BTCH_ID == batch.ID))
                {
                    db.EMPLOYEES_SUBJECT.Remove(es);
                }
                try { 
                    db.SaveChanges();
                }
                catch (Exception e) { 
                    ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message);
                    return View();
                }
            }
            TempData["admission_list"] = admission_list;
            ViewBag.Notice = "Graduated selected students successfully";
            return RedirectToAction("Graduation", new {id = id, Notice = ViewBag.Notice });
        }
        public ActionResult Subject_Transfer(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            var elective_groups = batch.ELECTIVE_GROUP.Where(x => x.IS_DELETED == false).ToList();
            ViewData["elective_groups"] = elective_groups;
            var normal_subjects = batch.Normal_Batch_Subject();
            ViewData["normal_subjects"] = normal_subjects.ToList();
            var elective_subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false).ToList();
            ViewData["elective_subjects"] = elective_subjects;

            return View();
        }
        public ActionResult New_Subject(int? id, int? id2)
        {
            var batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            ELECTIVE_GROUP elective_group = db.ELECTIVE_GROUP.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            if (id2 != null)
            {
                elective_group = batch.ELECTIVE_GROUP.Where(x=>x.ID == id2).FirstOrDefault();
            }
            ViewData["elective_group"] = elective_group;
            return PartialView("_New");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Subject([Bind(Include = "ID,NAME,CODE,BTCH_ID,NO_EXAMS,MAX_WKILY_CLSES,ELECTIVE_GRP_ID,IS_DEL,CREATED_AT,UPDATED_AT,CR_HRS,PREF_CNSC,AMT")] SUBJECT subject, int? batch_id, int? elective_group_id)
        {
            subject.ELECTIVE_GRP_ID = elective_group_id;
            subject.BTCH_ID = batch_id;
            subject.IS_DEL = false;
            db.SUBJECTs.Add(subject);
            BATCH batch = db.BATCHes.Find(subject.BTCH_ID);
            ViewData["batch"] = batch;
            try
            {
                db.SaveChanges();
                ViewBag.Notice = "Subject created successfully";
                var subjects = batch.Normal_Batch_Subject();
                ViewData["subjects"] = subjects;
                var normal_subjects = subjects;
                ViewData["normal_subjects"] = normal_subjects;
                var elective_groups = db.ELECTIVE_GROUP.Where(x => x.BTCH_ID == batch.ID).ToList();
                ViewData["elective_groups"] = elective_groups;
                var elective_subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false);
                ViewData["elective_subjects"] = elective_subjects;
                return RedirectToAction("Subject_Transfer", new {id= batch.ID, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message);
                return RedirectToAction("Subject_Transfer", new { id = batch.ID, ErrorMessage = ViewBag.ErrorMessage });
            }
        }
        public ActionResult Get_Previous_Batch_Subjects(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            int? course_id = batch.COURSE.ID;
            var previous_batch = (from bt in db.BATCHes
                                  join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                  join sub in db.SUBJECTs.Where(x => x.IS_DEL == false) on bt.ID equals sub.BTCH_ID
                                  where bt.ID < batch.ID && cs.ID == course_id && bt.IS_DEL == false
                                  select bt).OrderByDescending(x => x.ID).FirstOrDefault();
            ViewData["previous_batch"] = previous_batch;
            if (previous_batch != null)
            {
                var previous_batch_normal_subject = previous_batch.Normal_Batch_Subject().ToList();
                ViewData["previous_batch_normal_subject"] = previous_batch_normal_subject;
                var elective_groups = previous_batch.ELECTIVE_GROUP.Where(x => x.IS_DELETED == false).ToList();
                ViewData["elective_groups"] = elective_groups;
                var previous_batch_electives = db.SUBJECTs.Where(x => x.BTCH_ID == previous_batch.ID && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false).ToList();
                ViewData["previous_batch_electives"] = previous_batch_electives;
                return PartialView("_Previous_Batch_Subjects");
            }
            else
            {
                ViewBag.ErrorMessage = "No subjects found in previous batches.";
                return RedirectToAction("Subject_Transfer", new { id = batch.ID, ErrorMessage = ViewBag.ErrorMessage });
            }
        }
        public ActionResult Assign_All_Previous_Batch_Subjects(int? id)
        {
            string msg = "";
            string err = "";
            BATCH batch = db.BATCHes.Find(id);
            COURSE course = batch.COURSE;
            var all_batches = course.BATCHes.Where(x=>x.IS_DEL == false).OrderBy(x => x.ID).ToList();
            var BatchFromSubject = db.SUBJECTs.Where(x => x.IS_DEL == false).Select(x => x.BTCH_ID).Distinct().ToList();
            all_batches = all_batches.Where(x => BatchFromSubject.Contains(x.ID)).OrderBy(x => x.ID).ToList();
            var previous_batch = (from bt in all_batches
                                  join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                  join sub in db.SUBJECTs.Where(x => x.IS_DEL == false) on bt.ID equals sub.BTCH_ID
                                  where bt.ID < batch.ID && cs.ID == course.ID && bt.IS_DEL == false
                                  select bt).OrderByDescending(x => x.ID).FirstOrDefault();

            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == previous_batch.ID && x.IS_DEL == false);
            foreach(var subject in subjects)
            {
                SUBJECT sub_exists = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.NAME == subject.NAME && x.IS_DEL == false).FirstOrDefault();
                if(sub_exists == null)
                {
                    if(subject.ELECTIVE_GRP_ID == null)
                    {
                        SUBJECT NewSub = new SUBJECT() { NAME = subject.NAME, CODE = subject.CODE, BTCH_ID = batch.ID, NO_EXAMS = subject.NO_EXAMS, MAX_WKILY_CLSES = subject.MAX_WKILY_CLSES, ELECTIVE_GRP_ID = subject.ELECTIVE_GRP_ID, CR_HRS = subject.CR_HRS, IS_DEL = false };
                        db.SUBJECTs.Add(NewSub);
                    }
                    else
                    {
                        ELECTIVE_GROUP elect_group_exists = db.ELECTIVE_GROUP.Where(x => x.BTCH_ID == batch.ID && x.ELECTIVE_GRP_NAME == db.ELECTIVE_GROUP.Find(subject.ELECTIVE_GRP_ID).ELECTIVE_GRP_NAME).FirstOrDefault();
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
                    msg += string.Concat("The Subject ", subject.NAME,"  has been added to batch ", batch.NAME, ". ");
                }
                else
                {
                    err += string.Concat("Batch ",batch.NAME," already has subject ",subject.NAME,". ");
                }
            }
            db.SaveChanges();
            /*
            ViewData["batch"] = batch;
            ViewData["previous_batch"] = previous_batch;
            ViewData["previous_batch_normal_subject"] = previous_batch.Normal_Batch_Subject().ToList();
            ViewData["elective_groups"] = previous_batch.ELECTIVE_GROUP.ToList();
            ViewData["previous_batch_electives"] = db.SUBJECTs.Where(x => x.BTCH_ID == previous_batch.ID && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false).ToList();
            */
            if (!string.IsNullOrEmpty(msg))
            {
                ViewBag.Notice = string.Concat("subjects_assigned \n", msg);
            }
            if(!string.IsNullOrEmpty(err))
            {
                ViewBag.ErrorMessage = string.Concat("subjects_assigned \n", err);
            }
            return RedirectToAction("Subject_Transfer", new { id = batch.ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
        }
        public ActionResult Assign_Previous_Batch_Subject(int? id, int? id2)
        {
            SUBJECT subject = db.SUBJECTs.Find(id);
            BATCH batch = db.BATCHes.Find(id2);
            SUBJECT sub_exists = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.NAME == subject.NAME && x.IS_DEL == false).FirstOrDefault();
            if(sub_exists == null)
            {
                if (subject.ELECTIVE_GRP_ID == null)
                {
                    SUBJECT NewSub = new SUBJECT() { NAME = subject.NAME, CODE = subject.CODE, BTCH_ID = batch.ID, NO_EXAMS = subject.NO_EXAMS, MAX_WKILY_CLSES = subject.MAX_WKILY_CLSES, ELECTIVE_GRP_ID = subject.ELECTIVE_GRP_ID, CR_HRS = subject.CR_HRS, IS_DEL = false };
                    db.SUBJECTs.Add(NewSub);
                }
                else
                {
                    ELECTIVE_GROUP elect_group_exists = db.ELECTIVE_GROUP.Where(x => x.BTCH_ID == batch.ID && x.ELECTIVE_GRP_NAME == db.ELECTIVE_GROUP.Find(subject.ELECTIVE_GRP_ID).ELECTIVE_GRP_NAME).FirstOrDefault();
                    if (elect_group_exists == null)
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
                db.SaveChanges();
                ViewBag.Notice = string.Concat(subject.NAME, " has been added to batch ", batch.NAME);
            }
            else
            {
                ViewBag.ErrorMessage = string.Concat(batch.NAME, " already has a subject ", subject.NAME);
            }
            return RedirectToAction("Subject_Transfer", new { id = batch.ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
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
