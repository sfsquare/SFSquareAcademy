﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFSAcademy.Models
{
    public class FeeCategory
    {
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
    }

    public class FeeMasterCategory
    {
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
    }

    public class SelectFeeCategory
    {
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public bool Selected { get; set; }
    }

    public class CreateParticularOption
    {
        public int Id { set; get; }
        public string Name { set; get; }
    }

    public class FeeParticular
    {
        public FINANCE_FEE_PARTICULAR FeeParticularData { get; set; }
        public FINANCE_FEE_CATGEORY FeeCategoryData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
    }

    public class FeeDiscount
    {
        public FEE_DISCOUNT FeeDiscountData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public FINANCE_FEE_PARTICULAR FinanceFeeParticularData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
    }

    public class FeeFine
    {
        public FEE_FINE FeeFineData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public FINANCE_FEE_PARTICULAR FinanceFeeParticularData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
    }
    public class FeeCollectionDiscount
    {
        public FEE_COLLECTION_DISCOUNT FeeCollectionDiscountData { get; set; }
        public FINANCE_FEE_COLLECTION FinanceFeeCollectionData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public FINANCE_FEE_PARTICULAR FinanceFeeParticularData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
    }

    public class BATCHList
    {
        public int ID { set; get; }
        public string NAME { set; get; }
    }

    public class FeeCollection
    {
        public FINANCE_FEE_COLLECTION FinanceFeeCollectionData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
    }

    public class FeeSubmission
    {
        public FINANCE_FEE_COLLECTION FinanceFeeCollectionData { get; set; }
        public FINANCE_FEE_CATGEORY FinanceFeeCategoryData { get; set; }
        public BATCH BatchData { get; set; }
        public STUDENT StudentData { get; set; }
        public FINANCE_TRANSACTION FinanceTransactionData { get; set; }
        public STUDENT_CATGEORY StudentCategoryData { get; set; }
        public FEE_DISCOUNT FeeDiscountData { get; set; }
        public FINANCE_FEE FinanceFeeData { get; set; }
        public FEE_COLLECTION_PARTICULAR FeeCollectionParticularData { get; set; }
        public FEE_COLLECTION_DISCOUNT FeeCollectionDiscountData { get; set; }
    }

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
    public class StundentFee
    {
        public STUDENT StudentData { get; set; }
        public FINANCE_FEE FinanceFeeData { get; set; }
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
    }

    public class FinanceTransaction
    {
        public FINANCE_TRANSACTION FinanceTransactionData { get; set; }
        public FINANCE_TRANSACTION_CATEGORY TransactionCategoryData { get; set; }
        public FINANCE_TRANSACTION_TRIGGERS TransactionTriggersData { get; set; }
    }

    public class FeeCollectionFee
    {
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
        public FINANCE_FEE FinanceFeeData { get; set; }
    }
    public enum Gender
    {
        Male,
        Female
    }
    public enum PaymentMode
    {
        Cash,
        Check,
        BankDeposite,
        Others
    }

    public enum FinCategoryIncomeTypes
    {
        Y,
        N
    }

    public class CategoryTransactions
    {
        public FINANCE_TRANSACTION_CATEGORY TransactionCategoryData { get; set; }
        public decimal? TRANS_AMNT { get; set; }
    }


    public class FeeCollectionTransactions
    {
        public FINANCE_FEE_COLLECTION FeeCollectionData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public decimal? TRANS_AMNT { get; set; }
    }

    public class TransactionTriggers
    {
        public FINANCE_TRANSACTION_TRIGGERS TransactionTriggerData { get; set; }
        public FINANCE_TRANSACTION_CATEGORY TransactionCategoryData { get; set; }
    }
}