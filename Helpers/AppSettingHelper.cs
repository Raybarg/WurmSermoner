using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WurmSermoner.Helpers
{
    public class AppSettingHelper
    {
        public static bool RunServer()
        {
            return bool.Parse(ConfigHelper.addGet("RunServer", false.ToString()));
        }
    }
}
