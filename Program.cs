using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using GoreSama.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GoreSama
{
    class Program
    {
        private DiscordSocketClient _client;

        private ulong _NSFW = 596042136785387520;

        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100,
                ExclusiveBulkDelete = false
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = false
            });

            _client.Log += Logger;
            _commands.Log += Logger; // Jk

            _services = ConfigureServices();
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
            _client.UserJoined += _client_UserJoined;
            _client.ReactionAdded += AddedReactions;
            _client.ReactionRemoved += RemovedReactions;
        }

        private async Task RemovedReactions(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var guildChannel = channel as SocketGuildChannel;
            var userMessage = await message.GetOrDownloadAsync();
            var NSFW = new Emoji("🔞");

            SocketGuildUser reactionUser = reaction.User.IsSpecified ? reaction.User.Value as SocketGuildUser : null;
            var role = reactionUser.Guild.GetRole(_NSFW);
            if (reactionUser != null && !reactionUser.IsBot && reaction.MessageId == 595794238856232965)
            {
                if (reaction.Emote.Name == NSFW.Name)
                {
                    await reactionUser.RemoveRoleAsync(role);
                    Console.WriteLine($"Removed NSFW role from {reactionUser.Username}.");
                }
            }
        }

        private async Task AddedReactions(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var guildChannel = channel as SocketGuildChannel;
            var userMessage = await message.GetOrDownloadAsync();
            var NSFW = new Emoji("🔞");

            SocketGuildUser reactionUser = reaction.User.IsSpecified ? reaction.User.Value as SocketGuildUser : null;
            var role = reactionUser.Guild.GetRole(_NSFW);
            if (reactionUser != null && !reactionUser.IsBot && reaction.MessageId == 595794238856232965)
            {
                if (reaction.Emote.Name == NSFW.Name)
                {
                    await reactionUser.AddRoleAsync(role);
                    Console.WriteLine($"Gave NSFW role to {reactionUser.Username}.");
                }
            }
        }

        private async Task _client_UserJoined(SocketGuildUser arg)
        {
            if (!arg.IsBot) await arg.AddRoleAsync(arg.Guild.GetRole(362462632046886912));
            else
                await arg.AddRoleAsync(arg.Guild.GetRole(_NSFW));

            // channel for welcome messages
            var channel = arg.Guild.GetTextChannel(593551622177554484);

            if (!arg.IsBot)
            {
                var m = await channel.SendMessageAsync($"Welcome to **{arg.Guild.Name}**, {arg.Mention}! Please take a moment to read the rules in <#593553954722414597>, enjoy your stay!");
                await Misc.DeleteMessage(m, 30);
            }
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

            if (msg.HasStringPrefix("g!", ref pos))
            {
                var context = new SocketCommandContext(_client, msg);
                var result = await _commands.ExecuteAsync(context, pos, _services);
                if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand)
                    Console.WriteLine(result.ToString());
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
