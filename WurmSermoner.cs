using System;
using System.IO;
using System.Net.Http;
using System.Globalization;
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
using WurmSermoner.Helpers;
using WurmSermoner.Bots;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Configuration;
using WurmSermoner.Log;

namespace WurmSermoner
{
    public class WurmSermoner
    {
        SermonService sermonService;

        public bool Connected
        {
            get
            {
                return (client.ConnectionState == Discord.ConnectionState.Connected);
            }
        }
        public bool ServerResponded;

        public string BotToken;
        public ulong GuildID;
        public ulong ChannelID;

        public string Operator;
        public string OperatorDir;
        public string LogFile;
        public Parser parser;

        public string Server;
        public string Port;

        public ServiceProvider services;
        public DiscordSocketClient client;

        private bool bWokeUp = true;
        readonly bool bSilentMode = false;

        bool bPreachAvailAnnounced = false;
        bool bPreach5minAvailAnnounced = false;

        public IrcBot irc = new IrcBot("irc.rizon.net", 6667, "USER Raybot 0 * :Raybot", "Raybot", "#raytestageddon");


        public async Task MainAsync()
        {
            services = ConfigureServices();
            client = services.GetRequiredService<DiscordSocketClient>();
            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            try
            {
                sermonService = (SermonService)services.GetService(typeof(SermonService));
                irc.ss = sermonService;

                while (true)
                {
                    if (bWokeUp)
                    {
                        await Task.Delay(2000);
                        if (client.ConnectionState == ConnectionState.Connected)
                        {
                            DateTime last;
                            bWokeUp = false;
                            //preachers
                            if (sermonService.preachers.Count > 0)
                            {
                                Console.WriteLine("I woke up and last sermon was at " + sermonService.preachers.LastSermon().ToString("dd-MM-yyyy HH:mm:ss"));
                                last = sermonService.preachers.LastSermon();
                            }
                            else
                            {
                                last = DateTime.Now;
                            }
                            double diff = DateTime.Now.Subtract(last).TotalMinutes;
                            await Msg("I woke up and last sermon was at " + last.ToString("dd-MM-yyyy HH:mm:ss") + " this is `" + Convert.ToInt32(diff).ToString() + "` minutes ago", false);
                        }
                    } else
                    {
                        if (client.ConnectionState == ConnectionState.Connected)
                        {
                            DateTime last = sermonService.preachers.LastSermon();
                            double diff = DateTime.Now.Subtract(last).TotalMinutes;

                            if (Convert.ToInt32(diff) >= 30 && !bPreachAvailAnnounced)
                            {
                                bPreachAvailAnnounced = true;
                                string mention = "";
                                string first = sermonService.preachers.priestQueue.FirstInQueue();
                                long id = AppSettingHelper.PreacherDiscordID(first);
                                if (id > 0)
                                    mention = "<@" + id.ToString() + ">";

                                await Msg("`Can sermon now!!` **" + first + "** " + mention);
                            }

                            if(!sermonService.preachers.QueueMode)
                            {
                                // Announce preachers if they can preach again
                                foreach (Preacher p in sermonService.preachers)
                                {
                                    int preacherDiff = Convert.ToInt32(DateTime.Now.Subtract(p.LastSermon).TotalMinutes);
                                    if (preacherDiff < 1440)
                                    {
                                        if (Convert.ToInt32(diff) >= 25 && preacherDiff >= 175 && !p.CanPreachPreAnnounced)
                                        {
                                            p.CanPreachPreAnnounced = true;
                                            await Msg("`[Ring the bells!]` **" + p.Name + "** can preach in 5 minutes!!");
                                        }
                                        if (Convert.ToInt32(diff) >= 30 && preacherDiff > 180 && !p.CanPreachAnnounced)
                                        {
                                            p.CanPreachAnnounced = true;

                                            // Lets see if there is registered UserID for priest to mention
                                            string mention = "";
                                            long id = AppSettingHelper.PreacherDiscordID(p.Name);
                                            if (id > 0)
                                                mention = "<@" + id.ToString() + ">";

                                            await Msg("**" + p.Name + "** can preach now!! " + mention);
                                        }
                                    }
                                }
                            } else
                            {
                                if (Convert.ToInt32(diff) >= 25 && !bPreach5minAvailAnnounced)
                                {
                                    bPreach5minAvailAnnounced = true;
                                    string mention = "";
                                    string first = sermonService.preachers.priestQueue.FirstInQueue();
                                    long id = AppSettingHelper.PreacherDiscordID(first);
                                    if (id > 0)
                                        mention = "<@" + id.ToString() + ">";
                                    await Msg("`[Ring the bells!]` **Sermon in 5 minutes!** First in queue: **" + first + "** " + mention);
                                }
                            }
                             
                        }
                    }
                    sermonService.ListMessageUpdateTick();
                    await Task.Delay(200);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Parser_SermonEventOccurred(object sender, SermonEventArgs e)
        {
            Console.WriteLine("At: " + e.Sermon.Time.ToString("dd-MM-yyyy HH:mm:ss") + " by " + e.Sermon.Preacher);
            Console.WriteLine(e.RawLine);
            
            _ = Msg("👏👏 **" + e.Sermon.Preacher + "** sermoned at " + e.Sermon.Time.ToString("dd-MM-yyyy HH:mm:ss") + " 👏👏", false);
            
            sermonService.preachers.AddPreacher(e.Sermon.Preacher, e.Sermon.Time);
            
            if (!bWokeUp)
            {
                sermonService.RemoveSermonMessages();
                sermonService.RemoveLastSermonList();
                if (sermonService.preachers.QueueMode)
                {
                    sermonService.preachers.priestQueue.RemoveIfFirst(e.Sermon.Preacher);
                    sermonService.preachers.priestQueue.Add(e.Sermon.Preacher);
                    _ = Msg(sermonService.preachers.priestQueue.ListQueue());
                }
                else
                {
                    _ = Msg(sermonService.preachers.GetDiscordList(sermonService.users), false);
                    sermonService.ListMessageUpdate();
                }
            }
            bPreachAvailAnnounced = false;
            bPreach5minAvailAnnounced = false;
            sermonService.preachers.ResetAnnouncements(false);
        }

        public void LogFileChanged()
        {
            if (parser == null)
            {
                parser = new Parser(sermonService);
            }
            else
            {
                if (parser.IsRunning)
                {
                    parser.Stop();
                }
            }
            parser.Init(OperatorDir + LogFile, Operator);
            parser.SermonEventOccurred += Parser_SermonEventOccurred;
            parser.Start();

            sermonService.preachers.Clear();
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
            await client.LoginAsync(TokenType.Bot, BotToken);
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

        private async Task Msg(string msg, bool bAddToRemoveList = true)
        {
            if (!bSilentMode)
            {
                Console.WriteLine("Discord -> " + msg);
                if (client.ConnectionState == ConnectionState.Connected)
                {
                    try
                    {
                        var channel = client.GetChannel(ChannelID) as IMessageChannel;
                        var discordmsg = await channel.SendMessageAsync(msg);
                        if (sermonService != null)
                        {
                            sermonService.lastMessage = discordmsg;
                            if (bAddToRemoveList) sermonService.sermonMessages.Add(discordmsg);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private Task BroadcastSermon(Sermon.Sermon s)
        {
            try
            {
                string textToSend = "";
                XmlSerializer serializer = new XmlSerializer(typeof(Sermon.Sermon));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, s);
                    textToSend = writer.ToString();
                }

                var client = new UdpClient();
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Server), int.Parse(Port));
                client.Connect(ep);

                byte[] bytesToSend = Encoding.UTF8.GetBytes(EncryptionHelper.Encrypt(textToSend));
                client.Send(bytesToSend, bytesToSend.Length);


                // Lets see if we get response from server
                var wait = TimeSpan.FromSeconds(4);
                var result = client.BeginReceive(null, null);
                result.AsyncWaitHandle.WaitOne(wait);
                if (result.IsCompleted)
                {
                    try
                    {
                        IPEndPoint remote = null;
                        byte[] received = client.EndReceive(result, ref remote);
                        // could check if ep and remote are same?
                        ServerResponded = true;
                        Console.WriteLine("UDP received data from " + ep.ToString());
                        Console.WriteLine(EncryptionHelper.Decrypt(Encoding.UTF8.GetString(received)));
                    }
                    catch
                    {
                        // receive failed
                        ServerResponded = false;
                        Console.WriteLine("UDP Receive failed.");
                    }
                }
                else
                {
                    // timeout
                    ServerResponded = false;
                    Console.WriteLine("UDP Receive timeout.");
                }

                client.Close();
                //var receivedData = client.Receive(ref ep);

            }
            catch
            {
                ServerResponded = false;
                Console.WriteLine("UDP Connection failure.");
            }

            return Task.CompletedTask;
        }
    }
}
