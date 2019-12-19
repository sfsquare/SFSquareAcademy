using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SFSAcademy
{
    public partial class BATCH_EVENT : IHasTimeStamp
    {
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