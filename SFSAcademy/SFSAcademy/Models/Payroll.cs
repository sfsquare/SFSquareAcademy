using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFSAcademy.Models
{
    public class PayrollCategory
    {
        public PAYROLL_CATEGORY PayrollCatData { get; set; }
    }

    public class EmployeePayroll
    {
        public PAYROLL_CATEGORY PayrollCatData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
        public EMPLOYEE_SALARY_STRUCTURE SalaryStructureData { get; set; }
        public int EmployeeId { get; set; }
        public int PayrollCategoryId { get; set; }
        public decimal Amount { get; set; }
    }

    public class EmployeeDependentPayroll
    {
        public PAYROLL_CATEGORY PayrollCatData { get; set; }
        public PAYROLL_CATEGORY DependentPayrollCatData { get; set; }
        public EMPLOYEE_SALARY_STRUCTURE SalaryStructureData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
        public int DependentEmployeeId { get; set; }
        public int DependentPayrollCategoryId { get; set; }
        public decimal DependentAmount { get; set; }
    }

    public class EmployeeMonthlyPayslip
    {
        public PAYROLL_CATEGORY PayrollCatData { get; set; }
        public MONTHLY_PAYSLIP MonthlyPayslipData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
    }
}