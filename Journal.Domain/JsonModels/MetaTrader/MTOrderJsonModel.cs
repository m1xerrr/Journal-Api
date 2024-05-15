using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MetaTrader
{
    public class MTOrderJsonModel
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        [JsonProperty("magic")]
        public int Magic { get; set; }

        [JsonProperty("position_by_id")]
        public int PositionById { get; set; }

        [JsonProperty("position_id")]
        public int PositionId { get; set; }

        [JsonProperty("price_current")]
        public double PriceCurrent { get; set; }

        [JsonProperty("price_open")]
        public double PriceOpen { get; set; }

        [JsonProperty("price_stoplimit")]
        public double PriceStopLimit { get; set; }

        [JsonProperty("reason")]
        public int Reason { get; set; }

        [JsonProperty("sl")]
        public double Sl { get; set; }

        [JsonProperty("state")]
        public int State { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ticket")]
        public long Ticket { get; set; }

        [JsonProperty("time_done")]
        public long TimeDone { get; set; }

        [JsonProperty("time_done_msc")]
        public long TimeDoneMsc { get; set; }

        [JsonProperty("time_expiration")]
        public long TimeExpiration { get; set; }

        [JsonProperty("time_setup")]
        public long TimeSetup { get; set; }

        [JsonProperty("time_setup_msc")]
        public long TimeSetupMsc { get; set; }

        [JsonProperty("tp")]
        public double Tp { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("type_filling")]
        public int TypeFilling { get; set; }

        [JsonProperty("type_time")]
        public int TypeTime { get; set; }

        [JsonProperty("volume_current")]
        public double VolumeCurrent { get; set; }

        [JsonProperty("volume_initial")]
        public double VolumeInitial { get; set; }
    }
}
