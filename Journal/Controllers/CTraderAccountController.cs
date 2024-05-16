using Journal.Domain.JsonModels.CTrader;
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

        [HttpPost("GetCTraderAccountOrders")]
        public async Task<IActionResult> GetCTraderAccountOrders([FromBody] Guid accountId)
        {
            var response = await _ctraderAccountService.GetOrders(accountId);
            return Json(response);
        }

        [HttpPost("GetCTraderAccountPositions")]
        public async Task<IActionResult> GetCTraderAccountPositions([FromBody] Guid accountId)
        {
            var response = await _ctraderAccountService.GetPositions(accountId);
            return Json(response);
        }

        [HttpPost("GetCTraderAccountSymbols")]
        public async Task<IActionResult> GetCTraderAccountSymbols([FromBody] Guid accountId)
        {
            var response = await _ctraderAccountService.GetSymbols(accountId);
            return Json(response);
        }

        [HttpPost("OpenCTraderPosition")]
        public async Task<IActionResult> OpenCTraderPosition([FromBody] CTraderOpenPositionJsonModel model)
        {
            var response = await _ctraderAccountService.PlaceOrder(model.AccountId, model.Symbol, model.Type, model.Volume, model.Stoploss, model.TakeProfit, model.Price);
            return Json(response);
        }

        [HttpPost("CloseCTraderPosition")]
        public async Task<IActionResult> CloseCTraderPosition([FromBody] CTraderClosePositionJsonModel model)
        {
            var response = await _ctraderAccountService.CloseOrder(model.AccountId, model.Id);
            return Json(response);
        }
    }
}
