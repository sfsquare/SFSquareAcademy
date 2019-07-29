using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.Web.UI.WebControls;
using System;
using SFSAcademy;
using SFSAcademy.HtmlHelpers;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace SFSAcademy.Controllers
{
    public class StudentController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Student
        public ActionResult Index()
        {
            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID
                            join cs in db.COURSEs on b.CRS_ID equals cs.ID
                            where st.IS_DEL == false
                            orderby st.LAST_NAME, b.NAME
                            select new Student { StudentData = st, BatcheData = b, CourseData = cs }).Distinct();

            return View(StudentS.ToList());
        }


        // GET: Student
        public ActionResult ViewAll(string currentFilter, string searchString)
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
                if (!string.IsNullOrEmpty(currentFilter))
                {
                    result.Selected = item.BatchData.ID.ToString() == currentFilter ? true : false;
                }
                if (!string.IsNullOrEmpty(searchString))
                {
                    result.Selected = item.BatchData.ID.ToString() == searchString ? true : false;
                }               
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.searchString = options;
            if(!string.IsNullOrEmpty(currentFilter))
            {
                ViewBag.CurrentFilter = currentFilter;
            }
            return View(db.BATCHes.ToList());
        }

        // GET: Student
        public ActionResult _ListStudentsByCourse(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            int searchStringId = 0;
            if (searchString != null && !searchString.Equals("-1"))
            {
                page = 1;
                //searchString = db.BATCHes.Find(searchStringId).NAME.ToString();
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

            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID
                            join cs in db.COURSEs on b.CRS_ID equals cs.ID
                            where st.IS_DEL == false
                            orderby st.LAST_NAME, b.NAME
                            select new Student { StudentData = st, BatcheData = b, CourseData = cs }).Distinct();

            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                StudentS = StudentS.Where(s => s.BatcheData.ID.Equals(searchStringId));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.LAST_NAME);
                    break;
                case "Date":
                    StudentS = StudentS.OrderBy(s => s.StudentData.ADMSN_DATE);
                    break;
                case "date_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.ADMSN_DATE);
                    break;
                default:  // Name ascending 
                    StudentS = StudentS.OrderBy(s => s.StudentData.LAST_NAME);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(StudentS.ToPagedList(pageNumber, pageSize));
            //return View(db.USERS.ToList());
        }

        // GET: Fee Index
        public ActionResult BatchTransfer_Index()
        {
            return View();
        }

        // GET: Student
        public ActionResult BatchTransfer(string searchString, string currentFilter, string BatchId)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new RadioCourseBatch { CourseData = cs, BatchData = bt})
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
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Batch" });
            ViewBag.searchString = options;

            if ((!string.IsNullOrEmpty(searchString) && searchString != "-1") || (!string.IsNullOrEmpty(currentFilter)))
            {
                ViewBag.IsPostBack = 1;
            }
            if ((!string.IsNullOrEmpty(BatchId)))
            {
                if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
                {
                    int searchStringVal = Convert.ToInt32(searchString);
                    var OldBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == searchStringVal);

                    var sTUDENTfROM = db.STUDENTs.Where(x => x.BTCH_ID == searchStringVal && x.IS_DEL == false && x.IS_ACT == true).ToList();
                    int BatchIdVal = Convert.ToInt32(BatchId);
                    var NewBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == BatchIdVal);
                    string FaultedStudents = "";
                    foreach (var item in sTUDENTfROM)
                    {
                        STUDENT sTUDENTfROMuPD = db.STUDENTs.Find(item.ID);
                        var paid_fees_val = (from ff in db.FINANCE_FEE
                                             join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                             join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                             where ff.STDNT_ID == sTUDENTfROMuPD.ID && ff.IS_PD == false
                                             select new StundentFee { FeeCollectionData = fc, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                       // ViewData["paid_fees"] = paid_fees_val;

                        if (paid_fees_val != null && paid_fees_val.Count() != 0)
                        {
                            FaultedStudents = string.Concat(sTUDENTfROMuPD.ADMSN_NO, ",", FaultedStudents);
                        }
                        else
                        {
                            var sTUDENTtOiNS = new ARCHIVED_STUDENT()
                            {
                                ADMSN_NO = sTUDENTfROMuPD.ADMSN_NO,
                                CLS_ROLL_NO = sTUDENTfROMuPD.CLS_ROLL_NO,
                                ADMSN_DATE = sTUDENTfROMuPD.ADMSN_DATE,
                                FIRST_NAME = sTUDENTfROMuPD.FIRST_NAME,
                                MID_NAME = sTUDENTfROMuPD.MID_NAME,
                                LAST_NAME = sTUDENTfROMuPD.LAST_NAME,
                                BTCH_ID = sTUDENTfROMuPD.BTCH_ID,
                                DOB = sTUDENTfROMuPD.DOB,
                                GNDR = sTUDENTfROMuPD.GNDR,
                                BLOOD_GRP = sTUDENTfROMuPD.BLOOD_GRP,
                                BIRTH_PLACE = sTUDENTfROMuPD.BIRTH_PLACE,
                                LANG = sTUDENTfROMuPD.LANG,
                                RLGN = sTUDENTfROMuPD.RLGN,
                                ADDR_LINE1 = sTUDENTfROMuPD.ADDR_LINE1,
                                ADDR_LINE2 = sTUDENTfROMuPD.ADDR_LINE2,
                                CITY = sTUDENTfROMuPD.CITY,
                                STATE = sTUDENTfROMuPD.STATE,
                                PIN_CODE = sTUDENTfROMuPD.PIN_CODE,
                                CTRY_ID = sTUDENTfROMuPD.CTRY_ID,
                                PH1 = sTUDENTfROMuPD.PH1.ToString(),
                                PH2 = sTUDENTfROMuPD.PH2.ToString(),
                                EML = sTUDENTfROMuPD.EML,
                                IMMDT_CNTCT_ID = sTUDENTfROMuPD.IMMDT_CNTCT_ID,
                                IS_SMS_ENABL = sTUDENTfROMuPD.IS_SMS_ENABL,
                                PHTO_FILENAME = sTUDENTfROMuPD.PHTO_FILENAME,
                                PHTO_CNTNT_TYPE = sTUDENTfROMuPD.PHTO_CNTNT_TYPE,
                                PHTO_DATA = sTUDENTfROMuPD.IMAGE_DOCUMENTS_ID.ToString(),
                                STAT_DESCR = sTUDENTfROMuPD.STAT_DESCR,
                                IS_ACT = false,
                                IS_DEL = true,
                                CREATED_AT = sTUDENTfROMuPD.CREATED_AT,
                                UPDATED_AT = DateTime.Now,
                                PHTO_FILE_SIZE = sTUDENTfROMuPD.PHTO_FILE_SIZE,
                                STDNT_CAT_ID = sTUDENTfROMuPD.STDNT_CAT_ID,
                                NTLTY_ID = sTUDENTfROMuPD.NTLTY_ID,
                            };
                            db.ARCHIVED_STUDENT.Add(sTUDENTtOiNS);

                            sTUDENTfROMuPD.BTCH_ID = BatchIdVal;
                            sTUDENTfROMuPD.UPDATED_AT = System.DateTime.Now;
                            db.Entry(sTUDENTfROMuPD).State = EntityState.Modified;
                            try { db.SaveChanges(); }
                            catch (Exception e) { ViewBag.BatchTransferMessage = e.InnerException.InnerException.Message; }
                            //Find all available Fee Collection which are active.
                            var FeeCollectionSet = (from ffc in db.FINANCE_FEE_CATGEORY
                                                    join b in db.BATCHes on ffc.BTCH_ID equals b.ID
                                                    join st in db.STUDENTs on b.ID equals st.BTCH_ID
                                                    join fcol in db.FINANCE_FEE_COLLECTION on new { A = ffc.ID.ToString(), B = b.ID.ToString() } equals new { A = fcol.FEE_CAT_ID.ToString(), B = fcol.BTCH_ID.ToString() }
                                                    where st.ID == sTUDENTfROMuPD.ID && ffc.IS_DEL.Equals(false) && b.IS_DEL == false && st.IS_DEL == false && fcol.START_DATE <= System.DateTime.Now && fcol.END_DATE >= System.DateTime.Now && !ffc.NAME.Contains("Admission")
                                                    select new { FinanceFeeCategoryData = ffc, BatchData = b, StudentData = st, FeeCollectionData = fcol }).OrderBy(g => g.FinanceFeeCategoryData.ID).ToList();
                            foreach (var item2 in FeeCollectionSet)
                            {
                                STUDENT FeeStudent = db.STUDENTs.Find(item2.StudentData.ID);
                                FeeStudent.HAS_PD_FE = false;
                                FeeStudent.UPDATED_AT = System.DateTime.Now;
                                var FF_fEE = new FINANCE_FEE() { STDNT_ID = item2.StudentData.ID, FEE_CLCT_ID = item2.FeeCollectionData.ID, IS_PD = false };
                                db.FINANCE_FEE.Add(FF_fEE);
                            }
                            try { db.SaveChanges(); }
                            catch (Exception e) { ViewBag.BatchTransferMessage = e.InnerException.InnerException.Message; }
                        }                        
                    }
                    if(FaultedStudents != "")
                    {
                        ViewBag.BatchTransferMessage = string.Concat("Students of batch ", OldBatch.FirstOrDefault().COURSE.CODE, "-", OldBatch.FirstOrDefault().NAME, ", Except with Admission Numbers: ", FaultedStudents, " are transfered to ", NewBatch.FirstOrDefault().COURSE.CODE, "-", NewBatch.FirstOrDefault().NAME, ". Please check Fees Paid History for faulted students.");
                    }
                    else
                    {
                        ViewBag.BatchTransferMessage = string.Concat("All students of batch ", OldBatch.FirstOrDefault().COURSE.CODE, "-", OldBatch.FirstOrDefault().NAME, " are transfered to ", NewBatch.FirstOrDefault().COURSE.CODE, "-", NewBatch.FirstOrDefault().NAME);
                    }
                }
                ViewBag.IsPostBack = 0;
            }

            return View(queryCourceBatch.ToList());
        }

        // GET: Student
        public ActionResult BatchTransfer_Student_Search(string AdmissionNumber, string BatchId)
        {
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    select new RadioCourseBatch { CourseData = cs, BatchData = bt })
                        .OrderBy(x => x.BatchData.ID).ToList();

            if (!string.IsNullOrEmpty(AdmissionNumber))
            {
                ViewBag.IsPostBack = 1;
            }
            if ((!string.IsNullOrEmpty(BatchId)))
            {
                int BatchIdVal = Convert.ToInt32(BatchId);
                BATCH NewBatch = db.BATCHes.Include(x => x.COURSE).Where(x => x.ID == BatchIdVal).FirstOrDefault();
                BATCH OldBatch = db.BATCHes.Where(x=>x.ID == -1).FirstOrDefault();

                if ((!string.IsNullOrEmpty(AdmissionNumber)))
                {
                    var sTUDENTfROM = db.STUDENTs.Where(x => x.ADMSN_NO == AdmissionNumber && x.IS_DEL == false && x.IS_ACT == true).ToList().FirstOrDefault();
                    OldBatch = db.BATCHes.Include(x => x.COURSE).Where(x=>x.ID == sTUDENTfROM.BTCH_ID).FirstOrDefault();

                    STUDENT sTUDENTfROMuPD = db.STUDENTs.Find(sTUDENTfROM.ID);
                    var paid_fees_val = (from ff in db.FINANCE_FEE
                                         join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                         join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                         where ff.STDNT_ID == sTUDENTfROMuPD.ID && ff.IS_PD == false
                                         select new StundentFee { FeeCollectionData = fc, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                    // ViewData["paid_fees"] = paid_fees_val;

                    if (paid_fees_val != null && paid_fees_val.Count() != 0)
                    {
                        ViewBag.BatchTransferMessage = string.Concat("Following fees are pending to be paid by stundent: \n");
                        int i = 0;
                        foreach(var item in paid_fees_val)
                        {
                            ViewBag.BatchTransferMessage = string.Concat(ViewBag.BatchTransferMessage, i,") ", item.FeeCollectionData.NAME, ", BatchId: ", item.FeeCollectionData.BTCH_ID, "\n");
                            i++;
                        }
                        ViewBag.BatchTransferMessage = string.Concat(ViewBag.BatchTransferMessage, "Please pay all pending fees before attempting Batch Transfer.");
                    }
                    else
                    {
                        var sTUDENTtOiNS = new ARCHIVED_STUDENT()
                        {
                            ADMSN_NO = sTUDENTfROMuPD.ADMSN_NO,
                            CLS_ROLL_NO = sTUDENTfROMuPD.CLS_ROLL_NO,
                            ADMSN_DATE = sTUDENTfROMuPD.ADMSN_DATE,
                            FIRST_NAME = sTUDENTfROMuPD.FIRST_NAME,
                            MID_NAME = sTUDENTfROMuPD.MID_NAME,
                            LAST_NAME = sTUDENTfROMuPD.LAST_NAME,
                            BTCH_ID = sTUDENTfROMuPD.BTCH_ID,
                            DOB = sTUDENTfROMuPD.DOB,
                            GNDR = sTUDENTfROMuPD.GNDR,
                            BLOOD_GRP = sTUDENTfROMuPD.BLOOD_GRP,
                            BIRTH_PLACE = sTUDENTfROMuPD.BIRTH_PLACE,
                            LANG = sTUDENTfROMuPD.LANG,
                            RLGN = sTUDENTfROMuPD.RLGN,
                            ADDR_LINE1 = sTUDENTfROMuPD.ADDR_LINE1,
                            ADDR_LINE2 = sTUDENTfROMuPD.ADDR_LINE2,
                            CITY = sTUDENTfROMuPD.CITY,
                            STATE = sTUDENTfROMuPD.STATE,
                            PIN_CODE = sTUDENTfROMuPD.PIN_CODE,
                            CTRY_ID = sTUDENTfROMuPD.CTRY_ID,
                            PH1 = sTUDENTfROMuPD.PH1.ToString(),
                            PH2 = sTUDENTfROMuPD.PH2.ToString(),
                            EML = sTUDENTfROMuPD.EML,
                            IMMDT_CNTCT_ID = sTUDENTfROMuPD.IMMDT_CNTCT_ID,
                            IS_SMS_ENABL = sTUDENTfROMuPD.IS_SMS_ENABL,
                            PHTO_FILENAME = sTUDENTfROMuPD.PHTO_FILENAME,
                            PHTO_CNTNT_TYPE = sTUDENTfROMuPD.PHTO_CNTNT_TYPE,
                            PHTO_DATA = sTUDENTfROMuPD.IMAGE_DOCUMENTS_ID.ToString(),
                            STAT_DESCR = sTUDENTfROMuPD.STAT_DESCR,
                            IS_ACT = false,
                            IS_DEL = true,
                            CREATED_AT = sTUDENTfROMuPD.CREATED_AT,
                            UPDATED_AT = System.DateTime.Now,
                            PHTO_FILE_SIZE = sTUDENTfROMuPD.PHTO_FILE_SIZE,
                            STDNT_CAT_ID = sTUDENTfROMuPD.STDNT_CAT_ID,
                            NTLTY_ID = sTUDENTfROMuPD.NTLTY_ID,
                        };
                        db.ARCHIVED_STUDENT.Add(sTUDENTtOiNS);

                        sTUDENTfROMuPD.BTCH_ID = BatchIdVal;
                        sTUDENTfROMuPD.UPDATED_AT = System.DateTime.Now;
                        db.Entry(sTUDENTfROMuPD).State = EntityState.Modified;
                        try { db.SaveChanges(); }
                        catch (Exception e) { ViewBag.BatchTransferMessage = e.InnerException.InnerException.Message; }

                        //Find all available Fee Collection which are active.
                        var FeeCollectionSet = (from ffc in db.FINANCE_FEE_CATGEORY
                                                join b in db.BATCHes on ffc.BTCH_ID equals b.ID
                                                join st in db.STUDENTs on b.ID equals st.BTCH_ID
                                                join fcol in db.FINANCE_FEE_COLLECTION on new { A = ffc.ID.ToString(), B = b.ID.ToString() } equals new { A = fcol.FEE_CAT_ID.ToString(), B = fcol.BTCH_ID.ToString() }
                                                where st.ID == sTUDENTfROMuPD.ID && ffc.IS_DEL.Equals(false) && b.IS_DEL == false && st.IS_DEL == false && fcol.START_DATE <= System.DateTime.Now && fcol.END_DATE >= System.DateTime.Now && !ffc.NAME.Contains("Admission")
                                                select new { FinanceFeeCategoryData = ffc, BatchData = b, StudentData = st, FeeCollectionData = fcol }).OrderBy(g => g.FinanceFeeCategoryData.ID).ToList();
                        foreach (var item2 in FeeCollectionSet)
                        {
                            STUDENT FeeStudent = db.STUDENTs.Find(item2.StudentData.ID);
                            FeeStudent.HAS_PD_FE = false;
                            FeeStudent.UPDATED_AT = System.DateTime.Now;
                            var FF_fEE = new FINANCE_FEE() { STDNT_ID = item2.StudentData.ID, FEE_CLCT_ID = item2.FeeCollectionData.ID, IS_PD = false };
                            db.FINANCE_FEE.Add(FF_fEE);
                        }
                        try { db.SaveChanges(); ViewBag.BatchTransferMessage = string.Concat("Student with Admission Number ", AdmissionNumber, " is transfered from ", OldBatch.COURSE.CODE, "-", OldBatch.NAME, " to ", NewBatch.COURSE.CODE, " -", NewBatch.NAME); }
                        catch (Exception e) { Console.WriteLine(e); ViewBag.BatchTransferMessage = e.InnerException.InnerException.Message; }
                    }
                    
                }
                ViewBag.IsPostBack = 0;
            }
            return View(queryCourceBatch.ToList());
        }
        // GET: Student
        public ActionResult _ListStudentsForTransfer(string sortOrder, string currentFilter, string searchString, int? page, string currentFilter2, string AdmissionNumber)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            int searchStringId = 0;
            if (!string.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                page = 1;
                //searchString = db.BATCHes.Find(searchStringId).NAME.ToString();
                searchStringId = Convert.ToInt32(searchString);
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            if (!string.IsNullOrEmpty(AdmissionNumber)) { page = 1; }
            else { AdmissionNumber = currentFilter2; }
            ViewBag.CurrentFilter2 = AdmissionNumber;

            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID
                            join cs in db.COURSEs on b.CRS_ID equals cs.ID
                            where st.IS_DEL == false
                            orderby st.LAST_NAME, b.NAME
                            select new Student { StudentData = st, BatcheData = b, CourseData = cs }).Distinct();

            if (!String.IsNullOrEmpty(searchString) && !searchString.Equals("-1"))
            {
                StudentS = StudentS.Where(s => s.BatcheData.ID.Equals(searchStringId));
            }
            if (!String.IsNullOrEmpty(AdmissionNumber))
            {
                StudentS = StudentS.Where(s => s.StudentData.ADMSN_NO.Equals(AdmissionNumber));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.LAST_NAME);
                    break;
                case "Date":
                    StudentS = StudentS.OrderBy(s => s.StudentData.ADMSN_DATE);
                    break;
                case "date_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.ADMSN_DATE);
                    break;
                default:  // Name ascending 
                    StudentS = StudentS.OrderBy(s => s.StudentData.LAST_NAME);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(StudentS.ToPagedList(pageNumber, pageSize));
            //return View(db.USERS.ToList());
        }

        // GET: Student/Details/5
        public ActionResult Profiles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID into gi
                            from subb in gi.DefaultIfEmpty()
                            join c in db.COURSEs on subb.CRS_ID equals c.ID into gj
                            from subc in gj.DefaultIfEmpty()
                            join ct in db.COUNTRies on st.NTLTY_ID equals ct.ID into gk
                            from subct in gk.DefaultIfEmpty()
                            join cat in db.STUDENT_CATGEORY on st.STDNT_CAT_ID equals cat.ID into gl
                            from subcat in gl.DefaultIfEmpty()
                            join emp in db.EMPLOYEEs on subb.EMP_ID equals emp.ID into gm
                            from subemp in gm.DefaultIfEmpty()
                            join img in db.IMAGE_DOCUMENTS on st.PHTO_DATA equals img.DOCUMENTID into gn
                            from subimg in gn.DefaultIfEmpty()
                            where st.ID == id
                            orderby st.LAST_NAME, subb.NAME
                            select new Student { StudentData = st, BatcheData = (subb == null ? null : subb), CourseData = (subc == null ? null : subc), CountryData = (subct == null ? null : subct), CategoryData = (subcat == null ? null : subcat), EmployeeData = (subemp == null ? null : subemp), ImageData = (subimg == null ? null : subimg) }).Distinct();

            if (StudentS == null)
            {
                return HttpNotFound();
            }

            return View(StudentS.FirstOrDefault());
        }



        // GET: Student
        [HttpGet]
        public ActionResult ProfilePDF(int? id)
        {

            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID into gi
                            from subb in gi.DefaultIfEmpty()
                            join c in db.COURSEs on subb.CRS_ID equals c.ID into gj
                            from subc in gj.DefaultIfEmpty()
                            join ct in db.COUNTRies on st.NTLTY_ID equals ct.ID into gk
                            from subct in gk.DefaultIfEmpty()
                            join cat in db.STUDENT_CATGEORY on st.STDNT_CAT_ID equals cat.ID into gl
                            from subcat in gl.DefaultIfEmpty()
                            join emp in db.EMPLOYEEs on subb.EMP_ID equals emp.ID into gm
                            from subemp in gm.DefaultIfEmpty()
                            join img in db.IMAGE_DOCUMENTS on st.PHTO_DATA equals img.DOCUMENTID into gn
                            from subimg in gn.DefaultIfEmpty()
                            where st.ID == id
                            orderby st.LAST_NAME, subb.NAME
                            select new Student { StudentData = st, BatcheData = (subb == null ? null : subb), CourseData = (subc == null ? null : subc), CountryData = (subct == null ? null : subct), CategoryData = (subcat == null ? null : subcat), EmployeeData = (subemp == null ? null : subemp), ImageData = (subimg == null ? null : subimg) }).Distinct();

            return View(StudentS.FirstOrDefault());

        }

        // GET: Student
        public ActionResult AdvancedSearch(string sortOrder, string currentFilter, string searchString, int? page, string currentFilter2, string AdmissionNumber, string currentFilter3, string HadPdFees, int? currentFilter4, int? CourseBatches, string currentFilter5, string Category, string currentFilter6, string StudentGender, string currentFilter7, string BloodGroup, string currentFilter8, string StudentGrade, string currentFilter9, string StudentBirthFromDate, string currentFilter10, string StudentBirthToDate, string currentFilter11, string ActiveStudent, string currentFilter12, string MissingDetl)
        {
            DateTime NullDate = DateTime.Parse("31-12-9999");
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.NameSortParm2 = sortOrder == "FName" ? "name_desc_2" : "FName";
            ViewBag.NameSortParm3 = sortOrder == "RNum" ? "name_desc_3" : "RNum";
            ViewBag.NameSortParm4 = sortOrder == "AdNum" ? "name_desc_4" : "AdNum";
            ViewBag.NameSortParm5 = sortOrder == "DOB" ? "name_desc_5" : "DOB";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (!string.IsNullOrEmpty(searchString)) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            if (!string.IsNullOrEmpty(AdmissionNumber)) { page = 1; }
            else { AdmissionNumber = currentFilter2; }
            ViewBag.CurrentFilter2 = AdmissionNumber;
            if (!string.IsNullOrEmpty(HadPdFees)) { page = 1; }
            else { HadPdFees = currentFilter3; }
            ViewBag.CurrentFilter3 = HadPdFees;
            if (!CourseBatches.Equals(null)) { page = 1; }
            else { CourseBatches = currentFilter4; }
            ViewBag.CurrentFilter4 = CourseBatches;
            if (!string.IsNullOrEmpty(Category)) { page = 1; }
            else { Category = currentFilter5; }
            ViewBag.CurrentFilter5 = Category;
            if (!string.IsNullOrEmpty(StudentGender)) { page = 1; }
            else { StudentGender = currentFilter6; }
            ViewBag.CurrentFilter6 = StudentGender;
            if (!string.IsNullOrEmpty(BloodGroup)) { page = 1; }
            else { BloodGroup = currentFilter7; }
            ViewBag.CurrentFilter7 = BloodGroup;
            if (!string.IsNullOrEmpty(StudentGrade)) { page = 1; }
            else { StudentGrade = currentFilter8; }
            ViewBag.CurrentFilter8 = StudentGrade;
            if (!string.IsNullOrEmpty(StudentBirthFromDate))
            {
                page = 1;
            }
            else { StudentBirthFromDate = currentFilter9; }
            DateTime? dFrom; DateTime dtFrom;
            dFrom = DateTime.TryParse(StudentBirthFromDate, out dtFrom) ? dtFrom : (DateTime?)null;
            ViewBag.CurrentFilter9 = StudentBirthFromDate;
            if (!string.IsNullOrEmpty(StudentBirthToDate))
            {
                page = 1;
            }
            else { StudentBirthToDate = currentFilter10; }
            DateTime? dTo; DateTime dtTo;
            dTo = DateTime.TryParse(StudentBirthToDate, out dtTo) ? dtTo : (DateTime?)null;
            ViewBag.CurrentFilter10 = StudentBirthToDate;
            if (!string.IsNullOrEmpty(ActiveStudent)) { page = 1; }
            else { ActiveStudent = "Y"; };
            ViewBag.CurrentFilter11 = ActiveStudent;
            if (!string.IsNullOrEmpty(MissingDetl)) { page = 1; }
            else { MissingDetl = currentFilter12; }
            ViewBag.CurrentFilter12 = MissingDetl;

            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID into gi
                            from subb in gi.DefaultIfEmpty()
                            join c in db.COURSEs on subb.CRS_ID equals c.ID into gj
                            from subc in gj.DefaultIfEmpty()
                            join ct in db.COUNTRies on st.NTLTY_ID equals ct.ID into gk
                            from subct in gk.DefaultIfEmpty()
                            join cat in db.STUDENT_CATGEORY on st.STDNT_CAT_ID equals cat.ID into gl
                            from subcat in gl.DefaultIfEmpty()
                            join emp in db.EMPLOYEEs on subb.EMP_ID equals emp.ID into gm
                            from subemp in gm.DefaultIfEmpty()
                            join esc in db.EXAM_SCORE on st.ID equals esc.STDNT_ID into gn
                            from subesc in gn.DefaultIfEmpty()
                            join grl in db.GRADING_LEVEL on subesc.GRADING_LVL_ID equals grl.ID into go
                            from subgrl in go.DefaultIfEmpty()
                            orderby st.LAST_NAME, subb.NAME
                            select new Student { StudentData = st, BatcheData = (subb == null ? null : subb), CourseData = (subc == null ? null : subc), CountryData = (subct == null ? null : subct), CategoryData = (subcat == null ? null : subcat), EmployeeData = (subemp == null ? null : subemp), GradeData = (subgrl == null ? null : subgrl) }).Distinct();
            var StudentGuardians = (from st in db.STUDENTs
                                    join gd in db.GUARDIANs on st.ID equals gd.WARD_ID
                                    where st.IS_DEL == false
                                    select new StudentsGuardians { StudentData = st, GuardianData = gd }).OrderBy(x => x.StudentData.ID).Distinct();
            ViewData["guardians"] = StudentGuardians;

            if (!String.IsNullOrEmpty(searchString))
            {
                StudentS = StudentS.Where(s => s.StudentData.LAST_NAME.Contains(searchString)
                                       || s.StudentData.FIRST_NAME.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(AdmissionNumber))
            {
                StudentS = StudentS.Where(s => s.StudentData.ADMSN_NO.Equals(AdmissionNumber));
            }
            if (!String.IsNullOrEmpty(HadPdFees))
            {
                if(HadPdFees == "N")
                {
                    StudentS = StudentS.Where(s => s.StudentData.HAS_PD_FE==false);
                }
                else
                {
                    StudentS = StudentS.Where(s => s.StudentData.HAS_PD_FE == true);
                }
            }
            if (!CourseBatches.Equals(null))
            {
                StudentS = StudentS.Where(s => s.BatcheData.ID == CourseBatches);
            }
            if (!String.IsNullOrEmpty(Category))
            {
                StudentS = StudentS.Where(s => s.CategoryData.NAME.Contains(Category));
            }
            if (!String.IsNullOrEmpty(StudentGender) && !StudentGender.Contains("All"))
            {
                StudentS = StudentS.Where(s => s.StudentData.GNDR.Equals(StudentGender));
            }
            if (!String.IsNullOrEmpty(BloodGroup))
            {
                StudentS = StudentS.Where(s => s.StudentData.BLOOD_GRP.Equals(BloodGroup));
            }
            if (!String.IsNullOrEmpty(StudentGrade))
            {
                StudentS = StudentS.Where(s => s.GradeData.NAME.Equals(StudentGrade));
            }
            if (!String.IsNullOrEmpty(StudentBirthFromDate) && !String.IsNullOrEmpty(StudentBirthToDate))
            {
                StudentS = StudentS.Where(s => s.StudentData.DOB >= dFrom).Where(s => s.StudentData.DOB <= dTo);
            }
            if (!String.IsNullOrEmpty(ActiveStudent))
            {
                StudentS = StudentS.Where(s => s.StudentData.IS_ACT.Equals(ActiveStudent == "Y"? true: false));
            }
            if (!String.IsNullOrEmpty(MissingDetl))
            {
                DateTime firstDayCurAceYear = System.DateTime.Now;
                if (System.DateTime.Now.Month <= 3)
                {
                    firstDayCurAceYear = new DateTime(System.DateTime.Now.Year - 1, 4, 1);
                }
                else
                {
                    firstDayCurAceYear = new DateTime(System.DateTime.Now.Year, 4, 1);
                }
                switch (MissingDetl)
                {
                    case "DateOfBirth":
                        StudentS = StudentS.Where(s => s.StudentData.DOB.Equals(null) || s.StudentData.DOB == NullDate);
                        break;
                    case "PhoneNumber":
                        StudentS = StudentS.Where(s => s.StudentData.PH1.Equals(null) && s.StudentData.PH2.Equals(null));
                        break;
                    case "ParentDetails":
                        StudentS = StudentS.Where(s => s.GuardianData.ID.Equals(null));
                        break;
                    case "StundetsPicture":
                        StudentS = StudentS.Where(s => s.StudentData.IMAGE_DOCUMENTS_ID.Equals(null));
                        break;
                    case "SchoolBook":
                        StudentS = StudentS.Where(s => s.StudentData.BOOK_PURCHAGED.Equals(null) || s.StudentData.BOOK_PURCHAGED== false || s.StudentData.BOOK_PUR_DT < firstDayCurAceYear);
                        break;
                    case "SchoolDress":
                        StudentS = StudentS.Where(s => s.StudentData.DRESS_PURCHAGED.Equals(null) || s.StudentData.DRESS_PURCHAGED == false || s.StudentData.DRESS_PUR_DT < firstDayCurAceYear);
                        break;
                }
            }
            switch (sortOrder)
            {
                case "name_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.LAST_NAME);
                    break;
                case "Date":
                    StudentS = StudentS.OrderBy(s => s.StudentData.ADMSN_DATE);
                    break;
                case "date_desc":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.ADMSN_DATE);
                    break;
                case "FName":
                    StudentS = StudentS.OrderBy(s => s.StudentData.FIRST_NAME);
                    break;
                case "name_desc_2":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.FIRST_NAME);
                    break;
                case "RNum":
                    StudentS = StudentS.OrderBy(s => s.StudentData.CLS_ROLL_NO);
                    break;
                case "name_desc_3":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.CLS_ROLL_NO);
                    break;
                case "AdNum":
                    StudentS = StudentS.OrderBy(s => s.StudentData.ADMSN_NO);
                    break;
                case "name_desc_4":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.ADMSN_NO);
                    break;
                case "DOB":
                    StudentS = StudentS.OrderBy(s => s.StudentData.DOB);
                    break;
                case "name_desc_5":
                    StudentS = StudentS.OrderByDescending(s => s.StudentData.DOB);
                    break;
                default:  // Name ascending 
                    StudentS = StudentS.OrderBy(s => s.StudentData.LAST_NAME);
                    break;
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
                result.Selected = item.BatchData.ID == CourseBatches ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = null, Text = "ALL" });
            ViewBag.CourseBatches = options;

            List<SelectListItem> options2 = new SelectList(db.STUDENTs, "HAS_PD_FE", "HAS_PD_FE").Distinct().ToList();
            // add the 'ALL' option
            options2.Insert(0, new SelectListItem() { Value = null, Text = "Select Paid Fees Satus" });
            ViewBag.HadPdFees = options2;



            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(StudentS.ToPagedList(pageNumber, pageSize));
            //return View(db.USERS.ToList());
        }

        // GET: Student
        [HttpGet]
        public ActionResult AdvancedSearchPdf(string searchString, string AdmissionNumber, string HadPdFees, int? CourseBatches, string Category, string StudentGender, string BloodGroup, string StudentGrade, string StudentBirthFromDate, string StudentBirthToDate, string ActiveStudent, string MissingDetl)
        {
            DateTime NullDate = DateTime.Parse("31-12-9999");
            DateTime? dFrom; DateTime dtFrom;
            dFrom = DateTime.TryParse(StudentBirthFromDate, out dtFrom) ? dtFrom : (DateTime?)null;
            DateTime? dTo; DateTime dtTo;
            dTo = DateTime.TryParse(StudentBirthToDate, out dtTo) ? dtTo : (DateTime?)null;

            var StudentS = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID into gi
                            from subb in gi.DefaultIfEmpty()
                            join c in db.COURSEs on subb.CRS_ID equals c.ID into gj
                            from subc in gj.DefaultIfEmpty()
                            join ct in db.COUNTRies on st.NTLTY_ID equals ct.ID into gk
                            from subct in gk.DefaultIfEmpty()
                            join cat in db.STUDENT_CATGEORY on st.STDNT_CAT_ID equals cat.ID into gl
                            from subcat in gl.DefaultIfEmpty()
                            join emp in db.EMPLOYEEs on subb.EMP_ID equals emp.ID into gm
                            from subemp in gm.DefaultIfEmpty()
                            join esc in db.EXAM_SCORE on st.ID equals esc.STDNT_ID into gn
                            from subesc in gn.DefaultIfEmpty()
                            join grl in db.GRADING_LEVEL on subesc.GRADING_LVL_ID equals grl.ID into go
                            from subgrl in go.DefaultIfEmpty()
                            join grd in db.GUARDIANs on st.ID equals grd.WARD_ID into gd
                            from subgrd in gd.DefaultIfEmpty()
                            orderby st.LAST_NAME, subb.NAME
                            select new Student { StudentData = st, BatcheData = (subb == null ? null : subb), CourseData = (subc == null ? null : subc), CountryData = (subct == null ? null : subct), CategoryData = (subcat == null ? null : subcat), EmployeeData = (subemp == null ? null : subemp), GradeData = (subgrl == null ? null : subgrl), GuardianData = (subgrd == null ? null : subgrd) }).Distinct();

            if (!String.IsNullOrEmpty(searchString))
            {
                StudentS = StudentS.Where(s => s.StudentData.LAST_NAME.Contains(searchString)
                                       || s.StudentData.FIRST_NAME.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(AdmissionNumber))
            {
                StudentS = StudentS.Where(s => s.StudentData.ADMSN_NO.Equals(AdmissionNumber));
            }
            if (!String.IsNullOrEmpty(HadPdFees))
            {
                if(HadPdFees == "N")
                {
                    StudentS = StudentS.Where(s => s.StudentData.HAS_PD_FE==false);
                }
                else
                {
                    StudentS = StudentS.Where(s => s.StudentData.HAS_PD_FE==true);
                }
            }
            if (!CourseBatches.Equals(null))
            {
                StudentS = StudentS.Where(s => s.BatcheData.ID == CourseBatches);
            }
            if (!String.IsNullOrEmpty(Category))
            {
                StudentS = StudentS.Where(s => s.CategoryData.NAME.Contains(Category));
            }
            if (!String.IsNullOrEmpty(StudentGender) && !StudentGender.Contains("All"))
            {
                StudentS = StudentS.Where(s => s.StudentData.GNDR.Equals(StudentGender));
            }
            if (!String.IsNullOrEmpty(BloodGroup))
            {
                StudentS = StudentS.Where(s => s.StudentData.BLOOD_GRP.Equals(BloodGroup));
            }
            if (!String.IsNullOrEmpty(StudentGrade))
            {
                StudentS = StudentS.Where(s => s.GradeData.NAME.Equals(StudentGrade));
            }
            if (!String.IsNullOrEmpty(StudentBirthFromDate) && !String.IsNullOrEmpty(StudentBirthToDate))
            {
                StudentS = StudentS.Where(s => s.StudentData.DOB >= dFrom).Where(s => s.StudentData.DOB <= dTo);
            }
            if (!String.IsNullOrEmpty(ActiveStudent))
            {
                StudentS = StudentS.Where(s => s.StudentData.IS_ACT.Equals(ActiveStudent == "Y"? true : false));
            }
            if (!String.IsNullOrEmpty(MissingDetl))
            {
                switch (MissingDetl)
                {
                    case "DateOfBirth":
                        StudentS = StudentS.Where(s => s.StudentData.DOB.Equals(null) || s.StudentData.DOB == NullDate);
                        break;
                    case "PhoneNumber":
                        StudentS = StudentS.Where(s => s.StudentData.PH1.Equals(null) && s.StudentData.PH2.Equals(null));
                        break;
                    case "ParentDetails":
                        StudentS = StudentS.Where(s => s.GuardianData.ID.Equals(null));
                        break;
                    case "StundetsPicture":
                        StudentS = StudentS.Where(s => s.StudentData.IMAGE_DOCUMENTS_ID.Equals(null));
                        break;
                }
            }

            return View(StudentS);

        }

        // GET: Student/Edit/5
        public ActionResult ActivateStudent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTs.Find(id);
            if (sTUDENT == null)
            {
                return HttpNotFound();
            }
            sTUDENT.IS_ACT = true;
            sTUDENT.IS_DEL = false;
            sTUDENT.UPDATED_AT = System.DateTime.Now;
            db.Entry(sTUDENT).State = EntityState.Modified;
            try { db.SaveChanges(); }
            catch (Exception ex)
            {
                throw ex; // Oops, something went wrong, handle the exception
            }

            return RedirectToAction("Index");
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTs.Find(id);
            if (sTUDENT == null)
            {
                return HttpNotFound();
            }
            return View(sTUDENT);
        }

        //ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME");
        // GET: Student/Admission1
        public ActionResult Admission1()
        {
            DateTime PDate = Convert.ToDateTime(System.DateTime.Now);
            ViewBag.ReturnDate = PDate.ToShortDateString();
            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", "99");
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies.Where(o => o.NTLTY != " ").ToList(), "ID", "NTLTY", "99");
            ViewBag.STDNT_CAT_ID = new SelectList(db.STUDENT_CATGEORY, "ID", "NAME");
            ///Code to get the Batch along weith Course
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
                options.Add(result);
            }

            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Course and Batch" });
            ViewBag.BTCH_ID = options;
            //End of Code to get batch and Course

            var configValue = (from C in db.CONFIGURATIONs
                               where C.CONFIG_KEY == "AdmissionNumberAutoIncrement"
                               select new { CONFIG_VALUE = C.CONFIG_VAL }).FirstOrDefault();
            int NewAdmissionNumberNum = Convert.ToInt32(configValue.CONFIG_VALUE.ToString()) + 1;
            ViewBag.NewAdmissionNumber = NewAdmissionNumberNum.ToString();
            return View();
        }

        // POST: Student/Admission1
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Admission1([Bind(Include = "ID,ADMSN_NO,CLS_ROLL_NO,ADMSN_DATE,FIRST_NAME,MID_NAME,LAST_NAME,BTCH_ID,DOB,GNDR,BLOOD_GRP,BIRTH_PLACE,LANG,RLGN,ADDR_LINE1,ADDR_LINE2,CITY,STATE,PIN_CODE,CTRY_ID,PH1,PH2,EML,IMMDT_CNTCT_ID,IS_SMS_ENABL,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,STAT_DESCR,IS_ACT,IS_DEL,CREATED_AT,UPDATED_AT,HAS_PD_FE,PHTO_FILE_SIZE,USRID,STDNT_CAT_ID,NTLTY_ID,IMAGE_DOCUMENTS_ID")] STUDENT sTUDENT)
        {
            int UserId = Convert.ToInt32(this.Session["UserId"]);
            sTUDENT.USRID = UserId;
            ViewBag.NewAdmissionNumber = sTUDENT.ADMSN_NO;
            DateTime PDate = Convert.ToDateTime(sTUDENT.ADMSN_DATE);
            ViewBag.ReturnDate = PDate.ToShortDateString();
            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", sTUDENT.CTRY_ID);
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies.Where(o => o.NTLTY != " ").ToList(), "ID", "NTLTY", sTUDENT.NTLTY_ID);
            ViewBag.STDNT_CAT_ID = new SelectList(db.STUDENT_CATGEORY, "ID", "NAME", sTUDENT.STDNT_CAT_ID);

            ///Code to get the Batch along weith Course
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.IS_DEL == false && bt.IS_ACT == true
                                    select new { CourseData = cs, BatchData = bt })
                                    .OrderBy(x => x.BatchData.ID).ToList();


            List<SelectListItem> options = new List<SelectListItem>();
            foreach (var item in queryCourceBatch)
            {
                string BatchFullName = string.Concat(item.CourseData.CODE, "-", item.BatchData.NAME);
                var result1 = new SelectListItem();
                result1.Text = BatchFullName;
                result1.Value = item.BatchData.ID.ToString();
                result1.Selected = item.BatchData.ID == sTUDENT.BTCH_ID ? true : false;
                options.Add(result1);
            }

            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Course and Batch" });
            ViewBag.BTCH_ID = options;
            //End of Code to get batch and Course

            var configValue = (from C in db.CONFIGURATIONs
                               where C.CONFIG_KEY == "AdmissionNumberAutoIncrement"
                               select new { CONFIG_VALUE = C.CONFIG_VAL }).FirstOrDefault();
            int NewAdmissionNumberNum = Convert.ToInt32(configValue.CONFIG_VALUE.ToString()) + 1;
            ViewBag.NewAdmissionNumber = NewAdmissionNumberNum.ToString();

            if (ModelState.IsValid)
            {
                /////Picture Upload Code
                string FileName = null;
                SuccessModel viewModel = new SuccessModel();
                if (Request.Files.Count == 1 && Request.Files[0].FileName != "")
                {
                    var name = Request.Files[0].FileName;
                    var size = Request.Files[0].ContentLength;
                    var type = Request.Files[0].ContentType;
                    FileName = name;
                    int PhotoId = 0;
                    PhotoId = HandleUpload(Request.Files[0].InputStream, name, size, type, PhotoId);

                    sTUDENT.IMAGE_DOCUMENTS_ID = PhotoId;
                    sTUDENT.PHTO_FILENAME = null;
                }
                ////End to Picture Upload Code

                sTUDENT.IS_ACT = true;
                sTUDENT.IS_DEL = false;
                sTUDENT.HAS_PD_FE = false;
                sTUDENT.CREATED_AT = System.DateTime.Now;
                sTUDENT.UPDATED_AT = System.DateTime.Now;
                db.STUDENTs.Add(sTUDENT);
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
                    return View(sTUDENT);
                }
                catch (Exception e) {                   
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(sTUDENT);
                }

                var result = from u in db.CONFIGURATIONs where (u.CONFIG_KEY == "AdmissionNumberAutoIncrement") select u;
                if (result.Count() != 0)
                {
                    var dbConfig = result.First();

                    dbConfig.CONFIG_VAL = sTUDENT.ADMSN_NO;
                    db.SaveChanges();
                }

                string FullName = Regex.Replace(string.Concat(sTUDENT.FIRST_NAME, sTUDENT.LAST_NAME, Random_String(5)), @"\s", "");
                var StdUser = new USER() { USRNAME = FullName, FIRST_NAME = sTUDENT.FIRST_NAME, LAST_NAME = sTUDENT.LAST_NAME, EML = sTUDENT.EML, ROLE = "Student" };
                db.USERS.Add(StdUser);
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(sTUDENT);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(sTUDENT);
                }

                STUDENT StdResult = db.STUDENTs.Find(sTUDENT.ID);
                StdResult.USRID = StdUser.ID;

                //Find all available Fee Collection which are active.
                var FeeCollectionSet = (from ffc in db.FINANCE_FEE_CATGEORY
                                 join b in db.BATCHes on ffc.BTCH_ID equals b.ID
                                 join st in db.STUDENTs on b.ID equals st.BTCH_ID
                                 join fcol in db.FINANCE_FEE_COLLECTION on new { A = ffc.ID.ToString(), B = b.ID.ToString() } equals new { A = fcol.FEE_CAT_ID.ToString(), B = fcol.BTCH_ID.ToString() }
                                 where st.ID == sTUDENT.ID && ffc.IS_DEL.Equals(false) && b.IS_DEL == false && st.IS_DEL == false && fcol.START_DATE <= st.ADMSN_DATE && fcol.END_DATE >= st.ADMSN_DATE
                                        select new { FinanceFeeCategoryData = ffc, BatchData = b, StudentData = st, FeeCollectionData = fcol }).OrderBy(g => g.FinanceFeeCategoryData.ID).ToList();
                foreach (var item2 in FeeCollectionSet)
                {
                    STUDENT FeeStudent = db.STUDENTs.Find(item2.StudentData.ID);
                    FeeStudent.HAS_PD_FE = false;
                    FeeStudent.UPDATED_AT = System.DateTime.Now;
                    var FF_fEE = new FINANCE_FEE() { STDNT_ID = item2.StudentData.ID, FEE_CLCT_ID = item2.FeeCollectionData.ID, IS_PD = false };
                    db.FINANCE_FEE.Add(FF_fEE);
                }

                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage); } }
                    return View(sTUDENT);
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return View(sTUDENT);
                }
                // some code 
                TempData["alertMessage"] = string.Concat("Student Admission Done. Website User ID :", StdUser.USRNAME, "     Password :", StdUser.HASHED_PSWRD);
                return RedirectToAction("Admission2", "Student", new { Std_id = sTUDENT.ID });
            }

            return View(sTUDENT);
        }

        //ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME");
        // GET: Student/Admission1
        public ActionResult Admission2(int? Std_id)
        {
            ViewBag.ReturnDate = System.DateTime.Now;
            STUDENT sTUDENT = db.STUDENTs.Find(Std_id);
            ViewBag.StudentFullName = String.Format("{0} {1}", sTUDENT.FIRST_NAME.ToString(), sTUDENT.LAST_NAME.ToString());
            ViewBag.NewAdmissionNumber = sTUDENT.ADMSN_NO.ToString();
            if (!String.IsNullOrEmpty(sTUDENT.ADDR_LINE1))
            {
                ViewBag.AddressLine1 = sTUDENT.ADDR_LINE1.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.ADDR_LINE2))
            {
                ViewBag.AddressLine2 = sTUDENT.ADDR_LINE2.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.CITY))
            {
                ViewBag.StudentCity = sTUDENT.CITY.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.STATE))
            {
                ViewBag.StudentState = sTUDENT.STATE.ToString();
            }
            if (!sTUDENT.COUNTRY.Equals(null))
            {
                ViewBag.CountryName = sTUDENT.COUNTRY.ToString();
            }
            ViewBag.StudentId = Std_id;
            //COUNTRY cOUNTRY = db.COUNTRies.Find(sTUDENT.CTRY_ID);
            ViewBag.CountryName = sTUDENT.CTRY_ID;
            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", "99");
            DataTable dtGuardian = new DataTable();
            dtGuardian.Columns.Add("FIRST_NAME", typeof(string));
            dtGuardian.Columns.Add("LAST_NAME", typeof(string));
            dtGuardian.Columns.Add("REL", typeof(string));
            dtGuardian.Columns.Add("MOB", typeof(string));
            var GuardianVal = (from C in db.GUARDIANs
                               where C.WARD_ID == Std_id
                               select new { f_name = C.FIRST_NAME, l_name = C.LAST_NAME, rel = C.REL, mob = C.MOBL_PH }).ToList();

            foreach (var entity in GuardianVal.ToList())
            {
                var row = dtGuardian.NewRow();
                row["FIRST_NAME"] = entity.f_name;
                row["LAST_NAME"] = entity.l_name;
                row["REL"] = entity.rel;
                row["MOB"] = entity.mob;
                dtGuardian.Rows.Add(row);
            }
            ViewBag.Data = dtGuardian.AsEnumerable();
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Admission2([Bind(Include = "ID,WARD_ID,FIRST_NAME,LAST_NAME,REL,EML,OFF_PH1,OFF_PH2,MOBL_PH,OFF_ADDR_LINE1,OFF_ADDR_LINE2,CITY,STATE,CTRY_ID,DOB,OCCP,INCM,ED,CREATED_AT,UPDATED_AT,USRID")] GUARDIAN gUARDIAN)
        {
            if (ModelState.IsValid)
            {
                gUARDIAN.CREATED_AT = System.DateTime.Now;
                gUARDIAN.UPDATED_AT = System.DateTime.Now;
                db.GUARDIANs.Add(gUARDIAN);
                db.SaveChanges();
                return RedirectToAction("Admission2", "Student", new { Std_id = gUARDIAN.WARD_ID });
            }

            ViewBag.ReturnDate = System.DateTime.Now;
            STUDENT sTUDENT = db.STUDENTs.Find(gUARDIAN.WARD_ID);
            ViewBag.StudentFullName = String.Format("{0} {1} {2}", sTUDENT.FIRST_NAME.ToString(), sTUDENT.MID_NAME.ToString(), sTUDENT.LAST_NAME.ToString());
            ViewBag.NewAdmissionNumber = sTUDENT.ADMSN_NO.ToString();
            ViewBag.AddressLine1 = sTUDENT.ADDR_LINE1.ToString();
            ViewBag.AddressLine2 = sTUDENT.ADDR_LINE2.ToString();
            ViewBag.StudentCity = sTUDENT.CITY.ToString();
            ViewBag.StudentState = sTUDENT.STATE.ToString();
            ViewBag.StudentCountry = sTUDENT.COUNTRY.ToString();
            ViewBag.StudentId = gUARDIAN.WARD_ID;
            COUNTRY cOUNTRY = db.COUNTRies.Find(sTUDENT.CTRY_ID);
            ViewBag.CountryName = cOUNTRY.CTRY_NAME;
            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME");
            DataTable dtGuardian = new DataTable();
            dtGuardian.Columns.Add("FIRST_NAME", typeof(int));
            dtGuardian.Columns.Add("LAST_NAME", typeof(string));
            dtGuardian.Columns.Add("REL", typeof(string));
            var GuardianVal = (from C in db.GUARDIANs
                               where C.WARD_ID == gUARDIAN.WARD_ID
                               select new { f_name = C.FIRST_NAME, l_name = C.LAST_NAME, rel = C.REL }).ToList();

            foreach (var entity in GuardianVal.ToList())
            {
                var row = dtGuardian.NewRow();
                row["FIRST_NAME"] = entity.f_name;
                row["LAST_NAME"] = entity.l_name;
                row["REL"] = entity.rel;
                dtGuardian.Rows.Add(row);
            }
            ViewBag.Data = dtGuardian.AsEnumerable();
            return View();
        }


        // GET: Student/Admission3
        public ActionResult Admission3(int? Std_id)
        {
            //List<SelectGuardian> Guardian = new List<SelectGuardian>();

            var GuardianVal = (from C in db.GUARDIANs
                               where C.WARD_ID == Std_id
                               select new SelectGuardian { GuardianList = C }).ToList();
            if (GuardianVal.Count().Equals(0))
            {
                ViewBag.ErrorMessage = "Any guardian must be added to proceed forward.";
            }
            return View(GuardianVal);

        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: Student/Admission3
        public ActionResult Admission3(IList<SelectGuardian> model)
        {
            foreach (SelectGuardian item in model)
            {
                if (item.Selected)
                {
                    STUDENT sTUDENT = db.STUDENTs.Find(item.GuardianList.WARD_ID);
                    sTUDENT.IMMDT_CNTCT_ID = item.GuardianList.ID;
                    //db.SaveChanges();
                    try { db.SaveChanges(); ViewBag.ErrorMessage = string.Concat("Guardian Details added successfully for Studnet Id:", item.GuardianList.WARD_ID); }
                    catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; return View(model); }
                    break;
                }
            }
            return RedirectToAction("Profiles", "Student", new { id = model.FirstOrDefault().GuardianList.WARD_ID });
            //return View(model);

        }

        // GET: Student/Create
        public ActionResult Create()
        {
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME");
            ViewBag.STDNT_CAT_ID = new SelectList(db.STUDENT_CATGEORY, "ID", "NAME");
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME");
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ADMSN_NO,CLS_ROLL_NO,ADMSN_DATE,FIRST_NAME,MID_NAME,LAST_NAME,BTCH_ID,DOB,GNDR,BLOOD_GRP,BIRTH_PLACE,LANG,RLGN,ADDR_LINE1,ADDR_LINE2,CITY,STATE,PIN_CODE,CTRY_ID,PH1,PH2,EML,IMMDT_CNTCT_ID,IS_SMS_ENABL,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,STAT_DESCR,IS_ACT,IS_DEL,CREATED_AT,UPDATED_AT,HAS_PD_FE,PHTO_FILE_SIZE,USRID,STDNT_CAT_ID,NTLTY_ID")] STUDENT sTUDENT)
        {
            if (ModelState.IsValid)
            {
                sTUDENT.IS_ACT = true;
                sTUDENT.IS_DEL = false;
                sTUDENT.CREATED_AT = System.DateTime.Now;
                sTUDENT.UPDATED_AT = System.DateTime.Now;
                db.STUDENTs.Add(sTUDENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", sTUDENT.NTLTY_ID);
            ViewBag.STDNT_CAT_ID = new SelectList(db.STUDENT_CATGEORY, "ID", "NAME", sTUDENT.STDNT_CAT_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", sTUDENT.USRID);
            return View(sTUDENT);
        }

        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTs.Find(id);
            if (sTUDENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", sTUDENT.NTLTY_ID);
            ViewBag.STDNT_CAT_ID = new SelectList(db.STUDENT_CATGEORY, "ID", "NAME", sTUDENT.STDNT_CAT_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", sTUDENT.USRID);
            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", sTUDENT.CTRY_ID);

            ///Code to get the Batch along weith Course
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
                result.Selected = item.BatchData.ID == sTUDENT.BTCH_ID ? true : false;
                options.Add(result);
            }

            options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Course and Batch" });
            ViewBag.BTCH_ID_VAL = options;

            ViewBag.BTCH_ID = sTUDENT.BTCH_ID;
            //End of Code to get batch and Course
            return View(sTUDENT);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ADMSN_NO,CLS_ROLL_NO,ADMSN_DATE,FIRST_NAME,MID_NAME,LAST_NAME,BTCH_ID,DOB,GNDR,BLOOD_GRP,BIRTH_PLACE,LANG,RLGN,ADDR_LINE1,ADDR_LINE2,CITY,STATE,PIN_CODE,CTRY_ID,PH1,PH2,EML,IMMDT_CNTCT_ID,IS_SMS_ENABL,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,STAT_DESCR,IS_ACT,IS_DEL,CREATED_AT,UPDATED_AT,HAS_PD_FE,PHTO_FILE_SIZE,USRID,STDNT_CAT_ID,NTLTY_ID,IMAGE_DOCUMENTS_ID")] STUDENT sTUDENT)
        {
            if (ModelState.IsValid)
            {

                STUDENT sTUDENTtOuPDATE = db.STUDENTs.Find(sTUDENT.ID);

                /////Picture Upload Code
                string FileName = null;
                SuccessModel viewModel = new SuccessModel();
                if (Request.Files.Count == 1 && Request.Files[0].FileName != "")
                {
                    var name = Request.Files[0].FileName;
                    var size = Request.Files[0].ContentLength;
                    var type = Request.Files[0].ContentType;
                    FileName = name;
                    sTUDENTtOuPDATE.IMAGE_DOCUMENTS_ID = HandleUpload(Request.Files[0].InputStream, name, size, type, Convert.ToInt32(sTUDENT.IMAGE_DOCUMENTS_ID));
                }
                ////End to Picture Upload Code
                sTUDENTtOuPDATE.ADMSN_NO = sTUDENT.ADMSN_NO;
                sTUDENTtOuPDATE.ADMSN_DATE = sTUDENT.ADMSN_DATE;
                sTUDENTtOuPDATE.FIRST_NAME = sTUDENT.FIRST_NAME;
                sTUDENTtOuPDATE.MID_NAME = sTUDENT.MID_NAME;
                sTUDENTtOuPDATE.LAST_NAME = sTUDENT.LAST_NAME;
                sTUDENTtOuPDATE.BTCH_ID = sTUDENT.BTCH_ID;
                sTUDENTtOuPDATE.DOB = sTUDENT.DOB;
                sTUDENTtOuPDATE.GNDR = sTUDENT.GNDR;
                sTUDENTtOuPDATE.BLOOD_GRP = sTUDENT.BLOOD_GRP;
                sTUDENTtOuPDATE.BIRTH_PLACE = sTUDENT.BIRTH_PLACE;
                sTUDENTtOuPDATE.NTLTY_ID = sTUDENT.NTLTY_ID;
                sTUDENTtOuPDATE.LANG = sTUDENT.LANG;
                sTUDENTtOuPDATE.STDNT_CAT_ID = sTUDENT.STDNT_CAT_ID;
                sTUDENTtOuPDATE.RLGN = sTUDENT.RLGN;
                sTUDENTtOuPDATE.ADDR_LINE1 = sTUDENT.ADDR_LINE1;
                sTUDENTtOuPDATE.ADDR_LINE2 = sTUDENT.ADDR_LINE2;
                sTUDENTtOuPDATE.CITY = sTUDENT.CITY;
                sTUDENTtOuPDATE.STATE = sTUDENT.STATE;
                sTUDENTtOuPDATE.PIN_CODE = sTUDENT.PIN_CODE;
                sTUDENTtOuPDATE.CTRY_ID = sTUDENT.CTRY_ID;
                sTUDENTtOuPDATE.PH1 = sTUDENT.PH1;
                sTUDENTtOuPDATE.PH2 = sTUDENT.PH2;
                sTUDENTtOuPDATE.EML = sTUDENT.EML;
                sTUDENTtOuPDATE.CLS_ROLL_NO = sTUDENT.CLS_ROLL_NO;
                sTUDENTtOuPDATE.USRID = sTUDENT.USRID;
                sTUDENTtOuPDATE.IS_SMS_ENABL = sTUDENT.IS_SMS_ENABL;

                sTUDENTtOuPDATE.UPDATED_AT = System.DateTime.Now;
                db.Entry(sTUDENTtOuPDATE).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.ErrorMessage = "Student's Details updated successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }

                ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", sTUDENTtOuPDATE.NTLTY_ID);
                ViewBag.STDNT_CAT_ID = new SelectList(db.STUDENT_CATGEORY, "ID", "NAME", sTUDENTtOuPDATE.STDNT_CAT_ID);
                ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", sTUDENTtOuPDATE.USRID);
                ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", sTUDENTtOuPDATE.CTRY_ID);

                ///Code to get the Batch along weith Course
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
                    result.Selected = item.BatchData.ID == sTUDENTtOuPDATE.BTCH_ID ? true : false;
                    options.Add(result);
                }

                options.Insert(0, new SelectListItem() { Value = "-1", Text = "Select Course and Batch" });
                ViewBag.BTCH_ID_VAL = options;

                ViewBag.BTCH_ID = sTUDENT.BTCH_ID;
                return View(sTUDENTtOuPDATE);
            }
            ViewBag.NTLTY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", sTUDENT.NTLTY_ID);
            ViewBag.STDNT_CAT_ID = new SelectList(db.STUDENT_CATGEORY, "ID", "NAME", sTUDENT.STDNT_CAT_ID);
            ViewBag.USRID = new SelectList(db.USERS, "ID", "USRNAME", sTUDENT.USRID);
            return View(sTUDENT);
        }
        public ActionResult Remove(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTs.Find(id);
            if (sTUDENT == null)
            {
                return HttpNotFound();
            }
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where bt.ID == sTUDENT.BTCH_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                    .OrderBy(x => x.BatchData.ID).ToList();
            ViewBag.BTCH_ID = string.Concat(queryCourceBatch.FirstOrDefault().CourseData.CODE, "-", queryCourceBatch.FirstOrDefault().BatchData.NAME);
            return View(sTUDENT);
        }

        public ActionResult Change_To_Former(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTs.Find(id);
            if (sTUDENT == null)
            {
                return HttpNotFound();
            }
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where bt.ID == sTUDENT.BTCH_ID
                                    select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                    .OrderBy(x => x.BatchData.ID).ToList();
            ViewBag.BTCH_ID = string.Concat(queryCourceBatch.FirstOrDefault().CourseData.CODE, "-", queryCourceBatch.FirstOrDefault().BatchData.NAME);

            var paid_fees_val = (from ff in db.FINANCE_FEE
                                 join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                 join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                 where ff.STDNT_ID== sTUDENT.ID && ff.IS_PD == false
                                 select new StundentFee { FeeCollectionData = fc, StudentData = st, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
            ViewData["paid_fees"] = paid_fees_val;
            Session["STD_ID_FOR_TC"] = sTUDENT.ID;

            return View(sTUDENT);
        }

        // GET: Fee Index
        public ActionResult Student_TC_Generate(string id)
        {
            STUDENT sTUDENT = db.STUDENTs.Find(Convert.ToInt32(this.Session["STD_ID_FOR_TC"]));
            Session.Remove("STD_ID_FOR_TC");
            ViewBag.ADMSN_NO = sTUDENT.ADMSN_NO;
            ViewBag.Status_Id = id;
            var query = from m in db.GUARDIANs
                        where m.WARD_ID == sTUDENT.ID
                        select m;
            foreach (var entity in query.ToList())
            {
                db.GUARDIANs.Remove(entity);
                try { db.SaveChanges(); }
                catch (Exception e)
                {
                    ViewBag.TCErrorMessage = string.Concat(ViewBag.TCErrorMessage, "|", e.InnerException.InnerException.Message);
                    return PartialView("_Student_TC_Generate");
                }

            }

            var aRCHIVEDsTD = new ARCHIVED_STUDENT() { ADMSN_NO = sTUDENT.ADMSN_NO, CLS_ROLL_NO = sTUDENT.CLS_ROLL_NO, ADMSN_DATE = sTUDENT.ADMSN_DATE, FIRST_NAME = sTUDENT.FIRST_NAME, MID_NAME = sTUDENT.MID_NAME, LAST_NAME = sTUDENT.LAST_NAME, BTCH_ID = sTUDENT.BTCH_ID, DOB = sTUDENT.DOB, GNDR = sTUDENT.GNDR, BLOOD_GRP = sTUDENT.BLOOD_GRP, BIRTH_PLACE = sTUDENT.BIRTH_PLACE, NTLTY_ID = sTUDENT.NTLTY_ID, LANG = sTUDENT.LANG, RLGN= sTUDENT.RLGN, ADDR_LINE1= sTUDENT.ADDR_LINE1, ADDR_LINE2= sTUDENT.ADDR_LINE2, CITY= sTUDENT.CITY, STATE= sTUDENT.STATE, PIN_CODE= sTUDENT.PIN_CODE, CTRY_ID= sTUDENT.CTRY_ID, PH1= sTUDENT.PH1.ToString(), PH2= sTUDENT.PH2.ToString(), EML= sTUDENT.EML, PHTO_FILENAME= sTUDENT.PHTO_FILENAME, PHTO_CNTNT_TYPE= sTUDENT.PHTO_CNTNT_TYPE, PHTO_DATA= sTUDENT.PHTO_DATA.ToString(), STAT_DESCR= id.ToString(), IS_ACT= sTUDENT.IS_ACT, IS_DEL= sTUDENT.IS_DEL, IMMDT_CNTCT_ID= sTUDENT.IMMDT_CNTCT_ID, IS_SMS_ENABL= sTUDENT.IS_SMS_ENABL, CREATED_AT= sTUDENT.CREATED_AT, UPDATED_AT= sTUDENT.UPDATED_AT, PHTO_FILE_SIZE= sTUDENT.PHTO_FILE_SIZE, FRM_ID= sTUDENT.ID, STDNT_CAT_ID = sTUDENT.STDNT_CAT_ID};
            db.ARCHIVED_STUDENT.Add(aRCHIVEDsTD);
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.TCErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                    }
                }
                return PartialView("_Student_TC_Generate");
            }
            catch (Exception e)
            {
                ViewBag.TCErrorMessage = string.Concat(ViewBag.TCErrorMessage, "|", e.InnerException.InnerException.Message);
                return PartialView("_Student_TC_Generate");
            }


            sTUDENT.IS_ACT = false;
            sTUDENT.IS_DEL = true;
            sTUDENT.STAT_DESCR = id.ToString();
            sTUDENT.UPDATED_AT = System.DateTime.Now;
            db.Entry(sTUDENT).State = EntityState.Modified;
            try { db.SaveChanges(); }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ViewBag.TCErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", ve.ErrorMessage);
                    }
                }
                return PartialView("_Student_TC_Generate");
            }
            catch (Exception e)
            {
                ViewBag.TCErrorMessage = string.Concat(ViewBag.TCErrorMessage, "|", e.InnerException.InnerException.Message);
                return PartialView("_Student_TC_Generate");
            }


            return PartialView("_Student_TC_Generate");
        }

        // GET: Fee Index
        public ActionResult Generate_TC_pdf(string id, string Status_Id)
        {

            var StudentVal = (from st in db.STUDENTs
                           join b in db.BATCHes on st.BTCH_ID equals b.ID into gi
                           from subb in gi.DefaultIfEmpty()
                           join c in db.COURSEs on subb.CRS_ID equals c.ID into gj
                           from subc in gj.DefaultIfEmpty()
                           join emp in db.EMPLOYEEs on subb.EMP_ID equals emp.ID into gm
                           from subemp in gm.DefaultIfEmpty()
                           join grd in db.GUARDIANs on st.ID equals grd.WARD_ID into gd
                           from subgrd in gd.DefaultIfEmpty()
                           where st.ADMSN_NO == id
                           orderby st.LAST_NAME, subb.NAME
                           select new StudentsGuardians { StudentData = st, BatchData = (subb == null ? null : subb), CourseData = (subc == null ? null : subc), EmployeeData = (subemp == null ? null : subemp), GuardianData = (subgrd == null ? null : subgrd) }).Distinct();

            if (StudentVal.FirstOrDefault().GuardianData == null)
            {
                ViewBag.GuardianMessage = "No Parents added to this Student yet. Please click on 'Add Parents' button to add parents.";
            }
            if (Status_Id == "1")
            { ViewBag.Status_Descrition = "Not happy with studies"; }
            else if (Status_Id == "2")
            { ViewBag.Status_Descrition = "Not happy with environment"; }
            else if(Status_Id == "3")
            { ViewBag.Status_Descrition = "Cannot afford fee"; }
            else if(Status_Id == "4")
            { ViewBag.Status_Descrition = "Family moving to other place"; }
            else if(Status_Id == "5")
            { ViewBag.Status_Descrition = "Have concerns with teaching faculties"; }
            else if(Status_Id == "6")
            { ViewBag.Status_Descrition = "No transport facility from my place"; }
            else if(Status_Id == "7")
            { ViewBag.Status_Descrition = "Getting better options in other school"; }
            else if(Status_Id == "8")
            { ViewBag.Status_Descrition = "Have completed all the classes school offers"; }
            else
            { ViewBag.Status_Descrition = "Other reasons"; }
            //ViewBag.Status_Descrition = Status_Id.Equals(1) ? "" : "";
            var Nationality = db.COUNTRies.Find(StudentVal.FirstOrDefault().StudentData.NTLTY_ID);
            ViewBag.NTLTY_ID = Nationality.NTLTY;
            var Category = db.STUDENT_CATGEORY.Find(StudentVal.FirstOrDefault().StudentData.STDNT_CAT_ID);
            ViewBag.STDNT_CAT_ID = Category.NAME;
            var Country = db.COUNTRies.Find(StudentVal.FirstOrDefault().StudentData.CTRY_ID);
            ViewBag.CTRY_ID = Country.CTRY_NAME;

            return View(StudentVal.ToList());
        }

        // GET: Fee Index
        public ActionResult BonafideCertificate(int? id)
        {
            var Student = (from st in db.STUDENTs
                           join bt in db.BATCHes on st.BTCH_ID equals bt.ID
                           join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                           where st.ID == id
                           select new Student { StudentData = st, BatcheData = bt, CourseData = cs }).FirstOrDefault();

            return View(Student);
        }

        // GET: Fee Index
        public ActionResult Generate_BC_pdf(int? id)
        {
            var Student = (from st in db.STUDENTs
                           join bt in db.BATCHes on st.BTCH_ID equals bt.ID
                           join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                           where st.ID == id
                           select new Student { StudentData = st, BatcheData = bt, CourseData = cs }).FirstOrDefault();
            var StudentGuardians = (from st in db.STUDENTs
                                    join gd in db.GUARDIANs on st.ID equals gd.WARD_ID
                                    join bt in db.BATCHes on st.BTCH_ID equals bt.ID
                                    join cs in db.COURSEs on bt.CRS_ID equals cs.ID
                                    where st.ID == id
                                    select new StudentsGuardians { StudentData = st, GuardianData = gd, BatchData = bt, CourseData = cs }).OrderBy(x => x.StudentData.ID).Distinct();
            ViewData["guardians"] = StudentGuardians;

            return View(Student);
        }

        // GET: Student/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT sTUDENT = db.STUDENTs.Find(id);
            if (sTUDENT == null)
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
                result.Selected = item.BatchData.ID == sTUDENT.BTCH_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.BTCH_ID = options;
            return View(sTUDENT);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            STUDENT sTUDENT = db.STUDENTs.Find(id);
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
                result.Selected = item.BatchData.ID == sTUDENT.BTCH_ID ? true : false;
                options.Add(result);
            }
            // add the 'ALL' option
            options.Insert(0, new SelectListItem() { Value = "-1", Text = "ALL" });
            ViewBag.BTCH_ID = options;

            var query = from m in db.GUARDIANs
                        where m.WARD_ID == id
                        select m;
            foreach (var entity in query.ToList())
            {
                db.GUARDIANs.Remove(entity);
                try { db.SaveChanges();}
                catch (Exception e)
                {
                    ViewBag.DeleteMessage = string.Concat(ViewBag.DeleteMessage, "|", e.InnerException.InnerException.Message);
                    return View(sTUDENT);
                }

            }           
            sTUDENT.IS_ACT = false;
            sTUDENT.IS_DEL = true;
            sTUDENT.UPDATED_AT = System.DateTime.Now;
            db.Entry(sTUDENT).State = EntityState.Modified;
            try { db.SaveChanges(); ViewBag.DeleteMessage = "Student deleted from system successfully"; }
            catch (Exception e)
            {
                ViewBag.DeleteMessage = string.Concat(ViewBag.DeleteMessage, "|", e.InnerException.InnerException.Message);
                return View(sTUDENT);
            }
            return View(sTUDENT);
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
            var sTUDENTcATEGORY = db.STUDENT_CATGEORY.ToList();
            return View(sTUDENTcATEGORY);
        }

        // GET: Student/Delete/5
        public ActionResult _CategoriesDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STUDENT_CATGEORY sTUDENTcATEGORY = db.STUDENT_CATGEORY.Find(id);
            if (sTUDENTcATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(sTUDENTcATEGORY);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("_CategoriesDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult _CategoriesDeleteConfirmed(int id)
        {
            STUDENT_CATGEORY sTUDENTcATEGORY = db.STUDENT_CATGEORY.Find(id);
            db.STUDENT_CATGEORY.Remove(sTUDENTcATEGORY);
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
            STUDENT_CATGEORY sTUDENTcATEGORY = db.STUDENT_CATGEORY.Find(id);
            if (sTUDENTcATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(sTUDENTcATEGORY);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _CategoriesEdit([Bind(Include = "ID,NAME,IS_DEL")] STUDENT_CATGEORY sTUDENTcATEGORY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sTUDENTcATEGORY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Categories");
            }
            return View(sTUDENTcATEGORY);
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoriesCreate([Bind(Include = "ID,NAME,IS_DEL")] STUDENT_CATGEORY sTUDENTcATEGORY)
        {
            if (ModelState.IsValid)
            {
                db.STUDENT_CATGEORY.Add(sTUDENTcATEGORY);
                db.SaveChanges();
                return RedirectToAction("Categories");
            }

            return View(sTUDENTcATEGORY);
        }

        // GET: Student/Guardian Details
        public ActionResult Guardians(int? Std_id)
        {
            //var parents = db.GUARDIANs.Where(x=>x.WARD_ID == Std_id).ToList();

            var parents = (from st in db.STUDENTs
                            join b in db.BATCHes on st.BTCH_ID equals b.ID into gi
                            from subb in gi.DefaultIfEmpty()
                            join c in db.COURSEs on subb.CRS_ID equals c.ID into gj
                            from subc in gj.DefaultIfEmpty()
                            join emp in db.EMPLOYEEs on subb.EMP_ID equals emp.ID into gm
                            from subemp in gm.DefaultIfEmpty()
                            join grd in db.GUARDIANs on st.ID equals grd.WARD_ID into gd
                            from subgrd in gd.DefaultIfEmpty()
                           where st.ID == Std_id && st.IS_DEL == false
                           orderby st.LAST_NAME, subb.NAME
                            select new StudentsGuardians { StudentData = st, BatchData = (subb == null ? null : subb), CourseData = (subc == null ? null : subc),  EmployeeData = (subemp == null ? null : subemp), GuardianData = (subgrd == null ? null : subgrd) }).Distinct();

            if (parents.FirstOrDefault().GuardianData == null)
            {
                ViewBag.GuardianMessage = "No Parents added to this Student yet. Please click on 'Add Parents' button to add parents.";
            }
            ViewBag.WARD_ID = Std_id;

            return View(parents.ToList());
        }

        // GET: Student/Guardian Details
        public ActionResult Edit_Guardian(int? id)
        {
            GUARDIAN parents = db.GUARDIANs.Find(id);

            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", parents.CTRY_ID);
            DateTime SDate = Convert.ToDateTime(parents.DOB);
            ViewBag.DOB = SDate.ToShortDateString();
            //var aDMISSIONnUM = db.STUDENTs.Where(x => x.ID == parents.WARD_ID).ToList().FirstOrDefault().ADMSN_NO;

            ViewBag.ReturnDate = System.DateTime.Now;
            STUDENT sTUDENT = db.STUDENTs.Find(parents.WARD_ID);
            ViewBag.StudentFullName = String.Format("{0} {1}", sTUDENT.FIRST_NAME.ToString(), sTUDENT.LAST_NAME.ToString());
            ViewBag.NewAdmissionNumber = sTUDENT.ADMSN_NO.ToString();
            if (!String.IsNullOrEmpty(sTUDENT.ADDR_LINE1))
            {
                ViewBag.AddressLine1 = sTUDENT.ADDR_LINE1.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.ADDR_LINE2))
            {
                ViewBag.AddressLine2 = sTUDENT.ADDR_LINE2.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.CITY))
            {
                ViewBag.StudentCity = sTUDENT.CITY.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.STATE))
            {
                ViewBag.StudentState = sTUDENT.STATE.ToString();
            }
            if (!sTUDENT.COUNTRY.Equals(null))
            {
                ViewBag.CountryName = sTUDENT.COUNTRY.ToString();
            }
            ViewBag.StudentId = parents.WARD_ID;
            ViewBag.ADMSN_NO = sTUDENT.ADMSN_NO;

            return View(parents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Guardian([Bind(Include = "ID,WARD_ID,FIRST_NAME,LAST_NAME, REL, EML, OFF_PH1, OFF_PH2, MOBL_PH, OFF_ADDR_LINE1, OFF_ADDR_LINE2, CITY, STATE, CTRY_ID, DOB, OCCP, INCM, ED, USRID")] GUARDIAN gUARDIAN, string ADMSN_NO)
        {
            if (ModelState.IsValid)
            {
                GUARDIAN gUARDIAN_UPD = db.GUARDIANs.Find(gUARDIAN.ID);
                gUARDIAN_UPD.ID = gUARDIAN.ID;
                gUARDIAN_UPD.FIRST_NAME = gUARDIAN.FIRST_NAME;
                gUARDIAN_UPD.LAST_NAME = gUARDIAN.LAST_NAME;
                gUARDIAN_UPD.REL = gUARDIAN.REL;
                gUARDIAN_UPD.EML = gUARDIAN.EML;
                gUARDIAN_UPD.OFF_PH1 = gUARDIAN.OFF_PH1;
                gUARDIAN_UPD.OFF_PH2 = gUARDIAN.OFF_PH2;
                gUARDIAN_UPD.MOBL_PH = gUARDIAN.MOBL_PH;
                gUARDIAN_UPD.OFF_ADDR_LINE1 = gUARDIAN.OFF_ADDR_LINE1;
                gUARDIAN_UPD.OFF_ADDR_LINE2 = gUARDIAN.OFF_ADDR_LINE2;
                gUARDIAN_UPD.CITY = gUARDIAN.CITY;
                gUARDIAN_UPD.STATE = gUARDIAN.STATE;
                gUARDIAN_UPD.CTRY_ID = gUARDIAN.CTRY_ID;
                gUARDIAN_UPD.DOB = gUARDIAN.DOB;
                gUARDIAN_UPD.OCCP = gUARDIAN.OCCP;
                gUARDIAN_UPD.INCM = gUARDIAN.INCM;
                gUARDIAN_UPD.ED = gUARDIAN.ED;
                gUARDIAN_UPD.UPDATED_AT = DateTime.Now;
                db.Entry(gUARDIAN_UPD).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.GuardianEditMessage = "Guardian Details updated successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.GuardianEditMessage = e.InnerException.InnerException.Message; }
                ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", gUARDIAN.CTRY_ID);
                DateTime SDate1 = Convert.ToDateTime(gUARDIAN.DOB);
                ViewBag.DOB = SDate1.ToShortDateString();
                //var aDMISSIONnUM1 = db.STUDENTs.Where(x => x.ID == gUARDIAN.WARD_ID).ToList().FirstOrDefault().ADMSN_NO;
                ViewBag.ADMSN_NO = ADMSN_NO;
                return View(gUARDIAN);
            }
            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", gUARDIAN.CTRY_ID);
            DateTime SDate = Convert.ToDateTime(gUARDIAN.DOB);
            ViewBag.DOB = SDate.ToShortDateString();
            //var aDMISSIONnUM = db.STUDENTs.Where(x => x.ID == gUARDIAN.WARD_ID).ToList().FirstOrDefault().ADMSN_NO;
            ViewBag.ADMSN_NO = ADMSN_NO;
            return View(gUARDIAN);

        }

        // GET: Student/Delete/5
        public ActionResult Delete_Guardian(int? id, int? std_id)
        {
            GUARDIAN gUARDIAN = db.GUARDIANs.Find(id);
            db.GUARDIANs.Remove(gUARDIAN);
            db.SaveChanges();
            return RedirectToAction("Guardians", new { Std_id = std_id });
        }

        // GET: Student/Delete/5
        public ActionResult Add_Guardian(int? std_id)
        {
            ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", "99");
            
            ViewBag.ReturnDate = System.DateTime.Now;
            STUDENT sTUDENT = db.STUDENTs.Find(std_id);
            ViewBag.StudentFullName = sTUDENT.FIRST_NAME.ToString();
            if (sTUDENT.LAST_NAME != null)
            {
                ViewBag.StudentFullName = String.Format("{0} {1}", sTUDENT.FIRST_NAME.ToString(), sTUDENT.LAST_NAME.ToString());
            }
            ViewBag.NewAdmissionNumber = sTUDENT.ADMSN_NO.ToString();
            if (!String.IsNullOrEmpty(sTUDENT.ADDR_LINE1))
            {
                ViewBag.AddressLine1 = sTUDENT.ADDR_LINE1.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.ADDR_LINE2))
            {
                ViewBag.AddressLine2 = sTUDENT.ADDR_LINE2.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.CITY))
            {
                ViewBag.StudentCity = sTUDENT.CITY.ToString();
            }
            if (!String.IsNullOrEmpty(sTUDENT.STATE))
            {
                ViewBag.StudentState = sTUDENT.STATE.ToString();
            }
            if (!sTUDENT.COUNTRY.Equals(null))
            {
                ViewBag.CountryName = sTUDENT.COUNTRY.ToString();
            }
            ViewBag.StudentId = std_id;
            ViewBag.ADMSN_NO = sTUDENT.ADMSN_NO;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Guardian([Bind(Include = "ID,WARD_ID,FIRST_NAME,LAST_NAME,REL,EML,OFF_PH1,OFF_PH2,MOBL_PH,OFF_ADDR_LINE1,OFF_ADDR_LINE2,CITY,STATE,CTRY_ID,DOB,OCCP,INCM,ED,CREATED_AT,UPDATED_AT,USRID")] GUARDIAN gUARDIAN, string ADMSN_NO)
        {
            if (ModelState.IsValid)
            {
                gUARDIAN.CREATED_AT = DateTime.Now;
                gUARDIAN.UPDATED_AT = DateTime.Now;
                db.GUARDIANs.Add(gUARDIAN);
                string FullName = Regex.Replace(string.Concat(gUARDIAN.FIRST_NAME, gUARDIAN.LAST_NAME, Random_String(5)), @"\s", "");
                var ParUser = new USER() { USRNAME = FullName, FIRST_NAME = gUARDIAN.FIRST_NAME, LAST_NAME = gUARDIAN.LAST_NAME, EML = gUARDIAN.EML, ROLE = "Parent" };
                db.USERS.Add(ParUser);
                try { db.SaveChanges(); ViewBag.GuardianAddMessage = "Guardian Added successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.GuardianAddMessage = e.InnerException.InnerException.Message; }
                // some code 
                TempData["alertMessage"] = string.Concat("Student Admission Done. Website User ID :", ParUser.USRNAME, "     Password :", ParUser.HASHED_PSWRD);

                ViewBag.CTRY_ID = new SelectList(db.COUNTRies, "ID", "CTRY_NAME", "99");
                //DateTime SDate = Convert.ToDateTime(parents.DOB);
                //ViewBag.DOB = SDate.ToShortDateString();
                var aDMISSIONnUM = db.STUDENTs.Where(x => x.ID == gUARDIAN.WARD_ID).ToList().FirstOrDefault().ADMSN_NO;
                ViewBag.ADMSN_NO = aDMISSIONnUM;
                return RedirectToAction("admission3_1", new { Std_id = gUARDIAN .WARD_ID});
            }

            return View(gUARDIAN);
        }

        // GET: Student/Admission3
        public ActionResult admission3_1(int? Std_id)
        {
            //List<SelectGuardian> Guardian = new List<SelectGuardian>();

            var GuardianVal = (from C in db.GUARDIANs
                               where C.WARD_ID == Std_id
                               select new SelectGuardian { GuardianList = C }).ToList();
            if (GuardianVal.Count().Equals(0))
            {
                ViewBag.ErrorMessage = "Any guardian must be added to proceed forward.";
            }
            return View(GuardianVal);

        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: Student/Admission3
        public ActionResult admission3_1(IList<SelectGuardian> model)
        {
            foreach (SelectGuardian item in model)
            {
                if (item.Selected)
                {
                    STUDENT sTUDENT = db.STUDENTs.Find(item.GuardianList.WARD_ID);
                    sTUDENT.IMMDT_CNTCT_ID = item.GuardianList.ID;
                    //db.SaveChanges();
                    try { db.SaveChanges(); ViewBag.Notice = string.Concat("Guardian Details added successfully for Studnet Id:", item.GuardianList.WARD_ID); }
                    catch (Exception e) { ViewBag.ErrorMessage = e.InnerException.InnerException.Message; return View(model); }
                    break;
                }
            }
            return RedirectToAction("Profiles", "Student", new { id = model.FirstOrDefault().GuardianList.WARD_ID});
            //return View(model);
        }

        // GET: Student/Guardian Details
        public ActionResult Add_Existing_Guardian(int? Std_id)
        {
            //var parents = db.GUARDIANs.Where(x=>x.WARD_ID == Std_id).ToList();

            var parents = (from st in db.STUDENTs
                           join b in db.BATCHes on st.BTCH_ID equals b.ID into gi
                           from subb in gi.DefaultIfEmpty()
                           join c in db.COURSEs on subb.CRS_ID equals c.ID into gj
                           from subc in gj.DefaultIfEmpty()
                           join emp in db.EMPLOYEEs on subb.EMP_ID equals emp.ID into gm
                           from subemp in gm.DefaultIfEmpty()
                           join grd in db.GUARDIANs on st.ID equals grd.WARD_ID
                           where st.IS_DEL == false
                           orderby grd.LAST_NAME, grd.FIRST_NAME
                           select new StudentsGuardians { StudentData = st, BatchData = (subb == null ? null : subb), CourseData = (subc == null ? null : subc), EmployeeData = (subemp == null ? null : subemp), GuardianData = grd }).Distinct();

            ViewBag.WARD_ID = Std_id;

            return View(parents.ToList());
        }

        // GET: Student/Guardian Details
        public ActionResult Add_Existing_Guardian2(int? Std_id, int? Parent_id)
        {
            var gUARDIAN = db.GUARDIANs.Find(Parent_id);

            var gUARDIANnEW = new GUARDIAN()
            {
                WARD_ID = Std_id,
                FIRST_NAME = gUARDIAN.FIRST_NAME,
                LAST_NAME = gUARDIAN.LAST_NAME,
                REL = gUARDIAN.REL,
                EML = gUARDIAN.EML,
                OFF_PH1 = gUARDIAN.OFF_PH1,
                OFF_PH2 = gUARDIAN.OFF_PH2,
                MOBL_PH = gUARDIAN.MOBL_PH,
                OFF_ADDR_LINE1 = gUARDIAN.OFF_ADDR_LINE1,
                OFF_ADDR_LINE2 = gUARDIAN.OFF_ADDR_LINE2,
                CITY = gUARDIAN.CITY,
                STATE = gUARDIAN.STATE,
                CTRY_ID = gUARDIAN.CTRY_ID,
                DOB = gUARDIAN.DOB,
                OCCP = gUARDIAN.OCCP,
                INCM = gUARDIAN.INCM,
                ED = gUARDIAN.ED,
                CREATED_AT = System.DateTime.Now,
                UPDATED_AT = System.DateTime.Now,
                USRID = gUARDIAN.USRID
            };
            db.GUARDIANs.Add(gUARDIANnEW);
            try { db.SaveChanges(); ViewBag.GuardianAddMessage = "Guardian Added successfully."; }
            catch (Exception e) { Console.WriteLine(e); ViewBag.GuardianAddMessage = e.InnerException.InnerException.Message; }
            return RedirectToAction("Admission2", "Student", new { Std_id = Std_id });
        }


        // GET: Student/Guardian Details
        public ActionResult Fees(int? id, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            STUDENT student = db.STUDENTs.Find(id);
            var StudentValDefaulters = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                        where st.ID== id && fc.IS_DEL == false
                                        select new StundentFee { StudentData = st, FeeCollectionData = fc, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
            ViewData["dates"] = StudentValDefaulters;
            var batch_val = (from cs in db.COURSEs
                             join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                             where bt.ID == student.BTCH_ID
                             select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = batch_val;

            var batches_val = (from cs in db.COURSEs
                             join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                             select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batches"] = batches_val;

            var paid_fees_val = (from ff in db.FINANCE_FEE
                                 join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                 join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                 join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                 where st.ID == id && fc.IS_DEL == false
                                 select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff, FeeCollectionData = fc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewData["paid_fees"] = paid_fees_val;

            var fee_particulars_val = (from fcol in db.FINANCE_FEE_COLLECTION
                                       join fc in db.FINANCE_FEE_CATGEORY on fcol.FEE_CAT_ID equals fc.ID
                                       join ff in db.FINANCE_FEE_PARTICULAR on fc.ID equals ff.FIN_FEE_CAT_ID
                                       where ff.IS_DEL == "N" && (ff.STDNT_ID == student.ID || ff.STDNT_ID == null) && (ff.STDNT_CAT_ID == student.STDNT_CAT_ID || ff.STDNT_CAT_ID == null)
                                       select new FeeParticular { FeeParticularData = ff, FeeCategoryData = fc, FeeCollectionData = fcol }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
            ViewData["fee_particulars"] = fee_particulars_val;

            var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                       where ff.TYPE == "Batch" /*&& ff.RCVR_ID == student.BTCH_ID*/
                                       select ff);
            ViewData["batch_discounts"] = batch_discounts_val;
            /*Condition of ff.RCVR_ID == student.BTCH_ID is removed fromt this code as, adding this would have caused inssue in calculating discounts in case students chnages Batch."*/

            var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                         where ff.TYPE == "Student" && ff.RCVR_ID == student.ID
                                         select ff);
            ViewData["student_discounts"] = student_discounts_val;
            var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                          where ff.TYPE == "Student Category" && ff.RCVR_ID == student.STDNT_CAT_ID
                                          select ff);
            ViewData["category_discounts"] = category_discounts_val;



            var batch_fine_val = (from ff in db.FEE_FINE
                                  where ff.TYPE == "Batch" /*&& ff.RCVR_ID == student.BTCH_ID*/
                                  select ff);
            ViewData["batch_fine"] = batch_fine_val;
            /*Condition of ff.RCVR_ID == student.BTCH_ID is removed fromt this code as, adding this would have caused inssue in calculating discounts in case students chnages Batch."*/

            var student_fine_val = (from ff in db.FEE_FINE
                                    where ff.TYPE == "Student" && ff.RCVR_ID == student.ID
                                    select ff);
            ViewData["student_fine"] = student_fine_val;

            var category_fine_val = (from ff in db.FEE_FINE
                                     where ff.TYPE == "Student Category" && ff.RCVR_ID == student.STDNT_CAT_ID
                                     select ff);
            ViewData["category_fine"] = category_fine_val;

            var fee_select = db.FINANCE_FEE_COLLECTION.Include(x => x.FINANCE_FEE_CATGEORY).Where(x => x.BTCH_ID == student.BTCH_ID && !StudentValDefaulters.Any(p => x.ID.Equals(p.FeeCollectionData.ID))).ToList();
            ViewData["fee_select"] = fee_select;

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Fees([Bind(Include = "ID,ADMSN_NO,CLS_ROLL_NO,ADMSN_DATE,FIRST_NAME,MID_NAME,LAST_NAME,BTCH_ID,DOB,GNDR,BLOOD_GRP,BIRTH_PLACE,LANG,RLGN,ADDR_LINE1,ADDR_LINE2,CITY,STATE,PIN_CODE,CTRY_ID,PH1,PH2,EML,IMMDT_CNTCT_ID,IS_SMS_ENABL,PHTO_FILENAME,PHTO_CNTNT_TYPE,PHTO_DATA,STAT_DESCR,IS_ACT,IS_DEL,CREATED_AT,UPDATED_AT,HAS_PD_FE,PHTO_FILE_SIZE,USRID,STDNT_CAT_ID,NTLTY_ID,IMAGE_DOCUMENTS_ID")] STUDENT sTUDENT)
        {
            if (ModelState.IsValid)
            {

                STUDENT sTUDENTtOuPDATE = db.STUDENTs.Find(sTUDENT.ID);

                sTUDENTtOuPDATE.HAS_PD_FE = sTUDENT.HAS_PD_FE;

                sTUDENTtOuPDATE.UPDATED_AT = System.DateTime.Now;
                db.Entry(sTUDENTtOuPDATE).State = EntityState.Modified;
                try { db.SaveChanges(); ViewBag.ErrorMessage = "Student's Details updated successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }

                STUDENT student = db.STUDENTs.Find(sTUDENT.ID);
                var StudentValDefaulters = (from ff in db.FINANCE_FEE
                                            join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                            join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                            where st.ID == sTUDENT.ID && fc.IS_DEL == false
                                            select new StundentFee { StudentData = st, FeeCollectionData = fc, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                ViewData["dates"] = StudentValDefaulters;
                var batch_val = (from cs in db.COURSEs
                                 join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                 where bt.ID == student.BTCH_ID
                                 select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                                 .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batch"] = batch_val;
                var batches_val = (from cs in db.COURSEs
                                   join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                   select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
                ViewData["batches"] = batches_val;

                var paid_fees_val = (from ff in db.FINANCE_FEE
                                     join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                     join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                     join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                     where st.ID == sTUDENT.ID
                                     select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff, FeeCollectionData = fc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
                ViewData["paid_fees"] = paid_fees_val;

                var fee_particulars_val = (from fcol in db.FINANCE_FEE_COLLECTION
                                           join fc in db.FINANCE_FEE_CATGEORY on fcol.FEE_CAT_ID equals fc.ID
                                           join ff in db.FINANCE_FEE_PARTICULAR on fc.ID equals ff.FIN_FEE_CAT_ID
                                           where fcol.BTCH_ID == student.BTCH_ID && (fc.IS_DEL == false)
                                           select new FeeParticular { FeeParticularData = ff, FeeCategoryData = fc, FeeCollectionData = fcol }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
                ViewData["fee_particulars"] = fee_particulars_val;

                var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                           where ff.TYPE == "Batch" /*&& ff.RCVR_ID == student.BTCH_ID*/
                                           select ff);
                ViewData["batch_discounts"] = batch_discounts_val;
                /*Condition of ff.RCVR_ID == student.BTCH_ID is removed fromt this code as, adding this would have caused inssue in calculating discounts in case students chnages Batch."*/

                var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                             where ff.TYPE == "Student" && ff.RCVR_ID == student.ID
                                             select ff);
                ViewData["student_discounts"] = student_discounts_val;
                var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                              where ff.TYPE == "Student Category" && ff.RCVR_ID == student.STDNT_CAT_ID
                                              select ff);
                ViewData["category_discounts"] = category_discounts_val;



                var batch_fine_val = (from ff in db.FEE_FINE
                                      where ff.TYPE == "Batch" /*&& ff.RCVR_ID == student.BTCH_ID*/
                                      select ff);
                ViewData["batch_fine"] = batch_fine_val;
                /*Condition of ff.RCVR_ID == student.BTCH_ID is removed fromt this code as, adding this would have caused inssue in calculating discounts in case students chnages Batch."*/

                var student_fine_val = (from ff in db.FEE_FINE
                                        where ff.TYPE == "Student" && ff.RCVR_ID == student.ID
                                        select ff);
                ViewData["student_fine"] = student_fine_val;

                var category_fine_val = (from ff in db.FEE_FINE
                                         where ff.TYPE == "Student Category" && ff.RCVR_ID == student.STDNT_CAT_ID
                                         select ff);
                ViewData["category_fine"] = category_fine_val;

                return View(sTUDENTtOuPDATE);
            }
            ViewBag.ErrorMessage = "There seems to be some issue with ModelState.";
            return View(sTUDENT);
        }

        public ActionResult Add_Fee(int? id, int? std_id)
        {
            var dates = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                        where st.ID == std_id && fc.IS_DEL == false
                                        select new StundentFee { StudentData = st, FeeCollectionData = fc, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();

            var fee_select = db.FINANCE_FEE_COLLECTION.Include(x => x.FINANCE_FEE_CATGEORY).Where(x => x.ID == id && !dates.Any(p => x.ID.Equals(p.FeeCollectionData.ID))).ToList();

            if (fee_select != null && fee_select.Count() != 0)
            {
                FINANCE_FEE NewFee = new FINANCE_FEE { FEE_CLCT_ID = id, STDNT_ID = std_id, TRAN_ID = null, IS_PD = false };
                db.FINANCE_FEE.Add(NewFee);
                try { db.SaveChanges(); ViewBag.Notice = "Fee Collection added for student successfully."; }
                catch (Exception e) { Console.WriteLine(e); ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }
            }
            else
            {
                ViewBag.ErrorMessage = "Fee already added for student.";
            }
            return RedirectToAction("Fees", new { id = std_id, ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }
        // GET: Student/Guardian Details
        public ActionResult Fee_Details(int? id, int? id2, string Notice, string ErrorMessage)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            STUDENT student = db.STUDENTs.Find(id);
            var StudentValDefaulters = (from ff in db.FINANCE_FEE
                                        join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                        join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                        where st.ID == id && fc.IS_DEL == false && fc.ID==id2
                                        select new StundentFee { StudentData = st, FeeCollectionData = fc, FinanceFeeData = ff }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
            ViewData["dates"] = StudentValDefaulters;
            var batch_val = (from cs in db.COURSEs
                             join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                             where bt.ID == student.BTCH_ID
                             select new SelectCourseBatch { CourseData = cs, BatchData = bt, Selected = false })
                             .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = batch_val;

            var paid_fees_val = (from ff in db.FINANCE_FEE
                                 join st in db.STUDENTs on ff.STDNT_ID equals st.ID
                                 join ft in db.FINANCE_TRANSACTION on ff.ID equals ft.FIN_FE_ID
                                 join fc in db.FINANCE_FEE_COLLECTION on ff.FEE_CLCT_ID equals fc.ID
                                 where st.ID == id && fc.ID == id2
                                 select new FeeTransaction { FinanceTransactionData = ft, StudentData = st, FinanceFeeData = ff, FeeCollectionData = fc }).OrderBy(x => x.FinanceTransactionData.CRETAED_AT).Distinct();
            ViewData["paid_fees"] = paid_fees_val;

            var fee_particulars_val = (from fcol in db.FINANCE_FEE_COLLECTION
                                       join fc in db.FINANCE_FEE_CATGEORY on fcol.FEE_CAT_ID equals fc.ID
                                       join ffp in db.FINANCE_FEE_PARTICULAR on fc.ID equals ffp.FIN_FEE_CAT_ID
                                       where fc.IS_DEL == false && fcol.ID == id2 && ffp.IS_DEL == "N" && (ffp.STDNT_ID == student.ID || ffp.STDNT_ID == null) && (ffp.STDNT_CAT_ID == student.STDNT_CAT_ID || ffp.STDNT_CAT_ID == null)
                                       select new FeeParticular { FeeParticularData = ffp, FeeCategoryData = fc, FeeCollectionData = fcol }).OrderBy(x => x.FeeCollectionData.DUE_DATE).Distinct();
            ViewData["fee_particulars"] = fee_particulars_val;

            var batch_discounts_val = (from ff in db.FEE_DISCOUNT
                                       where ff.FIN_FEE_CAT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.BTCH_ID && (ff.FEE_CLCT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= StudentValDefaulters.FirstOrDefault().FeeCollectionData.DUE_DATE))
                                       select ff);
            ViewData["batch_discounts"] = batch_discounts_val;
            var student_discounts_val = (from ff in db.FEE_DISCOUNT
                                         where ff.FIN_FEE_CAT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == student.ID && (ff.FEE_CLCT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= StudentValDefaulters.FirstOrDefault().FeeCollectionData.DUE_DATE))
                                         select ff);
            ViewData["student_discounts"] = student_discounts_val;
            var category_discounts_val = (from ff in db.FEE_DISCOUNT
                                          where ff.FIN_FEE_CAT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == student.STDNT_CAT_ID && (ff.FEE_CLCT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.ID || (ff.FEE_CLCT_ID == null && ff.DISC_DATE <= StudentValDefaulters.FirstOrDefault().FeeCollectionData.DUE_DATE))
                                          select ff);
            ViewData["category_discounts"] = category_discounts_val;



            var batch_fine_val = (from ff in db.FEE_FINE
                                  where ff.FIN_FEE_CAT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.FEE_CAT_ID && ff.TYPE == "Batch" && ff.RCVR_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.BTCH_ID && (ff.FEE_CLCT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= StudentValDefaulters.FirstOrDefault().FeeCollectionData.DUE_DATE))
                                  select ff);
            ViewData["batch_fine"] = batch_fine_val;
            var student_fine_val = (from ff in db.FEE_FINE
                                    where ff.FIN_FEE_CAT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.FEE_CAT_ID && ff.TYPE == "Student" && ff.RCVR_ID == student.ID && (ff.FEE_CLCT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= StudentValDefaulters.FirstOrDefault().FeeCollectionData.DUE_DATE))
                                    select ff);
            ViewData["student_fine"] = student_fine_val;

            var category_fine_val = (from ff in db.FEE_FINE
                                     where ff.FIN_FEE_CAT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.FEE_CAT_ID && ff.TYPE == "Student Category" && ff.RCVR_ID == student.STDNT_CAT_ID && (ff.FEE_CLCT_ID == StudentValDefaulters.FirstOrDefault().FeeCollectionData.ID || (ff.FEE_CLCT_ID == null && ff.FINE_DATE <= StudentValDefaulters.FirstOrDefault().FeeCollectionData.DUE_DATE))
                                     select ff);
            ViewData["category_fine"] = category_fine_val;

            decimal total_discount_val = 0;
            decimal total_payable = 0;
            foreach (var item in fee_particulars_val)
            {
                total_payable = total_payable + (decimal)item.FeeParticularData.AMT;
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

            return View(student);
        }

        // GET: Student/Edit/5
        public ActionResult Activate_Fees(int? id, int? id2)
        {
            FINANCE_FEE Fee_To_Activate = db.FINANCE_FEE.Where(x => x.STDNT_ID == id && x.FEE_CLCT_ID == id2).FirstOrDefault();
            Fee_To_Activate.IS_PD = false;
            db.Entry(Fee_To_Activate).State = EntityState.Modified;
            try { db.SaveChanges(); ViewBag.Notice = "Fees Activated successfully to be edited in Fee Collection module."; }
            catch (Exception e) { ViewBag.ErrorMessage = e.InnerException.InnerException.Message; }

            return RedirectToAction("Fee_Details", new { id = id, id2 = id2, Notice = ViewBag.Notice , ErrorMessage = ViewBag.ErrorMessage });
        }

        // GET: Student/Edit/5
        public ActionResult Electives(int? id, int? id2, string Notice)
        {
            ViewBag.Notice = Notice;
            var queryCourceBatch = (from cs in db.COURSEs
                                    join bt in db.BATCHes on cs.ID equals bt.CRS_ID
                                    where cs.IS_DEL == false && bt.ID == id
                                    select new CoursesBatch { CourseData = cs, BatchData = bt})
                         .OrderBy(x => x.BatchData.ID).ToList();
            ViewData["batch"] = queryCourceBatch;
            var elective_subject = db.SUBJECTs.Find(id2);
            ViewData["elective_subject"] = elective_subject;
            var students = db.STUDENTs.Where(x => x.BTCH_ID == id).ToList();
            ViewData["students"] = students;
            var elective_group = db.ELECTIVE_GROUP.Find(elective_subject.ELECTIVE_GRP_ID);
            ViewData["elective_group"] = elective_group;
            var stud_assigned = db.STUDENT_SUBJECT.ToList();
            ViewData["stud_assigned"] = stud_assigned;

            return View();
        }

        // GET: Student/Edit/5
        public ActionResult Assign_Students(int? id, int? id2)
        {
            STUDENT student = db.STUDENTs.Find(id);
            SUBJECT elective_subject = db.SUBJECTs.Find(id2);
            STUDENT_SUBJECT StudentsSubject = new STUDENT_SUBJECT{STDNT_ID=id, SUBJ_ID = id2, BTCH_ID = student.BTCH_ID};
            db.STUDENT_SUBJECT.Add(StudentsSubject);
            db.SaveChanges();
            ViewBag.Notice = "Assign students successfully";
            return RedirectToAction("Electives",new { id = student.BTCH_ID, id2 = elective_subject.ID, Notice= ViewBag.Notice });
        }

        // GET: Student/Edit/5
        public ActionResult Assign_All_Students(int? id, int? id2)
        {
            BATCH batch = db.BATCHes.Find(id);
            var students = db.STUDENTs.Where(x => x.BTCH_ID == id).ToList();
            foreach(var item in students)
            {
                var assigned = db.STUDENT_SUBJECT.Where(x => x.STDNT_ID == item.ID && x.SUBJ_ID == id2).ToList();
                if(assigned == null || assigned.Count() ==0)
                {
                    STUDENT_SUBJECT StudentsSubject = new STUDENT_SUBJECT { STDNT_ID = item.ID, SUBJ_ID = id2, BTCH_ID = batch.ID };
                    db.STUDENT_SUBJECT.Add(StudentsSubject);
                    db.SaveChanges();
                }
            }
            SUBJECT elective_subject = db.SUBJECTs.Find(id2);
            ViewBag.Notice = "Assign all students successfully";
            return RedirectToAction("Electives", new { id = batch.ID, id2 = elective_subject.ID, Notice = ViewBag.Notice });
        }
        // GET: Student/Edit/5
        public ActionResult Unassign_Students(int? id, int? id2)
        {
            STUDENT student = db.STUDENTs.Find(id);
            SUBJECT elective_subject = db.SUBJECTs.Find(id2);
            var query = db.STUDENT_SUBJECT.Where(x=>x.STDNT_ID ==id && x.SUBJ_ID == id2).ToList();
            foreach (var entity in query)
            {
                db.STUDENT_SUBJECT.Remove(entity);
                try { db.SaveChanges(); }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Electives", new { id = student.BTCH_ID, id2 = elective_subject.ID, ErrorMessage = ViewBag.ErrorMessage });
                }

            }
            ViewBag.Notice = "Remove students successfully";
            return RedirectToAction("Electives", new { id = student.BTCH_ID, id2 = elective_subject.ID, Notice = ViewBag.Notice });
        }


        // GET: Student/Edit/5
        public ActionResult Unassign_All_Students(int? id, int? id2)
        {
            BATCH batch = db.BATCHes.Find(id);
            SUBJECT elective_subject = db.SUBJECTs.Find(id2);
            var students = db.STUDENTs.Where(x => x.BTCH_ID == id).ToList();
            foreach (var item in students)
            {
                var assigned = db.STUDENT_SUBJECT.Where(x => x.STDNT_ID == item.ID && x.SUBJ_ID == id2).ToList();
                if (assigned != null && assigned.Count() != 0)
                {
                    foreach (var entity in assigned)
                    {
                        db.STUDENT_SUBJECT.Remove(entity);
                        try { db.SaveChanges(); }
                        catch (Exception e)
                        {
                            ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                            return RedirectToAction("Electives", new { id = batch.ID, id2 = elective_subject.ID, ErrorMessage = ViewBag.ErrorMessage });
                        }

                    }
                }
            }
            ViewBag.Notice = "Assign all students successfully";
            return RedirectToAction("Electives", new { id = batch.ID, id2 = elective_subject.ID, Notice = ViewBag.Notice });
        }

        // GET: Student/Edit/5
        public ActionResult Add_Additional_Details(string ErrorMessage, string Notice)
        {
            ViewBag.Notice = Notice;
            ViewBag.ErrorMessage = ErrorMessage;
            var additional_details = db.STUDENT_ADDITIONAL_FIELD.Where(x => x.STAT == true).OrderBy(x => x.NAME).ToList();
            ViewData["additional_details"] = additional_details;
            var inactive_additional_details = db.STUDENT_ADDITIONAL_FIELD.Where(x => x.STAT == false).OrderBy(x => x.NAME).ToList();
            ViewData["inactive_additional_details"] = inactive_additional_details;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Additional_Details([Bind(Include = "ID,NAME,STAT")] STUDENT_ADDITIONAL_FIELD aDDITIONALfIELD)
        {
            if (ModelState.IsValid)
            {
                var additional_details = db.STUDENT_ADDITIONAL_FIELD.Where(x => x.STAT == true).OrderBy(x => x.NAME).ToList();
                ViewData["additional_details"] = additional_details;
                var inactive_additional_details = db.STUDENT_ADDITIONAL_FIELD.Where(x => x.STAT == false).OrderBy(x => x.NAME).ToList();
                ViewData["inactive_additional_details"] = inactive_additional_details;

                db.STUDENT_ADDITIONAL_FIELD.Add(aDDITIONALfIELD);
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
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Addional Field added in system successfully!";
                return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Edit_Additional_Details(int? id)
        {
            STUDENT_ADDITIONAL_FIELD aDDITIONALfIELD = db.STUDENT_ADDITIONAL_FIELD.Find(id);

            return View(aDDITIONALfIELD);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Additional_Details([Bind(Include = "ID,NAME,STAT")] STUDENT_ADDITIONAL_FIELD aDDITIONALfIELD)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aDDITIONALfIELD).State = EntityState.Modified;
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
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Additional Field updated in system successfully!";
                return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });

            }
            ViewBag.ErrorMessage = "There seems to be some issue with Model State!";
            return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
        }

        public ActionResult Delete_Additional_Details(int? id)
        {
            STUDENT_ADDITIONAL_FIELD aDDITIONALfIELD = db.STUDENT_ADDITIONAL_FIELD.Find(id);
            var students = (from std in db.STUDENTs
                             join ad in db.STUDENT_ADDITIONAL_DETAIL on std.ID equals ad.STDNT_ID
                             join af in db.STUDENT_ADDITIONAL_FIELD on ad.ADDL_FLD_ID equals af.ID
                             where af.ID == id && std.STATE == "Y"
                             select std).Distinct();
            if (students == null || students.Count() == 0)
            {
                db.STUDENT_ADDITIONAL_FIELD.Remove(aDDITIONALfIELD);
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
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = string.Concat(ViewBag.ErrorMessage, "|", e.InnerException.InnerException.Message);
                    return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
                }
                ViewBag.Notice = "Additional Field deleted fom system successfully!";
                return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }
            else
            {
                ViewBag.Notice = "Addional Field cannot be deleted as active students are attached to this Additional Field.";
                return RedirectToAction("Add_Additional_Details", new { ErrorMessage = ViewBag.ErrorMessage, Notice = ViewBag.Notice });
            }

        }

        /////Document Upload related methods////////////////////////////////////////////////////////////////

        [HttpGet]
        public ActionResult Show(int? id)
        {
            string mime;
            byte[] bytes = LoadImage(id.Value, out mime);
            return File(bytes, mime);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            SuccessModel viewModel = new SuccessModel();
            if (Request.Files.Count == 1)
            {
                var name = Request.Files[0].FileName;
                var size = Request.Files[0].ContentLength;
                var type = Request.Files[0].ContentType;
                int PhotoId = 0;
                //viewModel.Success = HandleUpload(Request.Files[0].InputStream, name, size, type, PhotoId);
                PhotoId = HandleUpload(Request.Files[0].InputStream, name, size, type, PhotoId);
            }
            return Json(viewModel);
        }

        private int HandleUpload(Stream fileStream, string name, int size, string type, int PhotoId)
        {
            bool handled = false;

            try
            {
                // Convert image to buffered stream
                var imageBufferedStream = new BufferedStream(fileStream);
                byte[] documentBytes = new byte[imageBufferedStream.Length];
                imageBufferedStream.Read(documentBytes, 0, documentBytes.Length);


                if (PhotoId == 0 || PhotoId.Equals(null))
                {
                    IMAGE_DOCUMENTS databaseDocument = new IMAGE_DOCUMENTS
                    {
                        CREATEDON = DateTime.Now,
                        FILECONTENT = documentBytes,
                        ISDELETED = false,
                        NAME = name,
                        SIZE = size,
                        TYPE = type
                    };

                    db.IMAGE_DOCUMENTS.Add(databaseDocument);
                    handled = (db.SaveChanges() > 0);
                    PhotoId = databaseDocument.DOCUMENTID;
                }
                else
                {
                    IMAGE_DOCUMENTS ImagetoUpdate = db.IMAGE_DOCUMENTS.Find(PhotoId);
                    ImagetoUpdate.CREATEDON = DateTime.Now;
                    ImagetoUpdate.FILECONTENT = documentBytes;
                    ImagetoUpdate.NAME = name;
                    ImagetoUpdate.SIZE = size;
                    ImagetoUpdate.TYPE = type;

                    db.Entry(ImagetoUpdate).State = EntityState.Modified;
                    handled = (db.SaveChanges() > 0);
                }

            }
            catch (Exception ex)
            {
                throw ex; // Oops, something went wrong, handle the exception
            }

            return PhotoId;
        }

        private string Random_String(int SALTLength)
        {
            string allowedChars = "";
            allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowedChars += "1,2,3,4,5,6,7,8,9,0,!,@,#,$,%,&,?";
            char[] sep = { ',' };
            string[] arr = allowedChars.Split(sep);
            string SALT = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < SALTLength; i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                SALT += temp;
            }
            return SALT;
        }
        private byte[] LoadImage(int id, out string type)
        {
            byte[] fileBytes = null;
            string fileType = null;
            var databaseDocument = db.IMAGE_DOCUMENTS.FirstOrDefault(doc => doc.DOCUMENTID == id);
            if (databaseDocument != null)
            {
                fileBytes = databaseDocument.FILECONTENT;
                fileType = databaseDocument.TYPE;
            }
            type = fileType;
            return fileBytes;
        }

    }
}
