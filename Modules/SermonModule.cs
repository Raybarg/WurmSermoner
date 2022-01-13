using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using WurmSermoner.Services;
using System.Globalization;
using System.Collections.Generic;
using System;
using System.Text;
using WurmSermoner.Sermon;
using System.Linq;

namespace WurmSermoner.Modules
{
    public class SermonModule : ModuleBase<SocketCommandContext>
    {
        public SermonService ss { get; set; }

        [Command("list")]
        public async Task ListAsync(IUser user = null)
        {
            StringBuilder sb = new StringBuilder();

            List<Preacher> pl = ss.preachers.OrderBy(o => o.LastSermon).ToList();

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

            await ReplyAsync(sb.ToString());
        }

        [Command("add")]
        public async Task AddAsync(string name, int offset)
        {
            DateTime preachTime = DateTime.Now.AddMinutes(-offset);

            ss.preachers.AddPreacher(name, preachTime);

            await ReplyAsync("Added sermon at " + preachTime.ToString("dd-MM-yyyy HH:mm:ss") + " by **" + name + "**");
        }
    }
}
