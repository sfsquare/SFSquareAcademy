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
    
    public partial class GROUPED_EXAM
    {
        public int ID { get; set; }
        public Nullable<int> EXAM_GROUP_ID { get; set; }
        public Nullable<int> BTCH_ID { get; set; }
        public Nullable<decimal> WTAGE { get; set; }
    
        public virtual BATCH BATCH { get; set; }
        public virtual EXAM_GROUP EXAM_GROUP { get; set; }
    }
}
