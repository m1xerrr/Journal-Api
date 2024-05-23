using Journal.Domain.JsonModels.CTrader;
using Journal.Domain.JsonModels.DXTrade;
using Journal.Domain.JsonModels.MetaTrader;
using Journal.Domain.JsonModels.TradeLocker;
using Journal.Domain.JsonModels.TradingAccount;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Journal.Controllers
{
    public class MTAccountController : Controller
    {
        private readonly IMTAccountService _mtAccountService;
        public MTAccountController(IMTAccountService mTAccountService)
        {
            _mtAccountService = mTAccountService;
        }

        [HttpPost("AddMTAccount")]
        public async Task<IActionResult> AddMTAccount([FromBody] MTAccountJsonModel mtAccountModel)
        {
            var response = await _mtAccountService.AddAccount(mtAccountModel);
            return Json(response);
        }

    }
}
