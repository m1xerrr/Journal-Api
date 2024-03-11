using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Interfaces
{
    public interface IDXTradeAccountService
    {
        Task<BaseResponse<List<AccountResponseModel>>> AddAccounts(string login, string password, string domain, Guid UserId);

        Task<BaseResponse<bool>> DeleteCTraderAccount(Guid id);

        Task<BaseResponse<AccountData>> GetAccountData(Guid id);

        Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId);
    }
}
