using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MatchTrade
{
    public class InitializeAccountJsonModel
    {
        public long Id { get; set; }

        public string UUID { get; set; }

        public string TradingApiToken { get; set; }

        public string CoAuthToken { get; set; }

    }
}
