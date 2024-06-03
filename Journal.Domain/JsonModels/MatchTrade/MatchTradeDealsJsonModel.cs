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
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("operations")]
        public List<MatchTradeDealJsonModel> Operations { get; set; }
    }

    public class MatchTradeDealJsonModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("openTime")]
        public string OpenTime { get; set; }

        [JsonProperty("openPrice")]
        public string OpenPrice { get; set; }

        [JsonProperty("stopLoss")]
        public string StopLoss { get; set; }

        [JsonProperty("takeProfit")]
        public string TakeProfit { get; set; }

        [JsonProperty("swap")]
        public string Swap { get; set; }

        [JsonProperty("profit")]
        public string Profit { get; set; }

        [JsonProperty("netProfit")]
        public string NetProfit { get; set; }

        [JsonProperty("currentPrice")]
        public string CurrentPrice { get; set; }

        [JsonProperty("stopLossInMainWallet")]
        public string StopLossInMainWallet { get; set; }

        [JsonProperty("takeProfitInMainWallet")]
        public string TakeProfitInMainWallet { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }

        [JsonProperty("bidPrice")]
        public string BidPrice { get; set; }

        [JsonProperty("askPrice")]
        public string AskPrice { get; set; }
    }
}
