using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class TradeLockerAccount : Account
    {
        public long Login {  get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Server {  get; set; }

        public bool Live { get; set; }

        public User User { get; set; }
    }
}
