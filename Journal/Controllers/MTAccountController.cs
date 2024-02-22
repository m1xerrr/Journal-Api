﻿using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Journal.Domain.JsonModels;

namespace Journal.Controllers
{
    public class MTAccountController : Controller
    {
        private readonly IMTDataService _mtDataService;
        private readonly IMTDealService _mtDealService;

        public MTAccountController(IMTDataService mTDataService, IMTDealService mtDealService)
        {
            _mtDataService = mTDataService;
            _mtDealService = mtDealService;
        }

        [HttpPost("Data")]
        public async Task<IActionResult> MTAccountData([FromBody] MTAccountJsonModel account)
        {
            var response = await _mtDataService.GetAccountData(account);
            return Json(response);
        }

        [HttpPost("AddDealImg")]
        public async Task<IActionResult> MTDealAddImg([FromBody] MTDealEditJsonModel deal)
        {
            var response = await _mtDealService.AddImage(deal.Id, deal.accountId, deal.Field);
            return Json(response);
        }

        [HttpPost("AddDealNote")]
       
        public async Task<IActionResult> MTDealAddNote([FromBody] MTDealEditJsonModel deal)
        {
            var response = await _mtDealService.AddNotes(deal.Id, deal.accountId, deal.Field);
            return Json(response);
        }

        [HttpPost("Deal")]
        public async Task<IActionResult> MTDeal([FromBody] MTDealEditJsonModel deal)
        {
            var response = await _mtDealService.GetDeal(deal.Id, deal.accountId);
            return Json(response);
        }
    }
}