using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Domain.ResponseModels;

namespace Journal.Service.Interfaces
{
    public interface ICTraderAccountService
    {
        Task<BaseResponse<List<CTraderAccountResponseModel>>> AddAccounts(string accessToken, Guid UserId);

        Task<BaseResponse<string>> GetAccessToken(string authorizationLink);
    }
}
