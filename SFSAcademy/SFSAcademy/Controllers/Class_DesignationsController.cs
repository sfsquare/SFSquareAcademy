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
    public class Class_DesignationsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Class_Designations
        public ActionResult Index()
        {          
            List<SelectListItem> options = new SelectList(db.COURSEs.FirstOrDefault().ACTIVE().OrderBy(x => x.ID), "ID", "CRS_NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.CRS_ID = options;
            COURSE course = db.COURSEs.Where(x => x.ID == -1).FirstOrDefault();
            ViewData["course"] = course;

            return View();
        }

        public ActionResult Load_Class_Designations(int? course_id, string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course_id).ToList();
            ViewData["class_designations"] = class_designations;
            CLASS_DESIGNATION class_designation = db.CLASS_DESIGNATION.Where(x => x.ID == -1).DefaultIfEmpty().FirstOrDefault();
            ViewData["class_designation"] = class_designation;

            return PartialView("_Course_Class_Designations");
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Class_Designation([Bind(Include = "ID,NAME,CGPA,MKS,CREATED_AT,UPDATED_AT,CRS_ID")] CLASS_DESIGNATION class_designation, int? course_id)
        {
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course_id).ToList();
            ViewData["class_designations"] = class_designations;
            if (ModelState.IsValid)
            {
                class_designation.CRS_ID = course.ID;
                db.CLASS_DESIGNATION.Add(class_designation);
                try
                {
                    db.SaveChanges();
                    class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course_id).ToList();
                    ViewData["class_designations"] = class_designations;
                    ViewBag.Notice = "Class Designation created successfully.";
                    return PartialView("_Course_Class_Designations");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(class_designation);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(class_designation);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "There is some issue in model state. Please cotact administrator.";
                return View(class_designation);
            }
        }
        public ActionResult Edit_Class_Designation(int? id)
        {
            CLASS_DESIGNATION class_designation = db.CLASS_DESIGNATION.Find(id);
            COURSE course = db.COURSEs.Find(class_designation.CRS_ID);
            ViewData["course"] = course;

            return PartialView("_Class_Edit_Form", class_designation);
        }
        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Class_Designation([Bind(Include = "ID,NAME,CGPA,MKS,CREATED_AT,UPDATED_AT,CRS_ID")] CLASS_DESIGNATION class_designation, int? course_id)
        {
            COURSE course = db.COURSEs.Find(course_id);
            ViewData["course"] = course;
            if (ModelState.IsValid)
            {
                class_designation.CRS_ID = course.ID;
                db.Entry(class_designation).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course_id).ToList();
                    ViewData["class_designations"] = class_designations;
                    ViewBag.Notice = "Class Designation updated successfully.";
                    return RedirectToAction("Load_Class_Designations", new { course_id = course.ID, Notice = ViewBag.Notice });
                }
                catch (DbEntityValidationException e)
                {
                    var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course_id).ToList();
                    ViewData["class_designations"] = class_designations;
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(class_designation);
                }
                catch (Exception e)
                {
                    var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course_id).ToList();
                    ViewData["class_designations"] = class_designations;
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(class_designation);
                }
            }
            else
            {
                var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course_id).ToList();
                ViewData["class_designations"] = class_designations;
                ViewBag.ErrorMessage = "There is some issue in model state. Please cotact administrator.";
                return View(class_designation);
            }
        }
        public ActionResult Delete_Class_Designation(int? id)
        {
            CLASS_DESIGNATION class_designation = db.CLASS_DESIGNATION.Find(id);
            COURSE course = db.COURSEs.Find(class_designation.CRS_ID);
            ViewData["course"] = course;
            db.CLASS_DESIGNATION.Remove(class_designation);
            try
            {
                db.SaveChanges();
                var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course.ID).ToList();
                ViewData["class_designations"] = class_designations;
                ViewBag.Notice = "Class Designation deleted successfully.";
                return RedirectToAction("Load_Class_Designations", new { course_id = course.ID, Notice = ViewBag.Notice });
            }
            catch (DbEntityValidationException e)
            {
                var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course.ID).ToList();
                ViewData["class_designations"] = class_designations;
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return View(class_designation);
            }
            catch (Exception e)
            {
                var class_designations = db.CLASS_DESIGNATION.Where(x => x.CRS_ID == course.ID).ToList();
                ViewData["class_designations"] = class_designations;
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return View(class_designation);
            }
        }

        [AllowAnonymous]
        public JsonResult CGPAIsNumeric([Bind(Prefix = "CGPA")] decimal? CGPA, [Bind(Prefix = "CRS_ID")] int? CRS_ID)
        {
            var course = db.COURSEs.Find(CRS_ID);
            int Num;
            bool isNum = int.TryParse(CGPA.ToString(), out Num);
            return Json(!(CRS_ID != null && isNum == false && course.GPA_Enabled()) == true, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult MarksIsNumeric([Bind(Prefix = "MKS")] decimal? MKS, [Bind(Prefix = "CRS_ID")] int? CRS_ID)
        {
            var course = db.COURSEs.Find(CRS_ID);
            int Num;
            bool isNum = int.TryParse(MKS.ToString(), out Num);
            return Json(!(CRS_ID != null && isNum == false && course.CWA_Enabled() == true), JsonRequestBehavior.AllowGet);
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
