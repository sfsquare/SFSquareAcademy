using iTextSharp.xmp.impl;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SFSAcademy.Models
{
    public class Employee
    {
        public EMPLOYEE EmployeeData { get; set; }
        public EMPLOYEE_CATEGORY CategoryData { get; set; }
        public EMPLOYEE_DEPARTMENT DepartmentData { get; set; }
        public EMPLOYEE_POSITION PositionData { get; set; }
        public EMPLOYEE_GRADE GradeData { get; set; }
        public COUNTRY NationalityData { get; set; }
        public int Employee_Id { get; set; }
        public int Reporting_Manager_Id { get; set; }
        public DateTime? Salary_date { get; set; }
    }

    public class EmployeeCategory
    {
        public EMPLOYEE_CATEGORY CategoryData { get; set; }
    }

    public class EmployeePosition
    {
        public EMPLOYEE_POSITION PositionData { get; set; }
        public EMPLOYEE_CATEGORY CategoryData { get; set; }
    }
    public class EmployeeDepartment
    {
        public EMPLOYEE_DEPARTMENT DepartmentData { get; set; }
    }
    public class EmployeeGrade
    {
        public EMPLOYEE_GRADE GradeData { get; set; }
    }

    public class EmployeeBankDetail
    {
        public EMPLOYEE_BANK_DETAIL BankDetailData { get; set; }
        public BANK_FIELD BankFieldData { get; set; }
        public EMPLOYEE EmployeedData { get; set; }

    }

    public class EmployeeBankFieldValue
    {
        public int? EMPLOYEE_ID { get; set; }
        public int? BANK_FIELD_ID { get; set; }
        public string FIELD_VALUE { get; set; }
        public BANK_FIELD BankFieldData { get; set; }
    }

    public class EmployeeAdditionalDetail
    {
        public EMPLOYEE_ADDITIONAL_DETAIL AdditionalDetailData { get; set; }
        public EMPLOYEE_ADDITIONAL_FIELD AdditionalFieldData { get; set; }
        public EMPLOYEE EmployeedData { get; set; }

    }

    public class EmployeeAdditionalDetailValue
    {
        public int? EMPLOYEE_ID { get; set; }
        public int? ADDITIONAL_DETAIL_ID { get; set; }
        public string ADDITIONAL_DETAIL_VALUE { get; set; }
        public EMPLOYEE_ADDITIONAL_FIELD AdditionalFieldData { get; set; }

    }

    public class FinanceManager
    {
        public EMPLOYEE EmployeeData { get; set; }
        public EMPLOYEE_DEPARTMENT EmpDepartmentData { get; set; }
        public PRIVILEGES_USERS PrivilegeUsersData { get; set; }
        public PRIVILEGE PrivilegeData { get; set; }

    }

    public class Payslip
    {
        public EMPLOYEE EmployeeData { get; set; }
        public MONTHLY_PAYSLIP MonthlyPayslipData { get; set; }
        public INDIVIDUAL_PAYSLIP_CATGEORY IndPayCatData { get; set; }
        public PAYROLL_CATEGORY PayrollCatogaryData { get; set; }
        public decimal? Monthy_Payslip_Amount { get; set; }
        public decimal? Individual_Pyaslip_Amount { get; set; }
        public decimal? Net_Amount { get; set; }
        public string Status { get; set; }
        public decimal? Aapproved_Amount { get; set; }
        public decimal? Net_Non_Deductionable_Amount { get; set; }
        public decimal? Net_Deductionable_Amount { get; set; }
    }

    public class MonthyPayslip
    {
        public EMPLOYEE EmployeeData { get; set; }
        public MONTHLY_PAYSLIP MonthlyPayslipData { get; set; }
        public PAYROLL_CATEGORY PayrollCatogaryData { get; set; }
    }

    public class LeaveReset
    {
        public bool automatic_leave_reset { get; set; }
        public string leave_reset_period { get; set; }
        public string last_reset_date { get; set; }
        public string financial_year_start_date { get; set; }
    }

    public class EmployeeLeaveReset
    {
        public EMPLOYEE EmployeeData { get; set; }
        public bool Selected { get; set; }
    }


    public enum Priority
    {
        [Display(Name = "1")]
        High,
        [Display(Name = "2")]
        Medium,
        [Display(Name = "3")]
        Simple
    }
}

