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

    public class EmployeeAdditionalDetail
    {
        public EMPLOYEE_ADDITIONAL_DETAIL AdditionalDetailData { get; set; }
        public EMPLOYEE_ADDITIONAL_FIELD AdditionalFieldData { get; set; }
        public EMPLOYEE EmployeedData { get; set; }

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

