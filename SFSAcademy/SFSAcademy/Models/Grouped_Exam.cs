using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SFSAcademy
{
    public class GroupedExamSelect
    {
        public GROUPED_EXAM GroupesdExamData { get; set; }
        public EXAM_GROUP ExamGroupData { get; set; }
        public BATCH BatchData { get; set; }
        public COURSE CourseData { get; set; }
        public bool Select { get; set; }
        public decimal? Weightage { get; set; }
        public int? Exam_Group_Id { get; set; }
        public int? Batch_Id { get; set; }
    }
}