using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SFSAcademy.Models;
using SFSAcademy.HtmlHelpers;
using System.IO;
using iTextSharp.text;
using System.Data.Entity.Validation;
using System.Globalization;

namespace SFSAcademy.Controllers
{
    public class StoreController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Store
        public ActionResult Index()
        {
            //var sTORE_PRODUCTS = db.STORE_PRODUCTS.Include(s => s.STORE_CATEGORY);
            return View();
        }

        // GET: Store/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_PRODUCTS sTORE_PRODUCTS = db.STORE_PRODUCTS.Find(id);
            if (sTORE_PRODUCTS == null)
            {
                return HttpNotFound();
            }
            return View(sTORE_PRODUCTS);
        }

        // GET: Store/Create
        public ActionResult Create(string ErrorMessage, string Notice)
        {
            List<SelectListItem> options = new SelectList(db.STORE_CATEGORY.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Category" });
            ViewBag.CATEGORY_ID = options;

            List<SelectListItem> options2 = new SelectList(db.STORE_SUB_CATEGORY.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Sub Category" });
            ViewBag.SUB_CATEGORY_ID = options2;

            List<SelectListItem> options3 = new SelectList(db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            options3.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Brand" });
            ViewBag.BRAND_ID = options3;

            var product_brands = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_brands"] = product_brands;

            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            //ViewBag.Brand_Id_Sel = Brand_Id_Sel;
            //ViewBag.Vendor_Id_Sel = Vendor_Id_Sel;

            DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
            ViewBag.ReturnDate = PDate.ToShortDateString();

            return View();
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpGet]
        public ActionResult Brand_Select(int? id)
        {
            List<SelectListItem> options3 = new SelectList(db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", id).ToList();
            options3.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Brand" });
            ViewBag.BRAND_ID = options3;

            var product_brands = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_brands"] = product_brands;

            return PartialView("_brand_select", product_brands);
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpGet]
        public ActionResult Vendor_Select(int? id)
        {
            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", id).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            return PartialView("_vendor_select", product_vendor);
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpGet]
        public ActionResult Sub_Categories_Select(int? id)
        {
            List<SelectListItem> options = new SelectList(db.STORE_SUB_CATEGORY.Where(x => x.IS_DEL == false && x.STORE_CATEGORY_ID == id).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Sub Category" });
            ViewBag.SUB_CATEGORY_ID = options;

            return PartialView("_Sub_Categories_Select");
        }

        // POST: Store/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PRODUCT_ID,NAME,CATEGORY_ID,SUB_CATEGORY_ID,BRAND_ID,TOTAL_UNIT,TOTAL_COST,COST_PER_UNIT,SELL_PRICE_PER_UNIT,PURCHASED_ON,VENDOR_ID,PAID_BY,UNIT_LEFT,IS_ACT,IS_DEL,CREATED_AT,UPDATED_AT")] STORE_PRODUCTS sTORE_PRODUCTS)
        {
            List<SelectListItem> options = new SelectList(db.STORE_CATEGORY.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.CATEGORY_ID).ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Category" });
            ViewBag.CATEGORY_ID = options;

            List<SelectListItem> options2 = new SelectList(db.STORE_SUB_CATEGORY.Where(x => x.IS_DEL == false && x.STORE_CATEGORY_ID == sTORE_PRODUCTS.CATEGORY_ID).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.SUB_CATEGORY_ID).ToList();
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Sub Category" });
            ViewBag.SUB_CATEGORY_ID = options2;

            List<SelectListItem> options3 = new SelectList(db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.BRAND_ID).ToList();
            options3.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Brand" });
            ViewBag.BRAND_ID = options3;

            var product_brands = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_brands"] = product_brands;

            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.VENDOR_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;


            if (ModelState.IsValid)
            {
                sTORE_PRODUCTS.IS_DEL = false;
                sTORE_PRODUCTS.CREATED_AT = DateTime.Now;
                sTORE_PRODUCTS.UPDATED_AT = DateTime.Now;
                db.STORE_PRODUCTS.Add(sTORE_PRODUCTS);
                try { db.SaveChanges(); ViewBag.Notice = "New product added in system successfully."; }
                catch (DbEntityValidationException e) {foreach (var eve in e.EntityValidationErrors){foreach (var ve in eve.ValidationErrors){ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);}}
                    return View();
                }
                catch (Exception e) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View();
                }
                return RedirectToAction("ViewAll", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State not valid. Please try again.";
            return View(sTORE_PRODUCTS);
        }

        // GET: Store/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_PRODUCTS sTORE_PRODUCTS = db.STORE_PRODUCTS.Find(id);
            if (sTORE_PRODUCTS == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> options = new SelectList(db.STORE_CATEGORY.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.ID), "ID", "NAME", sTORE_PRODUCTS.CATEGORY_ID).ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Category" });
            ViewBag.CATEGORY_ID = options;

            List<SelectListItem> options2 = new SelectList(db.STORE_SUB_CATEGORY.Where(x => x.IS_DEL == false && x.STORE_CATEGORY_ID == sTORE_PRODUCTS.CATEGORY_ID).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.SUB_CATEGORY_ID).ToList();
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Sub Category" });
            ViewBag.SUB_CATEGORY_ID = options2;

            List<SelectListItem> options3 = new SelectList(db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.BRAND_ID).ToList();
            options3.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Brand" });
            ViewBag.BRAND_ID = options3;

            var product_brands = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_brands"] = product_brands;


            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.VENDOR_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            ViewBag.PAID_BY = sTORE_PRODUCTS.PAID_BY;
            DateTime PDate = Convert.ToDateTime(sTORE_PRODUCTS.PURCHASED_ON);
            ViewBag.PURCHASED_ON = PDate.ToShortDateString();
            return View(sTORE_PRODUCTS);
        }

        // POST: Store/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PRODUCT_ID,NAME,CATEGORY_ID,SUB_CATEGORY_ID,BRAND_ID,TOTAL_UNIT,TOTAL_COST,COST_PER_UNIT,SELL_PRICE_PER_UNIT,PURCHASED_ON,VENDOR_ID,PAID_BY,UNIT_LEFT,IS_ACT")] STORE_PRODUCTS sTORE_PRODUCTS)
        {
            List<SelectListItem> options = new SelectList(db.STORE_CATEGORY.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.ID), "ID", "NAME", sTORE_PRODUCTS.CATEGORY_ID).ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Category" });
            ViewBag.CATEGORY_ID = options;

            List<SelectListItem> options2 = new SelectList(db.STORE_SUB_CATEGORY.Where(x => x.IS_DEL == false && x.STORE_CATEGORY_ID == sTORE_PRODUCTS.CATEGORY_ID).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.SUB_CATEGORY_ID).ToList();
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Sub Category" });
            ViewBag.SUB_CATEGORY_ID = options2;

            List<SelectListItem> options3 = new SelectList(db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.BRAND_ID).ToList();
            options3.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Brand" });
            ViewBag.BRAND_ID = options3;

            var product_brands = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_brands"] = product_brands;

            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.VENDOR_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            ViewBag.PAID_BY = sTORE_PRODUCTS.PAID_BY;
            DateTime PDateIner = Convert.ToDateTime(sTORE_PRODUCTS.PURCHASED_ON);
            ViewBag.PURCHASED_ON = PDateIner.ToString("dd/mm/yyyy");

            if (ModelState.IsValid)
            {
                STORE_PRODUCTS sTORE_PRODUCTS_UPD = db.STORE_PRODUCTS.Find(sTORE_PRODUCTS.PRODUCT_ID);
                sTORE_PRODUCTS_UPD.NAME = sTORE_PRODUCTS.NAME;
                sTORE_PRODUCTS_UPD.CATEGORY_ID = sTORE_PRODUCTS.CATEGORY_ID;
                sTORE_PRODUCTS_UPD.SUB_CATEGORY_ID = sTORE_PRODUCTS.SUB_CATEGORY_ID;
                sTORE_PRODUCTS_UPD.BRAND_ID = sTORE_PRODUCTS.BRAND_ID;
                sTORE_PRODUCTS_UPD.TOTAL_UNIT = sTORE_PRODUCTS.TOTAL_UNIT;
                sTORE_PRODUCTS_UPD.TOTAL_COST = sTORE_PRODUCTS.TOTAL_COST;
                sTORE_PRODUCTS_UPD.COST_PER_UNIT = sTORE_PRODUCTS.COST_PER_UNIT;
                sTORE_PRODUCTS_UPD.SELL_PRICE_PER_UNIT = sTORE_PRODUCTS.SELL_PRICE_PER_UNIT;
                sTORE_PRODUCTS_UPD.PURCHASED_ON = sTORE_PRODUCTS.PURCHASED_ON;
                sTORE_PRODUCTS_UPD.VENDOR_ID = sTORE_PRODUCTS.VENDOR_ID;
                sTORE_PRODUCTS_UPD.PAID_BY = sTORE_PRODUCTS.PAID_BY;
                sTORE_PRODUCTS_UPD.UNIT_LEFT = sTORE_PRODUCTS.UNIT_LEFT;
                sTORE_PRODUCTS_UPD.IS_ACT = sTORE_PRODUCTS.IS_ACT;
                sTORE_PRODUCTS_UPD.UPDATED_AT = DateTime.Now;
                db.Entry(sTORE_PRODUCTS_UPD).State = EntityState.Modified;

                try { db.SaveChanges(); ViewBag.Notice = string.Concat(ViewBag.Notice, "Product details updated successfully."); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(sTORE_PRODUCTS);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(sTORE_PRODUCTS);
                }
                return RedirectToAction("ViewAll", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            return View(sTORE_PRODUCTS);
        }

        // GET: Store/Edit/5
        public ActionResult New_Procurement(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //STORE_PRODUCTS sTORE_PRODUCTS = db.STORE_PRODUCTS.Find(id);
            var sTORE_PRODUCTS = (from prd in db.STORE_PRODUCTS
                                  join cat in db.STORE_CATEGORY on prd.CATEGORY_ID equals cat.ID
                                  join subcat in db.STORE_SUB_CATEGORY on prd.SUB_CATEGORY_ID equals subcat.ID
                                  join brd in db.STORE_BRAND on prd.BRAND_ID equals brd.ID
                                  where prd.PRODUCT_ID == id
                                  select new SFSAcademy.Models.Products { ProductData = prd, CategoryData = cat, SubCategoryData = subcat, BrandData = brd }).Distinct().FirstOrDefault();
            if (sTORE_PRODUCTS == null)
            {
                return HttpNotFound();
            }


            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.ProductData.VENDOR_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            ViewBag.PAID_BY = sTORE_PRODUCTS.ProductData.PAID_BY;
            DateTime PDate = Convert.ToDateTime(sTORE_PRODUCTS.ProductData.PURCHASED_ON);
            ViewBag.PURCHASED_ON = PDate.ToShortDateString();
            return View(sTORE_PRODUCTS);
        }

        // POST: Store/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New_Procurement(SFSAcademy.Models.Products sTORE_PRODUCTS, int? VENDOR_ID)
        {          
            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTORE_PRODUCTS.ProductData.VENDOR_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            ViewBag.PAID_BY = sTORE_PRODUCTS.ProductData.PAID_BY;
            DateTime PDateIner = Convert.ToDateTime(sTORE_PRODUCTS.ProductData.PURCHASED_ON);
            ViewBag.PURCHASED_ON = PDateIner.ToString("dd/mm/yyyy");

            if (ModelState.IsValid)
            {
                STORE_PRODUCTS sTORE_PRODUCTS_UPD = db.STORE_PRODUCTS.Find(sTORE_PRODUCTS.ProductData.PRODUCT_ID);
                sTORE_PRODUCTS_UPD.TOTAL_UNIT = sTORE_PRODUCTS.ProductData.TOTAL_UNIT;
                sTORE_PRODUCTS_UPD.TOTAL_COST = sTORE_PRODUCTS.ProductData.TOTAL_COST;
                sTORE_PRODUCTS_UPD.COST_PER_UNIT = sTORE_PRODUCTS.ProductData.COST_PER_UNIT;
                sTORE_PRODUCTS_UPD.SELL_PRICE_PER_UNIT = sTORE_PRODUCTS.ProductData.SELL_PRICE_PER_UNIT;
                sTORE_PRODUCTS_UPD.PURCHASED_ON = sTORE_PRODUCTS.ProductData.PURCHASED_ON;
                sTORE_PRODUCTS_UPD.VENDOR_ID = VENDOR_ID;
                sTORE_PRODUCTS_UPD.PAID_BY = sTORE_PRODUCTS.ProductData.PAID_BY;
                sTORE_PRODUCTS_UPD.UNIT_LEFT = sTORE_PRODUCTS_UPD.UNIT_LEFT + sTORE_PRODUCTS.ProductData.TOTAL_UNIT;
                sTORE_PRODUCTS_UPD.IS_ACT = true;
                sTORE_PRODUCTS_UPD.IS_DEL = false;
                sTORE_PRODUCTS_UPD.CREATED_AT = DateTime.Now;
                sTORE_PRODUCTS_UPD.UPDATED_AT = DateTime.Now;
                db.STORE_PRODUCTS.Add(sTORE_PRODUCTS_UPD);
                try { db.SaveChanges(); ViewBag.Notice = string.Concat(ViewBag.Notice, "Product details updated successfully."); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(sTORE_PRODUCTS);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(sTORE_PRODUCTS);
                }

                STORE_PRODUCTS sTORE_PRODUCTS_ORG = db.STORE_PRODUCTS.Find(sTORE_PRODUCTS.ProductData.PRODUCT_ID);
                sTORE_PRODUCTS_ORG.IS_ACT = false;
                sTORE_PRODUCTS_ORG.UPDATED_AT = DateTime.Now;
                db.Entry(sTORE_PRODUCTS_ORG).State = EntityState.Modified;

                try { db.SaveChanges(); ViewBag.Notice = string.Concat(ViewBag.Notice, "Previous information saved."); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(sTORE_PRODUCTS);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(sTORE_PRODUCTS);
                }
                return RedirectToAction("ViewAll", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            return View(sTORE_PRODUCTS);
        }


        public ActionResult Delete(int id)
        {
            STORE_PRODUCTS sTORE_PRODUCTS = db.STORE_PRODUCTS.Find(id);

            sTORE_PRODUCTS.IS_DEL = true;
            sTORE_PRODUCTS.IS_ACT = false;
            sTORE_PRODUCTS.UPDATED_AT = DateTime.Now;
            db.Entry(sTORE_PRODUCTS).State = EntityState.Modified;
            try { db.SaveChanges(); ViewBag.Notice = string.Concat(ViewBag.Notice, "Product Deleted Successfully."); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                return View(sTORE_PRODUCTS);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                return View(sTORE_PRODUCTS);
            }

            return RedirectToAction("ViewAll", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Student
        public ActionResult ViewAll(string CATEGORY_ID, string SUB_CATEGORY_ID, string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            ViewBag.CartItems = (from t in db.STORE_PURCHAGE_CART
                                 select t).Count();
            List<SelectListItem> options = new SelectList(db.STORE_CATEGORY.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.ID), "ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Category" });
            ViewBag.CATEGORY_ID = options;

            List<SelectListItem> options2 = null;
            if(!string.IsNullOrEmpty(CATEGORY_ID))
            {
                int? CATEGORY_ID_VAL = Convert.ToInt32(CATEGORY_ID);
                options2 = new SelectList(db.STORE_SUB_CATEGORY.Where(x => x.IS_DEL == false && x.STORE_CATEGORY_ID == CATEGORY_ID_VAL).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            }
            else
            {
                options2 = new SelectList(db.STORE_SUB_CATEGORY.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            }
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Sub Category" });
            ViewBag.SUB_CATEGORY_ID = options2;

            return View();
        }

        // GET: Store Products
        public ActionResult ListAllProducts(string sortOrder, string currentFilter, string CATEGORY_ID, string currentFilter2, string SUB_CATEGORY_ID, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            int? CATEGORY_ID_VAL = null;
            int? SUB_CATEGORY_ID_VAL = null;

            if (!string.IsNullOrEmpty(CATEGORY_ID) && CATEGORY_ID != "-1")
            {
                page = 1;
                CATEGORY_ID_VAL = Convert.ToInt32(CATEGORY_ID);
            } 
            else 
            {
                CATEGORY_ID = currentFilter;
            }

            ViewBag.CurrentFilter = CATEGORY_ID;
            if (!string.IsNullOrEmpty(SUB_CATEGORY_ID) && SUB_CATEGORY_ID != "-1") { page = 1; SUB_CATEGORY_ID_VAL = Convert.ToInt32(SUB_CATEGORY_ID); }
            else { SUB_CATEGORY_ID = currentFilter2; }
            ViewBag.CurrentFilter2 = SUB_CATEGORY_ID;

            var ProductS = (from pd in db.STORE_PRODUCTS.Include(x=>x.STORE_BRAND)
                            join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                            join subcat in db.STORE_SUB_CATEGORY on pd.SUB_CATEGORY_ID equals subcat.ID into gsc
                            from subgsc in gsc.DefaultIfEmpty()
                            orderby pd.NAME, ct.NAME
                            where pd.IS_DEL == false && pd.IS_ACT == true
                            select new Models.Products { ProductData = pd, CategoryData = ct, SubCategoryData = (subgsc == null ? null : subgsc) }).Distinct();

            if (!String.IsNullOrEmpty(CATEGORY_ID))
            {
                ProductS = ProductS.Where(s => s.CategoryData.ID == CATEGORY_ID_VAL);
            }
            if (!String.IsNullOrEmpty(SUB_CATEGORY_ID))
            {
                ProductS = ProductS.Where(s => s.SubCategoryData.ID == SUB_CATEGORY_ID_VAL);
            }
            switch (sortOrder)
            {
                case "name_desc":
                    ProductS = ProductS.OrderByDescending(s => s.ProductData.NAME);
                    break;
                case "Date":
                    ProductS = ProductS.OrderBy(s => s.ProductData.PURCHASED_ON);
                    break;
                case "date_desc":
                    ProductS = ProductS.OrderByDescending(s => s.ProductData.PURCHASED_ON);
                    break;
                default:  // Name ascending 
                    ProductS = ProductS.OrderBy(s => s.ProductData.NAME);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(ProductS.ToPagedList(pageNumber, pageSize));
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
            var sTOREcATEGORY = db.STORE_CATEGORY.Where(x=>x.IS_ACT==true && x.IS_DEL==false).OrderBy(x => x.NAME).ToList();
            return View(sTOREcATEGORY);
        }

        // GET: Student/Delete/5
        public ActionResult _stCategoriesDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_CATEGORY sTOREcATEGORY = db.STORE_CATEGORY.Find(id);
            if (sTOREcATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(sTOREcATEGORY);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("_stCategoriesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult _stCategoriesDeleteConfirmed(int id)
        {
            STORE_CATEGORY sTOREcATEGORY = db.STORE_CATEGORY.Find(id);
            db.STORE_CATEGORY.Remove(sTOREcATEGORY);
            try { db.SaveChanges(); ViewBag.CatDeleteMessage = "Store Category deleted successfully."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.CatDeleteMessage = e.InnerException.InnerException.Message; }
            return View(sTOREcATEGORY);
        }

        // GET: Student/Edit/5
        public ActionResult _CategoriesEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_CATEGORY sTOREcATEGORY = db.STORE_CATEGORY.Find(id);
            if (sTOREcATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(sTOREcATEGORY);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CategoriesEdit([Bind(Include = "ID,NAME,IS_ACT")] STORE_CATEGORY sTOREcATEGORY)
        {
            if (ModelState.IsValid)
            {
                STORE_CATEGORY sTOREcATEGORY_UPD = db.STORE_CATEGORY.Find(sTOREcATEGORY.ID);
                sTOREcATEGORY_UPD.NAME = sTOREcATEGORY.NAME;
                sTOREcATEGORY_UPD.IS_ACT = sTOREcATEGORY.IS_ACT;
                db.Entry(sTOREcATEGORY_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.CatEditMessage = "Store Category edited successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.CatEditMessage = e.InnerException.InnerException.Message; }
                return View(sTOREcATEGORY);
            }
            return View(sTOREcATEGORY);
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoriesCreate([Bind(Include = "ID,NAME,IS_ACT")] STORE_CATEGORY sTOREcATEGORY)
        {
            if (ModelState.IsValid)
            {
                sTOREcATEGORY.IS_DEL =false;
                sTOREcATEGORY.CREATED_AT = System.DateTime.Now;
                sTOREcATEGORY.UPDATED_AT = System.DateTime.Now;
                db.STORE_CATEGORY.Add(sTOREcATEGORY);
                try { db.SaveChanges(); ViewBag.CatCreateMessage = "Store Category created successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.CatCreateMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("Categories");
            }

            return View(sTOREcATEGORY);
        }

        // GET: Student/Details/5
        public ActionResult SubCategories(int Category_Id)
        {
            ViewBag.STORE_CATEGORY_ID = Category_Id;
            ViewBag.CATEGORY_ID = new SelectList(db.STORE_CATEGORY, "ID", "NAME", Category_Id);
            return View();
        }

        // GET: Student/Details/5
        [ChildActionOnly]
        public ActionResult _SubCategoriesList(int Category_Id)
        {
            var sTOREsUBcATEGORY = (from subcat in db.STORE_SUB_CATEGORY
                                    join cat in db.STORE_CATEGORY on subcat.STORE_CATEGORY_ID equals cat.ID
                                    where cat.ID == Category_Id
                                    select new Models.SubCategory { SubCategoryData = subcat, CategoryData = cat })
                                    .OrderBy(x => x.SubCategoryData.NAME).ToList();

            return View(sTOREsUBcATEGORY);
        }

        // GET: Student/Delete/5
        public ActionResult _SubCategoriesDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_SUB_CATEGORY sTOREsUBcATEGORY = db.STORE_SUB_CATEGORY.Find(id);
            if (sTOREsUBcATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(sTOREsUBcATEGORY);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("_SubCategoriesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult _SubCategoriesDeleteConfirmed(int id)
        {
            STORE_SUB_CATEGORY sTOREsUBcATEGORY = db.STORE_SUB_CATEGORY.Find(id);
            db.STORE_SUB_CATEGORY.Remove(sTOREsUBcATEGORY);
            try { db.SaveChanges(); ViewBag.SubCatDeteleMessage = "Store Sub Category deleted successfully."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.SubCatDeteleMessage = e.InnerException.InnerException.Message; }
            return View(sTOREsUBcATEGORY);
        }

        // GET: Student/Edit/5
        public ActionResult _SubCategoriesEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_SUB_CATEGORY sTOREsUBcATEGORY = db.STORE_SUB_CATEGORY.Find(id);
            if (sTOREsUBcATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(sTOREsUBcATEGORY);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _SubCategoriesEdit([Bind(Include = "ID,STORE_CATEGORY_ID,NAME,IS_ACT")] STORE_SUB_CATEGORY sTOREsUBcATEGORY)
        {
            if (ModelState.IsValid)
            {
                STORE_SUB_CATEGORY sTOREsUBcATEGORY_UPD = db.STORE_SUB_CATEGORY.Find(sTOREsUBcATEGORY.ID);
                sTOREsUBcATEGORY_UPD.NAME = sTOREsUBcATEGORY.NAME;
                sTOREsUBcATEGORY_UPD.IS_ACT = sTOREsUBcATEGORY.IS_ACT;
                db.Entry(sTOREsUBcATEGORY_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.SubCatEditMessage = "Store Sub Category edited successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.SubCatEditMessage = e.InnerException.InnerException.Message; }
                return View(sTOREsUBcATEGORY);
            }
            return View(sTOREsUBcATEGORY);
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubCategoriesCreate([Bind(Include = "ID,STORE_CATEGORY_ID,NAME,IS_ACT")] STORE_SUB_CATEGORY sTOREsUBcATEGORY, string CATEGORY_ID)
        {
            if (ModelState.IsValid)
            {
                sTOREsUBcATEGORY.STORE_CATEGORY_ID = Convert.ToInt32(CATEGORY_ID);
                sTOREsUBcATEGORY.IS_DEL = false;
                sTOREsUBcATEGORY.CREATED_AT = System.DateTime.Now;
                sTOREsUBcATEGORY.UPDATED_AT = System.DateTime.Now;
                db.STORE_SUB_CATEGORY.Add(sTOREsUBcATEGORY);
                try { db.SaveChanges(); ViewBag.SubCatCreateMessage = "Sub Category created successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.SubCatCreateMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("SubCategories", new { Category_Id = sTOREsUBcATEGORY.STORE_CATEGORY_ID });
            }

            return View(sTOREsUBcATEGORY);
        }

        // GET: Purchase/Create
        public ActionResult Purchase(int? QUANTY, int? PRODUCT_ID)
        {
            if (QUANTY >= 1)
            {
                int? SoldBy = Convert.ToInt32(this.Session["UserId"]);
                STORE_PRODUCTS sTOREpRODUCT = db.STORE_PRODUCTS.Find(PRODUCT_ID);
                var sTORE_PURCHAGE_cART = new STORE_PURCHAGE_CART() { PRODUCT_ID = sTOREpRODUCT.PRODUCT_ID, UNIT_SOLD = QUANTY, SOLD_PRICE = sTOREpRODUCT.SELL_PRICE_PER_UNIT * QUANTY, SOLD_BY_ID = SoldBy, SOLD_ON = DateTime.Now, STUDENT_ID = null, STUDENT_CONTACT_NO = null, MONEY_RECEIVED_BY_ID = SoldBy, IS_DEPOSITED = false, IS_ACT = true, IS_DEL = false, CREATED_AT = DateTime.Now, UPDATED_AT = DateTime.Now };
                db.STORE_PURCHAGE_CART.Add(sTORE_PURCHAGE_cART);

                sTOREpRODUCT.UPDATED_AT = DateTime.Now;
                sTOREpRODUCT.UNIT_LEFT = sTOREpRODUCT.UNIT_LEFT - QUANTY;
                db.Entry(sTOREpRODUCT).State = EntityState.Modified;
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e) {foreach (var eve in e.EntityValidationErrors){ foreach (var ve in eve.ValidationErrors){ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);}}
                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Quantity Entered is not appropriate.";
            }
            ViewBag.CartItems = (from t in db.STORE_PURCHAGE_CART
                                 select t).Count();
            return RedirectToAction("ViewAll",new { ErrorMessage = ViewBag.ErrorMessage });
        }

        // POST: Purchase/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Purchase([Bind(Include = "ID,PRODUCT_ID,UNIT_SOLD,SOLD_PRICE,SOLD_BY_ID,SOLD_ON,STUDENT_ID,STUDENT_CONTACT_NO,MONEY_RECEIVED_BY_ID,IS_DEPOSITED,IS_ACT,IS_DEL,CREATED_AT,UPDATED_AT")] STORE_PURCHAGE_CART sTORE_PURCHAGE_cART)
        {
            if (ModelState.IsValid)
            {
                sTORE_PURCHAGE_cART.IS_DEL = false;
                sTORE_PURCHAGE_cART.CREATED_AT = DateTime.Now;
                sTORE_PURCHAGE_cART.UPDATED_AT = DateTime.Now;
                db.STORE_PURCHAGE_CART.Add(sTORE_PURCHAGE_cART);
                db.SaveChanges();
                STORE_PRODUCTS sTOREpRODUCT = db.STORE_PRODUCTS.Find(sTORE_PURCHAGE_cART.PRODUCT_ID);
                sTOREpRODUCT.UPDATED_AT = DateTime.Now;
                sTOREpRODUCT.UNIT_LEFT = sTOREpRODUCT.UNIT_LEFT - sTORE_PURCHAGE_cART.UNIT_SOLD;
                db.Entry(sTOREpRODUCT).State = EntityState.Modified;
                db.SaveChanges();

                ViewBag.CartItems = (from t in db.STORE_PURCHAGE_CART
                                                 select t).Count();
                return RedirectToAction("ViewAll");
            }

            ViewBag.PRODUCT_ID = new SelectList(db.STORE_PRODUCTS, "PRODUCT_ID", "NAME", sTORE_PURCHAGE_cART.PRODUCT_ID);
            return View(sTORE_PURCHAGE_cART);
        }


        // GET: Student
        public ActionResult ViewAllSelling(string Notice, string ErrorMessage, string sortOrder, int? currentFilter, int? PRODUCT_ID, int? page, int? currentFilter2, int? STUDENT_ID, string currentFilter3, string ContactNumber, int? currentFilter4, int? USER_ID, int? currentFilter5, int? MoneyDeposited, string currentFilter9, string SoldFromDate, string currentFilter10, string SoldToDate, int? currentFilter11, int? IncludeBackup)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;

            ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";

            if (PRODUCT_ID != null && PRODUCT_ID != -1) { page = 1; }
            else { PRODUCT_ID = currentFilter; }

            ViewBag.CurrentFilter = PRODUCT_ID;
            if (STUDENT_ID != null && STUDENT_ID != -1) { page = 1; }
            else { STUDENT_ID = currentFilter2; }
            ViewBag.CurrentFilter2 = STUDENT_ID;
            if (!String.IsNullOrEmpty(ContactNumber)) { page = 1; }
            else { ContactNumber = currentFilter3; }
            ViewBag.CurrentFilter3 = ContactNumber;
            if (USER_ID != null && USER_ID != -1) { page = 1; }
            else { USER_ID = currentFilter4; }
            ViewBag.CurrentFilter4 = USER_ID;
            if (MoneyDeposited != null && MoneyDeposited != -1) { page = 1; }
            else { MoneyDeposited = currentFilter5; }
            ViewBag.CurrentFilter5 = MoneyDeposited;
            if (!String.IsNullOrEmpty(SoldFromDate))
            {
                page = 1;
            }
            else { SoldFromDate = currentFilter9; }
            DateTime? dFrom; DateTime dtFrom;
            dFrom = DateTime.TryParse(SoldFromDate, out dtFrom) ? dtFrom : (DateTime?)null;
            ViewBag.CurrentFilter9 = SoldFromDate;
            if (!String.IsNullOrEmpty(SoldToDate))
            {
                page = 1;
            }
            else { SoldToDate = currentFilter10; }
            DateTime? dTo; DateTime dtTo;
            dTo = DateTime.TryParse(SoldToDate, out dtTo) ? dtTo : (DateTime?)null;
            ViewBag.CurrentFilter10 = SoldToDate;
            if (IncludeBackup != null && IncludeBackup != -1) { page = 1; }
            else { IncludeBackup = currentFilter11; }
            ViewBag.CurrentFilter11 = IncludeBackup;
            IEnumerable<SFSAcademy.Models.Purchase> PurchaseS = Enumerable.Empty<SFSAcademy.Models.Purchase>();
            if (IncludeBackup == 1)
            {
                PurchaseS = (from pd in db.STORE_PRODUCTS
                                 join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                                 join pur in db.STORE_PURCHAGE on pd.PRODUCT_ID equals pur.PRODUCT_ID
                                 join std in db.STUDENTs on pur.STUDENT_ID equals std.ID into gstd
                                 from subgstd in gstd.DefaultIfEmpty()
                                 join usr in db.USERS on pur.SOLD_BY_ID equals usr.ID into gusr
                                 from subgusr in gusr.DefaultIfEmpty()
                                 orderby pur.SOLD_ON, pd.NAME, ct.NAME
                                 where pur.IS_DEL == false
                                 select new Models.Purchase { ID = pur.ID, UNIT_SOLD = pur.UNIT_SOLD, SOLD_PRICE = pur.SOLD_PRICE, SOLD_BY_ID = pur.SOLD_BY_ID, SOLD_ON = pur.SOLD_ON, IS_DEPOSITED = pur.IS_DEPOSITED, IS_BACKUP = false, STUDENT_ID = pur.STUDENT_ID, STUDENT_CONTACT_NO = pur.STUDENT_CONTACT_NO, MONEY_RECEIVED_BY_ID = pur.MONEY_RECEIVED_BY_ID, PurchaseData = pur, PurchaseBackupData = null, ProductData = pd, CategoryData = ct, StudentData = (subgstd == null ? null : subgstd), UserData = (subgusr == null ? null : subgusr) })
                                 .Union(from pd in db.STORE_PRODUCTS
                                        join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                                        join pur in db.STORE_PURCHAGE_BACKUP on pd.PRODUCT_ID equals pur.PRODUCT_ID
                                        join std in db.STUDENTs on pur.STUDENT_ID equals std.ID into gstd
                                        from subgstd in gstd.DefaultIfEmpty()
                                        join usr in db.USERS on pur.SOLD_BY_ID equals usr.ID into gusr
                                        from subgusr in gusr.DefaultIfEmpty()
                                        orderby pur.SOLD_ON, pd.NAME, ct.NAME
                                        where pur.IS_DEL == false
                                        select new Models.Purchase { ID=pur.ID, UNIT_SOLD = pur.UNIT_SOLD, SOLD_PRICE=pur.SOLD_PRICE, SOLD_BY_ID = pur.SOLD_BY_ID, SOLD_ON = pur.SOLD_ON, IS_DEPOSITED = pur.IS_DEPOSITED, IS_BACKUP = true, STUDENT_ID = pur.STUDENT_ID, STUDENT_CONTACT_NO = pur.STUDENT_CONTACT_NO, MONEY_RECEIVED_BY_ID = pur.MONEY_RECEIVED_BY_ID, PurchaseData = null, PurchaseBackupData = pur, ProductData = pd, CategoryData = ct, StudentData = (subgstd == null ? null : subgstd), UserData = (subgusr == null ? null : subgusr) }).Distinct();
            }
            else
            {
                PurchaseS = (from pd in db.STORE_PRODUCTS
                                 join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                                 join pur in db.STORE_PURCHAGE on pd.PRODUCT_ID equals pur.PRODUCT_ID
                                 join std in db.STUDENTs on pur.STUDENT_ID equals std.ID into gstd
                                 from subgstd in gstd.DefaultIfEmpty()
                                 join usr in db.USERS on pur.SOLD_BY_ID equals usr.ID into gusr
                                 from subgusr in gusr.DefaultIfEmpty()
                                 orderby pur.SOLD_ON, pd.NAME, ct.NAME
                                 where pur.IS_DEL == false
                                 select new Models.Purchase { ID = pur.ID, UNIT_SOLD = pur.UNIT_SOLD, SOLD_PRICE = pur.SOLD_PRICE, SOLD_BY_ID = pur.SOLD_BY_ID, SOLD_ON = pur.SOLD_ON, IS_DEPOSITED = pur.IS_DEPOSITED, IS_BACKUP = false, STUDENT_ID = pur.STUDENT_ID, STUDENT_CONTACT_NO = pur.STUDENT_CONTACT_NO, MONEY_RECEIVED_BY_ID = pur.MONEY_RECEIVED_BY_ID, PurchaseData = pur, PurchaseBackupData = null, ProductData = pd, CategoryData = ct, StudentData = (subgstd == null ? null : subgstd), UserData = (subgusr == null ? null : subgusr) }).Distinct();
            }
            

            if (PRODUCT_ID != null && PRODUCT_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.ProductData.PRODUCT_ID == PRODUCT_ID);
            }
            if (STUDENT_ID != null && STUDENT_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.STUDENT_ID == STUDENT_ID);
            }
            if (!String.IsNullOrEmpty(ContactNumber))
            {
                long? StdContactNum = Convert.ToInt64(ContactNumber);
                PurchaseS = PurchaseS.Where(s => s.STUDENT_CONTACT_NO == StdContactNum);
            }
            if (USER_ID != null && USER_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.SOLD_BY_ID == USER_ID);
            }
            if (MoneyDeposited != null && MoneyDeposited != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.IS_DEPOSITED.Equals(MoneyDeposited == 1 ? true : false));
            }
            if (!String.IsNullOrEmpty(SoldFromDate) && !String.IsNullOrEmpty(SoldToDate))
            {
                PurchaseS = PurchaseS.Where(s => s.SOLD_ON >= dFrom).Where(s => s.SOLD_ON <= dTo);
            }
            switch (sortOrder)
            {
                case "date_desc":
                    PurchaseS = PurchaseS.OrderBy(s => s.SOLD_ON);
                    break;
                case "Name":
                    PurchaseS = PurchaseS.OrderBy(s => s.ProductData.NAME);
                    break;
                case "name_desc":
                    PurchaseS = PurchaseS.OrderByDescending(s => s.ProductData.NAME);
                    break;
                default:  // Name ascending 
                    PurchaseS = PurchaseS.OrderByDescending(s => s.SOLD_ON);
                    break;
            }
            var queryStudent = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in queryStudent)
            {
                string StudentFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = StudentFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == currentFilter2 ? true : false;
                options2.Add(result);
            }
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Student" });
            ViewBag.STUDENT_ID = options2;

            var student_select = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["student_select"] = student_select;

            List<SelectListItem> options4 = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", currentFilter).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product" });
            ViewBag.PRODUCT_ID = options4;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            List<SelectListItem> options3 = new List<SelectListItem>();
            options3.Insert(0, new SelectListItem() { Value = null, Text = "Select Value", Selected = MoneyDeposited == -1 || MoneyDeposited == null ? true : false });
            options3.Insert(1, new SelectListItem() { Value = "1", Text = "Yes", Selected = MoneyDeposited == 1 ? true : false });
            options3.Insert(2, new SelectListItem() { Value = "0", Text = "No", Selected = MoneyDeposited == 0 ? true : false });
            ViewBag.MoneyDeposited = options3;

            List<SelectListItem> options5 = new List<SelectListItem>();
            options5.Insert(0, new SelectListItem() { Value = null, Text = "Select Value", Selected = IncludeBackup == -1 || IncludeBackup == null ? true : false });
            options5.Insert(1, new SelectListItem() { Value = "1", Text = "Yes", Selected = IncludeBackup == 1 ? true : false });
            options5.Insert(2, new SelectListItem() { Value = "0", Text = "No", Selected = IncludeBackup == 0 ? true : false });
            ViewBag.IncludeBackup = options5;

            var queryUser = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options6 = new List<SelectListItem>();
            foreach (var item in queryUser)
            {
                string UserFullName = string.Concat(item.FIRST_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = UserFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == currentFilter4 ? true : false;
                options6.Add(result);
            }
            options6.Insert(0, new SelectListItem() { Value = null, Text = "Select User" });
            ViewBag.USER_ID = options6;

            var user_select = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["user_select"] = user_select;

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(PurchaseS.ToPagedList(pageNumber, pageSize));
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpGet]
        public ActionResult User_Select(int? id)
        {
            var queryUser = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options6 = new List<SelectListItem>();
            foreach (var item in queryUser)
            {
                string UserFullName = string.Concat(item.FIRST_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = UserFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == id ? true : false;
                options6.Add(result);
            }
            options6.Insert(0, new SelectListItem() { Value = null, Text = "Select User" });
            ViewBag.USER_ID = options6;

            var user_select = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["user_select"] = user_select;

            return PartialView("_user_select", user_select);
        }

        // GET: Student
        [HttpGet]
        public void PurchasePdf(string sortOrder, int? PRODUCT_ID, int? STUDENT_ID, string ContactNumber, int? USER_ID, int? MoneyDeposited, string SoldFromDate, string SoldToDate, int? IncludeBackup)
        {
            ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";

            DateTime? dFrom; DateTime dtFrom;
            dFrom = DateTime.TryParse(SoldFromDate, out dtFrom) ? dtFrom : (DateTime?)null;            
            DateTime? dTo; DateTime dtTo;
            dTo = DateTime.TryParse(SoldToDate, out dtTo) ? dtTo : (DateTime?)null;
            ViewBag.CurrentFilter10 = SoldToDate;

            IEnumerable<SFSAcademy.Models.Purchase> PurchaseS = Enumerable.Empty<SFSAcademy.Models.Purchase>();
            if (IncludeBackup == 1)
            {
                PurchaseS = (from pd in db.STORE_PRODUCTS
                             join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                             join pur in db.STORE_PURCHAGE on pd.PRODUCT_ID equals pur.PRODUCT_ID
                             join std in db.STUDENTs on pur.STUDENT_ID equals std.ID into gstd
                             from subgstd in gstd.DefaultIfEmpty()
                             join usr in db.USERS on pur.SOLD_BY_ID equals usr.ID into gusr
                             from subgusr in gusr.DefaultIfEmpty()
                             orderby pur.SOLD_ON, pd.NAME, ct.NAME
                             where pur.IS_DEL == false
                             select new Models.Purchase { ID = pur.ID, UNIT_SOLD = pur.UNIT_SOLD, SOLD_PRICE = pur.SOLD_PRICE, SOLD_BY_ID = pur.SOLD_BY_ID, SOLD_ON = pur.SOLD_ON, IS_DEPOSITED = pur.IS_DEPOSITED, IS_BACKUP = false, STUDENT_ID = pur.STUDENT_ID, STUDENT_CONTACT_NO = pur.STUDENT_CONTACT_NO, MONEY_RECEIVED_BY_ID = pur.MONEY_RECEIVED_BY_ID, PurchaseBackupData = null, PurchaseData = pur, ProductData = pd, CategoryData = ct, StudentData = (subgstd == null ? null : subgstd), UserData = (subgusr == null ? null : subgusr) })
                                 .Union(from pd in db.STORE_PRODUCTS
                                        join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                                        join pur in db.STORE_PURCHAGE_BACKUP on pd.PRODUCT_ID equals pur.PRODUCT_ID
                                        join std in db.STUDENTs on pur.STUDENT_ID equals std.ID into gstd
                                        from subgstd in gstd.DefaultIfEmpty()
                                        join usr in db.USERS on pur.SOLD_BY_ID equals usr.ID into gusr
                                        from subgusr in gusr.DefaultIfEmpty()
                                        orderby pur.SOLD_ON, pd.NAME, ct.NAME
                                        where pur.IS_DEL == false
                                        select new Models.Purchase { ID = pur.ID, UNIT_SOLD = pur.UNIT_SOLD, SOLD_PRICE = pur.SOLD_PRICE, SOLD_BY_ID = pur.SOLD_BY_ID, SOLD_ON = pur.SOLD_ON, IS_DEPOSITED = pur.IS_DEPOSITED, IS_BACKUP = true, STUDENT_ID = pur.STUDENT_ID, STUDENT_CONTACT_NO = pur.STUDENT_CONTACT_NO, MONEY_RECEIVED_BY_ID = pur.MONEY_RECEIVED_BY_ID, PurchaseBackupData = pur, PurchaseData = null, ProductData = pd, CategoryData = ct, StudentData = (subgstd == null ? null : subgstd), UserData = (subgusr == null ? null : subgusr) }).Distinct();
            }
            else
            {
                PurchaseS = (from pd in db.STORE_PRODUCTS
                             join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                             join pur in db.STORE_PURCHAGE on pd.PRODUCT_ID equals pur.PRODUCT_ID
                             join std in db.STUDENTs on pur.STUDENT_ID equals std.ID into gstd
                             from subgstd in gstd.DefaultIfEmpty()
                             join usr in db.USERS on pur.SOLD_BY_ID equals usr.ID into gusr
                             from subgusr in gusr.DefaultIfEmpty()
                             orderby pur.SOLD_ON, pd.NAME, ct.NAME
                             where pur.IS_DEL == false
                             select new Models.Purchase { ID = pur.ID, UNIT_SOLD = pur.UNIT_SOLD, SOLD_PRICE = pur.SOLD_PRICE, SOLD_BY_ID = pur.SOLD_BY_ID, SOLD_ON = pur.SOLD_ON, IS_DEPOSITED = pur.IS_DEPOSITED, IS_BACKUP = false, STUDENT_CONTACT_NO = pur.STUDENT_CONTACT_NO, MONEY_RECEIVED_BY_ID = pur.MONEY_RECEIVED_BY_ID, PurchaseBackupData = null, PurchaseData = pur, ProductData = pd, CategoryData = ct, StudentData = (subgstd == null ? null : subgstd), UserData = (subgusr == null ? null : subgusr) }).Distinct();
            }

            if (PRODUCT_ID != null && PRODUCT_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.ProductData.PRODUCT_ID == PRODUCT_ID);
            }
            if (STUDENT_ID != null && STUDENT_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.STUDENT_ID == STUDENT_ID);
            }
            if (!String.IsNullOrEmpty(ContactNumber))
            {
                long? StdContactNum = Convert.ToInt64(ContactNumber);
                PurchaseS = PurchaseS.Where(s => s.STUDENT_CONTACT_NO == StdContactNum);
            }
            if (USER_ID != null && USER_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.SOLD_BY_ID == USER_ID);
            }
            if (MoneyDeposited != null && MoneyDeposited != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.IS_DEPOSITED.Equals(MoneyDeposited == 1 ? true : false));
            }
            if (!String.IsNullOrEmpty(SoldFromDate) && !String.IsNullOrEmpty(SoldToDate))
            {
                PurchaseS = PurchaseS.Where(s => s.SOLD_ON >= dFrom).Where(s => s.SOLD_ON <= dTo);
            }
            switch (sortOrder)
            {
                case "date_desc":
                    PurchaseS = PurchaseS.OrderBy(s => s.SOLD_ON);
                    break;
                case "Name":
                    PurchaseS = PurchaseS.OrderBy(s => s.ProductData.NAME);
                    break;
                case "name_desc":
                    PurchaseS = PurchaseS.OrderByDescending(s => s.ProductData.NAME);
                    break;
                default:  // Name ascending 
                    PurchaseS = PurchaseS.OrderByDescending(s => s.SOLD_ON);
                    break;
            }

            var PdfStoreS = (from res in PurchaseS
                               select new { PName = res.ProductData.NAME, ContactNum = res.STUDENT_CONTACT_NO, RecBy = res.MONEY_RECEIVED_BY_ID, MDepo = res.IS_DEPOSITED, SoldOn = res.SOLD_ON.Value.ToShortDateString(), IS_BACKUP = res.IS_BACKUP }).ToList();


            var configuration = new ReportConfiguration();
            //configuration.PageOrientation = PageSize.LETTER_LANDSCAPE.Rotate();
            configuration.LogoPath
                = Server.MapPath(Url.Content("~/Content/images/login/SF_Square_Logo-Small.jpg"));
            configuration.LogImageScalePercent = 50;
            configuration.ReportTitle
                = "S. F. Square Store Report";
            configuration.ReportSubTitle = "Result of Purchase Search";

            var report = new PdfTabularReport();
            report.ReportConfiguration = configuration;

            List<ReportColumn> columns = new List<ReportColumn>();
            columns.Add(new ReportColumn { ColumnName = "Sl. No.", Width = 100 });
            columns.Add(new ReportColumn { ColumnName = "Product Name", Width = 100 });
            columns.Add(new ReportColumn { ColumnName = "Student Contact", Width = 100 });
            columns.Add(new ReportColumn { ColumnName = "Money Received By", Width = 100 });
            columns.Add(new ReportColumn { ColumnName = "Is Money Deposited?", Width = 100 });
            columns.Add(new ReportColumn { ColumnName = "Sold On", Width = 100 });
            columns.Add(new ReportColumn { ColumnName = "Is Data Backed-up?", Width = 100 });

            var PdfStoreSI = new DataTable();

            PdfStoreSI.Columns.Add("Sl. No.", typeof(int));
            PdfStoreSI.Columns.Add("Product Name", typeof(string));
            PdfStoreSI.Columns.Add("Student Contact", typeof(string));
            PdfStoreSI.Columns.Add("Money Received By", typeof(string));
            PdfStoreSI.Columns.Add("Is Money Deposited?", typeof(string));
            PdfStoreSI.Columns.Add("Sold On", typeof(string));
            PdfStoreSI.Columns.Add("Is Data Backed-up?", typeof(string));

            int i = 1;
            foreach (var entity in PdfStoreS.ToList())
            {
                var row = PdfStoreSI.NewRow();
                row["Sl. No."] = i;
                row["Product Name"] = entity.PName;
                row["Student Contact"] = entity.ContactNum;
                row["Money Received By"] = entity.RecBy;
                row["Is Money Deposited?"] = entity.MDepo;
                row["Sold On"] = entity.SoldOn;
                row["Is Data Backed-up?"] = entity.IS_BACKUP;
                PdfStoreSI.Rows.Add(row);
                i = i + 1;
            }


            var stream = report.GetPdf(PdfStoreSI, columns);

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition",
                "attachment;filename=ExampleReport.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(stream.ToArray());
            Response.End();

        }


        // GET: Store/Edit/5
        public ActionResult EditSelling(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_PURCHAGE sTORE_PURCHASE = db.STORE_PURCHAGE.Find(id);
            if (sTORE_PURCHASE == null)
            {
                return HttpNotFound();
            }
            var queryStudent = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in queryStudent)
            {
                string StudentFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = StudentFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == sTORE_PURCHASE.STUDENT_ID ? true : false;
                options2.Add(result);
            }
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Student" });
            ViewBag.STUDENT_ID = options2;

            var student_select = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["student_select"] = student_select;

            List<SelectListItem> options4 = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", sTORE_PURCHASE.PRODUCT_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product" });
            ViewBag.PRODUCT_ID = options4;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            var queryUser = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options6 = new List<SelectListItem>();
            foreach (var item in queryUser)
            {
                string UserFullName = string.Concat(item.FIRST_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = UserFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == sTORE_PURCHASE.SOLD_BY_ID ? true : false;
                options6.Add(result);
            }
            options6.Insert(0, new SelectListItem() { Value = null, Text = "Select User" });
            ViewBag.USER_ID = options6;

            var user_select = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["user_select"] = user_select;

            DateTime SDate = Convert.ToDateTime(sTORE_PURCHASE.SOLD_ON);
            ViewBag.SOLD_ON = SDate.ToShortDateString();
            return View(sTORE_PURCHASE);
        }

        // POST: Store/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSelling([Bind(Include = "ID,PRODUCT_ID,UNIT_SOLD,SOLD_PRICE,SOLD_BY_ID,SOLD_ON,STUDENT_ID,STUDENT_CONTACT_NO,MONEY_RECEIVED_BY_ID,IS_DEPOSITED,IS_ACT,IS_DEL")] STORE_PURCHAGE sTORE_PURCHASE, int? USER_ID)
        {
            var queryStudent = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in queryStudent)
            {
                string StudentFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = StudentFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == sTORE_PURCHASE.STUDENT_ID ? true : false;
                options2.Add(result);
            }
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Student" });
            ViewBag.STUDENT_ID = options2;

            var student_select = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["student_select"] = student_select;

            List<SelectListItem> options4 = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", sTORE_PURCHASE.PRODUCT_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product" });
            ViewBag.PRODUCT_ID = options4;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            var queryUser = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options6 = new List<SelectListItem>();
            foreach (var item in queryUser)
            {
                string UserFullName = string.Concat(item.FIRST_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = UserFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == USER_ID ? true : false;
                options6.Add(result);
            }
            options6.Insert(0, new SelectListItem() { Value = null, Text = "Select User" });
            ViewBag.USER_ID = options6;

            var user_select = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["user_select"] = user_select;
            DateTime SDate = Convert.ToDateTime(sTORE_PURCHASE.SOLD_ON);
            ViewBag.SOLD_ON = SDate.ToShortDateString();
            if (ModelState.IsValid)
            {
                STORE_PURCHAGE sTORE_PURCHASE_UPD = db.STORE_PURCHAGE.Find(sTORE_PURCHASE.ID);
                sTORE_PURCHASE_UPD.PRODUCT_ID = sTORE_PURCHASE.PRODUCT_ID;
                sTORE_PURCHASE_UPD.UNIT_SOLD = sTORE_PURCHASE.UNIT_SOLD;
                sTORE_PURCHASE_UPD.SOLD_PRICE = sTORE_PURCHASE.SOLD_PRICE;
                sTORE_PURCHASE_UPD.SOLD_BY_ID = USER_ID == -1? null : USER_ID;
                sTORE_PURCHASE_UPD.SOLD_ON = sTORE_PURCHASE.SOLD_ON;
                sTORE_PURCHASE_UPD.STUDENT_ID = sTORE_PURCHASE.STUDENT_ID == -1? null : sTORE_PURCHASE.STUDENT_ID;
                sTORE_PURCHASE_UPD.STUDENT_CONTACT_NO = sTORE_PURCHASE.STUDENT_CONTACT_NO;
                sTORE_PURCHASE_UPD.MONEY_RECEIVED_BY_ID = USER_ID == -1 ? null : USER_ID;
                sTORE_PURCHASE_UPD.IS_DEPOSITED = sTORE_PURCHASE.IS_DEPOSITED;
                sTORE_PURCHASE_UPD.IS_ACT = sTORE_PURCHASE.IS_ACT;
                sTORE_PURCHASE_UPD.UPDATED_AT = DateTime.Now;
                db.Entry(sTORE_PURCHASE_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.Notice = "Selling Details are updated successfully."; }
                catch (Exception e) { ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("ViewAllSelling", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage});
            }
            ViewBag.ErrorMessage = "Model State does not seems to be valid";
            return View(sTORE_PURCHASE);
        }

        // GET: Student
        public ActionResult ViewCart(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.PURCHAGE_DATE = DateTime.Now.ToShortDateString();
            ViewBag.CartItems = (from t in db.STORE_PURCHAGE_CART
                                 select t).Count();
            var TotalPriceSel = db.STORE_PURCHAGE_CART.GroupBy(o => o.ID)
                .Select(g => new { membername = g.Key, total = g.Sum(p => p.SOLD_PRICE) });
            int TCost = 0;
            foreach (var group in TotalPriceSel)
            {
                TCost = TCost+ Convert.ToInt32(group.total);
            }

            ViewBag.TotalPrice = TCost.ToString();

            var ProductS = (from pd in db.STORE_PRODUCTS
                            join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                            join pct in db.STORE_PURCHAGE_CART on pd.PRODUCT_ID equals pct.PRODUCT_ID
                            orderby pd.NAME, ct.NAME
                            select new Models.PurchaseCart { ProductData = pd, CategoryData = ct, PurchaseCartData = pct, UNIT_SOLD = pct.UNIT_SOLD, SOLD_AMNT = pct.SOLD_PRICE, PUR_DATE = pct.CREATED_AT }).Distinct();


            var queryStudent = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryStudent)
            {
                string StudentFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = StudentFullName;
                result.Value = item.ID.ToString();
                options.Add(result);
            }
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Student" });
            ViewBag.STUDENT_ID = options;

            var student_select = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x=>x.MID_NAME).ThenBy(x=>x.LAST_NAME).ToList();
            ViewData["student_select"] = student_select;

            return View(ProductS.ToList());
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpGet]
        public ActionResult Student_Select(int? id)
        {
            var queryStudent = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryStudent)
            {
                string StudentFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = StudentFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == id ? true : false;
                options.Add(result);
            }
            options.Insert(0, new SelectListItem() { Value = null, Text = "Select Student" });
            ViewBag.STUDENT_ID = options;

            var student_select = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["student_select"] = student_select;

            return PartialView("_student_select", student_select);
        }

        // GET: Student
        //[HttpGet]
        public ActionResult Payment(string PAYMENT_MODE, int? STUDENT_ID, decimal? PAYMENT_AMOUNT, long? PHONE_NUMBER, string PURCHAGE_DATE)
        {
            int? UserId = Convert.ToInt32(this.Session["UserId"]);
            ViewBag.PHONE_NUMBER = PHONE_NUMBER;
            DateTime PDate = Convert.ToDateTime(PURCHAGE_DATE);
            ViewBag.PURCHAGE_DATE = PDate.ToShortDateString();
            ViewBag.PAYMENT_MODE = PAYMENT_MODE;
            if(STUDENT_ID != null && STUDENT_ID != -1)
            {
                STUDENT std = db.STUDENTs.Find(STUDENT_ID);
                ViewBag.STUDENT_NAME = string.Concat(std.FIRST_NAME, " ", std.MID_NAME, " ", std.LAST_NAME);
            }
            else
            {
                STUDENT_ID = null;
                ViewBag.STUDENT_NAME = "External Sales";
            }
            ViewBag.CartItems = (from t in db.STORE_PURCHAGE_CART
                                 select t).Count();
            var TotalPriceSel = db.STORE_PURCHAGE_CART.GroupBy(o => o.ID)
                .Select(g => new { membername = g.Key, total = g.Sum(p => p.SOLD_PRICE) });
            int TCost = 0;
            foreach (var group in TotalPriceSel)
            {
                TCost = TCost + Convert.ToInt32(group.total);
            }

            ViewBag.PaidPrice = TCost.ToString();
            ViewBag.TotalPrice = 0;
            if(PHONE_NUMBER == null)
            { PHONE_NUMBER = 9967803589; }

            var IDList = new int[100];
            int i = 0;

            //var pURcART = (from res in db.STORE_PURCHAGE_CART
            //             select res).ToList();

            var paymentsOrg = from p in db.STORE_PURCHAGE_CART
                               //where p.CREATED_AT.Value.ToShortDateString() == DateTime.Now.ToShortDateString()
                           group p by p.PRODUCT_ID into g
                           select new
                           {
                               ProductNo = g.Key,
                               UNIT_SOLD = g.Sum(x => x.UNIT_SOLD),
                               AMNT = g.Sum(x => x.SOLD_PRICE),
                               PUR_DATE = DateTime.Now,
                               SOLD_BY_ID = g.FirstOrDefault().SOLD_BY_ID,
                               CREATED_AT = g.FirstOrDefault().CREATED_AT,
                               MONEY_RECEIVED_BY_ID = g.FirstOrDefault().MONEY_RECEIVED_BY_ID,
                               UPDATED_AT = g.FirstOrDefault().UPDATED_AT
                           };

            var ProductSOrg = (from pd in db.STORE_PRODUCTS
                            join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                            join pct in paymentsOrg on pd.PRODUCT_ID equals pct.ProductNo
                            orderby pd.NAME, ct.NAME
                            select new Models.PurchaseCart { ProductData = pd, CategoryData = ct, UNIT_SOLD = pct.UNIT_SOLD, SOLD_AMNT = pct.AMNT, PUR_DATE = pct.PUR_DATE, SOLD_BY_ID=pct.SOLD_BY_ID, CREATED_AT = pct.CREATED_AT, UPDATED_AT = pct.UPDATED_AT, MONEY_RECEIVED_BY_ID = pct.MONEY_RECEIVED_BY_ID}).Distinct();

            foreach (var PurCarList in ProductSOrg.ToList())
            {
                var StorePur = new STORE_PURCHAGE()
                {
                    PRODUCT_ID = PurCarList.ProductData.PRODUCT_ID,
                    UNIT_SOLD = PurCarList.UNIT_SOLD,
                    SOLD_PRICE = (int)PurCarList.SOLD_AMNT,
                    SOLD_BY_ID = PurCarList.SOLD_BY_ID,
                    SOLD_ON = PDate,
                    STUDENT_ID = STUDENT_ID,
                    STUDENT_CONTACT_NO = PHONE_NUMBER,
                    PAYMENT_MODE = PAYMENT_MODE,
                    MONEY_RECEIVED_BY_ID = PurCarList.MONEY_RECEIVED_BY_ID,
                    IS_DEPOSITED = false,
                    IS_ACT = true,
                    IS_DEL = false,
                    CREATED_AT = PurCarList.CREATED_AT,
                    UPDATED_AT = PurCarList.UPDATED_AT,
                };
                db.STORE_PURCHAGE.Add(StorePur);

                if(STUDENT_ID != null && STUDENT_ID != -1)
                {
                    STUDENT std = db.STUDENTs.Find(STUDENT_ID);
                    STORE_PRODUCTS prd = db.STORE_PRODUCTS.Find(PurCarList.ProductData.PRODUCT_ID);
                    STORE_CATEGORY st_cat = db.STORE_CATEGORY.Find(prd.CATEGORY_ID);
                    STORE_SUB_CATEGORY st_subcat = db.STORE_SUB_CATEGORY.Find(prd.SUB_CATEGORY_ID);
                    if(st_cat.NAME.Contains("Stationary") && st_subcat.NAME.Contains("Book"))
                    {
                        std.BOOK_PURCHAGED = true;
                        std.BOOK_PUR_DT = System.DateTime.Now;
                        db.Entry(std).State = EntityState.Modified;
                    }
                    if (st_cat.NAME.Contains("Uniform") && (st_subcat.NAME.Contains("Pant") || st_subcat.NAME.Contains("Shirt")))
                    {
                        std.DRESS_PURCHAGED = true;
                        std.DRESS_PUR_DT = System.DateTime.Now;
                        db.Entry(std).State = EntityState.Modified;
                    }
                }
                try { db.SaveChanges(); ViewBag.PaymentMessage = "Payment Done Successfully."; }
                catch (Exception e) { ViewBag.PaymentMessage = e.InnerException.InnerException.Message; return View("_ListPurchagedProducts"); }
                IDList[i] = StorePur.ID;
                i++;

            }

            var payments = from p in db.STORE_PURCHAGE.Where(a => IDList.Any(s => a.ID.Equals(s)))
                           select new
                           {
                               ProductNo = p.PRODUCT_ID,
                               UNIT_SOLD = p.UNIT_SOLD,
                               AMNT = p.SOLD_PRICE,
                               PUR_DATE = DateTime.Now
                           };

            var ProductS = (from pd in db.STORE_PRODUCTS
                            join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                            join pct in payments on pd.PRODUCT_ID equals pct.ProductNo
                            orderby pd.NAME, ct.NAME
                            select new Models.PurchaseCart { ProductData = pd, CategoryData = ct, UNIT_SOLD = pct.UNIT_SOLD, SOLD_AMNT = pct.AMNT, PUR_DATE = pct.PUR_DATE }).Distinct();

            try { db.Database.ExecuteSqlCommand("DELETE FROM STORE_PURCHAGE_CART"); ViewBag.PaymentMessage = string.Concat(ViewBag.PaymentMessage,"Cart cleared now."); }
            catch (Exception e) { Console.WriteLine(e); ViewBag.StoreDeleteMessage = string.Concat(ViewBag.PaymentMessage, e.InnerException.InnerException.Message); }
          
            return View("_ListPurchagedProducts", ProductS.ToList());

        }

        // GET: Store/Edit/5
        public ActionResult EditSellingCart(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_PURCHAGE_CART sTORE_PURCHASE_CART = db.STORE_PURCHAGE_CART.Find(id);
            if (sTORE_PURCHASE_CART == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> options4 = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", sTORE_PURCHASE_CART.PRODUCT_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product" });
            ViewBag.PRODUCT_ID = options4;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            return View(sTORE_PURCHASE_CART);
        }

        // POST: Store/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSellingCart([Bind(Include = "ID,PRODUCT_ID,UNIT_SOLD,CREATED_AT")] STORE_PURCHAGE_CART sTORE_PURCHASE_CART)
        {
            List<SelectListItem> options4 = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", sTORE_PURCHASE_CART.PRODUCT_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product" });
            ViewBag.PRODUCT_ID = options4;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            if (ModelState.IsValid)
            {
                if (sTORE_PURCHASE_CART.UNIT_SOLD >= 1)
                {
                    STORE_PURCHAGE_CART sTORE_PURCHASE_CART_upd = db.STORE_PURCHAGE_CART.Find(sTORE_PURCHASE_CART.ID);

                    STORE_PRODUCTS sTOREpRODUCT = db.STORE_PRODUCTS.Find(sTORE_PURCHASE_CART.PRODUCT_ID);
                    sTOREpRODUCT.UPDATED_AT = DateTime.Now;
                    sTOREpRODUCT.UNIT_LEFT = sTOREpRODUCT.UNIT_LEFT + sTORE_PURCHASE_CART_upd.UNIT_SOLD - sTORE_PURCHASE_CART.UNIT_SOLD;
                    db.Entry(sTOREpRODUCT).State = EntityState.Modified;

                    sTORE_PURCHASE_CART_upd.PRODUCT_ID = sTORE_PURCHASE_CART.PRODUCT_ID;
                    sTORE_PURCHASE_CART_upd.UNIT_SOLD = sTORE_PURCHASE_CART.UNIT_SOLD;
                    sTORE_PURCHASE_CART_upd.SOLD_PRICE = sTOREpRODUCT.SELL_PRICE_PER_UNIT * sTORE_PURCHASE_CART.UNIT_SOLD;
                    sTORE_PURCHASE_CART_upd.UPDATED_AT = System.DateTime.Now;
                    db.Entry(sTORE_PURCHASE_CART_upd).State = EntityState.Modified;

                    try { db.SaveChanges(); ViewBag.Notice = "Selling Details updated successfully."; }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                        return View(sTORE_PURCHASE_CART);
                    }
                    catch (Exception e)
                    {
                        ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                        return View(sTORE_PURCHASE_CART);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Quantity Entered is not appropriate.";
                    return View(sTORE_PURCHASE_CART);
                }

                ViewBag.CartItems = (from t in db.STORE_PURCHAGE_CART
                                     select t).Count();
                return RedirectToAction("ViewCart", new { ErrorMessage = ViewBag.ErrorMessage });
            }

            ViewBag.ErrorMessage = "Model State does not seems to be valid.";
            return View(sTORE_PURCHASE_CART);
        }

        // GET: Store/Edit/5
        public ActionResult DeleteSellingCart(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_PURCHAGE_CART sTORE_PURCHASE_CART_Item = db.STORE_PURCHAGE_CART.Find(id);

            STORE_PRODUCTS sTOREpRODUCT = db.STORE_PRODUCTS.Find(sTORE_PURCHASE_CART_Item.PRODUCT_ID);
            sTOREpRODUCT.UPDATED_AT = DateTime.Now;
            sTOREpRODUCT.UNIT_LEFT = sTOREpRODUCT.UNIT_LEFT + sTORE_PURCHASE_CART_Item.UNIT_SOLD;
            db.Entry(sTOREpRODUCT).State = EntityState.Modified;
            try { db.SaveChanges(); ViewBag.Notice = "Product Details Updated."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, e.InnerException.InnerException.Message); }

            try { db.Database.ExecuteSqlCommand(string.Concat("DELETE FROM STORE_PURCHAGE_CART WHERE ID =",id)); ViewBag.Notice = string.Concat(ViewBag.Notice, " Transaction Deleted Successfully."); }
            catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, e.InnerException.InnerException.Message); }
            return RedirectToAction("ViewCart", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Student
        public ActionResult CancelTransaction()
        {

            var pURcART = (from res in db.STORE_PURCHAGE_CART
                             select res).ToList();
            foreach(var PurCarList in pURcART) 
            {
 
                STORE_PRODUCTS sTOREpRODUCT = db.STORE_PRODUCTS.Find(PurCarList.PRODUCT_ID);
                sTOREpRODUCT.UPDATED_AT = DateTime.Now;
                sTOREpRODUCT.UNIT_LEFT = sTOREpRODUCT.UNIT_LEFT + PurCarList.UNIT_SOLD;
                db.Entry(sTOREpRODUCT).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.Notice = "Product Details Updated."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, e.InnerException.InnerException.Message); }

            }
            try { db.Database.ExecuteSqlCommand("TRUNCATE TABLE STORE_PURCHAGE_CART"); ViewBag.Notice = string.Concat(ViewBag.Notice," Transaction Cancelled Successfully."); }
            catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, e.InnerException.InnerException.Message); }
            return RedirectToAction("ViewAll", new { ErrorMessage = ViewBag.ErrorMessage , Notice = ViewBag.Notice });
        }

        // GET: Student
        public ActionResult CleanCart()
        {
            var pURcART = (from res in db.STORE_PURCHAGE_CART
                           select res).ToList();
            foreach (var PurCarList in pURcART)
            {

                STORE_PRODUCTS sTOREpRODUCT = db.STORE_PRODUCTS.Find(PurCarList.PRODUCT_ID);
                sTOREpRODUCT.UPDATED_AT = DateTime.Now;
                sTOREpRODUCT.UNIT_LEFT = sTOREpRODUCT.UNIT_LEFT + PurCarList.UNIT_SOLD;
                db.Entry(sTOREpRODUCT).State = EntityState.Modified;
                db.SaveChanges();
            }

            db.Database.ExecuteSqlCommand("TRUNCATE TABLE STORE_PURCHAGE_CART");
            return RedirectToAction("ViewAll");
        }


        public ActionResult BackupTransaction(string ErrorMessage, string Notice, string sortOrder, int? currentFilter, int? PRODUCT_ID, int? page, int? currentFilter2, int? STUDENT_ID, string currentFilter3, string ContactNumber, int? currentFilter4, int? USER_ID, int? currentFilter5, int? MoneyDeposited, string currentFilter9, string SoldFromDate, string currentFilter10, string SoldToDate)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;

            ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";

            if (PRODUCT_ID != null && PRODUCT_ID != -1) { page = 1; }
            else { PRODUCT_ID = currentFilter; }

            ViewBag.CurrentFilter = PRODUCT_ID;
            if (STUDENT_ID != null && STUDENT_ID != -1) { page = 1; }
            else { STUDENT_ID = currentFilter2; }
            ViewBag.CurrentFilter2 = STUDENT_ID;
            if (!String.IsNullOrEmpty(ContactNumber)) { page = 1; }
            else { ContactNumber = currentFilter3; }
            ViewBag.CurrentFilter3 = ContactNumber;
            if (USER_ID != null && USER_ID != -1) { page = 1; }
            else { USER_ID = currentFilter4; }
            ViewBag.CurrentFilter4 = USER_ID;
            if (MoneyDeposited != null && MoneyDeposited != -1) { page = 1; }
            else { MoneyDeposited = currentFilter5; }
            ViewBag.CurrentFilter5 = MoneyDeposited;
            if (!String.IsNullOrEmpty(SoldFromDate))
            {
                page = 1;
            }
            else { SoldFromDate = currentFilter9; }
            DateTime? dFrom; DateTime dtFrom;
            dFrom = DateTime.TryParse(SoldFromDate, out dtFrom) ? dtFrom : (DateTime?)null;
            ViewBag.CurrentFilter9 = SoldFromDate;
            if (!String.IsNullOrEmpty(SoldToDate))
            {
                page = 1;
            }
            else { SoldToDate = currentFilter10; }
            DateTime? dTo; DateTime dtTo;
            dTo = DateTime.TryParse(SoldToDate, out dtTo) ? dtTo : (DateTime?)null;
            ViewBag.CurrentFilter10 = SoldToDate;

            var PurchaseS = (from pd in db.STORE_PRODUCTS
                             join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                             join pur in db.STORE_PURCHAGE on pd.PRODUCT_ID equals pur.PRODUCT_ID
                             join std in db.STUDENTs on pur.STUDENT_ID equals std.ID into gstd
                             from subgstd in gstd.DefaultIfEmpty()
                             join usr in db.USERS on pur.SOLD_BY_ID equals usr.ID into gusr
                             from subgusr in gusr.DefaultIfEmpty()
                             orderby pur.SOLD_ON, pd.NAME, ct.NAME
                             where pur.IS_DEL == false
                             select new Models.Purchase { PurchaseData = pur, ProductData = pd, CategoryData = ct, StudentData = (subgstd == null ? null : subgstd), UserData = (subgusr == null ? null : subgusr) }).Distinct();

            if (PRODUCT_ID != null && PRODUCT_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.ProductData.PRODUCT_ID == PRODUCT_ID);
            }
            if (STUDENT_ID != null && STUDENT_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.STUDENT_ID == STUDENT_ID);
            }
            if (!String.IsNullOrEmpty(ContactNumber))
            {
                long? StdContactNum = Convert.ToInt64(ContactNumber);
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.STUDENT_CONTACT_NO == StdContactNum);
            }
            if (USER_ID  != null && USER_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.SOLD_BY_ID == USER_ID);
            }
            if (MoneyDeposited != null && MoneyDeposited != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.IS_DEPOSITED.Equals(MoneyDeposited == 1 ? true : false));
            }
            if (!String.IsNullOrEmpty(SoldFromDate) && !String.IsNullOrEmpty(SoldToDate))
            {
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.SOLD_ON >= dFrom).Where(s => s.PurchaseData.SOLD_ON <= dTo);
            }
            switch (sortOrder)
            {
                case "date_desc":
                    PurchaseS = PurchaseS.OrderBy(s => s.PurchaseData.SOLD_ON);
                    break;
                case "Name":
                    PurchaseS = PurchaseS.OrderBy(s => s.ProductData.NAME);
                    break;
                case "name_desc":
                    PurchaseS = PurchaseS.OrderByDescending(s => s.ProductData.NAME);
                    break;
                default:  // Name ascending 
                    PurchaseS = PurchaseS.OrderByDescending(s => s.PurchaseData.SOLD_ON);
                    break;
            }
            var queryStudent = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options2 = new List<SelectListItem>();
            foreach (var item in queryStudent)
            {
                string StudentFullName = string.Concat(item.FIRST_NAME, " ", item.MID_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = StudentFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == currentFilter2 ? true : false;
                options2.Add(result);
            }
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Student" });
            ViewBag.STUDENT_ID = options2;

            var student_select = db.STUDENTs.Where(x => x.IS_DEL == false && x.IS_ACT == true).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.MID_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["student_select"] = student_select;

            List<SelectListItem> options4 = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", currentFilter).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product" });
            ViewBag.PRODUCT_ID = options4;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            List<SelectListItem> options3 = new List<SelectListItem>();
            options3.Insert(0, new SelectListItem() { Value = null, Text = "Select Value", Selected = ViewBag.CurrentFilter5 == -1 || ViewBag.CurrentFilter5 == null ? true : false });
            options3.Insert(1, new SelectListItem() { Value = "1", Text = "Yes", Selected = ViewBag.CurrentFilter5 == 1 ? true : false });
            options3.Insert(2, new SelectListItem() { Value = "0", Text = "No", Selected = ViewBag.CurrentFilter5 == 0 ? true : false });
            ViewBag.MoneyDeposited = options3;

            var queryUser = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            List<SelectListItem> options6 = new List<SelectListItem>();
            foreach (var item in queryUser)
            {
                string UserFullName = string.Concat(item.FIRST_NAME, " ", item.LAST_NAME);
                var result = new SelectListItem();
                result.Text = UserFullName;
                result.Value = item.ID.ToString();
                result.Selected = item.ID == USER_ID ? true : false;
                options6.Add(result);
            }
            options6.Insert(0, new SelectListItem() { Value = null, Text = "Select User" });
            ViewBag.USER_ID = options6;

            var user_select = db.USERS.Where(x => x.IS_DEL == false).OrderBy(x => x.FIRST_NAME).ThenBy(x => x.LAST_NAME).ToList();
            ViewData["user_select"] = user_select;

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(PurchaseS.ToPagedList(pageNumber, pageSize));
            //return View(db.USERS.ToList());
        }

        [OutputCache(Duration = 0, VaryByParam = "*")]
        [HttpGet]
        public ActionResult Product_Select(int? id)
        {
            List<SelectListItem> options4 = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", id).ToList();
            options4.Insert(0, new SelectListItem() { Value = null, Text = "Select Product" });
            ViewBag.PRODUCT_ID = options4;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            return PartialView("_product_select", product_select);
        }

        // GET: Student
        [HttpGet]
        public ActionResult BackupSelectedTransactions(int? PRODUCT_ID, int? STUDENT_ID, string ContactNumber, int? USER_ID, int? MoneyDeposited, string SoldFromDate, string SoldToDate)
        {
            DateTime? dFrom; DateTime dtFrom;
            dFrom = DateTime.TryParse(SoldFromDate, out dtFrom) ? dtFrom : (DateTime?)null;
            DateTime? dTo; DateTime dtTo;
            dTo = DateTime.TryParse(SoldToDate, out dtTo) ? dtTo : (DateTime?)null;
            var PurchaseS = (from pd in db.STORE_PRODUCTS
                             join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                             join pur in db.STORE_PURCHAGE on pd.PRODUCT_ID equals pur.PRODUCT_ID
                             orderby pur.SOLD_ON, pd.NAME, ct.NAME
                             where pur.IS_DEL == false
                             select new Models.Purchase { PurchaseData = pur, ProductData = pd, CategoryData = ct }).Distinct();

            if (PRODUCT_ID != null && PRODUCT_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.ProductData.PRODUCT_ID == PRODUCT_ID);
            }
            if (STUDENT_ID != null && STUDENT_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.STUDENT_ID == STUDENT_ID);
            }
            if (!String.IsNullOrEmpty(ContactNumber))
            {
                long? StdContactNum = Convert.ToInt64(ContactNumber);
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.STUDENT_CONTACT_NO == StdContactNum);
            }
            if (USER_ID != null && USER_ID != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.SOLD_BY_ID == USER_ID);
            }
            if (MoneyDeposited != null && MoneyDeposited != -1)
            {
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.IS_DEPOSITED.Equals(MoneyDeposited == 1 ? true : false));
            }
            if (!String.IsNullOrEmpty(SoldFromDate) && !String.IsNullOrEmpty(SoldToDate))
            {
                PurchaseS = PurchaseS.Where(s => s.PurchaseData.SOLD_ON >= dFrom).Where(s => s.PurchaseData.SOLD_ON <= dTo);
            }
            foreach (var item in PurchaseS.ToList())
            {
                var ST_PUR_BACKUp = new STORE_PURCHAGE_BACKUP() { ID = item.PurchaseData.ID, PRODUCT_ID = item.PurchaseData.PRODUCT_ID, UNIT_SOLD= item.PurchaseData.UNIT_SOLD, SOLD_PRICE=item.PurchaseData.SOLD_PRICE, SOLD_BY_ID=item.PurchaseData.SOLD_BY_ID, SOLD_ON=item.PurchaseData.SOLD_ON, STUDENT_ID=item.PurchaseData.STUDENT_ID, STUDENT_CONTACT_NO=item.PurchaseData.STUDENT_CONTACT_NO, MONEY_RECEIVED_BY_ID=item.PurchaseData.MONEY_RECEIVED_BY_ID, IS_DEPOSITED=item.PurchaseData.IS_DEPOSITED, IS_ACT=item.PurchaseData.IS_ACT, IS_DEL=item.PurchaseData.IS_DEL, CREATED_AT=item.PurchaseData.CREATED_AT, UPDATED_AT= item.PurchaseData.UPDATED_AT, PAYMENT_MODE = item.PurchaseData.PAYMENT_MODE };
                db.STORE_PURCHAGE_BACKUP.Add(ST_PUR_BACKUp);
                STORE_PURCHAGE sPURCHAGE = db.STORE_PURCHAGE.Find(item.PurchaseData.ID);
                db.STORE_PURCHAGE.Remove(sPURCHAGE);
            }
            try { db.SaveChanges(); ViewBag.Notice = "Selected Data is backed up successfully."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, e.InnerException.InnerException.Message); }

            return RedirectToAction("BackupTransaction", "Store", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });

        }

        // GET: Student/Details/5
        public ActionResult PurchageOrder(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            List<SelectListItem> options = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME").ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product" });
            ViewBag.PRODUCT_ID = options;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            List<SelectListItem> options3 = new SelectList(db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            options3.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Brand" });
            ViewBag.BRAND_ID = options3;

            var product_brands = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_brands"] = product_brands;

            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME").ToList();
            options4.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            return View();
        }

        // GET: Student/Details/5
        [ChildActionOnly]
        public ActionResult PurchageOrderList()
        {
            // A collection of strings to check against
            //var Status = new int[] { 1, "String B", "String C" };
            // Only grab the Actions where your Agent property contains any of the strings in your previous collection
            //actions = db.Actions.Where(a => search.Any(s => a.Agent.Contains(s)));
            var pURCHAGEoRDERS = (from po in db.STORE_PURCHAGE_ORDER
                                  join sps in db.STORE_PURCHAGE_STATUS on po.STATUS_ID equals sps.ID
                                  join spv in db.STORE_PURCHAGE_VENDOR on po.VENDOR_ID equals spv.ID into gspv
                                  from subgspv in gspv.DefaultIfEmpty()
                                  join pd in db.STORE_PRODUCTS on po.PRODUCT_ID equals pd.PRODUCT_ID
                            join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                            join subcat in db.STORE_SUB_CATEGORY on pd.SUB_CATEGORY_ID equals subcat.ID into gsc
                            from subgsc in gsc.DefaultIfEmpty()
                            join usr in db.USERS on po.EMPLOYEE_ID equals usr.ID
                            orderby pd.NAME, ct.NAME
                            where sps.NAME == "Pending" || sps.NAME == "Approved"
                                  select new Models.PurchageOrder { PurchaseOrderData = po,PurchageStatusData = sps, PurchageVendorData= (subgspv == null ? null : subgspv), ProductData = pd, CategoryData = ct, SubCategoryData = (subgsc == null ? null : subgsc), EmployeeData = usr }).ToList();

            //var pURCHAGEoRDERS = db.STORE_PURCHAGE_ORDER.Where(x => x.STATUS == 1 || x.STATUS == 2).ToList();
            return View(pURCHAGEoRDERS);
        }


        public ActionResult PurchageOrderDelete(int id)
        {
            STORE_PURCHAGE_ORDER sTOREpURCHAGEoRDER = db.STORE_PURCHAGE_ORDER.Find(id);
            db.STORE_PURCHAGE_ORDER.Remove(sTOREpURCHAGEoRDER);
            try { db.SaveChanges(); ViewBag.Notice = "Purchage Order deleted successfully."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
            return RedirectToAction("PurchageOrder", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Student/Edit/5
        public ActionResult PurchageOrderEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_PURCHAGE_ORDER sTOREpURCHAGEoRDER = db.STORE_PURCHAGE_ORDER.Find(id);
            if (sTOREpURCHAGEoRDER == null)
            {
                return HttpNotFound();
            }
            sTOREpURCHAGEoRDER.REVISION_NUMBER += 1;
            DateTime OrdDate = Convert.ToDateTime(System.DateTime.Now);
            if (sTOREpURCHAGEoRDER.ORDER_DATE != null)
            {
                OrdDate = Convert.ToDateTime(sTOREpURCHAGEoRDER.ORDER_DATE);
            }
            ViewBag.ORDER_DATE = OrdDate.ToShortDateString();
            DateTime ShipDate = Convert.ToDateTime(System.DateTime.Now);
            if (sTOREpURCHAGEoRDER.SHIP_DATE != null)
            {
                ShipDate = Convert.ToDateTime(sTOREpURCHAGEoRDER.SHIP_DATE);
            }
            ViewBag.SHIP_DATE = ShipDate.ToShortDateString();

            List<SelectListItem> options = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", sTOREpURCHAGEoRDER.PRODUCT_ID).ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product" });
            ViewBag.PRODUCT_ID = options;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            List<SelectListItem> options3 = new SelectList(db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTOREpURCHAGEoRDER.BRAND_ID).ToList();
            options3.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Brand" });
            ViewBag.BRAND_ID = options3;

            var product_brands = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_brands"] = product_brands;

            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTOREpURCHAGEoRDER.VENDOR_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            List<SelectListItem> options2 = new SelectList(db.STORE_PURCHAGE_STATUS.Where(x => x.IS_DEL == false).OrderBy(x => x.ID), "ID", "NAME", sTOREpURCHAGEoRDER.STATUS_ID).ToList();
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select PO Status" });
            ViewBag.STATUS_ID = options2;

            List<SelectListItem> options5 = new SelectList(db.EMPLOYEEs.OrderBy(x => x.ID), "ID", "EMP_NUM", sTOREpURCHAGEoRDER.EMPLOYEE_ID).ToList();
            // add the 'ALL' option
            options5.Insert(0, new SelectListItem() { Value = null, Text = "Select Employee" });
            ViewBag.EMPLOYEE_ID = options5;


            return View(sTOREpURCHAGEoRDER);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchageOrderEdit([Bind(Include = "ID,PO_NUMBER,PRODUCT_ID,REVISION_NUMBER,STATUS_ID, EMPLOYEE_ID, VENDOR_ID, SHIP_METHOD_ID,ORDER_DATE, ORDER_QUANTITY, SHIP_DATE, SUB_TOTAL, TAX_AMT, FREIGHT, TOTAL_DUE,BRAND_ID")] STORE_PURCHAGE_ORDER sTOREpURCHAGEoRDER)
        {
            DateTime OrdDate = Convert.ToDateTime(System.DateTime.Now);
            if (sTOREpURCHAGEoRDER.ORDER_DATE != null)
            {
                OrdDate = Convert.ToDateTime(sTOREpURCHAGEoRDER.ORDER_DATE);
            }
            ViewBag.ORDER_DATE = OrdDate.ToShortDateString();
            DateTime ShipDate = Convert.ToDateTime(System.DateTime.Now);
            if (sTOREpURCHAGEoRDER.SHIP_DATE != null)
            {
                ShipDate = Convert.ToDateTime(sTOREpURCHAGEoRDER.SHIP_DATE);
            }
            ViewBag.SHIP_DATE = ShipDate.ToShortDateString();

            List<SelectListItem> options = new SelectList(db.STORE_PRODUCTS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "PRODUCT_ID", "NAME", sTOREpURCHAGEoRDER.PRODUCT_ID).ToList();
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product" });
            ViewBag.PRODUCT_ID = options;

            var product_select = db.STORE_PRODUCTS.Include(x => x.STORE_CATEGORY).Include(x => x.STORE_SUB_CATEGORY).Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_select"] = product_select;

            List<SelectListItem> options3 = new SelectList(db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTOREpURCHAGEoRDER.BRAND_ID).ToList();
            options3.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Brand" });
            ViewBag.BRAND_ID = options3;

            var product_brands = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_brands"] = product_brands;

            List<SelectListItem> options4 = new SelectList(db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME), "ID", "NAME", sTOREpURCHAGEoRDER.VENDOR_ID).ToList();
            options4.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Product Vendor" });
            ViewBag.VENDOR_ID = options4;

            var product_vendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            ViewData["product_vendor"] = product_vendor;

            List<SelectListItem> options2 = new SelectList(db.STORE_PURCHAGE_STATUS.Where(x => x.IS_DEL == false).OrderBy(x => x.ID), "ID", "NAME", sTOREpURCHAGEoRDER.STATUS_ID).ToList();
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select PO Status" });
            ViewBag.STATUS_ID = options2;

            List<SelectListItem> options5 = new SelectList(db.EMPLOYEEs.OrderBy(x => x.ID), "ID", "EMP_NUM", sTOREpURCHAGEoRDER.EMPLOYEE_ID).ToList();
            // add the 'ALL' option
            options5.Insert(0, new SelectListItem() { Value = null, Text = "Select Employee" });
            ViewBag.EMPLOYEE_ID = options5;

            if (ModelState.IsValid)
            {
                STORE_PURCHAGE_ORDER sTOREpURCHAGEoRDER_UPD = db.STORE_PURCHAGE_ORDER.Find(sTOREpURCHAGEoRDER.ID);
                sTOREpURCHAGEoRDER_UPD.PRODUCT_ID = sTOREpURCHAGEoRDER.PRODUCT_ID == -1 ? null : sTOREpURCHAGEoRDER.PRODUCT_ID;
                sTOREpURCHAGEoRDER_UPD.BRAND_ID = sTOREpURCHAGEoRDER.BRAND_ID == -1 ? null : sTOREpURCHAGEoRDER.BRAND_ID;
                sTOREpURCHAGEoRDER_UPD.REVISION_NUMBER = sTOREpURCHAGEoRDER.REVISION_NUMBER;
                sTOREpURCHAGEoRDER_UPD.STATUS_ID = sTOREpURCHAGEoRDER.STATUS_ID;
                sTOREpURCHAGEoRDER_UPD.EMPLOYEE_ID = sTOREpURCHAGEoRDER.EMPLOYEE_ID;
                sTOREpURCHAGEoRDER_UPD.VENDOR_ID = sTOREpURCHAGEoRDER.VENDOR_ID == -1 ? null : sTOREpURCHAGEoRDER.VENDOR_ID;
                sTOREpURCHAGEoRDER_UPD.SHIP_METHOD_ID = sTOREpURCHAGEoRDER.SHIP_METHOD_ID;
                sTOREpURCHAGEoRDER_UPD.ORDER_DATE = sTOREpURCHAGEoRDER.ORDER_DATE;
                sTOREpURCHAGEoRDER_UPD.ORDER_QUANTITY = sTOREpURCHAGEoRDER.ORDER_QUANTITY;
                sTOREpURCHAGEoRDER_UPD.SHIP_DATE = sTOREpURCHAGEoRDER.SHIP_DATE;
                sTOREpURCHAGEoRDER_UPD.TOTAL_DUE = sTOREpURCHAGEoRDER.TOTAL_DUE;
                sTOREpURCHAGEoRDER_UPD.FREIGHT = sTOREpURCHAGEoRDER.FREIGHT;
                sTOREpURCHAGEoRDER_UPD.TAX_AMT = sTOREpURCHAGEoRDER.TAX_AMT;
                sTOREpURCHAGEoRDER_UPD.SUB_TOTAL = sTOREpURCHAGEoRDER.SUB_TOTAL;
                sTOREpURCHAGEoRDER_UPD.UPDATED_AT = System.DateTime.Now;
                if(sTOREpURCHAGEoRDER_UPD.STATUS_ID == 4 && (sTOREpURCHAGEoRDER_UPD.SHIP_DATE==null || sTOREpURCHAGEoRDER_UPD.TOTAL_DUE==null || sTOREpURCHAGEoRDER_UPD.FREIGHT==null || sTOREpURCHAGEoRDER_UPD.TAX_AMT==null || sTOREpURCHAGEoRDER_UPD.SUB_TOTAL==null))
                {
                    ViewBag.ErrorMessage = "Ship Date, Total Due, Frieght Cost, Tax Amount and Sub Total Cost is Mandatory to make PO Complete.";
                }
                else
                {
                    db.Entry(sTOREpURCHAGEoRDER_UPD).State = EntityState.Modified;
                    try { db.SaveChanges(); ViewBag.Notice = "Purchage Order edited successfully."; }
                    catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                    if (sTOREpURCHAGEoRDER_UPD.STATUS_ID == 4)
                    {
                        STORE_PRODUCTS NewProd = db.STORE_PRODUCTS.Find(sTOREpURCHAGEoRDER_UPD.PRODUCT_ID);

                        var sTORE_PRODUCTS_UPD = new STORE_PRODUCTS() { NAME = NewProd.NAME, CATEGORY_ID = NewProd.CATEGORY_ID,
                            SUB_CATEGORY_ID = NewProd.SUB_CATEGORY_ID,
                            BRAND_ID = sTOREpURCHAGEoRDER_UPD.BRAND_ID,
                        TOTAL_UNIT = sTOREpURCHAGEoRDER_UPD.ORDER_QUANTITY,
                        TOTAL_COST = Convert.ToInt32(sTOREpURCHAGEoRDER_UPD.SUB_TOTAL),
                        COST_PER_UNIT = Convert.ToInt32(Convert.ToInt32(sTOREpURCHAGEoRDER_UPD.SUB_TOTAL)/ sTOREpURCHAGEoRDER_UPD.ORDER_QUANTITY),
                        SELL_PRICE_PER_UNIT = null,
                        PURCHASED_ON = sTOREpURCHAGEoRDER_UPD.UPDATED_AT,
                        VENDOR_ID = sTOREpURCHAGEoRDER_UPD.VENDOR_ID,
                        PAID_BY = "School",
                        UNIT_LEFT = NewProd.TOTAL_UNIT + sTOREpURCHAGEoRDER_UPD.ORDER_QUANTITY,
                        IS_ACT = true,
                        IS_DEL = false,
                        CREATED_AT = DateTime.Now,
                        UPDATED_AT = DateTime.Now
                        };
                        db.STORE_PRODUCTS.Add(sTORE_PRODUCTS_UPD);
                        try { db.SaveChanges();}
                        catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }

                        STORE_PRODUCTS sTORE_PRODUCTS_ORG = db.STORE_PRODUCTS.Find(sTOREpURCHAGEoRDER_UPD.PRODUCT_ID);
                        sTORE_PRODUCTS_ORG.IS_ACT = false;
                        sTORE_PRODUCTS_ORG.UPDATED_AT = DateTime.Now;
                        db.Entry(sTORE_PRODUCTS_ORG).State = EntityState.Modified;
                        try { db.SaveChanges(); ViewBag.Notice = string.Concat(ViewBag.Notice, "Purchaged Product is added in Product List. Please update further details."); }
                        catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, e.InnerException.InnerException.Message); }

                    }
                }                                
                return RedirectToAction("PurchageOrder", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage});
            }
            return View(sTOREpURCHAGEoRDER);
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchageOrderCreate([Bind(Include = "ID,PO_NUMBER,PRODUCT_ID, VENDOR_ID,ORDER_QUANTITY,BRAND_ID")] STORE_PURCHAGE_ORDER sTOREpURCHAGEoRDER)
        {
            if (ModelState.IsValid)
            {
                if(sTOREpURCHAGEoRDER.PRODUCT_ID == null || sTOREpURCHAGEoRDER.PRODUCT_ID == -1)
                {
                    ViewBag.ErrorMessage = "Product ID Cannot be NULL.";
                    return RedirectToAction("PurchageOrder", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
                }
                else
                {
                    var Product = db.STORE_PRODUCTS.Find(sTOREpURCHAGEoRDER.PRODUCT_ID);
                    if (sTOREpURCHAGEoRDER.PRODUCT_ID == -1)
                    {
                        sTOREpURCHAGEoRDER.PRODUCT_ID = null;
                    }
                    if (sTOREpURCHAGEoRDER.BRAND_ID == -1)
                    {
                        sTOREpURCHAGEoRDER.BRAND_ID = null;
                    }
                    if (sTOREpURCHAGEoRDER.VENDOR_ID == -1)
                    {
                        sTOREpURCHAGEoRDER.VENDOR_ID = null;
                    }
                    sTOREpURCHAGEoRDER.REVISION_NUMBER = 1;
                    sTOREpURCHAGEoRDER.PO_NUMBER = string.Concat("PO-", Product.CATEGORY_ID, "-", Product.SUB_CATEGORY_ID, "-", Product.PRODUCT_ID);
                    sTOREpURCHAGEoRDER.STATUS_ID = 1;
                    sTOREpURCHAGEoRDER.EMPLOYEE_ID = Convert.ToInt32(this.Session["UserId"]);
                    sTOREpURCHAGEoRDER.ORDER_DATE = System.DateTime.Now;
                    sTOREpURCHAGEoRDER.CREATED_AT = System.DateTime.Now;
                    sTOREpURCHAGEoRDER.UPDATED_AT = System.DateTime.Now;
                    db.STORE_PURCHAGE_ORDER.Add(sTOREpURCHAGEoRDER);
                    try { db.SaveChanges(); ViewBag.Notice = "Purchage Order created successfully."; }
                    catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                    return RedirectToAction("PurchageOrder", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
                }               
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State. Please try again.";
            return RedirectToAction("PurchageOrder", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
        }

        // GET: Store Products
        public ActionResult ShortageProducts()
        {

            var ProductS = (from pd in db.STORE_PRODUCTS
                            join ct in db.STORE_CATEGORY on pd.CATEGORY_ID equals ct.ID
                            join subcat in db.STORE_SUB_CATEGORY on pd.SUB_CATEGORY_ID equals subcat.ID into gsc
                            from subgsc in gsc.DefaultIfEmpty()
                            join brd in db.STORE_BRAND on pd.BRAND_ID equals brd.ID into gbrd
                            from subgbrd in gbrd.DefaultIfEmpty()
                            orderby pd.NAME, ct.NAME
                            where pd.IS_DEL == false && pd.IS_ACT == true && pd.UNIT_LEFT <= 2
                            select new Models.Products { ProductData = pd, CategoryData = ct, SubCategoryData = (subgsc == null ? null : subgsc), BrandData = (subgbrd == null ? null : subgbrd) }).Distinct();

            return View(ProductS.ToList());
        }

        // GET: Store Products
        public ActionResult Setting()
        {
            return View();
        }

        // GET: Student/Details/5
        public ActionResult Vendor(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            return View();
        }

        // GET: Student/Details/5
        [ChildActionOnly]
        public ActionResult VendorList()
        {
            var StoreVendor = db.STORE_PURCHAGE_VENDOR.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            return View(StoreVendor);
        }


        public ActionResult VendorDelete(int id)
        {
            STORE_PURCHAGE_VENDOR StoreVendor = db.STORE_PURCHAGE_VENDOR.Find(id);
            db.STORE_PURCHAGE_VENDOR.Remove(StoreVendor);
            try { db.SaveChanges(); ViewBag.Notice = "Vendor details deleted successfully."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
            return RedirectToAction("Vendor", new { ErrorMessage = ViewBag.ErrorMessage , Notice = ViewBag.Notice } );
        }

        // GET: Student/Edit/5
        public ActionResult VendorEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_PURCHAGE_VENDOR StoreVendor = db.STORE_PURCHAGE_VENDOR.Find(id);
            if (StoreVendor == null)
            {
                return HttpNotFound();
            }
            return View(StoreVendor);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VendorEdit([Bind(Include = "ID,NAME,DESCRIPTION")] STORE_PURCHAGE_VENDOR StoreVendor)
        {
            if (ModelState.IsValid)
            {
                STORE_PURCHAGE_VENDOR StoreVendor_UPD = db.STORE_PURCHAGE_VENDOR.Find(StoreVendor.ID);
                StoreVendor_UPD.NAME = StoreVendor.NAME;
                StoreVendor_UPD.DESCRIPTION = StoreVendor.DESCRIPTION;
                db.Entry(StoreVendor_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.Notice = "Store Vendor Details edited successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("Vendor", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State does not seems to be valid. Please try again.";
            return View(StoreVendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VendorCreate([Bind(Include = "ID,NAME,DESCRIPTION")] STORE_PURCHAGE_VENDOR StoreVendor)
        {
            if (ModelState.IsValid)
            {
                StoreVendor.IS_DEL = false;
                StoreVendor.START_DATE = System.DateTime.Now;
                string End_Date_String = "Dec 31 9999";
                DateTime dateValue;
                if (DateTime.TryParse(End_Date_String, out dateValue) == true)
                {
                    StoreVendor.END_DATE = dateValue;
                }
                
                db.STORE_PURCHAGE_VENDOR.Add(StoreVendor);
                try { db.SaveChanges(); ViewBag.Notice = "Store Category created successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("Vendor", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State does not seems to be valid. Please try again.";
            return View(StoreVendor);
        }

        // GET: Student/Details/5
        public ActionResult Brand(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            return View();
        }

        // GET: Student/Details/5
        [ChildActionOnly]
        public ActionResult BrandList()
        {
            var StoreBrand = db.STORE_BRAND.Where(x => x.IS_DEL == false).OrderBy(x=>x.NAME).ToList();
            return View(StoreBrand);
        }


        public ActionResult BrandDelete(int id)
        {
            STORE_BRAND StoreBrand = db.STORE_BRAND.Find(id);
            db.STORE_BRAND.Remove(StoreBrand);
            try { db.SaveChanges(); ViewBag.Notice = "Brand details deleted successfully."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
            return RedirectToAction("Brand", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Student/Edit/5
        public ActionResult BrandEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_BRAND StoreBrand = db.STORE_BRAND.Find(id);
            if (StoreBrand == null)
            {
                return HttpNotFound();
            }
            return View(StoreBrand);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BrandEdit([Bind(Include = "ID,NAME,DESCRIPTION")] STORE_BRAND StoreBrand)
        {
            if (ModelState.IsValid)
            {
                STORE_BRAND StoreBrand_UPD = db.STORE_BRAND.Find(StoreBrand.ID);
                StoreBrand_UPD.NAME = StoreBrand.NAME;
                StoreBrand_UPD.DESCRIPTION = StoreBrand.DESCRIPTION;
                db.Entry(StoreBrand_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.Notice = "Store Brand Details edited successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("Brand", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State does not seems to be valid. Please try again.";
            return View(StoreBrand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BrandCreate([Bind(Include = "ID,NAME,DESCRIPTION")] STORE_BRAND StoreBrand)
        {
            if (ModelState.IsValid)
            {
                StoreBrand.IS_DEL = false;
                StoreBrand.START_DATE = System.DateTime.Now;
                string End_Date_String = "Dec 31 9999";
                DateTime dateValue;
                if (DateTime.TryParse(End_Date_String, out dateValue) == true)
                {
                    StoreBrand.END_DATE = dateValue;
                }

                db.STORE_BRAND.Add(StoreBrand);
                try { db.SaveChanges(); ViewBag.Notice = "Store Brand created successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("Brand", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State does not seems to be valid. Please try again.";
            return View(StoreBrand);
        }

        // GET: Student/Details/5
        public ActionResult PurchageStatus(string ErrorMessage, string Notice)
        {
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.Notice = Notice;
            return View();
        }

        // GET: Student/Details/5
        [ChildActionOnly]
        public ActionResult PurchageStatusList()
        {
            var PurchageStatus = db.STORE_PURCHAGE_STATUS.Where(x => x.IS_DEL == false).OrderBy(x => x.NAME).ToList();
            return View(PurchageStatus);
        }


        public ActionResult PurchageStatusDelete(int id)
        {
            STORE_PURCHAGE_STATUS PurchageStatus = db.STORE_PURCHAGE_STATUS.Find(id);
            db.STORE_PURCHAGE_STATUS.Remove(PurchageStatus);
            try { db.SaveChanges(); ViewBag.Notice = "Brand details deleted successfully."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
            return RedirectToAction("PurchageStatus", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        // GET: Student/Edit/5
        public ActionResult PurchageStatusEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STORE_PURCHAGE_STATUS PurchageStatus = db.STORE_PURCHAGE_STATUS.Find(id);
            if (PurchageStatus == null)
            {
                return HttpNotFound();
            }
            return View(PurchageStatus);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchageStatusEdit([Bind(Include = "ID,NAME,DESCRIPTION")] STORE_PURCHAGE_STATUS PurchageStatus)
        {
            if (ModelState.IsValid)
            {
                STORE_PURCHAGE_STATUS PurchageStatus_UPD = db.STORE_PURCHAGE_STATUS.Find(PurchageStatus.ID);
                PurchageStatus_UPD.NAME = PurchageStatus.NAME;
                PurchageStatus_UPD.DESCRIPTION = PurchageStatus.DESCRIPTION;
                db.Entry(PurchageStatus_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.Notice = "Store Brand Details edited successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("PurchageStatus", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State does not seems to be valid. Please try again.";
            return View(PurchageStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PurchageStatusCreate([Bind(Include = "ID,NAME,DESCRIPTION")] STORE_PURCHAGE_STATUS PurchageStatus)
        {
            if (ModelState.IsValid)
            {
                PurchageStatus.IS_DEL = false;
                PurchageStatus.START_DATE = System.DateTime.Now;
                string End_Date_String = "Dec 31 9999";
                DateTime dateValue;
                if (DateTime.TryParse(End_Date_String, out dateValue) == true)
                {
                    PurchageStatus.END_DATE = dateValue;
                }

                db.STORE_PURCHAGE_STATUS.Add(PurchageStatus);
                try { db.SaveChanges(); ViewBag.Notice = "Store Brand created successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
                return RedirectToAction("PurchageStatus", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "Model State does not seems to be valid. Please try again.";
            return View(PurchageStatus);
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
