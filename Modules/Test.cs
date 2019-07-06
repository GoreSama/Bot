using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoreSama.Modules
{
    public class Test : ModuleBase
    {

        [Command("rc", RunMode = RunMode.Async)]
        [Summary("Returns color information of a role.")]
        public async Task RoleInfo([Remainder] ulong id)
        {
            var role = Context.Guild.GetRole(id);
            await ReplyAsync($"Role Color RGB: {role.Color.R}, {role.Color.G}, {role.Color.G}\nHex value: {role.Color.R:X2}{role.Color.G:X2}{role.Color.B:X2}");
        }

    }
}
