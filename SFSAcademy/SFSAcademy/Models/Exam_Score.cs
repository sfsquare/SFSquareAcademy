using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SFSAcademy
{
    public class ExamScoreDetails
    {
        public string Name { get; set; }
        public int? Student_Id { get; set; }
        public int? Exam_Id { get; set; }
        public int? Marks { get; set; }
        public int? Grading_Level_Id { get; set; }
        public int? Subject_Id { get; set; }
        public string Remark { get; set; }
        public bool? Is_Fail { get; set; }
        public SUBJECT SubjectData { get; set; }
        public EXAM ExamData { get; set; }
        public STUDENT StudentData { get; set; }
    }

    public partial class EXAM_SCORE : IHasTimeStamp
    {
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
    }
}