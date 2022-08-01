using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace WurmSermoner.Helpers
{
    public class TextHelper
    {
        public static string ToTitleCase(string str)
        {
            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
            return ti.ToTitleCase(ti.ToLower(str));
        }
    }
}
