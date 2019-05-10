using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace SFSAcademy.Controllers
{
    public class ConfigurationController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: Configuration
        public ActionResult Index()
        {
            return View(db.CONFIGURATIONs.ToList());
        }

        // GET: Configuration/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Configuration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CONFIG_KEY,CONFIG_VAL")] CONFIGURATION cONFIGURATION)
        {
            if (ModelState.IsValid)
            {
                db.CONFIGURATIONs.Add(cONFIGURATION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cONFIGURATION);
        }

        // GET: Configuration/Create
        public ActionResult Settings()
        {
            var search = new string[] { "InstitutionName", "InstitutionAddress", "InstitutionPhoneNo", "StudentAttendanceType", "CurrencyType", "ExamResultType", "AdmissionNumberAutoIncrement", "EmployeeNumberAutoIncrement", "NetworkState", "Locale", "FinancialYearStartDate", "FinancialYearEndDate", "EnableNewsCommentModeration", "DefaultCountry", "TimeZone", "FirstTimeLoginEnable", "SchoolLogoId" };
            var config = db.CONFIGURATIONs.Where(a => search.Any(s => a.CONFIG_KEY.Contains(s))).Distinct();
            ViewData["config"] = config;
            //var grading_types = db.COURSEs.Where(x => x.IS_DEL == "N").Select(x => x.GRADING_TYPE).Distinct();
            var grading_types = (from cs in db.COURSEs
                                 where cs.IS_DEL == false
                                 select new SFSAcademy.GradingTypesSelect { GRADING_TYPE = cs.GRADING_TYPE, Select = false }).Distinct().ToList();
            ViewData["grading_types"] = grading_types;
            var search2 = new string[] { "GPA", "CWA","CCE" };
            var enabled_grading_types_val = db.CONFIGURATIONs.Where(a => search2.Any(s => a.CONFIG_KEY.Contains(s))).Distinct();
            var enabled_grading_types = enabled_grading_types_val.Where(x => x.CONFIG_VAL == "1").FirstOrDefault();
            ViewData["enabled_grading_types"] = enabled_grading_types;
            var countries = db.COUNTRies.ToList();
            ViewData["countries"] = countries;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Settings(IEnumerable<SFSAcademy.CONFIGURATION> config , IEnumerable<SFSAcademy.GradingTypesSelect> grading_types)
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

                foreach(var item in config)
                {
                    if(item.CONFIG_KEY == "SchoolLogoId")
                    {
                        item.CONFIG_VAL = PhotoId.ToString();
                    }
                }
                //CONFIg.SchoolLogoId = PhotoId;
            }
            ////End to Picture Upload Code
            foreach(var item in grading_types)
            {
                foreach (var item2 in config)
                {
                    if(item.GRADING_TYPE == item2.CONFIG_KEY)
                    {
                        item2.CONFIG_VAL = item.Select.ToString();
                    }
                }
            }
            foreach (var item in config)
            {
                if (item.CONFIG_KEY == "FinancialYearStartDate" && item.CONFIG_VAL != null)
                {
                    string Day_Month = string.Concat(Convert.ToDateTime(item.CONFIG_VAL).Day, "_", Convert.ToDateTime(item.CONFIG_VAL).Month);
                    item.CONFIG_VAL = Day_Month;
                }
                if (item.CONFIG_KEY == "FinancialYearEndDate" && item.CONFIG_VAL != null)
                {
                    string Day_Month = string.Concat(Convert.ToDateTime(item.CONFIG_VAL).Day, "_", Convert.ToDateTime(item.CONFIG_VAL).Month);
                    item.CONFIG_VAL = Day_Month;
                }
            }
            foreach (var item in config)
            {
                CONFIGURATION NewConfig = db.CONFIGURATIONs.Find(item.ID);
                NewConfig.CONFIG_VAL = item.CONFIG_VAL;
                db.Entry(NewConfig).State = EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.Notice = "Settings has been saved";

            var search = new string[] { "InstitutionName", "InstitutionAddress", "InstitutionPhoneNo", "StudentAttendanceType", "CurrencyType", "ExamResultType", "AdmissionNumberAutoIncrement", "EmployeeNumberAutoIncrement", "NetworkState", "Locale", "FinancialYearStartDate", "FinancialYearEndDate", "EnableNewsCommentModeration", "DefaultCountry", "TimeZone", "FirstTimeLoginEnable" };
            var config_Inner = db.CONFIGURATIONs.Where(a => search.Any(s => a.CONFIG_KEY.Contains(s))).Distinct();
            ViewData["config"] = config_Inner;
            //var grading_types = db.COURSEs.Where(x => x.IS_DEL == "N").Select(x => x.GRADING_TYPE).Distinct();
            var grading_types_Inner = (from cs in db.COURSEs
                                 where cs.IS_DEL == false
                                 select new SFSAcademy.GradingTypesSelect { GRADING_TYPE = cs.GRADING_TYPE, Select = false }).Distinct().ToList();
            ViewData["grading_types"] = grading_types_Inner;
            var search2 = new string[] { "GPA", "CWA", "CCE" };
            var enabled_grading_types_val = db.CONFIGURATIONs.Where(a => search2.Any(s => a.CONFIG_KEY.Contains(s))).Distinct();
            var enabled_grading_types = enabled_grading_types_val.Where(x => x.CONFIG_VAL == "1").FirstOrDefault();
            ViewData["enabled_grading_types"] = enabled_grading_types;
            var countries = db.COUNTRies.ToList();
            ViewData["countries"] = countries;

            return View();
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

    /////End of Document Upload related methods////////////////////////////////////////////////////////////////
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
