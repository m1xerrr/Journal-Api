using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class CTraderAccount : Account
    {
        public string AccessToken { get; set; }
        public long AccountId { get; set; }
        public long Login {  get; set; }
        public bool IsLive { get; set; }
        public User User { get; set; }

    }
}
