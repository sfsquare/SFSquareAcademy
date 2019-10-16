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
    public class Class_TimingsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Class_Timings
        public ActionResult Index(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;

            var Class_Timings_Entry = db.CLASS_TIMING_SET.Where(x=>x.IS_DEL ==false);
            return View(Class_Timings_Entry.ToList());
        }

        // GET: Class_Timings/Create
        public ActionResult Class_TimingSet_View(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            CLASS_TIMING_SET Class_Timing_Set = db.CLASS_TIMING_SET.Find(id);
            ViewData["Class_Timing_Set"] = Class_Timing_Set;

            var PeriodEntry = db.PERIOD_ENTRIES.ToList();
            ViewData["PeriodEntry"] = PeriodEntry;
            var Class_Timing_Entries = db.CLASS_TIMING_ENTRY.Include(x=>x.CLASS_TIMING_SET).Where(x => x.CLASS_TIMING_SET_ID == id && x.IS_DEL == false).OrderBy(x => x.START_TIME);
            ViewBag.CLASS_TIMING_SET_ID = id;
            return View(Class_Timing_Entries.ToList());
        }


        // GET: Class_Timings/Create
        public ActionResult Class_TimingSet_New()
        {          
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Class_TimingSet_New([Bind(Include = "ID,NAME,CREATED_AT,UPDATED_AT,IS_DEL")] CLASS_TIMING_SET cLASS_TIMING_sET)
        {
            if (ModelState.IsValid)
            {
                cLASS_TIMING_sET.UPDATED_AT = DateTime.Now;
                cLASS_TIMING_sET.CREATED_AT = DateTime.Now;
                db.CLASS_TIMING_SET.Add(cLASS_TIMING_sET);
                db.SaveChanges();
                ViewBag.Notice = "Class timing set was successfully created.";
                return RedirectToAction("Index", new { Notice = ViewBag.Notice });
            }
            ViewBag.Notice = "There seems to be some issue with Model State.";
            return View();
        }

        // GET: Class_Timings/Create
        public ActionResult Class_TimingSet_Delete(int? id)
        {
            CLASS_TIMING_SET Class_Timing_Set = db.CLASS_TIMING_SET.Find(id);
            var Class_Timing_Entry = db.CLASS_TIMING_ENTRY.Where(x => x.CLASS_TIMING_SET_ID == Class_Timing_Set.ID).ToList();

            foreach(var item in Class_Timing_Entry)
            {
                db.CLASS_TIMING_ENTRY.Remove(item);
            }
            db.CLASS_TIMING_SET.Remove(Class_Timing_Set);
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return RedirectToAction("Index", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return RedirectToAction("Index", new {ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.Notice = string.Concat("Class Timing Set deleted successfully.");
            return RedirectToAction("Index", new {ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Class_Timings/Create
        public ActionResult New(int? id)
        {
            CLASS_TIMING_SET Class_Timing_Set = db.CLASS_TIMING_SET.Find(id);
            ViewData["Class_Timing_Set"] = Class_Timing_Set;
            return PartialView("_New");
        }

        // POST: Class_Timings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CLASS_TIMING_SET_ID,NAME,START_TIME,END_TIME,IS_BRK,IS_DEL")] CLASS_TIMING_ENTRY cLASS_TIMING_ENTRY, int? CLASS_TIMING_SET_ID)
        {
            if (ModelState.IsValid)
            {
                cLASS_TIMING_ENTRY.CLASS_TIMING_SET_ID = CLASS_TIMING_SET_ID;
                cLASS_TIMING_ENTRY.IS_DEL = false;
                db.CLASS_TIMING_ENTRY.Add(cLASS_TIMING_ENTRY);
                db.SaveChanges();
                ViewBag.Notice = "Class timing entry was successfully created.";
                return RedirectToAction("Class_TimingSet_View", new { id= CLASS_TIMING_SET_ID, Notice = ViewBag.Notice});
            }

            ViewBag.ErrorMessage = "There seems to be some issue with Model State.";
            return RedirectToAction("Class_TimingSet_View", new { id = CLASS_TIMING_SET_ID, ErrorMessage = ViewBag.ErrorMessage });
        }

        // GET: Class_Timings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLASS_TIMING_ENTRY cLASS_TIMING_ENTRY = db.CLASS_TIMING_ENTRY.Include(x=>x.CLASS_TIMING_SET).Where(x=>x.ID == id).FirstOrDefault();
            CLASS_TIMING_SET Class_Timing_Set = db.CLASS_TIMING_SET.Find(cLASS_TIMING_ENTRY.CLASS_TIMING_SET_ID);
            ViewData["Class_Timing_Set"] = Class_Timing_Set;
            if (cLASS_TIMING_ENTRY == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit", cLASS_TIMING_ENTRY);
        }

        // POST: Class_Timings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "ID,CLASS_TIMING_SET_ID,NAME,START_TIME,END_TIME,IS_BRK,IS_DEL")] CLASS_TIMING_ENTRY cLASS_TIMING_ENTRY, int? CLASS_TIMING_SET_ID)
        {
            if (ModelState.IsValid)
            {
                CLASS_TIMING_ENTRY cLASS_TIMING_ENTRY_Update = db.CLASS_TIMING_ENTRY.Find(cLASS_TIMING_ENTRY.ID);
                cLASS_TIMING_ENTRY_Update.CLASS_TIMING_SET_ID = CLASS_TIMING_SET_ID;
                cLASS_TIMING_ENTRY_Update.NAME = cLASS_TIMING_ENTRY.NAME;
                cLASS_TIMING_ENTRY_Update.START_TIME = cLASS_TIMING_ENTRY.START_TIME;
                cLASS_TIMING_ENTRY_Update.END_TIME = cLASS_TIMING_ENTRY.END_TIME;
                cLASS_TIMING_ENTRY_Update.IS_BRK = cLASS_TIMING_ENTRY.IS_BRK;
                cLASS_TIMING_ENTRY_Update.IS_DEL = false;
                db.Entry(cLASS_TIMING_ENTRY_Update).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Notice = "Class timing updated successfully.";
                return RedirectToAction("Class_TimingSet_View", new { id = CLASS_TIMING_SET_ID, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with the Model State.";
            return RedirectToAction("Class_TimingSet_View", new { id = CLASS_TIMING_SET_ID, ErrorMessage = ViewBag.ErrorMessage });
        }

        // GET: Class_Timings/Delete/5
        public ActionResult Destroy(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLASS_TIMING_ENTRY cLASS_TIMING_ENTRY = db.CLASS_TIMING_ENTRY.Find(id);
            int? CLASS_TIMING_SET_ID = cLASS_TIMING_ENTRY.CLASS_TIMING_SET_ID;
            if (cLASS_TIMING_ENTRY == null)
            {
                return HttpNotFound();
            }
            db.CLASS_TIMING_ENTRY.Remove(cLASS_TIMING_ENTRY);
            db.SaveChanges();
            ViewBag.Notice = "Class timing entry deleted successfully.";
            return RedirectToAction("Class_TimingSet_View", new { id = CLASS_TIMING_SET_ID, Notice = ViewBag.Notice });
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
