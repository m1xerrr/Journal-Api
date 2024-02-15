using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class DealJson
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("commission")]
        public double Commission { get; set; }

        [JsonProperty("entry")]
        public int Entry { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("fee")]
        public double Fee { get; set; }

        [JsonProperty("magic")]
        public int Magic { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("position_id")]
        public long PositionId { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("profit")]
        public double Profit { get; set; }

        [JsonProperty("reason")]
        public int Reason { get; set; }

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

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("volume")]
        public double Volume { get; set; }
    }
}