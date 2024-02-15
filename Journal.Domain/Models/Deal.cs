using Journal.Domain.Enums;

namespace Journal.Domain.Models
{
    public class Deal
    {
        public long PositionId { get; set; }

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

        public string Notes { get; set; }
    }
}
