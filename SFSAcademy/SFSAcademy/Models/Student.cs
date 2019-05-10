using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFSAcademy
{
    public class Student
    {
        public STUDENT StudentData { get; set; }
        public BATCH BatcheData { get; set; }
        public COURSE CourseData { get; set; }
        public COUNTRY CountryData { get; set; }
        public STUDENT_CATGEORY CategoryData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
        public GRADING_LEVEL GradeData { get; set; }
        public IMAGE_DOCUMENTS ImageData { get; set; }
        public GUARDIAN GuardianData { get; set; }
    }

    public class StudentsBatch
    {
        public IEnumerable<BATCH> BatchList { get; set; }
        public string SearchString { get; set; }
    }

    public class RadioCourseBatch
    {
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public string BatchId { get; set; }
    }

    public class StudentsGuardians
    {
        public GUARDIAN GuardianData { get; set; }
        public STUDENT StudentData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public EMPLOYEE EmployeeData { get; set; }
    }
    public class SelectGuardian
    {
        public GUARDIAN GuardianList { get; set; }
        public bool Selected { get; set; }
    }
    public class StudentTC
    {
        public STUDENT StundentData { get; set; }
        public string Status_Descrition { get; set; }
    }

    public enum MissingDetl
    {
        DateOfBirth,
        PhoneNumber,
        ParentDetails,
        StundetsPicture,
        SchoolBook,
        SchoolDress
    }

    public enum HadPdFees
    {
        Y,
        N
    }
    public enum BloodGroup
    {
        [Display(Name = "O+")]
        O_Positive,
        [Display(Name = "A+")]
        A_Positive,
        [Display(Name = "B+")]
        B_Positive,
        [Display(Name = "AB+")]
        AB_Positive,
        [Display(Name = "AB-")]
        AB_Negative,
        [Display(Name = "A-")]
        A_Negative,
        [Display(Name = "B-")]
        B_Negative,
        [Display(Name = "O-")]
        O_Negative

    }
    public enum Relationship
    {
        [Display(Name = "Father")]
        Father,
        [Display(Name = "Mother")]
        Mother,
        [Display(Name = "Uncle")]
        Uncle,
        [Display(Name = "Auntie")]
        Auntie,
        [Display(Name = "Other")]
        Other

    }
}