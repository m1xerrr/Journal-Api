using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradingAccount
{
    public class AccountPeriodJsonModel
    {
        public Guid AccountId { get; set; }

        public string Provider { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
