using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels
{
    public class MTAccountJsonModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public int Login { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
    }
}
