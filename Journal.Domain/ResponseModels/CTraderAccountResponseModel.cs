using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class CTraderAccountResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public string AccessToken { get; set; }
        public long AccountId { get; set; }
        public long Login { get; set; }
        public bool IsLive { get; set; }

        public double Deposit { get; set; }

        public double Profit { get; set; }

        public double ProfitPercentage { get; set; }

        public double Balance { get; set; }

        public int DealsCount { get; set; }

        public CTraderAccountResponseModel() { }
        public CTraderAccountResponseModel(CTraderAccount account) 
        {
            Id = account.Id;
            UserID = account.UserID;
            AccessToken = account.AccessToken;
            AccountId = account.AccountId;
            Login = account.Login;
            IsLive = account.IsLive;
        }
    }
}
