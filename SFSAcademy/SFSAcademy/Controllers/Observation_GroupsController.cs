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
    public class Observation_GroupsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Observation_Groups
        public ActionResult Index()
        {
            var oBSERVATION_GROUP = db.OBSERVATION_GROUP.Include(o => o.CCE_GRADE_SET);
            return View(oBSERVATION_GROUP.ToList());
        }

        // GET: Observation_Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBSERVATION_GROUP oBSERVATION_GROUP = db.OBSERVATION_GROUP.Find(id);
            if (oBSERVATION_GROUP == null)
            {
                return HttpNotFound();
            }
            return View(oBSERVATION_GROUP);
        }

        // GET: Observation_Groups/Create
        public ActionResult Create()
        {
            ViewBag.CCE_GRADE_SET_ID = new SelectList(db.CCE_GRADE_SET, "ID", "NAME");
            return View();
        }

        // POST: Observation_Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,HDR_NAME,DESCR,CCE_GRADE_SET_ID,CREATED_AT,UPDATED_AT,OBSV_KIND,MAX_MKS,IS_DEL")] OBSERVATION_GROUP oBSERVATION_GROUP)
        {
            if (ModelState.IsValid)
            {
                db.OBSERVATION_GROUP.Add(oBSERVATION_GROUP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CCE_GRADE_SET_ID = new SelectList(db.CCE_GRADE_SET, "ID", "NAME", oBSERVATION_GROUP.CCE_GRADE_SET_ID);
            return View(oBSERVATION_GROUP);
        }

        // GET: Observation_Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBSERVATION_GROUP oBSERVATION_GROUP = db.OBSERVATION_GROUP.Find(id);
            if (oBSERVATION_GROUP == null)
            {
                return HttpNotFound();
            }
            ViewBag.CCE_GRADE_SET_ID = new SelectList(db.CCE_GRADE_SET, "ID", "NAME", oBSERVATION_GROUP.CCE_GRADE_SET_ID);
            return View(oBSERVATION_GROUP);
        }

        // POST: Observation_Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,HDR_NAME,DESCR,CCE_GRADE_SET_ID,CREATED_AT,UPDATED_AT,OBSV_KIND,MAX_MKS,IS_DEL")] OBSERVATION_GROUP oBSERVATION_GROUP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oBSERVATION_GROUP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CCE_GRADE_SET_ID = new SelectList(db.CCE_GRADE_SET, "ID", "NAME", oBSERVATION_GROUP.CCE_GRADE_SET_ID);
            return View(oBSERVATION_GROUP);
        }

        // GET: Observation_Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OBSERVATION_GROUP oBSERVATION_GROUP = db.OBSERVATION_GROUP.Find(id);
            if (oBSERVATION_GROUP == null)
            {
                return HttpNotFound();
            }
            return View(oBSERVATION_GROUP);
        }

        // POST: Observation_Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OBSERVATION_GROUP oBSERVATION_GROUP = db.OBSERVATION_GROUP.Find(id);
            db.OBSERVATION_GROUP.Remove(oBSERVATION_GROUP);
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
