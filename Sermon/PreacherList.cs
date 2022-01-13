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

    }
}
