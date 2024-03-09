﻿using Journal.Domain.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.Models
{
    public class AccountData
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Provider {  get; set; }
        public AccountData()
        {
            Deals = new List<DealResponseModel>();
        }
        public double currentBalance {  get; set; }

        public double Deposit { get; set; }

        public double Profit { get; set; }

        public double ProfitPercentage { get; set; }

        public List<DealResponseModel> Deals { get; set; }

        public int TotalDeals { get; set; }

        public int LostDeals { get; set; }

        public int WonDeals { get; set; }

        public int BreakevenDeals { get; set; }

        public int LongDeals { get; set; }

        public int ShortDeals { get; set; }

    }
}
