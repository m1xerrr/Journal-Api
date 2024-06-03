using Journal.Domain.JsonModels.MatchTrade;
using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Journal.Controllers
{
    public class MatchTradeAccountController : Controller
    {
        private readonly IMatchTradeAccountService _matchTradeAccountService;
        public MatchTradeAccountController(IMatchTradeAccountService matchTradeAccountService)
        {
            _matchTradeAccountService = matchTradeAccountService;
        }

        [HttpPost("AddMatchTradeAccount")]
        public async Task<IActionResult> AddMatchTradeAccount([FromBody] MatchTradeAccountJsonModel model)
        {
            var response = await _matchTradeAccountService.AddAccount(model.Email, model.BrokerId, model.Password, model.IsLive, model.UserId, model.AccountId);
            return Json(response);
        }
    }
}
