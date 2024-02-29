using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Domain.ResponseModels;

namespace Journal.Service.Interfaces
{
    public interface IMTAccountService
    {
        Task<BaseResponse<MTAccountResponseModel>> AddAccount(MTAccountJsonModel accountModel);

        Task<BaseResponse<bool>> DeleteMTAccount(Guid accountId);

        Task<BaseResponse<List<MTAccountResponseModel>>> GetMTAccountsByUser(Guid userId);
    }
}
