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
using Microsoft.Extensions.DependencyInjection;


namespace WurmSermoner
{
    public class WurmSermoner
    {
        DiscordSocketClient client;
        public ServiceProvider services;

        public async Task MainAsync()
        {
            services = ConfigureServices();
            client = services.GetRequiredService<DiscordSocketClient>();
            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            try
            {
                while(true)
                {
                    await Task.Delay(1000);
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
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}
