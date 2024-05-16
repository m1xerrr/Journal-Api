using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MetaTrader
{
    public class MTOpenPositionJsonModel
    {
        public Guid AccountId { get; set; }

        public string Symbol { get; set; }

        public float Volume { get; set; }

        public byte Type { get; set; }

        public float Price { get; set; }

        public float Stoploss { get; set; }

        public float TakeProfit { get; set; }
    }
}
