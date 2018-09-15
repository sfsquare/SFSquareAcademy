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
    public class Elective_GroupsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Elective_Groups
        public ActionResult Index(int? BatchId, string Notice)
        {
            ViewBag.Notice = Notice;
            var elective_groups = db.ELECTIVE_GROUP.Where(X => X.BTCH_ID == BatchId).ToList();
            //ViewData["elective_groups"] = elective_groups;
            var batch = (from bt in db.BATCHes
                         join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                         where bt.ID == BatchId
                         select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs }).FirstOrDefault();
            ViewData["batch"] = batch;
            var Subject = db.SUBJECTs.Where(x=>x.IS_DEL == "N").ToList();
            ViewData["Subject"] = Subject;

            return View(elective_groups);
        }

        // GET: Elective_Groups/Create
        public ActionResult New(int? BatchId)
        {
            var batch = (from bt in db.BATCHes
                         join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                         where bt.ID == BatchId
                         select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs }).FirstOrDefault();
            ViewData["batch"] = batch;
            return View();
        }

        // POST: Elective_Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "ID,ELECTIVE_GRP_NAME,ELECTIVE_GRP_DESCR,BTCH_ID")] ELECTIVE_GROUP eLECTIVE_GROUP)
        {
            if (ModelState.IsValid)
            {
                db.ELECTIVE_GROUP.Add(eLECTIVE_GROUP);
                db.SaveChanges();
                ViewBag.Notice = "New elective group created.";
                return RedirectToAction("Index",new { BatchId = eLECTIVE_GROUP.BTCH_ID, Notice = ViewBag.Notice });
            }

            return View(eLECTIVE_GROUP);
        }

        // GET: Elective_Groups/Edit/5
        public ActionResult Show(int? id, int? BatchId)
        {
            var electives = db.SUBJECTs.Where(x => x.BTCH_ID == BatchId && x.ELECTIVE_GRP_ID == id && x.IS_DEL == "N").ToList();
            ViewData["electives"] = electives;
            BATCH batch = db.BATCHes.Find(BatchId);
            ViewData["batch"] = batch;
            ELECTIVE_GROUP elective_group = db.ELECTIVE_GROUP.Find(id);
            ViewData["elective_group"] = elective_group;
            return View();
        }

        // GET: Elective_Groups/Edit/5
        public ActionResult Edit(int? id, int? BatchId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ELECTIVE_GROUP eLECTIVE_GROUP = db.ELECTIVE_GROUP.Find(id);
            if (eLECTIVE_GROUP == null)
            {
                return HttpNotFound();
            }
            var batch = (from bt in db.BATCHes
                         join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                         where bt.ID == BatchId
                         select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs }).FirstOrDefault();
            ViewData["batch"] = batch;
            return View(eLECTIVE_GROUP);
        }

        // POST: Elective_Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ELECTIVE_GRP_NAME,ELECTIVE_GRP_DESCR")] ELECTIVE_GROUP eLECTIVE_GROUP, int? BatchId)
        {
            if (ModelState.IsValid)
            {
                ELECTIVE_GROUP eLECTIVE_GROUP_Upd = db.ELECTIVE_GROUP.Find(eLECTIVE_GROUP.ID);
                eLECTIVE_GROUP_Upd.ELECTIVE_GRP_NAME = eLECTIVE_GROUP.ELECTIVE_GRP_NAME;
                eLECTIVE_GROUP_Upd.ELECTIVE_GRP_DESCR = eLECTIVE_GROUP.ELECTIVE_GRP_DESCR;
                eLECTIVE_GROUP_Upd.BTCH_ID = BatchId;
                db.Entry(eLECTIVE_GROUP_Upd).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Notice = "Elective group updated successfully!";
                return RedirectToAction("Index", new { BatchId = BatchId, Notice = ViewBag.Notice });
            }
            return View(eLECTIVE_GROUP);
        }

        // GET: Elective_Groups/Delete/5
        public ActionResult Delete(int? id, int? BatchId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ELECTIVE_GROUP eLECTIVE_GROUP = db.ELECTIVE_GROUP.Find(id);
            if (eLECTIVE_GROUP == null)
            {
                return HttpNotFound();
            }
            var subjects = db.SUBJECTs.Where(x => x.ELECTIVE_GRP_ID == id).ToList();
            if(subjects != null || subjects.Count() != 0)
            {
                foreach(var item in subjects)
                {
                    var StudentSubject = db.STUDENT_SUBJECT.Where(x => x.SUBJ_ID == item.ID && x.BTCH_ID == BatchId).ToList();
                    foreach(var item2 in StudentSubject)
                    {
                        db.STUDENT_SUBJECT.Remove(item2);
                    }
                    db.SUBJECTs.Remove(item);
                }

            }
            db.ELECTIVE_GROUP.Remove(eLECTIVE_GROUP);
            try { db.SaveChanges();}
            catch (Exception e) {ViewBag.Notice = e.InnerException.InnerException.Message;
                return RedirectToAction("Index", new { BatchId = BatchId, Notice = ViewBag.Notice });
            }
            ViewBag.Notice = "Deleted elective group!";
            return RedirectToAction("Index", new { BatchId = BatchId, Notice = ViewBag.Notice });
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
