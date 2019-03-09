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
                                    where cs.IS_DEL == false
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
            var wEEKDAYs = db.WEEKDAYs.Where(x=>x.IS_DEL==false).Include(w => w.BATCH);
            var day = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            ViewData["day"] = day;
            var days = new string[] { "0", "1", "2", "3", "4", "5", "6" };
            ViewData["days"] = days;
            
            return View(wEEKDAYs.ToList());
        }

        // GET: Weekday/Create
        public ActionResult Week(int? id)
        {
            var weekdays = db.WEEKDAYs.Where(x => x.BTCH_ID == id && x.IS_DEL == false).ToList();
            ViewData["weekdays"] = weekdays;
            //var ClassTimingSet = db.CLASS_TIMING_SET.Include(x=>x.CLASS_TIMING_ENTRY.Where(m=>m.IS_DEL == false)).Where(x => x.IS_DEL == false).ToList();
            //ViewData["ClassTimingSet"] = ClassTimingSet;
            ViewBag.ClassTimingSet = new SelectList(db.CLASS_TIMING_SET.Where(x => x.IS_DEL == false), "ID", "NAME");
            var wd = new SFSAcademy.Models.Weekdays();
            int match = 0;
            foreach(var item in weekdays)
            {
                if(item.WKDAY == "Sunday")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Sunday", Id = 0, Select =true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if(match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Sunday", Id = 0, ClassTimingSet = "Default" });  }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Monday")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Monday", Id = 1, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Monday", Id = 1, ClassTimingSet = "Default" }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Tuesday")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Tuesday", Id = 2, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Tuesday", Id = 2, ClassTimingSet = "Default" }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Wednesday")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Wednesday", Id = 3, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0){ wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Wednesday", Id = 3, ClassTimingSet = "Default" });}
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Thursday")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Thursday", Id = 4, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Thursday", Id = 4, ClassTimingSet = "Default" }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Friday")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Friday", Id = 5, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Friday", Id = 5, ClassTimingSet = "Default" });}
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "Saturday")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Saturday", Id = 6, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0){ wd.WeekdayIds.Add(new SFSAcademy.Models.WeekdaysSelect { Day = "Saturday", Id = 6, ClassTimingSet = "Default" }); }
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
                var old = db.WEEKDAYs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).ToList();
                var old_Inactive = db.WEEKDAYs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == true).ToList();                

                foreach (var item in model.WeekdayIds.Where(x=>x.Select == true))
                {
                    int match = 0;
                    int? TimingSetId = db.CLASS_TIMING_SET.Where(x => x.NAME == item.ClassTimingSet).FirstOrDefault().ID;
                    foreach (var item21 in old_Inactive)
                    {
                        if (item.Day == item21.WKDAY)
                        {
                            match = 1;
                            WEEKDAY WkToUpdate = db.WEEKDAYs.Find(item21.ID);
                            WkToUpdate.IS_DEL = false;
                            WkToUpdate.CLASS_TIMING_SET_ID = TimingSetId;                            
                            db.Entry(WkToUpdate).State = EntityState.Modified;                                        
                            break;
                        }
                    }
                    foreach (var item2 in old)
                    {
                        if(item.Day == item2.WKDAY)
                        {
                            match = 2;
                            WEEKDAY WkToUpdate = db.WEEKDAYs.Find(item2.ID);
                            WkToUpdate.IS_DEL = false;
                            WkToUpdate.CLASS_TIMING_SET_ID = TimingSetId;
                            db.Entry(WkToUpdate).State = EntityState.Modified;
                            break;
                        }
                    }
                    if (match == 0)
                    {

                        WEEKDAY WkDay = new WEEKDAY() { NAME = item.Day, WKDAY = item.Day, BTCH_ID = batch_id, DAY_OF_WK = item.Id, SRT_ORD = item.Id, IS_DEL = false, CLASS_TIMING_SET_ID = TimingSetId };
                        db.WEEKDAYs.Add(WkDay);
                    }
                    int matchClassTiming = 0;
                    var ClassTiming = db.CLASS_TIMING.Where(x => x.BTCH_ID == batch_id).ToList();
                    var ClassTimingEntry = db.CLASS_TIMING_ENTRY.Where(x => x.CLASS_TIMING_SET_ID == TimingSetId).ToList();
                    foreach (var item22 in ClassTimingEntry)
                    {
                        foreach (var item23 in ClassTiming)
                        {
                            if (item22.START_TIME == item23.START_TIME && item22.END_TIME == item23.END_TIME)
                            {
                                matchClassTiming = 1;
                                item22.IS_DEL = false;
                                db.Entry(item22).State = EntityState.Modified;
                            }
                        }
                        if (matchClassTiming == 0)
                        {
                            CLASS_TIMING ct = new CLASS_TIMING() { BTCH_ID = batch_id, NAME = item22.NAME, START_TIME = item22.START_TIME, END_TIME = item22.END_TIME, IS_BRK = item22.IS_BRK, IS_DEL = false };
                            db.CLASS_TIMING.Add(ct);
                        }
                    }
                    try { db.SaveChanges(); }
                    catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; return RedirectToAction("Index", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage }); }
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
                        WkToUpdate.IS_DEL = true;
                        db.Entry(WkToUpdate).State = EntityState.Modified;
                    }                  
                }
                var CT_Old = db.CLASS_TIMING.Where(x => x.BTCH_ID == batch_id).ToList();
                foreach (var item41 in CT_Old)
                {
                    int matchClassTiming2 = 0;               
                    foreach (var item42 in model.WeekdayIds.Where(x => x.Select == true))
                    {
                        var wk_inner = db.WEEKDAYs.Where(x => x.WKDAY == item42.Day).FirstOrDefault();
                        var ClassTimingEntry = db.CLASS_TIMING_ENTRY.Where(x => x.CLASS_TIMING_SET_ID == wk_inner.CLASS_TIMING_SET_ID).ToList();
                        foreach (var item22 in ClassTimingEntry)
                        {
                            if (item22.START_TIME == item41.START_TIME && item22.END_TIME == item41.END_TIME)
                            {
                                matchClassTiming2 = 1;

                            }
                        }
                    }
                    if (matchClassTiming2 == 0)
                    {
                        CLASS_TIMING ct_inner = db.CLASS_TIMING.Find(item41.ID);
                        ct_inner.IS_DEL = true;
                        db.Entry(ct_inner).State = EntityState.Modified;
                    }

                }
                try { db.SaveChanges(); }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; return RedirectToAction("Index", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage }); }
                ViewBag.Notice = "Weekdays modified.";
                return RedirectToAction("Index",new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage });
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
