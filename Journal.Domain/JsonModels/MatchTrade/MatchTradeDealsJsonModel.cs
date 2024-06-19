using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MatchTrade
{
    public class MatchTradeDealsJsonModel
    {
        public List<MatchTradeDealJsonModel> Operations { get; set; }
    }

    public class MatchTradeDealJsonModel
    {
        public DateTime OpenTime { get; set; }
        public double OpenPrice { get; set; }
        public string Symbol { get; set; }
        public string Alias { get; set; }
        public string Id { get; set; }
        public double Volume { get; set; }
        public double StopLoss { get; set; }
        public double TakeProfit { get; set; }
        public DateTime Time { get; set; }
        public double ClosePrice { get; set; }
        public double Commission { get; set; }
        public double Swap { get; set; }
        public double Profit { get; set; }
        public string Side { get; set; }
        public double NetProfit { get; set; }
        public string Uid { get; set; }
        public string ClosingOrderID { get; set; }
        public string CloseReason { get; set; }
    }
}
