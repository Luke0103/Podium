using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace Podium
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        const int argIdx = 1;

        [Command("help")]
        public async Task Help()
        {
            StringBuilder helpcontent = new StringBuilder();

            helpcontent.AppendLine("!register - register your discord server to Podium(must be done first)");
            helpcontent.AppendLine("!addtable {tablename} - add table {tablename} for record savings");
            helpcontent.AppendLine("!viewtables - view all tables created");
            helpcontent.AppendLine("!addmatch {tablename} - add match result to table {tablename}");
            helpcontent.AppendLine("!viewrank {tablename} {number1} {number2} - view players from {tablename}, rank #{number1} to #{number2}");
            helpcontent.AppendLine("!player {name} - view player {name}");
            
            await ReplyAsync(helpcontent.ToString());
        }

        [Command("register", RunMode = RunMode.Async)]
        public async Task Register()
        {
            GoogleSheetConnection conn = new GoogleSheetConnection();
            bool hasSheet = false;

            Console.WriteLine("***");

            await Task.Run(() =>
            {
                conn.Initiate();

                hasSheet = conn.HasSheet(Context.Guild.Id);
            });

            Console.WriteLine("***");
            Console.WriteLine(hasSheet);

            await Task.Run(() =>
            {
                if (!hasSheet)
                {
                    conn.Create(Context.Guild.Name, Context.Guild.Id);
                    return;
                }
                else
                    ReplyAsync("Already registered");
            });
            Console.WriteLine("***");   
        }

        [Command("addtable")]
        public async Task AddTable()
        {
            
        }

        [Command("addmatch")]
        public async Task AddMatch()
        {
            string[] split = Context.Message.Content.Split(' ');
            string tableName = split[argIdx];

            await ReplyAsync("Type the winners");
            await AddMatchWinner();
            await ReplyAsync("Type the losers");
            await AddMatchLoser();

            await ReplyAsync("Match added");
        }

        public async Task AddMatchWinner()
        {
            await ReplyAsync("Winners confirmed");
        }
        public async Task AddMatchLoser()
        {
            await ReplyAsync("Losers confirmed");
        }

        [Command("viewrank")]
        public async Task ViewRank()
        {
            List<string> playerList = new List<string>();
            string[] split = Context.Message.Content.Split(' ');
            string tableName = split[argIdx];
            string highBound = split[argIdx + 1];
            string lowBound = split[argIdx + 2];

            for (int i = 0; i < playerList.Count; i++)
            {
                await ReplyAsync(playerList[i]);
            }
        }

        [Command("player")]
        public async Task Player()
        {
            await ReplyAsync("Player info");
        }
    }
}
