using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Journal.Domain.ResponseModels;
using Journal.Domain.Enums;
using Journal.Domain.Models;
using Journal.Domain.Helpers;
using Journal.DAL.Interfaces;
using Azure;
using System.Reflection.Metadata.Ecma335;
using Journal.Domain.JsonModels.User;

namespace Journal.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMTAccountService _mtAccountService;
        private readonly IDXTradeAccountService _dxtraderAccountService;
        private readonly ICTraderAccountService _ctraderAccountService;
        private readonly IMTAccountRepository _mtAccountRepository;
        private readonly IDXTradeAccountRepository _dxtraderAccountRepository;
        private readonly ICTraderAccountRepository _ctraderAccountRepository;
        private readonly ITradeLockerAccountService _tradeLockerAccountService;
        private readonly IDealRepository _dealRepository;

        public UserService(IUserRepository userRepository, ISubscriptionRepository subscriptionRepository, IMTAccountService mtAccountService, IDXTradeAccountService dxtraderAccountService, ICTraderAccountService ctraderAccountService, IMTAccountRepository mTAccountRepository, IDXTradeAccountRepository dXTradeAccountRepository, ICTraderAccountRepository cTraderAccountRepository, IDealRepository dealRepository, ITradeLockerAccountService tradeLockerAccountService)
        {
            _userRepository = userRepository;
            _mtAccountService = mtAccountService;
            _ctraderAccountService = ctraderAccountService;
            _dxtraderAccountService = dxtraderAccountService;
            _mtAccountRepository = mTAccountRepository;
            _dxtraderAccountRepository = dXTradeAccountRepository;
            _ctraderAccountRepository = cTraderAccountRepository;
            _dealRepository = dealRepository;   
            _tradeLockerAccountService = tradeLockerAccountService;
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
                    if(users.Where(x => x.Name == userModel.Name).Count() > 0 && users.FirstOrDefault(x => x.Id == userModel.Id).Name != userModel.Name)
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Nickname is already in use";
                        return response;
                    }
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
                response.StatusCode = StatusCode.ERROR;
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
                response.StatusCode = StatusCode.ERROR;
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
                response.StatusCode = StatusCode.ERROR;
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
                response.StatusCode = StatusCode.ERROR;
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
                if (users.Where(x => x.Name == userModel.Name).Count() > 0 && users.FirstOrDefault(x => x.Id == userModel.Id).Name != userModel.Name)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "This name is already in use";
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
                        user.Password = userModel.Password;
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
                response.StatusCode = StatusCode.ERROR;
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
                    baseResponse.StatusCode = StatusCode.ERROR;
                    baseResponse.Message = $"User with email {user.Email} already registred";
                    return baseResponse;
                }
                if (users.FirstOrDefault(x => x.Name == user.Name) != null)
                {
                    baseResponse.StatusCode = StatusCode.ERROR;
                    baseResponse.Message = $"Nickname {user.Name} is already in use";
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
                baseResponse.StatusCode = StatusCode.ERROR;
                baseResponse.Message = $"[Create User] {ex.Message}";
            }
            return baseResponse;
        }

        public async Task<BaseResponse<UserResponseModel>> Verify(LoginUserJsonModel user)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                if(user.Password.Length != 64) user.Password = HashPasswordHelper.HashPassword(user.Password);
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
                response.StatusCode = StatusCode.ERROR;
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
                response.StatusCode = StatusCode.ERROR;
                response.Message= ex.Message;
            }
            return response;
        }



        public async Task<BaseResponse<List<ShareAccountResponseModel>>> GetLeaderboard()
        {
            var response = new BaseResponse<List<ShareAccountResponseModel>>();
            try
            {
                var accountsResponse = new List<ShareAccountResponseModel>();
                var users = _userRepository.SelectAll();
                foreach (var user in users)
                {
                    var name = user.Name;
                    foreach(var account in (await _mtAccountService.GetMTAccountsByUser(user.Id)).Data)
                    {
                        var accountData = new ShareAccountResponseModel(account);
                        accountData.Username = name;
                        accountsResponse.Add(accountData);
                    }
                    foreach (var account in (await _dxtraderAccountService.GetUserAccounts(user.Id)).Data)
                    {
                        var accountData = new ShareAccountResponseModel(account);
                        accountData.Username = name;
                        accountsResponse.Add(accountData);
                    }
                    foreach (var account in (await _ctraderAccountService.GetUserAccounts(user.Id)).Data)
                    {
                        var accountData = new ShareAccountResponseModel(account);
                        accountData.Username = name;
                        accountsResponse.Add(accountData);
                    }
                    foreach (var account in (await _tradeLockerAccountService.GetUserAccounts(user.Id)).Data)
                    {
                        var accountData = new ShareAccountResponseModel(account);
                        accountData.Username = name;
                        accountsResponse.Add(accountData);
                    }
                    response.Data = accountsResponse.OrderByDescending(x => x.ProfitPercentage).ThenByDescending(x => x.Profit).ToList();
                }
                response.StatusCode = StatusCode.OK;
                response.Message = "Success";
            }
            
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<ShareAccountResponseModel>> GetProfit(Guid accountID, string provider, DateTime startDate, DateTime endDate)
        {
            var response = new BaseResponse<ShareAccountResponseModel>();
            try
            {
                response.Data = new ShareAccountResponseModel();
                response.Data.Provider = provider;
                switch (provider)
                {
                    case "MetaTrader 5":
                        response.Data.Deposit = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountID).Deposit;
                        response.Data.Login = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountID).Login;
                        break;
                    case "CTrader":
                        response.Data.Deposit = _ctraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountID).Deposit;
                        response.Data.Login = _ctraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountID).Login;
                        break;
                    case "DXTrade":
                        response.Data.Deposit = _dxtraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountID).Deposit;
                        response.Data.Login = _dxtraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountID).Login;
                        break;
                    default:
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Provider not found";
                        return response;
                }
                var deals = _dealRepository.SelectAll().Where(x => x.AccountId == accountID).Where(x => x.EntryTime > startDate).Where(x => x.ExitTime < endDate);
                response.Data.Profit = deals.Sum(x => x.Profit);
                response.Data.ProfitPercentage = deals.Sum(x => x.ProfitPercentage);
                response.Data.Commission = deals.Sum(x => x.Comission);
                response.Data.DealsCount = deals.Count();
                response.Data.Symbols = deals.Select(deal => deal.Symbol).Distinct().ToList();
                response.StatusCode = StatusCode.OK;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<UserResponseModel>> TGLogin(string username)
        {
            var response = new BaseResponse<UserResponseModel>();
            try
            {
                if(_userRepository.SelectAll().FirstOrDefault(x => x.TGUsername == username) == null)
                {
                    User user = new User()
                    {
                        Id = Guid.NewGuid(),
                        TGUsername = username,
                        Email = username+"@mail.com",
                        Password = HashPasswordHelper.HashPassword(username+"Password"),
                        Name = username,
                        Role = Role.User
                    };
                    if(await _userRepository.Create(user))
                    {
                        response.Message = "Success";
                        response.StatusCode= StatusCode.OK;
                        response.Data = new UserResponseModel(user);
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Can not create new account";
                    }
                }
                else
                {
                    response.StatusCode = StatusCode.OK;
                    response.Message = "Success";
                    response.Data = new UserResponseModel(_userRepository.SelectAll().FirstOrDefault(x => x.TGUsername == username));
                }
            }
            catch (Exception ex)
            {
                response.Message= ex.Message;
                response.StatusCode = StatusCode.ERROR;
            }
            return response;
        }

        
    }
}
