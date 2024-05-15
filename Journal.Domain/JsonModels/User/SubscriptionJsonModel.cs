using Journal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.User
{
    public class SubscriptionJsonModel
    {
        public Guid UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
    }
}
