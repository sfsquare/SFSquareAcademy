﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using SFSAcademy.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Text;

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

        // GET: Fee Index
        public ActionResult Fees_Index()
        {
            return View();
        }

        // GET: Fee Index
        public ActionResult Master_Fees()
        {
            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == "Y").OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.searchString = options;
            return View();
        }

        // GET: Fee Index
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
                                where ffc.IS_DEL.Equals("N") && ffc.IS_MSTR == "Y"
                                orderby ffc.NAME
                                select new Models.FeeCategory { FinanceFeeCategoryData = ffc }).Distinct();

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
        public ActionResult Master_Category_Create()
        {
            ViewBag.NAME = "";
            ViewBag.DESCR = "";
            return View();
        }

        [HttpGet]
        public ActionResult _Master_Category_Create_Form()
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
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
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.searchString = options;

            List<SelectListItem> sFeeCategory = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == "Y").OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            sFeeCategory.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Master category" });
            ViewBag.MSTR_CATGRY_ID = sFeeCategory;

            return View(queryCourceBatch);
        }

        // POST: Finance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Master_Category_Create_Form(IList<SelectCourseBatch> model, string radioName, string NAME, string DESCR, int? MSTR_CATGRY_ID)
        {
            if (ModelState.IsValid)
            {
                int BatchCount = 0;
                if (string.IsNullOrEmpty(NAME) || string.IsNullOrEmpty(DESCR))
                {
                    Session["FeeCatErrorMessage"] = "Name or Description canot be blank";
                }
                else
                {
                    if (radioName == "Yes")
                    {
                        var FF_cATAGORY = new FINANCE_FEE_CATGEORY() { NAME = NAME, DESCR = DESCR, BTCH_ID = null, IS_DEL = "N", IS_MSTR = "Y", CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now, MSTR_CATGRY_ID = null };
                        db.FINANCE_FEE_CATGEORY.Add(FF_cATAGORY);
                        db.SaveChanges();
                        Session["FeeCatErrorMessage"] = "Master Category Added Successfully.";
                    }
                    else
                    {
                        foreach (SelectCourseBatch item in model)
                        {
                            if (item.Selected)
                            {
                                BatchCount++;
                                var FF_cATAGORY = new FINANCE_FEE_CATGEORY() { NAME = NAME, DESCR = DESCR, BTCH_ID = item.BatchData.ID, IS_DEL = "N", IS_MSTR = "N", CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now, MSTR_CATGRY_ID = MSTR_CATGRY_ID };
                                db.FINANCE_FEE_CATGEORY.Add(FF_cATAGORY);
                                db.SaveChanges();
                            }
                        }
                        Session["FeeCatErrorMessage"] = "Fee Category Added Successfully.";
                    }

                    if (radioName == "No" && BatchCount.Equals(0))
                    {
                        Session["FeeCatErrorMessage"] = "Please select at least one Batch";
                    }
                }
            }

            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
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
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.searchString = options;

            List<SelectListItem> sFeeCategory = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == "Y").OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            sFeeCategory.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Master category" });
            ViewBag.MSTR_CATGRY_ID = sFeeCategory;

            //return View(queryCourceBatch);
            return RedirectToAction("Master_Category_Create");
        }

        // GET: Fee Index
        public ActionResult Fee_Category_View()
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
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
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.searchString = options;
            return View();
        }

        // GET: Fee Index
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
                                where ffc.IS_DEL.Equals("N") && ffc.IS_MSTR.Equals("N")
                                orderby ffc.NAME
                                select new Models.FeeMasterCategory { FinanceFeeCategoryData = ffc, BatchData = b, CourseData = cs }).Distinct();



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


        // GET: Fee Index
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
                                  select new Models.FeeParticular { FeeParticularData = fp, FeeCategoryData = fc, StudentCategoryData = (subgsc == null ? null : subgsc), StudentData = (subgst == null ? null : subgst) })
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
            fINANCE_FEE_CATEGORY.IS_DEL = "Y";
            fINANCE_FEE_CATEGORY.UPDATED_AT = System.DateTime.Now;
            db.SaveChanges();
            ViewBag.ErrorMessage = string.Concat("Master Category Deleted Successfully");
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
                                    select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
        public ActionResult Master_Category_Edit([Bind(Include = "ID,NAME,DESCR,BTCH_ID,IS_DEL,IS_MSTR,CREATED_AT,UPDATED_AT")] FINANCE_FEE_CATGEORY fINANCE_FEE_CATEGORY)
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
                    fINANCE_FEE_CATEGORY_UPD.BTCH_ID = fINANCE_FEE_CATEGORY.BTCH_ID;
                    db.Entry(fINANCE_FEE_CATEGORY_UPD).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    fINANCE_FEE_CATEGORY.UPDATED_AT = System.DateTime.Now;
                    db.Entry(fINANCE_FEE_CATEGORY).State = EntityState.Modified;
                    db.SaveChanges();
                }
                ViewBag.ErrorMessage = string.Concat("Master Category Updated Successfully");
            }

            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
        public ActionResult Fees_Particulars_New(int? FeeCatCount)
        {
            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == "Y").OrderBy(x => x.NAME).Distinct(), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Master Category" });
            ViewBag.MASTER_CATGEORY_ID = options;

            if (!FeeCatCount.Equals(null))
            {
                ViewBag.FeeCatError = string.Concat("Fee Particular Details of ", FeeCatCount, " Student Batch updated in system successfully"); ;
            }
            return View();
        }


        [HttpGet]
        public ActionResult _Fees_Particulars_Create()
        {
            List<SelectListItem> sCategory = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.ID), "ID", "NAME").ToList();
            // add the 'ALL' option
            //sCategory.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.STDNT_CAT_ID = sCategory;
            ViewBag.radioName = "";
            ViewBag.FeePartOption = "";
            //ViewBag.FeeCatError = TempData["FeeCatError"];
            return View();
        }



        // POST: Finance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Fees_Particulars_Create([Bind(Include = "ID,NAME,DESCR,AMT,FIN_FEE_CAT_ID,STDNT_CAT_ID,ADMSN_NO,STDNT_ID,IS_DEL,CREATED_AT,UPDATED_AT")] FINANCE_FEE_PARTICULAR fINANCE_FEE_pARTUCULAR, IList<SelectFeeCategory> model, string FeePartOption, string radioName)
        {
            Session["FeeParticularMessage"] = "";
            if (ModelState.IsValid)
            {
                if (radioName == "Admission No.")
                {
                    int StdUpdated = 0;
                    foreach (var AdmissionNoList in HtmlHelpers.ApplicationHelper.StringToIntList(fINANCE_FEE_pARTUCULAR.ADMSN_NO).ToList())
                    {
                        //var StdResult = from u in db.STUDENTs where (u.ADMSN_NO == AdmissionNoList.ToString()) select u;
                        int stCount = 0;
                        foreach (SelectFeeCategory item in model)
                        {
                            if (item.Selected)
                            {
                                fINANCE_FEE_pARTUCULAR.FIN_FEE_CAT_ID = item.FinanceFeeCategoryData.ID;
                                var StdFinResult = from u in db.STUDENTs
                                                   where (u.ADMSN_NO == AdmissionNoList.ToString()
                                                    && u.BTCH_ID == item.BatchData.ID)
                                                   select u;
                                stCount = StdFinResult.Count();

                                if (StdFinResult.Count() != 0)
                                {
                                    StdUpdated = StdUpdated + 1;
                                    fINANCE_FEE_pARTUCULAR.STDNT_CAT_ID = null;
                                    fINANCE_FEE_pARTUCULAR.ADMSN_NO = AdmissionNoList.ToString();
                                    fINANCE_FEE_pARTUCULAR.STDNT_ID = StdFinResult.FirstOrDefault().ID;
                                    fINANCE_FEE_pARTUCULAR.IS_DEL = "N";
                                    fINANCE_FEE_pARTUCULAR.CREATED_AT = System.DateTime.Now;
                                    fINANCE_FEE_pARTUCULAR.UPDATED_AT = System.DateTime.Now;
                                    db.FINANCE_FEE_PARTICULAR.Add(fINANCE_FEE_pARTUCULAR);
                                    db.SaveChanges();
                                }
                            }
                            if (!stCount.Equals(0)) { break; }
                        }
                        if (stCount.Equals(0)) { Session["FeeParticularMessage"] = string.Concat(Session["FeeParticularMessage"], "Admission Number: ", AdmissionNoList, " is either invalid or does not belong to batch selected. "); }
                    }
                    if (StdUpdated == 0)
                    {
                        Session["FeeParticularMessage"] = "No student found with given Admission Numbers or they are not from the Batch selected.";
                    }
                    else
                    {
                        Session["FeeParticularMessage"] = string.Concat(Session["FeeParticularMessage"], " Fee Particular Details of ", StdUpdated, " Students updated in system successfully");
                    }
                }
                else if (radioName == "Student Category")
                {
                    int StdCatUpdated = 0;
                    foreach (SelectFeeCategory item in model)
                    {
                        if (item.Selected)
                        {
                            StdCatUpdated++;
                            fINANCE_FEE_pARTUCULAR.FIN_FEE_CAT_ID = item.FinanceFeeCategoryData.ID;
                            fINANCE_FEE_pARTUCULAR.IS_DEL = "N";
                            fINANCE_FEE_pARTUCULAR.CREATED_AT = System.DateTime.Now;
                            fINANCE_FEE_pARTUCULAR.UPDATED_AT = System.DateTime.Now;
                            db.FINANCE_FEE_PARTICULAR.Add(fINANCE_FEE_pARTUCULAR);
                            db.SaveChanges();
                        }
                    }
                    if (!StdCatUpdated.Equals(0)) { Session["FeeParticularMessage"] = string.Concat("Fee Particular Details of ", StdCatUpdated, " Student Category updated in system successfully"); }
                    else { Session["FeeParticularMessage"] = string.Concat("No valid category selected"); }
                }
                else
                {
                    int StdFeeUpdated = 0;
                    foreach (SelectFeeCategory item in model)
                    {
                        if (item.Selected)
                        {
                            StdFeeUpdated++;
                            fINANCE_FEE_pARTUCULAR.STDNT_CAT_ID = null;
                            fINANCE_FEE_pARTUCULAR.FIN_FEE_CAT_ID = item.FinanceFeeCategoryData.ID;
                            fINANCE_FEE_pARTUCULAR.IS_DEL = "N";
                            fINANCE_FEE_pARTUCULAR.CREATED_AT = System.DateTime.Now;
                            fINANCE_FEE_pARTUCULAR.UPDATED_AT = System.DateTime.Now;
                            db.FINANCE_FEE_PARTICULAR.Add(fINANCE_FEE_pARTUCULAR);
                            try { db.SaveChanges(); }
                            catch (Exception e) { Console.WriteLine(e); Session["FeeParticularMessage"] = e; }
                        }
                    }
                    if (!StdFeeUpdated.Equals(0)) { Session["FeeParticularMessage"] = string.Concat("Fee Particular Details of ", StdFeeUpdated, " Student Category updated in system successfully"); }
                    else { Session["FeeParticularMessage"] = string.Concat("No valid category selected"); }
                }

            }
            List<SelectListItem> sCategory = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.ID), "ID", "NAME", fINANCE_FEE_pARTUCULAR.STDNT_CAT_ID).ToList();
            // add the 'ALL' option
            sCategory.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.STDNT_CAT_ID = sCategory;
            return RedirectToAction("Fees_Particulars_New");

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

            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == "Y").OrderBy(x => x.NAME).Distinct(), "ID", "NAME", fINANCE_FEE_PARTICULAR.FIN_FEE_CAT_ID).ToList();
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

            ViewBag.FeePartOption = "";
            //ViewBag.FeeCatError = TempData["FeeCatError"];

            return View(fINANCE_FEE_PARTICULAR);
        }

        // POST: Finance/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Master_Category_Particulars_Edit([Bind(Include = "ID,NAME,DESCR,AMT,FIN_FEE_CAT_ID,STDNT_CAT_ID,ADMSN_NO,STDNT_ID,IS_DEL,CREATED_AT,UPDATED_AT")] FINANCE_FEE_PARTICULAR fINANCE_FEE_pARTUCULAR, string FeePartOption, string radioName)
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

            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == "Y").OrderBy(x => x.NAME).Distinct(), "ID", "NAME", fINANCE_FEE_PARTICULAR_UPD.FIN_FEE_CAT_ID).ToList();
            ViewBag.FIN_FEE_CAT_ID = options;

            List<SelectListItem> sCategory = new SelectList(db.STUDENT_CATGEORY.OrderBy(x => x.ID), "ID", "NAME", fINANCE_FEE_PARTICULAR_UPD.STDNT_CAT_ID).ToList();
            ViewBag.STDNT_CAT_ID = sCategory;
            if (fINANCE_FEE_PARTICULAR_UPD.STDNT_ID != null)
            { ViewBag.radioName = "Admission No."; }
            else if (fINANCE_FEE_PARTICULAR_UPD.STDNT_CAT_ID != null)
            { ViewBag.radioName = "Student Category"; }
            else
            { ViewBag.radioName = "All"; }

            ViewBag.FeePartOption = "";
            return View(fINANCE_FEE_PARTICULAR_UPD);
        }




        [HttpGet]
        public ActionResult Fee_Discounts(string sortOrder, string currentFilter, string BTCH_ID, int? page, string currentFilter2, string FINANCE_FEE_CATGEORY_ID)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == "N" && bt.IS_DEL == "N"
                                    select new { CourseData = cs, BatchData = bt })
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

            //List<SelectListItem> options = new SelectList(db.BATCHes.OrderBy(x => x.ID).Distinct(), "NAME", "NAME", BTCH_ID).ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.BTCH_ID = options;

            List<SelectListItem> options2 = new SelectList(db.FINANCE_FEE_CATGEORY.OrderBy(x => x.NAME).Distinct(), "ID", "NAME", FINANCE_FEE_CATGEORY_ID).ToList();
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.FINANCE_FEE_CATGEORY_ID = options2;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (BTCH_ID != null)
            {
                if (BTCH_ID != "-1")
                {
                    page = 1;
                }
                else { BTCH_ID = currentFilter; }
            }
            else { BTCH_ID = currentFilter; }
            ViewBag.CurrentFilter = BTCH_ID;

            if (FINANCE_FEE_CATGEORY_ID != null)
            {
                if (FINANCE_FEE_CATGEORY_ID != "-1")
                {
                    page = 1;
                }
                else { FINANCE_FEE_CATGEORY_ID = currentFilter2; }

            }
            else { FINANCE_FEE_CATGEORY_ID = currentFilter2; }
            ViewBag.CurrentFilter2 = FINANCE_FEE_CATGEORY_ID;

            var Fee_discountSData = (from fd in db.FEE_DISCOUNT
                                     join ffc in db.FINANCE_FEE_CATGEORY on fd.FIN_FEE_CAT_ID equals ffc.ID
                                     join bt in db.BATCHes on ffc.BTCH_ID equals bt.ID
                                     join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                     join fds in (from crt in db.FEE_DISCOUNT where crt.TYPE == "Student" select crt) on fd.ID equals fds.ID into gms
                                     from subgms in gms.DefaultIfEmpty()
                                     join std in db.STUDENTs on subgms.RCVR_ID equals std.ID into gm  
                                     from substd in gm.DefaultIfEmpty()
                                     join fdsc in (from crtc in db.FEE_DISCOUNT where crtc.TYPE == "Student Category" select crtc) on fd.ID equals fdsc.ID into gmsc
                                     from subgmsc in gmsc.DefaultIfEmpty()
                                     join cat in db.STUDENT_CATGEORY on subgmsc.RCVR_ID equals cat.ID into gl 
                                     from subcat in gl.DefaultIfEmpty()
                                     orderby fd.NAME
                                     select new Models.FeeDiscount { FeeDiscountData = fd, FinanceFeeCategoryData = ffc, BatchData = bt, CourseData = cs, StudentData = (substd == null ? null : substd), StudentCategoryData = (subcat == null ? null : subcat) }).Distinct();

            if (BTCH_ID != null && !BTCH_ID.Equals("-1"))
            {
                Fee_discountSData = Fee_discountSData.Where(s => s.BatchData.NAME.Contains(BTCH_ID));
            }
            if (FINANCE_FEE_CATGEORY_ID != null && !FINANCE_FEE_CATGEORY_ID.Equals("-1"))
            {
                Fee_discountSData = Fee_discountSData.Where(s => s.FinanceFeeCategoryData.NAME.Contains(FINANCE_FEE_CATGEORY_ID));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    Fee_discountSData = Fee_discountSData.OrderByDescending(s => s.FeeDiscountData.NAME);
                    break;
                case "Date":
                    Fee_discountSData = Fee_discountSData.OrderBy(s => s.StudentData.ADMSN_DATE);
                    break;
                case "date_desc":
                    Fee_discountSData = Fee_discountSData.OrderByDescending(s => s.StudentData.ADMSN_DATE);
                    break;
                default:  // Name ascending 
                    Fee_discountSData = Fee_discountSData.OrderBy(s => s.FeeDiscountData.NAME);
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
                                       where cs.IS_DEL == "N" && bt.IS_DEL == "N" && fcc.IS_DEL == "N"
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
                if (radioName == "Batch")
                {
                    var fEEcATEGORYaACCESS = (from ffc in db.FINANCE_FEE_CATGEORY
                                              where ffc.ID == FIN_FEE_CAT_ID
                                              select new { FinanceFeeCategoryData = ffc }).ToList();

                    fEEdISCOUNT.TYPE = radioName;
                    fEEdISCOUNT.FIN_FEE_CAT_ID = fEEcATEGORYaACCESS.FirstOrDefault().FinanceFeeCategoryData.ID;
                    if (radioName2.Equals("Amount"))
                    {
                        fEEdISCOUNT.IS_AMT = "Y";
                    }
                    else
                    {
                        fEEdISCOUNT.IS_AMT = "N";
                    }
                    db.FEE_DISCOUNT.Add(fEEdISCOUNT);
                    db.SaveChanges();
                    ViewBag.ErrorMessage = string.Concat("Fee Discounts of select batch added in system successfully");
                }
                else if (radioName == "Student Category")
                {
                    fEEdISCOUNT.RCVR_ID = STUDENT_CATGEORY_ID;
                    fEEdISCOUNT.TYPE = radioName;
                    fEEdISCOUNT.FIN_FEE_CAT_ID = FIN_FEE_CAT_ID;
                    if (radioName2.Equals("Amount"))
                    {
                        fEEdISCOUNT.IS_AMT = "Y";
                    }
                    else
                    {
                        fEEdISCOUNT.IS_AMT = "N";
                    }
                    db.FEE_DISCOUNT.Add(fEEdISCOUNT);
                    db.SaveChanges();
                    ViewBag.ErrorMessage = string.Concat("Fee Discounts of Student Category added in system successfully");
                }
                else if (radioName == "Student")
                {
                    if (HtmlHelpers.ApplicationHelper.StringToIntList(ADMSN_NO).Count() != 0)
                    {
                        int stdCount = 0;
                        foreach (var AdmissionNoList in HtmlHelpers.ApplicationHelper.StringToIntList(ADMSN_NO).ToList())
                        {
                            var StdResult = from u in db.STUDENTs where (u.ADMSN_NO == AdmissionNoList.ToString() && u.IS_DEL.Equals("N") && u.IS_ACT.Equals("Y")) select u;
                            if (StdResult.Count() != 0)
                            {
                                stdCount++;
                                fEEdISCOUNT.RCVR_ID = StdResult.FirstOrDefault().ID;
                                var fEEcATEGORYaACCESS = (from ffc in db.FINANCE_FEE_CATGEORY
                                                          where ffc.ID == FIN_FEE_CAT_ID
                                                          select new { FinanceFeeCategoryData = ffc }).ToList();

                                fEEdISCOUNT.TYPE = radioName;
                                fEEdISCOUNT.FIN_FEE_CAT_ID = fEEcATEGORYaACCESS.FirstOrDefault().FinanceFeeCategoryData.ID;
                                if (radioName2.Equals("Amount"))
                                {
                                    fEEdISCOUNT.IS_AMT = "Y";
                                }
                                else
                                {
                                    fEEdISCOUNT.IS_AMT = "N";
                                }
                                db.FEE_DISCOUNT.Add(fEEdISCOUNT);
                            }
                        }
                        try { db.SaveChanges(); }
                        catch (Exception e) { Console.WriteLine(e); }
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

            var queryCourceBatchFee = (from cs in db.COURSEs
                                       join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                       join fcc in db.FINANCE_FEE_CATGEORY on bt.ID equals fcc.BTCH_ID
                                       where cs.IS_DEL == "N" && bt.IS_DEL == "N" && fcc.IS_DEL == "N"
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
        public ActionResult Edit_Fee_Discount([Bind(Include = "ID,TYPE,NAME,RCVR_ID,FIN_FEE_CAT_ID,DISC,IS_AMT")] FEE_DISCOUNT fEE_DISCOUNT)
        {
            if (ModelState.IsValid)
            {
                FEE_DISCOUNT fEE_DISCOUNT_UPD = db.FEE_DISCOUNT.Find(fEE_DISCOUNT.ID);
                fEE_DISCOUNT_UPD.TYPE = fEE_DISCOUNT.TYPE;
                fEE_DISCOUNT_UPD.NAME = fEE_DISCOUNT.NAME;
                fEE_DISCOUNT_UPD.DISC = fEE_DISCOUNT.DISC;
                fEE_DISCOUNT_UPD.IS_AMT = fEE_DISCOUNT.IS_AMT;
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


        // GET: Fee Index
        public ActionResult Fee_Collection()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Fee_Collection_New()
        {
            List<SelectListItem> options = new SelectList(db.FINANCE_FEE_CATGEORY.Where(x => x.IS_MSTR == "Y").OrderBy(x => x.NAME).Distinct(), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Master Category" });
            ViewBag.MASTER_CATGEORY_ID = options;

            return View();
        }

        [HttpGet]
        public ActionResult _Select_Batch(int? id)
        {
            var fEEcATEGORYaACCESS = (from ffc in db.FINANCE_FEE_CATGEORY
                                      join b in db.BATCHes on ffc.BTCH_ID equals b.ID
                                      join cs in db.COURSEs on b.CRS_ID equals cs.ID
                                      where ffc.IS_DEL.Equals("N") && b.IS_DEL.Equals("N") && ffc.IS_MSTR.Equals("N") && ffc.MSTR_CATGRY_ID == id
                                      select new Models.SelectFeeCategory { FinanceFeeCategoryData = ffc, BatchData = b, CourseData = cs, Selected = false }).OrderBy(g => g.FinanceFeeCategoryData.ID).ToList();


            return PartialView("_Select_Batch", fEEcATEGORYaACCESS);
        }

        [HttpGet]
        public ActionResult _Fee_Collection_Create()
        {
            ViewBag.ReturnDate = System.DateTime.Now;
            ViewBag.ErrorMessage = null;
            return View();
        }



        // POST: Finance/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Fee_Collection_Create([Bind(Include = "ID,NAME,START_DATE,END_DATE,FEE_CAT_ID,BTCH_ID,IS_DEL,DUE_DATE")] FINANCE_FEE_COLLECTION fINANCE_FEE_cOLLECTION, IList<SelectFeeCategory> model)
        {

            if (ModelState.IsValid)
            {
                int FeeCatCount = 0;
                int FeeCatId = 0;
                int SelectFeeCatId = 0;
                foreach (SelectFeeCategory item in model)
                {
                    if (item.Selected)
                    {
                        FeeCatCount++;
                        SelectFeeCatId = item.FinanceFeeCategoryData.ID;
                        var FFeeColl = new FINANCE_FEE_COLLECTION() { NAME = fINANCE_FEE_cOLLECTION.NAME, START_DATE = fINANCE_FEE_cOLLECTION.START_DATE, END_DATE = fINANCE_FEE_cOLLECTION.END_DATE, DUE_DATE = fINANCE_FEE_cOLLECTION.DUE_DATE, FEE_CAT_ID = item.FinanceFeeCategoryData.ID, BTCH_ID = item.BatchData.ID, IS_DEL = "N" };
                        db.FINANCE_FEE_COLLECTION.Add(FFeeColl);
                        db.SaveChanges();
                        //var StdResult = from u in db.STUDENTs where (u.BTCH_ID == item.BatchData.ID) select u;
                        var StdResult = (from ffc in db.FINANCE_FEE_CATGEORY
                                         join b in db.BATCHes on ffc.BTCH_ID equals b.ID
                                         join st in db.STUDENTs on b.ID equals st.BTCH_ID
                                         join fcol in db.FINANCE_FEE_COLLECTION on new { A = ffc.ID.ToString(), B = b.ID.ToString() } equals new { A = fcol.FEE_CAT_ID.ToString(), B = fcol.BTCH_ID.ToString() }
                                         where ffc.ID == item.FinanceFeeCategoryData.ID && ffc.IS_DEL.Equals("N") && b.IS_DEL == "N" && st.IS_DEL == "N" && st.IS_ACT == "Y"
                                         select new { FinanceFeeCategoryData = ffc, BatchData = b, StudentData = st, FeeCollectionData = fcol }).OrderBy(g => g.FinanceFeeCategoryData.ID).ToList();
                        foreach (var item2 in StdResult)
                        {
                            STUDENT FeeStudent = db.STUDENTs.Find(item2.StudentData.ID);
                            FeeStudent.HAS_PD_FE = "N";
                            FeeStudent.UPDATED_AT = System.DateTime.Now;
                            db.SaveChanges();
                            var FF_fEE = new FINANCE_FEE() { STDNT_ID = item2.StudentData.ID, FEE_CLCT_ID = item2.FeeCollectionData.ID, IS_PD = "N" };
                            db.FINANCE_FEE.Add(FF_fEE);
                            db.SaveChanges();
                        }
                        if (!FeeCatId.Equals(SelectFeeCatId))
                        {
                            var FF_eVENT = new EVENT() { TTIL = "Fees Due", DESCR = fINANCE_FEE_cOLLECTION.NAME, START_DATE = fINANCE_FEE_cOLLECTION.START_DATE, END_DATE = fINANCE_FEE_cOLLECTION.END_DATE, IS_DUE = "Y", ORIGIN_ID = 1, ORIGIN_TYPE = "Fee Collection" };
                            db.EVENTs.Add(FF_eVENT);
                            db.SaveChanges();
                            FeeCatId = SelectFeeCatId;
                        }

                    }
                }
                if (FeeCatCount.Equals(0))
                {
                    Session["FeeCollectionMessage"] = "Please select valid Fee Category";
                }
                else { Session["FeeCollectionMessage"] = string.Concat("Fee Collection for ", FeeCatCount, " Fee Categories added in system"); }
                return RedirectToAction("Fee_Collection_New");
            }
            Session["FeeCollectionMessage"] = "There seems to be some issue with Model State";
            return View(fINANCE_FEE_cOLLECTION);
        }


        public ActionResult Fee_Collection_View(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
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
                                  where fc.IS_DEL == "N"
                                  orderby fc.NAME
                                  select new Models.FeeCollection { FinanceFeeCollectionData = fc, BatchData = b, CourseData = cs }).Distinct();


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
                                    select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
                                    select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
            db.FINANCE_FEE_COLLECTION.Remove(fINANCE_FEE_cOLLECTION);
            db.SaveChanges();
            ViewBag.ErrorMessage = string.Concat("Fee Collection Deleted Successfully");
            return View();
        }


        // GET: Fee Index
        public ActionResult Fees_Submission_Index()
        {
            return View();
        }

        // GET: Fee Index
        public ActionResult Fees_Submission_Batch()
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
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
            Session["student_serach"] = null;

            return View();

        }

        // GET: Fee Index
        public ActionResult _fees_collection_dates(int? id)
        {
            var queryCollections = (from cl in db.FINANCE_FEE_COLLECTION
                                    where cl.IS_DEL == "N" && cl.BTCH_ID == id
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

            //List<SelectListItem> options2 = new SelectList(db.FINANCE_FEE_COLLECTION.OrderBy(x => x.NAME).Distinct(), "NAME", "NAME").ToList();
            // add the 'ALL' option
            //options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Fee Collection Date" });
            //ViewBag.COLLECTION_ID = options2;

            return PartialView("_fees_collection_dates");

        }

        // GET: Fee Index
        public ActionResult _Student_Fees_Submission(int? id, int? batch_id, int? student_id)
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
                             select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
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
                                  where st.BTCH_ID == date.BTCH_ID
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
                                        where st.BTCH_ID == date.BTCH_ID
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
                                        where st.BTCH_ID == date.BTCH_ID
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
                                     select new Models.FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
                ViewData["fee_category"] = fee_category;

                var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                           where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                           select ff).ToList();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Batch"
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID
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
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (student_discounts_val != null && student_discounts_val.Count() != 0)
                {
                    foreach (var item in student_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (category_discounts_val != null && category_discounts_val.Count() != 0)
                {
                    foreach (var item in category_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (total_discount_val > total_payable)
                {
                    total_discount_val = total_payable;
                }
                ViewBag.total_discount = total_discount_val;
                decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
                ViewBag.total_discount_percentage = total_discount_percentage_val;

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Batch"
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID
                                        select ff);
                ViewData["student_fine"] = student_fine_val;

                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.FIN_FEE_CAT_ID == date.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID
                                         select ff);
                ViewData["category_fine"] = category_fine_val;
                decimal total_fine_val = 0;
                if (batch_fine_val != null && batch_fine_val.Count() != 0)
                {
                    foreach(var item in batch_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                    
                }
                if (student_fine_val != null && student_fine_val.Count() != 0)
                {
                    foreach (var item in student_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }                   
                }
                if (category_fine_val != null && category_fine_val.Count() != 0)
                {
                    foreach (var item in category_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                ViewBag.fine = 0;
                ViewBag.total_fine = total_fine_val;
            }
            else
            {
                ViewBag.FeeCollectionMessage = "No students have been assigned this fee.";
            }

            return PartialView("_Student_Fees_Submission");
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Update_Fine_Ajax(int? student, int? batch_id, int? date, int? fee)
        {
            if (ModelState.IsValid)
            {
                DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
                ViewBag.ReturnDate = PDate.ToShortDateString();
                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == batch_id
                                 select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;
                FINANCE_FEE_COLLECTION date_val = db.FINANCE_FEE_COLLECTION.Find(date);
                ViewData["date"] = date_val;
                FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(date);

                var feeVal = (from ff in db.FINANCE_FEE
                              join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                              where ff.FEE_CLCT_ID == fee_collection.ID
                              select new Models.StundentFee { FinanceFeeData = ff, StudentData = st }).OrderBy(x => x.StudentData.ID).Distinct();

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
                                     select new Models.FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
                ViewData["fee_category"] = fee_category;
                var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                           select ff).ToList();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch"
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == student
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID
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
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (student_discounts_val != null && student_discounts_val.Count() != 0)
                {
                    foreach (var item in student_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (category_discounts_val != null && category_discounts_val.Count() != 0)
                {
                    foreach (var item in category_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (total_discount_val > total_payable)
                {
                    total_discount_val = total_payable;
                }
                ViewBag.total_discount = total_discount_val;
                decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
                ViewBag.total_discount_percentage = total_discount_percentage_val;
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
                        IS_AMT = "Y",
                        FINE_DATE = System.DateTime.Now
                    };
                    db.FEE_FINE.Add(FeeFine);
                    try { db.SaveChanges(); ViewBag.FeeCollectionMessage = string.Concat("Fee Collection Edited Successfully"); }
                    catch (Exception e) { ViewBag.FeeCollectionMessage = string.Concat("Please eneter valid value for Fine"); Console.WriteLine(e); }
                }
                else
                {
                    ViewBag.FeeCollectionMessage = string.Concat("Please eneter valid value for Fine");
                }

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch"
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == student
                                        select ff);
                ViewData["student_fine"] = student_fine_val;
                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID
                                         select ff);
                ViewData["category_fine"] = category_fine_val;
                decimal total_fine_val = 0;
                if (batch_fine_val != null && batch_fine_val.Count() != 0)
                {
                    foreach (var item in batch_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (student_fine_val != null && student_fine_val.Count() != 0)
                {
                    foreach (var item in student_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (category_fine_val != null && category_fine_val.Count() != 0)
                {
                    foreach (var item in category_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }

                
                ViewBag.total_fine = total_fine_val;
            }        

            return PartialView("_Student_Fees_Submission");
        }



        public ActionResult update_ajax(int? student, int? batch_id, int? date, int? fine, String PAYMENT_MODE, String PAYMENT_NOTE, decimal? PAYMENT_AMOUNT, String PAYMENT_DATE)
        {
            if (ModelState.IsValid)
            {
                DateTime PDate = Convert.ToDateTime(PAYMENT_DATE);
                ViewBag.ReturnDate = PDate.ToShortDateString();
                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == batch_id
                                 select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;
                FINANCE_FEE_COLLECTION date_val = db.FINANCE_FEE_COLLECTION.Find(date);
                ViewData["date"] = date_val;
                FINANCE_FEE_COLLECTION fee_collection = db.FINANCE_FEE_COLLECTION.Find(date);

                var feeVal = (from ff in db.FINANCE_FEE
                              join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                              where ff.FEE_CLCT_ID == fee_collection.ID
                              select new Models.StundentFee { FinanceFeeData = ff, StudentData = st }).OrderBy(x => x.StudentData.ID).Distinct();

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
                                     select new Models.FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
                ViewData["fee_category"] = fee_category;
                var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && (ff.STDNT_ID == StudentVal.FirstOrDefault().Student_data.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                           select ff).ToList();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch"
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == student
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID
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
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (student_discounts_val != null && student_discounts_val.Count() != 0)
                {
                    foreach (var item in student_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (category_discounts_val != null && category_discounts_val.Count() != 0)
                {
                    foreach (var item in category_discounts_val)
                    {
                        total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                    }
                }
                if (total_discount_val > total_payable)
                {
                    total_discount_val = total_payable;
                }
                ViewBag.total_discount = total_discount_val;
                decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
                ViewBag.total_discount_percentage = total_discount_percentage_val;

                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch"
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == student
                                        select ff);
                //ViewData["student_fine"] = student_fine_val;
                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.STDNT_CAT_ID
                                         select ff);
                ViewData["category_fine"] = category_fine_val;
                decimal total_fine_val = 0;
                if (batch_fine_val != null && batch_fine_val.Count() != 0)
                {
                    foreach (var item in batch_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (student_fine_val != null && student_fine_val.Count() != 0)
                {
                    foreach (var item in student_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (category_fine_val != null && category_fine_val.Count() != 0)
                {
                    foreach (var item in category_fine_val)
                    {
                        total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                    }
                }
                if (fine >= 0)
                {
                    total_fine_val += (decimal)fine;
                }
                else
                { ViewBag.FeeCollectionMessage = string.Concat("Please select valid fine value"); }
                ViewBag.fine = 0;
                ViewBag.total_fine = total_fine_val;

                decimal total_fees = 0;
                foreach (var item in fee_particulars_val)
                {
                    total_fees += (decimal)item.AMT;
                }
                if (total_fine_val != 0)
                {
                    if (financefeeVal != null && financefeeVal.Count() != 0)
                    {
                        if (financefeeVal.FirstOrDefault().IS_PD != "Y")
                        {
                            total_fees += total_fine_val;

                        }
                        else
                        {
                            total_fees = (decimal)fine;
                        }
                    }
                    else { total_fees += total_fine_val; }

                }

                decimal fees_paid = (decimal)PAYMENT_AMOUNT;
                var FeeCat = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Fees").Distinct();
                if(FeeCat != null && FeeCat.Count()>0)
                {
                    if (fees_paid >= 0)
                    {
                        if (fees_paid <= total_fees)
                        {
                            //var FeeCat = db.FINANCE_TRANSACTION_CATEGORY.Where(x => x.NAME == "Fees").Distinct();
                            int TranCatId = FeeCat != null ? Convert.ToInt32(FeeCat.FirstOrDefault().ID) : -1;
                            int FinanceFee_id = financefeeVal != null && financefeeVal.Count() != 0 ? financefeeVal.FirstOrDefault().ID : -1;
                            String ReceiptNo = feeVal != null ? feeVal.FirstOrDefault().FinanceFeeData.ID.ToString() : "";
                            int PayeeId = StudentVal != null ? StudentVal.FirstOrDefault().Student_data.ID : -1;

                            var transaction = new FINANCE_TRANSACTION()
                            {
                                TIL = total_fees > fees_paid ? string.Concat("RN.Partial:", ReceiptNo) : string.Concat("RN:", ReceiptNo),
                                CAT_ID = TranCatId,
                                DESCR = total_fees > fees_paid ? string.Concat("Part Payment: ", PAYMENT_MODE, " : ", PAYMENT_NOTE, ":", ReceiptNo) : string.Concat("Full Payment: ", PAYMENT_MODE, " : ", PAYMENT_NOTE, ":", ReceiptNo),
                                PAYEE_ID = PayeeId,
                                PAYEE_TYPE = "Student",
                                AMT = (decimal)fees_paid,
                                FINE_AMT = total_fine_val,
                                FINE_INCLD = total_fine_val != 0 ? "Y" : "N",
                                FIN_FE_ID = FinanceFee_id,
                                MSTRTRAN_ID = -1,
                                RCPT_NO = ReceiptNo,
                                TRAN_DATE = PDate,
                                CRETAED_AT = System.DateTime.Now,
                                UPDATED_AT = System.DateTime.Now
                            };
                            db.FINANCE_TRANSACTION.Add(transaction);
                            db.SaveChanges();
                            if (ViewData["paid_fees"] == null)
                            {
                                transaction.MSTRTRAN_ID = transaction.ID;
                            }
                            db.Entry(transaction).State = EntityState.Modified;
                            db.SaveChanges();

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
                            ffUpdate.IS_PD = fees_paid == total_fees ? "Y" : "N";
                            db.Entry(ffUpdate).State = EntityState.Modified;
                            db.SaveChanges();
                            if (fine > 0)
                            {
                                var FineRecord = new FEE_FINE()
                                {
                                    TYPE = "Student",
                                    NAME = "Ad-hoc fine",
                                    RCVR_ID = PayeeId,
                                    FIN_FEE_CAT_ID = fee_category.ID,
                                    FINE = (decimal)fine,
                                    IS_AMT = "Y",
                                    FINE_DATE = System.DateTime.Now
                                };
                                db.FEE_FINE.Add(FineRecord);
                                db.SaveChanges();
                            }

                            var paid_fees_val2 = (from ff in db.FINANCE_FEE
                                                  join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                                  join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                                  where ff.FEE_CLCT_ID == fee_collection.ID && st.ID == student
                                                  select new Models.FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                            ViewData["paid_fees"] = paid_fees_val2;
                            var student_fine_val2 = (from ff in db.FEE_FINE
                                                     where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.FirstOrDefault().Student_data.ID
                                                     select ff);
                            ViewData["student_fine"] = student_fine_val2;

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
            return PartialView("_Student_Fees_Submission");
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
                                 select new Models.FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewData["paid_fees"] = paid_fees_val;

            FINANCE_FEE_CATGEORY fee_category = db.FINANCE_FEE_CATGEORY.Find(fee_collection.FEE_CAT_ID);
            ViewData["fee_category"] = fee_category;

            var fee_particulars_val = (from ff in db.FINANCE_FEE_PARTICULAR
                                       where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && (ff.STDNT_ID == StudentVal.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == StudentVal.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                       select ff).ToList();
            ViewData["fee_particulars"] = fee_particulars_val;

            var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                       where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch"
                                       select ff);
            ViewData["batch_discounts"] = batch_discounts_val;
            var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                         where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.ID
                                         select ff);
            ViewData["student_discounts"] = student_discounts_val;
            var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                          where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.STDNT_CAT_ID
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
                    total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }               
            }
            if (student_discounts_val != null && student_discounts_val.Count() != 0)
            {
                foreach (var item in student_discounts_val)
                {
                    total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }
            }
            if (category_discounts_val != null && category_discounts_val.Count() != 0)
            {
                foreach (var item in category_discounts_val)
                {
                    total_discount_val += item.IS_AMT == "Y" ? (decimal)item.DISC : total_payable * (decimal)item.DISC / 100;
                }
            }
            if (total_discount_val > total_payable)
            {
                total_discount_val = total_payable;
            }
            ViewBag.total_discount = total_discount_val;
            decimal total_discount_percentage_val = total_discount_val / total_payable * 100;
            ViewBag.total_discount_percentage = total_discount_percentage_val;
            var batch_fine_val = (from ff in db.FEE_FINE
                                  where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Batch"
                                  select ff);
            ViewData["batch_fine"] = batch_fine_val;
            var student_fine_val = (from ff in db.FEE_FINE
                                    where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == StudentVal.ID
                                    select ff);
            ViewData["student_fine"] = student_fine_val;
            var category_fine_val = (from ff in db.FEE_FINE
                                     where ff.FIN_FEE_CAT_ID == date_val.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == StudentVal.STDNT_CAT_ID
                                     select ff);
            ViewData["category_fine"] = category_fine_val;
            decimal total_fine_val = 0;
            if (batch_fine_val != null && batch_fine_val.Count() != 0)
            {
                foreach (var item in batch_fine_val)
                {
                    total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            if (student_fine_val != null && student_fine_val.Count() != 0)
            {
                foreach (var item in student_fine_val)
                {
                    total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            if (category_fine_val != null && category_fine_val.Count() != 0)
            {
                foreach (var item in category_fine_val)
                {
                    total_fine_val += item.IS_AMT == "Y" ? (decimal)item.FINE : total_payable * (decimal)item.FINE / 100;
                }
            }
            ViewBag.total_fine = total_fine_val;

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


        // GET: Fee Index
        public ActionResult Fees_Student_Search()
        {

            return View();

        }

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
                                 select new Models.SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;

                var queryCollections = (from cl in db.FINANCE_FEE_COLLECTION
                                        where cl.IS_DEL == "N" && cl.BTCH_ID == student.BTCH_ID
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
                db.SaveChanges();
                return RedirectToAction("Index");
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
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FEE_CLCT_ID = new SelectList(db.FINANCE_FEE_COLLECTION, "ID", "NAME", fINANCE_FEE.FEE_CLCT_ID);
            ViewBag.TRAN_ID = new SelectList(db.FINANCE_TRANSACTION, "ID", "TIL", fINANCE_FEE.TRAN_ID);
            ViewBag.STDNT_ID = new SelectList(db.STUDENTs, "ID", "ADMSN_NO", fINANCE_FEE.STDNT_ID);
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

        // GET: Student/Details/5
        public ActionResult Categories()
        {
            return View();
        }

        // GET: Student/Details/5
        [ChildActionOnly]
        public ActionResult _CategoriesList()
        {
            var fINANCEcATEGORY = db.FINANCE_TRANSACTION_CATEGORY.Where(d => d.DEL == "N").ToList();
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
            fINANCEcATEGORY.DEL = "Y";
            db.Entry(fINANCEcATEGORY).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Categories");
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
                db.SaveChanges();
                return RedirectToAction("Categories");
            }
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
                fINANCEcATEGORY.DEL = "N";
                db.FINANCE_TRANSACTION_CATEGORY.Add(fINANCEcATEGORY);
                db.SaveChanges();
                return RedirectToAction("Categories");
            }

            return View(fINANCEcATEGORY);
        }

    }
}
