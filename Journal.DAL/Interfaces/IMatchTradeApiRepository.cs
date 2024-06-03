using Journal.Domain.JsonModels.MatchTrade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Interfaces
{
    public interface IMatchTradeApiRepository
    {
        Task<InitializeAccountJsonModel> InitializeSession(bool isLive, string email, string password, int brokerId, long accountNumber);

        Task<double> GetDeposit(bool isLive, string email, string password, int brokerId, long accountNumber);

        Task<List<MatchTradeDealJsonModel>> GetDeals(bool isLive, string email, string password, int brokerId, long accountNumber, string TradingApiToken, string CoAuthToken);

        Task<MatchTradePositionsJsonModel> GetPositions(bool isLive, string email, string password, int brokerId, long accountNumber);

        Task<MatchTraderOrdersJsonModel> GetOrders(bool isLive, string email, string password, int brokerId, long accountNumber);

        Task<bool> OpenPosition(bool isLive, string email, string password, int brokerId, long accountNumber, double price, double stoploss, double takeprofit, double volume, byte type, string symbol);

        Task<bool> ClosePosition(bool isLive, string email, string password, int brokerId, long accountNumber, string positionId, string instrument, string side, string type, double volume);
    }
}
