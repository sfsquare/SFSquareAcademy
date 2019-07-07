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
    public class Student_AttendanceController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Student_Attendance
        public ActionResult Index()
        {
            var Config_Val = new Configuration();
            ViewBag.config = Config_Val.find_by_config_key("StudentAttendanceType");
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewBag.privilege = userdetails.privilage_list.ToList();
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            ViewData["employee_subjects"] = employee_subjects;
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
