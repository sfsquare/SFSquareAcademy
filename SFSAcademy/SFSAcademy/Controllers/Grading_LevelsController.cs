using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using PagedList;
using SFSAcademy;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace SFSAcademy.Controllers
{
    public class Grading_LevelsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Grading_Levels
        public ActionResult Index(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var batches = db.BATCHes.Include(x => x.COURSE).FirstOrDefault().ACTIVE();
            List<SelectListItem> options = new SelectList(batches.OrderBy(x => x.ID), "ID", "Course_full_name").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Batch" });
            ViewBag.BATCH_ID = options;
            ViewData["batch"] = null;

            var grading_levels = db.GRADING_LEVEL.FirstOrDefault().Default();
            ViewData["grading_levels"] = grading_levels;
            return View();
        }

        // GET: Employee_Attendances
        public ActionResult Show(int? batch_id, string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            ViewData["batch"] = null;
            if(batch_id == null)
            {
                var grading_levels = db.GRADING_LEVEL.FirstOrDefault().Default();
                ViewData["grading_levels"] = grading_levels;
            }
            else
            {
                var grading_levels = db.GRADING_LEVEL.FirstOrDefault().For_Batch((int)batch_id);
                ViewData["grading_levels"] = grading_levels;
                BATCH batch = db.BATCHes.Find(batch_id);
                ViewData["batch"] = batch;
            }
            return PartialView("_Grading_Levels");
        }

        public ActionResult New(int? id)
        {
            BATCH batch = db.BATCHes.Where(x => x.ID == -1).FirstOrDefault();
            if (id != null)
            {
                batch = db.BATCHes.Find(id);
                ViewBag.credit = (batch.GPA_Enabled() || batch.CCE_Enabled()) ? true : false;
            }
            else
            {
                ViewBag.credit = (db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CCE").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString() == "1" || db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CWA").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString() == "1" || db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "GPA").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString() == "1") ? true : false;
            }
            ViewData["batch"] = batch;

            return PartialView("_New");
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,BTCH_ID,MIN_SCORE,ORD,IS_DEL,CREATED_AT,UPDATED_AT,CRED_PT,DESCR")] GRADING_LEVEL gRADING_LEVEL, int? batch_id)
        {
            BATCH batch = db.BATCHes.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            var grading_levels = db.GRADING_LEVEL.Where(x => x.ID == -1).DefaultIfEmpty().ToList();
            if (batch_id != null)
            {
                batch = db.BATCHes.Find(batch_id);
            }
            ViewData["batch"] = batch;
            if (ModelState.IsValid)
            {
                gRADING_LEVEL.BTCH_ID = batch_id;
                db.GRADING_LEVEL.Add(gRADING_LEVEL);
                try { 
                    db.SaveChanges();
                    if (batch_id == null)
                    {
                        grading_levels = db.GRADING_LEVEL.FirstOrDefault().Default().ToList();
                    }
                    else
                    {
                        grading_levels = db.GRADING_LEVEL.FirstOrDefault().For_Batch((int)batch_id).ToList();
                    }
                    ViewBag.Notice = "Grading level created sucessfully.";
                    ViewData["grading_levels"] = grading_levels;
                    return PartialView("_Grading_Levels");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(gRADING_LEVEL);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(gRADING_LEVEL);
                }
            }
            else
            {
                return View(gRADING_LEVEL);
            }
        }

        // GET: Grading_Levels/Edit/5
        public ActionResult Edit(int? id, int? batch_id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRADING_LEVEL gRADING_LEVEL = db.GRADING_LEVEL.Find(id);
            if (gRADING_LEVEL == null)
            {
                return HttpNotFound();
            }
            BATCH batch = db.BATCHes.Where(x => x.ID == -1).FirstOrDefault();
            if (batch_id != null)
            {
                batch = db.BATCHes.Find(batch_id);
                ViewBag.credit = (batch.GPA_Enabled() || batch.CCE_Enabled()) ? true : false;
            }
            else
            {
                ViewBag.credit = (db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CCE").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString() == "1" || db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CWA").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString() == "1" || db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "GPA").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString() == "1") ? true : false;
            }
            ViewData["batch"] = batch;

            return PartialView("_Edit", gRADING_LEVEL);
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "ID,NAME,BTCH_ID,MIN_SCORE,ORD,IS_DEL,CREATED_AT,UPDATED_AT,CRED_PT,DESCR")] GRADING_LEVEL gRADING_LEVEL, int? batch_id)
        {
            BATCH batch = db.BATCHes.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            var grading_levels = db.GRADING_LEVEL.Where(x => x.ID == -1).DefaultIfEmpty().ToList();
            if (batch_id != null)
            {
                batch = db.BATCHes.Find(batch_id);
            }
            ViewData["batch"] = batch;

            if (ModelState.IsValid)
            {
                var GR_Update = db.GRADING_LEVEL.Find(gRADING_LEVEL.ID);
                GR_Update.NAME = gRADING_LEVEL.NAME;
                GR_Update.BTCH_ID = batch_id;
                GR_Update.MIN_SCORE = gRADING_LEVEL.MIN_SCORE;
                GR_Update.ORD = gRADING_LEVEL.ORD;
                GR_Update.DESCR = gRADING_LEVEL.DESCR;
                GR_Update.CRED_PT = gRADING_LEVEL.CRED_PT;
                GR_Update.IS_DEL = gRADING_LEVEL.IS_DEL;
                GR_Update.UPDATED_AT = DateTime.Now;
                db.Entry(GR_Update).State = EntityState.Modified;
                try { 
                    db.SaveChanges();
                    if (batch_id == null)
                    {
                        grading_levels = db.GRADING_LEVEL.FirstOrDefault().Default().ToList();
                    }
                    else
                    {
                        grading_levels = db.GRADING_LEVEL.FirstOrDefault().For_Batch((int)batch_id).ToList();
                    }
                    ViewBag.Notice = "Grading level updated sucessfully.";
                    ViewData["grading_levels"] = grading_levels;
                    return PartialView("_Grading_Levels");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return PartialView("_Grading_Levels");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return PartialView("_Grading_Levels");
                }
            }
            else
            {
                return View(gRADING_LEVEL);
            }
        }

        // GET: Grading_Levels/Delete/5
        public ActionResult Delete(int? id, int? batch_id)
        {
            BATCH batch = db.BATCHes.Where(x => x.ID == -1).FirstOrDefault();
            var grading_levels = db.GRADING_LEVEL.Where(x => x.BTCH_ID == -1).DefaultIfEmpty().ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GRADING_LEVEL grading_level = db.GRADING_LEVEL.Find(id);
            if (grading_level == null)
            {
                return HttpNotFound();
            }
            grading_level.Inactivate();
            if (batch_id == null)
            {
                grading_levels = db.GRADING_LEVEL.FirstOrDefault().Default().ToList();
            }
            else
            {
                grading_levels = db.GRADING_LEVEL.FirstOrDefault().For_Batch((int)batch_id).ToList();
                batch = db.BATCHes.Find(batch_id);
            }
            ViewBag.Notice = "Grading level deleted sucessfully.";
            ViewData["grading_levels"] = grading_levels;
            ViewData["batch"] = batch;
            return RedirectToAction("Index", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage});
        }

        [AllowAnonymous]
        public JsonResult GradeExists([Bind(Prefix = "NAME")] string NAME)
        {   
            return Json(!db.GRADING_LEVEL.Any(x => x.NAME.ToUpper() == NAME.ToUpper() && x.IS_DEL == false), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult CreditPointRequired([Bind(Prefix = "CRED_PT")] decimal? CRED_PT, [Bind(Prefix = "BTCH_ID")] int? BTCH_ID)
        {
            var batch = db.BATCHes.Find(BTCH_ID);
            return Json(!(BTCH_ID != null && CRED_PT == null && batch.GPA_Enabled() == true), JsonRequestBehavior.AllowGet);
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
