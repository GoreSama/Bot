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

        [Command("serverinfo", RunMode = RunMode.Async)]
        [Alias("si")]
        [Summary("Returns server information")]
        public async Task ServerInfo()
        {
            var guild = Context.Guild;
            var icon = guild.IconUrl;
            var date = $"{guild.CreatedAt.Month}/{guild.CreatedAt.Day}/{guild.CreatedAt.Year}";
            var users = await Context.Guild.GetUsersAsync();

            var embed = new EmbedBuilder()
            {
                Color = new Discord.Color(29, 140, 209),
                ThumbnailUrl = icon
            };

            string roles = "";

            foreach (var r in Context.Guild.Roles)
            {
                if (!r.Name.ToLower().Contains("everyone"))
                {
                    roles += $"{r.Name}\n";
                }
            }


            embed.Title = $"**{guild.Name}** server information";
            embed.Description = $"**Date created**: {date}\n**Guild members**: {users.Count()}\n**Roles**:\n{roles}";
            await ReplyAsync("", false, embed.Build());
        }
    }
}
