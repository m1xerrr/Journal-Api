using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MetaTrader
{
    public class MTPositionJsonModel
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("identifier")]
        public long Identifier { get; set; }

        [JsonProperty("magic")]
        public int Magic { get; set; }

        [JsonProperty("price_current")]
        public double PriceCurrent { get; set; }

        [JsonProperty("price_open")]
        public double PriceOpen { get; set; }

        [JsonProperty("profit")]
        public double Profit { get; set; }

        [JsonProperty("reason")]
        public int Reason { get; set; }

        [JsonProperty("sl")]
        public double Sl { get; set; }

        [JsonProperty("swap")]
        public double Swap { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ticket")]
        public long Ticket { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("time_msc")]
        public long TimeMsc { get; set; }

        [JsonProperty("time_update")]
        public long TimeUpdate { get; set; }

        [JsonProperty("time_update_msc")]
        public long TimeUpdateMsc { get; set; }

        [JsonProperty("tp")]
        public double Tp { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }

    }
}

