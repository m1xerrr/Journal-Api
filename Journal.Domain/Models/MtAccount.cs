using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class MTAccount
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public int Login { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }
        public User User { get; set; }

        public List<MTDeal>? Deals { get; set; }
    }
}
