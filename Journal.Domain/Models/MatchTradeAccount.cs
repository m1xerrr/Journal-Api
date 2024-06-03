using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class MatchTradeAccount : Account
    {
        public long Login { get; set; }

        public int BrokerId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string UUID { get; set; }

        public bool LiveStatus { get; set; }

        public User User { get; set; }
    }
}
