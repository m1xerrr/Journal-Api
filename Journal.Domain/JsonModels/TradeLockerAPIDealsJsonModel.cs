using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels
{
    public class TradeLockerAPIDealsJsonModel
    {
            public string Name { get; set; }
            public List<string> Headers { get; set; }
            public List<List<string>> Data { get; set; }
            public double TotalAmount { get; set; }
            public double TotalSwap { get; set; }
            public double TotalCommission { get; set; }
            public double TotalProfit { get; set; }
            public double TotalNetProfit { get; set; }
    }

}
