using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Domain.JsonModels;

namespace Journal.Service.Interfaces
{
    public interface IMTDataService
    {
        Task<BaseResponse<AccountData>> GetAccountData(Guid accountId);

    }
}
