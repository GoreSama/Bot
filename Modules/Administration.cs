using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoreSama.Modules
{
    public class Administration : ModuleBase
    {
        [Command("beep", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task FetchUserInfo([Remainder] ulong id)
        {
            var user = await Context.Guild.GetUserAsync(id) as SocketGuildUser;
            var embed = new EmbedBuilder() {
                Title = $"{user.Username}'s information",
                ThumbnailUrl = user.GetAvatarUrl(),
                Color = Discord.Color.Red
            };

            var P = user.GetAvatarUrl();
            var C = user.JoinedAt;
            var S = user.Status;
            var R = "";
            var ID = user.Id;

            foreach (var p in user.Roles)
            {
                if (!p.Name.ToLower().Contains("everyone"))
                {
                    R += $"{p.Name}\n";
                }
            }

            embed.Description = $"**Username**: {user.Username}\n**Avatar url**: {P}\n**Joined**: {C}\n**Status**: {S}\n**Roles**:\n{R}\n**User ID**: {ID}";

            await ReplyAsync("", false, embed.Build());
        }
    }
}
