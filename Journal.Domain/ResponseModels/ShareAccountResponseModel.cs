using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class ShareAccountResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public long Login { get; set; }

        public string Provider { get; set; }

        public double Deposit { get; set; }

        public double Profit { get; set; }

        public double ProfitPercentage { get; set; }

        public double Balance { get; set; }

        public int DealsCount { get; set; }

        public double Commission {  get; set; }

        public List<string> Symbols { get; set; }

        public ShareAccountResponseModel(AccountResponseModel account)
        {
            Id = account.Id;
            UserId = account.UserId;
            Login = account.Login;
            Provider = account.Provider;
            Deposit = account.Deposit;
            Profit = account.Profit;
            ProfitPercentage = account.ProfitPercentage;
            Balance = account.Balance;
            DealsCount = account.DealsCount;
        }
        public ShareAccountResponseModel() { Symbols = new List<string>(); }  
    }
}
