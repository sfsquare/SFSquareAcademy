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
    public class FA_GroupsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: FA_Groups
        public ActionResult Index()
        {
            var fA_GROUP = db.FA_GROUP.Include(f => f.CCE_EXAM_CATEGORY).Include(f => f.CCE_GRADE_SET);
            return View(fA_GROUP.ToList());
        }

        // GET: FA_Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FA_GROUP fA_GROUP = db.FA_GROUP.Find(id);
            if (fA_GROUP == null)
            {
                return HttpNotFound();
            }
            return View(fA_GROUP);
        }

        // GET: FA_Groups/Create
        public ActionResult Create()
        {
            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME");
            ViewBag.CCE_GRADE_SET_ID = new SelectList(db.CCE_GRADE_SET, "ID", "NAME");
            return View();
        }

        // POST: FA_Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,DESCR,CCE_EXAM_CAT_ID,CREATED_AT,UPDATED_AT,CCE_GRADE_SET_ID,MAX_MKS,IS_DEL")] FA_GROUP fA_GROUP)
        {
            if (ModelState.IsValid)
            {
                db.FA_GROUP.Add(fA_GROUP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME", fA_GROUP.CCE_EXAM_CAT_ID);
            ViewBag.CCE_GRADE_SET_ID = new SelectList(db.CCE_GRADE_SET, "ID", "NAME", fA_GROUP.CCE_GRADE_SET_ID);
            return View(fA_GROUP);
        }

        // GET: FA_Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FA_GROUP fA_GROUP = db.FA_GROUP.Find(id);
            if (fA_GROUP == null)
            {
                return HttpNotFound();
            }
            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME", fA_GROUP.CCE_EXAM_CAT_ID);
            ViewBag.CCE_GRADE_SET_ID = new SelectList(db.CCE_GRADE_SET, "ID", "NAME", fA_GROUP.CCE_GRADE_SET_ID);
            return View(fA_GROUP);
        }

        // POST: FA_Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,DESCR,CCE_EXAM_CAT_ID,CREATED_AT,UPDATED_AT,CCE_GRADE_SET_ID,MAX_MKS,IS_DEL")] FA_GROUP fA_GROUP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fA_GROUP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CCE_EXAM_CAT_ID = new SelectList(db.CCE_EXAM_CATEGORY, "ID", "NAME", fA_GROUP.CCE_EXAM_CAT_ID);
            ViewBag.CCE_GRADE_SET_ID = new SelectList(db.CCE_GRADE_SET, "ID", "NAME", fA_GROUP.CCE_GRADE_SET_ID);
            return View(fA_GROUP);
        }

        // GET: FA_Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FA_GROUP fA_GROUP = db.FA_GROUP.Find(id);
            if (fA_GROUP == null)
            {
                return HttpNotFound();
            }
            return View(fA_GROUP);
        }

        // POST: FA_Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FA_GROUP fA_GROUP = db.FA_GROUP.Find(id);
            db.FA_GROUP.Remove(fA_GROUP);
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
