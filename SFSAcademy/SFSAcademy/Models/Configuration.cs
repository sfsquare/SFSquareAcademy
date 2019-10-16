using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Caching;
using System.Web.Mvc;


namespace SFSAcademy
{
    public enum AttendanceType
    {
        [Display(Name = "Daily")]
        Daily,
        [Display(Name = "Subject Wise")]
        SubjectWise
    }
    public enum NetworkState
    {
        [Display(Name = "Online")]
        Online,
        [Display(Name = "Offline")]
        Offline
    }

    [MetadataType(typeof(ConfigMetadata))]
    public partial class CONFIGURATION : IValidatableObject
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        internal sealed class ConfigMetadata
        {

        }
        private string ErrorMessage { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            AttendanceType AttendanceType;
            if (this.CONFIG_KEY == "StudentAttendanceType" && !Enum.TryParse<AttendanceType>(this.CONFIG_VAL, out AttendanceType))
            {
                ErrorMessage = "Student Attendance Type should be any one of " + string.Join(",", AttendanceType.GetValues(AttendanceType.GetType()));
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "CONFIG_VAL" });
            }

            NetworkState NetworkState;
            if (this.CONFIG_KEY == "StudentAttendanceType" && !Enum.TryParse<NetworkState>(this.CONFIG_VAL, out NetworkState))
            {
                ErrorMessage = "Student Attendance Type should be any one of " + string.Join(",", NetworkState.GetValues(NetworkState.GetType()));
                yield return new ValidationResult($"* {ErrorMessage}.", new[] { "CONFIG_VAL" });
            }

        }
        public void Clear_School_Cache(USER user, Cache Cache)
        {
            //IDictionaryEnumerator enumerator = Cache.GetEnumerator();
            Cache.Remove(user.ID.ToString());
        }
        public string Get_Config_Value(string key)
        {
            CONFIGURATION c = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == key).FirstOrDefault();
            return c == null ? null : c.CONFIG_VAL;
        }
        public List<string> Available_Modules()
        {
            var modules = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "AvailableModules").ToList();
            List<string> mList = new List<string>();
            foreach (var m in modules)
            {
                mList.Add(m.CONFIG_VAL);
            }
            return mList;
        }
        public void Set_Config_Values(List<SelectListItem> values_hash)
        {
            foreach (var item in values_hash)
            {
                CONFIGURATION NewConfig = new CONFIGURATION() { CONFIG_KEY = item.Text, CONFIG_VAL = item.Value };
                db.CONFIGURATIONs.Add(NewConfig);
            }
            db.SaveChanges();

        }
        public void Set_Value(string key, string value)
        {
            CONFIGURATION NewConfig = new CONFIGURATION() { CONFIG_KEY = key, CONFIG_VAL = value };
            db.CONFIGURATIONs.Add(NewConfig);
            db.SaveChanges();

        }
        public List<SelectListItem> Get_Multiple_Configs_As_Hash(List<string> keys)
        {
            var conf = db.CONFIGURATIONs.Where(x => keys.Contains(x.CONFIG_KEY)).ToList();
            List<SelectListItem> conf_hash = new List<SelectListItem>();
            foreach (var item in conf)
            {
                var result = new SelectListItem();
                result.Text = item.CONFIG_KEY;
                result.Value = item.CONFIG_VAL;
                conf_hash.Add(result);
            }
            return conf_hash;
        }
        public List<string> Get_Grading_Types()
        {
            GradingTypes GradingTypes;
            var types = db.CONFIGURATIONs.Where(x => Enum.TryParse<GradingTypes>(x.CONFIG_KEY, out GradingTypes)).ToList();
            List<string> GrgTypes = new List<string>();
            foreach (var item in types)
            {
                GrgTypes.Add(item.CONFIG_KEY);
            }
            return GrgTypes;
        }
        public int? Default_Country()
        {
            return Convert.ToInt32(db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == "DefaultCountry").FirstOrDefault().CONFIG_VAL);
        }
        public void Set_Grading_Types(List<string> updates)
        {
            var grading_types = (from GradingTypes gr in Enum.GetValues(typeof(GradingTypes))
                                 select new { ID = (int)gr, Name = gr.ToString() }).ToList();

            var deletions = grading_types.Where(x => !updates.Contains(x.Name)).ToList();
            foreach (var t in updates)
            {
                if (db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == t).FirstOrDefault() == null)
                {
                    CONFIGURATION NewConfig = new CONFIGURATION() { CONFIG_KEY = t, CONFIG_VAL = "1" };
                    db.CONFIGURATIONs.Add(NewConfig);
                }
            }
            db.SaveChanges();
            foreach (var t in deletions)
            {
                CONFIGURATION UpdConfig = db.CONFIGURATIONs.Where(x => x.CONFIG_KEY == t.Name).FirstOrDefault();
                if (UpdConfig == null)
                {
                    CONFIGURATION NewConfig = new CONFIGURATION() { CONFIG_KEY = t.Name, CONFIG_VAL = "0" };
                    db.CONFIGURATIONs.Add(NewConfig);
                }
                else
                {
                    UpdConfig.CONFIG_VAL = "0";
                    db.Entry(UpdConfig).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }
        public bool CCE_Enabled
        {
            get { return Get_Config_Value("CCE") == "1"? true: false; }
        }
        public bool Has_GPA
        {
            get { return Get_Config_Value("GPA") == "1" ? true : false; }
        }
        public bool Has_CWA
        {
            get { return Get_Config_Value("CWA") == "1" ? true : false; }
        }
    }
}