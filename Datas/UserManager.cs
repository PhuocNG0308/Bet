using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Bet.Datas
{
    public class UserManager
    {
        private Dictionary<ulong, UserData> _userData = new Dictionary<ulong, UserData>();

        public UserManager()
        {
            LoadDataAsync().GetAwaiter().GetResult();
        }

        public void AddUserData(UserData user)
        {
            _userData[user.UserId] = user;
        }

        public UserData GetUserData(ulong userId)
        {
            _userData.TryGetValue(userId, out var user);
            return user;
        }

        public void UpdateUserData(UserData user)
        {
            if (_userData.ContainsKey(user.UserId))
            {
                _userData[user.UserId] = user;
            }
        }

        public async Task SaveDataAsync()
        {
            var json = JsonConvert.SerializeObject(_userData.Values, Formatting.Indented);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "userData.json");
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task LoadDataAsync()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "userData.json");
            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath);
            _userData = JsonConvert.DeserializeObject<List<UserData>>(json).ToDictionary(x => x.UserId);
        }

        public List<UserData> GetLeaderboard()
        {
            return _userData.Values.OrderByDescending(x => x.MaxGoldAmount).ToList();
        }
    }

}
