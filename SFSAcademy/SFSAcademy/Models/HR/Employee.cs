using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace SFSAcademy
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
        public EMPLOYEE EmployeeData { get; set; }
        public int? EMPLOYEE_ID { get; set; }
        public int? BANK_FIELD_ID { get; set; }
        public string FIELD_VALUE { get; set; }

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
        public EMPLOYEE EmployeeData { get; set; }

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
    public class CalulatedSalary
    {
        public decimal? net_amount { get; set; }
        public decimal? net_deductionable_amount { get; set; }
        public decimal? net_non_deductionable_amount { get; set; }
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
    public enum Marital_Status
    {
        [Display(Name = "Single")]
        Single,
        [Display(Name = "Married")]
        Married,
        [Display(Name = "Divorced")]
        Divorced
    }

    [MetadataType(typeof(EmployeeMetadata))]
    public partial class EMPLOYEE : IHasTimeStamp, IHasBeforeSave
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class EmployeeMetadata
        {
            [Required]
            public string EMP_NUM { get; set; }
            [Required]
            public string FIRST_NAME { get; set; }
            [Required]
            public string GNDR { get; set; }

            [Required]
            public int? EMP_CAT_ID { get; set; }

            [Required(ErrorMessage = "Enter Joining Date.")]
            public DateTime? JOINING_DATE { get; set; }

            [Required(ErrorMessage = "Date of birth is required.")]
            public DateTime? DOB { get; set; }

            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string EML { get; set; }

            [Required]
            public int? EMP_POS_ID { get; set; }
            [Required]
            public int? EMP_DEPT_ID { get; set; }

        }
        public void DoTimeStamp(string EntityStateVal)
        {

            if (EntityStateVal.Equals("Added"))
            {
                //add creation date_time            
                CREATED_AT = DateTime.Now;
                UPDATED_AT = DateTime.Now;
            }

            if (EntityStateVal.Equals("Modified"))
            {
                //update Updation time            
                UPDATED_AT = DateTime.Now;
            }
        }

        public void Before_Save()
        {
            STAT = true;
        }
        public void Status_True()
        {
            if(STAT != true)
            {
                STAT = true;
            }
        }
        public IEnumerable<BATCH> Employee_Batches()
        {
            var batches_with_employees = db.BATCHes.Where(x => x.IS_ACT == true && x.EMP_ID != null).ToList();
            var assigned_batches = batches_with_employees.Where(x=> HtmlHelpers.ApplicationHelper.SplitCommaString(x.EMP_ID).Contains(ID.ToString())).ToList();
            return (IEnumerable<BATCH>)assigned_batches;
        }
        public decimal Max_Hours_Per_Day()
        {
            decimal? MaxHours = EMPLOYEE_GRADE.MAX_DILY_HRS == null ? 0 : EMPLOYEE_GRADE.MAX_DILY_HRS;
            return (decimal)MaxHours;
        }
        public decimal Max_Hours_Per_Week()
        {
            decimal? MaxHoursWeek = EMPLOYEE_GRADE.MAX_WKILY_HRS == null ? 0 : EMPLOYEE_GRADE.MAX_WKILY_HRS;
            return (decimal)MaxHoursWeek;
        }
        public EMPLOYEE Next_Employee()
        {
            var emp = db.EMPLOYEEs.Include(x => x.EMPLOYEE_DEPARTMENT).Where(x => x.EMPLOYEE_DEPARTMENT.ID == EMP_DEPT_ID && x.ID > ID).OrderBy(x => x.ID).ToList();
            return emp.FirstOrDefault();
        }
        public EMPLOYEE Previous_Employee()
        {
            var emp = db.EMPLOYEEs.Include(x => x.EMPLOYEE_DEPARTMENT).Where(x => x.EMPLOYEE_DEPARTMENT.ID == EMP_DEPT_ID && x.ID < ID).OrderByDescending(x => x.ID).ToList();
            return emp.FirstOrDefault();
        }
        public string Full_Name
        {
            get { return string.Concat(FIRST_NAME, " ", MID_NAME, " ", LAST_NAME); }
        }
        public bool Is_Payslip_Approved(DateTime? date)
        {
            var approve = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == ID && x.SAL_DATE == date && x.IS_APPR == true).ToList();
            if(approve != null && approve.Count() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Is_Payslip_Rejected(DateTime? date)
        {
            var rejected = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == ID && x.SAL_DATE == date && x.IS_RJCT == true).ToList();
            if (rejected != null && rejected.Count() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public IEnumerable<MONTHLY_PAYSLIP> All_Salaries(DateTime? start_date, DateTime? end_date)
        {
            return db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == ID && x.SAL_DATE >= start_date && x.SAL_DATE <= end_date && x.IS_APPR == true).OrderByDescending(x=>x.SAL_DATE).ToList();

        }
        public decimal Employee_Salary(DateTime? salary_date)
        {
            var monthly_payslips = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == ID && x.SAL_DATE == salary_date && x.IS_APPR == true).OrderByDescending(x => x.SAL_DATE).ToList();
            var individual_payslip_category = db.INDIVIDUAL_PAYSLIP_CATGEORY.Where(x => x.EMP_ID == ID && x.SAL_DATE == salary_date).OrderByDescending(x => x.SAL_DATE).ToList();
            decimal? individual_category_non_deductionable = 0;
            decimal? individual_category_deductionable = 0;
            foreach(var pc in individual_payslip_category)
            {
                if(pc.IS_DED == false)
                {
                    individual_category_non_deductionable = individual_category_non_deductionable + (decimal)pc.AMT;
                }
            }
            foreach (var pc in individual_payslip_category)
            {
                if (pc.IS_DED == true)
                {
                    individual_category_deductionable = individual_category_deductionable + (decimal)pc.AMT;
                }
            }
            decimal? non_deductionable_amount = 0;
            decimal? deductionable_amount = 0;
            foreach(var mp in monthly_payslips)
            {
                var category1 = db.PAYROLL_CATEGORY.Find(mp.PYRL_CAT_ID);
                if(category1.IS_DED == false)
                {
                    non_deductionable_amount = non_deductionable_amount + (decimal)mp.AMT;
                }
            }
            foreach (var mp in monthly_payslips)
            {
                var category2 = db.PAYROLL_CATEGORY.Find(mp.PYRL_CAT_ID);
                if (category2.IS_DED == true)
                {
                    deductionable_amount = deductionable_amount + (decimal)mp.AMT;
                }
            }
            decimal? net_non_deductionable_amount = individual_category_non_deductionable + non_deductionable_amount;
            decimal? net_deductionable_amount = individual_category_deductionable + deductionable_amount;
            decimal? net_amount = net_non_deductionable_amount - net_deductionable_amount;

            return (decimal)net_amount;

        }
        public decimal Total_Employees_Salary(IEnumerable<EMPLOYEE> employees, DateTime? start_date,  DateTime? end_date)
        {
            decimal? salary = 0;
            foreach(var e in employees)
            {
                var salary_dates = e.All_Salaries(start_date, end_date);
                foreach(var s in salary_dates)
                {
                    salary += e.Employee_Salary(s.SAL_DATE);
                }
            }
            return (decimal)salary;
        }
        public IEnumerable<MONTHLY_PAYSLIP> Salary(DateTime? start_date, DateTime? end_date)
        {
            return db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == ID && x.SAL_DATE >= start_date && x.SAL_DATE <= end_date && x.IS_APPR == true).OrderByDescending(x=>x.SAL_DATE).ToList();

        }
        public bool Archive_Employee(bool status)
        {
            var aRCHIVEDsTD = new ARCHIVED_EMPLOYEE() { RPTG_MGR_ID = RPTG_MGR_ID, EMP_CAT_ID = EMP_CAT_ID, EMP_NUM = EMP_NUM, JOINING_DATE = JOINING_DATE, FIRST_NAME = FIRST_NAME, MID_NAME = MID_NAME, LAST_NAME = LAST_NAME, GNDR = GNDR, JOB_TIL = JOB_TIL, EMP_POS_ID = EMP_POS_ID, EMP_DEPT_ID = EMP_DEPT_ID, EMP_GRADE_ID = EMP_GRADE_ID, QUAL = QUAL, EXPNC_DETL = EXPNC_DETL, EXPNC_YEAR = EXPNC_YEAR, EXPNC_MONTH = EXPNC_MONTH, STAT = STAT, STAT_DESCR = STAT_DESCR, DOB = DOB, MARITAL_STAT = MARITAL_STAT, CHLD_CNT = CHLD_CNT, FTHR_NAME = FTHR_NAME, MTHR_NAME = MTHR_NAME, HUSBND_NAME = HUSBND_NAME, BLOOD_GRP = BLOOD_GRP, NTLTY_ID = NTLTY_ID, HOME_ADDR_LINE1 = HOME_ADDR_LINE1, HOME_ADDR_LINE2 = HOME_ADDR_LINE2, HOME_CITY = HOME_CITY, HOME_STATE = HOME_STATE, HOME_CTRY_ID = HOME_CTRY_ID, HOME_PIN_CODE = HOME_PIN_CODE, OFF_ADDR_LINE1 = OFF_ADDR_LINE1, OFF_ADDR_LINE2 = OFF_ADDR_LINE2, OFF_CITY = OFF_CITY, OFF_STATE = OFF_STATE, OFF_CTRY_ID = OFF_CTRY_ID, OFF_PIN_CODE = OFF_PIN_CODE, OFF_PH1 = OFF_PH1, OFF_PH2 = OFF_PH2, MOBL_PH = MOBL_PH, HOME_PH = HOME_PH, EML = EML, FAX = FAX, CREATED_AT = System.DateTime.Now, UPDATED_AT = System.DateTime.Now, USRID = USRID, IMAGE_DOCUMENTS_ID = IMAGE_DOCUMENTS_ID, LIBRARY_CARD = LIBRARY_CARD, FRMR_ID = ID.ToString() };
            db.ARCHIVED_EMPLOYEE.Add(aRCHIVEDsTD);
            try { db.SaveChanges();
                var employee_salary_structures = db.EMPLOYEE_SALARY_STRUCTURE.Where(x => x.EMP_ID == ID).ToList();
                var employee_bank_details = db.EMPLOYEE_BANK_DETAIL.Where(x => x.EMP_ID == ID).ToList();
                var employee_additional_details = db.EMPLOYEE_ADDITIONAL_DETAIL.Where(x => x.EMP_ID == ID).ToList();
                var employee_atterndences = db.EMPLOYEE_ATTENDENCES.Where(x => x.EMP_ID == ID).ToList();
                foreach (var item in employee_salary_structures)
                {
                    var aRCHIVEDsSalStr = new ARCHIVED_EMPLOYEE_SALARY_STRUCTURE() { AMT = item.AMT, EMP_ID = aRCHIVEDsTD.ID, PYRL_CAT_ID = item.PYRL_CAT_ID };
                    db.ARCHIVED_EMPLOYEE_SALARY_STRUCTURE.Add(aRCHIVEDsSalStr);
                    db.EMPLOYEE_SALARY_STRUCTURE.Remove(item);
                }
                foreach (var item in employee_bank_details)
                {
                    var aRCHIVEDsBankDetl = new ARCHIVED_EMPLOYEE_BANK_DETAIL() { BANK_INFO = item.BANK_INFO, EMP_ID = aRCHIVEDsTD.ID, BANK_FLD_ID = item.BANK_FLD_ID };
                    db.ARCHIVED_EMPLOYEE_BANK_DETAIL.Add(aRCHIVEDsBankDetl);
                    db.EMPLOYEE_BANK_DETAIL.Remove(item);
                }
                foreach (var item in employee_additional_details)
                {
                    var aRCHIVEDsEmpAdDetl = new ARCHIVED_EMPLOYEE_ADDITIONAL_DETAIL() { ADDL_FLD_ID = item.ADDL_FLD_ID, EMP_ID = aRCHIVEDsTD.ID, ADDL_INFO = item.ADDL_INFO };
                    db.ARCHIVED_EMPLOYEE_ADDITIONAL_DETAIL.Add(aRCHIVEDsEmpAdDetl);
                    db.EMPLOYEE_ADDITIONAL_DETAIL.Remove(item);
                }
                foreach (var item in employee_atterndences)
                {
                    var aRCHIVEDsEmpAttend = new ARCHIVED_EMPLOYEE_ATTENDENCES() { FRMR_ID = item.ID, ATNDENCE_DATE = item.ATNDENCE_DATE, EMP_ID = aRCHIVEDsTD.ID, EMP_LEAVE_TYPE_ID = item.EMP_LEAVE_TYPE_ID, RSN = item.RSN, IS_HALF_DAY = item.IS_HALF_DAY };
                    db.ARCHIVED_EMPLOYEE_ATTENDENCES.Add(aRCHIVEDsEmpAttend);
                    db.EMPLOYEE_ATTENDENCES.Remove(item);
                }
                if (USRID != null)
                {
                    USER user = db.USERS.Find(USRID);
                    user.IS_DEL = true;
                    db.Entry(user).State = EntityState.Modified;
                }
                var employee_leave = db.EMPLOYEE_LEAVE.Where(x => x.EMP_ID == ID).ToList();
                foreach (var item in employee_leave)
                {
                    db.EMPLOYEE_LEAVE.Remove(item);
                }
                var employee_payslips = db.MONTHLY_PAYSLIP.Where(x => x.EMP_ID == ID).ToList();
                foreach (var item in employee_payslips)
                {
                    db.MONTHLY_PAYSLIP.Remove(item);
                }
                var employee_timetableentry = db.TIMETABLE_ENTRY.Where(x => x.EMP_ID == ID).ToList();
                foreach (var item in employee_timetableentry)
                {
                    item.EMP_ID = null;
                    db.Entry(item).State = EntityState.Modified;
                }
                EMPLOYEE emp = db.EMPLOYEEs.Find(ID);
                emp.STAT = status;
                db.Entry(emp).State = EntityState.Modified;
                try { db.SaveChanges(); }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { Console.WriteLine(e); } }
                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e); return false;
                }
                return true; }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors) { foreach (var ve in eve.ValidationErrors) { Console.WriteLine(e); } }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e); return false;
            }

        }
        public SFSAcademy.CalulatedSalary Calculate_Salary(IEnumerable<MONTHLY_PAYSLIP> monthly_payslip, IEnumerable<INDIVIDUAL_PAYSLIP_CATGEORY> individual_payslip_category)
        {
            decimal? individual_category_non_deductionable = 0;
            decimal? individual_category_deductionable = 0;
            if(individual_payslip_category != null && individual_payslip_category.Count() != 0)
            {
                foreach(var pc in individual_payslip_category)
                {
                    if (pc.IS_DED == true)
                    {
                        individual_category_deductionable = individual_category_deductionable + (decimal)pc.AMT;
                    }
                    else
                    {
                        individual_category_non_deductionable = individual_category_non_deductionable + (decimal)pc.AMT;
                    }
                }
            }
            decimal? non_deductionable_amount = 0;
            decimal? deductionable_amount = 0;
            if (monthly_payslip != null && monthly_payslip.Count() != 0)
            {
                foreach (var mp in monthly_payslip)
                {
                    if(mp.PAYROLL_CATEGORY != null)
                    {
                        if (mp.PAYROLL_CATEGORY.IS_DED == true)
                        {
                            deductionable_amount = deductionable_amount + (decimal)mp.AMT;
                        }
                        else
                        {
                            non_deductionable_amount = non_deductionable_amount + (decimal)mp.AMT;
                        }
                    }
                }
            }

            decimal? net_non_deductionable_amount = individual_category_non_deductionable + non_deductionable_amount;
            decimal? net_deductionable_amount = individual_category_deductionable + deductionable_amount;
            decimal? net_amount = net_non_deductionable_amount - net_deductionable_amount;

            CalulatedSalary cs = new CalulatedSalary() { net_amount = net_amount, net_deductionable_amount = net_deductionable_amount, net_non_deductionable_amount = net_non_deductionable_amount };

            return cs;

        }
    }
 }

