using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Domain.Enums;

namespace Journal.Service.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<UserResponseModel>> CreateAccount(SignUpUserJsonModel user);

        Task<BaseResponse<UserResponseModel>> TGLogin(string username);

        Task<BaseResponse<UserResponseModel>> Verify(LoginUserJsonModel user);

        Task<BaseResponse<bool>> DeleteAccount(Guid userId);

        Task<BaseResponse<List<UserResponseModel>>> GetAllUsers();

        Task<BaseResponse<UserResponseModel>> ChangePassword(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeName(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeEmail(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeRole(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> EditUser(EditUserJsonModel userModel);

        Task<BaseResponse<SubscriptionResponseModel>> Subscribe(Guid userId, DateTime ExpirationDate, SubscriptionType subscriptionType);

        Task<BaseResponse<SubscriptionResponseModel>> ExtendSubscription(Guid userId, DateTime ExpirationDate);

        Task<BaseResponse<bool>> DeleteSubscription(Guid userId);

        Task<BaseResponse<SubscriptionResponseModel>> ChangeSubscriptionType(Guid userId, SubscriptionType subscriptionType);

        Task<BaseResponse<SubscriptionResponseModel>> UserSubscriptionStatus(Guid userId);

        Task<BaseResponse<List<ShareAccountResponseModel>>> GetLeaderboard();

        Task<BaseResponse<ShareAccountResponseModel>> GetProfit(Guid accountID, string provider, DateTime startDate, DateTime endDate);

    }
}
