﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Text;
using System.Data.Entity.Validation;

namespace SFSAcademy.Controllers
{
    public class FinanceController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Finance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fees_Index()
        {
            return View();
        }

        public ActionResult Master_Fees()
        {
            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == true).OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.searchString = options;
            return View();
        }

        public ActionResult _Master_Fee_List(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null && !searchString.Equals("-1"))
            {
                page = 1;
                ///As Drop down list sends Id, we will ahve to convert this to text which is different from text box
                int searchStringId = Convert.ToInt32(searchString);
                searchString = db.FINANCE_FEE_CATGEORY.Find(searchStringId).NAME.ToString();
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var FeeCategoryS = (from ffc in db.FINANCE_FEE_CATGEORY
                                where ffc.IS_DEL.Equals(false) && ffc.IS_MSTR == true
                                orderby ffc.NAME
                                select new FeeCategory { FinanceFeeCategoryData = ffc }).Distinct();

            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("ALL"))
            {
                FeeCategoryS = FeeCategoryS.Where(s => s.FinanceFeeCategoryData.NAME.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    FeeCategoryS = FeeCategoryS.OrderByDescending(s => s.FinanceFeeCategoryData.NAME);
                    break;
                case "Date":
                    FeeCategoryS = FeeCategoryS.OrderBy(s => s.FinanceFeeCategoryData.CREATED_AT);
                    break;
                case "date_desc":
                    FeeCategoryS = FeeCategoryS.OrderByDescending(s => s.FinanceFeeCategoryData.CREATED_AT);
                    break;
                default:  // Name ascending 
                    FeeCategoryS = FeeCategoryS.OrderBy(s => s.FinanceFeeCategoryData.NAME);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(FeeCategoryS.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Master_Category_Create(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            ViewBag.NAME = "";
            ViewBag.DESCR = "";
            return View();
        }

        [HttpGet]
        public ActionResult _Master_Category_Create_Form()
        {
            var batches = db.BATCHes.FirstOrDefault().ACTIVE();

            List<SelectListItem> sFeeCategory = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == true).OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            sFeeCategory.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Master category" });
            ViewBag.MSTR_CATGRY_ID = sFeeCategory;

            return View(batches);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Master_Category_Create_Form(IEnumerable<BATCH> model, string radioName, string NAME, string DESCR, string FEE_FREQ, int? MSTR_CATGRY_ID)
        {
            if (radioName == "Yes")
            {
                var FF_cATAGORY = new FINANCE_FEE_CATGEORY() { NAME = NAME, DESCR = DESCR, FEE_FREQ = FEE_FREQ, BTCH_ID = null, IS_DEL = false, IS_MSTR = true, CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now, MSTR_CATGRY_ID = null };
                db.FINANCE_FEE_CATGEORY.Add(FF_cATAGORY);
            }
            else
            {
                foreach (var item in model.Where(x => x.Select == true))
                {
                    var FF_cATAGORY = new FINANCE_FEE_CATGEORY() { NAME = NAME, DESCR = DESCR, FEE_FREQ = FEE_FREQ, BTCH_ID = item.ID, IS_DEL = false, IS_MSTR = false, CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now, MSTR_CATGRY_ID = MSTR_CATGRY_ID };
                    db.FINANCE_FEE_CATGEORY.Add(FF_cATAGORY);
                }
            }
            try { db.SaveChanges(); ViewBag.Notice = "Fee Category Added Successfully."; }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message);
                return RedirectToAction("Master_Category_Create", new { ErrorMessage = ViewBag.ErrorMessage });
            }
            if (radioName == "No" && model.Where(x => x.Select == true).Count() == 0)
            {
                ViewBag.ErrorMessage = "Please select at least one Batch";
            }
            return RedirectToAction("Master_Category_Create", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Fee_Category_View()
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.searchString = options;
            return View();
        }

        public ActionResult _Fee_Category_List(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            int searchStringId = 0;

            if (searchString != null && !searchString.Equals("-1"))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (!string.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                ///As Drop down list sends Id, we will ahve to convert this to text which is different from text box
                searchStringId = Convert.ToInt32(searchString);
            }
            ViewBag.CurrentFilter = searchString;

            var FeeCategoryS = (from ffc in db.FINANCE_FEE_CATGEORY
                                join b in db.BATCHes on ffc.BTCH_ID equals b.ID
                                join cs in db.COURSEs on b.CRS_ID equals cs.ID
                                where ffc.IS_DEL.Equals(false) && ffc.IS_MSTR.Equals(false)
                                orderby ffc.NAME
                                select new FeeMasterCategory { FinanceFeeCategoryData = ffc, BatchData = b, CourseData = cs }).Distinct();



            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                FeeCategoryS = FeeCategoryS.Where(s => s.BatchData.ID.Equals(searchStringId));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    FeeCategoryS = FeeCategoryS.OrderByDescending(s => s.FinanceFeeCategoryData.NAME);
                    break;
                case "Date":
                    FeeCategoryS = FeeCategoryS.OrderBy(s => s.FinanceFeeCategoryData.CREATED_AT);
                    break;
                case "date_desc":
                    FeeCategoryS = FeeCategoryS.OrderByDescending(s => s.FinanceFeeCategoryData.CREATED_AT);
                    break;
                default:  // Name ascending 
                    FeeCategoryS = FeeCategoryS.OrderBy(s => s.FinanceFeeCategoryData.NAME);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(FeeCategoryS.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Master_Category_Particulars(int? id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            int searchStringId = 0;
            ViewBag.FeeCatId = id;
            if (searchString != null && !searchString.Equals("-1"))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (!string.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                ///As Drop down list sends Id, we will ahve to convert this to text which is different from text box
                searchStringId = Convert.ToInt32(searchString);
            }
            ViewBag.CurrentFilter = searchString;


            var FeeParticularS = (from fp in db.FINANCE_FEE_PARTICULAR
                                  join fc in db.FINANCE_FEE_CATGEORY on fp.FIN_FEE_CAT_ID equals fc.ID
                                  join sc in db.STUDENT_CATGEORY on fp.STDNT_CAT_ID equals sc.ID into gsc
                                  from subgsc in gsc.DefaultIfEmpty()
                                  join st in db.STUDENTs on fp.STDNT_ID equals st.ID into gst
                                  from subgst in gst.DefaultIfEmpty()
                                  where fp.IS_DEL == "N" && fp.FIN_FEE_CAT_ID == id
                                  select new FeeParticular { FeeParticularData = fp, FeeCategoryData = fc, StudentCategoryData = (subgsc == null ? null : subgsc), StudentData = (subgst == null ? null : subgst) })
                             .OrderBy(x => x.FeeCategoryData.ID).Distinct();


            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                FeeParticularS = FeeParticularS.Where(s => s.FeeParticularData.ID.Equals(searchStringId));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    FeeParticularS = FeeParticularS.OrderByDescending(s => s.FeeParticularData.NAME);
                    break;
                case "Date":
                    FeeParticularS = FeeParticularS.OrderBy(s => s.FeeParticularData.CREATED_AT);
                    break;
                case "date_desc":
                    FeeParticularS = FeeParticularS.OrderByDescending(s => s.FeeParticularData.CREATED_AT);
                    break;
                default:  // Name ascending 
                    FeeParticularS = FeeParticularS.OrderBy(s => s.FeeParticularData.NAME);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(FeeParticularS.ToPagedList(pageNumber, pageSize));
        }



        // GET: Finance/Delete/5
        public ActionResult Master_Category_Delete(int? id)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = string.Concat("It is a bad request");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE_CATGEORY fINANCE_FEE_CATEGORY = db.FINANCE_FEE_CATGEORY.Find(id);
            if (fINANCE_FEE_CATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(fINANCE_FEE_CATEGORY);
        }

        // POST: Finance/Delete/5
        [HttpPost, ActionName("Master_Category_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Master_Category_DeleteConfirmed(int id)
        {
            FINANCE_FEE_CATGEORY fINANCE_FEE_CATEGORY = db.FINANCE_FEE_CATGEORY.Find(id);
            var FeeCollection = db.FINANCE_FEE_COLLECTION.Where(x => x.FEE_CAT_ID == id).ToList();
            if(FeeCollection != null && FeeCollection.Count() !=0)
            {
                ViewBag.ErrorMessage = string.Concat("Finance Category canot be deleted as there are collections assigned to this Category.");
            }
            else
            {
                fINANCE_FEE_CATEGORY.IS_DEL = true;
                fINANCE_FEE_CATEGORY.UPDATED_AT = System.DateTime.Now;
                db.Entry(fINANCE_FEE_CATEGORY).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.ErrorMessage = string.Concat("Master Category Deleted Successfully");
            }
            return View();
        }

        // GET: Finance/Edit/5
        public ActionResult Master_Category_Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE_CATGEORY fINANCE_FEE_CATEGORY = db.FINANCE_FEE_CATGEORY.Find(id);
            if (fINANCE_FEE_CATEGORY == null)
            {
                return HttpNotFound();
            }

            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                    .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID == fINANCE_FEE_CATEGORY.BTCH_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.BTCH_ID = options;

            return View(fINANCE_FEE_CATEGORY);
        }

        // POST: Finance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Master_Category_Edit([Bind(Include = "ID,NAME,DESCR,FEE_FREQ,BTCH_ID,IS_DEL,IS_MSTR,CREATED_AT,UPDATED_AT")] FINANCE_FEE_CATGEORY fINANCE_FEE_CATEGORY)
        {
            if (ModelState.IsValid)
            {
                FINANCE_FEE_CATGEORY fINANCE_FEE_CATEGORY_UPD = db.FINANCE_FEE_CATGEORY.Find(fINANCE_FEE_CATEGORY.ID);
                if (fINANCE_FEE_CATEGORY.BTCH_ID.Equals(-1))
                {
                    fINANCE_FEE_CATEGORY_UPD.BTCH_ID = null;
                    fINANCE_FEE_CATEGORY_UPD.UPDATED_AT = System.DateTime.Now;
                    fINANCE_FEE_CATEGORY_UPD.NAME = fINANCE_FEE_CATEGORY.NAME;
                    fINANCE_FEE_CATEGORY_UPD.DESCR = fINANCE_FEE_CATEGORY.DESCR;
                    fINANCE_FEE_CATEGORY_UPD.FEE_FREQ = fINANCE_FEE_CATEGORY.FEE_FREQ;
                    fINANCE_FEE_CATEGORY_UPD.BTCH_ID = fINANCE_FEE_CATEGORY.BTCH_ID;
                    db.Entry(fINANCE_FEE_CATEGORY_UPD).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    fINANCE_FEE_CATEGORY_UPD.UPDATED_AT = System.DateTime.Now;
                    fINANCE_FEE_CATEGORY_UPD.NAME = fINANCE_FEE_CATEGORY.NAME;
                    fINANCE_FEE_CATEGORY_UPD.DESCR = fINANCE_FEE_CATEGORY.DESCR;
                    fINANCE_FEE_CATEGORY_UPD.FEE_FREQ = fINANCE_FEE_CATEGORY.FEE_FREQ;
                    fINANCE_FEE_CATEGORY_UPD.BTCH_ID = fINANCE_FEE_CATEGORY.BTCH_ID;
                    db.Entry(fINANCE_FEE_CATEGORY_UPD).State = EntityState.Modified;
                    db.SaveChanges();
                }
                ViewBag.ErrorMessage = string.Concat("Master Category Updated Successfully");
            }

            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                    .OrderBy(x => x.BatchData.ID).ToList();
            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID == fINANCE_FEE_CATEGORY.BTCH_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.BTCH_ID = options;
            return View(fINANCE_FEE_CATEGORY);
        }


        [HttpGet]
        public ActionResult Fees_Particulars_New(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == true && x.IS_DEL == false).OrderBy(x => x.NAME).Distinct(), "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Master Category" });
            ViewBag.MASTER_CATGEORY_ID = options;
            return View();
        }

        [HttpGet]
        public ActionResult Select_Batch_Particular(int? id)
        {
            var FinanceFeeCategory = db.FINANCE_FEE_CATGEORY.Include(x => x.BATCH).Include(x => x.BATCH.COURSE).Where(x => x.IS_DEL == false && x.BATCH.IS_DEL == false && x.IS_MSTR == false && x.MSTR_CATGRY_ID == id && x.BATCH.IS_ACT == true).OrderBy(x => x.ID).ToList();

            return PartialView("_Select_Batch_Particular", FinanceFeeCategory);
        }

        [HttpGet]
        public ActionResult _Fees_Particulars_Create()
        {
            List<SelectListItem> sCategory = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.ID), "ID", "NAME").ToList();
            ViewBag.STDNT_CAT_ID = sCategory;
            ViewBag.radioName = "";
            //ViewBag.FeePartOption = "";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Fees_Particulars_Create([Bind(Include = "ID,NAME,DESCR,AMT,FIN_FEE_CAT_ID,STDNT_CAT_ID,ADMSN_NO,STDNT_ID,IS_DEL,CREATED_AT,UPDATED_AT")] FINANCE_FEE_PARTICULAR fINANCE_FEE_pARTUCULAR, IEnumerable<FINANCE_FEE_CATGEORY> model, string radioName)
        {
            if (radioName == "Admission No.")
            {
                int StdUpdated = 0;
                foreach (var AdmissionNoList in HtmlHelpers.ApplicationHelper.StringToIntList(fINANCE_FEE_pARTUCULAR.ADMSN_NO).ToList())
                {
                    int stCount = 0;
                    foreach (var item in model.Where(x=>x.Select == true))
                    {
                        fINANCE_FEE_pARTUCULAR.FIN_FEE_CAT_ID = item.ID;
                        var StdFinResult = from u in db.STUDENTs
                                           where (u.ADMSN_NO == AdmissionNoList.ToString()
                                            && u.BTCH_ID == item.BTCH_ID)
                                           select u;
                        stCount = StdFinResult.Count();

                        if (StdFinResult.Count() != 0)
                        {
                            StdUpdated = StdUpdated + 1;
                            fINANCE_FEE_pARTUCULAR.STDNT_CAT_ID = null;
                            fINANCE_FEE_pARTUCULAR.ADMSN_NO = AdmissionNoList.ToString();
                            fINANCE_FEE_pARTUCULAR.STDNT_ID = StdFinResult.FirstOrDefault().ID;
                            fINANCE_FEE_pARTUCULAR.IS_DEL = "N";
                            db.FINANCE_FEE_PARTICULAR.Add(fINANCE_FEE_pARTUCULAR);
                            try{db.SaveChanges();}
                            catch (Exception e){
                                ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message);
                                return RedirectToAction("Fees_Particulars_New", new { ErrorMessage = ViewBag.ErrorMessage });
                            }
                        }
                        if (!stCount.Equals(0)) { break; }
                    }
                    if (stCount.Equals(0)) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "Admission Number: ", AdmissionNoList, " is either invalid or does not belong to batch selected. "); }
                }
                if (StdUpdated == 0)
                {
                    ViewBag.ErrorMessage = "No student found with given Admission Numbers or they are not from the Batch selected.";
                }
                else
                {
                    ViewBag.Notice = string.Concat(ViewBag.Notice, " Fee Particular Details of ", StdUpdated, " Students updated in system successfully");
                }
            }
            else if (radioName == "Student Category")
            {
                int StdCatUpdated = 0;
                foreach (var item in model.Where(x=>x.Select == true))
                {
                    StdCatUpdated++;
                    fINANCE_FEE_pARTUCULAR.FIN_FEE_CAT_ID = item.ID;
                    fINANCE_FEE_pARTUCULAR.IS_DEL = "N";
                    fINANCE_FEE_pARTUCULAR.CREATED_AT = System.DateTime.Now;
                    fINANCE_FEE_pARTUCULAR.UPDATED_AT = System.DateTime.Now;
                    db.FINANCE_FEE_PARTICULAR.Add(fINANCE_FEE_pARTUCULAR);
                    try { db.SaveChanges(); }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message);
                        return RedirectToAction("Fees_Particulars_New", new { ErrorMessage = ViewBag.ErrorMessage });
                    }
                }
                if (!StdCatUpdated.Equals(0)) { ViewBag.Notice = string.Concat("Fee Particular Details of ", StdCatUpdated, " Student Category updated in system successfully"); }
                else { ViewBag.ErrorMessage = string.Concat("No valid category selected"); }
            }
            else
            {
                int StdFeeUpdated = 0;
                foreach (var item in model.Where(x=>x.Select == true))
                {
                    StdFeeUpdated++;
                    fINANCE_FEE_pARTUCULAR.STDNT_CAT_ID = null;
                    fINANCE_FEE_pARTUCULAR.FIN_FEE_CAT_ID = item.ID;
                    fINANCE_FEE_pARTUCULAR.IS_DEL = "N";
                    fINANCE_FEE_pARTUCULAR.CREATED_AT = System.DateTime.Now;
                    fINANCE_FEE_pARTUCULAR.UPDATED_AT = System.DateTime.Now;
                    db.FINANCE_FEE_PARTICULAR.Add(fINANCE_FEE_pARTUCULAR);
                    try { db.SaveChanges(); }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message);
                        return RedirectToAction("Fees_Particulars_New", new { ErrorMessage = ViewBag.ErrorMessage });
                    }
                }
                if (!StdFeeUpdated.Equals(0)) { ViewBag.Notice = string.Concat("Fee Particular Details of ", StdFeeUpdated, " Student Category updated in system successfully"); }
                else { ViewBag.ErrorMessage = string.Concat("No valid category selected"); }
            }
            return RedirectToAction("Fees_Particulars_New",new {ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });

        }
        // GET: Finance/Delete/5
        public ActionResult Master_Category_Particulars_Delete(int? id, int? FeeCatId)
        {
            if (id == null)
            {
                ViewBag.ErrorMessage = string.Concat("It is a bad request");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE_PARTICULAR fINANCE_FEE_PARTICULAR = db.FINANCE_FEE_PARTICULAR.Find(id);
            if (fINANCE_FEE_PARTICULAR == null)
            {
                return HttpNotFound();
            }
            fINANCE_FEE_PARTICULAR.IS_DEL = "Y";
            fINANCE_FEE_PARTICULAR.UPDATED_AT = System.DateTime.Now;
            db.SaveChanges();
            Session["FeePartDelMessage"] = string.Concat("Master Category Particular Deleted Successfully");
            return RedirectToAction("master_category_particulars", new { id = FeeCatId });
        }

        // GET: Finance/Edit/5
        public ActionResult Master_Category_Particulars_Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE_PARTICULAR fINANCE_FEE_PARTICULAR = db.FINANCE_FEE_PARTICULAR.Find(id);
            if (fINANCE_FEE_PARTICULAR == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == true).OrderBy(x => x.NAME).Distinct(), "ID", "NAME", fINANCE_FEE_PARTICULAR.FIN_FEE_CAT_ID).ToList();
            ViewBag.FIN_FEE_CAT_ID = options;

            List<SelectListItem> sCategory = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.ID), "ID", "NAME", fINANCE_FEE_PARTICULAR.STDNT_CAT_ID).ToList();
            // add the 'ALL' option
            //sCategory.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.STDNT_CAT_ID = sCategory;
            if (fINANCE_FEE_PARTICULAR.STDNT_ID != null)
            { ViewBag.radioName = "Admission No."; }
            else if (fINANCE_FEE_PARTICULAR.STDNT_CAT_ID != null)
            { ViewBag.radioName = "Student Category"; }
            else
            { ViewBag.radioName = "All"; }

            //ViewBag.FeePartOption = "";
            //ViewBag.FeeCatError = TempData["FeeCatError"];

            return View(fINANCE_FEE_PARTICULAR);
        }

        // POST: Finance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Master_Category_Particulars_Edit([Bind(Include = "ID,NAME,DESCR,AMT,FIN_FEE_CAT_ID,STDNT_CAT_ID,ADMSN_NO,STDNT_ID,IS_DEL,CREATED_AT,UPDATED_AT")] FINANCE_FEE_PARTICULAR fINANCE_FEE_pARTUCULAR, string radioName)
        {
            int StdFeeUpdated = 0;
            FINANCE_FEE_CATGEORY fINANCE_FEE_CATGEORY = db.FINANCE_FEE_CATGEORY.Find(fINANCE_FEE_pARTUCULAR.FIN_FEE_CAT_ID);
            FINANCE_FEE_PARTICULAR fINANCE_FEE_PARTICULAR_UPD = db.FINANCE_FEE_PARTICULAR.Find(fINANCE_FEE_pARTUCULAR.ID);
            if (ModelState.IsValid)
            {
                fINANCE_FEE_PARTICULAR_UPD.NAME = fINANCE_FEE_pARTUCULAR.NAME;
                fINANCE_FEE_PARTICULAR_UPD.DESCR = fINANCE_FEE_pARTUCULAR.DESCR;
                fINANCE_FEE_PARTICULAR_UPD.AMT = fINANCE_FEE_pARTUCULAR.AMT;
                fINANCE_FEE_PARTICULAR_UPD.UPDATED_AT = System.DateTime.Now;
                db.Entry(fINANCE_FEE_PARTICULAR_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); StdFeeUpdated++; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.FeePartEditMessage = e; }
                if (!StdFeeUpdated.Equals(0))
                { ViewBag.FeePartEditMessage = string.Concat("Fee Particular Details of ", StdFeeUpdated, " Student Category updated in system successfully"); }
                else { ViewBag.FeePartEditMessage = string.Concat("Update was not successfull!"); }
            }

            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == true).OrderBy(x => x.NAME).Distinct(), "ID", "NAME", fINANCE_FEE_PARTICULAR_UPD.FIN_FEE_CAT_ID).ToList();
            ViewBag.FIN_FEE_CAT_ID = options;

            List<SelectListItem> sCategory = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.ID), "ID", "NAME", fINANCE_FEE_PARTICULAR_UPD.STDNT_CAT_ID).ToList();
            ViewBag.STDNT_CAT_ID = sCategory;
            if (fINANCE_FEE_PARTICULAR_UPD.STDNT_ID != null)
            { ViewBag.radioName = "Admission No."; }
            else if (fINANCE_FEE_PARTICULAR_UPD.STDNT_CAT_ID != null)
            { ViewBag.radioName = "Student Category"; }
            else
            { ViewBag.radioName = "All"; }

            //ViewBag.FeePartOption = "";
            return View(fINANCE_FEE_PARTICULAR_UPD);
        }


        [HttpGet]
        public ActionResult Fee_Discounts(string sortOrder, string currentFilter, string BTCH_ID, int? page, string currentFilter2, string FINANCE_FEE_CATGEORY_ID)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.IS_DEL == false && bt.IS_ACT == true
                                    select new { CourseData = cs, BatchData = bt })
                                    .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID.ToString() == BTCH_ID ? true : false;
                options.Add(result);
            }
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.BTCH_ID = options;

            var queryFeeCategoryBatch = (from fc in db.FINANCE_FEE_CATGEORY
                                         join bt in db.BATCHes.Include(x => x.COURSE) on fc.BTCH_ID equals bt.ID
                                         where fc.IS_DEL == false && bt.IS_DEL == false && fc.IS_MSTR == false && bt.IS_ACT == true
                                         select new { FeeCategoryData = fc, BatchData = bt })
                                    .OrderBy(x => x.FeeCategoryData.NAME).ToList();


            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in queryFeeCategoryBatch)
            {
                string FeeCategoryFullName = string.Concat(item.FeeCategoryData.NAME, "-", item.BatchData.COURSE.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = FeeCategoryFullName;
                result.Value = item.FeeCategoryData.ID.ToString();
                result.Selected = item.FeeCategoryData.ID.ToString() == FINANCE_FEE_CATGEORY_ID ? true : false;
                options2.Add(result);
            }
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.FINANCE_FEE_CATGEORY_ID = options2;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            int BatchIdVal = 0;
            if (BTCH_ID != null && !BTCH_ID.Equals("-1"))
            {
                page = 1;
            }
            else
            {
                BTCH_ID = currentFilter;
            }
            if (!string.IsNullOrEmpty(BTCH_ID) && !BTCH_ID.Equals("-1"))
            {
                BatchIdVal = Convert.ToInt32(BTCH_ID);
            }
            ViewBag.CurrentFilter = BTCH_ID;

            int FeeCategoryVal = 0;
            if (FINANCE_FEE_CATGEORY_ID != null && !FINANCE_FEE_CATGEORY_ID.Equals("-1"))
            {
                page = 1;
            }
            else
            {
                FINANCE_FEE_CATGEORY_ID = currentFilter2;
            }
            if (!string.IsNullOrEmpty(FINANCE_FEE_CATGEORY_ID) && !FINANCE_FEE_CATGEORY_ID.Equals("-1"))
            {
                FeeCategoryVal = Convert.ToInt32(FINANCE_FEE_CATGEORY_ID);
            }
            ViewBag.CurrentFilter2 = FINANCE_FEE_CATGEORY_ID;

            var Fee_discountSData = (from fd in db.FEE_DISCOUNT
                                     join ffc in db.FINANCE_FEE_CATGEORY on fd.FIN_FEE_CAT_ID equals ffc.ID
                                     join bt in db.BATCHes on ffc.BTCH_ID equals bt.ID
                                     join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                     join ffcol in db.FINANCE_FEE_COLLECTION on fd.FEE_CLCT_ID equals ffcol.ID into gffcol
                                     from subgffcol in gffcol.DefaultIfEmpty()
                                     join fds in (from crt in db.FEE_DISCOUNT where crt.TYPE == "Student" select crt) on fd.ID equals fds.ID into gms
                                     from subgms in gms.DefaultIfEmpty()
                                     join std in db.STUDENTs on subgms.RCVR_ID equals std.ID into gm  
                                     from substd in gm.DefaultIfEmpty()
                                     join fdsc in (from crtc in db.FEE_DISCOUNT where crtc.TYPE == "Student Category" select crtc) on fd.ID equals fdsc.ID into gmsc
                                     from subgmsc in gmsc.DefaultIfEmpty()
                                     join cat in db.STUDENT_CATGEORY on subgmsc.RCVR_ID equals cat.ID into gl 
                                     from subcat in gl.DefaultIfEmpty()
                                     orderby fd.NAME
                                     select new FeeDiscount { FeeDiscountData = fd, FinanceFeeCategoryData = ffc, BatchData = bt, CourseData = cs, StudentData = (substd == null ? null : substd), StudentCategoryData = (subcat == null ? null : subcat), FeeCollectionData = (subgffcol == null ? null : subgffcol) }).Distinct();

            if (!String.IsNullOrEmpty(BTCH_ID) && !BTCH_ID.Equals("-1"))
            {
                Fee_discountSData = Fee_discountSData.Where(s => s.BatchData.ID.Equals(BatchIdVal));
            }
            if (!String.IsNullOrEmpty(FINANCE_FEE_CATGEORY_ID) && !FINANCE_FEE_CATGEORY_ID.Equals("-1"))
            {
                Fee_discountSData = Fee_discountSData.Where(s => s.FinanceFeeCategoryData.ID.Equals(FeeCategoryVal));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    Fee_discountSData = Fee_discountSData.OrderByDescending(s => s.BatchData.ID);
                    break;
                case "Date":
                    Fee_discountSData = Fee_discountSData.OrderBy(s => s.FeeDiscountData.DISC_DATE);
                    break;
                case "date_desc":
                    Fee_discountSData = Fee_discountSData.OrderByDescending(s => s.FeeDiscountData.DISC_DATE);
                    break;
                default:  // Name ascending 
                    Fee_discountSData = Fee_discountSData.OrderBy(s => s.BatchData.ID);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(Fee_discountSData.ToPagedList(pageNumber, pageSize));
            //return View(db.USERS.ToList());
        }


        // GET: Finance/Create
        public ActionResult Fee_Discount_New()
        {

            var queryCourceBatchFee = (from cs in db.COURSEs
                                       join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                       join fcc in db.FINANCE_FEE_CATGEORY on bt.ID equals fcc.BTCH_ID
                                       where cs.IS_DEL == false && bt.IS_DEL == false && fcc.IS_DEL == false && bt.IS_ACT == true
                                       select new { CourseData = cs, BatchData = bt, FeeCategoryData = fcc })
                                    .OrderBy(x => x.FeeCategoryData.ID).ToList();

            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatchFee)
            {
                string FeeBatchFullName = string.Concat(item.FeeCategoryData.NAME, "-", item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = FeeBatchFullName;
                result.Value = item.FeeCategoryData.ID.ToString();
                options.Add(result);
            }

            //List<SelectListItem> options = new SelectList(db.BATCHes.OrderBy(x => x.ID).Distinct(), "NAME", "NAME", BTCH_ID).ToList();
            // add the 'ALL' option
            // options.Insert(0, new SelectListItem() { Value = null, Text = "Select a Course" });
            //ViewBag.BATCH_ID = options;

            //List<SelectListItem> options2 = new SelectList(db.FINANCE_FEE_CATGEORY.OrderBy(x => x.NAME).Where(x => x.IS_DEL=="N"), "NAME", "NAME").Distinct().ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Category" });
            ViewBag.FIN_FEE_CAT_ID = options;


            List<SelectListItem> options4 = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.NAME).Distinct(), "ID", "NAME").ToList();
            // add the 'ALL' option
            //options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Student Category" });
            ViewBag.STUDENT_CATGEORY_ID = options4;

            return View();
        }


        // POST: Finance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fee_Discount_New([Bind(Include = "ID,TYPE,NAME,RCVR_ID,FIN_FEE_CAT_ID,DISC,IS_AMT")] FEE_DISCOUNT fEEdISCOUNT, int? STUDENT_CATGEORY_ID, int? FIN_FEE_CAT_ID, string ADMSN_NO, string radioName, string radioName2)
        {
            if (ModelState.IsValid)
            {
                var queryCourceBatchFee = (from cs in db.COURSEs
                                           join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                           join fcc in db.FINANCE_FEE_CATGEORY on bt.ID equals fcc.BTCH_ID
                                           where cs.IS_DEL == false && bt.IS_DEL == false && fcc.IS_DEL == false && bt.IS_ACT == true
                                           select new { CourseData = cs, BatchData = bt, FeeCategoryData = fcc })
                                    .OrderBy(x => x.FeeCategoryData.ID).ToList();

                List<SelectListItem> options = new List<SelectListItem>();
                foreach (var item in queryCourceBatchFee)
                {
                    string FeeBatchFullName = string.Concat(item.FeeCategoryData.NAME, "-", item.CourseData.CODE, "-", item.BatchData.NAME);
                    var result = new SelectListItem();
                    result.Text = FeeBatchFullName;
                    result.Value = item.FeeCategoryData.ID.ToString();
                    result.Selected = item.FeeCategoryData.ID == FIN_FEE_CAT_ID ? true : false;
                    options.Add(result);
                }

                // add the 'ALL' option
                options.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Category" });
                ViewBag.FIN_FEE_CAT_ID = options;


                List<SelectListItem> options4 = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.NAME).Distinct(), "ID", "NAME", STUDENT_CATGEORY_ID).ToList();
                // add the 'ALL' option
                //options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Student Category" });
                ViewBag.STUDENT_CATGEORY_ID = options4;

                if (radioName == "Batch")
                {
                    var fEEcATEGORYaACCESS = (from ffc in db.FINANCE_FEE_CATGEORY
                                              where ffc.ID == FIN_FEE_CAT_ID
                                              select new { FinanceFeeCategoryData = ffc }).ToList();

                    fEEdISCOUNT.TYPE = radioName;
                    fEEdISCOUNT.RCVR_ID = fEEcATEGORYaACCESS.FirstOrDefault().FinanceFeeCategoryData.BTCH_ID;
                    fEEdISCOUNT.FIN_FEE_CAT_ID = fEEcATEGORYaACCESS.FirstOrDefault().FinanceFeeCategoryData.ID;
                    fEEdISCOUNT.DISC_DATE = System.DateTime.Now;
                    fEEdISCOUNT.DESCR = "Common Batch Discount";
                    if (radioName2.Equals("Amount"))
                    {
                        fEEdISCOUNT.IS_AMT = true;
                    }
                    else
                    {
                        fEEdISCOUNT.IS_AMT = false;
                    }
                    db.FEE_DISCOUNT.Add(fEEdISCOUNT);
                    try { db.SaveChanges(); }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                            }
                        }
                        return View();
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                        return View();
                    }
                    ViewBag.ErrorMessage = string.Concat("Fee Discounts of select batch added in system successfully");
                }
                else if (radioName == "Student Category")
                {
                    fEEdISCOUNT.RCVR_ID = STUDENT_CATGEORY_ID;
                    fEEdISCOUNT.TYPE = radioName;
                    fEEdISCOUNT.FIN_FEE_CAT_ID = FIN_FEE_CAT_ID;
                    fEEdISCOUNT.DISC_DATE = System.DateTime.Now;
                    fEEdISCOUNT.DESCR = "Common Student Category Discount";
                    if (radioName2.Equals("Amount"))
                    {
                        fEEdISCOUNT.IS_AMT = true;
                    }
                    else
                    {
                        fEEdISCOUNT.IS_AMT = false;
                    }
                    db.FEE_DISCOUNT.Add(fEEdISCOUNT);
                    try { db.SaveChanges(); }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                            }
                        }
                        return View();
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                        return View();
                    }
                    ViewBag.ErrorMessage = string.Concat("Fee Discounts of Student Category added in system successfully");
                }
                else if (radioName == "Student")
                {
                    if (HtmlHelpers.ApplicationHelper.StringToIntList(ADMSN_NO).Count() != 0)
                    {
                        int stdCount = 0;
                        foreach (var AdmissionNoList in HtmlHelpers.ApplicationHelper.StringToIntList(ADMSN_NO).ToList())
                        {
                            var StdResult = from u in db.STUDENTs where (u.ADMSN_NO == AdmissionNoList.ToString() && u.IS_DEL.Equals(false) && u.IS_ACT.Equals(true)) select u;
                            if (StdResult.Count() != 0)
                            {
                                stdCount++;
                                fEEdISCOUNT.RCVR_ID = StdResult.FirstOrDefault().ID;
                                var fEEcATEGORYaACCESS = (from ffc in db.FINANCE_FEE_CATGEORY
                                                          where ffc.ID == FIN_FEE_CAT_ID
                                                          select new { FinanceFeeCategoryData = ffc }).ToList();

                                fEEdISCOUNT.TYPE = radioName;
                                fEEdISCOUNT.FIN_FEE_CAT_ID = fEEcATEGORYaACCESS.FirstOrDefault().FinanceFeeCategoryData.ID;
                                fEEdISCOUNT.DISC_DATE = System.DateTime.Now;
                                fEEdISCOUNT.DESCR = "Special Student Discount";
                                if (radioName2.Equals("Amount"))
                                {
                                    fEEdISCOUNT.IS_AMT = true;
                                }
                                else
                                {
                                    fEEdISCOUNT.IS_AMT = false;
                                }
                                db.FEE_DISCOUNT.Add(fEEdISCOUNT);
                            }
                        }
                        try { db.SaveChanges(); }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                                }
                            }
                            return View();
                        }
                        catch (Exception e)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                            return View();
                        }
                        ViewBag.ErrorMessage = string.Concat("Fee Discounts of ", stdCount, " Students added in system successfully");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = string.Concat("Please select valid Discount Type");
                }

            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewBag.ErrorMessage = string.Concat("No proper value selected");
            }

            return View();
        }


        // GET: Finance/Edit/5
        public ActionResult Edit_Fee_Discount(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE_DISCOUNT fEE_DISCOUNT = db.FEE_DISCOUNT.Find(id);
            if (fEE_DISCOUNT == null)
            {
                return HttpNotFound();
            }
            ViewBag.RCVR_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO", fEE_DISCOUNT.RCVR_ID);
            ViewBag.FIN_FEE_CAT_ID = new SelectList(db.FINANCE_FEE_CATGEORY, "ID", "NAME", fEE_DISCOUNT.FIN_FEE_CAT_ID);
            return View(fEE_DISCOUNT);
        }

        // POST: Finance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Fee_Discount([Bind(Include = "ID,TYPE,NAME,RCVR_ID,FIN_FEE_CAT_ID,DISC,DISC_DATE,DESCR,IS_AMT")] FEE_DISCOUNT fEE_DISCOUNT)
        {
            if (ModelState.IsValid)
            {
                FEE_DISCOUNT fEE_DISCOUNT_UPD = db.FEE_DISCOUNT.Find(fEE_DISCOUNT.ID);
                fEE_DISCOUNT_UPD.TYPE = fEE_DISCOUNT.TYPE;
                fEE_DISCOUNT_UPD.NAME = fEE_DISCOUNT.NAME;
                fEE_DISCOUNT_UPD.DISC = fEE_DISCOUNT.DISC;
                fEE_DISCOUNT_UPD.IS_AMT = fEE_DISCOUNT.IS_AMT;
                fEE_DISCOUNT_UPD.DISC_DATE = fEE_DISCOUNT.DISC_DATE;
                fEE_DISCOUNT_UPD.DESCR = fEE_DISCOUNT.DESCR;
                db.Entry(fEE_DISCOUNT_UPD).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.ErrorMessage = string.Concat("Fee Discount Edited Successfully");
            ViewBag.RCVR_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO", fEE_DISCOUNT.RCVR_ID);
            ViewBag.FIN_FEE_CAT_ID = new SelectList(db.FINANCE_FEE_CATGEORY, "ID", "NAME", fEE_DISCOUNT.FIN_FEE_CAT_ID);
            return View(fEE_DISCOUNT);
        }


        // GET: Finance/Delete/5
        public ActionResult Delete_Fee_Discount(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE_DISCOUNT fEE_DISCOUNT = db.FEE_DISCOUNT.Find(id);
            if (fEE_DISCOUNT == null)
            {
                return HttpNotFound();
            }
            return View(fEE_DISCOUNT);
        }

        // POST: Finance/Delete/5
        [HttpPost, ActionName("Delete_Fee_Discount")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Fee_DiscountConfirmed(int id)
        {
            FEE_DISCOUNT fEE_DISCOUNT = db.FEE_DISCOUNT.Find(id);
            db.FEE_DISCOUNT.Remove(fEE_DISCOUNT);
            db.SaveChanges();
            ViewBag.ErrorMessage = string.Concat("Fee Discount Deleted Successfully");
            return View();
        }


        [HttpGet]
        public ActionResult Fee_Fine(string sortOrder, string currentFilter, string BTCH_ID, int? page, string currentFilter2, string FINANCE_FEE_CATGEORY_ID)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.IS_DEL == false && bt.IS_ACT == true
                                    select new { CourseData = cs, BatchData = bt })
                                    .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID.ToString() == BTCH_ID ? true : false;
                options.Add(result);
            }
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.BTCH_ID = options;

            var queryFeeCategoryBatch = (from fc in db.FINANCE_FEE_CATGEORY
                                    join bt in db.BATCHes.Include(x=>x.COURSE) on fc.BTCH_ID equals bt.ID
                                    where fc.IS_DEL == false && bt.IS_DEL == false &&  fc.IS_MSTR == false && bt.IS_ACT == true
                                    select new { FeeCategoryData = fc, BatchData = bt })
                                    .OrderBy(x => x.FeeCategoryData.NAME).ToList();


            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in queryFeeCategoryBatch)
            {
                string FeeCategoryFullName = string.Concat(item.FeeCategoryData.NAME, "-", item.BatchData.COURSE.CODE, "-",item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = FeeCategoryFullName;
                result.Value = item.FeeCategoryData.ID.ToString();
                result.Selected = item.FeeCategoryData.ID.ToString() == FINANCE_FEE_CATGEORY_ID ? true : false;
                options2.Add(result);
            }
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.FINANCE_FEE_CATGEORY_ID = options2;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            int BatchIdVal = 0;
            if (BTCH_ID != null && !BTCH_ID.Equals("-1"))
            {
                page = 1;
            }
            else
            {
                BTCH_ID = currentFilter;
            }
            if (!string.IsNullOrEmpty(BTCH_ID) && !BTCH_ID.Equals("-1"))
            {
                BatchIdVal = Convert.ToInt32(BTCH_ID);
            }
            ViewBag.CurrentFilter = BTCH_ID;

            int FeeCategoryVal = 0;
            if (FINANCE_FEE_CATGEORY_ID != null && !FINANCE_FEE_CATGEORY_ID.Equals("-1"))
            {
                page = 1;
            }
            else
            {
                FINANCE_FEE_CATGEORY_ID = currentFilter2;
            }
            if (!string.IsNullOrEmpty(FINANCE_FEE_CATGEORY_ID) && !FINANCE_FEE_CATGEORY_ID.Equals("-1"))
            {
                FeeCategoryVal = Convert.ToInt32(FINANCE_FEE_CATGEORY_ID);
            }
            ViewBag.CurrentFilter2 = FINANCE_FEE_CATGEORY_ID;


            var Fee_fineSData = (from fd in db.FEE_FINE
                                     join ffc in db.FINANCE_FEE_CATGEORY on fd.FIN_FEE_CAT_ID equals ffc.ID
                                     join bt in db.BATCHes on ffc.BTCH_ID equals bt.ID
                                     join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                     join fcol in db.FINANCE_FEE_COLLECTION on fd.FEE_CLCT_ID equals fcol.ID into gfcol
                                     from subgfcol in gfcol.DefaultIfEmpty()
                                     join fds in (from crt in db.FEE_FINE where crt.TYPE == "Student" select crt) on fd.ID equals fds.ID into gms
                                     from subgms in gms.DefaultIfEmpty()
                                     join std in db.STUDENTs on subgms.RCVR_ID equals std.ID into gm
                                     from substd in gm.DefaultIfEmpty()
                                     join fdsc in (from crtc in db.FEE_FINE where crtc.TYPE == "Student Category" select crtc) on fd.ID equals fdsc.ID into gmsc
                                     from subgmsc in gmsc.DefaultIfEmpty()
                                     join cat in db.STUDENT_CATGEORY on subgmsc.RCVR_ID equals cat.ID into gl
                                     from subcat in gl.DefaultIfEmpty()
                                     orderby fd.NAME
                                     select new FeeFine { FeeFineData = fd, FinanceFeeCategoryData = ffc, BatchData = bt, CourseData = cs, StudentData = (substd == null ? null : substd), StudentCategoryData = (subcat == null ? null : subcat), FeeCollectionData = (subgfcol == null ? null : subgfcol) }).OrderBy(x=>x.BatchData.ID).ThenBy(x=>x.FeeFineData.FINE_DATE).Distinct();


            if (!String.IsNullOrEmpty(BTCH_ID) && !BTCH_ID.Equals("-1"))
            {
                Fee_fineSData = Fee_fineSData.Where(s => s.BatchData.ID.Equals(BatchIdVal));
            }

            if (!String.IsNullOrEmpty(FINANCE_FEE_CATGEORY_ID) && !FINANCE_FEE_CATGEORY_ID.Equals("-1"))
            {
                Fee_fineSData = Fee_fineSData.Where(s => s.FinanceFeeCategoryData.ID.Equals(FeeCategoryVal));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    Fee_fineSData = Fee_fineSData.OrderByDescending(s => s.BatchData.ID);
                    break;
                case "Date":
                    Fee_fineSData = Fee_fineSData.OrderBy(s => s.FeeFineData.FINE_DATE);
                    break;
                case "date_desc":
                    Fee_fineSData = Fee_fineSData.OrderByDescending(s => s.FeeFineData.FINE_DATE);
                    break;
                default:  // Name ascending 
                    Fee_fineSData = Fee_fineSData.OrderBy(s => s.BatchData.ID);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(Fee_fineSData.ToPagedList(pageNumber, pageSize));
            //return View(db.USERS.ToList());
        }


        // GET: Finance/Create
        public ActionResult Fee_Fine_New()
        {
            var queryCourceBatchFee = (from cs in db.COURSEs
                                       join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                       join fcc in db.FINANCE_FEE_CATGEORY on bt.ID equals fcc.BTCH_ID
                                       where cs.IS_DEL == false && bt.IS_DEL == false && fcc.IS_DEL == false && bt.IS_ACT == true
                                       select new { CourseData = cs, BatchData = bt, FeeCategoryData = fcc })
                                    .OrderBy(x => x.FeeCategoryData.ID).ToList();

            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatchFee)
            {
                string FeeBatchFullName = string.Concat(item.FeeCategoryData.NAME, "-", item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = FeeBatchFullName;
                result.Value = item.FeeCategoryData.ID.ToString();
                options.Add(result);
            }
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Category" });
            ViewBag.FIN_FEE_CAT_ID = options;

            List<SelectListItem> options4 = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.NAME).Distinct(), "ID", "NAME").ToList();
            ViewBag.STUDENT_CATGEORY_ID = options4;

            return View();
        }


        // POST: Finance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fee_Fine_New([Bind(Include = "ID,TYPE,NAME,DESCR,RCVR_ID,FIN_FEE_CAT_ID,FINE,IS_AMT,FINE_DATE")] FEE_FINE fEEfINE, int? STUDENT_CATGEORY_ID, int? FIN_FEE_CAT_ID, string ADMSN_NO, string radioName, string radioName2)
        {
            if (ModelState.IsValid)
            {
                var queryCourceBatchFee = (from cs in db.COURSEs
                                           join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                           join fcc in db.FINANCE_FEE_CATGEORY on bt.ID equals fcc.BTCH_ID
                                           where cs.IS_DEL == false && bt.IS_DEL == false && fcc.IS_DEL == false && bt.IS_ACT == true
                                           select new { CourseData = cs, BatchData = bt, FeeCategoryData = fcc })
                                    .OrderBy(x => x.FeeCategoryData.ID).ToList();

                List<SelectListItem> options = new List<SelectListItem>();
                foreach (var item in queryCourceBatchFee)
                {
                    string FeeBatchFullName = string.Concat(item.FeeCategoryData.NAME, "-", item.CourseData.CODE, "-", item.BatchData.NAME);
                    var result = new SelectListItem();
                    result.Text = FeeBatchFullName;
                    result.Value = item.FeeCategoryData.ID.ToString();
                    result.Selected = item.FeeCategoryData.ID == FIN_FEE_CAT_ID ? true : false;
                    options.Add(result);
                }

                // add the 'ALL' option
                options.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Category" });
                ViewBag.FIN_FEE_CAT_ID = options;


                List<SelectListItem> options4 = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.NAME).Distinct(), "ID", "NAME", STUDENT_CATGEORY_ID).ToList();
                // add the 'ALL' option
                //options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Student Category" });
                ViewBag.STUDENT_CATGEORY_ID = options4;

                if (radioName == "Batch")
                {
                    var fEEcATEGORYaACCESS = (from ffc in db.FINANCE_FEE_CATGEORY
                                              where ffc.ID == FIN_FEE_CAT_ID
                                              select new { FinanceFeeCategoryData = ffc }).ToList();

                    fEEfINE.TYPE = radioName;
                    fEEfINE.RCVR_ID = fEEcATEGORYaACCESS.FirstOrDefault().FinanceFeeCategoryData.BTCH_ID;
                    fEEfINE.FIN_FEE_CAT_ID = fEEcATEGORYaACCESS.FirstOrDefault().FinanceFeeCategoryData.ID;
                    fEEfINE.FINE_DATE = System.DateTime.Now;
                    fEEfINE.DESCR = "Common Batch Fine";
                    if (radioName2.Equals("Amount"))
                    {
                        fEEfINE.IS_AMT = true;
                    }
                    else
                    {
                        fEEfINE.IS_AMT = false;
                    }
                    db.FEE_FINE.Add(fEEfINE);
                    try { db.SaveChanges(); }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                            }
                        }
                        return View();
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                        return View();
                    }
                    ViewBag.ErrorMessage = string.Concat("Fee Fine for select batch added in system successfully");
                }
                else if (radioName == "Student Category")
                {
                    fEEfINE.RCVR_ID = STUDENT_CATGEORY_ID;
                    fEEfINE.TYPE = radioName;
                    fEEfINE.FIN_FEE_CAT_ID = FIN_FEE_CAT_ID;
                    fEEfINE.FINE_DATE = System.DateTime.Now;
                    fEEfINE.DESCR = "Common Stundet Category Fine";
                    if (radioName2.Equals("Amount"))
                    {
                        fEEfINE.IS_AMT = true;
                    }
                    else
                    {
                        fEEfINE.IS_AMT = false;
                    }
                    db.FEE_FINE.Add(fEEfINE);
                    try { db.SaveChanges(); }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                            }
                        }
                        return View();
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                        return View();
                    }
                    ViewBag.ErrorMessage = string.Concat("Fee Fine for Student Category added in system successfully");
                }
                else if (radioName == "Student")
                {
                    if (HtmlHelpers.ApplicationHelper.StringToIntList(ADMSN_NO).Count() != 0)
                    {
                        int stdCount = 0;
                        foreach (var AdmissionNoList in HtmlHelpers.ApplicationHelper.StringToIntList(ADMSN_NO).ToList())
                        {
                            var StdResult = from u in db.STUDENTs where (u.ADMSN_NO == AdmissionNoList.ToString() && u.IS_DEL.Equals(false) && u.IS_ACT.Equals(true)) select u;
                            if (StdResult.Count() != 0)
                            {
                                stdCount++;
                                fEEfINE.RCVR_ID = StdResult.FirstOrDefault().ID;
                                var fEEcATEGORYaACCESS = (from ffc in db.FINANCE_FEE_CATGEORY
                                                          where ffc.ID == FIN_FEE_CAT_ID
                                                          select new { FinanceFeeCategoryData = ffc }).ToList();

                                fEEfINE.TYPE = radioName;
                                fEEfINE.FIN_FEE_CAT_ID = fEEcATEGORYaACCESS.FirstOrDefault().FinanceFeeCategoryData.ID;
                                fEEfINE.FINE_DATE = System.DateTime.Now;
                                fEEfINE.DESCR = "Specific Student Fine";
                                if (radioName2.Equals("Amount"))
                                {
                                    fEEfINE.IS_AMT = true;
                                }
                                else
                                {
                                    fEEfINE.IS_AMT = false;
                                }
                                db.FEE_FINE.Add(fEEfINE);
                            }
                        }
                        try { db.SaveChanges(); }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                                }
                            }
                            return View();
                        }
                        catch (Exception e)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                            return View();
                        }
                        ViewBag.ErrorMessage = string.Concat("Fee Fine for ", stdCount, " Students added in system successfully");
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = string.Concat("Please select valid Fine Type");
                }

            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewBag.ErrorMessage = string.Concat("No proper value selected");
            }

            return View();
        }


        // GET: Finance/Edit/5
        public ActionResult Edit_Fee_Fine(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE_FINE fEE_FINE = db.FEE_FINE.Find(id);
            if (fEE_FINE == null)
            {
                return HttpNotFound();
            }
            ViewBag.RCVR_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO", fEE_FINE.RCVR_ID);
            ViewBag.FIN_FEE_CAT_ID = new SelectList(db.FINANCE_FEE_CATGEORY, "ID", "NAME", fEE_FINE.FIN_FEE_CAT_ID);
            return View(fEE_FINE);
        }

        // POST: Finance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Fee_Fine([Bind(Include = "ID,TYPE,NAME,RCVR_ID,FIN_FEE_CAT_ID,FINE,FINE_DATE,DESCR,IS_AMT")] FEE_FINE fEE_FINE)
        {
            if (ModelState.IsValid)
            {
                FEE_FINE fEE_FINE_UPD = db.FEE_FINE.Find(fEE_FINE.ID);
                fEE_FINE_UPD.TYPE = fEE_FINE.TYPE;
                fEE_FINE_UPD.NAME = fEE_FINE.NAME;
                fEE_FINE_UPD.DESCR = fEE_FINE.DESCR;
                fEE_FINE_UPD.FINE = fEE_FINE.FINE;
                fEE_FINE_UPD.IS_AMT = fEE_FINE.IS_AMT;
                fEE_FINE_UPD.FINE_DATE = fEE_FINE.FINE_DATE;
                fEE_FINE_UPD.DESCR = fEE_FINE.DESCR;
                db.Entry(fEE_FINE_UPD).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.ErrorMessage = string.Concat("Fee Fine Edited Successfully");
            ViewBag.RCVR_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO", fEE_FINE.RCVR_ID);
            ViewBag.FIN_FEE_CAT_ID = new SelectList(db.FINANCE_FEE_CATGEORY, "ID", "NAME", fEE_FINE.FIN_FEE_CAT_ID);
            return View(fEE_FINE);
        }


        // GET: Finance/Delete/5
        public ActionResult Delete_Fee_Fine(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE_FINE fEE_FINE = db.FEE_FINE.Find(id);
            if (fEE_FINE == null)
            {
                return HttpNotFound();
            }
            return View(fEE_FINE);
        }

        // POST: Finance/Delete/5
        [HttpPost, ActionName("Delete_Fee_Fine")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete_Fee_FineConfirmed(int id)
        {
            FEE_FINE fEE_FINE = db.FEE_FINE.Find(id);
            db.FEE_FINE.Remove(fEE_FINE);
            db.SaveChanges();
            ViewBag.ErrorMessage = string.Concat("Fee Fine Deleted Successfully");
            return View();
        }


        // GET: Fee Index
        public ActionResult Fee_Collection()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Fee_Collection_New(string Notice, string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == true && x.IS_DEL == false).OrderBy(x => x.NAME).Distinct(), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Master Category" });
            ViewBag.MASTER_CATGEORY_ID = options;

            return View();
        }

        [HttpGet]
        public ActionResult Select_Batch_Collection(int? id)
        {
            var ParticularData = db.FINANCE_FEE_PARTICULAR.Where(x => x.IS_DEL == "N").Select(x=>x.FIN_FEE_CAT_ID).Distinct().ToList();
            var FinanceFeeCategory = db.FINANCE_FEE_CATGEORY.Include(x=>x.FINANCE_FEE_PARTICULAR).Include(x => x.BATCH).Include(x => x.BATCH.COURSE).Where(x => x.IS_DEL == false && x.BATCH.IS_DEL == false && x.IS_MSTR == false && x.MSTR_CATGRY_ID == id && x.BATCH.IS_ACT == true && ParticularData.Contains(x.ID)).OrderBy(x => x.ID).ToList();

            if (FinanceFeeCategory == null || FinanceFeeCategory.Count() == 0)
            {
                ViewBag.ErrorMessage = "No data to display. Either Fee Category is missing or no Particulars added.";
            }

            return PartialView("_Select_Batch_Collection", FinanceFeeCategory);
        }

        [HttpGet]
        public ActionResult _Fee_Collection_Create()
        {
            ViewBag.ReturnDate = System.DateTime.Now;
            ViewBag.ErrorMessage = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Fee_Collection_Create([Bind(Include = "ID,NAME,START_DATE,END_DATE,FEE_CAT_ID,BTCH_ID,IS_DEL,DUE_DATE")] FINANCE_FEE_COLLECTION fINANCE_FEE_cOLLECTION, IEnumerable<FINANCE_FEE_CATGEORY> model)
        {
            int FeeCatCount = 0;
            int FeeCatId = 0;
            int SelectFeeCatId = 0;
            foreach (var item in model.Where(x=>x.Select == true))
            {
                FeeCatCount++;
                SelectFeeCatId = item.ID;
                var FFeeColl = new FINANCE_FEE_COLLECTION() { NAME = fINANCE_FEE_cOLLECTION.NAME, START_DATE = fINANCE_FEE_cOLLECTION.START_DATE, END_DATE = fINANCE_FEE_cOLLECTION.END_DATE, DUE_DATE = fINANCE_FEE_cOLLECTION.DUE_DATE, FEE_CAT_ID = item.ID, BTCH_ID = item.BTCH_ID, IS_DEL = false };
                db.FINANCE_FEE_COLLECTION.Add(FFeeColl);
                try { db.SaveChanges(); }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message);
                    return RedirectToAction("Fee_Collection_New", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                //var StdResult = from u in db.STUDENTs where (u.BTCH_ID == item.BatchData.ID) select u;
                var StdResult = (from ffc in db.FINANCE_FEE_CATGEORY
                                 join b in db.BATCHes on ffc.BTCH_ID equals b.ID
                                 join st in db.STUDENTs on b.ID equals st.BTCH_ID
                                 join fcol in db.FINANCE_FEE_COLLECTION on new { A = ffc.ID.ToString(), B = b.ID.ToString() } equals new { A = fcol.FEE_CAT_ID.ToString(), B = fcol.BTCH_ID.ToString() }
                                 where fcol.ID == FFeeColl.ID && ffc.ID == item.ID && ffc.IS_DEL.Equals(false) && b.IS_DEL == false && st.IS_DEL == false && b.IS_ACT == true
                                 select new { FinanceFeeCategoryData = ffc, BatchData = b, StudentData = st, FeeCollectionData = fcol }).OrderBy(g => g.FinanceFeeCategoryData.ID).ToList();
                foreach (var item2 in StdResult)
                {
                    STUDENT FeeStudent = db.STUDENTs.Find(item2.StudentData.ID);
                    FeeStudent.HAS_PD_FE = false;
                    FeeStudent.UPDATED_AT = System.DateTime.Now;
                    db.Entry(FeeStudent).State = EntityState.Modified;
                    var FF_fEE = new FINANCE_FEE() { STDNT_ID = item2.StudentData.ID, FEE_CLCT_ID = item2.FeeCollectionData.ID, IS_PD = false };
                    db.FINANCE_FEE.Add(FF_fEE);
                }
                if (!FeeCatId.Equals(SelectFeeCatId))
                {
                    var FF_eVENT = new EVENT() { TTIL = "Fees Due", DESCR = fINANCE_FEE_cOLLECTION.NAME, START_DATE = fINANCE_FEE_cOLLECTION.START_DATE, END_DATE = fINANCE_FEE_cOLLECTION.END_DATE, IS_DUE = true, ORIGIN_ID = FFeeColl.ID, ORIGIN_TYPE = "Finance_Fee_Collection", CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now };
                    db.EVENTs.Add(FF_eVENT);
                    FeeCatId = SelectFeeCatId;
                }
                try { db.SaveChanges(); }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message);
                    return RedirectToAction("Fee_Collection_New", new { ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            if (FeeCatCount.Equals(0))
            {
                ViewBag.ErrorMessage = "Please select valid Fee Category";
            }
            else { ViewBag.Notice = string.Concat("Fee Collection for ", FeeCatCount, " Fee Categories added in system"); }
            return RedirectToAction("Fee_Collection_New",new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage});
        }


        public ActionResult Fee_Collection_View(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.searchString = options;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            int searchStringId = 0;

            if (searchString != null && !searchString.Equals("-1"))
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (!string.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                ///As Drop down list sends Id, we will ahve to convert this to text which is different from text box
                searchStringId = Convert.ToInt32(searchString);
            }

            ViewBag.CurrentFilter = searchString;

            var FeeCollectionS = (from fc in db.FINANCE_FEE_COLLECTION
                                  join b in db.BATCHes on fc.BTCH_ID equals b.ID
                                  join cs in db.COURSEs on b.CRS_ID equals cs.ID
                                  where fc.IS_DEL == false
                                  orderby fc.NAME
                                  select new FeeCollection { FinanceFeeCollectionData = fc, BatchData = b, CourseData = cs }).Distinct();


            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                FeeCollectionS = FeeCollectionS.Where(s => s.BatchData.ID.Equals(searchStringId));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    FeeCollectionS = FeeCollectionS.OrderByDescending(s => s.FinanceFeeCollectionData.NAME);
                    break;
                case "Date":
                    FeeCollectionS = FeeCollectionS.OrderBy(s => s.FinanceFeeCollectionData.DUE_DATE);
                    break;
                case "date_desc":
                    FeeCollectionS = FeeCollectionS.OrderByDescending(s => s.FinanceFeeCollectionData.DUE_DATE);
                    break;
                default:  // Name ascending 
                    FeeCollectionS = FeeCollectionS.OrderBy(s => s.FinanceFeeCollectionData.NAME);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(FeeCollectionS.ToPagedList(pageNumber, pageSize));
            //return View(db.USERS.ToList());
        }

        // GET: Finance/Edit/5
        public ActionResult Fee_Collection_Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE_COLLECTION fINANCE_FEE_cOLLECTION = db.FINANCE_FEE_COLLECTION.Find(id);
            if (fINANCE_FEE_cOLLECTION == null)
            {
                return HttpNotFound();
            }
            ViewBag.FEE_CAT_ID = new SelectList(db.FINANCE_FEE_CATGEORY, "ID", "NAME", fINANCE_FEE_cOLLECTION.FEE_CAT_ID);
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                         .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID == fINANCE_FEE_cOLLECTION.BTCH_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            //options.Insert(0, new SelectListItem() { Value = null, Text = "Select a Batch" });
            ViewBag.BTCH_ID = options;

            return View(fINANCE_FEE_cOLLECTION);
        }

        // POST: Finance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fee_Collection_Edit([Bind(Include = "ID,NAME,START_DATE,END_DATE,FEE_CAT_ID,BTCH_ID,IS_DEL,DUE_DATE")] FINANCE_FEE_COLLECTION fINANCE_FEE_cOLLECTION)
        {
            if (ModelState.IsValid)
            {
                FINANCE_FEE_COLLECTION fINANCE_FEE_cOLLECTION_UPD = db.FINANCE_FEE_COLLECTION.Find(fINANCE_FEE_cOLLECTION.ID);
                fINANCE_FEE_cOLLECTION_UPD.NAME = fINANCE_FEE_cOLLECTION.NAME;
                fINANCE_FEE_cOLLECTION_UPD.BTCH_ID = fINANCE_FEE_cOLLECTION.BTCH_ID;
                fINANCE_FEE_cOLLECTION_UPD.FEE_CAT_ID = fINANCE_FEE_cOLLECTION.FEE_CAT_ID;
                fINANCE_FEE_cOLLECTION_UPD.START_DATE = fINANCE_FEE_cOLLECTION.START_DATE;
                fINANCE_FEE_cOLLECTION_UPD.END_DATE = fINANCE_FEE_cOLLECTION.END_DATE;
                fINANCE_FEE_cOLLECTION_UPD.DUE_DATE = fINANCE_FEE_cOLLECTION.DUE_DATE;
                fINANCE_FEE_cOLLECTION_UPD.IS_DEL = fINANCE_FEE_cOLLECTION.IS_DEL;
                db.Entry(fINANCE_FEE_cOLLECTION_UPD).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.ErrorMessage = string.Concat("Fee Collection Edited Successfully");
            ViewBag.FEE_CAT_ID = new SelectList(db.FINANCE_FEE_CATGEORY, "ID", "NAME", fINANCE_FEE_cOLLECTION.FEE_CAT_ID);
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                         .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID == fINANCE_FEE_cOLLECTION.BTCH_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select a Batch" });
            ViewBag.BTCH_ID = options;
            return View(fINANCE_FEE_cOLLECTION);
        }

        // GET: Finance/Delete/5
        public ActionResult Fee_Collection_Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE_COLLECTION fINANCE_FEE_cOLLECTION = db.FINANCE_FEE_COLLECTION.Find(id);
            if (fINANCE_FEE_cOLLECTION == null)
            {
                return HttpNotFound();
            }
            return View(fINANCE_FEE_cOLLECTION);
        }

        // POST: Finance/Delete/5
        [HttpPost, ActionName("Fee_Collection_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Fee_Collection_DeleteConfirmed(int id)
        {
            FINANCE_FEE_COLLECTION fINANCE_FEE_cOLLECTION = db.FINANCE_FEE_COLLECTION.Find(id);
            var FinanceFee = (from ff in db.FINANCE_FEE
                              where ff.FEE_CLCT_ID == id
                              select ff).ToList();
            int IsAllPaid = 1;
            foreach(var item in FinanceFee)
            {
                if(item.IS_PD == false)
                {
                    IsAllPaid = 0;
                    break;
                }
            }
            if(IsAllPaid == 0)
            {
                ViewBag.ErrorMessage = string.Concat("There a pending fee submission by Students against this fee collection. This cannot be deleted at tis moment.");
                return View(fINANCE_FEE_cOLLECTION);
            }
            else
            {
                fINANCE_FEE_cOLLECTION.IS_DEL = true;
                db.Entry(fINANCE_FEE_cOLLECTION).State = EntityState.Modified;
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View(fINANCE_FEE_cOLLECTION);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(fINANCE_FEE_cOLLECTION);
                }

                ViewBag.ErrorMessage = string.Concat("The Fee Collection deleted successfully.");
                return View(fINANCE_FEE_cOLLECTION);
            }
                       
        }


        // GET: Fee Index
        public ActionResult Fees_Submission_Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Fees_Submission_Batch(string ErrorMessage)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL ==false && bt.IS_DEL == false
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
            Session["student_serach"] = null;
            ViewBag.FeeCollectionMessage = ErrorMessage;

            return View();

        }

        [HttpGet]
        public ActionResult _fees_collection_dates(int? id)
        {
            var queryCollections = (from cl in db.FINANCE_FEE_COLLECTION
                                    where cl.IS_DEL == false && cl.BTCH_ID == id
                                    select new { cl.ID, cl.NAME, cl.DUE_DATE })
                         .OrderBy(x => x.NAME).Distinct().ToList();


            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in queryCollections)
            {
                string CollectionDate = string.Concat(item.NAME, "-", Convert.ToDateTime(item.DUE_DATE).ToString("dd-MMM-yy"));
                var result = new SelectListItem();
                result.Text = CollectionDate;
                result.Value = item.ID.ToString();
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Collection Date" });
            ViewBag.COLLECTION_ID = options2;
            ViewBag.STUDENT_ID = null;

            return PartialView("_fees_collection_dates");

        }

        [HttpGet]
        // GET: Fee Index
        public ActionResult _Student_Fees_Submission(int? id, int? batch_id, int? student_id)
        {
            //ModelState.Clear();
            DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
            ViewBag.ReturnDate = PDate.ToShortDateString();
            FINANCE_FEE_COLLECTION date = db.FINANCE_FEE_COLLECTION.Find(id);
            ViewData["date"] = date;
            //FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(id);
            var fee_collection = (from st in db.STUDENTs
                                  join fc in db.FINANCE_FEE_COLLECTION on st.BTCH_ID equals fc.BTCH_ID
                                  join ffp in db.FINANCE_FEE_PARTICULAR on fc.FEE_CAT_ID equals ffp.FIN_FEE_CAT_ID
                                  where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false && st.IS_ACT == true && fc.ID == id && (ffp.STDNT_ID == st.ID || ffp.STDNT_ID == null) && (ffp.STDNT_CAT_ID == st.STDNT_CAT_ID || ffp.STDNT_CAT_ID == null)
                                  select new { Fee_Collectoin_data = fc }).OrderBy(x => x.Fee_Collectoin_data.ID).Distinct();

            STUDENT student = db.STUDENTs.Find(student_id == null ? -1 : student_id);
            //ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME");
            var batch_val = (from cs in db.COURSEs
                             join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                             where bt.ID == date.BTCH_ID
                             select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = batch_val;
            var fee = (from ff in db.FINANCE_FEE
                       join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                       where ff.FEE_CLCT_ID == id
                       select new { Finance_Fee_Data = ff, Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
            if (student != null)
            {
                if(student.IS_ACT == false)
                {
                    ViewBag.ErrorMessage = "This Admission Number does not exit, Or Student has left school. Please contact Admin Office.";
                    return PartialView("_Student_Fees_Submission");
                }
                else
                {
                    fee = fee.Where(x => x.Student_data.ID == student.ID);
                }
            }

            if (fee_collection != null && fee_collection.Count() !=0)
            {
                var StudentVal = (from st in db.STUDENTs
                                  join fc in db.FINANCE_FEE_COLLECTION on st.BTCH_ID equals fc.BTCH_ID
                                  join ffp in db.FINANCE_FEE_PARTICULAR on fc.FEE_CAT_ID equals ffp.FIN_FEE_CAT_ID
                                  where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false && st.IS_ACT == true && fc.ID == id && (ffp.STDNT_ID == st.ID || ffp.STDNT_ID == null) && (ffp.STDNT_CAT_ID == st.STDNT_CAT_ID || ffp.STDNT_CAT_ID == null)
                                  select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                int StudentValId = StudentVal.FirstOrDefault().Student_data.ID;
                
                if (student != null)
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == student.ID);
                    ViewData["student"] = db.STUDENTs.Where(x => x.ID == student.ID).FirstOrDefault();
                }
                else
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == StudentValId);
                    ViewData["student"] = db.STUDENTs.Where(x => x.ID == StudentValId).FirstOrDefault();
                }
                STUDENT_CATGEORY StudentCategoryVal = db.STUDENT_CATGEORY.Find(StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID);
                ViewBag.StudentCategory = StudentCategoryVal.NAME;
                var prev_student_val = (from st in db.STUDENTs
                                        join fc in db.FINANCE_FEE_COLLECTION on st.BTCH_ID equals fc.BTCH_ID
                                        join ffp in db.FINANCE_FEE_PARTICULAR on fc.FEE_CAT_ID equals ffp.FIN_FEE_CAT_ID
                                        where st.BTCH_ID == date.BTCH_ID && st.IS_DEL ==false && st.IS_ACT == true && fc.ID == id && (ffp.STDNT_ID == st.ID || ffp.STDNT_ID == null) && (ffp.STDNT_CAT_ID == st.STDNT_CAT_ID || ffp.STDNT_CAT_ID == null)
                                        select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                int prev_student_id = -1;
                if (StudentVal != null)
                {
                    prev_student_val = prev_student_val.Where(x => x.Student_data.ID < StudentVal.FirstOrDefault().Student_data.ID);
                    if (prev_student_val != null && prev_student_val.Count() != 0)
                    {
                        prev_student_id = prev_student_val.Max(x => x.Student_data.ID);
                    }
                }
                STUDENT prev_student = db.STUDENTs.Find(prev_student_id);
                ViewData["prev_student"] = prev_student;
                ViewBag.prev_student_id = prev_student_id;

                var next_student_val = (from st in db.STUDENTs
                                        join fc in db.FINANCE_FEE_COLLECTION on st.BTCH_ID equals fc.BTCH_ID
                                        join ffp in db.FINANCE_FEE_PARTICULAR on fc.FEE_CAT_ID equals ffp.FIN_FEE_CAT_ID
                                        where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false && st.IS_ACT == true && fc.ID == id && (ffp.STDNT_ID == st.ID || ffp.STDNT_ID == null) && (ffp.STDNT_CAT_ID == st.STDNT_CAT_ID || ffp.STDNT_CAT_ID == null)
                                        select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                int next_student_id = -1;
                if (StudentVal != null)
                {
                    next_student_val = next_student_val.Where(x => x.Student_data.ID > StudentVal.FirstOrDefault().Student_data.ID);
                    if (next_student_val != null && next_student_val.Count() != 0)
                    {
                        next_student_id = next_student_val.Min(x => x.Student_data.ID);
                    }
                }
                STUDENT next_student = db.STUDENTs.Find(next_student_id);
                ViewData["next_student"] = next_student;
                ViewBag.next_student_id = next_student_id;

                var financefeeVal = (from ff in db.FINANCE_FEE
                                     where ff.FEE_CLCT_ID == id && ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID
                                     select ff).ToList();

                ViewData["financefee"] = financefeeVal;
                ViewBag.due_date = date.DUE_DATE;

                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     where ff.FEE_CLCT_ID == id && st.ID == StudentVal.FirstOrDefault().Student_data.ID
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(date.FEE_CAT_ID);
                ViewData["fee_category"] = fee_category;

                var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                           where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.IS_DEL == "N" && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                           select ff).ToList();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date.DUE_DATE))
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date.DUE_DATE))
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date.DUE_DATE))
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;

                decimal total_discount_val = 0;
                decimal total_payable = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_payable = total_payable + (decimal)item.AMT;
                }
                ViewBag.total_payable = total_payable;
                if (batch_discounts_val != null && batch_discounts_val.Count() != 0)
                {
                    foreach (var item in batch_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (student_discounts_val != null && student_discounts_val.Count() != 0)
                {
                    foreach (var item in student_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (category_discounts_val != null && category_discounts_val.Count() != 0)
                {
                    foreach (var item in category_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                        select ff);
                ViewData["student_fine"] = student_fine_val;

                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                         select ff);
                ViewData["category_fine"] = category_fine_val;
                decimal total_fine_val = 0;
                if (batch_fine_val != null && batch_fine_val.Count() != 0)
                {
                    foreach(var item in batch_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                    
                }
                if (student_fine_val != null && student_fine_val.Count() != 0)
                {
                    foreach (var item in student_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }                   
                }
                if (category_fine_val != null && category_fine_val.Count() != 0)
                {
                    foreach (var item in category_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                ViewBag.fine = 0;
                ViewBag.total_fine = total_fine_val;

                if (total_discount_val > total_payable + total_fine_val && total_payable + total_fine_val >= 0)
                {
                    total_discount_val = total_payable + total_fine_val;
                }
                ViewBag.total_discount = total_discount_val;
                decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
                ViewBag.total_discount_percentage = total_discount_percentage_val;
            }
            else
            {
                ViewBag.ErrorMessage = "No students have been assigned this fee.";
            }

            //ModelState.Clear();

            //foreach (var key in TempData.Keys.ToList())
            //{
                //TempData.Remove(key);
            //}

            SFSAcademy.SubmitFeeDiscounts vModelDiscount = new SFSAcademy.SubmitFeeDiscounts();
            ViewData["FeeDiscountsAdd"] = vModelDiscount;

            SFSAcademy.SubmitFeeFine vModelFine = new SFSAcademy.SubmitFeeFine();
            ViewData["FeeFineAdd"] = vModelFine;

            SFSAcademy.SubmitFees vModel = new SFSAcademy.SubmitFees();
            this.PreventResubmit(vModel);// << Fill TempData & ViewModel PreventResubmit Property


            return PartialView("_Student_Fees_Submission", vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Fine_Ajax(SFSAcademy.SubmitFeeFine vModelFine)
        {
            int? studentID = vModelFine.StudentID;
            int? batch_id = vModelFine.Batch_id;
            int? date = vModelFine.Date;
            decimal? fee = vModelFine.Fine;
            string fee_desc = vModelFine.Fine_Desc;
            if (ModelState.IsValid)
            {
                DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
                ViewBag.ReturnDate = PDate.ToShortDateString();
                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == batch_id
                                 select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;
                FINANCE_FEE_COLLECTION date_val = db.FINANCE_FEE_COLLECTION.Find(date);
                ViewData["date"] = date_val;
                FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(date);

                var feeVal = (from ff in db.FINANCE_FEE
                              join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                              where ff.FEE_CLCT_ID == fee_collection.ID
                              select new StundentFee { FinanceFeeData = ff, StudentData = st }).OrderBy(x => x.StudentData.ID).Distinct();

                if (studentID != null)
                {
                    feeVal = feeVal.Where(x => x.StudentData.ID == studentID);
                }
                var StudentVal = (from st in db.STUDENTs
                                  select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                if (studentID != null)
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == studentID);
                    ViewData["student"] = db.STUDENTs.Find(studentID);
                }
                else
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == feeVal.FirstOrDefault().StudentData.ID);
                    ViewData["student"] = db.STUDENTs.Where(x => x.ID == feeVal.FirstOrDefault().StudentData.ID).FirstOrDefault();
                }
                STUDENT_CATGEORY StudentCategoryVal = db.STUDENT_CATGEORY.Find(StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID);
                ViewBag.StudentCategory = StudentCategoryVal.NAME;
                var prev_student_val = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        where ff.FEE_CLCT_ID == fee_collection.ID
                                        select st).OrderBy(x => x.ID).Distinct();
                int prev_student_id = -1;
                if (studentID != null)
                {
                    prev_student_val = prev_student_val.Where(x => x.ID < studentID);
                    if (prev_student_val != null && prev_student_val.Count() != 0)
                    {
                        prev_student_id = prev_student_val.Max(x => x.ID);
                    }
                }
                STUDENT prev_student = db.STUDENTs.Find(prev_student_id);
                ViewData["prev_student"] = prev_student;
                ViewBag.prev_student_id = prev_student_id;

                var next_student_val = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        where ff.FEE_CLCT_ID == fee_collection.ID
                                        select st).OrderBy(x => x.ID).Distinct();
                int next_student_id = -1;
                if (studentID != null)
                {
                    next_student_val = next_student_val.Where(x => x.ID > studentID);
                    if (next_student_val != null && next_student_val.Count() != 0)
                    {
                        next_student_id = next_student_val.Min(x => x.ID);
                    }
                }
                STUDENT next_student = db.STUDENTs.Find(next_student_id);
                ViewData["next_student"] = next_student;
                ViewBag.next_student_id = next_student_id;

                var financefeeVal = (from ff in db.FINANCE_FEE
                                     where ff.FEE_CLCT_ID == fee_collection.ID && ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID
                                     select ff).ToList();
                ViewData["financefee"] = financefeeVal;
                ViewBag.due_date = fee_collection.DUE_DATE;
                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     where ff.FEE_CLCT_ID == fee_collection.ID && st.ID == studentID
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
                ViewData["fee_category"] = fee_category;
                var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.IS_DEL == "N" && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                           select ff).ToList();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == studentID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;
                decimal total_discount_val = 0;
                decimal total_payable = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_payable = total_payable + (decimal)item.AMT;
                }
                ViewBag.total_payable = total_payable;
                if (batch_discounts_val != null && batch_discounts_val.Count() != 0)
                {
                    foreach (var item in batch_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (student_discounts_val != null && student_discounts_val.Count() != 0)
                {
                    foreach (var item in student_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (category_discounts_val != null && category_discounts_val.Count() != 0)
                {
                    foreach (var item in category_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == studentID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                        select ff);
                ViewData["student_fine"] = student_fine_val;
                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                         select ff);
                ViewData["category_fine"] = category_fine_val;
                decimal total_fine_val = 0;
                if (batch_fine_val != null && batch_fine_val.Count() != 0)
                {
                    foreach (var item in batch_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (student_fine_val != null && student_fine_val.Count() != 0)
                {
                    foreach (var item in student_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (category_fine_val != null && category_fine_val.Count() != 0)
                {
                    foreach (var item in category_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }      
                ViewBag.total_fine = total_fine_val;

                if (total_discount_val > total_payable + total_fine_val && total_payable + total_fine_val >= 0)
                {
                    total_discount_val = total_payable + total_fine_val;
                }
                ViewBag.total_discount = total_discount_val;
                decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
                ViewBag.total_discount_percentage = total_discount_percentage_val;

                /* New code added to handle message and fee paid status*/

                decimal total_fees = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_fees += (decimal)item.AMT;
                }
                if (total_discount_val != 0)
                {
                    total_fees -= total_discount_val;
                }
                if (total_fine_val != 0)
                {
                    total_fees += total_fine_val;
                }
                decimal paid_before = 0;
                foreach (var item13 in paid_fees_val)
                {
                    paid_before += (decimal)item13.FinanceTransactionData.AMT;
                }
                decimal fees_paid = (decimal)fee;
                var FeeCat = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Fees").Distinct();
                if (FeeCat != null && FeeCat.Count() > 0)
                {
                    if (fees_paid >= 0)
                    {
                        ViewBag.fine = fee;
                        if (fee > 0)
                        {
                            var FeeFine = new FEE_FINE()
                            {
                                TYPE = "Student",
                                NAME = "Ad-hoc fine",
                                RCVR_ID = StudentVal.FirstOrDefault().Student_data.ID,
                                FIN_FEE_CAT_ID = date_val.FEE_CAT_ID,
                                FINE = fee,
                                DESCR = fee_desc,
                                FEE_CLCT_ID = date_val.ID,
                                IS_AMT = true,
                                FINE_DATE = System.DateTime.Now
                            };
                            db.FEE_FINE.Add(FeeFine);
                            try { db.SaveChanges(); ViewBag.FeeCollectionMessage = string.Concat("Fee Fine added in Collection Successfully"); }
                            catch (Exception e) { ViewBag.FeeCollectionMessage = string.Concat("Please eneter valid value for Fine"); Console.WriteLine(e); }
                        }
                        else
                        {
                            ViewBag.FeeCollectionMessage = string.Concat("Please eneter valid value for Fine");
                        }

                        FINANCE_FEE ffUpdate = db.FINANCE_FEE.Find(financefeeVal.FirstOrDefault().ID);
                        ffUpdate.IS_PD = fees_paid == total_fees - paid_before ? true : false;
                        db.Entry(ffUpdate).State = EntityState.Modified;
                        try { db.SaveChanges(); }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    ViewBag.FeeCollectionMessage = string.Concat(ViewBag.FeeCollectionMessage, "|", ve.ErrorMessage);
                                }
                            }
                            return PartialView("_Student_Fees_Submission");
                        }
                        catch (Exception e)
                        {
                            ViewBag.FeeCollectionMessage = string.Concat(ViewBag.FeeCollectionMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                            return PartialView("_Student_Fees_Submission");
                        }
                        var student_fine_val2 = (from ff in db.FEE_FINE
                                                 where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                                 select ff);
                        ViewData["student_fine"] = student_fine_val2;
                        decimal updated_fine_val = 0;
                        if (batch_fine_val != null && batch_fine_val.Count() != 0)
                        {
                            foreach (var item in batch_fine_val)
                            {
                                updated_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                            }
                        }
                        if (student_fine_val2 != null && student_fine_val2.Count() != 0)
                        {
                            foreach (var item in student_fine_val2)
                            {
                                updated_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                            }
                        }
                        if (category_fine_val != null && category_fine_val.Count() != 0)
                        {
                            foreach (var item in category_fine_val)
                            {
                                updated_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                            }
                        }
                        ViewBag.total_fine = updated_fine_val;

                        if (total_discount_val > total_payable + updated_fine_val && total_payable + updated_fine_val >= 0)
                        {
                            total_discount_val = total_payable + updated_fine_val;
                        }
                        ViewBag.total_discount = total_discount_val;
                        total_discount_percentage_val = total_discount_val / total_payable * 100;
                        ViewBag.total_discount_percentage = total_discount_percentage_val;
                    }
                    else
                    {
                        ViewBag.fine = fee;
                        ViewBag.FeeCollectionMessage = string.Concat("This does not seem to be a valid transaction.");
                    }
                }
                else
                {
                    ViewBag.FeeCollectionMessage = string.Concat("A Finance Category of name *Fees* and type *Income* has to be made in order to proceed with Payment.");
                }
                /* End of New code added to handle message and fee paid status*/
            }

            SFSAcademy.SubmitFeeDiscounts vModelDiscount = new SFSAcademy.SubmitFeeDiscounts();
            ViewData["FeeDiscountsAdd"] = vModelDiscount;

            SFSAcademy.SubmitFeeFine vModelFine2 = new SFSAcademy.SubmitFeeFine();
            ViewData["FeeFineAdd"] = vModelFine2;

            SFSAcademy.SubmitFees vModel = new SFSAcademy.SubmitFees();
            this.PreventResubmit(vModel);// << Fill TempData & ViewModel PreventResubmit Property

            return PartialView("_Student_Fees_Submission", vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update_Discount_Ajax(SFSAcademy.SubmitFeeDiscounts vModelDiscount)
        {
            int? studentID = vModelDiscount.StudentID;
            int? batch_id = vModelDiscount.Batch_id;
            int? date = vModelDiscount.Date;
            decimal? discount = vModelDiscount.Discount;
            string discount_desc = vModelDiscount.Discount_Desc;
            if (ModelState.IsValid)
            {
                DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
                ViewBag.ReturnDate = PDate.ToShortDateString();
                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == batch_id
                                 select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;
                FINANCE_FEE_COLLECTION date_val = db.FINANCE_FEE_COLLECTION.Find(date);
                ViewData["date"] = date_val;
                FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(date);

                var feeVal = (from ff in db.FINANCE_FEE
                              join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                              where ff.FEE_CLCT_ID == fee_collection.ID
                              select new StundentFee { FinanceFeeData = ff, StudentData = st }).OrderBy(x => x.StudentData.ID).Distinct();

                if (studentID != null)
                {
                    feeVal = feeVal.Where(x => x.StudentData.ID == studentID);
                }
                var StudentVal = (from st in db.STUDENTs
                                  select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                if (studentID != null)
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == studentID);
                    ViewData["student"] = db.STUDENTs.Find(studentID);
                }
                else
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == feeVal.FirstOrDefault().StudentData.ID);
                    ViewData["student"] = db.STUDENTs.Where(x => x.ID == feeVal.FirstOrDefault().StudentData.ID).FirstOrDefault();
                }
                STUDENT_CATGEORY StudentCategoryVal = db.STUDENT_CATGEORY.Find(StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID);
                ViewBag.StudentCategory = StudentCategoryVal.NAME;
                var prev_student_val = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        where ff.FEE_CLCT_ID == fee_collection.ID
                                        select st).OrderBy(x => x.ID).Distinct();
                int prev_student_id = -1;
                if (studentID != null)
                {
                    prev_student_val = prev_student_val.Where(x => x.ID < studentID);
                    if (prev_student_val != null && prev_student_val.Count() != 0)
                    {
                        prev_student_id = prev_student_val.Max(x => x.ID);
                    }
                }
                STUDENT prev_student = db.STUDENTs.Find(prev_student_id);
                ViewData["prev_student"] = prev_student;
                ViewBag.prev_student_id = prev_student_id;

                var next_student_val = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        where ff.FEE_CLCT_ID == fee_collection.ID
                                        select st).OrderBy(x => x.ID).Distinct();
                int next_student_id = -1;
                if (studentID != null)
                {
                    next_student_val = next_student_val.Where(x => x.ID > studentID);
                    if (next_student_val != null && next_student_val.Count() != 0)
                    {
                        next_student_id = next_student_val.Min(x => x.ID);
                    }
                }
                STUDENT next_student = db.STUDENTs.Find(next_student_id);
                ViewData["next_student"] = next_student;
                ViewBag.next_student_id = next_student_id;

                var financefeeVal = (from ff in db.FINANCE_FEE
                                     where ff.FEE_CLCT_ID == fee_collection.ID && ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID
                                     select ff).ToList();
                ViewData["financefee"] = financefeeVal;
                ViewBag.due_date = fee_collection.DUE_DATE;
                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     where ff.FEE_CLCT_ID == fee_collection.ID && st.ID == studentID
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
                ViewData["fee_category"] = fee_category;
                var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.IS_DEL == "N" && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                           select ff).ToList();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == studentID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;
                decimal total_discount_val = 0;
                decimal total_payable = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_payable = total_payable + (decimal)item.AMT;
                }
                ViewBag.total_payable = total_payable;

                if (batch_discounts_val != null && batch_discounts_val.Count() != 0)
                {
                    foreach (var item in batch_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (student_discounts_val != null && student_discounts_val.Count() != 0)
                {
                    foreach (var item in student_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (category_discounts_val != null && category_discounts_val.Count() != 0)
                {
                    foreach (var item in category_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == studentID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                        select ff);
                ViewData["student_fine"] = student_fine_val;
                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                         select ff);
                ViewData["category_fine"] = category_fine_val;
                decimal total_fine_val = 0;
                if (batch_fine_val != null && batch_fine_val.Count() != 0)
                {
                    foreach (var item in batch_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (student_fine_val != null && student_fine_val.Count() != 0)
                {
                    foreach (var item in student_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (category_fine_val != null && category_fine_val.Count() != 0)
                {
                    foreach (var item in category_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }


                ViewBag.total_fine = total_fine_val;

                if (total_discount_val > total_payable + total_fine_val && total_payable + total_fine_val >= 0)
                {
                    total_discount_val = total_payable + total_fine_val;
                }
                ViewBag.total_discount = total_discount_val;
                decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
                ViewBag.total_discount_percentage = total_discount_percentage_val;

                /* New code added to handle message and fee paid status*/

                decimal total_fees = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_fees += (decimal)item.AMT;
                }
                if (total_discount_val != 0)
                {
                    total_fees -= total_discount_val;
                }
                if (total_fine_val != 0)
                {
                    total_fees += total_fine_val;
                }
                decimal paid_before = 0;
                foreach (var item13 in paid_fees_val)
                {
                    paid_before += (decimal)item13.FinanceTransactionData.AMT;
                }
                decimal fees_paid = (decimal)discount;
                var FeeCat = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Fees").Distinct();
                if (FeeCat != null && FeeCat.Count() > 0)
                {
                    if (fees_paid >= 0)
                    {
                        if (fees_paid <= total_fees - paid_before)
                        {
                            ViewBag.discount = discount;
                            if (discount > 0)
                            {
                                var FeeDiscount = new FEE_DISCOUNT()
                                {
                                    TYPE = "Student",
                                    NAME = "Ad-hoc Discount",
                                    RCVR_ID = StudentVal.FirstOrDefault().Student_data.ID,
                                    FIN_FEE_CAT_ID = date_val.FEE_CAT_ID,
                                    DISC = discount,
                                    DESCR = discount_desc,
                                    FEE_CLCT_ID = date_val.ID,
                                    IS_AMT = true,
                                    DISC_DATE = System.DateTime.Now
                                };
                                db.FEE_DISCOUNT.Add(FeeDiscount);
                                try { db.SaveChanges(); ViewBag.FeeCollectionMessage = string.Concat("Fee Discount Added in Collection Successfully"); }
                                catch (Exception e) { ViewBag.FeeCollectionMessage = string.Concat("Please eneter valid value for Discount"); Console.WriteLine(e); }
                            }
                            else
                            {
                                ViewBag.FeeCollectionMessage = string.Concat("Please eneter valid value for Discount");
                            }

                            FINANCE_FEE ffUpdate = db.FINANCE_FEE.Find(financefeeVal.FirstOrDefault().ID);
                            ffUpdate.IS_PD = fees_paid == total_fees - paid_before ? true : false;
                            db.Entry(ffUpdate).State = EntityState.Modified;
                            try { db.SaveChanges(); }
                            catch (DbEntityValidationException e)
                            {
                                foreach (var eve in e.EntityValidationErrors)
                                {
                                    foreach (var ve in eve.ValidationErrors)
                                    {
                                        ViewBag.FeeCollectionMessage = string.Concat(ViewBag.FeeCollectionMessage, "|", ve.ErrorMessage);
                                    }
                                }
                                return PartialView("_Student_Fees_Submission");
                            }
                            catch (Exception e)
                            {
                                ViewBag.FeeCollectionMessage = string.Concat(ViewBag.FeeCollectionMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                                return PartialView("_Student_Fees_Submission");
                            }
                            var student_discounts_val2 = (from ff in db.FEE_DISCOUNT
                                                          where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == studentID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                                          select ff);
                            ViewData["student_discounts"] = student_discounts_val2;
                            decimal updated_discount_val = 0;

                            if (batch_discounts_val != null && batch_discounts_val.Count() != 0)
                            {
                                foreach (var item in batch_discounts_val)
                                {
                                    updated_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                                }
                            }
                            if (student_discounts_val2 != null && student_discounts_val2.Count() != 0)
                            {
                                foreach (var item in student_discounts_val2)
                                {
                                    updated_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                                }
                            }
                            if (category_discounts_val != null && category_discounts_val.Count() != 0)
                            {
                                foreach (var item in category_discounts_val)
                                {
                                    updated_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                                }
                            }

                            if (updated_discount_val > total_payable + total_fine_val && total_payable + total_fine_val >= 0)
                            {
                                updated_discount_val = total_payable + total_fine_val;
                            }
                            ViewBag.total_discount = updated_discount_val;
                            decimal updated_discount_percentage_val = updated_discount_val / total_payable * 100;
                            ViewBag.total_discount_percentage = updated_discount_percentage_val;
                        }
                        else
                        {
                            ViewBag.discount = discount;
                            ViewBag.FeeCollectionMessage = string.Concat("After discount, you are paying more then the Fees. Please correct amount first.");
                        }
                    }
                    else
                    {
                        ViewBag.discount = discount;
                        ViewBag.FeeCollectionMessage = string.Concat("This does not seem to be a valid transaction.");
                    }
                }
                else
                {
                    ViewBag.FeeCollectionMessage = string.Concat("A Finance Category of name *Fees* and type *Income* has to be made in order to proceed with Payment.");
                }
                /* End of New code added to handle message and fee paid status*/
            }

            SFSAcademy.SubmitFeeDiscounts vModelDiscount2 = new SFSAcademy.SubmitFeeDiscounts();
            ViewData["FeeDiscountsAdd"] = vModelDiscount2;

            SFSAcademy.SubmitFeeFine vModelFine = new SFSAcademy.SubmitFeeFine();
            ViewData["FeeFineAdd"] = vModelFine;

            SFSAcademy.SubmitFees vModel = new SFSAcademy.SubmitFees();
            this.PreventResubmit(vModel);// << Fill TempData & ViewModel PreventResubmit Property

            return PartialView("_Student_Fees_Submission", vModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult update_ajax( SFSAcademy.SubmitFees vModel)
        {
            int? student = vModel.StudentID;
            int? batch_id = vModel.Batch_id;
            int? date = vModel.Date;
            string PAYMENT_MODE = vModel.PAYMENT_MODE;
            string PAYMENT_NOTE = vModel.PAYMENT_NOTE;
            decimal? PAYMENT_AMOUNT = vModel.PAYMENT_AMOUNT;
            DateTime PAYMENT_DATE = Convert.ToDateTime(vModel.PAYMENT_DATE);
            if (ModelState.IsValid)
            {
                //DateTime PDate = Convert.ToDateTime(PAYMENT_DATE);
                DateTime PDate = PAYMENT_DATE;
                ViewBag.ReturnDate = PDate.ToShortDateString();
                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == batch_id
                                 select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;
                FINANCE_FEE_COLLECTION date_val = db.FINANCE_FEE_COLLECTION.Find(date);
                ViewData["date"] = date_val;
                FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(date);

                var feeVal = (from ff in db.FINANCE_FEE
                              join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                              where ff.FEE_CLCT_ID == fee_collection.ID
                              select new StundentFee { FinanceFeeData = ff, StudentData = st }).OrderBy(x => x.StudentData.ID).Distinct();

                if (student != null)
                {
                    feeVal = feeVal.Where(x => x.StudentData.ID == student);
                }
                var StudentVal = (from st in db.STUDENTs
                                  select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                if (student != null)
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == student);
                    ViewData["student"] = db.STUDENTs.Find(student);
                }
                else
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == feeVal.FirstOrDefault().StudentData.ID);
                    ViewData["student"] = db.STUDENTs.Where(x => x.ID == feeVal.FirstOrDefault().StudentData.ID).FirstOrDefault();
                }
                STUDENT_CATGEORY StudentCategoryVal = db.STUDENT_CATGEORY.Find(StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID);
                ViewBag.StudentCategory = StudentCategoryVal.NAME;
                var prev_student_val = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        where ff.FEE_CLCT_ID == fee_collection.ID
                                        select st).OrderBy(x => x.ID).Distinct();
                int prev_student_id = -1;
                if (student != null)
                {
                    prev_student_val = prev_student_val.Where(x => x.ID < student);
                    if (prev_student_val != null && prev_student_val.Count() != 0)
                    {
                        prev_student_id = prev_student_val.Max(x => x.ID);
                    }
                }
                STUDENT prev_student = db.STUDENTs.Find(prev_student_id);
                ViewData["prev_student"] = prev_student;
                ViewBag.prev_student_id = prev_student_id;

                var next_student_val = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        where ff.FEE_CLCT_ID == fee_collection.ID
                                        select st).OrderBy(x => x.ID).Distinct();
                int next_student_id = -1;
                if (student != null)
                {
                    next_student_val = next_student_val.Where(x => x.ID > student);
                    if (next_student_val != null && next_student_val.Count() != 0)
                    {
                        next_student_id = next_student_val.Min(x => x.ID);
                    }
                }
                STUDENT next_student = db.STUDENTs.Find(next_student_id);
                ViewData["next_student"] = next_student;
                ViewBag.next_student_id = next_student_id;

                var financefeeVal = (from ff in db.FINANCE_FEE
                                     where ff.FEE_CLCT_ID == fee_collection.ID && ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID
                                     select ff).ToList();
                ViewData["financefee"] = financefeeVal;
                ViewBag.due_date = fee_collection.DUE_DATE;
                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     where ff.FEE_CLCT_ID == fee_collection.ID && st.ID == student
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
                ViewData["fee_category"] = fee_category;
                var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.IS_DEL == "N" && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                           select ff).ToList();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == student && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;
                decimal total_discount_val = 0;
                decimal total_payable = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_payable = total_payable + (decimal)item.AMT;
                }
                ViewBag.total_payable = total_payable;
                if (batch_discounts_val != null && batch_discounts_val.Count() != 0)
                {
                    foreach (var item in batch_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (student_discounts_val != null && student_discounts_val.Count() != 0)
                {
                    foreach (var item in student_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (category_discounts_val != null && category_discounts_val.Count() != 0)
                {
                    foreach (var item in category_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == student && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                        select ff);
                ViewData["student_fine"] = student_fine_val;
                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                         select ff);
                ViewData["category_fine"] = category_fine_val;
                decimal total_fine_val = 0;
                if (batch_fine_val != null && batch_fine_val.Count() != 0)
                {
                    foreach (var item in batch_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (student_fine_val != null && student_fine_val.Count() != 0)
                {
                    foreach (var item in student_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (category_fine_val != null && category_fine_val.Count() != 0)
                {
                    foreach (var item in category_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                ViewBag.total_fine = total_fine_val;

                if (total_discount_val > total_payable + total_fine_val && total_payable + total_fine_val >= 0)
                {
                    total_discount_val = total_payable + total_fine_val;
                }
                ViewBag.total_discount = total_discount_val;
                decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
                ViewBag.total_discount_percentage = total_discount_percentage_val;

                decimal total_fees = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_fees += (decimal)item.AMT;
                }
                if(total_discount_val != 0)
                {
                    total_fees -= total_discount_val;
                }
                if (total_fine_val != 0)
                {
                    total_fees += total_fine_val;
                }
                decimal paid_before = 0;
                foreach (var item13 in paid_fees_val)
                {
                    paid_before += (decimal)item13.FinanceTransactionData.AMT;
                }
                decimal fees_paid = (decimal)PAYMENT_AMOUNT;
                var FeeCat = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Fees").Distinct();
                if(FeeCat != null && FeeCat.Count()>0)
                {
                    if (fees_paid >= 0)
                    {
                        if (fees_paid <= total_fees- paid_before)
                        {
                            if (!this.IsResubmit(vModel)) //  << Check Resubmit
                            {
                                //var FeeCat = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Fees").Distinct();
                                int TranCatId = FeeCat != null ? Convert.ToInt32(FeeCat.FirstOrDefault().ID) : -1;
                                int FinanceFee_id = financefeeVal != null && financefeeVal.Count() != 0 ? financefeeVal.FirstOrDefault().ID : -1;
                                String ReceiptNo = (feeVal != null && feeVal.Count() != 0) ? feeVal.FirstOrDefault().FinanceFeeData.ID.ToString() : "";
                                int PayeeId = StudentVal != null ? StudentVal.FirstOrDefault().Student_data.ID : -1;

                                var transaction = new FINANCE_TRANSACTION()
                                {
                                    TIL = fees_paid < total_fees - paid_before ? string.Concat("FN.Partial: ", ReceiptNo) : string.Concat("FN: ", ReceiptNo),
                                    CAT_ID = TranCatId,
                                    DESCR = fees_paid < total_fees - paid_before ? string.Concat(PAYMENT_MODE, " : ", PAYMENT_NOTE, " : FN.-", ReceiptNo) : string.Concat("Full Payment: ", PAYMENT_MODE, " : ", PAYMENT_NOTE, " : FN.", ReceiptNo),
                                    PAYEE_ID = PayeeId,
                                    PAYEE_TYPE = "Student",
                                    AMT = (decimal)fees_paid,
                                    FINE_AMT = total_fine_val,
                                    FINE_INCLD = total_fine_val != 0 ? true : false,
                                    FIN_FE_ID = FinanceFee_id,
                                    MSTRTRAN_ID = -1,
                                    RCPT_NO = ReceiptNo,
                                    TRAN_DATE = PDate,
                                    CRETAED_AT = System.DateTime.Now,
                                    UPDATED_AT = System.DateTime.Now
                                };
                                db.FINANCE_TRANSACTION.Add(transaction);
                                try { db.SaveChanges(); }
                                catch (DbEntityValidationException e)
                                {
                                    foreach (var eve in e.EntityValidationErrors)
                                    {
                                        foreach (var ve in eve.ValidationErrors)
                                        {
                                            ViewBag.FeeCollectionMessage = string.Concat(ViewBag.FeeCollectionMessage, "|", ve.ErrorMessage);
                                        }
                                    }
                                    return PartialView("_Student_Fees_Submission");
                                }
                                catch (Exception e)
                                {
                                    ViewBag.FeeCollectionMessage = string.Concat(ViewBag.FeeCollectionMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                                    if(e.InnerException != null)
                                    {
                                        if (e.InnerException.InnerException.Message.Contains("FIN_F_FINA_FINA_NI5"))
                                        {
                                            ViewBag.ErrorMessage = "This Student is added in system after current fee collection was set-up. Current fee has to be collected manually. This Student’s fee can be collected through system next collection onward.";
                                        }
                                    }
                                    return PartialView("_Student_Fees_Submission");
                                }

                                string tid = null;
                                if (financefeeVal.FirstOrDefault().TRAN_ID != null)
                                {
                                    tid = string.Concat(financefeeVal.FirstOrDefault().TRAN_ID, ",", transaction.ID.ToString());
                                }
                                else
                                {
                                    tid = transaction.ID.ToString();
                                }
                                FINANCE_FEE ffUpdate = db.FINANCE_FEE.Find(financefeeVal.FirstOrDefault().ID);
                                ffUpdate.TRAN_ID = tid;
                                ffUpdate.IS_PD = fees_paid == total_fees - paid_before ? true : false;
                                db.Entry(ffUpdate).State = EntityState.Modified;
                                try { db.SaveChanges(); }
                                catch (DbEntityValidationException e)
                                {
                                    foreach (var eve in e.EntityValidationErrors)
                                    {
                                        foreach (var ve in eve.ValidationErrors)
                                        {
                                            ViewBag.FeeCollectionMessage = string.Concat(ViewBag.FeeCollectionMessage, "|", ve.ErrorMessage);
                                        }
                                    }
                                    return PartialView("_Student_Fees_Submission");
                                }
                                catch (Exception e)
                                {
                                    ViewBag.FeeCollectionMessage = string.Concat(ViewBag.FeeCollectionMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                                    return PartialView("_Student_Fees_Submission");
                                }
                                var paid_fees_val2 = (from ff in db.FINANCE_FEE
                                                      join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                                      join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                                      where ff.FEE_CLCT_ID == fee_collection.ID && st.ID == student
                                                      select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                                ViewData["paid_fees"] = paid_fees_val2;
                                var student_fine_val2 = (from ff in db.FEE_FINE
                                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                                         select ff);
                                ViewData["student_fine"] = student_fine_val2;
                                ViewBag.FeeCollectionMessage = "Student's fee details updated successfully.";
                            }
                            else
                            {
                                ViewBag.FeeCollectionMessage = "Please re-initiate the transaction.";
                            }
                             
                        }
                        else
                        {
                            ViewData["paid_fees"] = paid_fees_val;
                            ViewBag.FeeCollectionMessage = string.Concat("You are paying more then the Fees. Please correct amount.");
                        }
                    }
                    else
                    {
                        ViewData["paid_fees"] = paid_fees_val;
                        ViewBag.FeeCollectionMessage = string.Concat("This does not seem to be a valid transaction.");
                    }
                }
                else
                {
                    ViewBag.FeeCollectionMessage = string.Concat("A Finance Category of name *Fees* and type *Income* has to be made in order to proceed with Payment.");
                }
                
                
            }

            SFSAcademy.SubmitFeeDiscounts vModelDiscount = new SFSAcademy.SubmitFeeDiscounts();
            ViewData["FeeDiscountsAdd"] = vModelDiscount;

            SFSAcademy.SubmitFeeFine vModelFine = new SFSAcademy.SubmitFeeFine();
            ViewData["FeeFineAdd"] = vModelFine;

            this.PreventResubmit(vModel);// << Fill TempData & ViewModel PreventResubmit Property
            return PartialView("_Student_Fees_Submission", vModel);
        }

        // GET: Finance/Delete/5
        [HttpGet]
        public ActionResult student_fee_receipt_pdf(int? id, int? id2)
        {
            FINANCE_FEE_COLLECTION date_val = db.FINANCE_FEE_COLLECTION.Find(id2);
            ViewData["date"] = date_val;
            FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(id2);
            var StudentVal = db.STUDENTs.Find(id);
            ViewData["student"] = StudentVal;

            var financefeeVal = (from ff in db.FINANCE_FEE
                                 where ff.FEE_CLCT_ID == fee_collection.ID && ff.STDNT_ID == id
                                 select ff).ToList();
            ViewData["financefee"] = financefeeVal;

            ViewBag.due_date = fee_collection.DUE_DATE;
            var paid_fees_val = (from ff in db.FINANCE_FEE
                                 join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                 join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                 where ff.FEE_CLCT_ID == fee_collection.ID && st.ID == StudentVal.ID
                                 select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewData["paid_fees"] = paid_fees_val;

            FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
            ViewData["fee_category"] = fee_category;

            var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                       where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.IS_DEL == "N" && (ff.STDNT_ID == StudentVal.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                       select ff).ToList();
            ViewData["fee_particulars"] = fee_particulars_val;

            var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                       where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                       select ff);
            ViewData["batch_discounts"] = batch_discounts_val;
            var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                         select ff);
            ViewData["student_discounts"] = student_discounts_val;
            var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                          where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                          select ff);
            ViewData["category_discounts"] = category_discounts_val;
            decimal total_discount_val = 0;
            decimal total_payable = 0;
            foreach (var item in fee_particulars_val)
            {
                total_payable = total_payable + (decimal)item.AMT;
            }
            ViewBag.total_payable = total_payable;
            if (batch_discounts_val != null && batch_discounts_val.Count() != 0)
            {
                foreach (var item in batch_discounts_val)
                {
                    total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }               
            }
            if (student_discounts_val != null && student_discounts_val.Count() != 0)
            {
                foreach (var item in student_discounts_val)
                {
                    total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }
            }
            if (category_discounts_val != null && category_discounts_val.Count() != 0)
            {
                foreach (var item in category_discounts_val)
                {
                    total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }
            }

            var batch_fine_val = (from ff in db.FEE_FINE
                                  where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                  select ff);
            ViewData["batch_fine"] = batch_fine_val;
            var student_fine_val = (from ff in db.FEE_FINE
                                    where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                    select ff);
            ViewData["student_fine"] = student_fine_val;
            var category_fine_val = (from ff in db.FEE_FINE
                                     where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                     select ff);
            ViewData["category_fine"] = category_fine_val;
            decimal total_fine_val = 0;
            if (batch_fine_val != null && batch_fine_val.Count() != 0)
            {
                foreach (var item in batch_fine_val)
                {
                    total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            if (student_fine_val != null && student_fine_val.Count() != 0)
            {
                foreach (var item in student_fine_val)
                {
                    total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            if (category_fine_val != null && category_fine_val.Count() != 0)
            {
                foreach (var item in category_fine_val)
                {
                    total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            ViewBag.total_fine = total_fine_val;

            if (total_discount_val > total_payable + total_fine_val && total_payable + total_fine_val >= 0)
            {
                total_discount_val = total_payable + total_fine_val;
            }
            ViewBag.total_discount = total_discount_val;
            decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
            ViewBag.total_discount_percentage = total_discount_percentage_val;

            decimal total_fees = 0;
            foreach (var item in fee_particulars_val)
            {
                total_fees += (decimal)item.AMT;
            }
            if (total_fine_val != 0)
            {
                total_fees += total_fine_val;
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult ExportToPDF(string GridHtml, string CSSHtml)
        {

            var html = GridHtml;
            var css = CSSHtml;
            //Register a single font
            //FontFactory.Register(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "comic.ttf"), "Comic Sans MS");


            //Placeholder variable for later
            Byte[] bytes;

            using (var ms = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    doc.SetPageSize(PageSize.A4.Rotate());

                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();

                        //Get a stream of our HTML
                        using (var msHTML = new MemoryStream(Encoding.UTF8.GetBytes(html)))
                        {

                            //Get a stream of our CSS
                            using (var msCSS = new MemoryStream(Encoding.UTF8.GetBytes(css)))
                            {

                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHTML, msCSS, Encoding.UTF8, FontFactory.FontImp);
                            }
                        }

                        doc.Close();
                    }
                }

                bytes = ms.ToArray();
                //return bytes;
                return File(bytes, "application/pdf", "Grid.pdf");
            }
        }

        [HttpGet]
        // GET: Fee Index
        public ActionResult Fees_Student_Search(string ErrorMessage)
        {
            var StudentVal = (from st in db.STUDENTs
                              where st.IS_DEL==false && st.IS_ACT == true
                              select st ).OrderBy(x => x.ID).Distinct();
            ViewBag.FeeCollectionMessage = ErrorMessage;

            //var student_select = db.STUDENTs.Include(x=>x.b).Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            var student_select = (from std in db.STUDENTs
                                  join bt in db.BATCHes on std.BTCH_ID equals bt.ID
                                  join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                  where std.IS_DEL == false && std.IS_ACT == true
                                  select new Student {StudentData = std, BatcheData = bt, CourseData = cs }).OrderBy(x => x.StudentData.FIRST_NAME).ThenBy(x => x.StudentData.MID_NAME).ThenBy(x => x.StudentData.LAST_NAME).ToList();
            ViewData["student_select"] = student_select;

            return View(StudentVal.ToList());

        }

        [HttpGet]
        // GET: Fee Index
        public ActionResult fees_student_dates(string id)
        {
            var StudentVal = (from st in db.STUDENTs
                              where st.ADMSN_NO == id
                              select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();

            if (StudentVal != null && StudentVal.Count() != 0)
            {
                STUDENT student = db.STUDENTs.Find(StudentVal.FirstOrDefault().Student_data.ID);
                ViewData["student"] = student;

                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == student.BTCH_ID
                                 select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;


                var queryCollections = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                     join bt in db.BATCHes on fc.BTCH_ID equals bt.ID
                                     join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                     where st.ID == student.ID && fc.BTCH_ID == student.BTCH_ID && ff.IS_PD == false && fc.IS_DEL == false
                                     select new { fc.ID, fc.NAME, fc.DUE_DATE, cs.CODE }).OrderBy(a => a.CODE).Distinct().ToList();

                List<SelectListItem> options2 = new List<SelectListItem>();
                foreach (var item in queryCollections)
                {
                    string CollectionDate = string.Concat(item.CODE, " / ", item.NAME, " / ", Convert.ToDateTime(item.DUE_DATE).ToString("dd-MMM-yy"));
                    var result = new SelectListItem();
                    result.Text = CollectionDate;
                    result.Value = item.ID.ToString();
                    options2.Add(result);
                }
                // add the 'ALL' option
                options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Collection Date" });
                ViewBag.COLL_ID = options2;
                Session["student_serach"] = "Y";
            }
            else
            {
                int MinStudentId = db.STUDENTs.Min(x => x.ID);
                STUDENT student = db.STUDENTs.Find(MinStudentId);
                ViewData["student"] = student;
                int MinBatchId = db.BATCHes.Min(x => x.ID);
                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == MinBatchId
                                 select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                 .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;
                List<SelectListItem> options2 = new List<SelectListItem>();
                options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Collection Date" });
                ViewBag.COLL_ID = options2;
                ViewBag.FeeCollectionMessage = "This Admission Number does not exit, Or Student has left school. Please contact Admin Office.";
            }

            return PartialView("_fees_student_dates");
        }




        // GET: Finance/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE fINANCE_FEE = db.FINANCE_FEE.Find(id);
            if (fINANCE_FEE == null)
            {
                return HttpNotFound();
            }
            return View(fINANCE_FEE);
        }



        // GET: Finance/Create
        public ActionResult Create()
        {
            ViewBag.FEE_CLCT_ID = new SelectList(db.FINANCE_FEE_COLLECTION, "ID", "NAME");
            ViewBag.TRAN_ID = new SelectList(db.FINANCE_TRANSACTION, "ID", "TIL");
            ViewBag.STDNT_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO");
            return View();
        }

        // POST: Finance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FEE_CLCT_ID,TRAN_ID,STDNT_ID,IS_PD")] FINANCE_FEE fINANCE_FEE)
        {
            if (ModelState.IsValid)
            {
                db.FINANCE_FEE.Add(fINANCE_FEE);
                ViewBag.FEE_CLCT_ID = new SelectList(db.FINANCE_FEE_COLLECTION, "ID", "NAME");
                ViewBag.TRAN_ID = new SelectList(db.FINANCE_TRANSACTION, "ID", "TIL");
                ViewBag.STDNT_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO");
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return PartialView("_Student_Fees_Submission");
                }
                return View();
            }

            ViewBag.FEE_CLCT_ID = new SelectList(db.FINANCE_FEE_COLLECTION, "ID", "NAME", fINANCE_FEE.FEE_CLCT_ID);
            ViewBag.TRAN_ID = new SelectList(db.FINANCE_TRANSACTION, "ID", "TIL", fINANCE_FEE.TRAN_ID);
            ViewBag.STDNT_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO", fINANCE_FEE.STDNT_ID);
            return View(fINANCE_FEE);
        }

        // GET: Finance/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE fINANCE_FEE = db.FINANCE_FEE.Find(id);
            if (fINANCE_FEE == null)
            {
                return HttpNotFound();
            }
            ViewBag.FEE_CLCT_ID = new SelectList(db.FINANCE_FEE_COLLECTION, "ID", "NAME", fINANCE_FEE.FEE_CLCT_ID);
            ViewBag.TRAN_ID = new SelectList(db.FINANCE_TRANSACTION, "ID", "TIL", fINANCE_FEE.TRAN_ID);
            ViewBag.STDNT_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO", fINANCE_FEE.STDNT_ID);
            return View(fINANCE_FEE);
        }

        // POST: Finance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FEE_CLCT_ID,TRAN_ID,STDNT_ID,IS_PD")] FINANCE_FEE fINANCE_FEE)
        {
            if (ModelState.IsValid)
            {
                FINANCE_FEE fINANCE_FEE_UPD = db.FINANCE_FEE.Find(fINANCE_FEE.ID);
                fINANCE_FEE_UPD.FEE_CLCT_ID = fINANCE_FEE.FEE_CLCT_ID;
                fINANCE_FEE_UPD.TRAN_ID = fINANCE_FEE.TRAN_ID;
                fINANCE_FEE_UPD.IS_PD = fINANCE_FEE.IS_PD;
                db.Entry(fINANCE_FEE_UPD).State = EntityState.Modified;

                ViewBag.FEE_CLCT_ID = new SelectList(db.FINANCE_FEE_COLLECTION, "ID", "NAME", fINANCE_FEE.FEE_CLCT_ID);
                ViewBag.TRAN_ID = new SelectList(db.FINANCE_TRANSACTION, "ID", "TIL", fINANCE_FEE.TRAN_ID);
                ViewBag.STDNT_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO", fINANCE_FEE.STDNT_ID);

                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View(fINANCE_FEE);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(fINANCE_FEE);
                }
                return RedirectToAction("Index");
            }

            return View(fINANCE_FEE);
        }

        // GET: Finance/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_FEE fINANCE_FEE = db.FINANCE_FEE.Find(id);
            if (fINANCE_FEE == null)
            {
                return HttpNotFound();
            }
            return View(fINANCE_FEE);
        }

        // POST: Finance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FINANCE_FEE fINANCE_FEE = db.FINANCE_FEE.Find(id);
            db.FINANCE_FEE.Remove(fINANCE_FEE);
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                    }
                }
                return View(fINANCE_FEE);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return View(fINANCE_FEE);
            }
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

        // GET: Student/Details/5
        public ActionResult Categories()
        {
            return View();
        }

        // GET: Student/Details/5
        [ChildActionOnly]
        public ActionResult _CategoriesList()
        {
            var fINANCEcATEGORY = db.FINANCE_TRANSACTION_CATEGORY.Where(d => d.DEL == false).ToList();
            return View(fINANCEcATEGORY);
        }

        // GET: Student/Delete/5
        public ActionResult _CategoriesDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_TRANSACTION_CATEGORY fINANCEcATEGORY = db.FINANCE_TRANSACTION_CATEGORY.Find(id);
            if (fINANCEcATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(fINANCEcATEGORY);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("_CategoriesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult _CategoriesDeleteConfirmed(int id)
        {
            FINANCE_TRANSACTION_CATEGORY fINANCEcATEGORY = db.FINANCE_TRANSACTION_CATEGORY.Find(id);
            fINANCEcATEGORY.DEL = true;
            db.Entry(fINANCEcATEGORY).State = EntityState.Modified;
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                    }
                }
                return View(fINANCEcATEGORY);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return View(fINANCEcATEGORY);
            }
            ViewBag.ErrorMessage = "Finance Category Deleted Sucessfully!";
            return View(fINANCEcATEGORY);
        }

        // GET: Student/Edit/5
        public ActionResult _CategoriesEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FINANCE_TRANSACTION_CATEGORY fINANCEcATEGORY = db.FINANCE_TRANSACTION_CATEGORY.Find(id);
            if (fINANCEcATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(fINANCEcATEGORY);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CategoriesEdit([Bind(Include = "ID,NAME,DESCR,IS_INCM,DEL")] FINANCE_TRANSACTION_CATEGORY fINANCEcATEGORY)
        {
            if (ModelState.IsValid)
            {
                FINANCE_TRANSACTION_CATEGORY fINANCEcATEGORY_UPD = db.FINANCE_TRANSACTION_CATEGORY.Find(fINANCEcATEGORY.ID);
                fINANCEcATEGORY_UPD.NAME = fINANCEcATEGORY.NAME;
                fINANCEcATEGORY_UPD.DESCR = fINANCEcATEGORY.DESCR;
                fINANCEcATEGORY_UPD.IS_INCM = fINANCEcATEGORY.IS_INCM;
                db.Entry(fINANCEcATEGORY_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View(fINANCEcATEGORY);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(fINANCEcATEGORY);
                }
                ViewBag.ErrorMessage = "Finance Category Edited Sucessfully!";
                return View(fINANCEcATEGORY);
            }
            ViewBag.ErrorMessage = "There seems to be some issue with view state.";
            return View(fINANCEcATEGORY);
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoriesCreate([Bind(Include = "ID,NAME,DESCR,IS_INCM,DEL")] FINANCE_TRANSACTION_CATEGORY fINANCEcATEGORY)
        {
            if (ModelState.IsValid)
            {
                fINANCEcATEGORY.DEL = false;
                db.FINANCE_TRANSACTION_CATEGORY.Add(fINANCEcATEGORY);
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View(fINANCEcATEGORY);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(fINANCEcATEGORY);
                }
                return RedirectToAction("Categories");
            }

            return View(fINANCEcATEGORY);
        }

        public ActionResult Fees_Defaulters_Index()
        {
            return View();
        }

        // GET: Student/Edit/5
        public ActionResult Fees_Defaulters_Batch()
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
            Session["student_serach"] = null;

            return View();
        }

        // GET: Fee Index
        public ActionResult Fees_Collection_Dates_Defaulters(int? id)
        {
            var queryCollections = (from cl in db.FINANCE_FEE_COLLECTION
                                    where cl.IS_DEL == false && cl.BTCH_ID == id
                                    select new { cl.ID, cl.NAME, cl.DUE_DATE })
              .OrderBy(x => x.NAME).Distinct().ToList();


            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in queryCollections)
            {
                string CollectionDate = string.Concat(item.NAME, "-", Convert.ToDateTime(item.DUE_DATE).ToString("dd-MMM-yy"));
                var result = new SelectListItem();
                result.Text = CollectionDate;
                result.Value = item.ID.ToString();
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Collection Date" });
            ViewBag.COLLECTION_ID = options2;
            ViewBag.STUDENT_ID = null;

            return PartialView("_Fees_Collection_Dates_Defaulters");
        }

        // GET: Fee Index
        public ActionResult fees_defaulters_students(int? id, int? student_id)
        {
            FINANCE_FEE_COLLECTION date = db.FINANCE_FEE_COLLECTION.Find(id);
            ViewData["date"] = date;
            FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(id);
            STUDENT student = db.STUDENTs.Find(student_id == null ? -1 : student_id);
            var batch_val = (from cs in db.COURSEs
                             join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                             where bt.ID == date.BTCH_ID
                             select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = batch_val;

            if (fee_collection != null)
            {
                var StudentVal = (from st in db.STUDENTs
                                  where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false
                                  select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                if (student != null)
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == student.ID);
                }
                ViewData["student"] = StudentVal;

                var StudentValDefaulters = (from ff in db.FINANCE_FEE
                                            join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                            join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                            where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false && ff.IS_PD == false && fc.ID == date.ID && fc.BTCH_ID == date.BTCH_ID
                                            select new StundentFee { StudentData = st, FeeCollectionData = fc }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                if (student != null)
                {
                    StudentValDefaulters = StudentValDefaulters.Where(x => x.StudentData.ID == student.ID);
                }
                ViewData["defaulters"] = StudentValDefaulters;

                var StudentGuardians = (from st in db.STUDENTs
                                            join gd in db.GUARDIANs on st.ID equals gd.WARD_ID
                                            where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false
                                            select new StudentsGuardians { StudentData = st, GuardianData = gd }).OrderBy(x => x.StudentData.ID).Distinct();
                if (student != null)
                {
                    StudentGuardians = StudentGuardians.Where(x => x.StudentData.ID == student.ID);
                }
                ViewData["guardians"] = StudentGuardians;

                var fee_particulars_val = (from fcol in db.FINANCE_FEE_COLLECTION
                                           join fc in db.FINANCE_FEE_CATGEORY on fcol.FEE_CAT_ID equals fc.ID
                                           join ff in db.FINANCE_FEE_PARTICULAR on fc.ID equals ff.FIN_FEE_CAT_ID
                                           where fcol.ID == id && fc.IS_DEL == false && ff.IS_DEL == "N"
                                           select new FeeParticular { FeeParticularData = ff, FeeCategoryData = fc, FeeCollectionData = fcol }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                ViewData["fee_particulars"] = fee_particulars_val;

                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     where ff.FEE_CLCT_ID == id
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.TYPE == "Batch"
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.TYPE == "Student"
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.TYPE == "Student Category"
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;



                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.TYPE == "Batch" && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.TYPE == "Student" && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                        select ff);
                ViewData["student_fine"] = student_fine_val;

                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.TYPE == "Student Category" && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                         select ff);
                ViewData["category_fine"] = category_fine_val;

            }
            else
            {
                ViewBag.FeeDefaultersMessage = "No students have been assigned this fee.";
            }

            return PartialView("_Student_Defaulters");
        }

        // GET: Student/Edit/5
        public ActionResult Fee_Defaulters_pdf(int? batch_id, int? date)
        {
            FINANCE_FEE_COLLECTION dateVal = db.FINANCE_FEE_COLLECTION.Find(date);
            ViewData["date"] = dateVal;
            FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(date);
            var batch_val = (from cs in db.COURSEs
                             join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                             where bt.ID == batch_id
                             select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = batch_val;

            if (fee_collection != null)
            {

                var StudentValDefaulters = (from ff in db.FINANCE_FEE
                                            join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                            join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                            where st.BTCH_ID == batch_id && ff.IS_PD ==false && fc.ID == date
                                            select new StundentFee { StudentData = st, FeeCollectionData = fc }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                ViewData["student"] = StudentValDefaulters;

                var StudentGuardians = (from st in db.STUDENTs
                                        join gd in db.GUARDIANs on st.ID equals gd.WARD_ID
                                        where st.BTCH_ID == batch_id && st.IS_DEL == false
                                        select new StudentsGuardians { StudentData = st, GuardianData = gd }).OrderBy(x => x.StudentData.ID).Distinct();
                ViewData["guardians"] = StudentGuardians;

                var fee_particulars_val = (from fcol in db.FINANCE_FEE_COLLECTION
                                           join fc in db.FINANCE_FEE_CATGEORY on fcol.FEE_CAT_ID equals fc.ID
                                           join ff in db.FINANCE_FEE_PARTICULAR on fc.ID equals ff.FIN_FEE_CAT_ID
                                           where fcol.ID == date && fc.IS_DEL == false && ff.IS_DEL == "N"
                                           select new FeeParticular { FeeParticularData = ff, FeeCategoryData = fc, FeeCollectionData = fcol }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                ViewData["fee_particulars"] = fee_particulars_val;

                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     where ff.FEE_CLCT_ID == date
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.TYPE == "Batch"
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.TYPE == "Student"
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.TYPE == "Student Category"
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;



                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.TYPE == "Batch" && (ff.FEE_CLCT_ID == dateVal.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= dateVal.DUE_DATE))
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.TYPE == "Student" && (ff.FEE_CLCT_ID == dateVal.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= dateVal.DUE_DATE))
                                        select ff);
                ViewData["student_fine"] = student_fine_val;

                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.TYPE == "Student Category" && (ff.FEE_CLCT_ID == dateVal.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= dateVal.DUE_DATE))
                                         select ff);
                ViewData["category_fine"] = category_fine_val;

            }
            else
            {
                ViewBag.FeeDefaulterspdfMessage = "No students have been assigned this fee.";
            }

            return View();
        }

        // GET: Student
        public ActionResult Aggregated_Fees_Due(string sortOrder, int? currentFilter, int? searchString, int? page)
        {
            int? MaxBatchId = db.BATCHes.Where(x=>x.IS_DEL == false && x.IS_ACT == true).Max(x => x.ID);
            if (searchString == null || searchString == -1) { currentFilter = MaxBatchId; searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentPage = page;

            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                        .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.BatchData.ID.ToString();
                result.Selected = item.BatchData.ID == searchString ? true : false;
                options.Add(result);
            }
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.searchString = options;
            return View(db.BATCHes.ToList());
        }

        // GET: Student
        public ActionResult ListDefaulterStudentsByCourse(string sortOrder, int? currentFilter, int? searchString, int? page)
        {
            ViewBag.CurrentDate = System.DateTime.Now;
            ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null && searchString != -1) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = page;


            var StudentValDefaulters = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                        where st.IS_DEL == false && ff.IS_PD == false && fc.IS_DEL == false && fc.START_DATE <= DateTime.Now 
                                        select new StundentFee { StudentData = st, FeeCollectionData = fc, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();

            if (searchString != null && searchString != -1)
            {
                StudentValDefaulters = StudentValDefaulters.Where(s => s.StudentData.BTCH_ID == searchString);
                StudentValDefaulters = StudentValDefaulters.Where(s => s.FeeCollectionData.BTCH_ID == searchString);
            }
            ViewData["defaulters"] = StudentValDefaulters;

            var fee_particulars_val = (from fcol in db.FINANCE_FEE_COLLECTION
                                       join fc in db.FINANCE_FEE_CATGEORY on fcol.FEE_CAT_ID equals fc.ID
                                       join ff in db.FINANCE_FEE on fcol.ID equals ff.FEE_CLCT_ID
                                       join ffp in db.FINANCE_FEE_PARTICULAR on fc.ID equals ffp.FIN_FEE_CAT_ID
                                       where fc.IS_DEL == false && ffp.IS_DEL == "N" && ff.IS_PD == false && fcol.IS_DEL == false && fcol.START_DATE <= DateTime.Now
                                       select new FeeParticular { FeeParticularData = ffp, FeeCategoryData = fc, FeeCollectionData = fcol }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
            if (searchString != null && searchString != -1)
            {
                fee_particulars_val = fee_particulars_val.Where(s => s.FeeCollectionData.BTCH_ID == searchString);
            }
            ViewData["fee_particulars"] = fee_particulars_val;

            var StudentGuardians = (from st in db.STUDENTs
                                    join gd in db.GUARDIANs on st.ID equals gd.WARD_ID
                                    where st.IS_DEL == false
                                    select new StudentsGuardians { StudentData = st, GuardianData = gd }).OrderBy(x => x.StudentData.ID).Distinct();

            if (searchString != null && searchString != -1)
            {
                StudentGuardians = StudentGuardians.Where(s => s.StudentData.BTCH_ID == searchString);
            }
            ViewData["guardians"] = StudentGuardians;

            var paid_fees_val = (from ff in db.FINANCE_FEE
                                 join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                 join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                 join fcol in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fcol.ID
                                 where ff.IS_PD == false && fcol.START_DATE <= DateTime.Now
                                 select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff, FeeCollectionData = fcol }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            if (searchString != null && searchString != -1)
            {
                paid_fees_val = paid_fees_val.Where(s => s.FeeCollectionData.BTCH_ID == searchString);
            }
            ViewData["paid_fees"] = paid_fees_val;

            var batch_discounts_val = (from ff in db.FEE_DISCOUNT.Include(x=>x.FINANCE_FEE_CATGEORY)
                                       where ff.TYPE == "Batch"
                                       select ff);
            ViewData["batch_discounts"] = batch_discounts_val;
            var student_discounts_val = (from ff in db.FEE_DISCOUNT.Include(x => x.FINANCE_FEE_CATGEORY)
                                         where ff.TYPE == "Student"
                                         select ff);
            ViewData["student_discounts"] = student_discounts_val;
            var category_discounts_val = (from ff in db.FEE_DISCOUNT.Include(x => x.FINANCE_FEE_CATGEORY)
                                          where ff.TYPE == "Student Category"
                                          select ff);
            ViewData["category_discounts"] = category_discounts_val;



            var batch_fine_val = (from ff in db.FEE_FINE
                                  where ff.TYPE == "Batch" && (ff.FEE_CLCT_ID != null || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= System.DateTime.Now))
                                  select ff);
            ViewData["batch_fine"] = batch_fine_val;
            var student_fine_val = (from ff in db.FEE_FINE
                                    where ff.TYPE == "Student" && (ff.FEE_CLCT_ID != null || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= System.DateTime.Now))
                                    select ff);
            ViewData["student_fine"] = student_fine_val;

            var category_fine_val = (from ff in db.FEE_FINE
                                     where ff.TYPE == "Student Category" && (ff.FEE_CLCT_ID != null || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= System.DateTime.Now))
                                     select ff);
            ViewData["category_fine"] = category_fine_val;

            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID
                            join cs in db.COURSEs on b.CRS_ID equals cs.ID
                            where st.IS_DEL == false
                            orderby st.LAST_NAME, b.NAME
                            select new Student { StudentData = st, BatcheData = b, CourseData = cs}).Distinct();

            if (searchString != null && searchString != -1)
            {
                StudentS = StudentS.Where(s => s.BatcheData.ID == searchString);
            }
            switch (sortOrder)
            {
                case "Name":
                    StudentS = StudentS.OrderBy(s => s.StudentData.FIRST_NAME).ThenBy(s => s.StudentData.MID_NAME).ThenBy(s => s.StudentData.LAST_NAME);
                    break;
                case "name_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.FIRST_NAME).ThenByDescending(s => s.StudentData.MID_NAME).ThenByDescending(s => s.StudentData.LAST_NAME);
                    break;
                case "Date":
                    StudentS = StudentS.OrderBy(s => s.StudentData.CLS_ROLL_NO);
                    break;
                case "date_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.CLS_ROLL_NO);
                    break;
                default:  // Name ascending 
                    StudentS = StudentS.OrderBy(s => s.StudentData.CLS_ROLL_NO);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(StudentS.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Edit/5
        public ActionResult Aggregated_Fees_Due_pdf(string sortOrder, int? batch_id, int? page)
        {
            BATCH BatchVal = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == batch_id).FirstOrDefault();
            ViewBag.Batch = string.Concat(BatchVal.COURSE.CRS_NAME, " - ", BatchVal.NAME);

            var StudentValDefaulters = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                        where st.IS_DEL == false && ff.IS_PD == false && fc.IS_DEL == false && fc.START_DATE <= DateTime.Now
                                        select new StundentFee { StudentData = st, FeeCollectionData = fc, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();

            if (batch_id != null && batch_id != -1)
            {
                StudentValDefaulters = StudentValDefaulters.Where(s => s.StudentData.BTCH_ID == batch_id);
                StudentValDefaulters = StudentValDefaulters.Where(s => s.FeeCollectionData.BTCH_ID == batch_id);
            }
            ViewData["defaulters"] = StudentValDefaulters;

            var fee_particulars_val = (from fcol in db.FINANCE_FEE_COLLECTION
                                       join fc in db.FINANCE_FEE_CATGEORY on fcol.FEE_CAT_ID equals fc.ID
                                       join ff in db.FINANCE_FEE on fcol.ID equals ff.FEE_CLCT_ID
                                       join ffp in db.FINANCE_FEE_PARTICULAR on fc.ID equals ffp.FIN_FEE_CAT_ID
                                       where fc.IS_DEL == false && ffp.IS_DEL == "N" && ff.IS_PD == false && fcol.IS_DEL == false && fcol.START_DATE <= DateTime.Now
                                       select new FeeParticular { FeeParticularData = ffp, FeeCategoryData = fc, FeeCollectionData = fcol }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
            if (batch_id != null && batch_id != -1)
            {
                fee_particulars_val = fee_particulars_val.Where(s => s.FeeCollectionData.BTCH_ID == batch_id);
            }
            ViewData["fee_particulars"] = fee_particulars_val;

            var StudentGuardians = (from st in db.STUDENTs
                                    join gd in db.GUARDIANs on st.ID equals gd.WARD_ID
                                    where st.IS_DEL == false
                                    select new StudentsGuardians { StudentData = st, GuardianData = gd }).OrderBy(x => x.StudentData.ID).Distinct();

            if (batch_id != null && batch_id != -1)
            {
                StudentGuardians = StudentGuardians.Where(s => s.StudentData.BTCH_ID == batch_id);
            }
            ViewData["guardians"] = StudentGuardians;

            var paid_fees_val = (from ff in db.FINANCE_FEE
                                 join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                 join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                 join fcol in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fcol.ID
                                 where ff.IS_PD == false && fcol.START_DATE <= DateTime.Now
                                 select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff, FeeCollectionData = fcol }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            if (batch_id != null && batch_id != -1)
            {
                paid_fees_val = paid_fees_val.Where(s => s.FeeCollectionData.BTCH_ID == batch_id);
            }
            ViewData["paid_fees"] = paid_fees_val;

            var batch_discounts_val = (from ff in db.FEE_DISCOUNT.Include(x => x.FINANCE_FEE_CATGEORY)
                                       where ff.TYPE == "Batch"
                                       select ff);
            ViewData["batch_discounts"] = batch_discounts_val;
            var student_discounts_val = (from ff in db.FEE_DISCOUNT.Include(x => x.FINANCE_FEE_CATGEORY)
                                         where ff.TYPE == "Student"
                                         select ff);
            ViewData["student_discounts"] = student_discounts_val;
            var category_discounts_val = (from ff in db.FEE_DISCOUNT.Include(x => x.FINANCE_FEE_CATGEORY)
                                          where ff.TYPE == "Student Category"
                                          select ff);
            ViewData["category_discounts"] = category_discounts_val;



            var batch_fine_val = (from ff in db.FEE_FINE
                                  where ff.TYPE == "Batch" && (ff.FEE_CLCT_ID != null || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= System.DateTime.Now))
                                  select ff);
            ViewData["batch_fine"] = batch_fine_val;
            var student_fine_val = (from ff in db.FEE_FINE
                                    where ff.TYPE == "Student" && (ff.FEE_CLCT_ID != null || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= System.DateTime.Now))
                                    select ff);
            ViewData["student_fine"] = student_fine_val;

            var category_fine_val = (from ff in db.FEE_FINE
                                     where ff.TYPE == "Student Category" && (ff.FEE_CLCT_ID != null || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= System.DateTime.Now))
                                     select ff);
            ViewData["category_fine"] = category_fine_val;

            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID
                            join cs in db.COURSEs on b.CRS_ID equals cs.ID
                            where st.IS_DEL == false
                            orderby st.LAST_NAME, b.NAME
                            select new Student { StudentData = st, BatcheData = b, CourseData = cs }).Distinct();

            if (batch_id != null && batch_id != -1)
            {
                StudentS = StudentS.Where(s => s.BatcheData.ID == batch_id);
            }
            switch (sortOrder)
            {
                case "Name":
                    StudentS = StudentS.OrderBy(s => s.StudentData.FIRST_NAME).ThenBy(s => s.StudentData.MID_NAME).ThenBy(s => s.StudentData.LAST_NAME);
                    break;
                case "name_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.FIRST_NAME).ThenByDescending(s => s.StudentData.MID_NAME).ThenByDescending(s => s.StudentData.LAST_NAME);
                    break;
                case "Date":
                    StudentS = StudentS.OrderBy(s => s.StudentData.CLS_ROLL_NO);
                    break;
                case "date_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.CLS_ROLL_NO);
                    break;
                default:  // Name ascending 
                    StudentS = StudentS.OrderBy(s => s.StudentData.CLS_ROLL_NO);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(StudentS.ToPagedList(pageNumber, pageSize));
        }

        // GET: Fee Index
        public ActionResult Fees_Defaulters_Student_Search()
        {

            var StudentVal = (from st in db.STUDENTs
                              where st.IS_DEL == false && st.IS_ACT == true
                              select st).OrderBy(x => x.ID).Distinct();
            return View(StudentVal.ToList());

        }
  
        // GET: Fee Index
        public ActionResult Students_Defaulted_Fees(string id)
        {
            var StudentVal = (from st in db.STUDENTs
                              where st.ADMSN_NO == id
                              select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();

            var dateVal = (from ff in db.FINANCE_FEE_COLLECTION
                           where ff.BTCH_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID
                           select ff).ToList();

            ViewData["date"] = dateVal;
            var batch_val = (from cs in db.COURSEs
                             join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                             where bt.ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID
                             select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = batch_val;

            if (dateVal != null)
            {
                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                     where st.ID == StudentVal.FirstOrDefault().Student_data.ID
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff, FeeCollectionData = fc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                ViewData["student"] = StudentVal;

                var StudentValDefaulters = (from ff in db.FINANCE_FEE
                                            join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                            join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                            where st.ID == StudentVal.FirstOrDefault().Student_data.ID && ff.IS_PD == false && fc.BTCH_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID
                                            select new StundentFee { StudentData = st, FeeCollectionData = fc }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                ViewData["defaulters"] = StudentValDefaulters;
                var fee_particulars_val = (from fcol in db.FINANCE_FEE_COLLECTION
                                           join fc in db.FINANCE_FEE_CATGEORY on fcol.FEE_CAT_ID equals fc.ID
                                           join ff in db.FINANCE_FEE_PARTICULAR on fc.ID equals ff.FIN_FEE_CAT_ID
                                           where fcol.BTCH_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && fc.IS_DEL == false && ff.IS_DEL == "N"
                                           select new FeeParticular { FeeParticularData = ff, FeeCategoryData = fc, FeeCollectionData = fcol }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID 
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;

                

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID 
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID
                                        select ff);
                ViewData["student_fine"] = student_fine_val;

                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID
                                         select ff);
                ViewData["category_fine"] = category_fine_val;

            }
            else
            {
                ViewBag.FeeDefaultersMessage = "No students have been assigned this fee.";
            }

            return PartialView("_Students_Defaulted_Fees");
        }

        // GET: Fee Index
        public ActionResult Fees_Student_Structure_Search()
        {
            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID
                            join cs in db.COURSEs on b.CRS_ID equals cs.ID
                            where st.IS_DEL == false
                            orderby st.LAST_NAME, b.NAME
                            select new Student { StudentData = st, BatcheData = b, CourseData = cs }).Distinct();

            return View(StudentS.ToList());

        }

        public ActionResult Fees_Structure_Dates( int? id)
        {
            var StudentVal = (from st in db.STUDENTs
                              where st.ID == id
                              select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();

            if (StudentVal != null && StudentVal.Count() != 0)
            {
                STUDENT student = db.STUDENTs.Find(StudentVal.FirstOrDefault().Student_data.ID);
                ViewData["student"] = student;

                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == student.BTCH_ID
                                 select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;

                var queryCollections = (from cl in db.FINANCE_FEE_COLLECTION
                                        join ff in db.FINANCE_FEE_PARTICULAR on cl.FEE_CAT_ID equals ff.FIN_FEE_CAT_ID
                                        where cl.IS_DEL == false && cl.BTCH_ID == student.BTCH_ID && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                        select new { cl.ID, cl.NAME, cl.DUE_DATE })
             .OrderBy(x => x.NAME).Distinct().ToList();

                List<SelectListItem> options2 = new List<SelectListItem>();
                foreach (var item in queryCollections)
                {
                    string CollectionDate = string.Concat(item.NAME, "-", Convert.ToDateTime(item.DUE_DATE).ToString("dd-MMM-yy"));
                    var result = new SelectListItem();
                    result.Text = CollectionDate;
                    result.Value = item.ID.ToString();
                    options2.Add(result);
                }
                // add the 'ALL' option
                options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Collection Date" });
                ViewBag.COLL_ID = options2;
                Session["student_serach"] = "Y";
            }
            else
            {
                ViewBag.FeeCollectionMessage = "No Stundet found with this Admission Number.";
            }

            return View();

        }

        public ActionResult Fees_Structure_For_Student(int? id, int? batch_id, int? student_id)
        {
            DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
            ViewBag.ReturnDate = PDate.ToShortDateString();
            FINANCE_FEE_COLLECTION date = db.FINANCE_FEE_COLLECTION.Find(id);
            ViewData["date"] = date;
            FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(id);
            STUDENT student = db.STUDENTs.Find(student_id == null ? -1 : student_id);
            //ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME");
            var batch_val = (from cs in db.COURSEs
                             join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                             where bt.ID == date.BTCH_ID
                             select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = batch_val;
            var fee = (from ff in db.FINANCE_FEE
                       join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                       where ff.FEE_CLCT_ID == fee_collection.ID
                       select new { Finance_Fee_Data = ff, Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
            if (student != null)
            {
                fee = fee.Where(x => x.Student_data.ID == student.ID);
            }

            if (fee_collection != null)
            {
                var StudentVal = (from st in db.STUDENTs
                                  where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false
                                  select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                int StudentValId = StudentVal.FirstOrDefault().Student_data.ID;
                if (student != null)
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == student.ID);
                    ViewData["student"] = db.STUDENTs.Where(x => x.ID == student.ID).FirstOrDefault();
                }
                else
                {
                    StudentVal = StudentVal.Where(x => x.Student_data.ID == StudentValId);
                    ViewData["student"] = db.STUDENTs.Where(x => x.ID == StudentValId).FirstOrDefault();
                }
                STUDENT_CATGEORY StudentCategoryVal = db.STUDENT_CATGEORY.Find(StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID);
                ViewBag.StudentCategory = StudentCategoryVal.NAME;
                var prev_student_val = (from st in db.STUDENTs
                                        where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false
                                        select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                int prev_student_id = -1;
                if (StudentVal != null)
                {
                    prev_student_val = prev_student_val.Where(x => x.Student_data.ID < StudentVal.FirstOrDefault().Student_data.ID);
                    if (prev_student_val != null && prev_student_val.Count() != 0)
                    {
                        prev_student_id = prev_student_val.Max(x => x.Student_data.ID);
                    }
                }
                STUDENT prev_student = db.STUDENTs.Find(prev_student_id);
                ViewData["prev_student"] = prev_student;
                ViewBag.prev_student_id = prev_student_id;

                var next_student_val = (from st in db.STUDENTs
                                        where st.BTCH_ID == date.BTCH_ID && st.IS_DEL == false
                                        select new { Student_data = st }).OrderBy(x => x.Student_data.ID).Distinct();
                int next_student_id = -1;
                if (StudentVal != null)
                {
                    next_student_val = next_student_val.Where(x => x.Student_data.ID > StudentVal.FirstOrDefault().Student_data.ID);
                    if (next_student_val != null && next_student_val.Count() != 0)
                    {
                        next_student_id = next_student_val.Min(x => x.Student_data.ID);
                    }
                }
                STUDENT next_student = db.STUDENTs.Find(next_student_id);
                ViewData["next_student"] = next_student;
                ViewBag.next_student_id = next_student_id;

                var financefeeVal = (from ff in db.FINANCE_FEE
                                     where ff.FEE_CLCT_ID == fee_collection.ID && ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID
                                     select ff).ToList();

                ViewData["financefee"] = financefeeVal;
                ViewBag.due_date = fee_collection.DUE_DATE;

                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     where ff.FEE_CLCT_ID == fee_collection.ID && st.ID == StudentVal.FirstOrDefault().Student_data.ID
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
                ViewData["fee_category"] = fee_category;

                var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                           where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.IS_DEL == "N" && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                           select ff).ToList();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date.DUE_DATE))
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date.DUE_DATE))
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date.DUE_DATE))
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;

                decimal total_discount_val = 0;
                decimal total_payable = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_payable = total_payable + (decimal)item.AMT;
                }
                ViewBag.total_payable = total_payable;
                if (batch_discounts_val != null && batch_discounts_val.Count() != 0)
                {
                    foreach (var item in batch_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (student_discounts_val != null && student_discounts_val.Count() != 0)
                {
                    foreach (var item in student_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (category_discounts_val != null && category_discounts_val.Count() != 0)
                {
                    foreach (var item in category_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.BTCH_ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                        select ff);
                ViewData["student_fine"] = student_fine_val;

                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date.DUE_DATE))
                                         select ff);
                ViewData["category_fine"] = category_fine_val;
                decimal total_fine_val = 0;
                if (batch_fine_val != null && batch_fine_val.Count() != 0)
                {
                    foreach (var item in batch_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }

                }
                if (student_fine_val != null && student_fine_val.Count() != 0)
                {
                    foreach (var item in student_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (category_fine_val != null && category_fine_val.Count() != 0)
                {
                    foreach (var item in category_fine_val)
                    {
                        total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                ViewBag.fine = 0;
                ViewBag.total_fine = total_fine_val;

                if (total_discount_val > total_payable + total_fine_val && total_payable + total_fine_val >= 0)
                {
                    total_discount_val = total_payable + total_fine_val;
                }
                ViewBag.total_discount = total_discount_val;
                decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
                ViewBag.total_discount_percentage = total_discount_percentage_val;
            }
            else
            {
                ViewBag.FeeCollectionMessage = "No students have been assigned this fee.";
            }

            return PartialView("_Fees_Structure");

        }

        // GET: Finance/Delete/5
        [HttpGet]
        public ActionResult pdf_Fee_Structure(int? id, int? id2)
        {
            FINANCE_FEE_COLLECTION date_val = db.FINANCE_FEE_COLLECTION.Find(id2);
            ViewData["date"] = date_val;
            FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(id2);
            var StudentVal = db.STUDENTs.Find(id);
            ViewData["student"] = StudentVal;

            var financefeeVal = (from ff in db.FINANCE_FEE
                                 where ff.FEE_CLCT_ID == fee_collection.ID && ff.STDNT_ID == id
                                 select ff).ToList();
            ViewData["financefee"] = financefeeVal;

            ViewBag.due_date = fee_collection.DUE_DATE;
            var paid_fees_val = (from ff in db.FINANCE_FEE
                                 join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                 join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                 where ff.FEE_CLCT_ID == fee_collection.ID && st.ID == StudentVal.ID
                                 select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewData["paid_fees"] = paid_fees_val;

            FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
            ViewData["fee_category"] = fee_category;

            var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                       where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.IS_DEL == "N" && (ff.STDNT_ID == StudentVal.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                       select ff).ToList();
            ViewData["fee_particulars"] = fee_particulars_val;

            var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                       where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                       select ff);
            ViewData["batch_discounts"] = batch_discounts_val;
            var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                         select ff);
            ViewData["student_discounts"] = student_discounts_val;
            var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                          where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= date_val.DUE_DATE))
                                          select ff);
            ViewData["category_discounts"] = category_discounts_val;
            decimal total_discount_val = 0;
            decimal total_payable = 0;
            foreach (var item in fee_particulars_val)
            {
                total_payable = total_payable + (decimal)item.AMT;
            }
            ViewBag.total_payable = total_payable;
            if (batch_discounts_val != null && batch_discounts_val.Count() != 0)
            {
                foreach (var item in batch_discounts_val)
                {
                    total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }
            }
            if (student_discounts_val != null && student_discounts_val.Count() != 0)
            {
                foreach (var item in student_discounts_val)
                {
                    total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }
            }
            if (category_discounts_val != null && category_discounts_val.Count() != 0)
            {
                foreach (var item in category_discounts_val)
                {
                    total_discount_val += item.IS_AMT == true ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }
            }

            var batch_fine_val = (from ff in db.FEE_FINE
                                  where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentVal.BTCH_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                  select ff);
            ViewData["batch_fine"] = batch_fine_val;
            var student_fine_val = (from ff in db.FEE_FINE
                                    where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                    select ff);
            ViewData["student_fine"] = student_fine_val;
            var category_fine_val = (from ff in db.FEE_FINE
                                     where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.STDNT_CAT_ID && (ff.FEE_CLCT_ID == date_val.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= date_val.DUE_DATE))
                                     select ff);
            ViewData["category_fine"] = category_fine_val;
            decimal total_fine_val = 0;
            if (batch_fine_val != null && batch_fine_val.Count() != 0)
            {
                foreach (var item in batch_fine_val)
                {
                    total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            if (student_fine_val != null && student_fine_val.Count() != 0)
            {
                foreach (var item in student_fine_val)
                {
                    total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            if (category_fine_val != null && category_fine_val.Count() != 0)
            {
                foreach (var item in category_fine_val)
                {
                    total_fine_val += item.IS_AMT == true ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            ViewBag.total_fine = total_fine_val;

            if (total_discount_val > total_payable + total_fine_val && total_payable + total_fine_val >= 0)
            {
                total_discount_val = total_payable + total_fine_val;
            }
            ViewBag.total_discount = total_discount_val;
            decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
            ViewBag.total_discount_percentage = total_discount_percentage_val;

            decimal total_fees = 0;
            foreach (var item in fee_particulars_val)
            {
                total_fees += (decimal)item.AMT;
            }
            if (total_fine_val != 0)
            {
                total_fees += total_fine_val;
            }
            return View();
        }

        // GET: Finance
        public ActionResult Transactions()
        {
            return View();
        }

        // GET: Finance
        public ActionResult Expense_Create(string TransactionTitle)
        {
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                              select new FinanceTransaction {FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewBag.TransactionTitle = TransactionTitle;

            string[] search = { "Fees", "Salary", "Donation" };
            List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x=>x.DEL == false && x.IS_INCM == false && !search.Any(val => x.NAME.Equals(val))).OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
            ViewBag.CAT_ID = options;
            return View();
        }

 
        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction_Create([Bind(Include = "ID,MSTRTRAN_ID,TIL,DESCR,AMT,FINE_INCLD,CAT_ID,STDNT_ID,FIN_FE_ID,CRETAED_AT,UPDATED_AT,TRAN_DATE,FINE_AMT,FIN_ID,FIN_TYPE,PAYEE_ID,PAYEE_TYPE,RCPT_NO,VCHR_NO")] FINANCE_TRANSACTION fINANCEtRANSACTIONS, string PageName)
        {
            if(PageName == "Expense_Create")
            {
                string[] search = { "Fees", "Salary", "Donation" };
                List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.DEL == false && x.IS_INCM == false && !search.Any(val => x.NAME.Equals(val))).OrderBy(x => x.ID), "ID", "NAME").ToList();
                options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
                ViewBag.CAT_ID = options;
            }
            else if (PageName == "Income_Create")
            {
                string[] search = { "Fees", "Salary", "Donation" };
                List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.DEL == false && x.IS_INCM == true && !search.Any(val => x.NAME.Equals(val))).OrderBy(x => x.ID), "ID", "NAME").ToList();
                options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
                ViewBag.CAT_ID = options;
            }
            else
            {
                List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.DEL == false).OrderBy(x => x.ID), "ID", "NAME").ToList();
                options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
                ViewBag.CAT_ID = options;
            }
            ViewBag.PageName = PageName;
            if (ModelState.IsValid)
            {
                fINANCEtRANSACTIONS.CRETAED_AT = System.DateTime.Now;
                fINANCEtRANSACTIONS.UPDATED_AT = System.DateTime.Now;
                db.FINANCE_TRANSACTION.Add(fINANCEtRANSACTIONS);
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View(PageName);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(PageName);
                }
                ViewBag.ErrorMessage = string.Concat("Transaction Added Successfully. Receipt No. - ", fINANCEtRANSACTIONS.ID);
                return View(PageName);
            }

            ViewBag.ErrorMessage = "There seems to be some issue with Model State.";
            return View(PageName);
        }

        // GET: Finance
        public ActionResult Income_Create(string TransactionTitle)
        {
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewBag.TransactionTitle = TransactionTitle;
            string[] search = { "Fees", "Salary", "Donation" };
            List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.DEL == false && x.IS_INCM ==true && !search.Any(val => x.NAME.Equals(val))).OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
            ViewBag.CAT_ID = options;
            return View();
        }

        // GET: Finance
        public ActionResult Expense_List(string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            return View();
        }

        // GET: Finance
        public ActionResult Expense_List_Update(DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            if (END_TRAN_DATE < START_TRAN_DATE)
            {
                ViewBag.ErrorNotice = "End Date should be greater than or equal to Start Date!";
                return View();
            }
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE && tc.IS_INCM == false
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;
            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Expense_List_pdf(DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            if (END_TRAN_DATE < START_TRAN_DATE)
            {
                ViewBag.ErrorNotice = "End Date should be greater than or equal to Start Date!";
                return View();
            }
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE && tc.IS_INCM == true
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewBag.START_TRAN_DATE = START_TRAN_DATE.ToShortDateString();
            ViewBag.END_TRAN_DATE = END_TRAN_DATE.ToShortDateString();
            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Delete_Transaction(int? id)
        {
            FINANCE_TRANSACTION TransToDelete = db.FINANCE_TRANSACTION.Find(id);
            FINANCE_TRANSACTION_CATEGORY TranCatToDelete = db.FINANCE_TRANSACTION_CATEGORY.Find(TransToDelete.CAT_ID);
            bool? IncomeType = TranCatToDelete.IS_INCM;
            if (IncomeType == true)
            {
                var DependentTrans = (from tr in db.FINANCE_TRANSACTION
                                      where tr.MSTRTRAN_ID == TransToDelete.ID
                                      select tr).Distinct().ToList();
                foreach (var item in DependentTrans)
                {
                    FINANCE_TRANSACTION DepTransToDelete = db.FINANCE_TRANSACTION.Find(item.ID);
                    db.FINANCE_TRANSACTION.Remove(DepTransToDelete);
                    try { db.SaveChanges(); }
                    catch (Exception e)
                    {
                        ViewData["Warn_Notice"] = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                        return RedirectToAction("Expense_List_Update", "Finance", new ViewDataDictionary { { "Warn_Notice", ViewData["Warn_Notice"] } });
                    }
                }
                ViewBag.ErrorMessage = string.Concat("All Dependent Transactions Removed from System!");
            }
            db.FINANCE_TRANSACTION.Remove(TransToDelete);
            try { db.SaveChanges(); }
            catch (Exception e)
            {
                ViewData["Warn_Notice"] = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                if (IncomeType == true)
                {
                    return RedirectToAction("Income_List", "Finance", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                else
                {
                    return RedirectToAction("Expense_List", "Finance", new { ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            ViewBag.ErrorMessage = string.Concat("Selected Transactions Removed from System Successfully!");
            if(IncomeType == true)
            {
                return RedirectToAction("Income_List", "Finance", new { ErrorMessage = ViewBag.ErrorMessage });
            }
            else
            {
                return RedirectToAction("Expense_List", "Finance", new {ErrorMessage = ViewBag.ErrorMessage });
            }
        }

        // GET: Finance
        public ActionResult Expense_Edit(int? id, string ErrorMessage )
        {
            FINANCE_TRANSACTION TransToEdit = db.FINANCE_TRANSACTION.Find(id);
            FINANCE_TRANSACTION_CATEGORY TranCatToEdit = db.FINANCE_TRANSACTION_CATEGORY.Find(TransToEdit.CAT_ID);
            List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x =>x.IS_INCM == false).OrderBy(x => x.ID), "ID", "NAME", TransToEdit.CAT_ID).ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
            ViewData["CAT_ID"] = options;
            ViewData["SpecialCategory"] = null;
            if (TranCatToEdit.NAME == "Salary")
            { ViewData["SpecialCategory"] = "Y"; }
            ViewBag.ErrorMessage = ErrorMessage;
            return View(TransToEdit);
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction_Edit([Bind(Include = "ID,MSTRTRAN_ID,TIL,DESCR,AMT,FINE_INCLD,CAT_ID,STDNT_ID,FIN_FE_ID,CRETAED_AT,UPDATED_AT,TRAN_DATE,FINE_AMT,FIN_ID,FIN_TYPE,PAYEE_ID,PAYEE_TYPE,RCPT_NO,VCHR_NO")] FINANCE_TRANSACTION fINANCEtRANSACTIONS, string PageName)
        {
            ViewBag.PageName = PageName;
            FINANCE_TRANSACTION fINANCEtRANSACTIONS_UPD = db.FINANCE_TRANSACTION.Find(fINANCEtRANSACTIONS.ID);
            fINANCEtRANSACTIONS_UPD.TIL = fINANCEtRANSACTIONS.TIL;
            fINANCEtRANSACTIONS_UPD.DESCR = fINANCEtRANSACTIONS.DESCR;
            fINANCEtRANSACTIONS_UPD.AMT = fINANCEtRANSACTIONS.AMT;
            fINANCEtRANSACTIONS_UPD.VCHR_NO = fINANCEtRANSACTIONS.VCHR_NO;
            fINANCEtRANSACTIONS_UPD.TRAN_DATE = fINANCEtRANSACTIONS.TRAN_DATE;
            fINANCEtRANSACTIONS_UPD.CAT_ID = fINANCEtRANSACTIONS.CAT_ID;
            fINANCEtRANSACTIONS_UPD.UPDATED_AT = System.DateTime.Now;
            db.Entry(fINANCEtRANSACTIONS_UPD).State = EntityState.Modified;
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                    }
                }
                return RedirectToAction(PageName, "Finance", new { id = fINANCEtRANSACTIONS.ID, ErrorMessage = ViewBag.ErrorMessage });
                //return View(PageName);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return RedirectToAction(PageName, "Finance", new { id = fINANCEtRANSACTIONS.ID, ErrorMessage = ViewBag.ErrorMessage });
                //return View(PageName);
            }
            ViewBag.ErrorMessage = "Transaction Updated Successfully!";
            return RedirectToAction(PageName, "Finance", new { id = fINANCEtRANSACTIONS.ID, ErrorMessage = ViewBag.ErrorMessage });
            //return View(PageName);
        }

        // GET: Finance
        public ActionResult Income_List(string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            return View();
        }

        // GET: Finance
        public ActionResult Income_List_Update(string sortOrder, DateTime START_TRAN_DATE, DateTime END_TRAN_DATE, string ADMSN_NO)
        {
            if (END_TRAN_DATE < START_TRAN_DATE)
            {
                ViewBag.ErrorNotice = "End Date should be greater than or equal to Start Date!";
                return View();
            }
            ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  join ff in db.FINANCE_FEE on tr.FIN_FE_ID equals ff.ID into gff
                                  from subgff in gff.DefaultIfEmpty()
                                  join std in db.STUDENTs on subgff.STDNT_ID equals std.ID into gstd
                                  from subgstd in gstd.DefaultIfEmpty()
                                  join bt in db.BATCHes.Include(x => x.COURSE) on subgstd.BTCH_ID equals bt.ID into gbt
                                  from subgbt in gbt.DefaultIfEmpty()
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE && tc.IS_INCM == true
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc, FinanceFeeData = (subgff == null ? null : subgff), StudentData = (subgstd == null ? null : subgstd), BatchData = (subgbt == null ? null : subgbt) }).Distinct();
            if (!string.IsNullOrEmpty(ADMSN_NO))
            {
                TransactionVal = TransactionVal.Where(x => x.StudentData.ADMSN_NO == ADMSN_NO);
            }

            switch (sortOrder)
            {
                case "Name":
                    TransactionVal = TransactionVal.OrderBy(s => s.BatchData.ID);
                    break;
                case "name_desc":
                    TransactionVal = TransactionVal.OrderByDescending(s => s.BatchData.ID);
                    break;
                /*case "Date":
                    TransactionVal = TransactionVal.OrderBy(s => s.FinanceTransactionData.TRAN_DATE);
                    break;
                    */
                case "date_desc":
                    TransactionVal = TransactionVal.OrderByDescending(s => s.FinanceTransactionData.TRAN_DATE);
                    break;
                default:  // Name ascending 
                    TransactionVal = TransactionVal.OrderBy(s => s.FinanceTransactionData.TRAN_DATE);
                    break;
            }

            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;
            ViewBag.ADMSN_NO = ADMSN_NO;
            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Income_Edit(int? id, string ErrorMessage)
        {
            FINANCE_TRANSACTION TransToEdit = db.FINANCE_TRANSACTION.Find(id);
            FINANCE_TRANSACTION_CATEGORY TranCatToEdit = db.FINANCE_TRANSACTION_CATEGORY.Find(TransToEdit.CAT_ID);
            List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.IS_INCM == true).OrderBy(x => x.ID), "ID", "NAME", TransToEdit.CAT_ID).ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
            ViewData["CAT_ID"] = options;
            ViewData["SpecialCategory"] = null;
            if (TranCatToEdit.NAME == "Fees" || TranCatToEdit.NAME == "Salary" || TranCatToEdit.NAME == "Donation")
            { ViewData["SpecialCategory"] = "Y"; }
            ViewBag.ErrorMessage = ErrorMessage;
            return View(TransToEdit);
        }

        // GET: Finance
        public ActionResult Income_List_pdf(DateTime START_TRAN_DATE, DateTime END_TRAN_DATE, string ADMSN_NO)
        {
            if (END_TRAN_DATE < START_TRAN_DATE)
            {
                ViewBag.ErrorNotice = "End Date should be greater than or equal to Start Date!";
                return View();
            }
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  join ff in db.FINANCE_FEE on tr.FIN_FE_ID equals ff.ID into gff
                                  from subgff in gff.DefaultIfEmpty()
                                  join std in db.STUDENTs on subgff.STDNT_ID equals std.ID into gstd
                                  from subgstd in gstd.DefaultIfEmpty()
                                  join bt in db.BATCHes.Include(x => x.COURSE) on subgstd.BTCH_ID equals bt.ID into gbt
                                  from subgbt in gbt.DefaultIfEmpty()
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE && tc.IS_INCM == true
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc, FinanceFeeData = (subgff == null ? null : subgff), StudentData = (subgstd == null ? null : subgstd), BatchData = (subgbt == null ? null : subgbt) }).OrderBy(x => x.BatchData.ID).ThenBy(x => x.StudentData.ID).ThenBy(x => x.FinanceTransactionData.TRAN_DATE).Distinct();

            if (!string.IsNullOrEmpty(ADMSN_NO))
            {
                TransactionVal = TransactionVal.Where(x => x.StudentData.ADMSN_NO == ADMSN_NO);
            }

            ViewBag.START_TRAN_DATE = START_TRAN_DATE.ToShortDateString();
            ViewBag.END_TRAN_DATE = END_TRAN_DATE.ToShortDateString();
            ViewBag.ADMSN_NO = ADMSN_NO;
            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Monthly_Report()
        {
            return View();
        }


        public ActionResult Update_Monthly_Report(DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            if (END_TRAN_DATE < START_TRAN_DATE)
            {
                ViewBag.ErrorNotice = "End Date should be greater than or equal to Start Date!";
                return View();
            }
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewData["privilege"] = userdetails.privilage_list.ToList();

            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();

            ViewData["transactions"] = TransactionVal;
            ViewData["start_date"] = START_TRAN_DATE;
            ViewData["end_date"] = END_TRAN_DATE;

            ViewBag.hr = null;
            var configValue = (from C in db.CONFIGURATIONs
                               select C).Distinct();
            foreach (var item in configValue)
            {
                if(item.CONFIG_KEY == "HR")
                {
                    ViewBag.hr = item.CONFIG_VAL;
                }
            }
            decimal transactions_fees_income = 0;
            decimal transactions_fees_expense = 0;
            decimal transactions_fees = 0;
            foreach (var item in TransactionVal)
            {
                if(item.TransactionCategoryData.NAME.Contains("Fees"))
                {
                    if(item.TransactionCategoryData.IS_INCM == true)
                    {
                        transactions_fees_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        transactions_fees_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            transactions_fees = transactions_fees_income - transactions_fees_expense;
            ViewBag.transactions_fees = transactions_fees;

            decimal salary = 0;
            decimal salary_income = 0;
            decimal salary_expense = 0;
            foreach (var item in TransactionVal)
            {
                if (item.TransactionCategoryData.NAME.Contains("Salary"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        salary_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        salary_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            salary = salary_expense - salary_income;
            ViewBag.salary = salary;

            decimal donations_total = 0;
            decimal donations_income = 0;
            decimal donations_expense = 0;
            foreach (var item in TransactionVal)
            {
                if (item.TransactionCategoryData.NAME.Contains("Donation"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        donations_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        donations_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            donations_total = donations_income - donations_expense;
            ViewBag.donations_total = donations_total;

            var TotalTrans = db.FINANCE_TRANSACTION.Where(x=>x.TRAN_DATE >= START_TRAN_DATE && x.TRAN_DATE <= END_TRAN_DATE).GroupBy(o => o.CAT_ID)
                .Select(g => new { membername = g.Key, total = g.Sum(p => p.AMT) });


            string[] search = { "Fees", "Salary", "Donation" };

            var CatTrans = (from ct in db.FINANCE_TRANSACTION_CATEGORY
                            join tt in TotalTrans on ct.ID equals tt.membername
                            where !search.Any(val => ct.NAME.Equals(val))
                            orderby ct.NAME
                            select new CategoryTransactions { TransactionCategoryData = ct, TRANS_AMNT = tt.total }).Distinct();

            ViewData["other_transaction_categories"] = CatTrans;

            ViewBag.graph = null;
            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Transaction_pdf(DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            if (END_TRAN_DATE < START_TRAN_DATE)
            {
                ViewBag.ErrorNotice = "End Date should be greater than or equal to Start Date!";
                return View();
            }
            var userdetails = this.Session["CurrentUser"] as UserDetails;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewData["privilege"] = userdetails.privilage_list.ToList();

            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();

            ViewData["transactions"] = TransactionVal;
            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;

            /*int grand_total = 0;
            foreach (var item in TransactionVal)
            {
                grand_total = grand_total + Convert.ToInt32(item.FinanceTransactionData.AMT);
            }

            ViewBag.grand_total = grand_total;*/

            ViewBag.hr = null;
            var configValue = (from C in db.CONFIGURATIONs
                               select C).Distinct();
            foreach (var item in configValue)
            {
                if (item.CONFIG_KEY == "HR")
                {
                    ViewBag.hr = item.CONFIG_VAL;
                }
            }
            decimal transactions_fees_income = 0;
            decimal transactions_fees_expense = 0;
            decimal transactions_fees = 0;
            foreach (var item in TransactionVal)
            {
                if (item.TransactionCategoryData.NAME.Contains("Fees"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        transactions_fees_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        transactions_fees_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            transactions_fees = transactions_fees_income - transactions_fees_expense;
            ViewBag.transactions_fees = transactions_fees;

            decimal salary = 0;
            decimal salary_income = 0;
            decimal salary_expense = 0;
            foreach (var item in TransactionVal)
            {
                if (item.TransactionCategoryData.NAME.Contains("Salary"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        salary_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        salary_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            salary = salary_expense - salary_income;
            ViewBag.salary = salary;

            decimal donations_total = 0;
            decimal donations_income = 0;
            decimal donations_expense = 0;
            foreach (var item in TransactionVal)
            {
                if (item.TransactionCategoryData.NAME.Contains("Donation"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        donations_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        donations_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            donations_total = donations_income - donations_expense;
            ViewBag.donations_total = donations_total;

            var TotalTrans = db.FINANCE_TRANSACTION.Where(x => x.TRAN_DATE >= START_TRAN_DATE && x.TRAN_DATE <= END_TRAN_DATE).GroupBy(o => o.CAT_ID)
                .Select(g => new { membername = g.Key, total = g.Sum(p => p.AMT) });


            string[] search = { "Fees", "Salary", "Donation" };

            var CatTrans = (from ct in db.FINANCE_TRANSACTION_CATEGORY
                            join tt in TotalTrans on ct.ID equals tt.membername
                            where !search.Any(val => ct.NAME.Equals(val))
                            orderby ct.NAME
                            select new CategoryTransactions { TransactionCategoryData = ct, TRANS_AMNT = tt.total }).Distinct();

            ViewData["other_transaction_categories"] = CatTrans;

            ViewBag.graph = null;
            return View();
        }

        // GET: Finance
        public ActionResult Salary_Department(DateTime start_date, DateTime end_date)
        {
            ViewData["start_date"] = start_date;
            ViewData["end_date"] = end_date;
            ViewData["departments"] = db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).ToList();
            return View();
        }
        public ActionResult Salary_Employee(DateTime start, DateTime end, int? id)
        {
            ViewData["start_date"] = start;
            ViewData["end_date"] = end;
            EMPLOYEE_DEPARTMENT department = db.EMPLOYEE_DEPARTMENT.Find(id);
            ViewData["department"] = department;
            ViewData["employees"] = department.EMPLOYEEs.ToList();
            ViewData["payslips"] = db.MONTHLY_PAYSLIP.FirstOrDefault().Total_Employees_Salary(start, end, id); 
            return View();
        }
        public ActionResult Employee_Payslip_Monthly_Report(DateTime? id2, int? id)
        {
            ViewData["salary_date"] = id2;
            ActiveOrArchiveEmployee employee = db.EMPLOYEEs.FirstOrDefault().Find_In_Active_Or_Archived((int)id);
            ViewData["employee"] = employee;
            var currency_type = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CurrencyType").FirstOrDefault().CONFIG_VAL;
            ViewBag.currency_type = currency_type;
            if (id2 == null)
            {
                return RedirectToAction("View_Employee_Payslip", new { id = id, salary_date = id2 });
            }
            var monthly_payslips = db.MONTHLY_PAYSLIP.Include(x => x.PAYROLL_CATEGORY).Where(x => x.EMP_ID == id && x.SAL_DATE == id2).ToList();
            ViewData["monthly_payslips"] = monthly_payslips;
            var individual_payslips = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.SAL_DATE == id2).ToList();
            ViewData["individual_payslips"] = individual_payslips;
            ViewData["salary"] = db.EMPLOYEEs.FirstOrDefault().Calculate_Salary(monthly_payslips, individual_payslips);
            return View();
        }

        // GET: Finance
        public ActionResult Donations_Report(DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE && tc.NAME.Contains("Donation")
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();

            decimal donations_income = 0; decimal donations_expenses = 0;
            foreach (var item in TransactionVal)
            {
                if (item.FinanceTransactionData.MSTRTRAN_ID == null)
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        donations_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        donations_expenses += (decimal)item.FinanceTransactionData.AMT;
                    }
                }
                else
                {
                    donations_expenses += (decimal)item.FinanceTransactionData.AMT;

                }
            }
            ViewBag.donations_income = donations_income;
            ViewBag.donations_expenses = donations_expenses;
            decimal donations_total = donations_income - donations_expenses;
            ViewBag.donations_total = donations_total;
            return View(TransactionVal.ToList());
        }


        // GET: Finance
        public ActionResult Fees_Report(DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;
            var TransactionVal = (from ff in db.FINANCE_FEE
                                   join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                   join tc in db.FINANCE_TRANSACTION_CATEGORY on ft.CAT_ID equals tc.ID
                                   join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                   where ft.TRAN_DATE >= START_TRAN_DATE && ft.TRAN_DATE <= END_TRAN_DATE && tc.NAME.Equals("Fees")
                                   select new FeeTransaction { FinanceTransactionData = ft,FinanceFeeData = ff, FeeCollectionData=fc, TransactionCategoryData= tc })
                                   .GroupBy(o => o.FeeCollectionData.ID)
                                   .Select(g => new { membername = g.Key, total = g.Sum(p => p.FinanceTransactionData.AMT) });

            var CollectionTrans = (from fc in db.FINANCE_FEE_COLLECTION
                                   join bt in db.BATCHes on fc.BTCH_ID equals bt.ID
                                   join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                   join tt in TransactionVal on fc.ID equals tt.membername
                            orderby fc.NAME
                            select new FeeCollectionTransactions { FeeCollectionData = fc, BatchData = bt, CourseData=cs, TRANS_AMNT = tt.total }).Distinct();

            ViewData["fee_collection"] = CollectionTrans;

            return View();
        }

        // GET: Finance
        public ActionResult Batch_Fees_Report(int? id, DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;
            var TransactionVal = (from ff in db.FINANCE_FEE
                                  join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                  join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on ft.CAT_ID equals tc.ID
                                  join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                  join bt in db.BATCHes on fc.BTCH_ID equals bt.ID
                                  join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                  where ft.TRAN_DATE >= START_TRAN_DATE && ft.TRAN_DATE <= END_TRAN_DATE && tc.NAME.Equals("Fees") && fc.ID == id
                                  select new FeeTransaction { FinanceTransactionData = ft, StudentData = st ,FinanceFeeData = ff, FeeCollectionData = fc, TransactionCategoryData = tc, BatchData = bt, CourseData = cs }).Distinct();

            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Income_Details(int? id, DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE && tc.ID == id
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();

            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Income_Details_pdf(int? id, DateTime START_TRAN_DATE, DateTime END_TRAN_DATE)
        {
            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE && tc.ID == id
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();

            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Compare_Report()
        {
           
            return View();
        }

        // GET: Finance
        public ActionResult Report_Compare(DateTime START_TRAN_DATE, DateTime END_TRAN_DATE, DateTime START_TRAN_DATE2, DateTime END_TRAN_DATE2)
        {

            if (END_TRAN_DATE < START_TRAN_DATE)
            {
                ViewBag.ErrorMessage = "End Date should be greater than or equal to Start Date!";
                return View();
            }
            if (END_TRAN_DATE2 < START_TRAN_DATE2)
            {
                ViewBag.ErrorMessage = "End Date should be greater than or equal to Start Date!";
                return View();
            }
            ViewBag.START_TRAN_DATE = START_TRAN_DATE;
            ViewBag.END_TRAN_DATE = END_TRAN_DATE;
            ViewBag.START_TRAN_DATE2 = START_TRAN_DATE2;
            ViewBag.END_TRAN_DATE2 = END_TRAN_DATE2;

            ViewBag.hr = null;
            var configValue = (from C in db.CONFIGURATIONs
                               select C).Distinct();
            foreach (var item in configValue)
            {
                if (item.CONFIG_KEY == "HR")
                {
                    ViewBag.hr = item.CONFIG_VAL;
                }
            }
            var TransactionVal = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE && tr.TRAN_DATE <= END_TRAN_DATE
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();

            ViewData["transactions"] = TransactionVal;

            var TransactionVal2 = (from tr in db.FINANCE_TRANSACTION
                                  join tc in db.FINANCE_TRANSACTION_CATEGORY on tr.CAT_ID equals tc.ID
                                  where tr.TRAN_DATE >= START_TRAN_DATE2 && tr.TRAN_DATE <= END_TRAN_DATE2
                                  select new FinanceTransaction { FinanceTransactionData = tr, TransactionCategoryData = tc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();

            ViewData["transactions"] = TransactionVal2;

            decimal transactions_fees_income = 0;
            decimal transactions_fees_expense = 0;
            decimal transactions_fees = 0;
            foreach (var item in TransactionVal)
            {
                if (item.TransactionCategoryData.NAME.Equals("Fees"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        transactions_fees_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        transactions_fees_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            transactions_fees = transactions_fees_income - transactions_fees_expense;
            ViewBag.transactions_fees = transactions_fees;

            decimal transactions_fees_income2 = 0;
            decimal transactions_fees_expense2 = 0;
            decimal transactions_fees2 = 0;
            foreach (var item in TransactionVal2)
            {
                if (item.TransactionCategoryData.NAME.Equals("Fees"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        transactions_fees_income2 += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        transactions_fees_expense2 += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            transactions_fees2 = transactions_fees_income2 - transactions_fees_expense2;
            ViewBag.transactions_fees2 = transactions_fees2;

            decimal salary = 0;
            decimal salary_income = 0;
            decimal salary_expense = 0;
            foreach (var item in TransactionVal)
            {
                if (item.TransactionCategoryData.NAME.Contains("Salary"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        salary_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        salary_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            salary = salary_expense - salary_income;
            ViewBag.salary = salary;

            decimal salary2 = 0;
            decimal salary_income2 = 0;
            decimal salary_expense2 = 0;
            foreach (var item in TransactionVal2)
            {
                if (item.TransactionCategoryData.NAME.Contains("Salary"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        salary_income2 += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        salary_expense2 += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            salary2 = salary_expense2 - salary_income2;
            ViewBag.salary2 = salary2;

            decimal donations_total = 0;
            decimal donations_income = 0;
            decimal donations_expense = 0;
            foreach (var item in TransactionVal)
            {
                if (item.TransactionCategoryData.NAME.Contains("Donation"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        donations_income += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        donations_expense += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            donations_total = donations_income - donations_expense;
            ViewBag.donations_total = donations_total;

            decimal donations_total2 = 0;
            decimal donations_income2 = 0;
            decimal donations_expense2 = 0;
            foreach (var item in TransactionVal2)
            {
                if (item.TransactionCategoryData.NAME.Contains("Donation"))
                {
                    if (item.TransactionCategoryData.IS_INCM == true)
                    {
                        donations_income2 += (decimal)item.FinanceTransactionData.AMT;
                    }
                    else
                    {
                        donations_expense2 += (decimal)item.FinanceTransactionData.AMT;
                    }
                }

            }
            donations_total2 = donations_income2 - donations_expense2;
            ViewBag.donations_total2 = donations_total2;

            var TotalTrans = db.FINANCE_TRANSACTION.Where(x => x.TRAN_DATE >= START_TRAN_DATE && x.TRAN_DATE <= END_TRAN_DATE).GroupBy(o => o.CAT_ID)
                .Select(g => new { membername = g.Key, total = g.Sum(p => p.AMT) });


            string[] search = { "Fees", "Salary", "Donation" };

            var CatTrans = (from ct in db.FINANCE_TRANSACTION_CATEGORY
                            join tt in TotalTrans on ct.ID equals tt.membername
                            where !search.Any(val => ct.NAME.Equals(val))
                            orderby ct.NAME
                            select new CategoryTransactions { TransactionCategoryData = ct, TRANS_AMNT = tt.total }).Distinct();

            ViewData["other_transaction_categories"] = CatTrans;

            var TotalTrans2 = db.FINANCE_TRANSACTION.Where(x => x.TRAN_DATE >= START_TRAN_DATE2 && x.TRAN_DATE <= END_TRAN_DATE2).GroupBy(o => o.CAT_ID)
                .Select(g => new { membername = g.Key, total = g.Sum(p => p.AMT) });


            string[] search2 = { "Fees", "Salary", "Donation" };

            var CatTrans2 = (from ct in db.FINANCE_TRANSACTION_CATEGORY
                            join tt in TotalTrans2 on ct.ID equals tt.membername
                            where !search2.Any(val => ct.NAME.Equals(val))
                            orderby ct.NAME
                            select new CategoryTransactions { TransactionCategoryData = ct, TRANS_AMNT = tt.total }).Distinct();

            ViewData["other_transaction_categories2"] = CatTrans2;

            ViewBag.graph = null;
            return View(TransactionVal.ToList());
        }

        // GET: Finance
        public ActionResult Donation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Donation([Bind(Include = "ID,DNR,DESCR,AMT,TRAN_ID,CREATED_AT,UPDATED_AT,TRAN_DATE")] FINANCE_DONATION fINANCEdONATION)
        {
            if (ModelState.IsValid)
            {
                var TransactionCatVal = (from tc in db.FINANCE_TRANSACTION_CATEGORY
                                      where tc.NAME== "Donation" && tc.DEL == false
                                      select tc).Distinct();

                var transaction = new FINANCE_TRANSACTION()
                {
                    TIL = fINANCEdONATION.DNR,
                    CAT_ID = TransactionCatVal.FirstOrDefault().ID,
                    DESCR = fINANCEdONATION.DESCR,
                    PAYEE_ID = null,
                    PAYEE_TYPE = null,
                    AMT = (decimal)fINANCEdONATION.AMT,
                    FINE_AMT = null,
                    FINE_INCLD = false,
                    FIN_FE_ID = null,
                    MSTRTRAN_ID = null,
                    RCPT_NO = null,
                    TRAN_DATE = fINANCEdONATION.TRAN_DATE,
                    CRETAED_AT = System.DateTime.Now,
                    UPDATED_AT = System.DateTime.Now
                };
                db.FINANCE_TRANSACTION.Add(transaction);
                fINANCEdONATION.CREATED_AT = System.DateTime.Now;
                fINANCEdONATION.UPDATED_AT = System.DateTime.Now;
                fINANCEdONATION.TRAN_ID = transaction.ID;
                db.FINANCE_DONATION.Add(fINANCEdONATION);
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View(fINANCEdONATION);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(fINANCEdONATION);
                }
                ViewBag.Notice = "Donation added in system sucessfully!";
                return RedirectToAction("Donation_Receipt", new { id = fINANCEdONATION .ID, Notice = ViewBag.Notice});
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State. Please try again later.";
            return View(fINANCEdONATION);
        }

        // GET: Finance
        public ActionResult Donation_Receipt(int? id, string Notice)
        {
            FINANCE_DONATION donation = db.FINANCE_DONATION.Find(id);
            ViewBag.Notice = Notice;
            return View(donation);
        }
        // GET: Finance
        public ActionResult Donation_Receipt_pdf(int? id)
        {
            FINANCE_DONATION donation = db.FINANCE_DONATION.Find(id);
            return View(donation);
        }
        // GET: Finance
        public ActionResult Donors(string Notice)
        {
            ViewBag.Notice = Notice;
            var donation = (from dn in db.FINANCE_DONATION
                            select dn).OrderBy(x=>x.TRAN_DATE).Distinct();
            return View(donation.ToList());
        }
        // GET: Finance
        public ActionResult Donation_Edit(int? id)
        {
            FINANCE_DONATION donor = db.FINANCE_DONATION.Find(id);
            return View(donor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Donation_Edit([Bind(Include = "ID,DNR,DESCR,AMT,TRAN_ID,CREATED_AT,UPDATED_AT,TRAN_DATE")] FINANCE_DONATION fINANCEdONATION)
        {
            if (ModelState.IsValid)
            {
                FINANCE_TRANSACTION tRANSACTION_tO_uPDATE = db.FINANCE_TRANSACTION.Find(fINANCEdONATION.TRAN_ID);
                tRANSACTION_tO_uPDATE.TIL = fINANCEdONATION.DNR;
                tRANSACTION_tO_uPDATE.DESCR = fINANCEdONATION.DESCR;
                tRANSACTION_tO_uPDATE.AMT = fINANCEdONATION.AMT;
                tRANSACTION_tO_uPDATE.TRAN_DATE = fINANCEdONATION.TRAN_DATE;
                tRANSACTION_tO_uPDATE.UPDATED_AT = System.DateTime.Now;
                db.Entry(tRANSACTION_tO_uPDATE).State = EntityState.Modified;

                FINANCE_DONATION fINANCEdONATION_tO_uPDATE = db.FINANCE_DONATION.Find(fINANCEdONATION.ID);
                fINANCEdONATION_tO_uPDATE.DNR = fINANCEdONATION.DNR;
                fINANCEdONATION_tO_uPDATE.DESCR = fINANCEdONATION.DESCR;
                fINANCEdONATION_tO_uPDATE.AMT = fINANCEdONATION.AMT;
                fINANCEdONATION_tO_uPDATE.TRAN_DATE = fINANCEdONATION.TRAN_DATE;
                fINANCEdONATION_tO_uPDATE.UPDATED_AT = System.DateTime.Now;
                db.Entry(fINANCEdONATION_tO_uPDATE).State = EntityState.Modified;


                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return View(fINANCEdONATION);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return View(fINANCEdONATION);
                }
                ViewBag.Notice = "Donation updated in system sucessfully!";
                return RedirectToAction("Donors", new { Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State. Please try again later.";
            return View(fINANCEdONATION);
        }

        // GET: Finance
        public ActionResult Donation_Delete(int? id)
        {
            var donation = db.FINANCE_DONATION.Find(id);
            var transaction = db.FINANCE_TRANSACTION.Find(donation.TRAN_ID);
            db.FINANCE_TRANSACTION.Remove(transaction);
            try { db.SaveChanges(); }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return View(donation);
            }
            db.FINANCE_DONATION.Remove(donation);
            try { db.SaveChanges(); }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return View(donation);
            }
            ViewBag.Notice = "Donation deleted from system sucessfully!";
            return RedirectToAction("Donors", new {Notice = ViewBag.Notice });
        }

        // GET: Finance
        public ActionResult Automatic_Transactions(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            string[] search = { "Fees", "Salary"};
            List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.DEL == false && x.IS_INCM == false && !search.Any(val => x.NAME.Equals(val))).OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
            ViewBag.FIN_CAT_ID = options;
            var TansactionTrigger = (from tt in db.FINANCE_TRANSACTION_TRIGGERS
                                     join tc in db.FINANCE_TRANSACTION_CATEGORY on tt.FIN_CAT_ID equals tc.ID
                                     select new TransactionTriggers { TransactionTriggerData = tt, TransactionCategoryData = tc}).OrderBy(x=>x.TransactionTriggerData.TIL).Distinct();
            return View(TansactionTrigger.ToList());
        }

        // GET: Finance
        public ActionResult Transaction_Trigger_Create_Form()
        {
            string[] search = { "Fees", "Salary" };
            List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.DEL == false && x.IS_INCM == false && !search.Any(val => x.NAME.Equals(val))).OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
            ViewBag.FIN_CAT_ID = options;
            return PartialView("_Transaction_Trigger_Create_Form");
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transaction_Trigger_Create([Bind(Include = "ID,FIN_CAT_ID,PCT,TIL,DESCR")] FINANCE_TRANSACTION_TRIGGERS fINANCEtRANStRIGGER)
        {
            string[] search = { "Fees", "Salary" };
            List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.DEL == false && x.IS_INCM == false && !search.Any(val => x.NAME.Equals(val))).OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
            ViewBag.FIN_CAT_ID = options;

            if (ModelState.IsValid)
            {
                db.FINANCE_TRANSACTION_TRIGGERS.Add(fINANCEtRANStRIGGER);
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return RedirectToAction("Automatic_Transactions", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return RedirectToAction("Automatic_Transactions", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                ViewBag.Notice = "Transaction Trigger added in system sucessfully!";
                return RedirectToAction("Automatic_Transactions", new { Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State. Please try again later.";
            return RedirectToAction("Automatic_Transactions", new { ErrorMessage = ViewBag.ErrorMessage });
        }

        // GET: Finance
        public ActionResult Transaction_Trigger_Edit(int? id, string ErrorMessage)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            FINANCE_TRANSACTION_TRIGGERS TansactionTrigger = db.FINANCE_TRANSACTION_TRIGGERS.Find(id);
            string[] search = { "Fees", "Salary" };
            List<SelectListItem> options = new SelectList(db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.DEL == false && x.IS_INCM == false && !search.Any(val => x.NAME.Equals(val))).OrderBy(x => x.ID), "ID", "NAME", TansactionTrigger.FIN_CAT_ID).ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Transaction Category" });
            ViewBag.FIN_CAT_ID = options;
            /*var TansactionTrigger = (from tt in db.FINANCE_TRANSACTION_TRIGGERS
                                     join tc in db.FINANCE_TRANSACTION_CATEGORY on tt.FIN_CAT_ID equals tc.ID
                                     select new TransactionTriggers { TransactionTriggerData = tt, TransactionCategoryData = tc }).OrderBy(x => x.TransactionTriggerData.TIL).Distinct();*/
            return View(TansactionTrigger);
        }

        // GET: Finance
        public ActionResult Transaction_Trigger_Update([Bind(Include = "ID,FIN_CAT_ID,PCT,TIL,DESCR")] FINANCE_TRANSACTION_TRIGGERS fINANCEtRANStRIGGER)
        {
            if (ModelState.IsValid)
            {
                FINANCE_TRANSACTION_TRIGGERS fINANCEtRANStRIGGER_tO_uPDATE = db.FINANCE_TRANSACTION_TRIGGERS.Find(fINANCEtRANStRIGGER.ID);
                fINANCEtRANStRIGGER_tO_uPDATE.TIL = fINANCEtRANStRIGGER.TIL;
                fINANCEtRANStRIGGER_tO_uPDATE.FIN_CAT_ID = fINANCEtRANStRIGGER.FIN_CAT_ID;
                fINANCEtRANStRIGGER_tO_uPDATE.PCT = fINANCEtRANStRIGGER.PCT;
                fINANCEtRANStRIGGER_tO_uPDATE.DESCR = fINANCEtRANStRIGGER.DESCR;
                db.Entry(fINANCEtRANStRIGGER_tO_uPDATE).State = EntityState.Modified;
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    return RedirectToAction("Transaction_Trigger_Edit", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    return RedirectToAction("Transaction_Trigger_Edit", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                ViewBag.Notice = "Transaction Trigger updated in system sucessfully!";
                return RedirectToAction("Automatic_Transactions", new { Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State. Please try again later.";
            return RedirectToAction("Transaction_Trigger_Edit", new { ErrorMessage = ViewBag.ErrorMessage });
        }


        // GET: Finance
        public ActionResult Transaction_Trigger_Delete(int? id)
        {
            var transaction_trigger = db.FINANCE_TRANSACTION_TRIGGERS.Find(id);
            db.FINANCE_TRANSACTION_TRIGGERS.Remove(transaction_trigger);
            try { db.SaveChanges(); }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                return RedirectToAction("Automatic_Transactions", new { ErrorMessage = ViewBag.ErrorMessage });
            }
            ViewBag.Notice = "Donation deleted from system sucessfully!";
            return RedirectToAction("Automatic_Transactions", new { Notice = ViewBag.Notice });
        }

        // GET: Finance
        public ActionResult Payslip_Index(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            return View();
        }

        public ActionResult View_Monthly_Payslip(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;

            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).OrderBy(x=>x.NAMES).Distinct(), "ID", "NAMES").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.departments = options;
            ViewBag.IsPost = false;
            var PSL_Sal_date = (from psl in db.MONTHLY_PAYSLIP
                                select new { Salary_date = psl.SAL_DATE }).Distinct()
                        .OrderBy(x => x.Salary_date).ToList();


            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in PSL_Sal_date)
            {
                string sala_Month_Year = string.Concat(((DateTime)item.Salary_date.Value).ToString("MMMM"), "-", item.Salary_date.Value.Year);
                var result = new SelectListItem();
                result.Text = sala_Month_Year;
                result.Value = item.Salary_date.ToString();
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Salary Date" });
            ViewBag.Salary_Date = options2;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult View_Monthly_Payslip(int? departments, DateTime? Salary_Date, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.IsPost = true;
            ViewBag.Selected_Salary_Date = Salary_Date;
            List<SelectListItem> options = new SelectList(db.EMPLOYEE_DEPARTMENT.Where(x => x.STAT == true).Distinct(), "ID", "NAMES", departments).ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Department" });
            ViewBag.departments = options;
            var PSL_Sal_date = (from psl in db.MONTHLY_PAYSLIP
                                select new { Salary_date = psl.SAL_DATE }).Distinct()
                        .OrderBy(x => x.Salary_date).ToList();


            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in PSL_Sal_date)
            {
                string sala_Month_Year = string.Concat(((DateTime)item.Salary_date.Value).ToString("MMMM"), "-", item.Salary_date.Value.Year);
                var result = new SelectListItem();
                result.Text = sala_Month_Year;
                result.Value = item.Salary_date.ToString();
                result.Selected = item.Salary_date == Salary_Date ? true : false;
                options2.Add(result);
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Salary Date" });
            ViewBag.Salary_Date = options2;

            if (departments != null || Salary_Date != null)
            {
                if (departments != null && Salary_Date != null)
                {
                    var monthly_payslips = (from ps in db.MONTHLY_PAYSLIP
                                            join pc in db.PAYROLL_CATEGORY on ps.PYRL_CAT_ID equals pc.ID
                                            where ps.SAL_DATE == Salary_Date
                                            select new MonthyPayslip { MonthlyPayslipData = ps, PayrollCatogaryData = pc });

                    var grouped_monthly_payslips = from mps in monthly_payslips
                                                   group mps by mps.MonthlyPayslipData.EMP_ID into g
                                                   select new
                                                   {
                                                       Employee_ID = g.Key,
                                                       Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                                       Monthy_Payslip_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT),
                                                       Salare_Date = g.Max(x => x.MonthlyPayslipData.SAL_DATE)
                                                   };

                    var approved_grouped_monthly_payslips = from mps in monthly_payslips
                                                            where mps.MonthlyPayslipData.IS_APPR == true
                                                            group mps by mps.MonthlyPayslipData.EMP_ID into g
                                                            select new
                                                            {
                                                                Employee_ID = g.Key,
                                                                Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                                                Aapproved_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT),
                                                                Salare_Date = g.Max(x => x.MonthlyPayslipData.SAL_DATE)
                                                            };

                    var grouped_individual_payslip_categories = from ps in db.INDIVIDUAL_PAYSLIP_CATGEORY
                                                                where ps.SAL_DATE == Salary_Date
                                                                group ps by ps.EMP_ID into g
                                                                select new
                                                                {
                                                                    Employee_ID = g.Key,
                                                                    Individual_Pyaslip_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : -x.AMT),
                                                                    Salare_Date = g.Max(x => x.SAL_DATE)
                                                                };

                    var payslips = (from emp in db.EMPLOYEEs
                                    join gmp in grouped_monthly_payslips on emp.ID equals gmp.Employee_ID
                                    join gamp in approved_grouped_monthly_payslips on emp.ID equals gamp.Employee_ID into ggamp
                                    from subggamp in ggamp.DefaultIfEmpty()
                                    join gipc in grouped_individual_payslip_categories on emp.ID equals gipc.Employee_ID into ggipc
                                    from subgipc in ggipc.DefaultIfEmpty()
                                    where emp.EMP_DEPT_ID == departments
                                    select new SFSAcademy.Payslip { EmployeeData = emp, SAL_DATE = gmp.Salare_Date, Monthy_Payslip_Amount = gmp.Monthy_Payslip_Amount, Status = gmp.Status, Aapproved_Amount = (subggamp == null ? null : subggamp.Aapproved_Amount), Individual_Pyaslip_Amount = (subgipc == null ? null : subgipc.Individual_Pyaslip_Amount), Net_Amount = (subgipc == null ? gmp.Monthy_Payslip_Amount : gmp.Monthy_Payslip_Amount + subgipc.Individual_Pyaslip_Amount) }).Distinct();
                    ViewData["payslips"] = payslips.ToList();

                    return View();
                }
                else
                {
                    ViewBag.Notice = "Select salary date";
                    return RedirectToAction("Department_Payslip", new { Notice = ViewBag.Notice });
                }
            }
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult View_Employee_Payslip(int? id, DateTime? salary_date, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Salary_Date = salary_date;
            ViewBag.Selected_Salary_Date = salary_date.Value.ToShortDateString();
            EMPLOYEE Employee = db.EMPLOYEEs.Find(id);
            bool is_present_employee = true;
            if(Employee == null)
            {
                is_present_employee = false;
            }
            ViewBag.is_present_employee = is_present_employee;
            ViewBag.currency_type = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "CurrencyType").Select(x => x.CONFIG_VAL).FirstOrDefault().ToString();
            var monthly_payslips = (from emp in db.EMPLOYEEs
                                    join mp in db.MONTHLY_PAYSLIP on emp.ID equals mp.EMP_ID
                                    join pc in db.PAYROLL_CATEGORY on mp.PYRL_CAT_ID equals pc.ID
                                    where mp.SAL_DATE == salary_date && emp.ID == id
                                    select new MonthyPayslip { EmployeeData = emp, MonthlyPayslipData = mp, PayrollCatogaryData = pc }).OrderBy(x => x.PayrollCatogaryData.NAME).ToList();
            ViewData["monthly_payslips"] = monthly_payslips;
            ViewBag.Status = monthly_payslips.FirstOrDefault().MonthlyPayslipData.IS_APPR == true ? "Approved" : (monthly_payslips.FirstOrDefault().MonthlyPayslipData.IS_RJCT == true ? "Rejected" : null);
            var individual_payslips = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.SAL_DATE == salary_date).ToList();
            ViewData["individual_payslips"] = individual_payslips;

            var monthly_payslips_val = (from ps in db.MONTHLY_PAYSLIP
                                        join pc in db.PAYROLL_CATEGORY on ps.PYRL_CAT_ID equals pc.ID
                                        where ps.SAL_DATE == salary_date
                                        select new MonthyPayslip { MonthlyPayslipData = ps, PayrollCatogaryData = pc });

            var grouped_monthly_payslips = from mps in monthly_payslips_val
                                           group mps by mps.MonthlyPayslipData.EMP_ID into g
                                           select new
                                           {
                                               Employee_ID = g.Key,
                                               Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                               Monthy_Payslip_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT),
                                               Non_Deductionable_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : 0),
                                               Deductionable_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == true ? x.MonthlyPayslipData.AMT : 0),
                                               Salare_Date = g.Max(x => x.MonthlyPayslipData.SAL_DATE)
                                           };

            var approved_grouped_monthly_payslips = from mps in monthly_payslips_val
                                                    where mps.MonthlyPayslipData.IS_APPR == true
                                                    group mps by mps.MonthlyPayslipData.EMP_ID into g
                                                    select new
                                                    {
                                                        Employee_ID = g.Key,
                                                        Status = (from ps2 in g select ps2.MonthlyPayslipData.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.MonthlyPayslipData.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                                        Aapproved_Amount = g.Sum(x => x.PayrollCatogaryData.IS_DED == false ? x.MonthlyPayslipData.AMT : -x.MonthlyPayslipData.AMT)
                                                    };

            var grouped_individual_payslip_categories = from ps in db.INDIVIDUAL_PAYSLIP_CATGEORY
                                                        where ps.SAL_DATE == salary_date
                                                        group ps by ps.EMP_ID into g
                                                        select new
                                                        {
                                                            Employee_ID = g.Key,
                                                            Individual_Pyaslip_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : -x.AMT),
                                                            Non_Deductionable_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : 0),
                                                            Deductionable_Amount = g.Sum(x => x.IS_DED == true ? x.AMT : 0)
                                                        };

            var payslips = (from emp in db.EMPLOYEEs
                            join gmp in grouped_monthly_payslips on emp.ID equals gmp.Employee_ID
                            join gamp in approved_grouped_monthly_payslips on emp.ID equals gamp.Employee_ID into ggamp
                            from subggamp in ggamp.DefaultIfEmpty()
                            join gipc in grouped_individual_payslip_categories on emp.ID equals gipc.Employee_ID into ggipc
                            from subgipc in ggipc.DefaultIfEmpty()
                            where emp.ID == id
                            select new SFSAcademy.Payslip { EmployeeData = emp, SAL_DATE = gmp.Salare_Date, Monthy_Payslip_Amount = gmp.Monthy_Payslip_Amount, Status = gmp.Status, Aapproved_Amount = (subggamp == null ? null : subggamp.Aapproved_Amount), Individual_Pyaslip_Amount = (subgipc == null ? null : subgipc.Individual_Pyaslip_Amount), Net_Amount = (subgipc == null ? gmp.Monthy_Payslip_Amount : gmp.Monthy_Payslip_Amount + subgipc.Individual_Pyaslip_Amount), Net_Non_Deductionable_Amount = (subgipc == null ? gmp.Non_Deductionable_Amount : gmp.Non_Deductionable_Amount + subgipc.Non_Deductionable_Amount), Net_Deductionable_Amount = (subgipc == null ? gmp.Deductionable_Amount : gmp.Deductionable_Amount + subgipc.Deductionable_Amount) }).Distinct();

            ViewData["salary"] = payslips;

            return View(Employee);
        }
        public ActionResult Employee_Payslip_Accept_Form(int? id, DateTime? id2)
        {
            ViewBag.id = id;
            ViewBag.id2 = id2;

            return PartialView("_Accept_Form");
        }
        public ActionResult Employee_Payslip_Reject_Form(int? id, DateTime? id2)
        {
            ViewBag.id = id;
            ViewBag.id2 = id2;

            return PartialView("_Reject_Form");
        }
        public ActionResult Employee_Payslip_Approve(int? id, int Sal_Year, int Sal_Month, int Sal_Day, string RMRK)
        {
            DateTime SalaryDate = new DateTime(Sal_Year, Sal_Month, Sal_Day);
            var dates = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == id && x.SAL_DATE == SalaryDate).ToList();
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            foreach (var d in dates)
            {
                d.Approve(UserId, RMRK);
            }
            ViewBag.Notice = "Payslip has been approved";
            EMPLOYEE employee = db.EMPLOYEEs.Find(id);
            var monthly_payslips = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == id && x.IS_APPR == true && x.SAL_DATE == SalaryDate).OrderByDescending(x => x.SAL_DATE).ToList();
            var individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == id && x.SAL_DATE == SalaryDate).OrderByDescending(x => x.SAL_DATE).ToList();
            CalulatedSalary cs = employee.Calculate_Salary(monthly_payslips, individual_payslip_category);

            //var FeeCat = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Fees").Distinct();
            int? TranCatId = db.FINANCE_TRANSACTION_CATEGORY.Where(x=>x.NAME == "Salary").Select(x=>x.ID).FirstOrDefault();
            //int FinanceFee_id = financefeeVal != null && financefeeVal.Count() != 0 ? financefeeVal.FirstOrDefault().ID : -1;
            string ReceiptNo = ""; int Index = 1;
            foreach (var item in monthly_payslips)
            {
                if(Index == 1)
                {
                    ReceiptNo = item.ID.ToString();
                }
                else
                {
                    ReceiptNo = string.Concat(ReceiptNo, "-", item.ID.ToString());
                }
                Index += 1;
            }
            int PayeeId = 1;

            var OldTrans = db.FINANCE_TRANSACTION.Where(x => x.RCPT_NO == ReceiptNo && x.CAT_ID == TranCatId).ToList();
            if(OldTrans != null && OldTrans.Count() != 0)
            {
                foreach(var item in OldTrans)
                {
                    FINANCE_TRANSACTION OldTransId = db.FINANCE_TRANSACTION.Find(item.ID);
                    db.FINANCE_TRANSACTION.Remove(OldTransId);
                }
            }

            var transaction = new FINANCE_TRANSACTION()
            {
                TIL = string.Concat("SAL-", employee.FIRST_NAME),
                CAT_ID = TranCatId,
                DESCR = string.Concat("Salary Paid for ", employee.Full_Name, "For the month of ", SalaryDate.ToString("MMMM")),
                PAYEE_ID = PayeeId,
                PAYEE_TYPE = "Institution",
                AMT = (decimal)cs.net_amount,
                FINE_AMT = (decimal)cs.net_deductionable_amount,
                FINE_INCLD = false,
                FIN_FE_ID = null,
                MSTRTRAN_ID = -1,
                RCPT_NO = ReceiptNo,
                TRAN_DATE = DateTime.Today,
                CRETAED_AT = System.DateTime.Now,
                UPDATED_AT = System.DateTime.Now
            };
            db.FINANCE_TRANSACTION.Add(transaction);
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                    }
                }
                RedirectToAction("View_Employee_Payslip", new { id = id, salary_date = SalaryDate, ErrorMessage = ViewBag.ErrorMessage });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                RedirectToAction("View_Employee_Payslip", new { id = id, salary_date = SalaryDate, ErrorMessage = ViewBag.ErrorMessage });
            }

            return RedirectToAction("View_Employee_Payslip",new { id = id, salary_date = SalaryDate, Notice = ViewBag.Notice});
        }
        public ActionResult Employee_Payslip_Reject(int? id, int Sal_Year, int Sal_Month, int Sal_Day, string RSN)
        {
            DateTime SalaryDate = new DateTime(Sal_Year, Sal_Month, Sal_Day);
            var dates = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == id && x.SAL_DATE == SalaryDate).ToList();
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            EMPLOYEE employee = db.EMPLOYEEs.Find(id);
            var monthly_payslips = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == id && x.IS_APPR == true && x.SAL_DATE == SalaryDate).OrderByDescending(x => x.SAL_DATE).ToList();
            int? TranCatId = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Salary").Select(x => x.ID).FirstOrDefault();
            string ReceiptNo = ""; int Index = 1;
            foreach (var item in monthly_payslips)
            {
                if (Index == 1)
                {
                    ReceiptNo = item.ID.ToString();
                }
                else
                {
                    ReceiptNo = string.Concat(ReceiptNo, "-", item.ID.ToString());
                }
                Index += 1;
            }
            var OldTrans = db.FINANCE_TRANSACTION.Where(x => x.RCPT_NO == ReceiptNo && x.CAT_ID == TranCatId).ToList();
            if (OldTrans != null && OldTrans.Count() != 0)
            {
                foreach (var item in OldTrans)
                {
                    FINANCE_TRANSACTION OldTransId = db.FINANCE_TRANSACTION.Find(item.ID);
                    db.FINANCE_TRANSACTION.Remove(OldTransId);
                }
            }
            try { db.SaveChanges(); }
            catch (Exception e) { Console.WriteLine(e); }

            foreach (var d in dates)
            {
                d.Reject(UserId, RSN);
            }
            ViewBag.Notice = "Payslip Rejected";

            /*var privilege = Privilege.find_by_name("PayslipPowers");
            var hr_ids = privilege.user_ids; */
            var hr_ids = 1;
            string subject = "Payslip Rejected";
            string body= string.Concat("Payslip has been rejected for " , employee.FIRST_NAME , " " , employee.LAST_NAME , " (Employee Number :" , employee.EMP_NUM , ") For the month: " , SalaryDate.ToShortDateString());
            REMINDER rm = new REMINDER() { SNDR = UserId, SUB = subject, BODY = body, RCPNT = hr_ids };
            db.REMINDERs.Add(rm);
            try { db.SaveChanges(); }
            catch (Exception e) { Console.WriteLine(e); }

            return RedirectToAction("View_Employee_Payslip", new { id = id, salary_date = SalaryDate, Notice = ViewBag.Notice });
        }

        public ActionResult Approve_Monthly_Payslip(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            var PSL_Sal_date = (from psl in db.MONTHLY_PAYSLIP
                                select new { Salary_date = psl.SAL_DATE }).Distinct()
             .OrderBy(x => x.Salary_date).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in PSL_Sal_date)
            {
                string sala_Month_Year = string.Concat(((DateTime)item.Salary_date.Value).ToString("MMMM"), "-", item.Salary_date.Value.Year);
                var result = new SelectListItem();
                result.Text = sala_Month_Year;
                result.Value = item.Salary_date.ToString();
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select a month" });
            ViewBag.salary_dates = options;

            return View();
        }
        public ActionResult One_Click_Approve(string salary_date)
        {
            DateTime salary_date_val = DateTime.Parse(salary_date);
            ViewBag.salary_date = salary_date_val;
            var monthly_payslips = (from emp in db.EMPLOYEEs
                                    join mp in db.MONTHLY_PAYSLIP on emp.ID equals mp.EMP_ID
                                    join pc in db.PAYROLL_CATEGORY on mp.PYRL_CAT_ID equals pc.ID
                                    where mp.SAL_DATE == salary_date_val && (mp.IS_APPR == false || (mp.IS_APPR  == true && mp.RMRK == null))
                                    select new MonthyPayslip { EmployeeData = emp, MonthlyPayslipData = mp, PayrollCatogaryData = pc }).OrderBy(x => x.PayrollCatogaryData.NAME).ToList();
            ViewData["dates"] = monthly_payslips;

            return PartialView("_One_Click_Approve");
        }
        public ActionResult One_Click_Approve_Submit(DateTime? date)
        {
            ViewBag.salary_date = date;
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            var dates = db.MONTHLY_PAYSLIP.Where(x => x.SAL_DATE == date && x.IS_RJCT == false).AsEnumerable();
            foreach (var d in dates)
            {
                d.Approve(UserId, "One Click Approved");                
            }
            var Employees = db.EMPLOYEEs.Where(x => dates.Select(p => p.EMP_ID).Distinct().Contains(x.ID)).ToList();
            foreach(var employee in Employees)
            {
                //EMPLOYEE employee = db.EMPLOYEEs.Find(d.EMP_ID);
                var monthly_payslips = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == employee.ID && x.IS_APPR == true && x.SAL_DATE == date).OrderByDescending(x => x.SAL_DATE).ToList();
                var individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == employee.ID && x.SAL_DATE == date).OrderByDescending(x => x.SAL_DATE).ToList();
                CalulatedSalary cs = employee.Calculate_Salary(monthly_payslips, individual_payslip_category);
                int? TranCatId = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Salary").Select(x => x.ID).FirstOrDefault();
                string ReceiptNo = ""; int Index = 1;
                foreach (var item in monthly_payslips)
                {
                    if (Index == 1)
                    {
                        ReceiptNo = item.ID.ToString();
                    }
                    else
                    {
                        ReceiptNo = string.Concat(ReceiptNo, "-", item.ID.ToString());
                    }
                    Index += 1;
                }
                int PayeeId = 1;

                var OldTrans = db.FINANCE_TRANSACTION.Where(x => x.RCPT_NO == ReceiptNo && x.CAT_ID == TranCatId).ToList();
                if (OldTrans != null && OldTrans.Count() != 0)
                {
                    foreach (var item in OldTrans)
                    {
                        FINANCE_TRANSACTION OldTransId = db.FINANCE_TRANSACTION.Find(item.ID);
                        db.FINANCE_TRANSACTION.Remove(OldTransId);
                    }
                }

                var transaction = new FINANCE_TRANSACTION()
                {
                    TIL = string.Concat("SAL-", employee.FIRST_NAME),
                    CAT_ID = TranCatId,
                    DESCR = string.Concat("Salary Paid for ", employee.Full_Name, "For the month of ", date.Value.ToString("MMMM")),
                    PAYEE_ID = PayeeId,
                    PAYEE_TYPE = "Institution",
                    AMT = (decimal)cs.net_amount,
                    FINE_AMT = (decimal)cs.net_deductionable_amount,
                    FINE_INCLD = false,
                    FIN_FE_ID = null,
                    MSTRTRAN_ID = -1,
                    RCPT_NO = ReceiptNo,
                    TRAN_DATE = DateTime.Today,
                    CRETAED_AT = System.DateTime.Now,
                    UPDATED_AT = System.DateTime.Now
                };
                db.FINANCE_TRANSACTION.Add(transaction);
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                        }
                    }
                    RedirectToAction("Payslip_Index", new { ErrorMessage = ViewBag.ErrorMessage });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));
                    RedirectToAction("Payslip_Index", new { ErrorMessage = ViewBag.ErrorMessage });
                }
            }
            ViewBag.Notice = "Payslip has been approved.";        
            return RedirectToAction("Payslip_Index", new { Notice = ViewBag.Notice });
        }
        [AllowAnonymous]
        public JsonResult AmtIsNumeric([Bind(Prefix = "AMT")] decimal? AMT)
        {
            return Json(!(AMT < 0), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult UniqueName([Bind(Prefix = "NAME")] string NAME)
        {
            //check if any of the UserName matches the UserName specified in the Parameter using the ANY extension method.   
            return Json(!db.FINANCE_FEE_CATGEORY.Include(x => x.BATCH).Where(x => x.BATCH.IS_DEL == false).Any(x => x.NAME.ToUpper() == NAME.ToUpper()), JsonRequestBehavior.AllowGet);
        }
    }
}
