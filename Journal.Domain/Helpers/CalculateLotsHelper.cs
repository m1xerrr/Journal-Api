using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Helpers
{
    public class CalculateLotsHelper
    {
        static public double CalculateForexLots(double risk, double price, double stoploss)
        {
            return Math.Round((risk / (Math.Abs(price - stoploss) * 100000)) , 2);
        }
    }
}
