using Journal.Domain.JsonModels.TradeLocker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Interfaces
{
    public interface ITradeLockerAPIRepository
    {
        Task<TradeLockerApiAccountJsonModel> GetAccount(string email, string password, string server, bool isLive, long accountId);
        Task<string> Initialize(string email, string password, string server, bool isLive);
        Task<double> GetDeposit(string email, string password, string server, bool isLive, long accountId);
        Task<TradeLockerAPIDealsJsonModel> GetDeals(string email, string password, string server, bool isLive, long accountId);
    }
}
