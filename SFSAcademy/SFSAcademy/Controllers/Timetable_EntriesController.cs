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
    public class Timetable_EntriesController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Timetable_Entries
        public ActionResult Index()
        {
            var tIMETABLE_ENTRY = db.TIMETABLE_ENTRY.Include(t => t.BATCH).Include(t => t.CLASS_TIMING).Include(t => t.EMPLOYEE).Include(t => t.SUBJECT).Include(t => t.TIMETABLE).Include(t => t.WEEKDAY);
            return View(tIMETABLE_ENTRY.ToList());
        }

        // GET: Timetable_Entries
        public ActionResult New(int? timetable_id, string Notice)
        {
            ViewBag.Notice = Notice;
            var timetable = db.TIMETABLE_ENTRY.Find(timetable_id);
            ViewData["timetable"] = timetable;
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false
                                    select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                         .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select a Batch" });
            ViewBag.BTCH_ID = options;
            ViewBag.Edit = "Insert";
            return View();
        }

        // GET: Timetable_Entries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIMETABLE_ENTRY tIMETABLE_ENTRY = db.TIMETABLE_ENTRY.Find(id);
            if (tIMETABLE_ENTRY == null)
            {
                return HttpNotFound();
            }
            return View(tIMETABLE_ENTRY);
        }

        // GET: Timetable_Entries/Create
        public ActionResult Create()
        {
            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME");
            ViewBag.CLS_TMNG_ID = new SelectList(db.CLASS_TIMING, "ID", "NAME");
            ViewBag.EMP_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM");
            ViewBag.SUBJ_ID = new SelectList(db.SUBJECTs, "ID", "NAME");
            ViewBag.TIMT_ID = new SelectList(db.TIMETABLEs, "ID", "IS_ACT");
            ViewBag.WK_DAY_ID = new SelectList(db.WEEKDAYs, "ID", "NAME");
            return View();
        }

        // POST: Timetable_Entries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,BTCH_ID,WK_DAY_ID,EMP_ID,SUBJ_ID,TIMT_ID,CLS_TMNG_ID")] TIMETABLE_ENTRY tIMETABLE_ENTRY)
        {
            if (ModelState.IsValid)
            {
                db.TIMETABLE_ENTRY.Add(tIMETABLE_ENTRY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME", tIMETABLE_ENTRY.BTCH_ID);
            ViewBag.CLS_TMNG_ID = new SelectList(db.CLASS_TIMING, "ID", "NAME", tIMETABLE_ENTRY.CLS_TMNG_ID);
            ViewBag.EMP_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", tIMETABLE_ENTRY.EMP_ID);
            ViewBag.SUBJ_ID = new SelectList(db.SUBJECTs, "ID", "NAME", tIMETABLE_ENTRY.SUBJ_ID);
            ViewBag.TIMT_ID = new SelectList(db.TIMETABLEs, "ID", "IS_ACT", tIMETABLE_ENTRY.TIMT_ID);
            ViewBag.WK_DAY_ID = new SelectList(db.WEEKDAYs, "ID", "NAME", tIMETABLE_ENTRY.WK_DAY_ID);
            return View(tIMETABLE_ENTRY);
        }

        // GET: Timetable_Entries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIMETABLE_ENTRY tIMETABLE_ENTRY = db.TIMETABLE_ENTRY.Find(id);
            if (tIMETABLE_ENTRY == null)
            {
                return HttpNotFound();
            }
            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME", tIMETABLE_ENTRY.BTCH_ID);
            ViewBag.CLS_TMNG_ID = new SelectList(db.CLASS_TIMING, "ID", "NAME", tIMETABLE_ENTRY.CLS_TMNG_ID);
            ViewBag.EMP_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", tIMETABLE_ENTRY.EMP_ID);
            ViewBag.SUBJ_ID = new SelectList(db.SUBJECTs, "ID", "NAME", tIMETABLE_ENTRY.SUBJ_ID);
            ViewBag.TIMT_ID = new SelectList(db.TIMETABLEs, "ID", "IS_ACT", tIMETABLE_ENTRY.TIMT_ID);
            ViewBag.WK_DAY_ID = new SelectList(db.WEEKDAYs, "ID", "NAME", tIMETABLE_ENTRY.WK_DAY_ID);
            return View(tIMETABLE_ENTRY);
        }

        // POST: Timetable_Entries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,BTCH_ID,WK_DAY_ID,EMP_ID,SUBJ_ID,TIMT_ID,CLS_TMNG_ID")] TIMETABLE_ENTRY tIMETABLE_ENTRY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tIMETABLE_ENTRY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME", tIMETABLE_ENTRY.BTCH_ID);
            ViewBag.CLS_TMNG_ID = new SelectList(db.CLASS_TIMING, "ID", "NAME", tIMETABLE_ENTRY.CLS_TMNG_ID);
            ViewBag.EMP_ID = new SelectList(db.EMPLOYEEs, "ID", "EMP_NUM", tIMETABLE_ENTRY.EMP_ID);
            ViewBag.SUBJ_ID = new SelectList(db.SUBJECTs, "ID", "NAME", tIMETABLE_ENTRY.SUBJ_ID);
            ViewBag.TIMT_ID = new SelectList(db.TIMETABLEs, "ID", "IS_ACT", tIMETABLE_ENTRY.TIMT_ID);
            ViewBag.WK_DAY_ID = new SelectList(db.WEEKDAYs, "ID", "NAME", tIMETABLE_ENTRY.WK_DAY_ID);
            return View(tIMETABLE_ENTRY);
        }

        // GET: Timetable_Entries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TIMETABLE_ENTRY tIMETABLE_ENTRY = db.TIMETABLE_ENTRY.Find(id);
            if (tIMETABLE_ENTRY == null)
            {
                return HttpNotFound();
            }
            return View(tIMETABLE_ENTRY);
        }

        // POST: Timetable_Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TIMETABLE_ENTRY tIMETABLE_ENTRY = db.TIMETABLE_ENTRY.Find(id);
            db.TIMETABLE_ENTRY.Remove(tIMETABLE_ENTRY);
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
