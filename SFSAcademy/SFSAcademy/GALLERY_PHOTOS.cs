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
    
    public partial class GALLERY_PHOTOS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GALLERY_PHOTOS()
        {
            this.GALLERY_TAGS = new HashSet<GALLERY_TAGS>();
        }
    
        public int ID { get; set; }
        public string DESCR { get; set; }
        public Nullable<System.DateTime> CREATED_AT { get; set; }
        public Nullable<System.DateTime> UPDATED_AT { get; set; }
        public string PHTO_FILENAME { get; set; }
        public string PHTO_CNTNT_TYPE { get; set; }
        public Nullable<int> PHTO_FILE_SIZE { get; set; }
        public Nullable<System.DateTime> PHTO_UPDATED_AT { get; set; }
        public string NAME { get; set; }
        public Nullable<int> GAL_CAT_ID { get; set; }
    
        public virtual GALLERY_CATEGORY GALLERY_CATEGORY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GALLERY_TAGS> GALLERY_TAGS { get; set; }
    }
}
