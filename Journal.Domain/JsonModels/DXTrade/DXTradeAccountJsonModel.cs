using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.DXTrade
{
    public class DXTradeAccountJsonModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public Guid UserId { get; set; }
    }
}
