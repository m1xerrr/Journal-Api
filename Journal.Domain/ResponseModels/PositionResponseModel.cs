using Journal.Domain.Enums;
using Journal.Domain.JsonModels.MetaTrader;
using Journal.Domain.JsonModels.TradeLocker;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class PositionResponseModel
    {
        public string Id { get; set; }

        public string Symbol { get; set; }

        public Direction Direction { get; set; }

        public double OpenPrice { get; set; }

        public double Volume { get; set; }

        public DateTime OpenTime { get; set; }

        public double Profit { get; set; }

        public PositionResponseModel() { }
    }
}
