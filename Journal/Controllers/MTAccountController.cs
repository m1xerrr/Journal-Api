using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Journal.Domain.JsonModels;

namespace Journal.Controllers
{
    public class MTAccountController : Controller
    {
        private readonly IMTDataService _mtDataService;
        private readonly IMTDealService _mtDealService;

        public MTAccountController(IMTDataService mTDataService, IMTDealService mtDealService)
        {
            _mtDataService = mTDataService;
            _mtDealService = mtDealService;
        }

        [HttpPost("MTAccountData")]
        public async Task<IActionResult> MTAccountData([FromBody] Guid accountId)
        {
            var response = await _mtDataService.GetAccountData(accountId);
            return Json(response);
        }

        [HttpPost("AddDealImg")]
        public async Task<IActionResult> MTDealAddImg([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.AddImage(deal.Id, deal.accountId, deal.Field);
            return Json(response);
        }

        [HttpPost("AddDealNote")]
       
        public async Task<IActionResult> MTDealAddNote([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.AddNotes(deal.Id, deal.accountId, deal.Field);
            return Json(response);
        }

        [HttpPost("GetMTDeal")]
        public async Task<IActionResult> MTDeal([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.GetDeal(deal.Id, deal.accountId);
            return Json(response);
        }
    }
}
