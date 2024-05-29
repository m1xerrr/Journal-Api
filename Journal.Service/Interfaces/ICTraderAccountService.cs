using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Domain.ResponseModels;
using Journal.Domain.Models;
using Google.Protobuf.Collections;
using Journal.Domain.JsonModels.TradingAccount;

namespace Journal.Service.Interfaces
{
    public interface ICTraderAccountService
    {
        Task<BaseResponse<AccountResponseModel>> AddAccount(string accessToken, Guid UserId, long accountId);

        Task<BaseResponse<string>> GetAccessToken(string authorizationLink);

        Task<BaseResponse<bool>> DeleteCTraderAccount(Guid id);

        Task<BaseResponse<bool>> LoadAccountData(Guid id);

        Task<BaseResponse<AccountData>> GetAccountData(Guid id);

        Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId);

        Task<BaseResponse<List<OrderResponseModel>>> GetOrders(Guid accountId);

        Task<BaseResponse<List<PositionResponseModel>>> GetPositions(Guid accountId);

        Task<BaseResponse<bool>> PlaceOrder(OpenPositionJsonModel model);

        Task<BaseResponse<List<string>>> GetSymbols(Guid accountId);

        Task<BaseResponse<bool>> CloseOrder(Guid accountId, long id);
    }
}
