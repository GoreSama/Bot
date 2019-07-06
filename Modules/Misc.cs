using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoreSama.Modules
{
    public class Misc : ModuleBase
    {
        private CommandService _service;

        public Misc(CommandService service)
        {
            _service = service;
        }

        [Command("help", RunMode = RunMode.Async)]
        [Alias("h")]
        [Summary("Returns a list of the available commands per module.")]
        public async Task PlzHelp()
        {
            string prefix = "g!";

            var bot = await Context.Client.GetApplicationInfoAsync();

            var embed = new EmbedBuilder()
            {
                Color = new Discord.Color(114, 137, 218),
                Description = "__Currently available commands__",
                ThumbnailUrl = bot.IconUrl
            };

            foreach (var module in _service.Modules)
            {
                string description = "";

                foreach(var cmd in module.Commands)
                {
                    description += $"`{prefix}{cmd.Aliases.First()}` | **{cmd.Summary}**\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    embed.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            var m = await ReplyAsync("", false, embed.Build());
            await DeleteMessage(m, 30);
        }

        public static async Task DeleteMessage(IUserMessage msg, int seconds)
        {
            seconds = seconds * 1000;
            await Task.Delay(seconds);
            await msg.DeleteAsync();
            Console.WriteLine("Message has been automatically deleted.");
        }
    }
}
