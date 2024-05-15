using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.TradeLocker
{
    public class TradeLockerApiAccountJsonModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public int AccNum { get; set; }
        public double AccountBalance { get; set; }
        /*
        public string Id { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public string AccNum { get; set; }
        public string AccountBalance { get; set; }*/
    }

    public class TradeLockerApiAccountsContainerJsonModel
    {
        public List<TradeLockerApiAccountJsonModel> Accounts { get; set; }
    }
}
