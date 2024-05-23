using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradeLocker
{
    public class TradeLockerSymbolJsonModel
    {
        [JsonProperty("s")]
        public string Status { get; set; }

        [JsonProperty("d")]
        public TradeLockerInstruments Data { get; set; }
    }

        public class TradeLockerInstruments
    {
            [JsonProperty("instruments")]
            public List<TradeLockerInstrument> Instruments { get; set; }
        }

        public class TradeLockerInstrument
    {
            [JsonProperty("tradableInstrumentId")]
            public int TradableInstrumentId { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("tradingExchange")]
            public string TradingExchange { get; set; }

            [JsonProperty("marketDataExchange")]
            public string MarketDataExchange { get; set; }

            [JsonProperty("country")]
            public object Country { get; set; }

            [JsonProperty("logoUrl")]
            public object LogoUrl { get; set; }

            [JsonProperty("localizedName")]
            public string LocalizedName { get; set; }

            [JsonProperty("routes")]
            public List<TradeLockerInstrumentRoute> Routes { get; set; }

            [JsonProperty("barSource")]
            public string BarSource { get; set; }

            [JsonProperty("hasIntraday")]
            public bool HasIntraday { get; set; }

            [JsonProperty("hasDaily")]
            public bool HasDaily { get; set; }
        }

        public class TradeLockerInstrumentRoute
    {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }   
        }
}
