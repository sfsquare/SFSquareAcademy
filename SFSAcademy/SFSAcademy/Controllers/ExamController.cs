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
    public class ExamController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Exam
        public ActionResult Index(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "StudentAttendanceType").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewData["privilege"] = userdetails.privilage_list.ToList();
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            ViewData["employee_subjects"] = employee_subjects;
            ViewBag.allow_for_exams = true;
            var exam_list = (from ex in db.EXAMs
                             join sub in db.SUBJECTs on ex.SUBJ_ID equals sub.ID
                             where ex.EXAM_GRP_ID == id
                             select new ExamFormDetails { SubjectData = sub, Subject_Id = sub.ID, Deleted = false, End_Time = ex.END_TIME, Start_Time = ex.START_TIME, Maximum_Marks = ex.MAX_MKS, Minimum_Marks = ex.MIN_MKS })
                            .OrderBy(x => x.SubjectData.NAME).ToList();
            ViewData["exams"] = exam_list;
            return View();
        }

        public ActionResult Settings()
        {
            ViewBag.config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "StudentAttendanceType").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            ViewBag.CCE_config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CCE").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString() == "0" ? false : true;
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewData["privilege"] = userdetails.privilage_list.ToList();
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            ViewData["employee_subjects"] = employee_subjects;
            ViewBag.allow_for_exams = true;
            return View();
        }

        // GET: Exam/Create
        public ActionResult Create_Exam()
        {
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var privilege = userdetails.privilage_list.ToList();
            var course = db.COURSEs.Where(x => x.ID == -1).DefaultIfEmpty().AsEnumerable();
            if (userdetails.User.ADMIN_IND == true || privilege.Select(p => p.NAME).Contains("ExaminationControl") || privilege.Select(p => p.NAME).Contains("EnterResults"))
            {
                course = db.COURSEs.Where(x => x.IS_DEL == false).ToList();
            }
            else if (userdetails.User.EMP_IND == true)
            {
                var emp_sub = db.EMPLOYEES_SUBJECT.Include(x => x.SUBJECT).Where(x => x.EMP_ID == db.EMPLOYEEs.Where(p => p.USRID == UserId).FirstOrDefault().ID).Select(k => k.SUBJECT).ToList();
                var batches = db.BATCHes.Where(x => emp_sub.Select(p => p.BTCH_ID).Contains(x.ID)).Distinct().ToList();
                course = db.COURSEs.Where(x => batches.Select(p => p.CRS_ID).Contains(x.ID)).Distinct().ToList();
            }
            List<SelectListItem> options = new SelectList(course.OrderBy(x => x.ID), "ID", "CRS_NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Course" });
            ViewBag.CRS_ID = options;

            return View();
        }

        public ActionResult Update_Batch(int? course_id)
        {
            var batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.CRS_ID == course_id && x.IS_DEL == false && x.IS_ACT == true).ToList();
            ViewData["batch"] = batch;
            return PartialView("_Update_Batch");
        }

        public ActionResult Generate_Reports()
        {
            ViewBag.config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "StudentAttendanceType").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewBag.privilege = userdetails.privilage_list.ToList();
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            ViewData["employee_subjects"] = employee_subjects;
            ViewBag.allow_for_exams = true;
            return View();
        }

        public ActionResult Report_Center()
        {
            ViewBag.config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "StudentAttendanceType").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewBag.privilege = userdetails.privilage_list.ToList();
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            ViewData["employee_subjects"] = employee_subjects;
            ViewBag.allow_for_exams = true;
            return View();
        }
        // GET: Exam/Edit/5
        public ActionResult Edit_Previous_Marks(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXAM eXAM = db.EXAMs.Find(id);
            if (eXAM == null)
            {
                return HttpNotFound();
            }
            ViewBag.EV_ID = new SelectList(db.EVENTs, "ID", "TTIL", eXAM.EV_ID);
            ViewBag.EXAM_GRP_ID = new SelectList(db.EXAM_GROUP, "ID", "NAME", eXAM.EXAM_GRP_ID);
            ViewBag.GRADING_LVL_ID = new SelectList(db.GRADING_LEVEL, "ID", "NAME", eXAM.GRADING_LVL_ID);
            ViewBag.SUBJ_ID = new SelectList(db.SUBJECTs, "ID", "NAME", eXAM.SUBJ_ID);
            return View(eXAM);
        }

        // POST: Exam/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Previous_Marks([Bind(Include = "ID,EXAM_GRP_ID,START_TIME,END_TIME,MAX_MKS,MIN_MKS,GRADING_LVL_ID,WTAGE,EV_ID,CREATED_AT,UPDATED_AT,SUBJ_ID")] EXAM eXAM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eXAM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EV_ID = new SelectList(db.EVENTs, "ID", "TTIL", eXAM.EV_ID);
            ViewBag.EXAM_GRP_ID = new SelectList(db.EXAM_GROUP, "ID", "NAME", eXAM.EXAM_GRP_ID);
            ViewBag.GRADING_LVL_ID = new SelectList(db.GRADING_LEVEL, "ID", "NAME", eXAM.GRADING_LVL_ID);
            ViewBag.SUBJ_ID = new SelectList(db.SUBJECTs, "ID", "NAME", eXAM.SUBJ_ID);
            return View(eXAM);
        }

        public ActionResult Update_Exam_Form(string Name, string Type, int? CCE_Exam_Category_Id, int? Batch_id)
        {
            BATCH batch = db.BATCHes.Find(Batch_id);
            ViewData["batch"] = batch;
            string name = Name;
            ViewBag.name = name;
            var type = Type;
            ViewBag.type = type;
            var cce_exam_category_id = CCE_Exam_Category_Id;
            ViewBag.cce_exam_category_id = cce_exam_category_id;
            var cce_exam_categories = db.CCE_EXAM_CATEGORY.Where(x => x.ID == -1).DefaultIfEmpty().AsEnumerable();
            if (batch.CCE_Enabled())
            {
                cce_exam_categories = db.CCE_EXAM_CATEGORY.ToList();
            }
            ViewData["cce_exam_categories"] = cce_exam_categories;
            if(name != "")
            {
                EXAM_GROUP exam_group = new EXAM_GROUP() { NAME = name, BTCH_ID = batch.ID, EXAM_TYPE = type, CCE_EXAM_CAT_ID = cce_exam_category_id };
                ViewData["exam_group"] = exam_group;
                var normal_subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.NO_EXAMS == false && x.ELECTIVE_GRP_ID == null && x.IS_DEL == false).ToList();
                ViewData["normal_subjects"] = normal_subjects;
                List<SUBJECT> elective_subjects = new List<SUBJECT>();
                var elective_subjects_list = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.NO_EXAMS == false && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false).ToList();
                foreach(var e in elective_subjects_list)
                {
                    var is_assigned = db.STUDENT_SUBJECT.Where(x => x.SUBJ_ID == e.ID).ToList();
                    if(is_assigned != null && is_assigned.Count() != 0)
                    {
                        elective_subjects.Add(e);
                    }
                }
                var all_subjects = normal_subjects.Union(elective_subjects);

                var exam_list = (from sub in all_subjects
                                 select new ExamFormDetails { SubjectData = sub, Subject_Id = sub.ID, Deleted = false, End_Time = null, Start_Time = null, Maximum_Marks = null, Minimum_Marks = null })
                 .OrderBy(x => x.SubjectData.NAME).ToList();
                if (type == "Marks" || type == "MarksAndGrades")
                {
                    return PartialView("_Exam_Marks_Form", exam_list);

                }
                else
                {
                    return PartialView("_Exam_Grade_Form", exam_list);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Exam name can't be blank";
                return View();
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
