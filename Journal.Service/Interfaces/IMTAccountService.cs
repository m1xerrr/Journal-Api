using Journal.Domain.Responses;
using Journal.Domain.ViewModels;
using Journal.Domain.Models;

namespace Journal.Service.Interfaces
{
    public interface IMTAccountService
    {
        Task<BaseResponse<MTAccount>> AddAccount(MTAccountViewModel accountModel);
    }
}
