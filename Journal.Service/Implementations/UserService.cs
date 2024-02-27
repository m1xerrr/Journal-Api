using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;
using Journal.Domain.ResponseModels;
using Journal.Domain.Enums;
using Journal.Domain.Models;
using Journal.Domain.Helpers;
using Journal.DAL.Interfaces;
using Azure;

namespace Journal.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public UserService(IUserRepository userRepository, ISubscriptionRepository subscriptionRepository)
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }
        public async Task<BaseResponse<UserResponseModel>> ChangeName(EditUserJsonModel userModel)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                var users = _userRepository.SelectAll();
                if(users.FirstOrDefault(x => x.Id == userModel.Id) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "User with such ID not found";
                }
                else
                {
                    var user = users.FirstOrDefault(x => x.Id == userModel.Id);
                    user.Name = userModel.Name;
                    if (await _userRepository.Edit(user))
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Data = new UserResponseModel(user);
                    }
                    else
                    {
                        response.StatusCode= StatusCode.ERROR;
                        response.Message = "Can not update user";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> ChangePassword(EditUserJsonModel userModel)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                var users = _userRepository.SelectAll();
                if (users.FirstOrDefault(x => x.Id == userModel.Id) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "User with such ID not found";
                }
                else
                {
                    var user = users.FirstOrDefault(x => x.Id == userModel.Id);
                    user.Password = HashPasswordHelper.HashPassword(userModel.Password);
                    if (await _userRepository.Edit(user))
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Data = new UserResponseModel(user);
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Can not update user";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> ChangeEmail(EditUserJsonModel userModel)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                var users = _userRepository.SelectAll();
                if(users.FirstOrDefault(x => x.Email == userModel.Email) != null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "This email is already in use";
                }
                else
                { 
                if (users.FirstOrDefault(x => x.Id == userModel.Id) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "User with such ID not found";
                }
                else
                {
                    var user = users.FirstOrDefault(x => x.Id == userModel.Id);
                    user.Email = userModel.Email;
                    if (await _userRepository.Edit(user))
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Data = new UserResponseModel(user);
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Can not update user";
                    }
                }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> ChangeRole(EditUserJsonModel userModel)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                var users = _userRepository.SelectAll();
                if (users.FirstOrDefault(x => x.Id == userModel.Id) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "User with such ID not found";
                }
                else
                {
                    var user = users.FirstOrDefault(x => x.Id == userModel.Id);
                    user.Role = userModel.Role;
                    if (await _userRepository.Edit(user))
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Data = new UserResponseModel(user);
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Can not update user";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> EditUser(EditUserJsonModel userModel)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                var users = _userRepository.SelectAll();
                if (users.Where(x => x.Email == userModel.Email).Count() > 1)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "This email is already in use";
                }
                else
                {
                    if (users.FirstOrDefault(x => x.Id == userModel.Id) == null)
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "User with such ID not found";
                    }
                    else
                    {
                        var user = users.FirstOrDefault(x => x.Id == userModel.Id);
                        user.Name = userModel.Name;
                        user.Email = userModel.Email;
                        user.Password = HashPasswordHelper.HashPassword(userModel.Password);
                        user.Role = userModel.Role;
                        if (await _userRepository.Edit(user))
                        {
                            response.StatusCode = StatusCode.OK;
                            response.Data = new UserResponseModel(user);
                        }
                        else
                        {
                            response.StatusCode = StatusCode.ERROR;
                            response.Message = "Can not update user";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
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
                if (users.FirstOrDefault(x => x.Email == user.Email) != null)
                {
                    if (users.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password) != null)
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Data = new UserResponseModel(users.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password));
                    }
                    else
                    {
                        response.StatusCode= StatusCode.ERROR;
                        response.Message = "Wrong password";
                    }
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


        public async Task<BaseResponse<List<UserResponseModel>>> GetAllUsers()
        {
            var response = new BaseResponse<List<UserResponseModel>>();
            try
            {
                var users = _userRepository.SelectAll();
                var usersResponse = new List<UserResponseModel>();
                foreach(var user in users)
                {
                    usersResponse.Add(new UserResponseModel(user));
                }
                response.Data = usersResponse;
                response.StatusCode = StatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Message= ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> Subscribe(Guid userId, DateTime ExpirationDate, SubscriptionType subscriptionType)
        {
            var response = new BaseResponse<UserResponseModel>();
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
                }
                if(await _subscriptionRepository.Create(subscription))
                {
                    response.Data = new UserResponseModel(_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId));
                    response.StatusCode = StatusCode.OK;
                }
                else
                {
                    response.StatusCode= StatusCode.ERROR;
                    response.Message = "Can not add subscription";
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> ExtendSubscription(Guid userId, DateTime ExpirationDate)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                var subscription = _subscriptionRepository.SelectAll().FirstOrDefault(x => x.UserId == userId);
                subscription.ExpirationDate = ExpirationDate;
                if (_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No users with such ID";
                }
                if (await _subscriptionRepository.Edit(subscription))
                {
                    response.Data = new UserResponseModel(_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId));
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
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> DeleteSubscription(Guid userId)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                var subscription = _subscriptionRepository.SelectAll().FirstOrDefault(x => x.UserId == userId);
                if (_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No users with such ID";
                }
                if (await _subscriptionRepository.Delete(subscription))
                {
                    response.Data = new UserResponseModel(_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId));
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
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> ChangeSubscriptionType(Guid userId, SubscriptionType subscriptionType)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                var subscription = _subscriptionRepository.SelectAll().FirstOrDefault(x => x.UserId == userId);
                subscription.Subsctiption = subscriptionType;
                if (_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId) == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No users with such ID";
                }
                if (await _subscriptionRepository.Edit(subscription))
                {
                    response.Data = new UserResponseModel(_userRepository.SelectAll().FirstOrDefault(x => x.Id == userId));
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
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
