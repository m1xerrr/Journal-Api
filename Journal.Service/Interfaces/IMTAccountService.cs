using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Domain.ResponseModels;

namespace Journal.Service.Interfaces
{
    public interface IMTAccountService
    {
        Task<BaseResponse<AccountResponseModel>> AddAccount(MTAccountJsonModel accountModel);

        Task<BaseResponse<bool>> DeleteMTAccount(Guid accountId);

        Task<BaseResponse<List<AccountResponseModel>>> GetMTAccountsByUser(Guid userId);
    }
}
