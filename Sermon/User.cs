using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WurmSermoner.Sermon
{
    public class User
    {
        public string DiscordID;
        public bool Active;
        public List<string> Priests;

        public User()
        {
            Priests = new List<string>();
        }

        public void AddPriest(string priest)
        {
            Priests.Add(priest);
        }
    }
}
