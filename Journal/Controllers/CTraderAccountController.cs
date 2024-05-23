using Journal.Domain.JsonModels.CTrader;
using Journal.Domain.JsonModels.TradingAccount;
using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Journal.Controllers
{
    public class CTraderAccountController : Controller
    {
        private readonly ICTraderAccountService _ctraderAccountService;
        public CTraderAccountController(ICTraderAccountService cTraderAccountService)
        {
            _ctraderAccountService = cTraderAccountService;
        }
        [HttpPost("AddCTraderAccount")]
        public async Task<IActionResult> AddCTraderAccount([FromBody] CTraderAccountJsonModel model)
        {
            var response = await _ctraderAccountService.AddAccount(model.AccessToken, model.UserId, model.AccountId);
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
