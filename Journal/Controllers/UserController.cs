using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;
using Journal.Domain.Models;

namespace Journal.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMTAccountService _mtAccountService;
        private readonly ICTraderAccountService _ctraderAccountService;
        private readonly IDXTradeAccountService _dxTradeAccountService;
        public UserController(IUserService userService, IMTAccountService mTAccountService, ICTraderAccountService ctraderAccountService, IDXTradeAccountService dXTradeAccountService)
        {
            _userService = userService;
            _mtAccountService = mTAccountService;
            _ctraderAccountService = ctraderAccountService;
            _dxTradeAccountService = dXTradeAccountService;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserJsonModel user)
        {
            var response = await _userService.Verify(user);
            return Json(response);
        }
        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpUserJsonModel user)
        {
            var response = await _userService.CreateAccount(user);
            return Json(response);
        }
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> Delete([FromBody] Guid userId)
        {
            var response = await _userService.DeleteAccount(userId);
            return Json(response);
        }
        [HttpPost("UserTradingAccounts")]
        public async Task<IActionResult> UserTradingAccounts([FromBody] Guid userId)
        {
            var responseMT = await _mtAccountService.GetMTAccountsByUser(userId);
            var responseCT = await _ctraderAccountService.GetUserAccounts(userId);
            var responseDX = await _dxTradeAccountService.GetUserAccounts(userId);
            if (responseMT.StatusCode == Domain.Enums.StatusCode.OK && responseCT.StatusCode == Domain.Enums.StatusCode.OK)
            {
                foreach(var account in responseCT.Data)
                {
                    responseMT.Data.Add(account);
                }
                if(responseDX.StatusCode == Domain.Enums.StatusCode.OK)
                {
                    foreach (var account in responseDX.Data)
                    {
                        responseMT.Data.Add(account);
                    }
                }
                else
                {
                    return Json(responseDX);
                }
            }
            return Json(responseMT);
        }

        [HttpPost("ChangeUsername")]
        public async Task<IActionResult> ChangeName([FromBody] EditUserJsonModel user)
        {
            var response = await _userService.ChangeName(user);
            return Json(response);
        }
        [HttpPost("ChangeUserPassword")]
        public async Task<IActionResult> ChangePassword([FromBody] EditUserJsonModel user)
        {
            var response = await _userService.ChangePassword(user);
            return Json(response);
        }
        [HttpPost("ChangeUserEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] EditUserJsonModel user)
        {
            var response = await _userService.ChangeEmail(user);
            return Json(response);
        }
        [HttpPost("ChangeUserRole")]
        public async Task<IActionResult> ChangeRole([FromBody] EditUserJsonModel user)
        {
            var response = await _userService.ChangeRole(user);
            return Json(response);
        }
        [HttpPost("EditUser")]
        public async Task<IActionResult> EditUser([FromBody] EditUserJsonModel user)
        {
            var response = await _userService.EditUser(user);
            return Json(response);
        }
        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsers();
            return Json(response);
        }

        [HttpPost("DeleteUserSubscription")]
        public async Task<IActionResult> DeleteSubscription([FromBody] Guid id)
        {
            var response = await _userService.DeleteSubscription(id);
            return Json(response);
        }
        [HttpPost("GetLeaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var response = await _userService.GetLeaderboard();
            return Json(response);
        }

        [HttpPost("ShareProfit")]
        public async Task<IActionResult> ShareProfit([FromBody] AccountPeriodJsonModel account)
        {
            var response = await _userService.GetProfit(account.AccountId, account.Provider, account.StartDate, account.EndDate);
            return Json(response);
        }
    }
}
