using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradeLocker
{
    public class TradeLockerCloseOrderJsonModel
    {
        public Guid AccountId { get; set; }
        public long positionId { get; set; }
    }
}
