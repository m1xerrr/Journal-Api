using Azure;
using Journal.DAL.Interfaces;
using Journal.Domain.JsonModels.TradeLocker;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Implementations
{
    public class TradingAccountService : ITradingAccountService
    {
        private readonly IMTAccountService _mtAccountService;
        private readonly IDXTradeAccountService _dxtraderAccountService;
        private readonly ICTraderAccountService _ctraderAccountService;
        private readonly ITradeLockerAccountService _tradeLockerAccountService;

        public TradingAccountService(IMTAccountService mtAccountService, IDXTradeAccountService dxtraderAccountService, ICTraderAccountService ctraderAccountService, ITradeLockerAccountService tradeLockerAccountService)
        {
            _mtAccountService = mtAccountService;
            _ctraderAccountService = ctraderAccountService;
            _dxtraderAccountService = dxtraderAccountService;
            _tradeLockerAccountService = tradeLockerAccountService;
        }

        public async Task<BaseResponse<List<string>>> GetSymbols(List<TradingAccountJsonModel> accounts)
        {
            return null;
        }
    }
}
