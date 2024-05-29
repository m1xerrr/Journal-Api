using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradingAccount
{
    public class OpenPositionJsonModel
    {
        public string Provider { get; set; }
        public Guid AccountId { get; set; }

        public string Symbol { get; set; }

        public double Risk { get; set; }

        public byte Type { get; set; }

        public float Price { get; set; }

        public float Stoploss { get; set; }

        public float TakeProfit { get; set; }
    }
}
