using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SFSAcademy.Controllers
{
    public class Ranking_LevelsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Ranking_Levels
        public ActionResult Index()
        {
            //var batches = db.BATCHes.Include(x => x.COURSE).FirstOrDefault().ACTIVE();           
            List<SelectListItem> options = new SelectList(db.COURSEs.FirstOrDefault().ACTIVE().OrderBy(x => x.ID), "ID", "CRS_NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.CRS_ID = options;
            COURSE course = db.COURSEs.Where(x => x.ID == -1).FirstOrDefault();
            ViewData["course"] = course;

            //var RankingLevels = db.RANKING_LEVEL.Include(g => g.COURSE.BATCHes);

            return View();
        }

        // GET: Ranking_Levels/Details/5
        public ActionResult Load_Ranking_Levels(int? course_id, string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            var ranking_levels = db.RANKING_LEVEL.Where(x => x.CRS_ID == course_id).OrderBy(x=>x.PRIR).ToList();
            ViewData["ranking_levels"] = ranking_levels;
            RANKING_LEVEL ranking_level = db.RANKING_LEVEL.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            ViewData["ranking_level"] = ranking_level;

            return PartialView("_Course_Ranking_Levels");
        }
        public ActionResult Ranking_Level_Cancel(int? course_id, string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            var ranking_levels = db.RANKING_LEVEL.Where(x => x.CRS_ID == course_id).OrderBy(x => x.PRIR).ToList();
            ViewData["ranking_levels"] = ranking_levels;
            RANKING_LEVEL ranking_level = db.RANKING_LEVEL.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            ViewData["ranking_level"] = ranking_level;

            return PartialView("_Course_Ranking_Levels");
        }
        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Ranking_Level([Bind(Include = "ID,NAME,GPA,MKS,SUBJ_CNT,PRIR,CREATED_AT,UPDATED_AT,FULL_CRS,CRS_ID,SUBJ_LMT_TYPE,MKS_LMT_TYPE")] RANKING_LEVEL ranking_level, int? course_id)
        {
            int? priority = 1;
            COURSE course = db.COURSEs.Find(course_id);
            var ranks = db.RANKING_LEVEL.Where(x => x.CRS_ID == course.ID).ToList();
            if (ModelState.IsValid)
            {
                if (ranks != null && ranks.Count() != 0)
                {
                    int? last_priority = ranks.OrderByDescending(x => x.PRIR).Select(x => x.PRIR).FirstOrDefault();
                    priority = last_priority + 1;
                }
                ranking_level.PRIR = priority;
                ranking_level.CRS_ID = course.ID;
                db.RANKING_LEVEL.Add(ranking_level);
                try {
                    db.SaveChanges();
                    var ranking_levels = db.RANKING_LEVEL.Where(x => x.CRS_ID == course.ID).OrderBy(x => x.PRIR).ToList();
                    ViewBag.Notice = "Ranking Level created successfully.";
                    return RedirectToAction("Load_Ranking_Levels", new { course_id = course.ID, Notice = ViewBag.Notice });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return PartialView("_Course_Ranking_Levels");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return PartialView("_Course_Ranking_Levels");
                }
            }
            ViewBag.ErrorMessage = "There seems to have some issue with Model State. Please conteact administrator.";
            ViewData["course"] = course;
            ViewData["ranking_levels"] = ranks;
            return PartialView("_Course_Ranking_Levels");
        }

        public ActionResult Edit_Ranking_Level(int? id)
        {
            RANKING_LEVEL ranking_level = db.RANKING_LEVEL.Find(id);
            COURSE course = db.COURSEs.Find(ranking_level.CRS_ID);
            ViewData["course"] = course;

            return PartialView("_Rank_Edit_Form", ranking_level);
        }
        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Ranking_Level([Bind(Include = "ID,NAME,GPA,MKS,SUBJ_CNT,PRIR,CREATED_AT,UPDATED_AT,FULL_CRS,CRS_ID,SUBJ_LMT_TYPE,MKS_LMT_TYPE")] RANKING_LEVEL ranking_level, int? course_id)
        {
            COURSE course = db.COURSEs.Find(course_id);
            if (ModelState.IsValid)
            {
                ranking_level.CRS_ID = course.ID;
                db.Entry(ranking_level).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    var ranking_levels = db.RANKING_LEVEL.Where(x => x.CRS_ID == course.ID).OrderBy(x => x.PRIR).ToList();
                    ViewBag.Notice = "Ranking Level updated successfully.";
                    return RedirectToAction("Load_Ranking_Levels", new { course_id = course.ID, Notice = ViewBag.Notice });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return PartialView("_Course_Ranking_Levels");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return PartialView("_Course_Ranking_Levels");
                }
            }
            ViewBag.ErrorMessage = "There seems to have some issue with Model State. Please conteact administrator.";
            ViewData["course"] = course;
            var ranks = db.RANKING_LEVEL.Where(x => x.CRS_ID == course.ID).OrderBy(x=>x.PRIR).ToList();
            ViewData["ranking_levels"] = ranks;
            return PartialView("_Course_Ranking_Levels");
        }
        public ActionResult Delete_Ranking_Level(int? id)
        {
            RANKING_LEVEL ranking_level = db.RANKING_LEVEL.Find(id);
            COURSE course = db.COURSEs.Find(ranking_level.CRS_ID);
            ViewData["course"] = course;
            var GreaterRank = db.RANKING_LEVEL.Where(x => x.CRS_ID == course.ID && x.PRIR > ranking_level.PRIR).OrderBy(x => x.PRIR).ToList();
            db.RANKING_LEVEL.Remove(ranking_level);
            try
            {
                db.SaveChanges();
                foreach(var gr in GreaterRank)
                {
                    gr.PRIR = gr.PRIR - 1;
                    db.Entry(gr).State = EntityState.Modified;
                    db.SaveChanges();
                }
                ViewBag.Notice = "Ranking Level deleted successfully.";
                return RedirectToAction("Load_Ranking_Levels", new { course_id = course.ID, Notice = ViewBag.Notice });
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return PartialView("_Course_Ranking_Levels");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return PartialView("_Course_Ranking_Levels");
            }
        }
        public ActionResult Change_Priority(string order, int? id)
        {
            RANKING_LEVEL ranking_level = db.RANKING_LEVEL.Find(id);
            COURSE course = db.COURSEs.Find(ranking_level.CRS_ID);
            var priority = ranking_level.PRIR;
            var ranking_levels = db.RANKING_LEVEL.Where(x=>x.CRS_ID == course.ID).OrderBy(x => x.PRIR).ToList();
            var position = ranking_levels.IndexOf(ranking_level)+1;
            RANKING_LEVEL prev_rank = db.RANKING_LEVEL.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            if (order == "up")
            {
                prev_rank = db.RANKING_LEVEL.Where(x => x.PRIR == position - 1 && x.CRS_ID == course.ID).FirstOrDefault();
            }
            else
            {
                prev_rank = db.RANKING_LEVEL.Where(x => x.PRIR == position + 1 && x.CRS_ID == course.ID).FirstOrDefault();
            }
            ranking_level.PRIR = prev_rank.PRIR;
            db.Entry(ranking_level).State = EntityState.Modified;
            prev_rank.PRIR = priority;
            db.Entry(prev_rank).State = EntityState.Modified;
            try
            {
                db.SaveChanges();               
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return PartialView("_Course_Ranking_Levels");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return PartialView("_Course_Ranking_Levels");
            }

            ranking_levels = db.RANKING_LEVEL.Where(x => x.CRS_ID == course.ID).OrderBy(x => x.PRIR).ToList();
            ViewData["ranking_levels"] = ranking_levels;
            ViewData["course"] = course;
            ViewBag.Notice = "Priority updated successfully.";
            return PartialView("_Ranking_Levels");
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
