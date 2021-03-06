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
    
    public partial class STUDENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public STUDENT()
        {
            this.ARCHIVED_STUDENT = new HashSet<ARCHIVED_STUDENT>();
            this.ASSESSMENT_SCORE = new HashSet<ASSESSMENT_SCORE>();
            this.ATTENDENCEs = new HashSet<ATTENDENCE>();
            this.BATCH_STUDENT = new HashSet<BATCH_STUDENT>();
            this.CCE_REPORTS = new HashSet<CCE_REPORTS>();
            this.EXAM_SCORE = new HashSet<EXAM_SCORE>();
            this.FEE_COLLECTION_DISCOUNT = new HashSet<FEE_COLLECTION_DISCOUNT>();
            this.FEE_COLLECTION_PARTICULAR = new HashSet<FEE_COLLECTION_PARTICULAR>();
            this.FINANCE_FEE = new HashSet<FINANCE_FEE>();
            this.FINANCE_FEE_PARTICULAR = new HashSet<FINANCE_FEE_PARTICULAR>();
            this.FINANCE_FEE_STRUCTURE_ELEMENT = new HashSet<FINANCE_FEE_STRUCTURE_ELEMENT>();
            this.FINANCE_TRANSACTION = new HashSet<FINANCE_TRANSACTION>();
            this.GROUPED_EXAM_REPORT = new HashSet<GROUPED_EXAM_REPORT>();
            this.GUARDIANs = new HashSet<GUARDIAN>();
            this.STORE_SELLING = new HashSet<STORE_SELLING>();
            this.STORE_SELLING_BACKUP = new HashSet<STORE_SELLING_BACKUP>();
            this.STORE_SELLING_CART = new HashSet<STORE_SELLING_CART>();
            this.STUDENT_ADDITIONAL_DETAIL = new HashSet<STUDENT_ADDITIONAL_DETAIL>();
            this.STUDENT_PREVIOUS_DATA = new HashSet<STUDENT_PREVIOUS_DATA>();
            this.STUDENT_PREVIOUS_SUBJECT_MARK = new HashSet<STUDENT_PREVIOUS_SUBJECT_MARK>();
            this.STUDENT_SUBJECT = new HashSet<STUDENT_SUBJECT>();
            this.SUBJECT_LEAVE = new HashSet<SUBJECT_LEAVE>();
            this.PREVIOUS_EXAM_SCORE = new HashSet<PREVIOUS_EXAM_SCORE>();
        }
    
        public int ID { get; set; }
        public string ADMSN_NO { get; set; }
        public Nullable<int> CLS_ROLL_NO { get; set; }
        public Nullable<System.DateTime> ADMSN_DATE { get; set; }
        public string FIRST_NAME { get; set; }
        public string MID_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public Nullable<int> BTCH_ID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string GNDR { get; set; }
        public string BLOOD_GRP { get; set; }
        public string BIRTH_PLACE { get; set; }
        public string LANG { get; set; }
        public string RLGN { get; set; }
        public string ADDR_LINE1 { get; set; }
        public string ADDR_LINE2 { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string PIN_CODE { get; set; }
        public Nullable<int> CTRY_ID { get; set; }
        public Nullable<long> PH1 { get; set; }
        public Nullable<long> PH2 { get; set; }
        public string EML { get; set; }
        public Nullable<int> IMMDT_CNTCT_ID { get; set; }
        public bool IS_SMS_ENABL { get; set; }
        public string PHTO_FILENAME { get; set; }
        public string PHTO_CNTNT_TYPE { get; set; }
        public Nullable<int> PHTO_DATA { get; set; }
        public string STAT_DESCR { get; set; }
        public bool IS_ACT { get; set; }
        public bool IS_DEL { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public bool HAS_PD_FE { get; set; }
        public Nullable<int> PHTO_FILE_SIZE { get; set; }
        public int USRID { get; set; }
        public Nullable<int> STDNT_CAT_ID { get; set; }
        public Nullable<int> NTLTY_ID { get; set; }
        public Nullable<int> IMAGE_DOCUMENTS_ID { get; set; }
        public string LIBRARY_CARD { get; set; }
        public Nullable<bool> BOOK_PURCHAGED { get; set; }
        public Nullable<System.DateTime> BOOK_PUR_DT { get; set; }
        public Nullable<bool> DRESS_PURCHAGED { get; set; }
        public Nullable<System.DateTime> DRESS_PUR_DT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ARCHIVED_STUDENT> ARCHIVED_STUDENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSESSMENT_SCORE> ASSESSMENT_SCORE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTENDENCE> ATTENDENCEs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BATCH_STUDENT> BATCH_STUDENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCE_REPORTS> CCE_REPORTS { get; set; }
        public virtual COUNTRY COUNTRY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXAM_SCORE> EXAM_SCORE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FEE_COLLECTION_DISCOUNT> FEE_COLLECTION_DISCOUNT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FEE_COLLECTION_PARTICULAR> FEE_COLLECTION_PARTICULAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE> FINANCE_FEE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE_PARTICULAR> FINANCE_FEE_PARTICULAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_FEE_STRUCTURE_ELEMENT> FINANCE_FEE_STRUCTURE_ELEMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FINANCE_TRANSACTION> FINANCE_TRANSACTION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GROUPED_EXAM_REPORT> GROUPED_EXAM_REPORT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GUARDIAN> GUARDIANs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING> STORE_SELLING { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING_BACKUP> STORE_SELLING_BACKUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STORE_SELLING_CART> STORE_SELLING_CART { get; set; }
        public virtual STUDENT_CATGEORY STUDENT_CATGEORY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_ADDITIONAL_DETAIL> STUDENT_ADDITIONAL_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_PREVIOUS_DATA> STUDENT_PREVIOUS_DATA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_PREVIOUS_SUBJECT_MARK> STUDENT_PREVIOUS_SUBJECT_MARK { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STUDENT_SUBJECT> STUDENT_SUBJECT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SUBJECT_LEAVE> SUBJECT_LEAVE { get; set; }
        public virtual USER USER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PREVIOUS_EXAM_SCORE> PREVIOUS_EXAM_SCORE { get; set; }
    }
}
