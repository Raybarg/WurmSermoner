using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WurmSermoner.Sermon;

namespace WurmSermoner.Services
{
    public class SermonService
    {
        private int foo;
        public PreacherList preachers = new PreacherList();

        public SermonService()
            => foo = 0;
    }
}
