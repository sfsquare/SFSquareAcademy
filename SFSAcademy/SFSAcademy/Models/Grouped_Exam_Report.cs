using System;

namespace SFSAcademy
{
    public partial class GROUPED_EXAM_REPORT : IHasTimeStamp
    {
        public void DoTimeStamp(string EntityStateVal)
        {

            if (EntityStateVal.Equals("Added"))
            {           
                CREATED_AT = DateTime.Now;
                UPDATED_AT = DateTime.Now;
            }

            if (EntityStateVal.Equals("Modified"))
            {          
                UPDATED_AT = DateTime.Now;
            }
        }
    }
}