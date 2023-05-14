using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WurmSermoner.Sermon
{
    public class PreachSlot
    {
        public string Name;
        public DateTime Time;
        public string DiscordID;

        public bool IsEmpty()
        {
            return (this.Name == "empty");
        }

        public bool IsOwnedOrFree(string DiscordID)
        {
            return this.DiscordID == DiscordID || String.IsNullOrEmpty(this.DiscordID);
        }

        public PreachSlot(string name, DateTime time)
        {
            this.Name = name;
            this.Time = time;
            this.DiscordID = "";
        }
    }
}
