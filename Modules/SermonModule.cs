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
            await ReplyAsync(ss.preachers.GetDiscordList());
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
