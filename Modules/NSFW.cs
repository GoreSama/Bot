using Discord;
using Discord.Commands;
using GoreSama.Stuff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
                Console.WriteLine(response.StatusCode);
            }
            return result;
        }

        public async Task<string> GetResponseAsync(string v, WebHeaderCollection headers = null)
        {
            var wr = (HttpWebRequest)WebRequest.Create(v);
            if (headers != null)
                wr.Headers = headers;
            using (var sr = new StreamReader((await wr.GetResponseAsync()).GetResponseStream()))
            {
                return await sr.ReadToEndAsync();
            }
        }

        public async Task<string> GetDanbooruImageLink(string tag)
        {
            try
            {
                var rng = new Random();

                var webpage = await GetResponseAsync($"http://danbooru.donmai.us/posts?page={ rng.Next(0, 15) }&tags={ tag.Replace(" ", "_") }");
                var matches = Regex.Matches(webpage, "data-file-url=\"(?<id>.*?)\"");

                return $"{ matches[rng.Next(0, matches.Count)].Groups["id"].Value }";
            }
            catch
            {
                return null;
            }
        }

        [Command("danbooru", RunMode = RunMode.Async)]
        [RequireNsfw]
        [Summary("Searches Danbooru for a randomm image of a specified tag. Example: g!danbooru atago_(azur_lane)")]
        public async Task Danbooru([Remainder] string tag)
        {
            await ReplyAsync(await GetDanbooruImageLink(tag));
        }

        [Command("gelbooru", RunMode = RunMode.Async)]
        [Summary("Searches Gelbooru for a random image of a specified tag. Example: g!gelbooru atago_(azur_lane)")]
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
