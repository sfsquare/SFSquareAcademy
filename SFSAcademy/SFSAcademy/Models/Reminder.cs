using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace SFSAcademy
{
    public enum ReminderOptions
    {
        [Display(Name = "Delete")]
        delete,
        [Display(Name = "Mark as unread")]
        unread,
        [Display(Name = "Mark as read")]
        read
    }

    public class ReminderSelect
    {
        public REMINDER ReminderData { get; set; }
        public bool Selected { get; set; }
    }

}