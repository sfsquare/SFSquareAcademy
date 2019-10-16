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
    public class CCE_Exam_CategoriesController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: CCE_Exam_Categories
        public ActionResult Index()
        {
            return View(db.CCE_EXAM_CATEGORY.ToList());
        }

        // GET: CCE_Exam_Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_EXAM_CATEGORY cCE_EXAM_CATEGORY = db.CCE_EXAM_CATEGORY.Find(id);
            if (cCE_EXAM_CATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(cCE_EXAM_CATEGORY);
        }

        // GET: CCE_Exam_Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CCE_Exam_Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,DESCR,CREATED_AT,UPDATED_AT")] CCE_EXAM_CATEGORY cCE_EXAM_CATEGORY)
        {
            if (ModelState.IsValid)
            {
                db.CCE_EXAM_CATEGORY.Add(cCE_EXAM_CATEGORY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cCE_EXAM_CATEGORY);
        }

        // GET: CCE_Exam_Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_EXAM_CATEGORY cCE_EXAM_CATEGORY = db.CCE_EXAM_CATEGORY.Find(id);
            if (cCE_EXAM_CATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(cCE_EXAM_CATEGORY);
        }

        // POST: CCE_Exam_Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,DESCR,CREATED_AT,UPDATED_AT")] CCE_EXAM_CATEGORY cCE_EXAM_CATEGORY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cCE_EXAM_CATEGORY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cCE_EXAM_CATEGORY);
        }

        // GET: CCE_Exam_Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_EXAM_CATEGORY cCE_EXAM_CATEGORY = db.CCE_EXAM_CATEGORY.Find(id);
            if (cCE_EXAM_CATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(cCE_EXAM_CATEGORY);
        }

        // POST: CCE_Exam_Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CCE_EXAM_CATEGORY cCE_EXAM_CATEGORY = db.CCE_EXAM_CATEGORY.Find(id);
            db.CCE_EXAM_CATEGORY.Remove(cCE_EXAM_CATEGORY);
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
