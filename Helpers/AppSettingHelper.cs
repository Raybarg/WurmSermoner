using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace WurmSermoner.Helpers
{
    public class AppSettingHelper
    {
        public static bool RunServer()
        {
            return bool.Parse(ConfigHelper.addGet("RunServer", false.ToString()));
        }

        public static long PreacherDiscordID(string preacher)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[preacher] != null)
                return long.Parse(settings[preacher].Value);
            else
                return 0;
        }
    }
}
