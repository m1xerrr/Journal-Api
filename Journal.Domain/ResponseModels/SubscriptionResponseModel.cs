using Journal.Domain.Enums;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class SubscriptionResponseModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public SubscriptionType Subsctiption { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public SubscriptionResponseModel() { }

        public SubscriptionResponseModel(Subscription sub)
        {
            Id = sub.Id;
            UserId = sub.UserId;
            this.Subsctiption = sub.Subsctiption;
            PurchaseDate = sub.PurchaseDate;
            ExpirationDate = sub.ExpirationDate;
        }
    }
}
