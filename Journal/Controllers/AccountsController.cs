using Microsoft.AspNetCore.Mvc;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;

namespace Journal.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IMTAccountService _mtAccountService;
        public AccountsController(IMTAccountService mTAccountService)
        {
            _mtAccountService = mTAccountService;
        }

        [HttpPost("AddMTAccount")]
        public async Task<IActionResult> AddMTAccount([FromBody] MTAccountJsonModel mtAccountModel)
        {
            return Json(_mtAccountService.AddAccount(mtAccountModel));
        }
        [HttpPost("DeleteMTAccount")]
        public async Task<IActionResult> DeleteMTAccount([FromBody] Guid accountId)
        {
            return Json(_mtAccountService.DeleteMTAccount(accountId));
        }
    }
}
