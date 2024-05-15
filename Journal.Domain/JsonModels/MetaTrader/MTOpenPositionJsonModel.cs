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

        public string Symbol;

        public float Volume;

        public byte Type;

        public float Price;

        public float Stoploss;

        public float TakeProfit;
    }
}
