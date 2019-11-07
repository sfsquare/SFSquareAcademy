using System;
using System.ComponentModel.DataAnnotations;



namespace SFSAcademy
{

    [MetadataType(typeof(ArchiveEmpMetadata))]
    public partial class ARCHIVED_EMPLOYEE : IHasTimeStamp, IHasBeforeSave
    {
        internal sealed class ArchiveEmpMetadata
        {

        }
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
        public void Before_Save()
        {
            if(this.STAT != false)
            {
                STAT = false;
            }
        }
        
        public string Full_Name
        {
            get { return string.Concat(this.FIRST_NAME, " ", this.MID_NAME, " ", this.LAST_NAME); }
        }
    }
}