using Discord.Commands;
using Discord.Bet.Datas;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Discord.Bet
{
    public class BetCommands : ModuleBase<SocketCommandContext>
    {
        private readonly UserManager _userManager;

        public BetCommands(UserManager userManager)
        {
            _userManager = userManager;
        }

        [Command("leaderboard")]
        [Summary("View the leaderboard of players with the most gold.")]
        public async Task LeaderboardAsync()
        {
            var leaderboard = _userManager.GetLeaderboard();
            var leaderboardMessage = "Leaderboard:\n";

            foreach (var player in leaderboard.Take(10)) // Hiển thị top 10
            {
                leaderboardMessage += $"{Context.Client.GetUser(player.UserId)?.Username}: {player.MaxGoldAmount} gold\n";
            }

            await ReplyAsync(leaderboardMessage);
        }

        [Command("bet")]
        [Summary("Place a bet!")]
        public async Task BetAsync([Remainder] string betArgs)
        {
            var args = betArgs.Split(' ');

            if (args.Length == 1 && args[0].ToLower() == "start")
            {
                var user = _userManager.GetUserData(Context.User.Id);
                if (user != null && user.GoldAmount > 0)
                {
                    await ReplyAsync("You have already started the game.");
                    return;
                }

                if (user == null)
                {
                    user = new UserData
                    {
                        UserId = Context.User.Id,
                        GoldAmount = 2000,  // Start with 2000 gold
                        MaxGoldAmount = 2000
                    };
                    _userManager.AddUserData(user);  // Add user data to the manager
                }
                else
                    user.GoldAmount = 2000;

                await ReplyAsync($"Game started! You have been granted 2000 gold.");
                return;
            }

            if (args.Length != 2)
            {
                await ReplyAsync("Incorrect command format! Usage: !bet <amount> <h/t>");
                return;
            }

            int betAmount = 0;
            if (args[0] == "all")
            {
                betAmount = _userManager.GetUserData(Context.User.Id).GoldAmount;
            }
            else if (!int.TryParse(args[0], out betAmount))
            {
                await ReplyAsync("Bet amount must be a valid number.");
                return;
            }

            string betType = args[1].ToLower();
            if (betAmount <= 0)
            {
                await ReplyAsync("Bet amount must be greater than zero.");
                return;
            }

            if (betType != "h" && betType != "t")
            {
                await ReplyAsync("Invalid bet type! Please choose 'h' for head or 't' for tail.");
                return;
            }

            var player = _userManager.GetUserData(Context.User.Id);

            if (player == null || player.GoldAmount < betAmount)
            {
                await ReplyAsync($"You don't have enough gold to place this bet. You currently have {player?.GoldAmount ?? 0} gold.");
                return;
            }

            var random = new Random();
            string coinFlipResult = random.Next(0, 2) == 0 ? "h" : "t";

            if (betType == coinFlipResult)
            {
                player.GoldAmount += (int)((float)betAmount * 0.85f);
                if (player.GoldAmount > player.MaxGoldAmount)
                {
                    player.MaxGoldAmount = player.GoldAmount;
                }
                await ReplyAsync($"You won the bet! The coin landed on {coinFlipResult}. Your new gold amount is {player.GoldAmount}.");
            }
            else
            {
                player.GoldAmount -= betAmount;
                if (player.GoldAmount < player.MaxGoldAmount)
                {
                    await ReplyAsync($"You lost the bet! The coin landed on {coinFlipResult}. Your new gold amount is {player.GoldAmount}. Your max gold during the game was {player.MaxGoldAmount}.");
                }
                else
                {
                    await ReplyAsync($"You lost the bet! The coin landed on {coinFlipResult}. Your new gold amount is {player.GoldAmount}.");
                }
            }

            _userManager.UpdateUserData(player);
            await _userManager.SaveDataAsync();
        }

    }
}
