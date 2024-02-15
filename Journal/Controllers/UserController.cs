using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Journal.Domain.ViewModels;
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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserViewModel user)
        {
            var response = await _userService.Verify(user);
            if(response.StatusCode == Domain.Enums.StatusCode.ERROR)
            {
                return Json(response);
            }
            return Json(response.Data);
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpUserViewModel user)
        {
            var response = await _userService.CreateAccount(user);
            if (response.StatusCode == Domain.Enums.StatusCode.OK)
            {
                return Json(response.Data);
            }
            else
            {
                return Json(response.Data);
            }
            
        }
    }
}
