using Journal.Domain.JsonModels.DXTrade;
using Journal.Domain.JsonModels.TradeLocker;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Journal.Controllers
{
    public class TradeLockerAccountController : Controller
    {
        private readonly ITradeLockerAccountService _tradeLockerAccountService;
        public TradeLockerAccountController(ITradeLockerAccountService tradeLockerAccountService)
        {
            _tradeLockerAccountService = tradeLockerAccountService;
        }

        [HttpPost("AddTradeLockerAccount")]
        public async Task<IActionResult> AddTradeLockerAccount([FromBody] TradeLockerAccountJsonModel model)
        {
            var response = await _tradeLockerAccountService.AddAccount(model.Email, model.Server, model.Password, model.isLive, model.UserId, model.AccountId);
            return Json(response);
        }
    }
}
