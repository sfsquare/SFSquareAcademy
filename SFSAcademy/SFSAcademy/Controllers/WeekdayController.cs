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
            /*var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                         .OrderBy(x => x.BatchData.ID).ToList();
            */
            var queryCourceBatch = db.BATCHes.FirstOrDefault().ACTIVE().ToList();

            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = item.Course_full_name;
                var result = new SelectListItem();
                result.Text = BatchFullName;
                result.Value = item.ID.ToString();
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
            var wd = new Weekdays();
            int match = 0;
            foreach(var item in weekdays)
            {
                if(item.WKDAY == "0")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Sunday", Id = 0, Select =true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if(match == 0) { wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Sunday", Id = 0, ClassTimingSet = "Default" });  }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "1")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Monday", Id = 1, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Monday", Id = 1, ClassTimingSet = "Default" }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "2")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Tuesday", Id = 2, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Tuesday", Id = 2, ClassTimingSet = "Default" }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "3")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Wednesday", Id = 3, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0){ wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Wednesday", Id = 3, ClassTimingSet = "Default" });}
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "4")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Thursday", Id = 4, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Thursday", Id = 4, ClassTimingSet = "Default" }); }
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "5")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Friday", Id = 5, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0) { wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Friday", Id = 5, ClassTimingSet = "Default" });}
            match = 0;
            foreach (var item in weekdays)
            {
                if (item.WKDAY == "6")
                {
                    match = 1;
                    string ClassTimingSetVal = item.CLASS_TIMING_SET_ID == null ? "Default" : db.CLASS_TIMING_SET.Find(item.CLASS_TIMING_SET_ID).NAME.ToString();
                    wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Saturday", Id = 6, Select = true, ClassTimingSet = ClassTimingSetVal });
                    break;
                }
            }
            if (match == 0){ wd.WeekdayIds.Add(new WeekdaysSelect { Day = "Saturday", Id = 6, ClassTimingSet = "Default" }); }
            //return View(vm);
            ViewBag.BTCH_ID = id;

            return PartialView("_Weekdays", wd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Weekdays model, int? batch_id)
        {
            if (ModelState.IsValid)
            {
                var old = db.WEEKDAYs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == false).ToList();
                var old_Inactive = db.WEEKDAYs.Where(x => x.BTCH_ID == batch_id && x.IS_DEL == true).ToList();                

                foreach (var WkDayIds in model.WeekdayIds.Where(x=>x.Select == true))
                {
                    int match = 0;
                    int? TimingSetId = db.CLASS_TIMING_SET.Where(x => x.NAME == WkDayIds.ClassTimingSet).FirstOrDefault().ID;
                    foreach (var OldIn in old_Inactive)
                    {
                        if (WkDayIds.Id.ToString() == OldIn.WKDAY)
                        {
                            match = 1;
                            WEEKDAY WkToUpdate = db.WEEKDAYs.Find(OldIn.ID);
                            WkToUpdate.IS_DEL = false;
                            WkToUpdate.CLASS_TIMING_SET_ID = TimingSetId;                            
                            db.Entry(WkToUpdate).State = EntityState.Modified;                                        
                            break;
                        }
                    }
                    foreach (var OldWkD in old)
                    {
                        if(WkDayIds.Id.ToString() == OldWkD.WKDAY)
                        {
                            match = 2;
                            WEEKDAY WkToUpdate = db.WEEKDAYs.Find(OldWkD.ID);
                            WkToUpdate.IS_DEL = false;
                            WkToUpdate.CLASS_TIMING_SET_ID = TimingSetId;
                            db.Entry(WkToUpdate).State = EntityState.Modified;
                            break;
                        }
                    }
                    if (match == 0)
                    {

                        WEEKDAY WkDay = new WEEKDAY() { NAME = WkDayIds.Day, WKDAY = WkDayIds.Id.ToString(), BTCH_ID = batch_id, DAY_OF_WK = WkDayIds.Id, SRT_ORD = WkDayIds.Id, IS_DEL = false, CLASS_TIMING_SET_ID = TimingSetId };
                        db.WEEKDAYs.Add(WkDay);
                    }
                    int matchClassTiming = 0;
                    var ClassTiming = db.CLASS_TIMING.Where(x => x.BTCH_ID == batch_id).ToList();
                    var ClassTimingEntry = db.CLASS_TIMING_ENTRY.Where(x => x.CLASS_TIMING_SET_ID == TimingSetId).ToList();
                    foreach (var ctEntry in ClassTimingEntry)
                    {
                        foreach (var ct in ClassTiming)
                        {
                            if (ctEntry.START_TIME == ct.START_TIME && ctEntry.END_TIME == ct.END_TIME)
                            {
                                matchClassTiming = 1;
                                ct.IS_DEL = false;
                                db.Entry(ct).State = EntityState.Modified;
                            }
                        }
                        if (matchClassTiming == 0)
                        {
                            CLASS_TIMING ct = new CLASS_TIMING() { BTCH_ID = batch_id, NAME = ctEntry.NAME, START_TIME = ctEntry.START_TIME, END_TIME = ctEntry.END_TIME, IS_BRK = ctEntry.IS_BRK, IS_DEL = false };
                            db.CLASS_TIMING.Add(ct);
                        }
                    }
                    try { db.SaveChanges(); }
                    catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; return RedirectToAction("Index", new { Notice = ViewBag.Notice, ErrorMessage = ViewBag.ErrorMessage }); }
                }
                foreach(var OldWkD in old)
                {
                    int matchWeekday = 0;
                    foreach(var wkDayId in model.WeekdayIds.Where(x => x.Select == true))
                    {
                        if(OldWkD.WKDAY == wkDayId.Id.ToString())
                        {
                            matchWeekday = 1;
                            break;
                        }
                    }
                    if(matchWeekday == 0)
                    {
                        WEEKDAY WkToUpdate = db.WEEKDAYs.Find(OldWkD.ID);
                        WkToUpdate.IS_DEL = true;
                        db.Entry(WkToUpdate).State = EntityState.Modified;
                    }                  
                }
                var ClassTimingOld = db.CLASS_TIMING.Where(x => x.BTCH_ID == batch_id).ToList();
                foreach (var ctOld in ClassTimingOld)
                {
                    int matchClassTiming2 = 0;               
                    foreach (var wkDayId in model.WeekdayIds.Where(x => x.Select == true))
                    {
                        var wk_inner = db.WEEKDAYs.Where(x => x.WKDAY == wkDayId.Id.ToString() && x.BTCH_ID == ctOld.BTCH_ID).FirstOrDefault();
                        var ClassTimingEntry = db.CLASS_TIMING_ENTRY.Where(x => x.CLASS_TIMING_SET_ID == wk_inner.CLASS_TIMING_SET_ID).ToList();
                        foreach (var ctEntry in ClassTimingEntry)
                        {
                            if (ctEntry.START_TIME == ctOld.START_TIME && ctEntry.END_TIME == ctOld.END_TIME)
                            {
                                matchClassTiming2 = 1;
                                ctOld.IS_DEL = false;
                                db.Entry(ctOld).State = EntityState.Modified;
                                break;
                            }
                        }
                    }
                    if (matchClassTiming2 == 0)
                    {
                        CLASS_TIMING ct_inner = db.CLASS_TIMING.Find(ctOld.ID);
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
