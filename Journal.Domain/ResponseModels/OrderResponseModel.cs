using Journal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class OrderResponseModel
    {
        public long Id { get; set; }

        public string Symbol { get; set; }

        public Direction Direction { get; set; }

        public double Price { get; set; }

        public double Volume { get; set; }

        public DateTime OrderTime { get; set; }

        public OrderResponseModel() { }
    }
}
