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
        private readonly IMTDataService _mtDataService;
        private readonly IDealService _mtDealService;
        private readonly ICTraderAccountService _ctraderAccountService;

        public UserAccountsController(IMTDataService mTDataService, IDealService mtDealService, ICTraderAccountService ctraderAccountService)
        {
            _mtDataService = mTDataService;
            _mtDealService = mtDealService;
            _ctraderAccountService = ctraderAccountService;
        }

        [HttpPost("TradingAccountData")]
        public async Task<IActionResult> TradingAccountData([FromBody] TradingAccountJsonModel account)
        {
            BaseResponse<AccountData> response;
            switch (account.Provider)
            {
                case "MetaTrader 5":
                    response = await _mtDataService.GetAccountData(account.AccountId);
                    break;
                case "CTrader":
                    response = await _ctraderAccountService.GetAccountData(account.AccountId);
                    break;
                default:
                    response = new BaseResponse<AccountData> { };
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Invalid provider name";
                    break;
            }
            return Json(response);
        }

        [HttpPost("AddDealImg")]
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
        }

        [HttpPost("GetDeal")]
        public async Task<IActionResult> Deal([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.GetDeal(deal.Id, deal.accountId);
            return Json(response);
        }
    }
}
