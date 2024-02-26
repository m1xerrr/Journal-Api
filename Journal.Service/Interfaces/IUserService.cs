using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Domain.JsonModels;

namespace Journal.Service.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<UserResponseModel>> CreateAccount(SignUpUserJsonModel user);

        Task<BaseResponse<UserResponseModel>> Verify(LoginUserJsonModel user);

        Task<BaseResponse<bool>> DeleteAccount(Guid userId);

        Task<BaseResponse<List<UserResponseModel>>> GetAllUsers();

        Task<BaseResponse<UserResponseModel>> ChangePassword(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeName(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeEmail(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> ChangeRole(EditUserJsonModel userModel);

        Task<BaseResponse<UserResponseModel>> EditUser(EditUserJsonModel userModel);

    }
}
