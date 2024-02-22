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
        public UserController(IUserService userService)
        {
            _userService = userService;
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
    }
}
