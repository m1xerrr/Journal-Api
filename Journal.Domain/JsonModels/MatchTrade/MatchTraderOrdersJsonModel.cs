using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MatchTrade
{
    public class Order
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("activationPrice")]
        public string ActivationPrice { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("stopLoss")]
        public string StopLoss { get; set; }

        [JsonProperty("takeProfit")]
        public string TakeProfit { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }
    }

    public class MatchTraderOrdersJsonModel
    {
        [JsonProperty("orders")]
        public List<Order> Orders { get; set; }
    }
}
