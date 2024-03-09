using Microsoft.AspNetCore.Mvc;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;
using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Service.Implementations;
using Microsoft.Identity.Client;

namespace Journal.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IMTAccountService _mtAccountService;
        private readonly ICTraderAccountService _ctraderAccountService;
        public AccountsController(IMTAccountService mTAccountService, ICTraderAccountService cTraderAccountService)
        {
            _mtAccountService = mTAccountService;
            _ctraderAccountService = cTraderAccountService;
        }

        [HttpPost("AddMTAccount")]
        public async Task<IActionResult> AddMTAccount([FromBody] MTAccountJsonModel mtAccountModel)
        {
            var response = await _mtAccountService.AddAccount(mtAccountModel);
            return Json(response);
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
                default:
                    response = new BaseResponse<bool> { };
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Invalid provider name";
                    break;
            }
            return Json(response);
        }
        [HttpPost("AddCTraderAccount")]
        public async Task<IActionResult> AddCTraderAccount([FromBody] CTraderAccountJsonModel model)
        {
            var response = await _ctraderAccountService.AddAccounts(model.AccessToken, model.UserId);
            return Json(response);
        }

        [HttpPost("GetCTraderAccessToken")]
        public async Task<IActionResult> GetCTraderAccessToken([FromBody] string authorizationLink)
        {
            var response = await _ctraderAccountService.GetAccessToken(authorizationLink);
            return Json(response);
        }
    }
}
