using Journal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels
{
    public class DealEditJsonModel
    {
        public int Id { get; set; }

        public string Field { get; set; }

        public DescriptionType Type { get; set; }

        public Guid accountId { get; set; }
    }
}
