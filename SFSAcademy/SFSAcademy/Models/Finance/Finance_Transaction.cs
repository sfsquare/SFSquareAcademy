using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SFSAcademy
{
    public class FeeTransaction
    {
        public STUDENT StudentData { get; set; }
        public FINANCE_TRANSACTION FinanceTransactionData { get; set; }
        public FINANCE_TRANSACTION_CATEGORY TransactionCategoryData { get; set; }
        public FINANCE_FEE FinanceFeeData { get; set; }
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
        public COURSE CourseData { get; set; }
        public BATCH BatchData { get; set; }
    }
    
    public class FinanceTransaction
    {
        public FINANCE_TRANSACTION FinanceTransactionData { get; set; }
        public FINANCE_TRANSACTION_CATEGORY TransactionCategoryData { get; set; }
        public FINANCE_TRANSACTION_TRIGGERS TransactionTriggersData { get; set; }
        public FINANCE_FEE FinanceFeeData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
    }
}