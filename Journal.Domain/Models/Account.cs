using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public abstract class Account
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public List<Deal>? Deals { get; set; }
    }
}
