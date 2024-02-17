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

    }
}
