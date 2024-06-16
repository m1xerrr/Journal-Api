using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Domain.Enums;
using Journal.Domain.JsonModels.User;

namespace Journal.Service.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<UserResponseModel>> CreateAccount(SignUpUserJsonModel user);

        Task<BaseResponse<UserResponseModel>> TGLogin(TGLoginJsonModel model);

        Task<BaseResponse<UserResponseModel>> Verify(LoginUserJsonModel user);

        Task<BaseResponse<bool>> DeleteAccount(Guid userId);

        Task<BaseResponse<List<UserResponseModel>>> GetAllUsers();

        Task<BaseResponse<UserResponseModel>> ChangePassword(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeName(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeEmail(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeRole(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> EditUser(EditUserJsonModel userModel);

        Task<BaseResponse<List<ShareAccountResponseModel>>> GetLeaderboard();

        Task<BaseResponse<ShareAccountResponseModel>> GetProfit(Guid accountID, string provider, DateTime startDate, DateTime endDate);

        Task<BaseResponse<UserResponseModel>> FixUsers();

    }
}
