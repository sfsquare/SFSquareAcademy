using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI;

namespace SFSAcademy.Controllers
{
    public class BATCHController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: BATCH
        public ActionResult Index()
        {
            var bATCHes = db.BATCHes.Include(b => b.COURSE);
            return View(bATCHes.ToList());
        }

        // GET: Batches
        public ActionResult ManageBatches(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            //List<SelectListItem> options = new SelectList(db.COURSEs.OrderBy(x => x.ID), "ID", "CRS_NAME").ToList();
            //add the 'ALL' option
            //options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            //ViewBag.CurrentFilter = "Active";
            return View();
        }

        public ActionResult _Update_Batch(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null && !searchString.Equals("-1"))
            {
                page = 1;
                ///As Drop down list sends Id, we will ahve to convert this to text which is different from text box
                //int searchStringId = Convert.ToInt32(searchString);
                //searchString = db.COURSEs.Find(searchStringId).CODE.ToString();
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var CourseBatchS = (from cs in db.COURSEs
                                join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                join emp in db.EMPLOYEEs on bt.EMP_ID equals emp.ID into gemp
                                from subgemp in gemp.DefaultIfEmpty()
                                where cs.IS_DEL == false && bt.IS_DEL == false
                                orderby cs.CODE
                                select new CoursesBatch { CourseData = cs, BatchData = bt, EmployeeData = (subgemp == null ? null : subgemp) }).Distinct();

            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("ALL"))
            {
                if(searchString == "Active")
                {
                    CourseBatchS = CourseBatchS.Where(s => s.BatchData.IS_ACT == true);
                }
                else if (searchString == "Inactive")
                {
                    CourseBatchS = CourseBatchS.Where(s => s.BatchData.IS_ACT == false);
                }
            }
            switch (sortOrder)
            {
                case "name_desc":
                    CourseBatchS = CourseBatchS.OrderByDescending(s => s.BatchData.NAME);
                    break;
                case "Date":
                    CourseBatchS = CourseBatchS.OrderBy(s => s.BatchData.START_DATE);
                    break;
                case "date_desc":
                    CourseBatchS = CourseBatchS.OrderByDescending(s => s.BatchData.START_DATE);
                    break;
                default:  // Name ascending 
                    CourseBatchS = CourseBatchS.OrderBy(s => s.BatchData.NAME);
                    break;
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);
            return View(CourseBatchS.ToPagedList(pageNumber, pageSize));
        }

        // GET: BATCH/Create
        public ActionResult Create()
        {
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME");

            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in db.EMPLOYEEs.Where(x=>x.STAT == true).ToList())
            {
                string EmpFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME, " (", item.EMP_NUM, ")");
                var result = new SelectListItem();
                result.Text = EmpFullName;
                result.Value = item.ID.ToString();
                //result.Selected = item.ID == bATCH.EMP_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Employee" });
            ViewBag.EMP_ID = options;
            return View();
        }

        // POST: BATCH/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NAME,CRS_ID,START_DATE,END_DATE,IS_DEL,IS_ACT,EMP_ID,GRADING_TYPE")] BATCH bATCH)
        {
            if (ModelState.IsValid)
            {
                bATCH.IS_DEL = false;
                bATCH.IS_ACT = true;
                db.BATCHes.Add(bATCH);
                db.SaveChanges();
                ViewBag.Notice = "New Batch created successfully.";
                return RedirectToAction("ManageBatches", new { Notice = ViewBag.Notice});
            }

            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", bATCH.CRS_ID);
            return View(bATCH);
        }

        // GET: BATCH/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BATCH bATCH = db.BATCHes.Include(x=>x.COURSE).Where(x=>x.ID == id).FirstOrDefault();
            if (bATCH == null)
            {
                return HttpNotFound();
            }
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", bATCH.CRS_ID);
            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in db.EMPLOYEEs.Where(x => x.STAT == true).ToList())
            {
                string EmpFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME, " (", item.EMP_NUM, ")");
                var result = new SelectListItem();
                result.Text = EmpFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == bATCH.EMP_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Employee" });
            ViewBag.EMP_ID = options;

            return View(bATCH);
        }

        // POST: BATCH/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NAME,CRS_ID,START_DATE,END_DATE,IS_DEL,EMP_ID,GRADING_TYPE")] BATCH bATCH)
        {
            if (ModelState.IsValid)
            {
                BATCH BatchToUpdate = db.BATCHes.Find(bATCH.ID);
                BatchToUpdate.NAME = bATCH.NAME;
                BatchToUpdate.CRS_ID = bATCH.CRS_ID;
                BatchToUpdate.START_DATE = bATCH.START_DATE;
                BatchToUpdate.END_DATE = bATCH.END_DATE;
                BatchToUpdate.EMP_ID = bATCH.EMP_ID;
                BatchToUpdate.GRADING_TYPE = bATCH.GRADING_TYPE;
                db.Entry(BatchToUpdate).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    ViewBag.Notice = string.Concat(ViewBag.Notice, "Batch updated successfully.");
                    return RedirectToAction("ManageBatches", new { Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Error Occured. ", e.InnerException.InnerException.Message);
                    return RedirectToAction("ManageBatches", new { ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            ViewBag.Notice = "There seems to be some issue with Model State";
            ViewBag.CRS_ID = new SelectList(db.COURSEs, "ID", "CRS_NAME", bATCH.CRS_ID);
            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in db.EMPLOYEEs.Where(x => x.STAT == true).ToList())
            {
                string EmpFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME, " (", item.EMP_NUM, ")");
                var result = new SelectListItem();
                result.Text = EmpFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == bATCH.EMP_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Employee" });
            return View(bATCH);
        }

        // GET: BATCH/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BATCH bATCH = db.BATCHes.Find(id);
            if (bATCH == null)
            {
                return HttpNotFound();
            }
            //db.BATCHes.Remove(bATCH);
            bATCH.IS_DEL = true;
            bATCH.IS_ACT = false;
            db.Entry(bATCH).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                ViewBag.Notice = string.Concat(ViewBag.Notice, "Batch is deleted successfully");
                return RedirectToAction("ManageBatches", new { Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Error Occured. ", e.InnerException.InnerException.Message);
                return RedirectToAction("ManageBatches", new { ErrorMessage = ViewBag.ErrorMessage });
            }
        }

        public ActionResult Activate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BATCH bATCH = db.BATCHes.Find(id);
            if (bATCH == null)
            {
                return HttpNotFound();
            }
            bATCH.IS_ACT = true;
            db.Entry(bATCH).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                ViewBag.Notice = string.Concat(ViewBag.Notice, "Batch is activated successfully");
                return RedirectToAction("ManageBatches", new { Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Error Occured. ", e.InnerException.InnerException.Message);
                return RedirectToAction("ManageBatches", new { ErrorMessage = ViewBag.ErrorMessage });
            }
        }

        public ActionResult Deactivate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BATCH bATCH = db.BATCHes.Find(id);
            if (bATCH == null)
            {
                return HttpNotFound();
            }
            bATCH.IS_ACT = false;
            db.Entry(bATCH).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                ViewBag.Notice = string.Concat(ViewBag.Notice, "Batch is Deactivated successfully");
                return RedirectToAction("ManageBatches", new { Notice = ViewBag.Notice });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Error Occured. ", e.InnerException.InnerException.Message);
                return RedirectToAction("ManageBatches", new { ErrorMessage = ViewBag.ErrorMessage });
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
