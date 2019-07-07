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
    public class AttendancesController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Attendances
        public ActionResult Index(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            ViewBag.date_today = DateTime.Today.ToShortDateString();

            var batches = db.BATCHes.Where(x => x.ID == -1).DefaultIfEmpty().AsEnumerable();
            var current_user = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            if (current_user.User.ADMIN_IND == true)
            {
                batches = db.BATCHes.Include(x=>x.COURSE).FirstOrDefault().ACTIVE();
            }
            else if(current_user.privilage_list.Select(x=>x.NAME).Contains("StudentAttendanceRegister"))
            {
                batches = db.BATCHes.Include(x => x.COURSE).FirstOrDefault().ACTIVE();
            }
            else if(current_user.User.EMP_IND == true)
            {
                if(config == "Daily")
                {
                    foreach(var Sub in employee_subjects.Select(x=>x.SUBJECT))
                    {
                        batches = db.BATCHes.Include(x => x.COURSE).Where(x=>x.EMP_ID == Employee.ID).ToList();
                    }
                    
                }
                else
                {
                    batches = db.BATCHes.Include(x => x.COURSE).Where(x => x.EMP_ID == Employee.ID).ToList();
                    batches = batches.Union(employee_subjects.Select(x => x.SUBJECT.BATCH)).Distinct().ToList();
                }
            }
            List<SelectListItem> options = new SelectList(batches.OrderBy(x => x.ID), "ID", "Course_full_name").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.BATCH_ID = options;
            return View();
        }

        // GET: Employee_Attendances
        public ActionResult List_Subject(int? batch_id)
        {
            bool allow_access = true;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            if (batch_id != null)
            {
                BATCH batch = db.BATCHes.Find(batch_id);
                ViewData["batch"] = batch;
                //var subjects = db.BATCHes.Where(x => x.ID == batch_id).Select(x => x.SUBJECTs).ToList();
                var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch_id).ToList();
                if (current_user.User.EMP_IND == true && allow_access == true && current_user.privilage_list.Select(x => x.NAME).Contains("StudentAttendanceRegister"))
                {
                    if(batch.EMP_ID == Employee.ID)
                    {
                        subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch_id).ToList();
                    }
                    else
                    {
                        subjects = db.SUBJECTs.Include(x=>x.EMPLOYEES_SUBJECT).Where(x => x.EMPLOYEES_SUBJECT.FirstOrDefault().EMP_ID == Employee.ID && x.BTCH_ID == batch_id).ToList();
                    }
                }
                List<SelectListItem> options = new SelectList(subjects.OrderBy(x => x.NAME), "ID", "NAME").ToList();
                options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Subject" });
                ViewBag.SUB_ID = options;
            }
            return PartialView("_Subjects");
        }

        // GET: Employee_Attendances
        public ActionResult Show(int? batch_id, int? subject_id, string next, string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            DateTime today = System.DateTime.Today;
            if (next != "null")
            {
                today = Convert.ToDateTime(next);
            }
            ViewBag.today = today.ToShortDateString();
            ViewBag.today_Plus_1Month = today.AddMonths(1).ToShortDateString();
            ViewBag.today_Minus_1Month = today.AddMonths(-1).ToShortDateString();
            var start_date = new DateTime(today.Year, today.Month, 1);
            ViewBag.start_date = start_date;
            var end_date = start_date.AddMonths(1).AddDays(-1);
            ViewBag.end_date = end_date;
            List<DateTime> dates = new List<DateTime>();
            if (config == "Daily")
            {
                BATCH batch = db.BATCHes.Find(batch_id);
                ViewData["batch"] = batch;
                var Students = db.STUDENTs.Where(x => x.BTCH_ID == batch_id).ToList();
                ViewData["Students"] = Students;
                dates = batch.Working_Days(today);
                ViewData["dates"] = dates;
                ViewData["sub"] = db.SUBJECTs.Where(x=>x.ID == -1).FirstOrDefault();
            }
            else
            {
                SUBJECT sub = db.SUBJECTs.Find(subject_id);
                ViewData["sub"] = sub;
                BATCH batch = db.BATCHes.Find(batch_id);
                ViewData["batch"] = batch;
                if(sub.ELECTIVE_GRP_ID != null)
                {
                    var elective_student_ids = (from std in db.STUDENTs
                                            join stdsub in db.STUDENT_SUBJECT on std.ID equals stdsub.STDNT_ID
                                            where stdsub.ID == subject_id
                                            select std).ToList();
                    var students = (from std in db.STUDENTs
                                    join elstd in elective_student_ids on std.ID equals elstd.ID
                                    where std.BTCH_ID == batch_id
                                    select std).ToList();
                    ViewData["students"] = students;
                }
                else
                {
                    var students_nonele = db.STUDENTs.Where(x=>x.BTCH_ID == batch_id).ToList();
                    ViewData["students"] = students_nonele;
                }
                dates = db.TIMETABLEs.FirstOrDefault().tte_for_range(batch, today, sub);
                ViewData["dates"] = dates;
            }
            return PartialView("_Register");
        }

        // GET: Employee_Attendances
        public ActionResult Edit(int? Abs_Id, string next)
        {
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            //ATTENDENCE absenteeAt = db.ATTENDENCEs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            //SUBJECT_LEAVE absenteeSl = db.SUBJECT_LEAVE.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            STUDENT student = db.STUDENTs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            SFSAcademy.AttendanceSubLeave AttendanceSL = new SFSAcademy.AttendanceSubLeave();
            AttendanceSL.Next = next;
            if (config == "Daily")
            {
                var absentee = db.ATTENDENCEs.Find(Abs_Id);
                AttendanceSL.AttendanceDate = absentee.MONTH_DATE.Value.ToShortDateString();
                AttendanceSL.Batch_Id = absentee.BTCH_ID;
                AttendanceSL.Student_Id = absentee.STDNT_ID;
                AttendanceSL.Absentee_Id = absentee.ID;
                AttendanceSL.Reason = absentee.RSN;
                AttendanceSL.Forenoon = absentee.FORENOON;
                AttendanceSL.Afternoon = absentee.PM;
                student = db.STUDENTs.Find(absentee.STDNT_ID);
            }
            else
            {
                var absentee = db.SUBJECT_LEAVE.Find(Abs_Id);
                AttendanceSL.AttendanceDate = absentee.MONTH_DATE.Value.ToShortDateString();
                AttendanceSL.Batch_Id = absentee.BTCH_ID;
                AttendanceSL.Student_Id = absentee.STDNT_ID;
                AttendanceSL.Subject_Id = absentee.SUBJECT_ID;
                AttendanceSL.Absentee_Id = absentee.ID;
                AttendanceSL.Reason = absentee.RSN;
                AttendanceSL.Subject_Id = absentee.SUBJECT_ID;
                student = db.STUDENTs.Find(absentee.STDNT_ID);
            }
            ViewData["student"] = student;
            return PartialView("_Edit", AttendanceSL);
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(SFSAcademy.AttendanceSubLeave AttendanceSL)
        {
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            //ATTENDENCE absenteeAt = db.ATTENDENCEs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            //SUBJECT_LEAVE absenteeSl = db.SUBJECT_LEAVE.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            STUDENT student = db.STUDENTs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            if (config == "Daily")
            {
                var absentee = db.ATTENDENCEs.Find(AttendanceSL.Absentee_Id);
                absentee.RSN = AttendanceSL.Reason;
                absentee.FORENOON = AttendanceSL.Forenoon;
                absentee.PM = AttendanceSL.Afternoon;
                db.Entry(absentee).State = EntityState.Modified;
                student = db.STUDENTs.Find(absentee.STDNT_ID);
                ViewData["student"] = student;
            }
            else
            {
                var absentee = db.SUBJECT_LEAVE.Find(AttendanceSL.Absentee_Id);
                absentee.RSN = AttendanceSL.Reason;
                absentee.SUBJECT_ID = AttendanceSL.Subject_Id;
                db.Entry(absentee).State = EntityState.Modified;
                student = db.STUDENTs.Find(absentee.STDNT_ID);
                ViewData["student"] = student;
            }

            //Add Section Finished. REgister section begins.
            DateTime today = System.DateTime.Today;
            if (AttendanceSL.Next != "null")
            {
                today = Convert.ToDateTime(AttendanceSL.Next);
            }
            ViewBag.today = today.ToShortDateString();
            ViewBag.today_Plus_1Month = today.AddMonths(1).ToShortDateString();
            ViewBag.today_Minus_1Month = today.AddMonths(-1).ToShortDateString();
            var start_date = new DateTime(today.Year, today.Month, 1);
            ViewBag.start_date = start_date;
            var end_date = start_date.AddMonths(1).AddDays(-1);
            ViewBag.end_date = end_date;
            List<DateTime> dates = new List<DateTime>();
            if (config == "Daily")
            {
                BATCH batch = db.BATCHes.Find(AttendanceSL.Batch_Id);
                ViewData["batch"] = batch;
                var Students = db.STUDENTs.Where(x => x.BTCH_ID == AttendanceSL.Batch_Id).ToList();
                ViewData["Students"] = Students;
                dates = batch.Working_Days(today);
                ViewData["dates"] = dates;
                ViewData["sub"] = db.SUBJECTs.Where(x => x.ID == -1).FirstOrDefault();
            }
            else
            {
                SUBJECT sub = db.SUBJECTs.Find(AttendanceSL.Subject_Id);
                ViewData["sub"] = sub;
                BATCH batch = db.BATCHes.Find(AttendanceSL.Batch_Id);
                ViewData["batch"] = batch;
                if (sub.ELECTIVE_GRP_ID != null)
                {
                    var elective_student_ids = (from std in db.STUDENTs
                                                join stdsub in db.STUDENT_SUBJECT on std.ID equals stdsub.STDNT_ID
                                                where stdsub.ID == AttendanceSL.Subject_Id
                                                select std).ToList();
                    var students = (from std in db.STUDENTs
                                    join elstd in elective_student_ids on std.ID equals elstd.ID
                                    where std.BTCH_ID == AttendanceSL.Batch_Id
                                    select std).ToList();
                    ViewData["students"] = students;
                }
                else
                {
                    var students_nonele = db.STUDENTs.Where(x => x.BTCH_ID == AttendanceSL.Batch_Id).ToList();
                    ViewData["students"] = students_nonele;
                }
                dates = db.TIMETABLEs.FirstOrDefault().tte_for_range(batch, today, sub);
                ViewData["dates"] = dates;
            }
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return PartialView("_Register");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return PartialView("_Register");
            }
            return PartialView("_Register");

        }

        public ActionResult Destroy(int? Abs_Id, string next)
        {
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            //ATTENDENCE absenteeAt = db.ATTENDENCEs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            //SUBJECT_LEAVE absenteeSl = db.SUBJECT_LEAVE.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            int? Batch_Id = null;
            int? Subject_Id = null;
            STUDENT student = db.STUDENTs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            if (config == "Daily")
            {
                var absentee = db.ATTENDENCEs.Find(Abs_Id);
                Batch_Id = absentee.BTCH_ID;
                student = db.STUDENTs.Find(absentee.STDNT_ID);
                ViewData["student"] = student;
                db.ATTENDENCEs.Remove(absentee);                
            }
            else
            {
                var absentee = db.SUBJECT_LEAVE.Find(Abs_Id);
                Batch_Id = absentee.BTCH_ID;
                Subject_Id = absentee.SUBJECT_ID;
                student = db.STUDENTs.Find(absentee.STDNT_ID);
                ViewData["student"] = student;
                db.SUBJECT_LEAVE.Remove(absentee);               
            }

            //Add Section Finished. REgister section begins.
            DateTime today = System.DateTime.Today;
            if (next != "null")
            {
                today = Convert.ToDateTime(next);
            }
            ViewBag.today = today.ToShortDateString();
            ViewBag.today_Plus_1Month = today.AddMonths(1).ToShortDateString();
            ViewBag.today_Minus_1Month = today.AddMonths(-1).ToShortDateString();
            var start_date = new DateTime(today.Year, today.Month, 1);
            ViewBag.start_date = start_date;
            var end_date = start_date.AddMonths(1).AddDays(-1);
            ViewBag.end_date = end_date;
            List<DateTime> dates = new List<DateTime>();
            if (config == "Daily")
            {
                BATCH batch = db.BATCHes.Find(Batch_Id);
                ViewData["batch"] = batch;
                var Students = db.STUDENTs.Where(x => x.BTCH_ID == Batch_Id).ToList();
                ViewData["Students"] = Students;
                dates = batch.Working_Days(today);
                ViewData["dates"] = dates;
                ViewData["sub"] = db.SUBJECTs.Where(x => x.ID == -1).FirstOrDefault();
            }
            else
            {
                SUBJECT sub = db.SUBJECTs.Find(Subject_Id);
                ViewData["sub"] = sub;
                BATCH batch = db.BATCHes.Find(Batch_Id);
                ViewData["batch"] = batch;
                if (sub.ELECTIVE_GRP_ID != null)
                {
                    var elective_student_ids = (from std in db.STUDENTs
                                                join stdsub in db.STUDENT_SUBJECT on std.ID equals stdsub.STDNT_ID
                                                where stdsub.ID == Subject_Id
                                                select std).ToList();
                    var students = (from std in db.STUDENTs
                                    join elstd in elective_student_ids on std.ID equals elstd.ID
                                    where std.BTCH_ID == Batch_Id
                                    select std).ToList();
                    ViewData["students"] = students;
                }
                else
                {
                    var students_nonele = db.STUDENTs.Where(x => x.BTCH_ID == Batch_Id).ToList();
                    ViewData["students"] = students_nonele;
                }
                dates = db.TIMETABLEs.FirstOrDefault().tte_for_range(batch, today, sub);
                ViewData["dates"] = dates;
            }
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return PartialView("_Register");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return PartialView("_Register");
            }
            return PartialView("_Register");

        }

        // GET: Employee_Attendances
        public ActionResult New(DateTime Month_date, int? student_id, int? subject_id, string next)
        {
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            ViewBag.today = next;
            ViewBag.subject_id = subject_id;
            SFSAcademy.AttendanceSubLeave AttendanceSL = new SFSAcademy.AttendanceSubLeave();
            AttendanceSL.Next = next;
            AttendanceSL.AttendanceDate = Month_date.ToShortDateString();
            if (config == "Daily")
            {
                var student = db.STUDENTs.Find(student_id);
                ViewData["student"] = student;
                AttendanceSL.Student_Id = student_id;
                AttendanceSL.Batch_Id = student.BTCH_ID;
                AttendanceSL.Forenoon = true;
                AttendanceSL.Afternoon = true;
            }
            else
            {
                var student = db.STUDENTs.Find(student_id);
                ViewData["student"] = student;
                AttendanceSL.Student_Id = student_id;
                AttendanceSL.Batch_Id = student.BTCH_ID;
                AttendanceSL.Subject_Id = subject_id;
            }           
            return PartialView("_New", AttendanceSL);
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SFSAcademy.AttendanceSubLeave AttendanceSL)
        {
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            ATTENDENCE absenteeAt = db.ATTENDENCEs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            SUBJECT_LEAVE absenteeSl = db.SUBJECT_LEAVE.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            STUDENT student = db.STUDENTs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            if (config == "SubjectWise")
            {
                student = db.STUDENTs.Find(AttendanceSL.Student_Id);
                var absentee = new SUBJECT_LEAVE()
                {
                    STDNT_ID = AttendanceSL.Student_Id,
                    MONTH_DATE = Convert.ToDateTime(AttendanceSL.AttendanceDate),
                    RSN = AttendanceSL.Reason,
                    BTCH_ID = AttendanceSL.Batch_Id,
                    SUBJECT_ID = AttendanceSL.Subject_Id,
                };
                db.SUBJECT_LEAVE.Add(absentee);
                absenteeSl = absentee;
            }
            else
            {
                student = db.STUDENTs.Find(AttendanceSL.Student_Id);
                var absentee = new ATTENDENCE()
                {
                    STDNT_ID = AttendanceSL.Student_Id,
                    MONTH_DATE = Convert.ToDateTime(AttendanceSL.AttendanceDate),
                    RSN = AttendanceSL.Reason,
                    BTCH_ID = AttendanceSL.Batch_Id,
                    FORENOON = AttendanceSL.Forenoon,
                    PM = AttendanceSL.Afternoon
                };
                db.ATTENDENCEs.Add(absentee);
                absenteeAt = absentee;
            }

            //Add Section Finished. REgister section begins.
            DateTime today = System.DateTime.Today;
            if (AttendanceSL.Next != "null")
            {
                today = Convert.ToDateTime(AttendanceSL.Next);
            }
            ViewBag.today = today.ToShortDateString();
            ViewBag.today_Plus_1Month = today.AddMonths(1).ToShortDateString();
            ViewBag.today_Minus_1Month = today.AddMonths(-1).ToShortDateString();
            var start_date = new DateTime(today.Year, today.Month, 1);
            ViewBag.start_date = start_date;
            var end_date = start_date.AddMonths(1).AddDays(-1);
            ViewBag.end_date = end_date;
            List<DateTime> dates = new List<DateTime>();
            if (config == "Daily")
            {
                BATCH batch = db.BATCHes.Find(AttendanceSL.Batch_Id);
                ViewData["batch"] = batch;
                var Students = db.STUDENTs.Where(x => x.BTCH_ID == AttendanceSL.Batch_Id).ToList();
                ViewData["Students"] = Students;
                dates = batch.Working_Days(today);
                ViewData["dates"] = dates;
                ViewData["sub"] = db.SUBJECTs.Where(x => x.ID == -1).FirstOrDefault();
            }
            else
            {
                SUBJECT sub = db.SUBJECTs.Find(AttendanceSL.Subject_Id);
                ViewData["sub"] = sub;
                BATCH batch = db.BATCHes.Find(AttendanceSL.Batch_Id);
                ViewData["batch"] = batch;
                if (sub.ELECTIVE_GRP_ID != null)
                {
                    var elective_student_ids = (from std in db.STUDENTs
                                                join stdsub in db.STUDENT_SUBJECT on std.ID equals stdsub.STDNT_ID
                                                where stdsub.ID == AttendanceSL.Subject_Id
                                                select std).ToList();
                    var students = (from std in db.STUDENTs
                                    join elstd in elective_student_ids on std.ID equals elstd.ID
                                    where std.BTCH_ID == AttendanceSL.Batch_Id
                                    select std).ToList();
                    ViewData["students"] = students;
                }
                else
                {
                    var students_nonele = db.STUDENTs.Where(x => x.BTCH_ID == AttendanceSL.Batch_Id).ToList();
                    ViewData["students"] = students_nonele;
                }
                dates = db.TIMETABLEs.FirstOrDefault().tte_for_range(batch, today, sub);
                ViewData["dates"] = dates;
            }
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return PartialView("_Register");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return PartialView("_Register");
            }
            return PartialView("_Register");
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
