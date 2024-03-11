using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class DXTradeAccount : Account
    {
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long Login { get; set; }
        public User User { get; set; }
    }
}
