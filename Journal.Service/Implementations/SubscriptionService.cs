using Journal.DAL.Interfaces;
using Journal.DAL.Repositories;
using Journal.Domain.Enums;
using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;

        public SubscriptionService(IUserRepository userRepository, ISubscriptionRepository subscriptionRepository)
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }
        public async Task<BaseResponse<SubscriptionResponseModel>> Subscribe(Guid userId, DateTime ExpirationDate, SubscriptionType subscriptionType)
        {
            var response = new BaseResponse<SubscriptionResponseModel>();
            try
            {
                var subscription = new Subscription();
                subscription.UserId = userId;
                subscription.ExpirationDate = ExpirationDate;
                subscription.PurchaseDate = DateTime.Now;
                subscription.Id = Guid.NewGuid();
                subscription.Subsctiption = subscriptionType;
                if (_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No users with such ID";
                    return response;
                }
                if (await _subscriptionRepository.Create(subscription))
                {
                    response.Data = new SubscriptionResponseModel(subscription);
                    response.StatusCode = StatusCode.OK;
                    return response;
                }
                else
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Can not add subscription";
                    return response;
                }

            }
            catch (Exception ex)
            {

                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<SubscriptionResponseModel>> ExtendSubscription(Guid userId, DateTime ExpirationDate)
        {
            var response = new BaseResponse<SubscriptionResponseModel>();
            try
            {
                var subscription = _subscriptionRepository.SelectAll().FirstOrDefault(x => x.UserId == userId);
                subscription.ExpirationDate = ExpirationDate;
                if (_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No users with such ID";
                    return response;
                }
                if (await _subscriptionRepository.Edit(subscription))
                {
                    response.Data = new SubscriptionResponseModel(subscription);
                    response.StatusCode = StatusCode.OK;
                }
                else
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Can not extend subscription";
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> DeleteSubscription(Guid userId)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var subscription = _subscriptionRepository.SelectAll().FirstOrDefault(x => x.UserId == userId);
                if (_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No users with such ID";
                    return response;
                }
                if (await _subscriptionRepository.Delete(subscription))
                {
                    response.Data = true;
                    response.StatusCode = StatusCode.OK;
                }
                else
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Can not delete subscription";
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<SubscriptionResponseModel>> ChangeSubscriptionType(Guid userId, SubscriptionType subscriptionType)
        {
            var response = new BaseResponse<SubscriptionResponseModel>();
            try
            {
                var subscription = _subscriptionRepository.SelectAll().FirstOrDefault(x => x.UserId == userId);
                subscription.Subsctiption = subscriptionType;
                if (_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No users with such ID";
                    return response;
                }
                if (await _subscriptionRepository.Edit(subscription))
                {
                    response.Data = new SubscriptionResponseModel(subscription);
                    response.StatusCode = StatusCode.OK;
                }
                else
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Can not change subscription type";
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<SubscriptionResponseModel>> UserSubscriptionStatus(Guid userId)
        {
            var response = new BaseResponse<SubscriptionResponseModel>();
            try
            {
                if (_subscriptionRepository.SelectAll().FirstOrDefault(x => x.UserId == userId) == null)
                {
                    response.Message = "User has no subscriptions";
                    response.StatusCode = StatusCode.ERROR;
                }
                else
                {
                    response.StatusCode = StatusCode.OK;
                    response.Message = "Success";
                    response.Data = new SubscriptionResponseModel(_subscriptionRepository.SelectAll().FirstOrDefault(x => x.UserId == userId));
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
