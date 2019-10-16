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
        public ActionResult Index()
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

        public ActionResult Settings()
        {
            ViewBag.config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "StudentAttendanceType").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            ViewBag.CCE_config = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CCE").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString() == "0" ? false : true;
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewBag.privilege = userdetails.privilage_list.ToList();
            var Employee = db.EMPLOYEEs.Where(x => x.USRID == UserId).FirstOrDefault();
            var employee_subjects = db.EMPLOYEES_SUBJECT.Where(x => x.EMP_ID == Employee.ID).ToList();
            ViewData["employee_subjects"] = employee_subjects;
            ViewBag.allow_for_exams = true;
            return View();
        }

        // GET: Exam/Create
        public ActionResult Create_Exam()
        {
            ViewBag.EV_ID = new SelectList(db.EVENTs, "ID", "TTIL");
            ViewBag.EXAM_GRP_ID = new SelectList(db.EXAM_GROUP, "ID", "NAME");
            ViewBag.GRADING_LVL_ID = new SelectList(db.GRADING_LEVEL, "ID", "NAME");
            ViewBag.SUBJ_ID = new SelectList(db.SUBJECTs, "ID", "NAME");
            return View();
        }

        // POST: Exam/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Exam([Bind(Include = "ID,EXAM_GRP_ID,START_TIME,END_TIME,MAX_MKS,MIN_MKS,GRADING_LVL_ID,WTAGE,EV_ID,CREATED_AT,UPDATED_AT,SUBJ_ID")] EXAM eXAM)
        {
            if (ModelState.IsValid)
            {
                db.EXAMs.Add(eXAM);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EV_ID = new SelectList(db.EVENTs, "ID", "TTIL", eXAM.EV_ID);
            ViewBag.EXAM_GRP_ID = new SelectList(db.EXAM_GROUP, "ID", "NAME", eXAM.EXAM_GRP_ID);
            ViewBag.GRADING_LVL_ID = new SelectList(db.GRADING_LEVEL, "ID", "NAME", eXAM.GRADING_LVL_ID);
            ViewBag.SUBJ_ID = new SelectList(db.SUBJECTs, "ID", "NAME", eXAM.SUBJ_ID);
            return View(eXAM);
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
