using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using WurmSermoner.Services;

namespace WurmSermoner.Bots
{
    public class IrcBot : IBot
    {
        public SermonService ss { get; set; }
        
        // server to connect to (edit at will)
        private readonly string _server;
        // server port (6667 by default)
        private readonly int _port;
        // user information defined in RFC 2812 (IRC: Client Protocol) is sent to the IRC server 
        private readonly string _user;

        // the bot's nickname
        private readonly string _nick;
        // channel to join
        private readonly string _channel;

        private readonly int _maxRetries;

        private Task bottask;
        private bool running;

        public IrcBot(string server, int port, string user, string nick, string channel, int maxRetries = 3)
        {
            _server = server;
            _port = port;
            _user = user;
            _nick = nick;
            _channel = channel;
            _maxRetries = maxRetries;
        }

        public async Task<bool> MainAsync()
        {
            var retry = false;
            var retryCount = 0;
            do
            {
                try
                {
                    using (var irc = new TcpClient(_server, _port))
                    using (var stream = irc.GetStream())
                    using (var reader = new StreamReader(stream))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine("NICK " + _nick);
                        writer.Flush();
                        writer.WriteLine(_user);
                        writer.Flush();
                        
                        while (true && running)
                        {
                            string inputLine;
                            while ((inputLine = reader.ReadLine()) != null  && running)
                            {
                                Console.WriteLine("<- " + inputLine);

                                // split the lines sent from the server by spaces (seems to be the easiest way to parse them)
                                string[] splitInput = inputLine.Split(new Char[] { ' ' });

                                if (splitInput[0] == "PING")
                                {
                                    string PongReply = splitInput[1];
                                    Console.WriteLine("->PONG " + PongReply);
                                    writer.WriteLine("PONG " + PongReply);
                                    writer.Flush();
                                    //continue;
                                }

                                switch (splitInput[1])
                                {
                                    case "001":
                                        writer.WriteLine("JOIN " + _channel);
                                        writer.Flush();
                                        break;
                                    case "PRIVMSG":
                                        if (splitInput[3] == ":!list")
                                        {
                                            writer.WriteLine("PRIVMSG " + _channel + " :" + ss.preachers.GetDiscordList(ss.users));
                                            writer.Flush();
                                        }
                                        
                                        break;
                                    default:
                                        Console.WriteLine("-> " + splitInput[1]);
                                        break;
                                }
                            }
                            await Task.Delay(1000);
                        }
                    }
                }
                catch (Exception e)
                {
                    // shows the exception, sleeps for a little while and then tries to establish a new connection to the IRC server
                    Console.WriteLine(e.ToString());
                    Thread.Sleep(5000);
                    retry = ++retryCount <= _maxRetries;
                }
            } while (retry);
            return true;
        }
        
        public void Connect()
        {
            running = true;
            bottask = Task.Run(() => MainAsync());
        }
        
        public void Disconnect()
        {
            running = false;
        }
    }
}
