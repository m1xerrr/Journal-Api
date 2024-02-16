using Journal.Domain.Responses;
using Journal.Domain.ViewModels;
using Journal.Domain.ResponseModels;

namespace Journal.Service.Interfaces
{
    public interface IMTAccountService
    {
        Task<BaseResponse<MTAccountResponseModel>> AddAccount(MTAccountViewModel accountModel);
    }
}
