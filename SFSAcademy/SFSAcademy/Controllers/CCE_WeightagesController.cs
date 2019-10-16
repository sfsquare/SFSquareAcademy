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
    public class CCE_WeightagesController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: CCE_Weightages
        public ActionResult Index()
        {
            var cCE_WEIGHTAGE = db.CCE_WEIGHTAGE.Include(c => c.CCE_EXAM_CATEGORY);
            return View(cCE_WEIGHTAGE.ToList());
        }

        // GET: CCE_Weightages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_WEIGHTAGE cCE_WEIGHTAGE = db.CCE_WEIGHTAGE.Find(id);
            if (cCE_WEIGHTAGE == null)
            {
                return HttpNotFound();
            }
            return View(cCE_WEIGHTAGE);
        }

        // GET: CCE_Weightages/Create
        public ActionResult Create()
        {
            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME");
            return View();
        }

        // POST: CCE_Weightages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,WTAGE,CRITA_TYPE,CCE_EXAM_CAT_ID,CREATED_AT,UPDATED_AT")] CCE_WEIGHTAGE cCE_WEIGHTAGE)
        {
            if (ModelState.IsValid)
            {
                db.CCE_WEIGHTAGE.Add(cCE_WEIGHTAGE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME", cCE_WEIGHTAGE.CCE_EXAM_CAT_ID);
            return View(cCE_WEIGHTAGE);
        }

        // GET: CCE_Weightages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_WEIGHTAGE cCE_WEIGHTAGE = db.CCE_WEIGHTAGE.Find(id);
            if (cCE_WEIGHTAGE == null)
            {
                return HttpNotFound();
            }
            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME", cCE_WEIGHTAGE.CCE_EXAM_CAT_ID);
            return View(cCE_WEIGHTAGE);
        }

        // POST: CCE_Weightages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,WTAGE,CRITA_TYPE,CCE_EXAM_CAT_ID,CREATED_AT,UPDATED_AT")] CCE_WEIGHTAGE cCE_WEIGHTAGE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cCE_WEIGHTAGE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME", cCE_WEIGHTAGE.CCE_EXAM_CAT_ID);
            return View(cCE_WEIGHTAGE);
        }

        // GET: CCE_Weightages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_WEIGHTAGE cCE_WEIGHTAGE = db.CCE_WEIGHTAGE.Find(id);
            if (cCE_WEIGHTAGE == null)
            {
                return HttpNotFound();
            }
            return View(cCE_WEIGHTAGE);
        }

        // POST: CCE_Weightages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CCE_WEIGHTAGE cCE_WEIGHTAGE = db.CCE_WEIGHTAGE.Find(id);
            db.CCE_WEIGHTAGE.Remove(cCE_WEIGHTAGE);
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
