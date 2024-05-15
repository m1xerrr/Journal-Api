using Microsoft.AspNetCore.Mvc;
using Journal.Service.Interfaces;
using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Service.Implementations;
using Microsoft.Identity.Client;
using Journal.Domain.JsonModels.MetaTrader;
using Journal.Domain.JsonModels.DXTrade;
using Journal.Domain.JsonModels.CTrader;
using Journal.Domain.JsonModels.TradeLocker;
using Journal.Domain.JsonModels.Deal;

namespace Journal.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IMTAccountService _mtAccountService;
        private readonly ICTraderAccountService _ctraderAccountService;
        private readonly IDXTradeAccountService _dxTradeAccountService;
        private readonly ITradeLockerAccountService _tradeLockerAccountService;
        private readonly IDealService _dealService;
        public AccountsController(IMTAccountService mTAccountService, ICTraderAccountService cTraderAccountService, IDXTradeAccountService traderAccountService, ITradeLockerAccountService tradeLockerAccountService, IDealService dealService)
        {
            _mtAccountService = mTAccountService;
            _ctraderAccountService = cTraderAccountService;
            _dxTradeAccountService = traderAccountService;
            _tradeLockerAccountService = tradeLockerAccountService;
            _dealService = dealService;
        }

        
        [HttpPost("DeleteTradingAccount")]
        public async Task<IActionResult> DeleteTradingAccount([FromBody] TradingAccountJsonModel account)
        {
            BaseResponse<bool> response;
            switch (account.Provider)
            {
                case "MetaTrader 5":
                    response = await _mtAccountService.DeleteMTAccount(account.AccountId);
                    break;
                case "CTrader":
                    response = await _ctraderAccountService.DeleteCTraderAccount(account.AccountId);
                    break;
                case "DXTrade":
                    response = await _dxTradeAccountService.DeleteDXTradeAccount(account.AccountId);
                    break;
                case "TradeLocker":
                    response = await _tradeLockerAccountService.DeleteAccountTradeLockerAccount(account.AccountId);
                    break;
                default:
                    response = new BaseResponse<bool> { };
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Invalid provider name";
                    break;
            }
            return Json(response);
        }
        [HttpPost("TradingAccountData")]
        public async Task<IActionResult> TradingAccountData([FromBody] TradingAccountJsonModel account)
        {
            BaseResponse<AccountData> response = new BaseResponse<AccountData> { };
            switch (account.Provider)
            {
                case "MetaTrader 5":
                    response = await _mtAccountService.GetAccountData(account.AccountId);
                    response.Data.Provider = account.Provider;
                    response.Data.Id = account.AccountId;
                    break;
                case "CTrader":
                    response = await _ctraderAccountService.GetAccountData(account.AccountId);
                    response.Data.Provider = account.Provider;
                    response.Data.Id = account.AccountId;
                    break;
                case "DXTrade":
                    response = await _dxTradeAccountService.GetAccountData(account.AccountId);
                    response.Data.Provider = account.Provider;
                    response.Data.Id = account.AccountId;
                    break;
                case "TradeLocker":
                    response = await _tradeLockerAccountService.GetAccountData(account.AccountId);
                    response.Data.Provider = account.Provider;
                    response.Data.Id = account.AccountId;
                    break;
                default:
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Invalid provider name";
                    break;
            }
            return Json(response);
        }

        [HttpPost("LoadTradingAccountData")]
        public async Task<IActionResult> LoadTradingAccountData([FromBody] TradingAccountJsonModel account)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            switch (account.Provider)
            {
                case "MetaTrader 5":
                    response = await _mtAccountService.LoadAccountData(account.AccountId);
                    break;
                case "CTrader":
                    response = await _ctraderAccountService.LoadAccountData(account.AccountId);
                    break;
                case "DXTrade":
                    response = await _dxTradeAccountService.LoadAccountData(account.AccountId);
                    break;
                case "TradeLocker":
                    response = await _tradeLockerAccountService.LoadAccountData(account.AccountId);
                    break;
                default:
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Invalid provider name";
                    break;
            }
            return Json(response);
        }

        [HttpPost("AddDealDescription")]
        public async Task<IActionResult> AddDealDescription([FromBody] DealEditJsonModel deal)
        {
            var response = await _dealService.AddDescriptionItem(deal.Id, deal.accountId, deal.Field, deal.Type);
            return Json(response);
        }

        [HttpPost("DeleteDealDescription")]
        public async Task<IActionResult> DeleteDescription([FromBody] Guid itemId)
        {
            var response = await _dealService.DeleteDescriptionItem(itemId);
            return Json(response);
        }

        [HttpPost("EditDealDescription")]
        public async Task<IActionResult> EditDealDescription([FromBody] DescriptionEditJsonModel model)
        {
            var response = await _dealService.EditDescriptionItem(model.Id, model.Field);
            return Json(response);
        }

        [HttpPost("GetDeal")]
        public async Task<IActionResult> Deal([FromBody] DealEditJsonModel deal)
        {
            var response = await _dealService.GetDeal(deal.Id, deal.accountId);
            return Json(response);
        }
    }
}
