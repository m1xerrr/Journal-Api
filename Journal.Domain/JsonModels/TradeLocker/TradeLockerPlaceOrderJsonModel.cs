using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradeLocker
{
    public class TradeLockerPlaceOrderJsonModel
    {
        public Guid AccountId { get; set; }

        public double Price { get; set; }

        public double StopLoss { get; set; }

        public double TakeProfit { get; set; }

        public double Volume { get; set; }

        public byte Type { get; set; }

        public string Symbol { get; set; }


    }
}
