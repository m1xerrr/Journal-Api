using Microsoft.AspNetCore.Mvc;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;

namespace Journal.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IMTAccountService _mtAccountService;
        private readonly ICTraderAccountService _ctraderAccountService;
        public AccountsController(IMTAccountService mTAccountService, ICTraderAccountService cTraderAccountService)
        {
            _mtAccountService = mTAccountService;
            _ctraderAccountService = cTraderAccountService;
        }

        [HttpPost("AddMTAccount")]
        public async Task<IActionResult> AddMTAccount([FromBody] MTAccountJsonModel mtAccountModel)
        {
            var response = await _mtAccountService.AddAccount(mtAccountModel);
            return Json(response);
        }
        [HttpPost("DeleteMTAccount")]
        public async Task<IActionResult> DeleteMTAccount([FromBody] Guid accountId)
        {
            var response = await _mtAccountService.DeleteMTAccount(accountId);
            return Json(response);
        }
        [HttpPost("AddCTraderAccount")]
        public async Task<IActionResult> AddCTraderAccount([FromBody] CTraderAccountJsonModel model)
        {
            var response = await _ctraderAccountService.AddAccounts(model.AccessToken, model.UserId);
            return Json(response);
        }
        


        [HttpPost("GetCTraderAccessToken")]
        public async Task<IActionResult> GetCTraderAccessToken([FromBody] string authorizationLink)
        {
            var response = await _ctraderAccountService.GetAccessToken(authorizationLink);
            return Json(response);
        }
    }
}
