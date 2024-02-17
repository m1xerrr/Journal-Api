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

        [HttpPost("Data")]
        public async Task<IActionResult> MTAccountData([FromBody] MTAccountJsonModel account)
        {
            return Json(await _mtDataService.GetAccountData(account));
        }

        [HttpPost("AddDealImg")]
        public async Task<IActionResult> MTDealAddImg([FromBody] MTDealAddJsonModel deal)
        {
            return Json(await _mtDealService.AddImage(deal.Id, deal.Field));
        }

        [HttpPost("AddDealNote")]
       
        public async Task<IActionResult> MTDealAddNote([FromBody] MTDealAddJsonModel deal)
        {
            return Json(await _mtDealService.AddNotes(deal.Id, deal.Field));
        }

        [HttpPost("Deal")]
        public async Task<IActionResult> MTDeal([FromBody] int id)
        {
            return Json(await _mtDealService.GetDeal(id));
        }
    }
}
