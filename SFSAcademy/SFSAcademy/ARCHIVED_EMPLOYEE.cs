//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SFSAcademy
{
    using System;
    using System.Collections.Generic;
    
    public partial class ARCHIVED_EMPLOYEE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ARCHIVED_EMPLOYEE()
        {
            this.ARCHIVED_EMPLOYEE_SALARY_STRUCTURE = new HashSet<ARCHIVED_EMPLOYEE_SALARY_STRUCTURE>();
            this.ARCHIVED_EMPLOYEE_ADDITIONAL_DETAIL = new HashSet<ARCHIVED_EMPLOYEE_ADDITIONAL_DETAIL>();
            this.ARCHIVED_EMPLOYEE_BANK_DETAIL = new HashSet<ARCHIVED_EMPLOYEE_BANK_DETAIL>();
        }
    
        public int ID { get; set; }
        public Nullable<int> RPTG_MGR_ID { get; set; }
        public Nullable<int> EMP_CAT_ID { get; set; }
        public string EMP_NUM { get; set; }
        public Nullable<System.DateTime> JOINING_DATE { get; set; }
        public string FIRST_NAME { get; set; }
        public string MID_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string GNDR { get; set; }
        public string JOB_TIL { get; set; }
        public Nullable<int> EMP_POS_ID { get; set; }
        public Nullable<int> EMP_DEPT_ID { get; set; }
        public Nullable<int> EMP_GRADE_ID { get; set; }
        public string QUAL { get; set; }
        public string EXPNC_DETL { get; set; }
        public Nullable<int> EXPNC_YEAR { get; set; }
        public Nullable<int> EXPNC_MONTH { get; set; }
        public Nullable<bool> STAT { get; set; }
        public string STAT_DESCR { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string MARITAL_STAT { get; set; }
        public Nullable<int> CHLD_CNT { get; set; }
        public string FTHR_NAME { get; set; }
        public string MTHR_NAME { get; set; }
        public string HUSBND_NAME { get; set; }
        public string BLOOD_GRP { get; set; }
        public Nullable<int> NTLTY_ID { get; set; }
        public string HOME_ADDR_LINE1 { get; set; }
        public string HOME_ADDR_LINE2 { get; set; }
        public string HOME_CITY { get; set; }
        public string HOME_STATE { get; set; }
        public Nullable<int> HOME_CTRY_ID { get; set; }
        public string HOME_PIN_CODE { get; set; }
        public string OFF_ADDR_LINE1 { get; set; }
        public string OFF_ADDR_LINE2 { get; set; }
        public string OFF_CITY { get; set; }
        public string OFF_STATE { get; set; }
        public Nullable<int> OFF_CTRY_ID { get; set; }
        public string OFF_PIN_CODE { get; set; }
        public Nullable<long> OFF_PH1 { get; set; }
        public Nullable<long> OFF_PH2 { get; set; }
        public Nullable<long> MOBL_PH { get; set; }
        public Nullable<long> HOME_PH { get; set; }
        public string EML { get; set; }
        public string FAX { get; set; }
        public string PHTO_FILENAME { get; set; }
        public string PHTO_CNTNT_TYPE { get; set; }
        public string PHTO_DATA { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public Nullable<int> PHTO_FILE_SIZE { get; set; }
        public Nullable<int> USRID { get; set; }
        public Nullable<int> IMAGE_DOCUMENTS_ID { get; set; }
        public string LIBRARY_CARD { get; set; }
        public string FRMR_ID { get; set; }
    
        public virtual EMPLOYEE_CATEGORY EMPLOYEE_CATEGORY { get; set; }
        public virtual EMPLOYEE_DEPARTMENT EMPLOYEE_DEPARTMENT { get; set; }
        public virtual EMPLOYEE_GRADE EMPLOYEE_GRADE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_EMPLOYEE_SALARY_STRUCTURE> ARCHIVED_EMPLOYEE_SALARY_STRUCTURE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_EMPLOYEE_ADDITIONAL_DETAIL> ARCHIVED_EMPLOYEE_ADDITIONAL_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_EMPLOYEE_BANK_DETAIL> ARCHIVED_EMPLOYEE_BANK_DETAIL { get; set; }
        public virtual EMPLOYEE_POSITION EMPLOYEE_POSITION { get; set; }
        public virtual COUNTRY COUNTRY { get; set; }
        public virtual COUNTRY COUNTRY1 { get; set; }
        public virtual COUNTRY COUNTRY2 { get; set; }
        public virtual USER USER { get; set; }
    }
}
