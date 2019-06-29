using Discord;
using Discord.Commands;
using GoreSama.Stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoreSama.Modules
{
    public class NSFW : ModuleBase
    {
        private async Task<List<Gelbooru>> GetGelbooruInfoAsync(HttpClient client, string tag)
        {
            List<Gelbooru> result = null;
            HttpResponseMessage response = await client.GetAsync($"/index.php?page=dapi&s=post&q=index&json=1&tags={tag}");
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<List<Gelbooru>>();
            }
            else
            {
                Console.WriteLine("An error occurred.");
                Console.WriteLine(response.StatusCode);
            }
            return result;
        }

        [Command("gelbooru", RunMode = RunMode.Async)]
        [RequireNsfw]
        public async Task GelBoDu([Remainder] string tag)
        {
            Random r = new Random();
            string rating = "";
            tag = tag.Replace(" ", "_");
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://gelbooru.com/");
            var result = await GetGelbooruInfoAsync(client, tag);
            int random = r.Next(0, result.Count);
            switch (result[random].rating)
            {
                case "e":
                    rating = "Explicit";
                    break;
                case "s":
                    rating = "Safe";
                    break;
                case "q":
                    rating = "Questionable";
                    break;
            }

            string source = result[random].source == "" ? "No source provided" : result[random].source;

            var builder = new EmbedBuilder()
                .WithTitle("Gelbooru search: " + tag)
                .AddField("Rating", rating)
                .AddField("Source", source)
                .AddField("Resolution", $"{result[random].height}x{result[random].width}")
                .WithImageUrl(result[random].file_url);

            await ReplyAsync("", false, builder.Build()).ConfigureAwait(false);
        }
    }
}
