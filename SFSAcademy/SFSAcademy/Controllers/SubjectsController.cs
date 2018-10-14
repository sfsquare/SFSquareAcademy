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
    public class SubjectsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Subjects
        public ActionResult Index(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL== false
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
            return View();
        }

        public ActionResult Show(int? id)
        {
            //var batch = db.BATCHes.Find(id);
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    join sub in db.SUBJECTs.Where(x=>x.ELECTIVE_GRP_ID != null && x.IS_DEL == false) on bt.ID equals sub.BTCH_ID into gsub
                                    from subgsub in gsub.DefaultIfEmpty()
                                    where cs.IS_DEL == false && bt.ID == id
                                    select new Models.CoursesBatch { CourseData = cs, BatchData = bt, Elective_Batch_Subject = (subgsub == null ? null : subgsub) })
                         .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = queryCourceBatch;
            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == id && x.ELECTIVE_GRP_ID == null && x.IS_DEL ==false).ToList();
            ViewData["subjects"] = subjects;
            var elective_groups = (from eg in db.ELECTIVE_GROUP
                                   join sub in db.SUBJECTs.Where(x=>x.IS_DEL == false) on eg.ID equals sub.ELECTIVE_GRP_ID
                                   where sub.BTCH_ID == id
                                   select new SFSAcademy.Models.ElectiveGroups { ElectiveGroupData = eg}).Distinct().OrderBy(x => x.ElectiveGroupData.ELECTIVE_GRP_NAME).ToList();
            ViewData["elective_groups"] = elective_groups;
            //var subject = db.SUBJECTs.Find(sub_id);
            ViewData["subject"] = null;
            var Exam = db.EXAMs.ToList();
            ViewData["Exam"] = Exam;
            var TimetableEntry = (from tte in db.TIMETABLE_ENTRY
                                  join ssub in db.STUDENT_SUBJECT on tte.BTCH_ID equals ssub.BTCH_ID
                                  join sub in db.SUBJECTs.Where(x => x.IS_DEL == false) on ssub.SUBJ_ID equals sub.ID
                                  select new SFSAcademy.Models.TimetableEntry { TimeTableEntryData = tte, StudentSubjectData = ssub, SubjectData = sub }).ToList();
            ViewData["TimetableEntry"] = TimetableEntry;
            return PartialView("_Subjects");
        }

        // GET: Subjects/Create
        public ActionResult New(int? id, int? id2)
        {
            //BATCH batch = db.BATCHes.Find(id);
            var batch = (from bt in db.BATCHes
                         join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                         where bt.ID == id
                         select new SFSAcademy.Models.CoursesBatch { BatchData = bt, CourseData = cs }).FirstOrDefault();
            ViewData["batch"] = batch;
            if(id2 != null)
            {
                ELECTIVE_GROUP elective_group = db.ELECTIVE_GROUP.Find(id2);
                ViewData["elective_group"] = elective_group;
            }
            else
            {
                ViewData["elective_group"] = null;
            }
            GRADING_LEVEL greading_level = db.GRADING_LEVEL.Where(x=>x.BTCH_ID == id).FirstOrDefault();
            ViewData["greading_level"] = greading_level;

            return PartialView("_New");
        }


        // POST: Subjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,CODE,BTCH_ID,NO_EXAMS,MAX_WKILY_CLSES,ELECTIVE_GRP_ID,IS_DEL,CREATED_AT,UPDATED_AT,CR_HRS,PREF_CNSC,AMT")] SUBJECT sUBJECT)
        {
            if (ModelState.IsValid)
            {
                sUBJECT.CREATED_AT = System.DateTime.Now;
                sUBJECT.UPDATED_AT = System.DateTime.Now;
                sUBJECT.IS_DEL = false;
                db.SUBJECTs.Add(sUBJECT);
                db.SaveChanges();
                ViewBag.Notice = "New subject added sucessfully!";
                return RedirectToAction("Index",new { Notice = ViewBag.Notice });
            }

            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME", sUBJECT.BTCH_ID);
            return View(sUBJECT);
        }

        // GET: Subjects/Edit/5
        public ActionResult Edit(int? id, int? id2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SUBJECT subject = db.SUBJECTs.Find(id);
            ViewData["subject"] = subject;
            if (subject == null)
            {
                return HttpNotFound();
            }
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.ID == subject.BTCH_ID
                                    select new Models.CoursesBatch { CourseData = cs, BatchData = bt })
                         .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = queryCourceBatch;
            if(id2 != null)
            {
                ELECTIVE_GROUP elective_group = db.ELECTIVE_GROUP.Find(id2);
                ViewData["elective_group"] = elective_group;
            }
            GRADING_LEVEL greading_level = db.GRADING_LEVEL.Where(x => x.BTCH_ID == subject.BTCH_ID).FirstOrDefault();
            ViewData["greading_level"] = greading_level;
            return PartialView("_Edit", subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,CODE,BTCH_ID,NO_EXAMS,MAX_WKILY_CLSES,ELECTIVE_GRP_ID,IS_DEL,CREATED_AT,UPDATED_AT,CR_HRS,PREF_CNSC,AMT")] SUBJECT sUBJECT)
        {
            if (ModelState.IsValid)
            {
                SUBJECT sUBJECT_toUpdate = db.SUBJECTs.Find(sUBJECT.ID);
                sUBJECT_toUpdate.NAME = sUBJECT.NAME;
                sUBJECT_toUpdate.CODE = sUBJECT.CODE;
                sUBJECT_toUpdate.NO_EXAMS = sUBJECT.NO_EXAMS;
                sUBJECT_toUpdate.MAX_WKILY_CLSES = sUBJECT.MAX_WKILY_CLSES;
                db.Entry(sUBJECT_toUpdate).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Notice = "Subject edited sucessfully!";
                return RedirectToAction("Index",new { Notice = ViewBag.Notice });
            }
            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME", sUBJECT.BTCH_ID);
            return View(sUBJECT);
        }

        // GET: Subjects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SUBJECT sUBJECT = db.SUBJECTs.Find(id);
            if (sUBJECT == null)
            {
                return HttpNotFound();
            }
            var subject_exams = db.EXAMs.Where(x => x.SUBJ_ID == id).ToList();
            if(subject_exams == null || subject_exams.Count() == 0)
            {
                sUBJECT.UPDATED_AT = System.DateTime.Now;
                sUBJECT.IS_DEL = true;
                db.Entry(sUBJECT).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Notice = "Subject Deleted successfully!";
            }
            else
            {
                ViewBag.Notice = "Cannot Delete Subjects";
            }
            return RedirectToAction("Index",new { Notice = ViewBag.Notice });
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
