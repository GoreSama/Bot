using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoreSama.Modules
{
    public class PublicModule : ModuleBase
    {
        [Command("ping", RunMode = RunMode.Async)]
        [Summary("Ping pong!")]
        public async Task PingPong()
        {
            await ReplyAsync("Pong!");
        }

    }
}
