using Journal.Domain.Enums;
using Journal.Domain.JsonModels.TradeLocker;
using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Interfaces
{
    public interface ITradeLockerAccountService
    {
        Task<BaseResponse<AccountResponseModel>> AddAccount(string email, string server, string password, bool isLive, Guid UserId, long accountId);

        Task<BaseResponse<bool>> DeleteAccountTradeLockerAccount(Guid id);

        Task<BaseResponse<bool>> LoadAccountData(Guid id);

        Task<BaseResponse<AccountData>> GetAccountData(Guid id);

        Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId);

        Task<BaseResponse<List<string>>> GetSymbols(Guid id);

        Task<BaseResponse<TradeLockerOrdersJsonModel>> GetOrders(Guid id);

        Task<BaseResponse<TradeLockerPositionsJsonModel>> GetPositions(Guid id);

        Task<BaseResponse<bool>> PlaceOrder(TradeLockerPlaceOrderJsonModel model);

        Task<BaseResponse<bool>> DeleteOrder(TradeLockerCloseOrderJsonModel model);
    }
}
