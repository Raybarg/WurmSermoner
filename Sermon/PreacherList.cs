using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WurmSermoner.Sermon
{
    public class PreacherList : List<Preacher>
    {
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

        public string GetDiscordList()
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
                    if (diff < 180)
                    {
                        extra = "~~";
                        extra2 = " [" + (180 - diff).ToString() + " minutes]";
                    }

                    sb.AppendLine(extra + "**" + p.Name + "** (" + p.LastSermon.ToString("dd-MM-yyyy HH:mm:ss") + ") -> " + diff.ToString() + " minutes ago." + extra + extra2 + " [# " + p.PreachCount.ToString() + "]");
                }
            }
            return sb.ToString();
        }

    }
}
