using Journal.Domain.Responses;
using Journal.Domain.ResponseModels;
using Journal.Domain.Models;
using Journal.Domain.JsonModels.MetaTrader;
using Journal.Domain.Enums;

namespace Journal.Service.Interfaces
{
    public interface IMTAccountService
    {
        Task<BaseResponse<AccountResponseModel>> AddAccount(MTAccountJsonModel accountModel);

        Task<BaseResponse<bool>> DeleteMTAccount(Guid accountId);

        Task<BaseResponse<List<AccountResponseModel>>> GetMTAccountsByUser(Guid userId);

        Task<BaseResponse<bool>> LoadAccountData(Guid accountId);

        Task<BaseResponse<AccountData>> GetAccountData(Guid accountId);

        Task<BaseResponse<bool>> OpenPosition(MTOpenPositionJsonModel model);

        Task<BaseResponse<List<MTOrderJsonModel>>> GetOrders(Guid accountId);

        Task<BaseResponse<List<string>>> GetSymbols(Guid accountId);

        Task<BaseResponse<List<MTPositionJsonModel>>> GetPositions(Guid accountId);

        Task<BaseResponse<bool>> CloseOrder(MTCloseOrderJsonModel model);
    }
}
