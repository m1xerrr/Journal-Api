using Journal.Domain.JsonModels.TradingAccount;
using Journal.Domain.JsonModels.User;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Journal.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;
        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("Subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscriptionJsonModel subscription)
        {
            var response = await _subscriptionService.Subscribe(subscription.UserId, subscription.ExpirationDate, subscription.SubscriptionType);
            return Json(response);
        }
        [HttpPost("ExtendSubscription")]
        public async Task<IActionResult> ExtendSubscription([FromBody] SubscriptionJsonModel subscription)
        {
            var response = await _subscriptionService.ExtendSubscription(subscription.UserId, subscription.ExpirationDate);
            return Json(response);
        }
        [HttpPost("DeleteSubscription")]
        public async Task<IActionResult> DeleteSubscription([FromBody] Guid userId)
        {
            var response = await _subscriptionService.DeleteSubscription(userId);
            return Json(response);
        }
        [HttpPost("ChangeSubscriptionType")]
        public async Task<IActionResult> ChangeSubscriptionType([FromBody] SubscriptionJsonModel subscription)
        {
            var response = await _subscriptionService.ChangeSubscriptionType(subscription.UserId, subscription.SubscriptionType);
            return Json(response);
        }

        [HttpPost("UserSubscription")]
        public async Task<IActionResult> UserSubscription([FromBody] Guid userId)
        {
            var response = await _subscriptionService.UserSubscriptionStatus(userId);
            return Json(response);
        }
    }
}
