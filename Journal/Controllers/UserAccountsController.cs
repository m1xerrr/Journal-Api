using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Journal.Domain.JsonModels;
using Microsoft.Identity.Client;
using Journal.Domain.Responses;
using Journal.Domain.Models;

namespace Journal.Controllers
{
    public class UserAccountsController : Controller
    {
        private readonly IMTAccountService _mtAccountService;
        private readonly IDealService _mtDealService;
        private readonly ICTraderAccountService _ctraderAccountService;
        private readonly IDXTradeAccountService _dxTradeAccountService;
        private readonly ITradeLockerAccountService _tradeLockerAccountService;

        public UserAccountsController(IMTAccountService mTAccountService, IDealService mtDealService, ICTraderAccountService ctraderAccountService, IDXTradeAccountService dXTradeAccountService, ITradeLockerAccountService tradeLockerAccountService)
        {
            _mtAccountService = mTAccountService;
            _mtDealService = mtDealService;
            _ctraderAccountService = ctraderAccountService;
            _dxTradeAccountService = dXTradeAccountService;
            _tradeLockerAccountService = tradeLockerAccountService;
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

        /*[HttpPost("AddDealImg")]
        public async Task<IActionResult> DealAddImg([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.AddImage(deal.Id, deal.accountId, deal.Field);
            return Json(response);
        }

        [HttpPost("AddDealNote")]
       
        public async Task<IActionResult> DealAddNote([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.AddNotes(deal.Id, deal.accountId, deal.Field);
            return Json(response);
        }*/

        [HttpPost("AddDealDescription")]
        public async Task<IActionResult> AddDealDescription([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.AddDescriptionItem(deal.Id, deal.accountId, deal.Field, deal.Type);
            return Json(response);
        }

        [HttpPost("DeleteDealDescription")]
        public async Task<IActionResult> DeleteDescription([FromBody] Guid itemId)
        {
            var response = await _mtDealService.DeleteDescriptionItem(itemId);
            return Json(response);
        }

        [HttpPost("EditDealDescription")]
        public async Task<IActionResult> EditDealDescription([FromBody] DescriptionEditJsonModel model)
        {
            var response = await _mtDealService.EditDescriptionItem(model.Id, model.Field);
            return Json(response);
        }

        [HttpPost("GetDeal")]
        public async Task<IActionResult> Deal([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.GetDeal(deal.Id, deal.accountId);
            return Json(response);
        }
    }
}
