using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels
{
    public class CTraderAccountJsonModel
    {

        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string AccessToken { get; set; }

        public long AccountId { get; set; }

    }
}
