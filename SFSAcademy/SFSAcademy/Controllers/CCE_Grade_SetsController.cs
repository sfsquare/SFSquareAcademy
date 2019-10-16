using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFSAcademy;

namespace SFSAcademy.Controllers
{
    public class CCE_Grade_SetsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: CCE_Grade_Sets
        public ActionResult Index(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var grade_sets = db.CCE_GRADE_SET.ToList();
            ViewData["grade_sets"] = grade_sets;
            return View();
        }

        // GET: CCE_Grade_Sets/Details/5
        public ActionResult Show(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_GRADE_SET grade_set = db.CCE_GRADE_SET.Find(id);
            if (grade_set == null)
            {
                return HttpNotFound();
            }
            ViewData["grade_set"] = grade_set;
            var grades = db.CCE_GRADE.Where(x => x.CCE_GRADE_SET_ID == id).ToList();
            ViewData["grades"] = grades;
            return PartialView("_Grades", grades);
        }

        // GET: CCE_Grade_Sets/Create
        public ActionResult New()
        {
            return PartialView("_New");
        }

        // POST: CCE_Grade_Sets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,CREATED_AT,UPDATED_AT")] CCE_GRADE_SET grade_set)
        {
            if (ModelState.IsValid)
            {
                db.CCE_GRADE_SET.Add(grade_set);
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = "CCE Gradeset created successfully.";
                    var grade_sets = db.CCE_GRADE_SET.ToList();
                    ViewData["grade_sets"] = grade_sets;
                    return PartialView("_Grade_Sets");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(grade_set);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(grade_set);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "There is some issue in model state. Please cotact administrator.";
                return View(grade_set);
            }
        }

        // GET: CCE_Grade_Sets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_GRADE_SET grade_set = db.CCE_GRADE_SET.Find(id);
            if (grade_set == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit", grade_set);
        }

        // POST: CCE_Grade_Sets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "ID,NAME,CREATED_AT,UPDATED_AT")] CCE_GRADE_SET grade_set)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grade_set).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = "CCE Gradeset updated successfully.";
                    var grade_sets = db.CCE_GRADE_SET.ToList();
                    ViewData["grade_sets"] = grade_sets;
                    return PartialView("_Grade_Sets");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(grade_set);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(grade_set);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "There is some issue in model state. Please cotact administrator.";
                return View(grade_set);
            }
        }

        public ActionResult Destroy(int id)
        {
            CCE_GRADE_SET grade_set = db.CCE_GRADE_SET.Include(x=>x.OBSERVATION_GROUP).Where(x=>x.ID == id).FirstOrDefault();
            if(grade_set.OBSERVATION_GROUP == null || grade_set.OBSERVATION_GROUP.Count() == 0)
            {
                db.CCE_GRADE_SET.Remove(grade_set);
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = "Grade set deleted.";
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(grade_set);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(grade_set);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Grade set is associated to some Co-Scholastic groups. Clear them before deleting.";
            }
            var grade_sets = db.CCE_GRADE_SET.ToList();
            ViewData["grade_sets"] = grade_sets;
            return PartialView("_Grade_Sets");
        }
        public ActionResult New_Grade(int? id)
        {
            CCE_GRADE_SET grade_set = db.CCE_GRADE_SET.Find(id);
            ViewData["grade_set"] = grade_set;
            return PartialView("_New_Grade");
        }

        // POST: CCE_Grade_Sets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Grade([Bind(Include = "ID,NAME,GRADE_PT,CCE_GRADE_SET_ID,CREATED_AT,UPDATED_AT")] CCE_GRADE grade)
        {
            CCE_GRADE_SET grade_set = db.CCE_GRADE_SET.Find(grade.CCE_GRADE_SET_ID);
            ViewData["grade_set"] = grade_set;
            if (ModelState.IsValid)
            {
                db.CCE_GRADE.Add(grade);
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = "Grade created successfully";
                    var grades = db.CCE_GRADE.Where(x => x.CCE_GRADE_SET_ID == grade.CCE_GRADE_SET_ID).ToList();
                    ViewData["grades"] = grades;
                    return PartialView("_Grades");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(grade);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(grade);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "There is some issue in model state. Please cotact administrator.";
                return View(grade);
            }
        }

        // GET: CCE_Grade_Sets/Edit/5
        public ActionResult Edit_Grade(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CCE_GRADE grade = db.CCE_GRADE.Find(id);
            if (grade == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit_Grade", grade);
        }

        // POST: CCE_Grade_Sets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Grade([Bind(Include = "ID,NAME,GRADE_PT,CCE_GRADE_SET_ID,CREATED_AT,UPDATED_AT")] CCE_GRADE grade)
        {
            CCE_GRADE_SET grade_set = db.CCE_GRADE_SET.Find(grade.CCE_GRADE_SET_ID);
            ViewData["grade_set"] = grade_set;
            if (ModelState.IsValid)
            {
                db.Entry(grade).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = "Grade updated successfully.";
                    var grades = db.CCE_GRADE.Where(x => x.CCE_GRADE_SET_ID == grade.CCE_GRADE_SET_ID).ToList();
                    ViewData["grades"] = grades;
                    return PartialView("_Grades");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(grade);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(grade);
                }
            }
            else
            {
                ViewBag.ErrorMessage = "There is some issue in model state. Please cotact administrator.";
                return View(grade);
            }
        }

        public ActionResult Destroy_Grade(int id)
        {
            CCE_GRADE grade = db.CCE_GRADE.Find(id);
            CCE_GRADE_SET grade_set = db.CCE_GRADE_SET.Find(grade.CCE_GRADE_SET_ID);
            ViewData["grade_set"] = grade_set;
            int? Set_Id = grade.CCE_GRADE_SET_ID;
            db.CCE_GRADE.Remove(grade);
            try
            {
                db.SaveChanges();
                ViewBag.Notice = "Grade deleted.";
                var grades = db.CCE_GRADE.Where(x => x.CCE_GRADE_SET_ID == Set_Id).ToList();
                ViewData["grades"] = grades;
                return PartialView("_Grades");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return View(grade);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return View(grade);
            }
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
