using Journal.Domain.JsonModels.CTrader;
using Journal.Domain.JsonModels.DXTrade;
using Journal.Domain.JsonModels.MetaTrader;
using Journal.Domain.JsonModels.TradeLocker;
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

        [HttpPost("GetMTOrders")]
        public async Task<IActionResult> GetMTOrders([FromBody] Guid accountId)
        {
            var response = await _mtAccountService.GetOrders(accountId);
            return Json(response);
        }

        [HttpPost("GetMTPositions")]
        public async Task<IActionResult> GetMTPositions([FromBody] Guid accountId)
        {
            var response = await _mtAccountService.GetPositions(accountId);
            return Json(response);
        }

        [HttpPost("GetMTSymbols")]
        public async Task<IActionResult> GetMTSymbols([FromBody] Guid accountId)
        {
            var response = await _mtAccountService.GetSymbols(accountId);
            return Json(response);
        }

        [HttpPost("OpenMTPosition")]
        public async Task<IActionResult> OpenMTPosition([FromBody] MTOpenPositionJsonModel model)
        {
            var response = await _mtAccountService.OpenPosition(model);
            return Json(response);
        }

        [HttpPost("CloseMTPosition")]
        public async Task<IActionResult> CloseMTPosition([FromBody] MTCloseOrderJsonModel model)
        {
            var response = await _mtAccountService.CloseOrder(model);
            return Json(response);
        }

    }
}
