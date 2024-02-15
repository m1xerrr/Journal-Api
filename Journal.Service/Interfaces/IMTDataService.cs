using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Domain.ViewModels;

namespace Journal.Service.Interfaces
{
    public interface IMTDataService
    {
        Task<BaseResponse<MTAccountData>> GetAccountData(MTAccountViewModel account);
    }
}
