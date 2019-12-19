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
    [MetadataType(typeof(ReminderMetadata))]
    public partial class REMINDER : IHasTimeStamp
    {
        internal sealed class ReminderMetadata
        {
            [Required]
            public string BODY { get; set; }

        }
        public void DoTimeStamp(string EntityStateVal)
        {

            if (EntityStateVal.Equals("Added"))
            {
                CREATED_AT = DateTime.Now;
                UPDATED_AT = DateTime.Now;
            }

            if (EntityStateVal.Equals("Modified"))
            {
                UPDATED_AT = DateTime.Now;
            }
        }
    }
}