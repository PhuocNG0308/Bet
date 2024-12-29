using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Bet.Datas
{
    public class UserData
    {
        public ulong UserId { get; set; }
        public int GoldAmount { get; set; }
        public string UserName { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int MaxGoldAmount { get; set; }
    }
}
