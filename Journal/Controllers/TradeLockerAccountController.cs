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

        [HttpPost("GetTradeLockerSymbols")]
        public async Task<IActionResult> GetTradeLockerSymbols([FromBody] Guid id)
        {
            var response = await _tradeLockerAccountService.GetSymbols(id);
            return Json(response);
        }

        [HttpPost("GetTradeLockerOrders")]
        public async Task<IActionResult> GetTradeLockerOrders([FromBody] Guid id)
        {
            var response = await _tradeLockerAccountService.GetOrders(id);
            return Json(response);
        }

        [HttpPost("GetTradeLockerPositions")]
        public async Task<IActionResult> GetTradeLockerPositions([FromBody] Guid id)
        {
            var response = await _tradeLockerAccountService.GetPositions(id);
            return Json(response);
        }

        [HttpPost("PlaceTradeLockerOrder")]
        public async Task<IActionResult> PlaceTradeLockerOrder([FromBody] TradeLockerPlaceOrderJsonModel model)
        {
            var response = await _tradeLockerAccountService.PlaceOrder(model);
            return Json(response);
        }

        [HttpPost("DeleteTradeLockerOrder")]
        public async Task<IActionResult> DeleteTradeLockerOrder([FromBody] TradeLockerCloseOrderJsonModel model)
        {
            var response = await _tradeLockerAccountService.DeleteOrder(model);
            return Json(response);
        }
    }
}
