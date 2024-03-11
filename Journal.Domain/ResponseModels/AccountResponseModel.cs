using Azure.Core;
using Journal.Domain.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.ResponseModels
{
    public class AccountResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public long Login { get; set; }

        public string Provider { get; set; }

        public double Deposit { get; set; }

        public double Profit { get; set; }

        public double ProfitPercentage { get; set; }

        public double Balance { get; set; }

        public int DealsCount { get; set; }

        public AccountResponseModel(MTAccount account) 
        {
            Id = account.Id;
            UserId = account.UserID;
            Login = account.Login;
            Provider = "MetaTrader 5";
        }
        public AccountResponseModel(CTraderAccount account)
        {
            Id = account.Id;
            UserId = account.UserID;
            Login = account.Login;
            Provider = "CTrader";
        }
        public AccountResponseModel(DXTradeAccount account) 
        {
            Id = account.Id;
            UserId = account.UserID;
            Login = account.Login;
            Provider = "DXTrade";
        }
    }
}
