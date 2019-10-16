using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace SFSAcademy.Controllers
{
    public class Timetable_EntriesController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Timetable_Entries
        public ActionResult New(int? timetable_id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var timetable = db.TIMETABLEs.Find(timetable_id);
            DateTime StartDate = HtmlHelpers.ApplicationHelper.AcademicYearStartDate();
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.END_DATE >= StartDate
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
            ViewBag.Edit = "Insert";
            return View(timetable);
        }

        // GET: Timetable_Entries
        public ActionResult New_Entry(int? timetable_id, int? batch_id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            ViewBag.timetable_id = timetable_id;
            ViewBag.batch_id = batch_id;
            TIMETABLE timetable = db.TIMETABLEs.Find(timetable_id);
            BATCH batch = db.BATCHes.Include(x=>x.COURSE).Where(x=>x.ID == batch_id).FirstOrDefault();
            ViewData["batch"] = batch;

            var class_timing = db.CLASS_TIMING.Where(x => x.BTCH_ID == batch_id).ToList();
            ViewData["class_timing"] = class_timing;

            TTE_From_Batch_And_TT(timetable.ID, batch.ID);
            return PartialView("_New_Entry");
        }

        // GET: Timetable_Entries
        public ActionResult Update_Employees(int? subject_id,int? timetable_id, int? batch_id)
        {
            ViewBag.timetable_id = timetable_id;
            ViewBag.batch_id = batch_id;
            var employees_subject = db.EMPLOYEES_SUBJECT.Include(x=>x.SUBJECT).Include(x=>x.SUBJECT.ELECTIVE_GROUP).Include(x=>x.EMPLOYEE).Where(x => x.SUBJ_ID == subject_id).ToList();
            ViewData["employees_subject"] = employees_subject;
            return PartialView("_Employee_List");
        }


        public ActionResult Update_Multiple_Timetable_Entries2(int? emp_sub_id, string tte_ids, int? timetable_id, int? batch_id)
        {
            ViewBag.timetable_id = timetable_id;
            ViewBag.batch_id = batch_id;
            TIMETABLE timetable = db.TIMETABLEs.Find(timetable_id);
            BATCH batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == batch_id).FirstOrDefault();
            ViewData["batch"] = batch;
            EMPLOYEES_SUBJECT employees_subject = db.EMPLOYEES_SUBJECT.Include(x => x.EMPLOYEE).Include(x=>x.SUBJECT).Where(x=>x.ID == emp_sub_id).FirstOrDefault();
            var employees_subject_list = db.EMPLOYEES_SUBJECT.Include(x => x.EMPLOYEE).Include(x => x.SUBJECT).Where(x => x.ID == emp_sub_id);
            SUBJECT subject = db.SUBJECTs.Find(employees_subject.SUBJ_ID);
            EMPLOYEE employee = db.EMPLOYEEs.Include(x=>x.EMPLOYEE_GRADE).Where(x=>x.ID == employees_subject.EMP_ID).FirstOrDefault();

            DataTable validation_problems = new DataTable();
            validation_problems.Columns.Add("sub_id", typeof(int));
            validation_problems.Columns.Add("emp_id", typeof(int));
            validation_problems.Columns.Add("tte_id", typeof(string));
            validation_problems.Columns.Add("weekday_id", typeof(int));
            validation_problems.Columns.Add("class_timing_id", typeof(int));
            validation_problems.Columns.Add("messages", typeof(string));

            foreach (var tte_ids_list in HtmlHelpers.ApplicationHelper.SplitCommaString(tte_ids).ToList())
            {
                var weekday = Convert.ToInt32(tte_ids_list.Split('_')[0]);
                var class_timing = Convert.ToInt32(tte_ids_list.Split('_')[1]);
                TIMETABLE_ENTRY tte = db.TIMETABLE_ENTRY.Where(x => x.WK_DAY_ID == weekday && x.CLS_TMNG_ID == class_timing && x.BTCH_ID == batch_id && x.TIMT_ID == timetable_id).FirstOrDefault();
                string Message = "";
                if(subject.MAX_WKILY_CLSES != null)
                {
                    if(subject.MAX_WKILY_CLSES <= db.TIMETABLE_ENTRY.Where(x => x.SUBJ_ID == subject.ID && x.TIMT_ID == timetable_id).ToList().Count())
                    {
                        Message = "Weekly subject limit reached.";
                        var row = validation_problems.NewRow();
                        row["sub_id"] = employees_subject.SUBJ_ID;
                        row["emp_id"] = employees_subject.EMP_ID;
                        row["weekday_id"] = weekday;
                        row["class_timing_id"] = class_timing;
                        if (tte != null)
                        {
                            row["tte_id"] = string.Concat(weekday, "_", class_timing);
                        }
                        row["messages"] = Message;
                        validation_problems.Rows.Add(row);
                    }
                }

                CLASS_TIMING ct_for_tte = db.CLASS_TIMING.Find(class_timing);
                WEEKDAY wd_for_tte = db.WEEKDAYs.Find(weekday);
                //var ClassTimingOverlap = db.CLASS_TIMING.Where(x => x.START_TIME == ct_for_tte.START_TIME && x.END_TIME == ct_for_tte.END_TIME && x.IS_DEL == false).ToList();
                var overlap = (from timete in db.TIMETABLE_ENTRY.Include(x => x.BATCH).Include(x => x.BATCH.COURSE).Include(x => x.CLASS_TIMING).Include(x => x.WEEKDAY)
                               join sub in db.SUBJECTs on timete.SUBJ_ID equals sub.ID
                               join bt in db.BATCHes on sub.BTCH_ID equals bt.ID
                               where timete.TIMT_ID == timetable.ID && timete.WK_DAY_ID != wd_for_tte.ID && timete.WEEKDAY.NAME == wd_for_tte.NAME && timete.CLS_TMNG_ID != ct_for_tte.ID && timete.CLASS_TIMING.NAME == ct_for_tte.NAME && timete.EMP_ID == employee.ID && bt.IS_ACT == true && bt.IS_DEL == false
                               select new Timetable_Entries { TimetableEntryData = timete, BatchData = bt}).ToList();
                if (overlap != null && overlap.Count() != 0)
                {
                    ViewData["overlap"] = overlap;
                    var row = validation_problems.NewRow();
                    row["sub_id"] = employees_subject.SUBJ_ID;
                    row["emp_id"] = employees_subject.EMP_ID;
                    row["weekday_id"] = weekday;
                    row["class_timing_id"] = class_timing;
                    if (tte != null)
                    {
                        row["tte_id"] = string.Concat(weekday, "_", class_timing);
                    }
                    row["weekday_id"] = overlap.FirstOrDefault().TimetableEntryData.WK_DAY_ID;
                    row["class_timing_id"] = overlap.FirstOrDefault().TimetableEntryData.CLS_TMNG_ID;
                    foreach (var item2 in overlap)
                    {
                        Message = string.Concat("Class overlap occured with Batch :", item2.TimetableEntryData.BATCH.COURSE.CODE, "-", item2.TimetableEntryData.BATCH.NAME);
                    }
                    row["messages"] = Message;
                    validation_problems.Rows.Add(row);
                }

                if (subject.ELECTIVE_GRP_ID != null)
                {
                    subject subject_dy_gr = new subject();
                    employee = subject_dy_gr.Lower_Day_Grade(subject);
                }
                if(employee.EMPLOYEE_GRADE.MAX_DILY_HRS != null)
                {
                    int? tte_count = (from tte_in in db.TIMETABLE_ENTRY
                                     join sub in db.SUBJECTs on tte_in.SUBJ_ID equals sub.ID
                                     join bt in db.BATCHes on sub.BTCH_ID equals bt.ID
                                     where bt.IS_ACT == true && bt.IS_DEL == false && tte_in.WK_DAY_ID == weekday && tte_in.EMP_ID == employee.ID && tte_in.TIMT_ID == timetable.ID
                                     select new { tte_in }).ToList().Count();

                    if (employee.EMPLOYEE_GRADE.MAX_DILY_HRS <= tte_count)
                    {
                        Message = "Max hours per day exceeded";
                        var row = validation_problems.NewRow();
                        row["sub_id"] = employees_subject.SUBJ_ID;
                        row["emp_id"] = employees_subject.EMP_ID;
                        row["weekday_id"] = weekday;
                        row["class_timing_id"] = class_timing;
                        if (tte != null)
                        {
                            row["tte_id"] = string.Concat(weekday, "_", class_timing);
                        }
                        row["messages"] = Message;
                        validation_problems.Rows.Add(row);
                    }
                }

                if (subject.ELECTIVE_GRP_ID != null)
                {
                    subject subject_wk_gr = new subject();
                    employee = subject_wk_gr.Lower_Week_Grade(subject);
                }
                if (employee.EMPLOYEE_GRADE.MAX_WKILY_HRS != null)
                {
                    int? tte_count = (from tte_in in db.TIMETABLE_ENTRY
                                      join sub in db.SUBJECTs on tte_in.SUBJ_ID equals sub.ID
                                      join bt in db.BATCHes on sub.BTCH_ID equals bt.ID
                                      where bt.IS_ACT == true && bt.IS_DEL == false && tte_in.EMP_ID == employee.ID && tte_in.TIMT_ID == timetable.ID
                                      select new { tte_in }).ToList().Count();

                    if (employee.EMPLOYEE_GRADE.MAX_WKILY_HRS <= tte_count)
                    {
                        Message = "Max hours per week exceeded";
                        var row = validation_problems.NewRow();
                        row["sub_id"] = employees_subject.SUBJ_ID;
                        row["emp_id"] = employees_subject.EMP_ID;
                        row["weekday_id"] = weekday;
                        row["class_timing_id"] = class_timing;
                        if (tte != null)
                        {
                            row["tte_id"] = string.Concat(weekday, "_", class_timing);
                        }
                        row["messages"] = Message;
                        validation_problems.Rows.Add(row);
                    }
                }

                if(Message == "")
                {
                    if(tte != null)
                    {
                        TIMETABLE_ENTRY tte_upd = db.TIMETABLE_ENTRY.Find(tte.ID);
                        tte_upd.SUBJ_ID = subject.ID;
                        tte_upd.EMP_ID = employee.ID;
                        tte_upd.TIMT_ID = timetable.ID;
                        db.Entry(tte_upd).State = EntityState.Modified;                       
                    }
                    else
                    {
                        TIMETABLE_ENTRY tte_upd = new TIMETABLE_ENTRY { WK_DAY_ID = weekday, CLS_TMNG_ID = class_timing, SUBJ_ID = subject.ID, EMP_ID = employee.ID, BTCH_ID = batch_id, TIMT_ID = timetable.ID };
                        db.TIMETABLE_ENTRY.Add(tte_upd);
                        tte = tte_upd;
                    }
                    try { db.SaveChanges(); }
                    catch (DbEntityValidationException e){foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }return PartialView("_New_Entry");}
                    catch (Exception e){ ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message));return PartialView("_New_Entry");}
                }
            }
            ViewBag.validation_problems = validation_problems.AsEnumerable();
            TTE_From_Batch_And_TT(timetable.ID, batch_id);
            return PartialView("_New_Entry");
        }

        public ActionResult TT_Entry_Noupdate2(int? emp_id, int? sub_id, int? tte_id, int? timetable_id, DataTable validation_problems)
        {
            //This is not build yet and would be filled as and when technically designed.
            return PartialView("_New_Entry");
        }

        public JsonResult TT_Entry_Update2(int? emp_id, int? sub_id, string tte_id, int? timetable_id, int? batch_id, int? overwrite, string message)
        {            
            ViewBag.timetable_id = timetable_id;
            ViewBag.batch_id = batch_id;
            TIMETABLE timetable = db.TIMETABLEs.Find(timetable_id);
            //ViewData["timetable"] = timetable;
            BATCH batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == batch_id).FirstOrDefault();
            ViewData["batch"] = batch;           
            SUBJECT subject = db.SUBJECTs.Find(sub_id);
            var weekday = Convert.ToInt32(tte_id.Split('_')[0]);
            var class_timing = Convert.ToInt32(tte_id.Split('_')[1]);
            CLASS_TIMING ct_for_tte = db.CLASS_TIMING.Find(class_timing);
            WEEKDAY wd_for_tte = db.WEEKDAYs.Find(weekday);
            TIMETABLE_ENTRY tte = db.TIMETABLE_ENTRY.Where(x => x.WK_DAY_ID == weekday && x.CLS_TMNG_ID == class_timing && x.BTCH_ID == batch_id && x.TIMT_ID == timetable_id).FirstOrDefault();
            var overlapped_tte = db.TIMETABLE_ENTRY.Include(x=>x.WEEKDAY).Include(x=>x.CLASS_TIMING).Where(x => x.WEEKDAY.NAME == wd_for_tte.NAME && x.CLASS_TIMING.START_TIME == ct_for_tte.START_TIME && x.CLASS_TIMING.END_TIME == ct_for_tte.END_TIME && x.EMP_ID == emp_id && x.TIMT_ID == timetable_id).ToList();
            if(overlapped_tte == null || overlapped_tte.Count() == 0)
            {
                if(tte != null)
                {
                    TIMETABLE_ENTRY tte_upd = db.TIMETABLE_ENTRY.Find(tte.ID);
                    tte_upd.SUBJ_ID = sub_id;
                    tte_upd.EMP_ID = emp_id;
                    db.Entry(tte_upd).State = EntityState.Modified;
                }
                else
                {
                    TIMETABLE_ENTRY tte_new = new TIMETABLE_ENTRY { WK_DAY_ID = weekday, CLS_TMNG_ID = class_timing, SUBJ_ID = sub_id, EMP_ID = emp_id, BTCH_ID = batch.ID, TIMT_ID = timetable.ID };
                    db.TIMETABLE_ENTRY.Add(tte_new);
                }
            }
            else
            {
                if(overwrite != null)
                {
                    foreach(var item in overlapped_tte)
                    {
                        //db.TIMETABLE_ENTRY.Remove(item);
                        TIMETABLE_ENTRY tte_upd = db.TIMETABLE_ENTRY.Find(item.ID);
                        tte_upd.SUBJ_ID = null;
                        tte_upd.EMP_ID = null;
                        db.Entry(tte_upd).State = EntityState.Modified;
                    }
                }
                if (tte != null)
                {
                    TIMETABLE_ENTRY tte_upd = db.TIMETABLE_ENTRY.Find(tte.ID);
                    tte_upd.SUBJ_ID = sub_id;
                    tte_upd.EMP_ID = emp_id;
                    db.Entry(tte_upd).State = EntityState.Modified;
                }
                else
                {
                    TIMETABLE_ENTRY tte_new = new TIMETABLE_ENTRY { WK_DAY_ID = weekday, CLS_TMNG_ID = class_timing, SUBJ_ID = sub_id, EMP_ID = emp_id, BTCH_ID = batch.ID, TIMT_ID = timetable.ID };
                    db.TIMETABLE_ENTRY.Add(tte_new);
                }
            }
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e) { foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } } }
            catch (Exception e) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message)); }

            TTE_From_Batch_And_TT(timetable.ID, batch_id);

            //MyResultsModel model = CalculateOutputData(someExampleInput);
            // var EmployeeSelectPartialView = RenderRazorViewToString(this.ControllerContext, "_Employee_Select", model.TotalValuesModel);
            var EmployeeSelectPartialView = RenderRazorViewToString(this.ControllerContext, "_Employee_Select", null);
            //var TimetableBoxPartialView = RenderRazorViewToString(this.ControllerContext, "_Timetable_Box", model.SummaryValuesModel);
            var TimetableBoxPartialView = RenderRazorViewToString(this.ControllerContext, "_Timetable_Box", null);

            return Json(new { EmployeeSelectPartialView, TimetableBoxPartialView }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete_Employee2(int? id)
        {
            TIMETABLE_ENTRY tte = db.TIMETABLE_ENTRY.Find(id);
            ViewData["tte"] = tte;
            ViewBag.timetable_id = tte.TIMT_ID;
            ViewBag.batch_id = tte.BTCH_ID;
            TIMETABLE timetable = db.TIMETABLEs.Find(tte.TIMT_ID);
            BATCH batch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == tte.BTCH_ID).FirstOrDefault();
            ViewData["batch"] = batch;

            tte.SUBJ_ID = null;
            tte.EMP_ID = null;
            db.Entry(tte).State = EntityState.Modified;
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e) { foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } } }
            catch (Exception e) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", string.Concat(e.GetType().FullName, ":", e.Message)); }

            TTE_From_Batch_And_TT(timetable.ID, tte.BTCH_ID);

            return PartialView("_New_Entry");
        }

        private void TTE_From_Batch_And_TT(int? TT, int? batch_id)
        {
            TIMETABLE tt = db.TIMETABLEs.Find(TT);
            var class_timing = db.CLASS_TIMING.Where(x => x.BTCH_ID == batch_id).ToList();
            ViewData["class_timing"] = class_timing;
            if (class_timing == null || class_timing.Count() == 0)
            {
                var default_class_timing = class_timing.DefaultIfEmpty();
                ViewData["class_timing"] = default_class_timing;
            }
            var weekday = db.WEEKDAYs.Where(x => x.BTCH_ID == batch_id).ToList();
            ViewData["weekday"] = weekday;
            if (weekday == null || weekday.Count() == 0)
            {
                var default_weekday = weekday.DefaultIfEmpty();
                ViewData["weekday"] = default_weekday;
            }
            
            foreach(var item in weekday)
            {
                var classTimingEntry = db.CLASS_TIMING_ENTRY.Where(x => x.CLASS_TIMING_SET_ID == item.CLASS_TIMING_SET_ID).ToList();
                foreach(var item2 in classTimingEntry)
                {
                    CLASS_TIMING ct = db.CLASS_TIMING.Where(x => x.BTCH_ID == batch_id && x.START_TIME == item2.START_TIME && x.END_TIME == item2.END_TIME).FirstOrDefault();
                    TIMETABLE_ENTRY tte = db.TIMETABLE_ENTRY.Where(x => x.BTCH_ID == batch_id && x.TIMT_ID == TT && x.WK_DAY_ID == item.ID && x.CLS_TMNG_ID == ct.ID).FirstOrDefault();
                    if (tte == null)
                    {
                        TIMETABLE_ENTRY tte_new = new TIMETABLE_ENTRY { WK_DAY_ID = item.ID, CLS_TMNG_ID = ct.ID,BTCH_ID = batch_id, TIMT_ID = TT };
                        db.TIMETABLE_ENTRY.Add(tte_new);
                    }
                    try { db.SaveChanges(); }
                    catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = string.Concat(e.GetType().FullName, ":", e.Message); return; }
                }                
            }           
            var timetable_entries = db.TIMETABLE_ENTRY.Include(x => x.SUBJECT).Include(x => x.SUBJECT.ELECTIVE_GROUP).Include(x => x.EMPLOYEE).Include(x => x.WEEKDAY).Include(x => x.CLASS_TIMING).Where(x => x.BTCH_ID == batch_id && x.TIMT_ID == TT).ToList();
            ViewData["timetable"] = timetable_entries;
            var subjects = db.SUBJECTs.Where(x => x.BTCH_ID == batch_id && x.ELECTIVE_GRP_ID == null && x.IS_DEL == false).ToList();
            ViewData["subjects"] = subjects;
            var ele_subjects = db.SUBJECTs.Include(x=>x.ELECTIVE_GROUP).Where(x => x.BTCH_ID == batch_id && x.ELECTIVE_GRP_ID != null && x.IS_DEL == false).ToList();
            ViewData["ele_subjects"] = ele_subjects;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            if(model != null)
            {
                controllerContext.Controller.ViewData.Model = model;
            }

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
