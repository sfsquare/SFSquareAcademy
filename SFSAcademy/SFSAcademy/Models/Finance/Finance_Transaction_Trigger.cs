using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SFSAcademy
{

    public class TransactionTriggers
    {
        public FINANCE_TRANSACTION_TRIGGERS TransactionTriggerData { get; set; }
        public FINANCE_TRANSACTION_CATEGORY TransactionCategoryData { get; set; }
    }

}