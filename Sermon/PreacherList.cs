using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WurmSermoner.Helpers;

namespace WurmSermoner.Sermon
{
    public class PreacherList : List<Preacher>
    {
        public PriestQueue priestQueue = new PriestQueue(ConfigHelper.addGet("priestQueue", ""));
        public bool QueueMode = bool.Parse(ConfigHelper.addGet("queuemode", "False"));

        public bool IsOnCooldown
        {
            get
            {
                if (this.Count > 0)
                {
                    DateTime last = LastSermon();
                    int diff = Convert.ToInt32(DateTime.Now.Subtract(last).TotalMinutes);
                    if (diff < 30)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
        }

        public int CoolDownLeft
        {
            get
            {
                if (this.Count > 0)
                {
                    DateTime last = LastSermon();
                    int diff = Convert.ToInt32(DateTime.Now.Subtract(last).TotalMinutes);
                    if (diff < 30)
                    {
                        return 30 - diff;
                    }
                    else
                    {
                        return 0;
                    }
                }
                return 0;
            }
        }

        public void AddPreacher(string preacher, DateTime sermonTime)
        {
            Preacher p = this.Find(x => x.Name.Equals(preacher));

            if (p != null)
            {
                p.LastSermon = sermonTime;
                p.CanPreachAnnounced = false;
                p.CanPreachPreAnnounced = false;
                p.PreachCount++;
            }
            else
            {
                this.Add(new Preacher() { Name = preacher, LastSermon = sermonTime, CanPreachAnnounced = false, CanPreachPreAnnounced = false, PreachCount = 1 });
            }
        }

        public DateTime LastSermon()
        {
            return this.Max(i => i.LastSermon);
        }

        public void ResetAnnouncements(bool bToggle)
        {
            foreach (Preacher p in this)
            {
                p.CanPreachAnnounced = bToggle;
                p.CanPreachPreAnnounced = bToggle;
            }
        }

        public string GetDiscordList(UserList users)
        {
            StringBuilder sb = new StringBuilder();

            List<Preacher> pl = this.OrderBy(o => o.LastSermon).ToList();

            foreach (Preacher p in pl)
            {
                DateTime last = p.LastSermon;
                int diff = Convert.ToInt32(DateTime.Now.Subtract(last).TotalMinutes);

                if (diff < 1440)
                {
                    string extra = "";
                    string extra2 = "";
                    string afk = "";
                    if (diff < 180)
                    {
                        extra = "~~";
                        extra2 = " [" + (180 - diff).ToString() + " minutes]";
                    }

                    long id = AppSettingHelper.PreacherDiscordID(p.Name);
                    if (id > 0)
                        if (users.IsAfk(id.ToString()))
                            afk = "  - **AFK**";

                    sb.AppendLine(extra + "**" + p.Name + "** (" + p.LastSermon.ToString("dd-MM-yyyy HH:mm:ss") + ") -> " + diff.ToString() + " minutes ago." + extra + extra2 + " [# " + p.PreachCount.ToString() + "]" + afk);
                }
            }
            if (this.IsOnCooldown)
                sb.AppendLine("Sermoning appears to be on cooldown for " + this.CoolDownLeft.ToString() + " more minutes.");
            return sb.ToString();
        }

    }
}
