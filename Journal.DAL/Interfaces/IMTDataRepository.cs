using Journal.Domain.JsonModels.MetaTrader;
using Journal.Domain.ResponseModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Interfaces
{
    public interface IMTDataRepository
    {
        Task<List<MTDealJsonModel>> GetDeals(MTAccountJsonModel account);

        Task<bool> Initialize(MTAccountJsonModel account);

        Task<bool> DeleteOrder(int login, string password, string server, long ticket);

        Task<List<MTOrderJsonModel>> GetOrders(MTAccountJsonModel account);

        Task<List<MTPositionJsonModel>> GetPositions(MTAccountJsonModel account);

        Task<List<string>> GetSymbols(MTAccountJsonModel account);

        Task<bool> PlaceOrder(int login, string password, string server, string symbol, double volume, byte type, float price, float stoploss, float takeprofit);

    }
}
