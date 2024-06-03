using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MatchTrade
{
    public class PositionDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }

        [JsonProperty("oldPartialVolume")]
        public object OldPartialVolume { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("openTime")]
        public DateTime OpenTime { get; set; }

        [JsonProperty("openPrice")]
        public string OpenPrice { get; set; }

        [JsonProperty("swap")]
        public string Swap { get; set; }

        [JsonProperty("profit")]
        public string Profit { get; set; }

        [JsonProperty("netProfit")]
        public string NetProfit { get; set; }

        [JsonProperty("currentPrice")]
        public string CurrentPrice { get; set; }

        [JsonProperty("commission")]
        public string Commission { get; set; }
    }

    public class Position
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("openTime")]
        public DateTime OpenTime { get; set; }

        [JsonProperty("openTimeMillis")]
        public long OpenTimeMillis { get; set; }

        [JsonProperty("openPrice")]
        public double OpenPrice { get; set; }

        [JsonProperty("stopLoss")]
        public double StopLoss { get; set; }

        [JsonProperty("takeProfit")]
        public double TakeProfit { get; set; }

        [JsonProperty("trailingDistance")]
        public int TrailingDistance { get; set; }

        [JsonProperty("swap")]
        public double Swap { get; set; }

        [JsonProperty("profit")]
        public double Profit { get; set; }

        [JsonProperty("netProfit")]
        public double NetProfit { get; set; }

        [JsonProperty("currentPrice")]
        public double CurrentPrice { get; set; }

        [JsonProperty("commission")]
        public double Commission { get; set; }

        [JsonProperty("positions")]
        public List<PositionDetail> Positions { get; set; }
    }

    public class MatchTradePositionsJsonModel
    {
        [JsonProperty("positions")]
        public List<Position> Positions { get; set; }
    }
}
