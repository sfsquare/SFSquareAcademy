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
    public class EmployeeController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Employee
        public ActionResult Index()
        {
            var eMPLOYEEs = db.EMPLOYEEs.Include(e => e.COUNTRY).Include(e => e.COUNTRY1).Include(e => e.COUNTRY2).Include(e => e.EMPLOYEE_CATEGORY).Include(e => e.EMPLOYEE_POSITION).Include(e => e.EMPLOYEE_DEPARTMENT).Include(e => e.EMPLOYEE2).Include(e => e.EMPLOYEE_GRADE).Include(e => e.USER);
            return View(eMPLOYEEs.ToList());
        }

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            return View(eMPLOYEE);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME");
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME");
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME");
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME");
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME");
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "CODE");
            ViewBag.RPTG_MGR_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM");
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_CODE");
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME");
            return View();
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RPTG_MGR_ID,EMP_CAT_ID,EMP_NUM,JOINING_DATE,FIRST_NAME,MID_NAME,LAST_NAME,GNDR,JOB_TIL,EMP_POS_ID,EMP_DEPT_ID,EMP_GRADE_ID,QUAL,EXPNC_DETL,EXPNC_YEAR,EXPNC_MONTH,STAT,STAT_DESCR,DOB,MARITAL_STAT,CHLD_CNT,FTHR_NAME,MTHR_NAME,HUSBND_NAME,BLOOD_GRP,NTLTY_ID,HOME_ADDR_LINE1,HOME_ADDR_LINE2,HOME_CITY,HOME_STATE,HOME_CTRY_ID,HOME_PIN_CODE,OFF_ADDR_LINE1,OFF_ADDR_LINE2,OFF_CITY,OFF_STATE,OFF_CTRY_ID,OFF_PIN_CODE,OFF_PH1,OFF_PH2,MOBL_PH,HOME_PH,EML,FAX,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,CREATED_AT,UPDATED_AT,PHTO_FILE_SIZE,USRID")] EMPLOYEE eMPLOYEE)
        {
            if (ModelState.IsValid)
            {
                db.EMPLOYEEs.Add(eMPLOYEE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.NTLTY_ID);
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.HOME_CTRY_ID);
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.OFF_CTRY_ID);
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME", eMPLOYEE.EMP_CAT_ID);
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME", eMPLOYEE.EMP_POS_ID);
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "CODE", eMPLOYEE.EMP_DEPT_ID);
            ViewBag.RPTG_MGR_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", eMPLOYEE.RPTG_MGR_ID);
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_CODE", eMPLOYEE.EMP_GRADE_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", eMPLOYEE.USRID);
            return View(eMPLOYEE);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.NTLTY_ID);
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.HOME_CTRY_ID);
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.OFF_CTRY_ID);
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME", eMPLOYEE.EMP_CAT_ID);
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME", eMPLOYEE.EMP_POS_ID);
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "CODE", eMPLOYEE.EMP_DEPT_ID);
            ViewBag.RPTG_MGR_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", eMPLOYEE.RPTG_MGR_ID);
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_CODE", eMPLOYEE.EMP_GRADE_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", eMPLOYEE.USRID);
            return View(eMPLOYEE);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RPTG_MGR_ID,EMP_CAT_ID,EMP_NUM,JOINING_DATE,FIRST_NAME,MID_NAME,LAST_NAME,GNDR,JOB_TIL,EMP_POS_ID,EMP_DEPT_ID,EMP_GRADE_ID,QUAL,EXPNC_DETL,EXPNC_YEAR,EXPNC_MONTH,STAT,STAT_DESCR,DOB,MARITAL_STAT,CHLD_CNT,FTHR_NAME,MTHR_NAME,HUSBND_NAME,BLOOD_GRP,NTLTY_ID,HOME_ADDR_LINE1,HOME_ADDR_LINE2,HOME_CITY,HOME_STATE,HOME_CTRY_ID,HOME_PIN_CODE,OFF_ADDR_LINE1,OFF_ADDR_LINE2,OFF_CITY,OFF_STATE,OFF_CTRY_ID,OFF_PIN_CODE,OFF_PH1,OFF_PH2,MOBL_PH,HOME_PH,EML,FAX,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,CREATED_AT,UPDATED_AT,PHTO_FILE_SIZE,USRID")] EMPLOYEE eMPLOYEE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eMPLOYEE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.NTLTY_ID);
            ViewBag.HOME_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.HOME_CTRY_ID);
            ViewBag.OFF_CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", eMPLOYEE.OFF_CTRY_ID);
            ViewBag.EMP_CAT_ID = new SelectList(db.EMPLOYEE_CATEGORY, "ID", "NAME", eMPLOYEE.EMP_CAT_ID);
            ViewBag.EMP_POS_ID = new SelectList(db.EMPLOYEE_POSITION, "ID", "POS_NAME", eMPLOYEE.EMP_POS_ID);
            ViewBag.EMP_DEPT_ID = new SelectList(db.EMPLOYEE_DEPARTMENT, "ID", "CODE", eMPLOYEE.EMP_DEPT_ID);
            ViewBag.RPTG_MGR_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", eMPLOYEE.RPTG_MGR_ID);
            ViewBag.EMP_GRADE_ID = new SelectList(db.EMPLOYEE_GRADE, "ID", "GRADE_CODE", eMPLOYEE.EMP_GRADE_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", eMPLOYEE.USRID);
            return View(eMPLOYEE);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            if (eMPLOYEE == null)
            {
                return HttpNotFound();
            }
            return View(eMPLOYEE);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EMPLOYEE eMPLOYEE = db.EMPLOYEEs.Find(id);
            db.EMPLOYEEs.Remove(eMPLOYEE);
            db.SaveChanges();
            return RedirectToAction("Index");
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
