using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WurmSermoner.Sermon
{
    public class UserList : List<User>
    {
        public bool ToggleAfk(string ID)
        {
            User u = this.Find(x => x.DiscordID.Equals(ID));

            if (u != null)
            {
                // Found...
                u.Active = !u.Active;
                return u.Active;
            }
            else
            {
                this.Add(new User() { DiscordID = ID, Active = false });
                return false;
            }
        }

        public bool IsAfk(string ID)
        {
            User u = this.Find(x => x.DiscordID.Equals(ID));
            if (u != null)
            {
                return !u.Active;
            }
            else
            {
                return false;
            }
        }
    }
}
