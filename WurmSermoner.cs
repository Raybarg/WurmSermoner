using System;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using WurmSermoner.Services;
using WurmSermoner.Sermon;
using Microsoft.Extensions.DependencyInjection;


namespace WurmSermoner
{
    public class WurmSermoner
    {
        public ServiceProvider services;

        readonly bool bSilentMode = false;

        bool bWokeUp = true;
        bool bPreachAvailAnnounced = false;
        DiscordSocketClient client;

        public async Task MainAsync()
        {
            services = ConfigureServices();
            client = services.GetRequiredService<DiscordSocketClient>();
            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            try
            {
                SermonService sermon = (SermonService)services.GetService(typeof(SermonService));

                while (true)
                {
                    if (bWokeUp)
                    {
                        if (client.ConnectionState == ConnectionState.Connected)
                        {
                            DateTime last;
                            bWokeUp = false;
                            //preachers
                            if (sermon.preachers.Count > 0)
                            {
                                Console.WriteLine("I woke up and last sermon was at " + sermon.preachers.LastSermon().ToString("dd-MM-yyyy HH:mm:ss"));
                                last = sermon.preachers.LastSermon();
                            }
                            else
                            {
                                last = DateTime.Now;
                            }
                            double diff = DateTime.Now.Subtract(last).TotalMinutes;
                            await Msg("I woke up and last sermon was at " + last.ToString("dd-MM-yyyy HH:mm:ss") + " this is `" + Convert.ToInt32(diff).ToString() + "` minutes ago");

                        }
                    } else
                    {
                        if (client.ConnectionState == ConnectionState.Connected)
                        {
                            DateTime last = sermon.preachers.LastSermon();
                            double diff = DateTime.Now.Subtract(last).TotalMinutes;

                            if (Convert.ToInt32(diff) >= 30 && !bPreachAvailAnnounced)
                            {
                                bPreachAvailAnnounced = true;
                                await Msg("Can sermon now!!");
                            }

                            // Announce preachers if they can preach again
                            foreach (Preacher p in sermon.preachers)
                            {
                                int preacherDiff = Convert.ToInt32(DateTime.Now.Subtract(p.LastSermon).TotalMinutes);
                                if (preacherDiff < 1440)
                                {
                                    if (Convert.ToInt32(diff) >= 30 && preacherDiff > 180 && !p.CanPreachAnnounced)
                                    {
                                        p.CanPreachAnnounced = true;
                                        await Msg("**" + p.Name + "** can preach now!!");
                                    }
                                    if (Convert.ToInt32(diff) >= 25 && preacherDiff >= 175 && !p.CanPreachPreAnnounced)
                                    {
                                        p.CanPreachPreAnnounced = true;
                                        await Msg("`[Ring the bells!]` **" + p.Name + "** can preach in 5 minutes!!");
                                    }
                                }
                            }

                        }
                    }
                    await Task.Delay(200);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<SermonService>()
                .BuildServiceProvider();
        }

        public async void ConnectBot()
        {
            await client.LoginAsync(TokenType.Bot, Properties.Resources.BotToken);
            await client.StartAsync();
            bWokeUp = true;
        }

        public async void DisconnectBot()
        {
            await client.LogoutAsync();
            await client.StopAsync();
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task Msg(string msg)
        {
            if (!bSilentMode)
            {
                await client.GetGuild(ulong.Parse(Properties.Resources.Guild)).GetTextChannel(ulong.Parse(Properties.Resources.Channel)).SendMessageAsync(msg);
            }
        }
    }
}
