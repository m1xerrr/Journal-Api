using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Domain.ResponseModels;
using Journal.Domain.Models;

namespace Journal.Service.Interfaces
{
    public interface ICTraderAccountService
    {
        Task<BaseResponse<List<AccountResponseModel>>> AddAccount(string accessToken, Guid UserId, long accountId);

        Task<BaseResponse<string>> GetAccessToken(string authorizationLink);

        Task<BaseResponse<bool>> DeleteCTraderAccount(Guid id);

        Task<BaseResponse<bool>> LoadAccountData(Guid id);

        Task<BaseResponse<AccountData>> GetAccountData(Guid id);

        Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId);
    }
}
