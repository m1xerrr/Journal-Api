using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;

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
            return Json(await _userService.Verify(user));
        }
        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpUserJsonModel user)
        {
            return Json(await _userService.CreateAccount(user));
        }
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody] Guid userId)
        {
            return Json(await _userService.DeleteAccount(userId));
        }
    }
}
