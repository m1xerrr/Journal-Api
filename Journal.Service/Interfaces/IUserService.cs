using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Domain.JsonModels;

namespace Journal.Service.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<User>> CreateAccount(SignUpUserJsonModel user);

        Task<BaseResponse<User>> Verify(LoginUserJsonModel user);

    }
}
