using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels
{
    public class TradingAccountJsonModel
    {
        public Guid AccountId { get; set; }

        public string Provider { get; set; }
    }
}
