using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.IO;
using System.Globalization;

namespace SFSAcademy.Controllers
{
    public class Attendance_ReportsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Attendance_Reports
        public ActionResult Index(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            ViewBag.subject_id = "all_sub";
            ViewBag.date_today = DateTime.Today.ToShortDateString();

            var batches = db.BATCHes.Where(x => x.ID == -1).DefaultIfEmpty().AsEnumerable();
            var current_user = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            if (current_user.User.ADMIN_IND == true)
            {
                batches = db.BATCHes.Include(x => x.COURSE).FirstOrDefault().ACTIVE();
            }
            else if (current_user.privilage_list.Select(x => x.NAME).Contains("StudentAttendanceView"))
            {
                batches = db.BATCHes.Include(x => x.COURSE).FirstOrDefault().ACTIVE();
            }
            else if (current_user.User.EMP_IND == true)
            {
                batches = db.BATCHes.Include(x => x.COURSE).Where(x => x.EMP_ID == Employee.ID).ToList();
                batches = batches.Union(employee_subjects.Select(x => x.SUBJECT.BATCH)).Distinct().ToList();
            }
            List<SelectListItem> options = new SelectList(batches.OrderBy(x => x.ID), "ID", "Course_full_name").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.BATCH_ID = options;
            ViewBag.subject_id = null;
            return View();
        }

        // GET: Employee_Attendances
        public ActionResult Subject(int? batch_id)
        {
            bool allow_access = true;
            var current_user = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewData["batch"] = batch;
            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch_id).ToList();
            if (current_user.User.EMP_IND == true && allow_access == true)
            {
                var role_symb = current_user.privilage_list.Select(x => x.NAME).ToList();
                if(role_symb.Contains("StudentAttendanceView") || role_symb.Contains("StudentAttendanceRegister"))
                {
                    subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch_id).ToList();
                }
                else
                {
                    if (batch.EMP_ID == Employee.ID)
                    {
                        subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).ToList(); ;
                    }
                    else
                    {
                        subjects = db.SUBJECTs.Include(x => x.EMPLOYEES_SUBJECT).Where(x => x.EMPLOYEES_SUBJECT.FirstOrDefault().EMP_ID == Employee.ID && x.BTCH_ID == batch_id).ToList();
                    }
                }
                List<SelectListItem> options = new SelectList(subjects.OrderBy(x => x.NAME), "ID", "NAME").ToList();
                options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Subject" });
                ViewBag.SUB_ID = options;
            }
            else
            {
                subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).ToList();
                List<SelectListItem> options = new SelectList(subjects.OrderBy(x => x.NAME), "ID", "NAME").ToList();
                //options.Insert(-1, new SelectListItem() { Value = "-1", Text = "Select Subject" });
                SelectListItem selListItem = new SelectListItem() { Value = "all_sub", Text = "All Subjects" };
                options.Add(selListItem);
                options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Subject" });
                ViewBag.SUB_ID = options;
            }

            return PartialView("_Subjects");
        }

        // GET: Employee_Attendances
        public JsonResult Mode(int? batch_id, string subject_id, string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            DateTime today = System.DateTime.Today;
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewData["batch"] = batch;
            SUBJECT sub = db.SUBJECTs.Where(x => x.ID == -1).FirstOrDefault();
            List<SelectListItem> month = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                var result = new SelectListItem();
                result.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                result.Value = i.ToString();
                result.Selected = DateTime.Now.Month == i ? true : false;
                month.Add(result);                
            }
            month.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Month" });
            ViewBag.MONTH = month;
            List<SelectListItem> mode = new List<SelectListItem>();
            mode.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Mode" });
            mode.Insert(1, new SelectListItem() { Value = "monthly", Text = "Monthly" });
            mode.Insert(2, new SelectListItem() { Value = "overall", Text = "Overall" });
            ViewBag.MODE = mode;
            string Show = "Show";
            string Hide = "";
            if (config == "Daily")
            {
                if(subject_id != "-1")
                {
                    sub = db.SUBJECTs.Find(Convert.ToInt32(subject_id));
                    ViewData["sub"] = sub;
                    ViewBag.subject_id = subject_id;
                }
                else
                {
                    ViewBag.subject_id = 0;
                }
                var ModePartialView = db.RenderRazorViewToString(this.ControllerContext, "_Mode", Show);               
                var MonthPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Month", Hide);
                var YearPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Year", Hide);
                return Json(new { ModePartialView, MonthPartialView, YearPartialView }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (subject_id == "-1")
                {
                    var ModePartialView = db.RenderRazorViewToString(this.ControllerContext, "_Mode", Hide);
                    var MonthPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Month", Hide);
                    var YearPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Year", Hide);
                    return Json(new { ModePartialView, MonthPartialView, YearPartialView }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if(subject_id != "all_sub")
                    {
                        sub = db.SUBJECTs.Find(Convert.ToInt32(subject_id));
                        ViewData["sub"] = sub;
                        ViewBag.subject_id = subject_id;
                    }
                    else
                    {
                        ViewBag.subject_id = 0;
                    }
                    var ModePartialView = db.RenderRazorViewToString(this.ControllerContext, "_Mode", Show);
                    var MonthPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Month", Hide);
                    var YearPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Year", Hide);
                    return Json(new { ModePartialView, MonthPartialView, YearPartialView }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JsonResult Show(int? batch_id, string subject_id, string mode)
        {
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            ViewBag.report_type = mode;
            int? sub_id = null;
            if (subject_id != "all_sub" && subject_id != "0" && subject_id != "")
            {
                sub_id = Convert.ToInt32(subject_id);
            }
            ViewBag.subject_id = subject_id;
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewData["batch"] = batch;
            DateTime? start_date = batch.START_DATE;
            ViewBag.start_date = start_date;
            DateTime end_date = DateTime.Today.Date;
            ViewBag.end_date = end_date;
            //List<SelectListItem> leaves = new List<SelectListItem>();
            DataTable leaves = new DataTable();
            leaves.Columns.Add("student_id", typeof(int));
            leaves.Columns.Add("leave_type", typeof(string));
            leaves.Columns.Add("leave_count", typeof(int));
            SUBJECT subject = db.SUBJECTs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            var students = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).ToList();
            ViewData["students"] = students;            
            List<SelectListItem> month = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                var result = new SelectListItem();
                result.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                result.Value = i.ToString();
                month.Add(result);
            }
            month.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Month" });
            ViewBag.MONTH = month;
            var year = System.DateTime.Now.Year;
            List<SelectListItem> YearList = new List<SelectListItem>();
            for (int i = 1; i <= 3; i++)
            {
                var result = new SelectListItem();
                result.Text = (year + i -2).ToString();
                result.Value = (year + i - 2).ToString();
                YearList.Add(result);
            }
            YearList.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Year" });
            ViewBag.YEAR = YearList;
            ViewBag.monthVal = DateTime.Now.Month;
            string Show = "Show";
            string Hide = "";
            if (config != "Daily")
            {
                if(mode == "Overall")
                {                    
                    if (subject_id != "all_sub" && subject_id != "0" && subject_id != "")
                    {
                        subject = db.SUBJECTs.Find(sub_id);
                        ViewData["subject"] = subject;
                        if (subject.ELECTIVE_GRP_ID != null)
                        {
                            students = db.STUDENTs.Include(x => x.STUDENT_SUBJECT).Where(x => x.STUDENT_SUBJECT.FirstOrDefault().SUBJ_ID == subject.ID).ToList();
                        }
                        var academic_days = batch.Subject_Hours(start_date, end_date, sub_id).Count();
                        ViewBag.academic_days = academic_days;
                        var report = db.SUBJECT_LEAVE.Where(x => x.SUBJECT_ID == subject.ID && x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList().Count();
                        var grouped = db.SUBJECT_LEAVE.Where(x => x.SUBJECT_ID == subject.ID && x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                        foreach(var s in students)
                        {
                            if(grouped.Where(x=>x.STDNT_ID == s.ID).Count() == 0)
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = 0;
                                leaves.Rows.Add(row);
                            }
                            else
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = grouped.Where(x => x.STDNT_ID == s.ID).Count();
                                leaves.Rows.Add(row);
                            }
                            var FinalLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "leave").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            var Trow = leaves.NewRow();
                            Trow["student_id"] = s.ID;
                            Trow["leave_type"] = "total";
                            Trow["leave_count"] = academic_days - FinalLeave;
                            int FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            leaves.Rows.Add(Trow);
                            var Prow = leaves.NewRow();
                            Prow["student_id"] = s.ID;
                            Prow["leave_type"] = "percent";
                            double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                            if (academic_days != 0)
                            {
                                Prow["leave_count"] = Convert.ToInt32(Count);
                            }
                            else
                            {
                                Prow["leave_count"] = 0;
                            }
                            leaves.Rows.Add(Prow);
                        }
                    }
                    else
                    {
                        var academic_days = batch.Subject_Hours(start_date, end_date, 0).Count();
                        ViewBag.academic_days = academic_days;
                        var report = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList().Count();
                        var grouped = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                        foreach (var s in students)
                        {
                            if (grouped.Where(x => x.STDNT_ID == s.ID).Count() == 0)
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = 0;
                                leaves.Rows.Add(row);
                            }
                            else
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = grouped.Where(x => x.STDNT_ID == s.ID).Count();
                                leaves.Rows.Add(row);
                            }
                            int FinalLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "leave").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            var Trow = leaves.NewRow();
                            Trow["student_id"] = s.ID;
                            Trow["leave_type"] = "total";
                            Trow["leave_count"] = academic_days - FinalLeave;
                            int FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            leaves.Rows.Add(Trow);
                            var Prow = leaves.NewRow();
                            Prow["student_id"] = s.ID;
                            Prow["leave_type"] = "percent";
                            double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                            if (academic_days != 0)
                            {
                                Prow["leave_count"] = Convert.ToInt32(Count);
                            }
                            else
                            {
                                Prow["leave_count"] = 0;
                            }
                            leaves.Rows.Add(Prow);
                        }
                    }
                    ViewData["leaves"] = leaves;
                    var ReportPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Report", Show);
                    var MonthPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Month", Hide);
                    var YearPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Year", Hide);
                    return Json(new { ReportPartialView, MonthPartialView, YearPartialView }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.yearVal = DateTime.Now.Year;
                    ViewBag.academic_days = batch.Working_Days(DateTime.Now).Count();
                    subject = db.SUBJECTs.Find(sub_id);
                    ViewData["subject"] = subject;
                    var ReportPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Report", Hide);
                    var MonthPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Month", Show);
                    var YearPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Year", Show);
                    return Json(new { ReportPartialView, MonthPartialView, YearPartialView }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                if (mode == "Overall")
                {
                    var academic_days = batch.Academic_Days().Count();
                    ViewBag.academic_days = academic_days;
                    var leaves_forenoon = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == true && x.PM == false && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                    var leaves_afternoon = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == false && x.PM == true && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                    var leaves_full = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == true && x.PM == true && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();                  
                    foreach (var student in students)
                    {                       
                        int FinalLeave = Convert.ToInt32( (double)leaves_full.Where(x => x.STDNT_ID == student.ID).Count() + ((double)(leaves_forenoon.Where(x => x.STDNT_ID == student.ID).Count() + leaves_afternoon.Where(x => x.STDNT_ID == student.ID).Count()) * .5));
                        var Trow = leaves.NewRow();
                        Trow["student_id"] = student.ID;
                        Trow["leave_type"] = "total";
                        Trow["leave_count"] = academic_days - FinalLeave;
                        var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == student.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                        leaves.Rows.Add(Trow);
                        var Prow = leaves.NewRow();
                        Prow["student_id"] = student.ID;
                        Prow["leave_type"] = "percent";
                        double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                        if (academic_days != 0)
                        {
                            Prow["leave_count"] = Convert.ToInt32(Count);
                        }
                        else
                        {
                            Prow["leave_count"] = 0;
                        }
                        leaves.Rows.Add(Prow);
                    }
                    ViewData["leaves"] = leaves;
                    var ReportPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Report", Show);
                    var MonthPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Month", Hide);
                    var YearPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Year", Hide);
                    return Json(new { ReportPartialView, MonthPartialView, YearPartialView }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.yearVal = DateTime.Now.Year;
                    subject = db.SUBJECTs.Find(sub_id);
                    ViewData["subject"] = subject;
                    var ReportPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Report", Hide);
                    var MonthPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Month", Show);
                    var YearPartialView = db.RenderRazorViewToString(this.ControllerContext, "_Year", Show);
                    return Json(new { ReportPartialView, MonthPartialView, YearPartialView }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult Year(int? batch_id, int? subject_id, string month, string report_type)
        {
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewData["batch"] = batch;
            SUBJECT subject = db.SUBJECTs.Find(subject_id);
            ViewData["subject"] = subject;
            ViewBag.report_type = report_type;
            var year = System.DateTime.Now.Year;
            List<SelectListItem> YearList = new List<SelectListItem>();
            for (int i = 1; i <= 3; i++)
            {
                var result = new SelectListItem();
                result.Text = (year + i - 2).ToString();
                result.Value = (year + i - 2).ToString();
                YearList.Add(result);
            }
            YearList.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Year" });
            ViewBag.YEAR = YearList;
            ViewBag.yearVal = DateTime.Now.Year;
            List<SelectListItem> monthVal = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                var result = new SelectListItem();
                result.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                result.Value = i.ToString();
                result.Selected = Convert.ToInt32(month) == i ? true : false;
                monthVal.Add(result);
            }
            monthVal.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Month" });
            ViewBag.MONTH = monthVal;
            ViewBag.monthVal = month;
            string Show = "Show";
            /*
            if (Request.IsAjaxRequest())
            {
                ViewBag.YEAR = YearList;
                ViewBag.month = Convert.ToInt32(month);
            }
            else
            {
                ViewBag.YEAR = null;
                ViewBag.month = null;
            }
            */
            return PartialView("_Year", Show);
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(int? batch_id, string subject_id, string start_date, string end_date, string report_type, string RANGE, int? value)
        {
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            ViewBag.start_date = start_date;
            ViewBag.end_date = end_date;
            ViewBag.subject_id = subject_id;
            DateTime? start_date_val = Convert.ToDateTime(start_date);
            DateTime? end_date_val = Convert.ToDateTime(end_date);
            ViewBag.RANGE = RANGE;
            ViewBag.value = value;
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewData["batch"] = batch;
            var students = db.STUDENTs.Where(x => x.BTCH_ID == batch_id).ToList();
            ViewData["students"] = students;
            DateTime today = DateTime.Now;
            DataTable leaves = new DataTable();
            leaves.Columns.Add("student_id", typeof(int));
            leaves.Columns.Add("leave_type", typeof(string));
            leaves.Columns.Add("leave_count", typeof(int));
            SUBJECT subject = db.SUBJECTs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            var mode = report_type;
            ViewBag.report_type = report_type;
            var working_days = batch.Working_Days((DateTime)start_date_val);
            int? sub_id = null;
            if (subject_id != "all_sub" && subject_id != "0" && subject_id != "")
            {
                sub_id = Convert.ToInt32(subject_id);
            }
            if (start_date_val <= today)
            {
                if(config != "Daily")
                {
                    if (subject_id != "all_sub" && subject_id != "0" && subject_id != "")
                    {
                        subject = db.SUBJECTs.Find(sub_id);
                        ViewData["subject"] = subject;
                        var academic_days = batch.Subject_Hours(start_date_val, end_date_val, sub_id).Count();
                        ViewBag.academic_days = academic_days;
                        var report = db.SUBJECT_LEAVE.Where(x => x.SUBJECT_ID == subject.ID && x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <= end_date_val).ToList().Count();
                        var grouped = db.SUBJECT_LEAVE.Where(x => x.SUBJECT_ID == subject.ID && x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <= end_date_val).ToList();
                        foreach (var s in students)
                        {
                            if (grouped.Where(x => x.STDNT_ID == s.ID).Count() == 0)
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = 0;
                                leaves.Rows.Add(row);
                            }
                            else
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = grouped.Where(x => x.STDNT_ID == s.ID).Count();
                                leaves.Rows.Add(row);
                            }
                            var FinalLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "leave").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            var Trow = leaves.NewRow();
                            Trow["student_id"] = s.ID;
                            Trow["leave_type"] = "total";
                            Trow["leave_count"] = academic_days - FinalLeave;
                            var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            leaves.Rows.Add(Trow);
                            var Prow = leaves.NewRow();
                            Prow["student_id"] = s.ID;
                            Prow["leave_type"] = "percent";
                            double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                            if (academic_days != 0)
                            {
                                Prow["leave_count"] = Convert.ToInt32(Count);
                            }
                            else
                            {
                                Prow["leave_count"] = 0;
                            }
                            leaves.Rows.Add(Prow);
                        }
                    }
                    else
                    {
                        var academic_days = batch.Subject_Hours(start_date_val, end_date_val, 0).Count();
                        ViewBag.academic_days = academic_days;
                        var report = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <= end_date_val).ToList().Count();
                        var grouped = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <= end_date_val).ToList();
                        foreach (var s in students)
                        {
                            if (grouped.Where(x => x.STDNT_ID == s.ID).Count() == 0)
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = 0;
                                leaves.Rows.Add(row);
                            }
                            else
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = grouped.Where(x => x.STDNT_ID == s.ID).Count();
                                leaves.Rows.Add(row);
                            }
                            var FinalLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "leave").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            var Trow = leaves.NewRow();
                            Trow["student_id"] = s.ID;
                            Trow["leave_type"] = "total";
                            Trow["leave_count"] = academic_days - FinalLeave;
                            var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            leaves.Rows.Add(Trow);
                            var Prow = leaves.NewRow();
                            Prow["student_id"] = s.ID;
                            Prow["leave_type"] = "percent";
                            double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                            if (academic_days != 0)
                            {
                                Prow["leave_count"] = Convert.ToInt32(Count);
                            }
                            else
                            {
                                Prow["leave_count"] = 0;
                            }
                            leaves.Rows.Add(Prow);
                        }
                    }
                }
                else
                {
                    int academic_days = 0;
                    if (mode == "Overall")
                    {
                        academic_days = batch.Academic_Days().Count();                        
                    }
                    else
                    {

                        academic_days = working_days.Count();
                    }
                    ViewBag.academic_days = academic_days;
                    students = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).ToList();
                    var leaves_forenoon = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == true && x.PM == false && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <= end_date_val).ToList();
                    var leaves_afternoon = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == false && x.PM == true && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList();
                    var leaves_full = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == true && x.PM == true && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList();
                    foreach (var student in students)
                    {
                        int FinalLeave = Convert.ToInt32((double)leaves_full.Where(x => x.STDNT_ID == student.ID).Count() + ((double)(leaves_forenoon.Where(x => x.STDNT_ID == student.ID).Count() + leaves_afternoon.Where(x => x.STDNT_ID == student.ID).Count()) * .5));
                        var Trow = leaves.NewRow();
                        Trow["student_id"] = student.ID;
                        Trow["leave_type"] = "total";
                        Trow["leave_count"] = academic_days - FinalLeave;
                        var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == student.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                        leaves.Rows.Add(Trow);
                        var Prow = leaves.NewRow();
                        Prow["student_id"] = student.ID;
                        Prow["leave_type"] = "percent";
                        double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                        if (academic_days != 0)
                        {
                            Prow["leave_count"] = Convert.ToInt32(Count);
                        }
                        else
                        {
                            Prow["leave_count"] = 0;
                        }
                        leaves.Rows.Add(Prow);
                    }                   
                }
            }
            switch (RANGE)
            {
                case "Below":
                    var FilterStudent1 = leaves.AsEnumerable().Where(x => x.Field<string>("leave_type") == "percent" && x.Field<int>("leave_count") >= value).Select(x => x.Field<int>("student_id")).ToList();
                    foreach(var std in FilterStudent1)
                    {
                        for (int i = leaves.Rows.Count - 1; i >= 0; i--)
                        {
                            if (Convert.ToInt32(leaves.Rows[i]["student_id"]) == std)
                            {
                                leaves.Rows[i].Delete();
                            }
                        }
                        leaves.AcceptChanges();
                    }
                    break;
                case "Above":
                    var FilterStudent2 = leaves.AsEnumerable().Where(x => x.Field<string>("leave_type") == "percent" && x.Field<int>("leave_count") <= value).Select(x => x.Field<int>("student_id")).ToList();
                    foreach (var std in FilterStudent2)
                    {
                        for (int i = leaves.Rows.Count - 1; i >= 0; i--)
                        {
                            if (Convert.ToInt32(leaves.Rows[i]["student_id"]) == std)
                            {
                                leaves.Rows[i].Delete();
                            }
                        }
                        leaves.AcceptChanges();
                    }
                    break;
                case "Equals":
                    var FilterStudent3 = leaves.AsEnumerable().Where(x => x.Field<string>("leave_type") == "percent" && x.Field<int>("leave_count") != value).Select(x => x.Field<int>("student_id")).ToList();
                    foreach (var std in FilterStudent3)
                    {
                        for (int i = leaves.Rows.Count - 1; i >= 0; i--)
                        {
                            if (Convert.ToInt32(leaves.Rows[i]["student_id"]) == std)
                            {
                                leaves.Rows[i].Delete();
                            }
                        }
                        leaves.AcceptChanges();
                    }
                    break;
                default:  // Name ascending 
                    break;
            }
            ViewData["leaves"] = leaves;
            string Show = "Show";
            return PartialView("_Report", Show);

        }


        public ActionResult Report(int? batch_id, string subject_id, string month, string year, string report_type)
        {
            BATCH batch = db.BATCHes.Find(batch_id);
            ViewData["batch"] = batch;
            ViewBag.report_type = report_type;
            ViewBag.subject_id = subject_id;
            if (month != null && month != "")
            {
                List<SelectListItem> monthVal = new List<SelectListItem>();
                for (int i = 1; i <= 12; i++)
                {
                    var result = new SelectListItem();
                    result.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                    result.Value = i.ToString();
                    result.Selected = Convert.ToInt32(month) == i ? true : false;
                    monthVal.Add(result);
                }
                monthVal.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Month" });
                ViewBag.MONTH = monthVal;
            }
            if (year != null && year != "")
            {
                var yearInner = System.DateTime.Now.Year;
                List<SelectListItem> YearList = new List<SelectListItem>();
                for (int i = 1; i <= 3; i++)
                {
                    var result = new SelectListItem();
                    result.Text = (yearInner + i - 2).ToString();
                    result.Value = (yearInner + i - 2).ToString();
                    result.Selected = Convert.ToInt32(year) == (yearInner + i - 2) ? true : false;
                    YearList.Add(result);
                }
                YearList.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Year" });
                ViewBag.YEAR = YearList;
            }
            ViewBag.monthVal = month;
            ViewBag.yearVal = year;
            var students = db.STUDENTs.Where(x=>x.BTCH_ID == batch.ID).ToList();
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            DateTime? date = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
            DateTime start_date = (DateTime)date;
            ViewBag.start_date = start_date;
            DateTime today = DateTime.Today;
            DateTime BtStartBeginDate = new DateTime(batch.START_DATE.Value.Year, batch.START_DATE.Value.Month, 1);
            DateTime TodayNextMnBeginDate = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1);
            int? sub_id = null;
            if (subject_id != "all_sub" && subject_id != "0" && subject_id != "")
            {
                sub_id = Convert.ToInt32(subject_id);
            }
            if (start_date < BtStartBeginDate || start_date > batch.END_DATE || start_date >= TodayNextMnBeginDate)
            {
                ViewBag.academic_days = 0;
            }
            else
            {
                DataTable leaves = new DataTable();
                leaves.Columns.Add("student_id", typeof(int));
                leaves.Columns.Add("leave_type", typeof(string));
                leaves.Columns.Add("leave_count", typeof(int));
                var working_days = batch.Working_Days((DateTime)date);
                DateTime end_date = DateTime.Today;
                ViewBag.end_date = end_date;
                if (start_date <= DateTime.Today)
                {
                    if(Convert.ToInt32(month) == DateTime.Today.Month)
                    {
                        end_date = DateTime.Today;
                    }
                    else
                    {
                        end_date = start_date.AddMonths(1).AddDays(-1);
                    }
                    ViewBag.end_date = end_date;
                    if (config == "Daily")
                    {
                        var academic_days = working_days.Count();
                        ViewBag.academic_days = academic_days;
                        students = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).ToList();
                        ViewData["students"] = students;
                        var leaves_forenoon = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == true && x.PM == false && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                        var leaves_afternoon = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == false && x.PM == true && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                        var leaves_full = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == true && x.PM == true && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                        foreach (var student in students)
                        {
                            int FinalLeave = Convert.ToInt32((double)leaves_full.Where(x => x.STDNT_ID == student.ID).Count() + ((double)(leaves_forenoon.Where(x => x.STDNT_ID == student.ID).Count() + leaves_afternoon.Where(x => x.STDNT_ID == student.ID).Count()) * .5));
                            var Trow = leaves.NewRow();
                            Trow["student_id"] = student.ID;
                            Trow["leave_type"] = "total";
                            Trow["leave_count"] = academic_days - FinalLeave;
                            var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == student.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            leaves.Rows.Add(Trow);
                            var Prow = leaves.NewRow();
                            Prow["student_id"] = student.ID;
                            Prow["leave_type"] = "percent";
                            double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                            if (academic_days != 0)
                            {
                                Prow["leave_count"] = Convert.ToInt32(Count);
                            }
                            else
                            {
                                Prow["leave_count"] = 0;
                            }
                            leaves.Rows.Add(Prow);
                        }
                    }
                    else
                    {
                        if (subject_id != "all_sub" && subject_id != "0" && subject_id != "")
                        {
                            var subject = db.SUBJECTs.Find(sub_id);
                            ViewData["subject"] = subject;
                            if(subject.ELECTIVE_GRP_ID != null)
                            {
                                students = db.STUDENTs.Include(x => x.STUDENT_SUBJECT).Where(x => x.STUDENT_SUBJECT.FirstOrDefault().SUBJ_ID == subject.ID).ToList();
                            }
                            ViewData["students"] = students;
                            var academic_days = batch.Subject_Hours(start_date, end_date, sub_id).Count();
                            ViewBag.academic_days = academic_days;
                            var report = db.SUBJECT_LEAVE.Where(x => x.SUBJECT_ID == subject.ID && x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList().Count();
                            var grouped = db.SUBJECT_LEAVE.Where(x => x.SUBJECT_ID == subject.ID && x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                            foreach (var s in students)
                            {
                                if (grouped.Where(x => x.STDNT_ID == s.ID).Count() == 0)
                                {
                                    var row = leaves.NewRow();
                                    row["student_id"] = s.ID;
                                    row["leave_type"] = "leave";
                                    row["leave_count"] = 0;
                                    leaves.Rows.Add(row);
                                }
                                else
                                {
                                    var row = leaves.NewRow();
                                    row["student_id"] = s.ID;
                                    row["leave_type"] = "leave";
                                    row["leave_count"] = grouped.Where(x => x.STDNT_ID == s.ID).Count();
                                    leaves.Rows.Add(row);
                                }
                                var FinalLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "leave").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                                var Trow = leaves.NewRow();
                                Trow["student_id"] = s.ID;
                                Trow["leave_type"] = "total";
                                Trow["leave_count"] = academic_days - FinalLeave;
                                var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                                leaves.Rows.Add(Trow);
                                var Prow = leaves.NewRow();
                                Prow["student_id"] = s.ID;
                                Prow["leave_type"] = "percent";
                                double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                                if (academic_days != 0)
                                {
                                    Prow["leave_count"] = Convert.ToInt32(Count);
                                }
                                else
                                {
                                    Prow["leave_count"] = 0;
                                }
                                leaves.Rows.Add(Prow);
                            }
                        }
                        else
                        {
                            ViewData["students"] = students;
                            var academic_days = batch.Subject_Hours(start_date, end_date, 0).Count();
                            ViewBag.academic_days = academic_days;
                            var report = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList().Count();
                            var grouped = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date && x.MONTH_DATE <= end_date).ToList();
                            foreach (var s in students)
                            {
                                if (grouped.Where(x => x.STDNT_ID == s.ID).Count() == 0)
                                {
                                    var row = leaves.NewRow();
                                    row["student_id"] = s.ID;
                                    row["leave_type"] = "leave";
                                    row["leave_count"] = 0;
                                    leaves.Rows.Add(row);
                                }
                                else
                                {
                                    var row = leaves.NewRow();
                                    row["student_id"] = s.ID;
                                    row["leave_type"] = "leave";
                                    row["leave_count"] = grouped.Where(x => x.STDNT_ID == s.ID).Count();
                                    leaves.Rows.Add(row);
                                }
                                var FinalLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "leave").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                                var Trow = leaves.NewRow();
                                Trow["student_id"] = s.ID;
                                Trow["leave_type"] = "total";
                                Trow["leave_count"] = academic_days - FinalLeave;
                                var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                                leaves.Rows.Add(Trow);
                                var Prow = leaves.NewRow();
                                Prow["student_id"] = s.ID;
                                Prow["leave_type"] = "percent";
                                double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                                if (academic_days != 0)
                                {
                                    Prow["leave_count"] = Convert.ToInt32(Count);
                                }
                                else
                                {
                                    Prow["leave_count"] = 0;
                                }
                                leaves.Rows.Add(Prow);
                            }
                        }
                    }
                    ViewData["leaves"] = leaves;
                }
                else
                {
                    ViewData["leaves"] = null;
                }
            }
            string Show = "Show";
            return PartialView("_Report", Show);
        }

        public ActionResult Student_Details(int? id)
        {
            STUDENT student = db.STUDENTs.Find(id);
            ViewData["student"] = student;
            BATCH batch = db.BATCHes.Find(student.BTCH_ID);
            ViewData["batch"] = batch;
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            if(config == "Daily")
            {
                var reportAt = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.STDNT_ID == student.ID).ToList();
                ViewData["reportAt"] = reportAt;
            }
            else
            {
                var reportSL = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.STDNT_ID == student.ID).ToList();
                ViewData["reportSL"] = reportSL;
            }
            return PartialView("_Student_Details");
        }
        /*

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Report_pdf(int? batch_id, string subject_id, string start_date, string end_date, string report_type, string RANGE, string value)
        {
            return RedirectToAction("ReportF_pdf", new { batch_id = batch_id, subject_id = subject_id, start_date = start_date, end_date = end_date, report_type = report_type, RANGE = RANGE, value = value });
        }
        */

        public ActionResult Report_pdf(int? batch_id, string subject_id, string start_date, string end_date, string report_type, string RANGE, string value)
        {
            var Config_Val = new Configuration();
            string config = Config_Val.find_by_config_key("StudentAttendanceType");
            ViewBag.config = config;
            ViewBag.subject_id = subject_id;
            BATCH batch = db.BATCHes.Include(x=>x.COURSE).Where(x=>x.ID == batch_id).FirstOrDefault();
            ViewData["batch"] = batch;
            var students = db.STUDENTs.Where(x => x.BTCH_ID == batch_id).ToList();
            ViewData["students"] = students;
            ViewBag.RANGE = RANGE;
            ViewBag.value = value;
            DateTime today = DateTime.Now;
            DataTable leaves = new DataTable();
            DateTime? start_date_val = Convert.ToDateTime(start_date);
            DateTime? end_date_val = Convert.ToDateTime(end_date);
            leaves.Columns.Add("student_id", typeof(int));
            leaves.Columns.Add("leave_type", typeof(string));
            leaves.Columns.Add("leave_count", typeof(int));
            SUBJECT subject = db.SUBJECTs.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            var mode = report_type;
            var working_days = batch.Working_Days((DateTime)start_date_val);
            int? sub_id = null;
            if (subject_id != "all_sub" && subject_id != "0" && subject_id != "")
            {
                sub_id = Convert.ToInt32(subject_id);
            }
            if (start_date_val <= today)
            {
                if (config != "Daily")
                {
                    if (subject_id != "all_sub" && subject_id != "0" && subject_id != "")
                    {
                        subject = db.SUBJECTs.Find(sub_id);
                        ViewData["subject"] = subject;
                        var academic_days = batch.Subject_Hours(start_date_val,end_date_val, sub_id).Count();
                        ViewBag.academic_days = academic_days;
                        var report = db.SUBJECT_LEAVE.Where(x => x.SUBJECT_ID == subject.ID && x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList().Count();
                        var grouped = db.SUBJECT_LEAVE.Where(x => x.SUBJECT_ID == subject.ID && x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList();
                        foreach (var s in students)
                        {
                            if (grouped.Where(x => x.STDNT_ID == s.ID).Count() == 0)
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = 0;
                                leaves.Rows.Add(row);
                            }
                            else
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = grouped.Where(x => x.STDNT_ID == s.ID).Count();
                                leaves.Rows.Add(row);
                            }
                            var FinalLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "leave").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            var Trow = leaves.NewRow();
                            Trow["student_id"] = s.ID;
                            Trow["leave_type"] = "total";
                            Trow["leave_count"] = academic_days - FinalLeave;
                            var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            leaves.Rows.Add(Trow);
                            var Prow = leaves.NewRow();
                            Prow["student_id"] = s.ID;
                            Prow["leave_type"] = "percent";
                            double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                            if (academic_days != 0)
                            {
                                Prow["leave_count"] = Convert.ToInt32(Count);
                            }
                            else
                            {
                                Prow["leave_count"] = 0;
                            }
                            leaves.Rows.Add(Prow);
                        }
                    }
                    else
                    {
                        var academic_days = batch.Subject_Hours(start_date_val,end_date_val, 0).Count();
                        ViewBag.academic_days = academic_days;
                        var report = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList().Count();
                        var grouped = db.SUBJECT_LEAVE.Where(x => x.BTCH_ID == batch.ID && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList();
                        foreach (var s in students)
                        {
                            if (grouped.Where(x => x.STDNT_ID == s.ID).Count() == 0)
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = 0;
                                leaves.Rows.Add(row);
                            }
                            else
                            {
                                var row = leaves.NewRow();
                                row["student_id"] = s.ID;
                                row["leave_type"] = "leave";
                                row["leave_count"] = grouped.Where(x => x.STDNT_ID == s.ID).Count();
                                leaves.Rows.Add(row);
                            }
                            var FinalLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "leave").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            var Trow = leaves.NewRow();
                            Trow["student_id"] = s.ID;
                            Trow["leave_type"] = "total";
                            Trow["leave_count"] = academic_days - FinalLeave;
                            var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == s.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                            leaves.Rows.Add(Trow);
                            var Prow = leaves.NewRow();
                            Prow["student_id"] = s.ID;
                            Prow["leave_type"] = "percent";
                            double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                            if (academic_days != 0)
                            {
                                Prow["leave_count"] = Convert.ToInt32(Count);
                            }
                            else
                            {
                                Prow["leave_count"] = 0;
                            }
                            leaves.Rows.Add(Prow);
                        }
                    }
                }
                else
                {
                    int academic_days = 0;
                    if (mode == "Overall")
                    {
                        academic_days = batch.Academic_Days().Count();
                    }
                    else
                    {

                        academic_days = working_days.Count();
                    }
                    ViewBag.academic_days = academic_days;
                    students = db.STUDENTs.Where(x => x.BTCH_ID == batch.ID).ToList();
                    var leaves_forenoon = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == true && x.PM == false && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList();
                    var leaves_afternoon = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == false && x.PM == true && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList();
                    var leaves_full = db.ATTENDENCEs.Where(x => x.BTCH_ID == batch.ID && x.FORENOON == true && x.PM == true && x.MONTH_DATE >= start_date_val && x.MONTH_DATE <=end_date_val).ToList();
                    foreach (var student in students)
                    {
                        int FinalLeave = Convert.ToInt32((double)leaves_full.Where(x => x.STDNT_ID == student.ID).Count() + ((double)(leaves_forenoon.Where(x => x.STDNT_ID == student.ID).Count() + leaves_afternoon.Where(x => x.STDNT_ID == student.ID).Count()) * .5));
                        var Trow = leaves.NewRow();
                        Trow["student_id"] = student.ID;
                        Trow["leave_type"] = "total";
                        Trow["leave_count"] = academic_days - FinalLeave;
                        var FinalTLeave = leaves.AsEnumerable().Where(x => x.Field<int>("student_id") == student.ID && x.Field<string>("leave_type") == "total").Select(x => x.Field<int>("leave_count")).FirstOrDefault();
                        leaves.Rows.Add(Trow);
                        var Prow = leaves.NewRow();
                        Prow["student_id"] = student.ID;
                        Prow["leave_type"] = "percent";
                        double Count = ((double)(academic_days - FinalLeave) / academic_days) * 100;
                        if (academic_days != 0)
                        {
                            Prow["leave_count"] = Convert.ToInt32(Count);
                        }
                        else
                        {
                            Prow["leave_count"] = 0;
                        }
                        leaves.Rows.Add(Prow);
                    }                 
                }
            }
            ViewData["leaves"] = leaves;
            return View();
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
