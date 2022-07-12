using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Podium
{
    public class Commands : InteractiveBase<SocketCommandContext>
    {
        const int argIdx = 1;
        public async Task<PlayerInfo> GetCurrentPlayer()
        {
            if (!File.Exists(Context.Channel.Id + ".json"))
            {
                await Context.Channel.SendMessageAsync("**Player not set**");
            }

            StreamReader reader = new StreamReader(Context.Channel.Id + ".json");
            string content = reader.ReadToEnd();
            reader.Close();

            return JsonConvert.DeserializeObject<PlayerInfo>(content);
        }

        [Command("help")]
        public async Task Help()
        {
            var embed = new EmbedBuilder()
            {
                Title = "HELP",
                Description = "Command lists, etc."
            };
            embed.AddField("!set {name}", "Set sepcific player to analyze.");
            embed.AddField("!mmr", "View MMR of designated player.");
            embed.AddField("!match", "View match results of designated player.");
            embed.WithAuthor(Context.Client.CurrentUser)
                .WithColor(Color.Blue)
                .WithTitle("Help Message")
                .WithDescription("Commands list, etc.")
                .WithCurrentTimestamp();

            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("set")]
        public async Task SetPlayerInfo(string name, string region = null)
        {
            List<PlayerInfo> playerList =
                    JsonConvert.DeserializeObject<List<PlayerInfo>>(await HttpRequestManager.GetHttpRequest($"https://sc2ladder.herokuapp.com/api/player?query={name}&limit=200"));

            
            _ = Task.Run(() =>
            {
                int cnt = 0;

                string result;
                while (true)
                {
                    var embed = new EmbedBuilder()
                    {
                        Title = string.Format("Search Result: {0}", name)
                    };

                    while (cnt < playerList.Count)
                    {
                        embed.AddField(string.Format("{0}. <{1}>{2}", cnt + 1, playerList[cnt].Clan, playerList[cnt].Username), 
                            string.Format("Race: {0}, MMR: {1}, Region: {2}", playerList[cnt].Race, playerList[cnt].MMR, playerList[cnt].Region));
                        cnt++;
                        if (cnt % 25 == 0)
                            break;
                    }
                    embed.WithColor(Color.Blue);
                    Context.Channel.SendMessageAsync("", false, embed.Build());
                    Context.Channel.SendMessageAsync("**Input index of player**\n(next page = -1)");
                    var msg = Task.Run(() => NextMessageAsync(true));
                    result = msg.Result.Content;
                    if (result != "-1")
                        break;
                }
                
                int index;
                if (int.TryParse(result, out index))
                {
                    index--;
                }
                else
                {
                    Context.Channel.SendMessageAsync("**Wrong input**");
                }

                StreamWriter writer = new StreamWriter(Context.Channel.Id + ".json", false);
                writer.WriteLine(JsonConvert.SerializeObject(playerList[index]));
                writer.Close();

                Context.Channel.SendMessageAsync(string.Format("Player Set: <{0}>{1}", playerList[index].Clan, playerList[index].Username));
            });
        }

        [Command("mmr")]
        public async Task PrintPlayer()
        {
            PlayerInfo player = GetCurrentPlayer().Result;

            var playerEmbed = new EmbedBuilder()
            {
                Title = string.Format("Player Info: <{0}>{1}", player.Clan, player.Username)
            };
            playerEmbed.AddField("BattleTag", player.BattleTag);
            playerEmbed.AddField("MMR", player.MMR);
            playerEmbed.AddField("Rank", player.Rank);
            playerEmbed.WithColor(Color.Blue);
            await Context.Channel.SendMessageAsync("", false, playerEmbed.Build());
        }

        [Command("match")]
        public async Task PrintMatch()
        {
            PlayerInfo player = GetCurrentPlayer().Result;
            string accessToken = await HttpRequestManager.BlizzOAuthConnectAsync();
            var playerMatchHistory = await HttpRequestManager.GetHttpRequest
                    ($"https://kr.api.blizzard.com/sc2/legacy/profile/{player.GetRegionNum()}/{player.GetRealmNum()}/{player.ProfileID}/matches?access_token={accessToken}",
                    null,
                    accessToken);
            JArray obj = (JArray)JObject.Parse(playerMatchHistory)["matches"];

            var matchEmbed = new EmbedBuilder()
            {
                Title = string.Format("Player Match Results: <{0}>{1}", player.Clan, player.Username)
            };
            foreach (var m in obj.ToObject<List<GameMatch>>())
            {
                matchEmbed.AddField(string.Format("{0}", m.Map), string.Format("**{0}** - {1}", m.Result, m.Type));
            }
            matchEmbed.WithColor(Color.Blue);
            await Context.Channel.SendMessageAsync("", false, matchEmbed.Build());
        }
    }
}
