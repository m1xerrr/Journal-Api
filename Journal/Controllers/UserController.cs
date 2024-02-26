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
        public UserController(IUserService userService, IMTAccountService mTAccountService)
        {
            _userService = userService;
            _mtAccountService = mTAccountService;
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
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody] Guid userId)
        {
            var response = await _userService.DeleteAccount(userId);
            return Json(response);
        }
        [HttpPost("UserMTAccounts")]
        public IActionResult UserMTAccounts([FromBody] Guid userId)
        {
            var resposne = _mtAccountService.GetMTAccountsByUser(userId);
            return Json(resposne);
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

    }
}
