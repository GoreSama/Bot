using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GoreSama
{
    class Program
    {
        private DiscordSocketClient _client;

        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false
            });

            _client.Log += Logger;
            //_commands.Log += Logger; // Not using this for now
            _commands.CommandExecuted += _commands_CommandExecuted;

            _services = ConfigureServices();
        }

        private Task _commands_CommandExecuted(Optional<CommandInfo> arg1, ICommandContext arg2, IResult arg3)
        {
            ConsoleColor c = arg3.IsSuccess == true ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = c;
            Console.WriteLine($"Command \"{arg1.Value.Name}\" was executed by {arg2.User.Username}.");
            if (arg3.IsSuccess == false) Console.WriteLine($"Error: {arg3.ErrorReason}");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        private static IServiceProvider ConfigureServices()
        {
            var map = new ServiceCollection();

            return map.BuildServiceProvider();
        }

        private async Task InitializeCommands()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _client.MessageReceived += MessageHandler;
        }

        private async Task MainAsync()
        {
            await InitializeCommands();

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task MessageHandler(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

            int pos = 0;

            if (msg.HasCharPrefix('$', ref pos))
            {
                var context = new SocketCommandContext(_client, msg);
                var result = await _commands.ExecuteAsync(context, pos, _services);
            }
        }

        private Task Logger(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }
    }
}
