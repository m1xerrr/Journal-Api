using Journal.Domain.Enums;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class DealResponseModel
    {
        public long PositionId { get; set; }

        public Direction Direction { get; set; }

        public double EntryPrice { get; set; }

        public double ExitPrice { get; set; }

        public double Profit { get; set; }

        public double ProfitPercentage { get; set; }
        public double Volume { get; set; }

        public double Comission { get; set; }

        public DateTime EntryTime { get; set; }

        public DateTime ExitTime { get; set; }

        public string Symbol { get; set; }

        public Result Result { get; set; }

        public string Notes { get; set; }

        public string Image { get; set; }

        public DealResponseModel(Deal deal)
        {
            this.PositionId = deal.PositionId;
            this.Direction = deal.Direction;
            this.EntryPrice = deal.EntryPrice;
            this.ExitPrice = deal.ExitPrice;
            this.Profit = deal.Profit;
            this.Volume = deal.Volume;
            this.Comission = deal.Comission;
            this.EntryTime = deal.EntryTime;
            this.ExitTime = deal.ExitTime;
            this.Symbol = deal.Symbol;
            this.Result = deal.Result;
            this.Notes = deal.Notes;
            this.Image = deal.Image;
            this.ProfitPercentage = deal.ProfitPercentage;
        }
        public DealResponseModel() { }
    }
    
}
