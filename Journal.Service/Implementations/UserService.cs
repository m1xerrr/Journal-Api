using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;
using Journal.Domain.ResponseModels;
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
        public async Task<BaseResponse<UserResponseModel>> CreateAccount(SignUpUserJsonModel user)
        {
            var baseResponse = new BaseResponse<UserResponseModel>();
            try
            {
                var users = _userRepository.SelectAll();
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
                newUser.Role = Role.User;
                if(await _userRepository.Create(newUser))
                {
                    baseResponse.StatusCode = StatusCode.OK;
                    baseResponse.Data = new UserResponseModel(newUser);
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

        public async Task<BaseResponse<UserResponseModel>> Verify(LoginUserJsonModel user)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                user.Password = HashPasswordHelper.HashPassword(user.Password);
                var users = _userRepository.SelectAll();
                var u = users.ToList();
                if(users.FirstOrDefault(x=>x.Email == user.Email && x.Password == user.Password) != null)
                {
                    response.StatusCode = StatusCode.OK;
                    response.Data = new UserResponseModel(users.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password));
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

        public async Task<BaseResponse<bool>> DeleteAccount(Guid userId)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var users = _userRepository.SelectAll();
                var user = users.FirstOrDefault(x => x.Id == userId);
                if (user == null)
                {
                    response.Data = false;
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "User with such id does not exists";
                }
                else
                {
                    if(await _userRepository.Delete(user))
                    {
                        response.Data = true;
                        response.StatusCode = StatusCode.OK;
                        response.Message = "User deleted";
                    }
                    else
                    {
                        response.Data= false;
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "DB error";
                    }
                }
            }
            catch(Exception ex)
            {
                response.StatusCode= StatusCode.ERROR;
                response.Data = false;
                response.Message = ex.Message;
            }
            return response;
        }

    }
}
