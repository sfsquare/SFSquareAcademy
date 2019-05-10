using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

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

    public partial class SFSAcademyEntities : DbContext
    {

        public override int SaveChanges()
        {
            var TimeStampchangeSet = ChangeTracker.Entries<IHasTimeStamp>();

            if (TimeStampchangeSet != null)
            {
                foreach (var entry in TimeStampchangeSet.Where(c => c.State != EntityState.Unchanged))
                {
                    (entry.Entity as IHasTimeStamp).DoTimeStamp(entry.State.ToString());
                }
            }

            var BeforeSavechangeSet = ChangeTracker.Entries<IHasBeforeSave>();

            if (BeforeSavechangeSet != null)
            {
                foreach (var entry in BeforeSavechangeSet.Where(c => c.State != EntityState.Unchanged))
                {
                    (entry.Entity as IHasBeforeSave).Before_Save();
                }
            }

            return base.SaveChanges();

        }
    }

    public class CommonModel
    {
        public Dictionary<string, string> STUDENT_ATTENDANCE_TYPE_OPTIONS { get; set; }
        public Dictionary<string, string> NETWORK_STATES { get; set; }
        public string[] LOCALES { get; set; }
    }
}