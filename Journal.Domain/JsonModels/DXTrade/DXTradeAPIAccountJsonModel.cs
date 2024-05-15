using Journal.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.DXTrade
{
    public class DXTradeAPIAccountJsonModel
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("baseCurrency")]
        public string BaseCurrency { get; set; }
        [JsonProperty("registrationTime")]
        public DateTime RegistrationTime { get; set; }
        [JsonProperty("accountStatus")]
        public string AccountStatus { get; set; }
        [JsonProperty("pricingStream")]
        public string PricingStream { get; set; }
        [JsonProperty("positionBased")]
        public bool PositionBased { get; set; }
    }

    public class DXTradeAccountAPIUserJsonModel
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("fullName")]
        public string FullName { get; set; }
        [JsonProperty("accounts")]
        public List<DXTradeAPIAccountJsonModel> Accounts { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
    }
    public class DXTradeAccountAPIUserDetailsContainer
    {
        [JsonProperty("userDetails")]
        public List<DXTradeAccountAPIUserJsonModel> UserDetails { get; set; }
    }
}
