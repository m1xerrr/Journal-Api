using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Journal.Domain.JsonModels;

namespace Journal.Controllers
{
    public class UserAccountsController : Controller
    {
        private readonly IMTDataService _mtDataService;
        private readonly IDealService _mtDealService;
        private readonly ICTraderAccountService _ctraderAccountService;

        public UserAccountsController(IMTDataService mTDataService, IDealService mtDealService, ICTraderAccountService ctraderAccountService)
        {
            _mtDataService = mTDataService;
            _mtDealService = mtDealService;
            _ctraderAccountService = ctraderAccountService;
        }

        [HttpPost("MTAccountData")]
        public async Task<IActionResult> MTAccountData([FromBody] Guid accountId)
        {
            var response = await _mtDataService.GetAccountData(accountId);
            return Json(response);
        }

        [HttpPost("CTraderAccountData")]
        public async Task<IActionResult> CTraderAccountData([FromBody] Guid accountId)
        {
            var response = await _ctraderAccountService.GetAccountData(accountId);
            return Json(response);
        }

        [HttpPost("AddDealImg")]
        public async Task<IActionResult> DealAddImg([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.AddImage(deal.Id, deal.accountId, deal.Field);
            return Json(response);
        }

        [HttpPost("AddDealNote")]
       
        public async Task<IActionResult> DealAddNote([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.AddNotes(deal.Id, deal.accountId, deal.Field);
            return Json(response);
        }

        [HttpPost("GetDeal")]
        public async Task<IActionResult> Deal([FromBody] DealEditJsonModel deal)
        {
            var response = await _mtDealService.GetDeal(deal.Id, deal.accountId);
            return Json(response);
        }
    }
}
