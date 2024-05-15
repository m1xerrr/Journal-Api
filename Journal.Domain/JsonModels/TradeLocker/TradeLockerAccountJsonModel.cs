using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradeLocker
{
    public class TradeLockerAccountJsonModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public long AccountId { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public bool isLive { get; set; }

        public Guid UserId { get; set; }
    }
}
