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
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Configuration;

namespace WurmSermoner
{
    public class WurmSermoner
    {
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

        public string Server;
        public string Port;

        public bool LogFileConfirmed = false;
        public ServiceProvider services;
        public DiscordSocketClient client;

        readonly bool bSilentMode = false;

        bool bWokeUp = true;
        bool bPreachAvailAnnounced = false;
        

        public async Task MainAsync()
        {
            services = ConfigureServices();
            client = services.GetRequiredService<DiscordSocketClient>();
            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            try
            {
                bool bInitFirstTime = true;
                FileStream fs = null;
                StreamReader sr = null;
                SermonService sermon = (SermonService)services.GetService(typeof(SermonService));
                DateTime curTime = DateTime.ParseExact("00:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);

                while (true)
                {
                    if (LogFileConfirmed)
                    {
                        fs = new FileStream(OperatorDir + LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        sr = new StreamReader(fs);
                        LogFileConfirmed = false;
                        sermon.preachers.Clear();
                    }
                    if (fs != null && sr != null)
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("Logging started"))
                            {
                                string temp = line.Substring(16);
                                DateTime dt = DateTime.Parse(temp);
                                Console.WriteLine("Logstart: " + dt.ToString());

                                curTime = new DateTime(dt.Year, dt.Month, dt.Day, curTime.Hour, curTime.Minute, curTime.Second);
                                bInitFirstTime = true;
                            }

                            if (line.StartsWith("["))
                            {
                                string temp = line.Substring(1, 8);
                                DateTime td = DateTime.ParseExact(temp, "HH:mm:ss", CultureInfo.InvariantCulture);

                                if (bInitFirstTime)
                                {
                                    bInitFirstTime = false;
                                    curTime = new DateTime(curTime.Year, curTime.Month, curTime.Day, td.Hour, td.Minute, td.Second);
                                }

                                if (curTime.Hour > td.Hour)
                                {
                                    DateTime tempTime = new DateTime(curTime.Year, curTime.Month, curTime.Day + 1, td.Hour, td.Minute, td.Second);
                                    curTime = tempTime;
                                    Console.WriteLine("Day changed.");
                                }
                                else
                                {
                                    DateTime tempTime = new DateTime(curTime.Year, curTime.Month, curTime.Day, td.Hour, td.Minute, td.Second);
                                    curTime = tempTime;
                                }
                            }

                            // Sermon
                            if (line.Contains("finish") && line.Contains("sermon"))
                            {
                                string time = line.Substring(1, 8);
                                DateTime td = DateTime.ParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture);
                                td = new DateTime(curTime.Year, curTime.Month, curTime.Day, td.Hour, td.Minute, td.Second);

                                string[] lineSplit = line.Split(' ');
                                if (lineSplit[1] == "You")
                                {
                                    lineSplit[1] = Operator;
                                }

                                sermon.preachers.AddPreacher(lineSplit[1], td);
                                Sermon.Sermon s = new Sermon.Sermon();
                                s.Preacher = lineSplit[1];
                                s.Time = td;
                                await BroadcastSermon(s);

                                Console.WriteLine("At: " + td.ToString("dd-MM-yyyy HH:mm:ss") + " by " + lineSplit[1]);
                                Console.WriteLine(line);

                                if (!bWokeUp)
                                    await Msg("**" + lineSplit[1] + "** sermoned at " + td.ToString("dd-MM-yyyy HH:mm:ss"));

                                bPreachAvailAnnounced = false;

                                sermon.preachers.ResetAnnouncements(false);
                            }
                        }
                    }

                    if (bWokeUp)
                    {
                        await Task.Delay(2000);
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
                                        var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                                        var settings = configFile.AppSettings.Settings;
                                        if (settings[p.Name] != null)
                                            mention = "<@" + settings[p.Name].Value + ">";

                                        await Msg("**" + p.Name + "** can preach now!! " + mention);
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

        private async Task Msg(string msg)
        {
            if (!bSilentMode)
            {
                Console.WriteLine("Discord -> " + msg);
                if (client.ConnectionState == ConnectionState.Connected)
                    await client.GetGuild(GuildID).GetTextChannel(ChannelID).SendMessageAsync(msg);
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
