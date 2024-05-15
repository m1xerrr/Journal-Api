using Journal.Domain.JsonModels.DXTrade;
using Journal.Domain.JsonModels.TradeLocker;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Journal.Controllers
{
    public class DXTradeAccountController : Controller
    {
        private readonly IDXTradeAccountService _dxTradeAccountService;
        public DXTradeAccountController(IDXTradeAccountService traderAccountService)
        {
            _dxTradeAccountService = traderAccountService;
        }

        [HttpPost("AddDXTradeAccount")]
        public async Task<IActionResult> AddDXTradeAccount([FromBody] DXTradeAccountJsonModel model)
        {
            var response = await _dxTradeAccountService.AddAccounts(model.Username, model.Password, model.Domain, model.UserId);
            return Json(response);
        }
    }
}
