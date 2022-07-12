using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Diagnostics.Tracing;
using Discord.Addons.Interactive;

namespace Podium
{
    class Program
    {
        DiscordSocketClient client;
        CommandService command;
        IServiceProvider services;

        const string token = "token";

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig{WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance});
            command = new CommandService();
            services = new ServiceCollection().AddSingleton(client).AddSingleton(command).AddSingleton<InteractiveService>().BuildServiceProvider();

            client.Log += ClientLog;

            await RegisterCommandsAsync();
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await Task.Delay(-1);
        }

        Task ClientLog(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await command.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage msg = arg as SocketUserMessage;
            SocketCommandContext context = new SocketCommandContext(client, msg);

            int argPos = 0;

            if (msg.Author.IsBot)
                return;

            if (msg.HasStringPrefix("!", ref argPos))
            {
                var result = await command.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
