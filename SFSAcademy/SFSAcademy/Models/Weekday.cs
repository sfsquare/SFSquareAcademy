using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace SFSAcademy
{
    public partial class WEEKDAY
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();

        public IEnumerable<WEEKDAY> Weekday_By_Day(int? batch_id)
        {

            var weekdays = db.WEEKDAYs.Where(x => x.BTCH_ID == batch_id).ToList();
            if(weekdays == null)
            {
                weekdays = db.WEEKDAYs.Where(x => x.ID == -1).DefaultIfEmpty().ToList();
            }
            return weekdays;
        }
    }
}