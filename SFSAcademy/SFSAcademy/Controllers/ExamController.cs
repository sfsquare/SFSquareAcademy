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
        public ActionResult Index(string Notice, string ErrorMessage)
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
            return View();
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
            if (name != "")
            {
                EXAM_GROUP exam_group = new EXAM_GROUP() { NAME = name, BTCH_ID = batch.ID, EXAM_TYPE = type, CCE_EXAM_CAT_ID = cce_exam_category_id };
                ViewData["exam_group"] = exam_group;
                var normal_subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.NO_EXAMS == false && x.ELECTIVE_GRP_ID == null && x.IS_DEL == false).ToList();
                ViewData["normal_subjects"] = normal_subjects;
                List<SUBJECT> elective_subjects = new List<SUBJECT>();
                var elective_subjects_list = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.NO_EXAMS == false && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false).ToList();
                foreach (var e in elective_subjects_list)
                {
                    var is_assigned = db.STUDENT_SUBJECT.Where(x => x.SUBJ_ID == e.ID).ToList();
                    if (is_assigned != null && is_assigned.Count() != 0)
                    {
                        elective_subjects.Add(e);
                    }
                }
                var all_subjects = normal_subjects.Union(elective_subjects);

                var exam_list = (from sub in all_subjects
                                 select new ExamDetails { SubjectData = sub, Subject_Id = sub.ID, Deleted = false, End_Time = null, Start_Time = null, Maximum_Marks = null, Minimum_Marks = null })
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

        public ActionResult Publish(int? id, string status)
        {
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(id);
            var exams = db.EXAMs.Where(x=>x.EXAM_GRP_ID == exam_group.ID).ToList();
            BATCH batch = exam_group.BATCH;
            //string sms_setting_notice = "";
            string no_exam_notice = "";
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewData["privilege"] = userdetails.privilage_list.ToList();
            if (status == "schedule")
            {
                var students = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).ToList();
                List<int> available_user_ids = new List<int>();
                available_user_ids = students.Select(x => x.USRID).ToList();
                EVENT New_event = new EVENT() { TTIL = "Exam Scheduled", DESCR = string.Concat(exam_group.NAME, " has been scheduled. View Calendar."), ORIGIN_ID = UserId, ORIGIN_TYPE = "Exam", START_DATE = exam_group.EXAM_DATE, END_DATE = (DateTime)exams.Select(x=>x.END_TIME).Max(), IS_EXAM = true };
                db.EVENTs.Add(New_event);
                db.SaveChanges();
                ViewBag.message = "Exam Scheduled";
            }
            if(exams != null)
            {
                if (status == "schedule")
                {
                    exam_group.IS_PUB = true;
                }
                if (status == "result")
                {
                    exam_group.RSULT_PUB = true;
                }
                db.Entry(exam_group).State = EntityState.Modified;
                if (status == "result")
                {
                    var students = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).ToList();
                    List<int> available_user_ids = new List<int>();
                    available_user_ids = students.Select(x => x.USRID).ToList();
                    EVENT New_event = new EVENT() { TTIL = "Result Published", DESCR = string.Concat(exam_group.NAME, " result has been published. View Reports."), ORIGIN_ID = UserId, ORIGIN_TYPE = "Exam", START_DATE = exam_group.EXAM_DATE, IS_EXAM = true };
                    db.EVENTs.Add(New_event);
                    ViewBag.message = "Result Published";
                }
                db.SaveChanges();
            }
            else
            {
                no_exam_notice = "Exam scheduling not done yet.";
                ViewBag.no_exam_notice = no_exam_notice;
            }
            var exam_groups = db.EXAM_GROUP.Where(x => x.BTCH_ID == batch.ID).ToList();
            ViewData["exam_groups"] = exam_groups;
            return PartialView("_Publish");
        }

        public ActionResult Grouping(int? id)
        {
            BATCH batch = db.BATCHes.Find(id);
            ViewData["batch"] = batch;
            var exam_groups = db.EXAM_GROUP.Where(x => x.BTCH_ID == batch.ID).ToList();
            exam_groups = exam_groups.Where(x => x.EXAM_TYPE != "Grades").ToList();
            var exam_grouping = (from ex_gp in exam_groups
                                 join bt in db.BATCHes.Include(x => x.COURSE) on ex_gp.BTCH_ID equals bt.ID
                                 select new GroupedExamSelect { ExamGroupData = ex_gp, BatchData = bt, Exam_Group_Id = ex_gp.ID, Batch_Id = bt.ID, Select = false }
                                 ).OrderBy(x => x.ExamGroupData.NAME).ToList();
            ViewData["exam_grouping"] = exam_grouping;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Grouping(IEnumerable<GroupedExamSelect> exam_grouping_list, int? batch_id)
        {
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewData["batch"] = batch;
            var exam_groups = db.EXAM_GROUP.Where(x => x.BTCH_ID == batch.ID).ToList();
            exam_groups = exam_groups.Where(x => x.EXAM_TYPE != "Grades").ToList();
            var exam_grouping_Inner = (from ex_gp in exam_groups
                                       join bt in db.BATCHes.Include(x => x.COURSE) on ex_gp.BTCH_ID equals bt.ID
                                       select new GroupedExamSelect { ExamGroupData = ex_gp, BatchData = bt, Exam_Group_Id = ex_gp.ID, Batch_Id = bt.ID, Select = false }
                                 ).OrderBy(x => x.ExamGroupData.NAME).ToList();
            ViewData["exam_grouping"] = exam_grouping_Inner;

            var exam_grouping = exam_grouping_list.Where(x => x.Select == true);
            if (exam_grouping != null && exam_grouping.Count() != 0)
            {
                var exam_group_ids = exam_grouping.Select(x=>x.Exam_Group_Id).ToList();
                if (exam_group_ids != null && exam_group_ids.Count() != 0)
                {
                    decimal? total = 0;
                    foreach(var w in exam_grouping)
                    {
                        total += w.Weightage;
                    }
                    if(total != 100)
                    {
                        ViewBag.Notice = "Sum of the weightages must be 100%";
                        return View();
                    }
                    else
                    {
                        foreach(var item in db.GROUPED_EXAM.Where(x=>x.BTCH_ID == batch.ID).ToList())
                        {
                            db.GROUPED_EXAM.Remove(item);
                        }
                        db.SaveChanges();
                        foreach(var e in exam_grouping)
                        {
                            GROUPED_EXAM New_Grouped_Exam = new GROUPED_EXAM() { EXAM_GROUP_ID = e.Exam_Group_Id, BTCH_ID = e.Batch_Id, WTAGE = e.Weightage };
                            db.GROUPED_EXAM.Add(New_Grouped_Exam);
                        }
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                foreach (var item in db.GROUPED_EXAM.Where(x => x.BTCH_ID == batch.ID).ToList())
                {
                    db.GROUPED_EXAM.Remove(item);
                }
                db.SaveChanges();
            }
            ViewBag.Notice = "Selected exams grouped successfully.";
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
            List<SelectListItem> options = new SelectList(course.OrderBy(x => x.ID), "ID", "Full_Name").ToList();
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

        //REPORTS
        public ActionResult Generate_Reports(string Notice)
        {
            ViewBag.Notice = Notice;
            List<SelectListItem> options = new SelectList(db.COURSEs.Where(x => x.IS_DEL == false), "ID", "CRS_NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Course" });
            ViewBag.CRS_ID = options;
            return View();
        }
        public ActionResult List_Batch_Groups(int? course_id)
        {
            var batch_groups = db.BATCH_GROUP.Where(x => x.CRS_ID == course_id).ToList();
            ViewData["batch_groups"] = batch_groups;
            List<SelectListItem> options = new SelectList(batch_groups, "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Batch Group" });
            ViewBag.BTCH_GROUP_ID = options;
            return PartialView("_Select_Batch_Group");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate_Reports(int? CRS_ID, int? BTCH_GROUP_ID)
        {
            var batches = db.BATCHes.Where(x => x.ID == -1).ToList();
            if (CRS_ID != null)
            {
                COURSE course = db.COURSEs.Find(CRS_ID);               
                if (course.Has_batch_groups_with_active_batches())
                {
                    if(BTCH_GROUP_ID != null)
                    {
                        BATCH_GROUP batch_group = db.BATCH_GROUP.Find(BTCH_GROUP_ID);
                        batches = db.GROUPED_BATCH.Include(x => x.BATCH).Where(x => x.BTCH_GROUP_ID == batch_group.ID).Select(p => p.BATCH).ToList();
                    }
                }
                else
                {
                    batches = course.Active_Batches().ToList();
                }
            }
            if (batches != null && batches.Count() != 0)
            {
                string BatchNames = "";
                foreach(var batch in batches)
                {
                    batch.Generate_Batch_Reports();
                    BatchNames = string.Concat(BatchNames, batch.Full_Name, ",");
                }
                ViewBag.Notice = "Report generation done for batches " + BatchNames;
            }
            else
            {
                ViewBag.Notice = "Select atleast one Batch to continue.";
            }
            List<SelectListItem> options = new SelectList(db.COURSEs.Where(x => x.IS_DEL == false), "ID", "CRS_NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Course" });
            ViewBag.CRS_ID = options;
            return View();
        }
        public ActionResult Generate_Previous_Reports(string Notice)
        {
            ViewBag.Notice = Notice;
            List<SelectListItem> options = new SelectList(db.COURSEs.Where(x => x.IS_DEL == false), "ID", "CRS_NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Course" });
            ViewBag.CRS_ID = options;
            return View();
        }
        public ActionResult Select_Inactive_Batches(int? course_id)
        {
            var batches = (from bt in db.BATCHes
                           where bt.CRS_ID == course_id && bt.IS_ACT == false && bt.IS_DEL == false
                           select new BatchSelect { BatchData = bt, Select = false }).ToList();
            ViewData["batches"] = batches;
            return PartialView("_Inactive_Batch_List");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate_Previous_Reports(int? CRS_ID, IList<BatchSelect> batch_list)
        {
            COURSE course = db.COURSEs.Find(CRS_ID);
            List<int> SelectedIds = batch_list.Where(p => p.Select == true).Select(p => p.BatchData.ID).ToList();
            if (SelectedIds != null && SelectedIds.Count() != 0)
            {              
                var batches = db.BATCHes.Where(x => SelectedIds.Contains(x.ID)).ToList();
                string BatchNames = "";
                foreach (var batch in batches)
                {
                    batch.Generate_Previous_Batch_Reports();
                    BatchNames = string.Concat(BatchNames, batch.Full_Name, ",");
                }
                ViewBag.Notice = "Report generation done for batches " + BatchNames;
            }
            else
            {
                ViewBag.Notice = "Select atleast one Batch to continue.";
            }
            List<SelectListItem> options = new SelectList(db.COURSEs.Where(x => x.IS_DEL == false), "ID", "CRS_NAME", course.ID).ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Course" });
            ViewBag.CRS_ID = options;
            return View();
        }
        public ActionResult Report_Center()
        {
            return View();
        }
        public ActionResult Exam_Wise_Report(string Notice)
        {
            ViewBag.Notice = Notice;
            var batches = db.BATCHes.FirstOrDefault().ACTIVE();
            List<SelectListItem> options = new SelectList(batches.ToList(), "ID", "Full_Name").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Batch" });
            ViewBag.BTCH_ID = options;
            List<SelectListItem> options2 = new SelectList(db.EXAM_GROUP.ToList(), "ID", "NAME").ToList();
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Exam Group" });
            ViewBag.EXAM_GROUP_ID = options2;
            return View();
        }
        public ActionResult List_Exam_Types(int? batch_id)
        {
            BATCH batch = db.BATCHes.Find(batch_id);
            var exam_groups = db.EXAM_GROUP.Where(x => x.BTCH_ID == batch_id).ToList();
            List<SelectListItem> options = new SelectList(exam_groups, "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Exam Group" });
            ViewBag.EXAM_GROUP_ID = options;
            return PartialView("_Exam_Group_Select");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generated_Report(int? STDNT_ID, int? BTCH_ID, int? EXAM_GROUP_ID)
        {
            if(STDNT_ID == null)
            {
                if(BTCH_ID == null || EXAM_GROUP_ID == null)
                {
                    ViewBag.Notice = "Select a batch and exam to continue.";
                    return RedirectToAction("Exam_Wise_Report", new { Notice = ViewBag.Notice });
                }
            }
            else if(BTCH_ID == null)
            {
                ViewBag.Notice = "Invalid parameters.";
                return RedirectToAction("Exam_Wise_Report", new { Notice = ViewBag.Notice });
            }
            if(STDNT_ID == null)
            {
                EXAM_GROUP exam_group = db.EXAM_GROUP.Find(EXAM_GROUP_ID);
                ViewData["exam_group"] = exam_group;
                BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
                ViewData["batch"] = batch;
                var students = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).OrderBy(x => x.FIRST_NAME).ToList();
                ViewData["students"] = students;
                STUDENT student = db.STUDENTs.Where(x => x.ID == -1).FirstOrDefault();
                STUDENT prev_student = db.STUDENTs.Where(x => x.ID == -1).FirstOrDefault();
                STUDENT next_student = db.STUDENTs.Where(x => x.ID == -1).FirstOrDefault();
                if (students != null && students.Count() != 0)
                {
                    student = students.FirstOrDefault();
                    prev_student = db.STUDENTs.Where(x => x.ID < student.ID && x.BTCH_ID == batch.ID).OrderByDescending(x => x.ID).FirstOrDefault();
                    next_student = db.STUDENTs.Where(x => x.ID > student.ID && x.BTCH_ID == batch.ID).OrderBy(x => x.ID).FirstOrDefault();
                }
                ViewData["student"] = student;
                if (prev_student != null)
                {
                    ViewData["prev_student"] = prev_student;
                }
                if (next_student != null)
                {
                    ViewData["next_student"] = next_student;
                }
                if (student == null)
                {
                    ViewBag.Notice = "No students found in the batch.";
                    return RedirectToAction("Exam_Wise_Report", new { Notice = ViewBag.Notice });
                }
                var general_subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.ELECTIVE_GRP_ID == null).ToList();
                var student_electives = db.STUDENT_SUBJECT.Where(x => x.STDNT_ID == student.ID && x.BTCH_ID == batch.ID).ToList();
                List<int?> elective_subjects = new List<int?>();
                foreach(var elect in student_electives)
                {
                    elective_subjects.Add(elect.SUBJ_ID);
                }
                var subjects = general_subjects.Union(db.SUBJECTs.Where(x => elective_subjects.Contains(x.ID))).ToList();
                ViewData["subjects"] = subjects;
                List<int?> Exam_Ids = new List<int?>();
                foreach(var sub in subjects)
                {
                    EXAM exam = db.EXAMs.Where(x => x.EXAM_GRP_ID == exam_group.ID && x.SUBJ_ID == sub.ID).FirstOrDefault();
                    if(exam != null)
                    {
                        Exam_Ids.Add(exam.ID);
                    }
                }
                var exams = db.EXAMs.Where(x => Exam_Ids.Contains(x.ID)).ToList();
                ViewData["exams"] = exams;
                //var graph = open_flash_chart_object(770, 350, "/exam/graph_for_generated_report?batch=@batch.ID&examgroup=@exam_group.ID&student=@student.ID");
                //ViewData["graph"] = graph;
            }
            else
            {
                EXAM_GROUP exam_group = db.EXAM_GROUP.Find(EXAM_GROUP_ID);
                ViewData["exam_group"] = exam_group;
                var students = db.STUDENTs.Where(x => x.ID == STDNT_ID).OrderBy(x => x.FIRST_NAME).ToList();
                ViewData["students"] = students;
                STUDENT student = db.STUDENTs.Find(STDNT_ID);
                ViewData["student"] = student;
                STUDENT prev_student = db.STUDENTs.Where(x => x.ID < student.ID && x.BTCH_ID == student.BTCH_ID).OrderByDescending(x => x.ID).FirstOrDefault();
                STUDENT next_student = db.STUDENTs.Where(x => x.ID > student.ID && x.BTCH_ID == student.BTCH_ID).OrderBy(x => x.ID).FirstOrDefault();
                if (prev_student != null)
                {
                    ViewData["prev_student"] = prev_student;
                }
                if (next_student != null)
                {
                    ViewData["next_student"] = next_student;
                }
                BATCH batch = db.BATCHes.Find(student.BTCH_ID);
                ViewData["batch"] = batch;
                var general_subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch.ID && x.ELECTIVE_GRP_ID == null).ToList();
                var student_electives = db.STUDENT_SUBJECT.Where(x => x.STDNT_ID == student.ID && x.BTCH_ID == batch.ID).ToList();
                List<int?> elective_subjects = new List<int?>();
                foreach (var elect in student_electives)
                {
                    elective_subjects.Add(elect.SUBJ_ID);
                }
                var subjects = general_subjects.Union(db.SUBJECTs.Where(x => elective_subjects.Contains(x.ID))).ToList();
                ViewData["subjects"] = subjects;
                List<int?> Exam_Ids = new List<int?>();
                foreach (var sub in subjects)
                {
                    EXAM exam = db.EXAMs.Where(x => x.EXAM_GRP_ID == exam_group.ID && x.SUBJ_ID == sub.ID).FirstOrDefault();
                    Exam_Ids.Add(exam.ID);
                }
                var exams = db.EXAMs.Where(x => Exam_Ids.Contains(x.ID)).ToList();
                ViewData["exams"] = exams;
                //var graph = open_flash_chart_object(770, 350, "/exam/graph_for_generated_report?batch=@batch.ID&examgroup=@exam_group.ID&student=@student.ID");
                //ViewData["graph"] = graph;
                return PartialView("_Exam_Wise_Report");
            }
            return View();
        }





        // Previous Exams
        public ActionResult Previous_Batch_Exams()
        {
            List<SelectListItem> options = new SelectList(db.COURSEs.Where(x => x.IS_DEL == false), "ID", "Full_Name").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Course" });
            ViewBag.CRS_ID = options;
            return View();
        }
        public ActionResult List_Inactive_Batches(int? course_id)
        {
            var batch = db.BATCHes.Where(x => x.CRS_ID == course_id && x.IS_DEL == false && x.IS_ACT == false).ToList();
            List<SelectListItem> options = new SelectList(batch, "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select a Batch" });
            ViewBag.BTCH_ID = options;
            return PartialView("_Inactive_Batches");
        }
        public ActionResult List_Inactive_Exam_Groups(int? batch_id)
        {
            var exam_groups = db.EXAM_GROUP.Where(x => x.BTCH_ID == batch_id).ToList();
            List<int?> Rejected_exam_Group = db.GROUPED_EXAM.Where(x=>x.BTCH_ID == batch_id).Select(x=>x.EXAM_GROUP_ID).ToList();
            exam_groups = exam_groups.Where(x => !Rejected_exam_Group.Contains(x.ID)).ToList();
            List<SelectListItem> options = new SelectList(exam_groups, "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Exam Group" });
            ViewBag.EXAM_GROUP_ID = options;
            return PartialView("_Inactive_Exam_Groups");
        }
        public ActionResult Previous_Exam_Marks(int? exam_group_id)
        {            
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            ViewData["privilege"] = userdetails.privilage_list.ToList();
            if(userdetails.User.EMP_IND == true)
            {
                var Employee = db.EMPLOYEEs.Where(x => x.USRID == userdetails.User.ID).FirstOrDefault();
                var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
                ViewData["employee_subjects"] = employee_subjects;
            }
            var exam_group = db.EXAM_GROUP.Find(exam_group_id);
            ViewData["exam_group"] = exam_group;
            var exams = db.EXAMs.Include(x=>x.SUBJECT).Where(x => x.EXAM_GRP_ID == exam_group_id).ToList();
            ViewData["exams"] = exams;
            return PartialView("_Previous_Exam_Marks");
        }
        public ActionResult Edit_Previous_Marks(int? exam_id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            if (exam_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EXAM exam = db.EXAMs.Find(exam_id);
            ViewData["exam"] = exam;
            EXAM_GROUP exam_group = db.EXAM_GROUP.Find(exam.EXAM_GRP_ID);
            ViewData["exam_group"] = exam_group;
            BATCH batch = db.BATCHes.Find(exam_group.BTCH_ID);
            ViewData["batch"] = batch;
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            var privilege = userdetails.privilage_list.ToList();
            ViewData["privilege"] = privilege;
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.ID == -1).DefaultIfEmpty().ToList();
            if (userdetails.User.EMP_IND == true)
            {
                var Employee = db.EMPLOYEEs.Where(x => x.USRID == userdetails.User.ID).FirstOrDefault();
                employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
                if (!((employee_subjects != null && employee_subjects.Count() != 0 && employee_subjects.Select(x => x.SUBJ_ID).Contains(exam.SUBJ_ID)) || userdetails.User.ADMIN_IND == true || privilege.Select(p => p.NAME).Contains("ExaminationControl") || privilege.Select(p => p.NAME).Contains("EnterResults")))
                {
                    ViewBag.ErrorMessage = "Access denied!";
                    return RedirectToAction("Dashboard", "User", new { id = userdetails.User.ID, ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            if (!(userdetails.User.ADMIN_IND == true || privilege.Select(p => p.NAME).Contains("ExaminationControl") || privilege.Select(p => p.NAME).Contains("EnterResults")))
            {
                ViewBag.ErrorMessage = "Access denied!";
                return RedirectToAction("Dashboard", "User", new { id = userdetails.User.ID, ErrorMessage = ViewBag.ErrorMessage });
            }
            var exam_subject = db.SUBJECTs.Find(exam.SUBJ_ID);
            var is_elective = exam_subject.ELECTIVE_GRP_ID;
            List<int?> students = new List<int?>();
            if (is_elective == null)
            {
                var batch_students = db.BATCH_STUDENT.Where(x => x.BTCH_ID == batch.ID).ToList();
                if(batch_students != null && batch_students.Count() != 0)
                {
                    foreach(var b in batch_students)
                    {
                        var std = db.STUDENTs.Find(b.STDNT_ID);
                        if(std != null)
                        {
                            students.Add(std.ID);
                        }
                    }
                }
            }
            else
            {
                var assigned_students = db.STUDENT_SUBJECT.Where(x => x.SUBJ_ID == exam_subject.ID).ToList();
                foreach(var s in assigned_students)
                {
                    var std = db.STUDENTs.Find(s.ID);
                    if (std != null)
                    {
                        students.Add(std.ID);
                    }
                }
            }
            students.Sort();
            var exam_score = (from ex in db.EXAMs.Include(x => x.SUBJECT)
                              from std in db.STUDENTs.Where(x => students.Contains(x.ID))
                              where ex.ID == exam_id
                              select new SFSAcademy.ExamScoreDetails { SubjectData = ex.SUBJECT, ExamData = ex, StudentData = std, Name = string.Concat(std.FIRST_NAME, " ", std.MID_NAME, " ", std.LAST_NAME), Marks = null, Remark = "", Is_Fail = false })
                              .OrderBy(x => x.SubjectData.NAME).ToList();
            ViewData["exam_score"] = exam_score;
            var config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "ExamResultType").Select(x => x.CONFIG_VAL).FirstOrDefault();
            if(config == null)
            {
                config = "Marks";
            }
            ViewBag.config = config;
            var grades = batch.Grading_Level_List();
            List<SelectListItem> options = new SelectList(grades, "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Grade" });
            ViewBag.Grading_Level_Id = options;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Previous_Marks(IEnumerable<SFSAcademy.ExamScoreDetails> exam_score)
        {
            EXAM exam = db.EXAMs.Find(exam_score.FirstOrDefault().Exam_Id);
            bool? error = false;
            foreach (var item in exam_score)
            {
                EXAM_SCORE examscore = db.EXAM_SCORE.Where(x => x.STDNT_ID == item.Student_Id && x.EXAM_ID == exam.ID).FirstOrDefault();
                EXAM_SCORE prev_score = db.EXAM_SCORE.Where(x => x.STDNT_ID == item.Student_Id && x.EXAM_ID == exam.ID).FirstOrDefault();
                if (examscore != null)
                {
                    if ((item.Marks == null ? 0 : item.Marks) <= (exam.MAX_MKS == null ? 0 : exam.MAX_MKS))
                    {
                        examscore.MKS = item.Marks; examscore.GRADING_LVL_ID = item.Grading_Level_Id;
                        examscore.RMK = item.Remark;
                        db.Entry(examscore).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                            if(item.Is_Fail == true)
                            {
                                PREVIOUS_EXAM_SCORE NewPrevScore = new PREVIOUS_EXAM_SCORE() { STDNT_ID = prev_score.STDNT_ID, EXAM_ID = prev_score.EXAM_ID, MKS = prev_score.MKS, RMK = prev_score.RMK, IS_FAIL = prev_score.IS_FAIL };
                                db.PREVIOUS_EXAM_SCORE.Add(NewPrevScore);
                                db.SaveChanges();
                            }
                        }
                        catch (Exception e)
                        {
                            ViewBag.ErrorMessage = "Garading Levels are not set. Please set Garding Levels.";
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                            error = null;
                            return RedirectToAction("Edit_Previous_Marks", new { exam_id = exam.ID, ErrorMessage = ViewBag.ErrorMessage });
                        }
                    }
                    else
                    {
                        error = true;
                    }
                }
                else
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
            }
            if (error == true)
            {
                ViewBag.ErrorMessage = "Exam score exceeds Maximum Mark.";
            }
            if (error == false)
            {
                ViewBag.Notice = "Exam scores updated.";
            }
            return RedirectToAction("Edit_Previous_Marks", new { exam_id = exam.ID, Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });

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
