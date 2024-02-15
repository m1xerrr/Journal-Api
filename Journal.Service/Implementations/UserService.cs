using Journal.Domain.Responses;
using Journal.Domain.ViewModels;
using Journal.Service.Interfaces;
using Journal.DAL.Repositories;
using Journal.Domain.Enums;
using Journal.Domain.Models;
using Journal.Domain.Helpers;
using Journal.DAL.Interfaces;

namespace Journal.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<BaseResponse<User>> CreateAccount(SignUpUserViewModel user)
        {
            var baseResponse = new BaseResponse<User>();
            try
            {
                var users = await _userRepository.SelectAll();
                if (users.FirstOrDefault(x => x.Email == user.Email) != null)
                {
                    baseResponse.StatusCode = StatusCode.EmailError;
                    baseResponse.Message = $"User with email {user.Email} already registred";
                    return baseResponse;
                }
                User newUser = new User();
                newUser.Id = Guid.NewGuid();
                newUser.Name = user.Name;
                newUser.Email = user.Email;
                newUser.Password = HashPasswordHelper.HashPassword(user.Password);
                newUser.Role = Role.Administrator;
                if(await _userRepository.Create(newUser))
                {
                    baseResponse.StatusCode = StatusCode.OK;
                    baseResponse.Data = newUser;
                }
                else 
                {
                    baseResponse.StatusCode = StatusCode.ERROR;
                    baseResponse.Message = "DB Error";
                }
            }
            catch (Exception ex)
            {
                baseResponse.Message = $"[Create User] {ex.Message}";
            }
            return baseResponse;
        }

        public async Task<BaseResponse<User>> Verify(LoginUserViewModel user)
        {
            var response = new BaseResponse<User>();
            try
            {
                user.Password = HashPasswordHelper.HashPassword(user.Password);
                var users = await _userRepository.SelectAll();
                if(users.FirstOrDefault(x=>x.Email == user.Email && x.Password == user.Password) != null)
                {
                    response.Data = users.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password);
                }
                else
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                response.Message = $"[Verify User] {ex.Message}";
            }
            return response;
        }
    }
}
