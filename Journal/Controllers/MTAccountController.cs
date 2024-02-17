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

        [HttpPost("account")]
        public async Task<IActionResult> MTAccountData([FromBody] MTAccountJsonModel account)
        {
            var response = await _mtDataService.GetAccountData(account);
            return Json(response);
        }

        [HttpPost("dealImage")]
        public async Task<IActionResult> MTDealAddImg([FromBody] MTDealAddJsonModel deal)
        {
            var response = await _mtDealService.AddImage(deal.Id, deal.Field);
            return Json(response);
        }

        [HttpPost("dealNote")]
       
        public async Task<IActionResult> MTDealAddNote([FromBody] MTDealAddJsonModel deal)
        {
            var response = await _mtDealService.AddNotes(deal.Id, deal.Field);
            return Json(response);
        }

        [HttpPost("deal")]

        public async Task<IActionResult> MTDeal([FromBody] int id)
        {
            var response = await _mtDealService.GetDeal(id);
            return Json(response);
        }
    }
}
