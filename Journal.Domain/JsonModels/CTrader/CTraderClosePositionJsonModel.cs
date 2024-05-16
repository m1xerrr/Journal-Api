using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.CTrader
{
    public class CTraderClosePositionJsonModel
    {
        public Guid AccountId { get; set; }

        public long Id { get; set; }
    }
}
