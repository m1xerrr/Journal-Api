using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Domain.ViewModels;

namespace Journal.Service.Interfaces
{
    public interface IUserService
    {
        Task<BaseResponse<User>> CreateAccount(SignUpUserViewModel user);

        Task<BaseResponse<User>> Verify(LoginUserViewModel user);

    }
}
