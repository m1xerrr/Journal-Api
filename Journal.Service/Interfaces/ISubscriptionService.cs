using Journal.Domain.Enums;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Interfaces
{
    public interface ISubscriptionService
    {
        Task<BaseResponse<SubscriptionResponseModel>> Subscribe(Guid userId, DateTime ExpirationDate, SubscriptionType subscriptionType);

        Task<BaseResponse<SubscriptionResponseModel>> ExtendSubscription(Guid userId, DateTime ExpirationDate);

        Task<BaseResponse<bool>> DeleteSubscription(Guid userId);

        Task<BaseResponse<SubscriptionResponseModel>> ChangeSubscriptionType(Guid userId, SubscriptionType subscriptionType);

        Task<BaseResponse<SubscriptionResponseModel>> UserSubscriptionStatus(Guid userId);
    }
}
