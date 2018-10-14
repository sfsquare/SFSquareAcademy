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
    public class Class_TimingsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Class_Timings
        public ActionResult Index(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
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

            var cLASS_TIMING = db.CLASS_TIMING.Where(x=>x.BTCH_ID== null && x.IS_DEL ==false).OrderBy(x=>x.START_TIME).Include(c => c.BATCH);
            return View(cLASS_TIMING.ToList());
        }

        // GET: Class_Timings/Create
        public ActionResult Show(int? id)
        {
            var class_timings = db.CLASS_TIMING.Where(x => x.BTCH_ID == id).ToList();
            ViewData["class_timings"] = class_timings;
            var PeriodEntry = db.PERIOD_ENTRIES.Where(x => x.BTCH_ID == id).ToList();
            ViewData["PeriodEntry"] = PeriodEntry;
            ViewBag.BTCH_ID = id;
            return PartialView("_show_batch_timing");
        }

        // GET: Class_Timings/Create
        public ActionResult New(int? id)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.ID == id
                                    select new Models.CoursesBatch { CourseData = cs, BatchData = bt })
            .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = queryCourceBatch;
            /*List<SelectListItem> StartTime = new List<SelectListItem>();
            StartTime.Add(new SelectListItem { Text = "00:00", Value = "00:00" });
            StartTime.Add(new SelectListItem { Text = "00:30", Value = "Option2" });
            StartTime.Add(new SelectListItem { Text = "Option3", Value = "Option3", Selected = true });
            ViewBag.START_TIME = StartTime;*/
            return PartialView("_New");
        }

        // POST: Class_Timings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,BTCH_ID,NAME,START_TIME,END_TIME,IS_BRK,IS_DEL")] CLASS_TIMING cLASS_TIMING, int? BatchId)
        {
            if (ModelState.IsValid)
            {
                cLASS_TIMING.BTCH_ID = BatchId;
                cLASS_TIMING.IS_DEL = false;
                db.CLASS_TIMING.Add(cLASS_TIMING);
                db.SaveChanges();
                ViewBag.Notice = "Class timing was successfully created.";
                return RedirectToAction("Index",new { Notice = ViewBag.Notice});
            }

            ViewBag.BTCH_ID = BatchId;
            ViewBag.Notice = "There seems to be some issue with Model State.";
            return PartialView("_Edit", cLASS_TIMING);
        }

        // GET: Class_Timings/Edit/5
        public ActionResult Edit(int? id, int? BatchId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLASS_TIMING cLASS_TIMING = db.CLASS_TIMING.Find(id);
            if (cLASS_TIMING == null)
            {
                return HttpNotFound();
            }
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.ID == BatchId
                                    select new Models.CoursesBatch { CourseData = cs, BatchData = bt})
             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = queryCourceBatch;
            return PartialView("_Edit",cLASS_TIMING);
        }

        // POST: Class_Timings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "ID,BTCH_ID,NAME,START_TIME,END_TIME,IS_BRK,IS_DEL")] CLASS_TIMING cLASS_TIMING, int? BatchId)
        {
            if (ModelState.IsValid)
            {
                CLASS_TIMING cLASS_TIMING_Update = db.CLASS_TIMING.Find(cLASS_TIMING.ID);
                cLASS_TIMING_Update.BTCH_ID = BatchId;
                cLASS_TIMING_Update.NAME = cLASS_TIMING.NAME;
                cLASS_TIMING_Update.START_TIME = cLASS_TIMING.START_TIME;
                cLASS_TIMING_Update.END_TIME = cLASS_TIMING.END_TIME;
                cLASS_TIMING_Update.IS_BRK = cLASS_TIMING.IS_BRK;
                cLASS_TIMING_Update.IS_DEL = false;
                db.Entry(cLASS_TIMING_Update).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Notice = "Class timing updated successfully.";
                return RedirectToAction("Index",new { Notice = ViewBag.Notice});
            }
            ViewData["batch"] = BatchId;
            return PartialView("_Edit", cLASS_TIMING);
        }

        // GET: Class_Timings/Delete/5
        public ActionResult Destroy(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLASS_TIMING cLASS_TIMING = db.CLASS_TIMING.Find(id);
            if (cLASS_TIMING == null)
            {
                return HttpNotFound();
            }
            db.CLASS_TIMING.Remove(cLASS_TIMING);
            db.SaveChanges();
            ViewBag.Notice = "Class timing deleted successfully.";
            return RedirectToAction("Index",new { Notice = ViewBag.Notice});
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
