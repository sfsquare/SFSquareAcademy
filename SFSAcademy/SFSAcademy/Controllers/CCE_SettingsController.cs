﻿using System;
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
    public class CCE_SettingsController : Controller
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        // GET: CCE_Settings
        public ActionResult Index()
        {
            return View(db.CCE_EXAM_CATEGORY.ToList());
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