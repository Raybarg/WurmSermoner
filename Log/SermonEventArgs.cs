using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WurmSermoner.Log
{
    public class SermonEventArgs
    {
        public string RawLine { get; set; }
        public Sermon.Sermon Sermon { get; set; }
    }
}
