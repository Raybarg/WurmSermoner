using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using WurmSermoner.Helpers;

namespace WurmSermoner.Sermon
{
    public class UserList : List<User>
    {
        public static string UserListFileName = AppDomain.CurrentDomain.BaseDirectory + "\\Users.json";
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

        public void Save()
        {
            File.WriteAllText(UserListFileName, JsonSerializer.Serialize(this));
        }

        public void Load()
        {
            if (File.Exists(UserListFileName))
            {
                string json = File.ReadAllText(UserListFileName);
                DeserializeUserList(json);
            }
        }

        public void DeserializeUserList(string jsonUsers)
        {
            List<User> users = JsonSerializer.Deserialize<List<User>>(jsonUsers);
            this.AddRange(users);
        }

        /// <summary>
        /// Return the Discord ID of the user with the given priest
        /// Attempt to find it from app.config if not found in memory
        /// </summary>
        /// <param name="priest"></param>
        /// <returns></returns>
        public long DiscordIDByPriest(string priest) {
            priest = TextHelper.ToTitleCase(priest);
            User u = this.Find(x => x.Priests.Contains(priest));
            if (u != null)
            {
                return long.Parse(u.DiscordID);
            }
            else
            {
                long id = DiscorIDByPriestFromAppSettings(priest);
                return id;
            }
        }

        /// <summary>
        /// Returns the Discord ID of the user with the given priest from app.config
        /// </summary>
        /// <param name="priest"></param>
        /// <returns></returns>
        public long DiscorIDByPriestFromAppSettings(string priest)
        {
            long id = AppSettingHelper.PreacherDiscordID(priest);
            if(id > 0) {
                User u = this.Find(x => x.DiscordID.Equals(id.ToString()));
                if (u != null)
                {
                    u.AddPriest(priest);
                }
                else
                {
                    this.Add(new User() { DiscordID = id.ToString(), Active = false, Priests = new List<string>() { priest } });
                }
            }
            return id;
        }
    }
}
