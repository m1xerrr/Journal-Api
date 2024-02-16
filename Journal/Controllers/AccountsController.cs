﻿using Microsoft.AspNetCore.Mvc;
using Journal.Domain.ViewModels;
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

        [HttpPost("add")]
        public async Task<IActionResult> AddMTAccount([FromBody] MTAccountViewModel mtAccountModel)
        {
            var response = await _mtAccountService.AddAccount(mtAccountModel);
            return Json(response);
        }
    }
}
