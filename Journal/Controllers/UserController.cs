using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Journal.Service.Interfaces;
using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Domain.JsonModels.User;
using Journal.Domain.JsonModels.TradingAccount;

namespace Journal.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMTAccountService _mtAccountService;
        private readonly ICTraderAccountService _ctraderAccountService;
        private readonly IDXTradeAccountService _dxTradeAccountService;
        private readonly IMatchTradeAccountService _matchTradeAccountService;
        private readonly ITradeLockerAccountService _tradeLockerAccountService;
        public UserController(IUserService userService, IMTAccountService mTAccountService, ICTraderAccountService ctraderAccountService, IDXTradeAccountService dXTradeAccountService, ITradeLockerAccountService tradeLockerAccountService, IMatchTradeAccountService matchTradeAccountService)
        {
            _userService = userService;
            _mtAccountService = mTAccountService;
            _ctraderAccountService = ctraderAccountService;
            _dxTradeAccountService = dXTradeAccountService;
            _tradeLockerAccountService = tradeLockerAccountService;
            _matchTradeAccountService = matchTradeAccountService;
        }
        [HttpPost("TGLogin")]
        public async Task<IActionResult> TGLogin([FromBody] TGLoginJsonModel model)
        {
            var response = await _userService.TGLogin(model);
            return Json(response);
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
            var response = new BaseResponse<List<AccountResponseModel>>();
            response.StatusCode = Domain.Enums.StatusCode.OK;
            var responseMT = await _mtAccountService.GetMTAccountsByUser(userId);
            var responseCT = await _ctraderAccountService.GetUserAccounts(userId);
            var responseDX = await _dxTradeAccountService.GetUserAccounts(userId);
            var responseTL = await _tradeLockerAccountService.GetUserAccounts(userId);
            var responseMTT = await _matchTradeAccountService.GetUserAccounts(userId);
            var accounts = new List<AccountResponseModel>();
            foreach (var account in responseMT.Data) accounts.Add(account);
            foreach (var account in responseCT.Data) accounts.Add(account);
            foreach (var account in responseDX.Data) accounts.Add(account);
            foreach (var account in responseTL.Data) accounts.Add(account);
            foreach (var account in responseMTT.Data) accounts.Add(account);
            if (accounts.Count == 0)
            {
                response.Message = "User has no accounts";
            }
            else
            {
                response.Message = "Success";
                response.Data = accounts;
            }
            return Json(response);
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

        [HttpPost("Heartbeat")]
        public async Task<IActionResult> Heartbeat()
        {
            Console.WriteLine("Heartbeat");
            await _userService.GetAllUsers();
            return Json("Running");
        }

        /*[HttpPost ("FixUsers")]
        public async Task<IActionResult> FixUsers()
        {
            var response = await _userService.FixUsers();
            return Json(response);
        }*/
    }
}
