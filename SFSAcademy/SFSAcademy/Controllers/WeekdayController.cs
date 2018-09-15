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
    public class WeekdayController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Weekday
        public ActionResult Index(string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == "N"
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
            var wEEKDAYs = db.WEEKDAYs.Where(x=>x.IS_DEL=="N").Include(w => w.BATCH);
            var day = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            ViewData["day"] = day;
            var days = new string[] { "0", "1", "2", "3", "4", "5", "6" };
            ViewData["days"] = days;
            
            return View(wEEKDAYs.ToList());
        }

        // GET: Weekday/Create
        public ActionResult Week(int? id)
        {
            var weekdays = db.WEEKDAYs.Where(x => x.BTCH_ID == id && x.IS_DEL == "N").ToList();
            ViewData["weekdays"] = weekdays;
            var wd = new SFSAcademy.Models.Weekdays();
            int match = 0;
            foreach(var item in weekdays)
            {
                if(item.WKDAY == "Sunday")
                {
                    match = 1;
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Sunday", Id = 0, Select =true });
                    break;
                }
            }
            if(match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Sunday", Id = 0 });  }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Monday")
                {
                    match = 1;
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Monday", Id = 1, Select = true });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Monday", Id = 1 }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Tuesday")
                {
                    match = 1;
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Tuesday", Id = 2, Select = true });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Tuesday", Id = 2 }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Wednesday")
                {
                    match = 1;
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Wednesday", Id = 3, Select = true });
                    break;
                }
            }
            if (match == 0){ wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Wednesday", Id = 3 });}
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Thursday")
                {
                    match = 1;
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Thursday", Id = 4, Select = true });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Thursday", Id = 4 }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Friday")
                {
                    match = 1;
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Friday", Id = 5, Select = true });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Friday", Id = 5 });}
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Saturday")
                {
                    match = 1;
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Saturday", Id = 6, Select = true });
                    break;
                }
            }
            if (match == 0){ wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Saturday", Id = 6 }); }
            //return View(vm);
            ViewBag.BTCH_ID = id;

            return PartialView("_Weekdays", wd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SFSAcademy.Models.Weekdays model, int? batch_id)
        {
            if (ModelState.IsValid)
            {
                var old = db.WEEKDAYs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == "N").ToList();
                var old_Inactive = db.WEEKDAYs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == "Y").ToList();
                foreach (var item in model.WeekdayIds.Where(x=>x.Select == true))
                {
                    int match = 0;
                    foreach (var item21 in old_Inactive)
                    {
                        if (item.Day == item21.WKDAY)
                        {
                            match = 1;
                            WEEKDAY WkToUpdate = db.WEEKDAYs.Find(item21.ID);
                            WkToUpdate.IS_DEL = "N";
                            db.Entry(WkToUpdate).State = EntityState.Modified;
                            break;
                        }
                    }
                    foreach (var item2 in old)
                    {
                        if(item.Day == item2.WKDAY)
                        {
                            match = 2;
                            break;
                        }
                    }
                    if (match == 0)
                    {
                        WEEKDAY WkDay = new WEEKDAY() { NAME = item.Day, WKDAY = item.Day, BTCH_ID = batch_id, DAY_OF_WK = item.Id, SRT_ORD = item.Id, IS_DEL = "N" };
                        db.WEEKDAYs.Add(WkDay);
                    }
                }
                foreach(var item3 in old)
                {
                    int match2 = 0;
                    foreach(var item4 in model.WeekdayIds.Where(x => x.Select == true))
                    {
                        if(item3.WKDAY == item4.Day)
                        {
                            match2 = 1;
                            break;
                        }
                    }
                    if(match2 == 0)
                    {
                        WEEKDAY WkToUpdate = db.WEEKDAYs.Find(item3.ID);
                        WkToUpdate.IS_DEL = "Y";
                        db.Entry(WkToUpdate).State = EntityState.Modified;
                    }
                }
                try { db.SaveChanges();}
                catch (Exception e) { Console.WriteLine(e); ViewBag.Notice = e.InnerException.InnerException.Message; }
                ViewBag.Notice = "Weekdays modified.";
                return RedirectToAction("Index",new { Notice = ViewBag.Notice });
            }

            ViewBag.BTCH_ID = batch_id;
            ViewBag.Notice = "There seems to have some issue in Model State. Admin is looking into it.";
            return RedirectToAction("Index", new { Notice = ViewBag.Notice });
        }

        // GET: Weekday/Create
        public ActionResult New()
        {
            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME");
            return View();
        }

        // POST: Weekday/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New([Bind(Include = "ID,NAME,WKDAY,BTCH_ID,SRT_ORD,DAY_OF_WK,IS_DEL")] WEEKDAY wEEKDAY)
        {
            if (ModelState.IsValid)
            {
                db.WEEKDAYs.Add(wEEKDAY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BTCH_ID = new SelectList(db.BATCHes, "ID", "NAME", wEEKDAY.BTCH_ID);
            return View(wEEKDAY);
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
