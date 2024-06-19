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
        Task<TradeLockerSymbolJsonModel> GetSymbols(string email = "alihuim1xar1@gmail.com", string password = "gCvDDs7$", string server = "OSP-DEMO", bool isLive = false, long accountId = 518920);
        Task<TradeLockerPositionsJsonModel> GetPositions(string email, string password, string server, bool isLive, long accountId);
        Task<TradeLockerOrdersJsonModel> GetOrders(string email, string password, string server, bool isLive, long accountId);
        Task<bool> PlaceOrder(string email, string password, string server, bool isLive, long accountId, double price, double stoploss, double takeprofit, double volume, byte type, string symbol);
        Task<bool> DeleteOrder(string email, string password, string server, bool isLive, long accountId, long positionId);
        Task<double> GetPrice(string symbol, string email = "alihuim1xar1@gmail.com", string password = "gCvDDs7$", string server = "OSP-DEMO", bool isLive = false, long accountId = 518920);
    }
}
