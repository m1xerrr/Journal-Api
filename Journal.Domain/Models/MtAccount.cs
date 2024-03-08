using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class MTAccount : Account
    {
        public int Login { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }
        public User User { get; set; }
    }
}
