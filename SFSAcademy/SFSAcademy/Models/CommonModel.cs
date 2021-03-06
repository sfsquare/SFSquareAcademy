﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SFSAcademy
{
    public interface IHasTimeStamp
    {
        void DoTimeStamp(string EntityStateVal);
    }
    public interface IHasBeforeSave
    {
        void Before_Save();
    }
    public interface IHasBeforeDestroy
    {
        IEnumerable<ValidationResult> Before_Destroy(ValidationContext validationContext);
    }
    public interface IHasAfterCreate
    {
        void After_Create();
    }
    public interface IHasAfterUpdate
    {
        void After_Update();
    }

    public partial class SFSAcademyEntities : DbContext
    {
        //private int SaveChangeReturn { get; set; }
        public override int SaveChanges()
        {
            var TimeStampchangeSet = ChangeTracker.Entries<IHasTimeStamp>();

            if (TimeStampchangeSet != null)
            {
                foreach (var entry in TimeStampchangeSet.Where(c => c.State == EntityState.Added || c.State == EntityState.Modified))
                {
                    (entry.Entity as IHasTimeStamp).DoTimeStamp(entry.State.ToString());
                }
            }

            var BeforeSavechangeSet = ChangeTracker.Entries<IHasBeforeSave>();
            if (BeforeSavechangeSet != null)
            {
                foreach (var entry in BeforeSavechangeSet.Where(c => c.State == EntityState.Added || c.State == EntityState.Modified))
                {
                    (entry.Entity as IHasBeforeSave).Before_Save();
                }
            }
            var BeforeDestroychangeSet = ChangeTracker.Entries<IHasBeforeDestroy>();
            ValidationContext validationContext = new ValidationContext(this);
            if (BeforeDestroychangeSet != null)
            {
                foreach (var entry in BeforeDestroychangeSet.Where(c => c.State == EntityState.Deleted))
                {
                    (entry.Entity as IHasBeforeDestroy).Before_Destroy(validationContext);
                }
            }

            return base.SaveChanges();

            /*
            SaveChangeReturn = base.SaveChanges();

            var AfterCreatechangeSet = ChangeTracker.Entries<IHasAfterCreate>();
            if (AfterCreatechangeSet != null)
            {
                foreach (var entry in AfterCreatechangeSet.Where(c => c.State == EntityState.Unchanged))
                {
                    (entry.Entity as IHasAfterCreate).After_Create();
                }
            }

            var AfterUpdatechangeSet = ChangeTracker.Entries<IHasAfterUpdate>();
            if (AfterUpdatechangeSet != null)
            {
                foreach (var entry in AfterUpdatechangeSet.Where(c => c.State == EntityState.Unchanged))
                {
                    (entry.Entity as IHasAfterUpdate).After_Update();
                }
            }

            return SaveChangeReturn;
            */

        }

        public string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            if (model != null)
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

    public class CommonModel
    {
        public Dictionary<string, string> STUDENT_ATTENDANCE_TYPE_OPTIONS { get; set; }
        public Dictionary<string, string> NETWORK_STATES { get; set; }
        public string[] LOCALES { get; set; }
    }
}