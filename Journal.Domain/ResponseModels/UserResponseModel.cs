using Journal.Domain.Enums;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class UserResponseModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public SubscriptionResponseModel Subscription { get; set; }

        public UserResponseModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Password = user.Password;
            Role = user.Role;
            if (user.Subscription != null)
            {
                Subscription = new SubscriptionResponseModel(user.Subscription);
            }
        }
    }
}
