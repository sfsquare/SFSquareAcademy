using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFSAcademy.Models
{
    public class Configuration
    {
        private SFSAcademyEntities db = new SFSAcademyEntities();
        public string find_by_config_key(string Config_Key)
        {
            var Config_Value = (from Config in db.CONFIGURATIONs
                                 where Config.CONFIG_KEY == Config_Key
                                 select new {Val = Config.CONFIG_VAL}).ToList();

            if (Config_Value != null && Config_Value.Count() != 0)
            {
                return Config_Value.FirstOrDefault().Val.ToString();
            }
            else
            {
                return null;
            }
        }

        public string find_by_config_value(string Config_Value)
        {
            var Config_Key = (from Config in db.CONFIGURATIONs
                                where Config.CONFIG_VAL == Config_Value
                              select new { Key = Config.CONFIG_KEY }).ToList();
            if(Config_Key != null && Config_Key.Count()!= 0)
            {
                return Config_Key.FirstOrDefault().Key.ToString();
            }
            else
            {
                return null;
            }
            
        }
    }
}