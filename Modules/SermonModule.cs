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
using System.Configuration;

namespace WurmSermoner.Modules
{
    public class SermonModule : ModuleBase<SocketCommandContext>
    {
        public SermonService ss { get; set; }

        [Command("list")]
        public async Task ListAsync(IUser user = null)
        {
            string response = ss.preachers.GetDiscordList(ss.users);
            await ReplyAsync(response);
        }

        [Command("add")]
        public async Task AddAsync(string name, int offset)
        {
            DateTime preachTime = DateTime.Now.AddMinutes(-offset);

            ss.preachers.AddPreacher(name, preachTime);

            await ReplyAsync("Added sermon at " + preachTime.ToString("dd-MM-yyyy HH:mm:ss") + " by **" + name + "**");
        }

        [Command("mypriest")]
        public async Task MyPriest(string name)
        {
            Helpers.ConfigHelper.addUpdate(name, Context.User.Id.ToString());
            await ReplyAsync("Added priest " + name + " to <@" + Context.User.Id.ToString() + ">");
        }

        [Command("afk")]
        public async Task Afk(string time = "")
        {
            bool active = ss.users.ToggleAfk(Context.User.Id.ToString());
            string response = "<@" + Context.User.Id.ToString() + "> is now ";
            if (active)
                response += "active.";
            else
                response += "afk.";
            await ReplyAsync(response);
        }

        [Command("help")]
        public async Task Help()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("`!help` - This list of commands.");
            sb.AppendLine("`!list` - Lists all priests who sermoned in past 24 hours. Strikeout lines are priests with their 3h cooldown, with cooldown remaining in minutes in brackets. Last number is how many sermons this priest has had in current logfile.");
            sb.AppendLine("`!add Priestname 1` - Adds a sermon for Priestname 1 minutes ago. Useful when main logfile reading priest has disconnected.");
            sb.AppendLine("`!mypriest Priestname` - Links priestname with your Discord ID to mention you when your priest can preach.");
            sb.AppendLine("`!afk` - Toggles your Discord ID afk status.");

            await ReplyAsync(sb.ToString());
        }
    }
}
