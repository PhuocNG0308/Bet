using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Discord.Bet.Datas;

namespace Discord.Bet
{
    class MainClass
    {
        public static void Main(string[] args) => new MainClass().MainAsync().GetAwaiter().GetResult();

        private static DiscordSocketClient _client;
        private static CommandService _commands;

        private CommandHandler commandHandler;

        public async Task MainAsync()
        {
            var socketConfig = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(socketConfig);
            _commands = new CommandService();

            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<UserManager>();

            var serviceProvider = services.BuildServiceProvider();

            var commandHandler = new CommandHandler(_client, _commands, serviceProvider);
            await commandHandler.InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, "YOUR_TOKEN_HERE");
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
