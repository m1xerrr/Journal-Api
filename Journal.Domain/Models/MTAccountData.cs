using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class MTAccountData
    {
        public MTAccountData()
        {
            Deals = new List<Deal>();
        }
        public double currentBalance {  get; set; }

        public double Deposit { get; set; }

        public double Profit { get; set; }

        public List<Deal> Deals { get; set; }

        public int TotalDeals { get; set; }

        public int SLDeals { get; set; }

        public int TPDeals { get; set; }

        public int MarketDeals { get; set; }

        public int LongDeals { get; set; }

        public int ShortDeals { get; set; }


    }
}
