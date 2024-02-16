using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Journal.Domain.ViewModels;

namespace Journal.Controllers
{
    public class MTAccountController : Controller
    {
        private readonly IMTDataService _mtDataService;

        public MTAccountController(IMTDataService mTDataService)
        {
            _mtDataService = mTDataService;
        }

        [HttpPost("account")]
        public async Task<IActionResult> MTAccountData([FromBody] MTAccountViewModel account)
        {
            var response = await _mtDataService.GetAccountData(account);
            return Json(response);
        }
    }
}
