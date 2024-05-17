using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradeLocker
{
    public class TradeLockerPositionsJsonModel
    {
        [JsonProperty("s")]
        public string Status { get; set; }

        [JsonProperty("d")]
        public PositionData Data { get; set; }
    }
    public class PositionData
    {
        [JsonProperty("positions")]
        public List<List<object>> Positions { get; set; }
    }
}
