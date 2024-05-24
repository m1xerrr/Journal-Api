using Journal.Domain.JsonModels.TradeLocker;
using Journal.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Interfaces
{
    public interface ITradingAccountService
    {
        Task<BaseResponse<List<string>>> GetSymbols(List<TradingAccountJsonModel> accounts);
    }
}
