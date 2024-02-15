using Journal.Domain.Enums;

namespace Journal.Domain.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public SubscriptionType Subsctiption { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public User User { get; set; }
    }
}
