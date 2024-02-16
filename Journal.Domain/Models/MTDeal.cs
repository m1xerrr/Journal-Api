using Journal.Domain.Enums;

namespace Journal.Domain.Models
{
    public class MTDeal
    {
        public int Id { get; set; }
        public long PositionId { get; set; }

        public Guid AccountId { get; set; }

        public Direction Direction { get; set; }

        public double EntryPrice { get; set; }

        public double ExitPrice { get; set; }

        public double Profit { get; set; }

        public double Volume { get; set; }

        public double Comission { get; set; }

        public DateTime EntryTime { get; set; }

        public DateTime ExitTime { get; set; }

        public string Symbol { get; set; }

        public CloseType CloseType { get; set; }

        public string? Notes { get; set; }

        public string? Image { get; set; }

        public MTAccount Account { get; set; }
    }
}
