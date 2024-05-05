using Journal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class DescriptionItem
    {
        public Guid Id { get; set; }

        public int DealId { get; set; }
        
        public int Number { get; set; }

        public string Field { get; set; }

        public DescriptionType Type { get; set; }
        
        public Deal Deal { get; set; }
    }
}
