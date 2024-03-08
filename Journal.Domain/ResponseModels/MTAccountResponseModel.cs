using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class MTAccountResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public int Login { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public double Deposit { get; set; }

        public double Profit { get; set; }

        public double ProfitPercentage { get; set; }

        public double Balance { get; set; }

        public int DealsCount { get; set; }

        public MTAccountResponseModel(MTAccount account) 
        {
            Id = account.Id;
            UserId = account.UserID;
            Login = account.Login;
            Password = account.Password;
            Server = account.Server;
        }

    }
}
