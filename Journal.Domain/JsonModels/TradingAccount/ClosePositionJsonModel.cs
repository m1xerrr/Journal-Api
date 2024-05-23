using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradingAccount
{
    public class ClosePositionJsonModel
    {
        public string Provider { get; set; }
        public Guid AccountId { get; set; }
        public long positionId { get; set; }
    }
}
