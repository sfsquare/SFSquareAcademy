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
    public class Class_DesignationsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Class_Designations
        public ActionResult Index()
        {
            var cLASS_DESIGNATION = db.CLASS_DESIGNATION.Include(c => c.COURSE);
            return View(cLASS_DESIGNATION.ToList());
        }

        // GET: Class_Designations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLASS_DESIGNATION cLASS_DESIGNATION = db.CLASS_DESIGNATION.Find(id);
            if (cLASS_DESIGNATION == null)
            {
                return HttpNotFound();
            }
            return View(cLASS_DESIGNATION);
        }

        // GET: Class_Designations/Create
        public ActionResult Create()
        {
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME");
            return View();
        }

        // POST: Class_Designations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,CGPA,CREATED_AT,UPDATED_AT,MKS,CRS_ID")] CLASS_DESIGNATION cLASS_DESIGNATION)
        {
            if (ModelState.IsValid)
            {
                db.CLASS_DESIGNATION.Add(cLASS_DESIGNATION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", cLASS_DESIGNATION.CRS_ID);
            return View(cLASS_DESIGNATION);
        }

        // GET: Class_Designations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLASS_DESIGNATION cLASS_DESIGNATION = db.CLASS_DESIGNATION.Find(id);
            if (cLASS_DESIGNATION == null)
            {
                return HttpNotFound();
            }
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", cLASS_DESIGNATION.CRS_ID);
            return View(cLASS_DESIGNATION);
        }

        // POST: Class_Designations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,CGPA,CREATED_AT,UPDATED_AT,MKS,CRS_ID")] CLASS_DESIGNATION cLASS_DESIGNATION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cLASS_DESIGNATION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", cLASS_DESIGNATION.CRS_ID);
            return View(cLASS_DESIGNATION);
        }

        // GET: Class_Designations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CLASS_DESIGNATION cLASS_DESIGNATION = db.CLASS_DESIGNATION.Find(id);
            if (cLASS_DESIGNATION == null)
            {
                return HttpNotFound();
            }
            return View(cLASS_DESIGNATION);
        }

        // POST: Class_Designations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CLASS_DESIGNATION cLASS_DESIGNATION = db.CLASS_DESIGNATION.Find(id);
            db.CLASS_DESIGNATION.Remove(cLASS_DESIGNATION);
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
