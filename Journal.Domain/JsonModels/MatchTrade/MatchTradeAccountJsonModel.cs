using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MatchTrade
{
    public class MatchTradeAccountJsonModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public int BrokerId { get; set; }

        public bool IsLive { get; set; }

        public Guid UserId { get; set; }

        public long AccountId { get; set; }
    }
}
