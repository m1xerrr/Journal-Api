using Journal.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public List<MTAccount>? Accounts { get; set; }
        public Subscription Subscription { get; set; }

        public User() { 
            Accounts = new List<MTAccount>();
        }
    }
}
