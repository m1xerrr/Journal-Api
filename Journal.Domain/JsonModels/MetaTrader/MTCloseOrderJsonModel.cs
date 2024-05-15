using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.MetaTrader
{
    public class MTCloseOrderJsonModel
    {
        public Guid AccountId { get; set; }

        public long ticket {  get; set; }
    }
}
