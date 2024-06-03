using Journal.Domain.JsonModels.TradingAccount;
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
    public interface IMatchTradeAccountService
    {
        Task<BaseResponse<AccountResponseModel>> AddAccount(string email, int brokerId, string password, bool isLive, Guid UserId, long accountId);

        Task<BaseResponse<bool>> DeleteAccount(Guid id);

        Task<BaseResponse<bool>> LoadAccountData(Guid id);

        Task<BaseResponse<AccountData>> GetAccountData(Guid id);

        Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId);

        Task<BaseResponse<List<OrderResponseModel>>> GetOrders(Guid id);

        Task<BaseResponse<List<PositionResponseModel>>> GetPositions(Guid id);

        Task<BaseResponse<bool>> PlaceOrder(OpenPositionJsonModel model);

        Task<BaseResponse<bool>> DeleteOrder(ClosePositionJsonModel model);
    }
}
