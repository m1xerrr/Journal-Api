using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.CTrader
{
    public class CTraderOpenPositionJsonModel
    {
        public Guid AccountId { get; set; }

        public string Symbol { get; set; }

        public float Volume { get; set; }

        public byte Type { get; set; }

        public double Price { get; set; }

        public double Stoploss { get; set; }

        public double TakeProfit { get; set; }
    }
}
