using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using PagedList;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Text;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace SFSAcademy
{
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
        public DateTime? SAL_DATE { get; set; }
    }

    public class MonthyPayslip
    {
        public EMPLOYEE EmployeeData { get; set; }
        public MONTHLY_PAYSLIP MonthlyPayslipData { get; set; }
        public PAYROLL_CATEGORY PayrollCatogaryData { get; set; }
    }
    public class TotalSalary
    {
        public EMPLOYEE EmployeeData { get; set; }
        public MONTHLY_PAYSLIP MonthlyPayslipData { get; set; }
        public PAYROLL_CATEGORY PayrollCatogaryData { get; set; }
        public INDIVIDUAL_PAYSLIP_CATGEORY IndividualPayrollCategoryData { get; set; }
        public decimal? TotalSalaryAmount { get; set; }
    }
    public partial class MONTHLY_PAYSLIP : IValidatableObject
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SAL_DATE == null)
            {
                //yield return new ValidationResult($"Classic movies must have a release year earlier than {_classicYear}.", new[] { "ReleaseDate" });
                yield return new ValidationResult($"*", new[] { "Salary Date can not be null." });
            }
        }

        public bool Approve(int? user_id, string remark)
        {
            MONTHLY_PAYSLIP mp = db.MONTHLY_PAYSLIP.Where(x => x.ID == ID).FirstOrDefault();
            mp.IS_RJCT = false;
            mp.RJCT_ID = null;
            mp.RSN = null;
            mp.IS_APPR = true;
            mp.APRV_ID = user_id;
            mp.RMRK = remark;
            db.Entry(mp).State = EntityState.Modified;
            try { db.SaveChanges(); return true; }
            catch (Exception e) { Console.WriteLine(e); return false; }
        }
        public bool Reject(int? user_id, string reason)
        {
            MONTHLY_PAYSLIP mp = db.MONTHLY_PAYSLIP.Where(x => x.ID == ID).FirstOrDefault();
            mp.IS_APPR = false;
            mp.APRV_ID = null;
            mp.RMRK = null;
            mp.IS_RJCT = true;
            mp.RJCT_ID = user_id;
            mp.RSN = reason;
            db.Entry(mp).State = EntityState.Modified;
            try { db.SaveChanges(); return true; }
            catch (Exception e) { Console.WriteLine(e); return false; }
        }
        public IEnumerable<SFSAcademy.Payslip> Find_And_Filter_By_Department(DateTime? salary_date, int? dept_id)
        {
            var payslips = db.MONTHLY_PAYSLIP.Where(x=>x.ID == -1).DefaultIfEmpty().ToList();
            var individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.ID == -1).DefaultIfEmpty().ToList();
            if (dept_id != -1)
            {
                var active_employees_in_dept = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == dept_id).ToList();
                var archived_employees_in_dept = db.ARCHIVED_EMPLOYEE.Where(x => x.EMP_DEPT_ID == dept_id).ToList();
                List<int> all_employees_in_dept = new List<int>();
                foreach(var item in active_employees_in_dept)
                {
                    all_employees_in_dept.Add(item.ID);
                }
                foreach (var item in archived_employees_in_dept)
                {
                    all_employees_in_dept.Add(Convert.ToInt32(item.FRMR_ID));
                }
                payslips = db.MONTHLY_PAYSLIP.Include(x=>x.PAYROLL_CATEGORY).Where(x => x.SAL_DATE == salary_date && all_employees_in_dept.Contains((int)x.EMP_ID)).OrderBy(x=>x.PYRL_CAT_ID).ToList();
                individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.SAL_DATE == salary_date && all_employees_in_dept.Contains((int)x.EMP_ID)).OrderBy(x=>x.ID).ToList();
            }
            else
            {
                payslips = db.MONTHLY_PAYSLIP.Where(x => x.SAL_DATE == salary_date && x.EMP_ID == ID).OrderBy(x => x.PYRL_CAT_ID).ToList();
                individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.SAL_DATE == salary_date && x.EMP_ID == ID).OrderBy(x => x.ID).ToList();
            }
            var grouped_monthly_payslips = from mps in payslips
                                           group mps by mps.EMP_ID into g
                                           select new
                                           {
                                               Employee_ID = g.Key,
                                               Status = (from ps2 in g select ps2.IS_APPR).FirstOrDefault() == true ? "Approved" : ((from ps2 in g select ps2.IS_RJCT).FirstOrDefault() == true ? "Rejected" : null),
                                               Monthy_Payslip_Amount = g.Sum(x => x.PAYROLL_CATEGORY.IS_DED == false ? x.AMT : -x.AMT),
                                               Non_Deductionable_Amount = g.Sum(x => x.PAYROLL_CATEGORY.IS_DED == false ? x.AMT : 0),
                                               Deductionable_Amount = g.Sum(x => x.PAYROLL_CATEGORY.IS_DED == true ? x.AMT : 0)
                                           };
            var grouped_individual_payslip_categories = from ps in individual_payslip_category
                                                        group ps by ps.EMP_ID into g
                                                        select new
                                                        {
                                                            Employee_ID = g.Key,
                                                            Individual_Pyaslip_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : -x.AMT),
                                                            Non_Deductionable_Amount = g.Sum(x => x.IS_DED == false ? x.AMT : 0),
                                                            Deductionable_Amount = g.Sum(x => x.IS_DED == true ? x.AMT : 0)
                                                        };
            var hash = (from  gmp in grouped_monthly_payslips
                            join gipc in grouped_individual_payslip_categories on gmp.Employee_ID equals gipc.Employee_ID into ggipc
                            from subgipc in ggipc.DefaultIfEmpty()
                            select new SFSAcademy.Payslip { Monthy_Payslip_Amount = gmp.Monthy_Payslip_Amount, Status = gmp.Status, Individual_Pyaslip_Amount = (subgipc == null ? null : subgipc.Individual_Pyaslip_Amount), Net_Amount = (subgipc == null ? gmp.Monthy_Payslip_Amount : gmp.Monthy_Payslip_Amount + subgipc.Individual_Pyaslip_Amount), Net_Non_Deductionable_Amount = (subgipc == null ? gmp.Non_Deductionable_Amount : gmp.Non_Deductionable_Amount + subgipc.Non_Deductionable_Amount), Net_Deductionable_Amount = (subgipc == null ? gmp.Deductionable_Amount : gmp.Deductionable_Amount + subgipc.Deductionable_Amount) }).Distinct();

            return hash;
        }

        public string Status_As_Text()
        {
            if(IS_APPR)
            {
                return "Approved";
            }
            else if(IS_RJCT)
            {
                return "Rejected";
            }
            else
            {
                return "Pending";
            }
        }
        public IEnumerable<SFSAcademy.TotalSalary> Total_Employees_Salary(DateTime? start_date, DateTime? end_date, int? dept_id)
        {
            var total_monthly_payslips = db.MONTHLY_PAYSLIP.Where(x => x.ID == -1).DefaultIfEmpty().ToList();

            if (dept_id != -1 && dept_id != null)
            {
                var active_employees_in_dept = db.EMPLOYEEs.Where(x => x.EMP_DEPT_ID == dept_id).ToList();
                var archived_employees_in_dept = db.ARCHIVED_EMPLOYEE.Where(x => x.EMP_DEPT_ID == dept_id).ToList();
                List<int> all_employees_in_dept = new List<int>();
                foreach (var item in active_employees_in_dept)
                {
                    all_employees_in_dept.Add(item.ID);
                }
                foreach (var item in archived_employees_in_dept)
                {
                    all_employees_in_dept.Add(Convert.ToInt32(item.FRMR_ID));
                }
                total_monthly_payslips = db.MONTHLY_PAYSLIP.Include(x => x.PAYROLL_CATEGORY).Where(x => x.SAL_DATE >= start_date && x.SAL_DATE <= end_date && x.IS_APPR == true && x.RMRK != "" && all_employees_in_dept.Contains((int)x.EMP_ID)).OrderByDescending(x => x.SAL_DATE).ToList();                
            }
            else
            {
                total_monthly_payslips = db.MONTHLY_PAYSLIP.Include(x => x.PAYROLL_CATEGORY).Where(x => x.SAL_DATE >= start_date && x.SAL_DATE <= end_date && x.IS_APPR == true && x.RMRK != "" && x.EMP_ID == this.ID).OrderByDescending(x => x.SAL_DATE).ToList();
            }
            List<int> employee_ids = new List<int>();
            foreach(var item in total_monthly_payslips)
            {
                employee_ids.Add((int)item.EMP_ID);
            }
            var total_individual_payslips = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.ID == -1).DefaultIfEmpty().ToList();
            if (employee_ids != null && employee_ids.Count() != 0)
            {
                employee_ids.Distinct();
                total_individual_payslips = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.SAL_DATE >= start_date && x.SAL_DATE <= end_date && employee_ids.Contains((int)x.EMP_ID)).OrderBy(x => x.ID).ToList();
            }
            decimal? total_salary = 0;
            if(total_monthly_payslips != null && total_monthly_payslips.Count() != 0 && total_monthly_payslips.ElementAt(0) != null)
            {
                foreach(var item in total_monthly_payslips)
                {
                    if(item.PAYROLL_CATEGORY.IS_DED == false)
                    {
                        total_salary += (decimal)item.AMT;
                    }
                    if (item.PAYROLL_CATEGORY.IS_DED == true)
                    {
                        total_salary -= (decimal)item.AMT;
                    }
                }
            }
            if (total_individual_payslips != null && total_individual_payslips.Count() != 0 && total_individual_payslips.ElementAt(0) != null)
            {
                foreach (var item in total_individual_payslips)
                {
                    if (item.IS_DED == false)
                    {
                        total_salary += (decimal)item.AMT;
                    }
                    if (item.IS_DED == true)
                    {
                        total_salary -= (decimal)item.AMT;
                    }
                }
            }
            var hash = (from mp in total_monthly_payslips
                        select new SFSAcademy.TotalSalary { MonthlyPayslipData = mp, IndividualPayrollCategoryData = null, TotalSalaryAmount = total_salary }).Distinct();

            if (total_individual_payslips != null && total_individual_payslips.Count() != 0 && total_individual_payslips.ElementAt(0) != null)
            {
                hash = (from mp in total_monthly_payslips
                        join ipc in total_individual_payslips on mp.EMP_ID equals ipc.EMP_ID into ggipc
                        from subgipc in ggipc.DefaultIfEmpty()
                        select new SFSAcademy.TotalSalary { MonthlyPayslipData = mp, IndividualPayrollCategoryData = (subgipc == null ? null : subgipc), TotalSalaryAmount = total_salary }).Distinct();
            }
            return hash;
        }

        public ActiveOrArchiveEmployee Active_Or_Archived_Employee()
        {
            EMPLOYEE employee = db.EMPLOYEEs.Find(this.EMP_ID);
            ARCHIVED_EMPLOYEE archived_employee = db.ARCHIVED_EMPLOYEE.Find(-1);
            if(employee == null)
            {
                archived_employee = db.ARCHIVED_EMPLOYEE.Where(x => Convert.ToInt32(x.FRMR_ID) == this.EMP_ID).FirstOrDefault();
            }

            ActiveOrArchiveEmployee AcArEmp = new ActiveOrArchiveEmployee();
            AcArEmp.Employee = employee;
            AcArEmp.ArchivedEmployee = archived_employee;
            return AcArEmp;
        }
    }
}

