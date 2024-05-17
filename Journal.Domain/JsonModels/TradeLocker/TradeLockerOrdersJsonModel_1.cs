using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradeLocker
{
    public class TradeLockerOrdersJsonModel
    {
        [JsonProperty("s")]
        public string Status { get; set; }
        [JsonProperty("d")]
        public OrderData Data { get; set; }
    }
    public class OrderData
    {
        [JsonProperty("orders")]
        public List<List<object>> Orders { get; set; }
    }
}
