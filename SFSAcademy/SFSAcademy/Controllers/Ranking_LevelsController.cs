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
    public class Ranking_LevelsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Ranking_Levels
        public ActionResult Index()
        {
            var rANKING_LEVEL = db.RANKING_LEVEL.Include(r => r.COURSE);
            return View(rANKING_LEVEL.ToList());
        }

        // GET: Ranking_Levels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RANKING_LEVEL rANKING_LEVEL = db.RANKING_LEVEL.Find(id);
            if (rANKING_LEVEL == null)
            {
                return HttpNotFound();
            }
            return View(rANKING_LEVEL);
        }

        // GET: Ranking_Levels/Create
        public ActionResult Create()
        {
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME");
            return View();
        }

        // POST: Ranking_Levels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,GPA,MKS,SUBJ_CNT,PRIR,CREATED_AT,UPDATED_AT,FULL_CRS,CRS_ID,SUBJ_LMT_TYPE,MKS_LMT_TYPE")] RANKING_LEVEL rANKING_LEVEL)
        {
            if (ModelState.IsValid)
            {
                db.RANKING_LEVEL.Add(rANKING_LEVEL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", rANKING_LEVEL.CRS_ID);
            return View(rANKING_LEVEL);
        }

        // GET: Ranking_Levels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RANKING_LEVEL rANKING_LEVEL = db.RANKING_LEVEL.Find(id);
            if (rANKING_LEVEL == null)
            {
                return HttpNotFound();
            }
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", rANKING_LEVEL.CRS_ID);
            return View(rANKING_LEVEL);
        }

        // POST: Ranking_Levels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,GPA,MKS,SUBJ_CNT,PRIR,CREATED_AT,UPDATED_AT,FULL_CRS,CRS_ID,SUBJ_LMT_TYPE,MKS_LMT_TYPE")] RANKING_LEVEL rANKING_LEVEL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rANKING_LEVEL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", rANKING_LEVEL.CRS_ID);
            return View(rANKING_LEVEL);
        }

        // GET: Ranking_Levels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RANKING_LEVEL rANKING_LEVEL = db.RANKING_LEVEL.Find(id);
            if (rANKING_LEVEL == null)
            {
                return HttpNotFound();
            }
            return View(rANKING_LEVEL);
        }

        // POST: Ranking_Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RANKING_LEVEL rANKING_LEVEL = db.RANKING_LEVEL.Find(id);
            db.RANKING_LEVEL.Remove(rANKING_LEVEL);
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
