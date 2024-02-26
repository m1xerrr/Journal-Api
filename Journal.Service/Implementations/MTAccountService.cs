using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;
using Journal.Domain.ResponseModels;
using Microsoft.EntityFrameworkCore;

namespace Journal.Service.Implementations
{
    public class MTAccountService : IMTAccountService
    {
        private readonly IMTAccountRepository _mtAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMTDataRepository _mtDataRepository;

        public MTAccountService(IMTAccountRepository mtAccountRepository, IUserRepository userRepository, IMTDataRepository mtDataRepository)
        {
            _mtAccountRepository = mtAccountRepository;
            _userRepository = userRepository;
            _mtDataRepository = mtDataRepository;
        }

        public async Task<BaseResponse<MTAccountResponseModel>> AddAccount(MTAccountJsonModel accountModel)
        {
            var response = new BaseResponse<MTAccountResponseModel>();
            try
            {
                var accs = _mtAccountRepository.SelectAll();
                if (accs.FirstOrDefault(x => x.Login == accountModel.Login) != null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account exists";
                    return response;
                }
                if (!await _mtDataRepository.Initialize(accountModel))
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account initializing error";
                    return response;
                }
                
                var account = new MTAccount();

                account.Id = Guid.NewGuid();
                account.Login = accountModel.Login;
                account.Password = accountModel.Password;
                account.Server = accountModel.Server;
                account.UserId = accountModel.UserId;
                var users = _userRepository.SelectAll();
                account.User = users.FirstOrDefault(x => x.Id == accountModel.UserId);

                if (await _mtAccountRepository.Create(account))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Data = new MTAccountResponseModel(account);
                    }
                    else
                    {
                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "DB Error";
                    }

            }
            catch(Exception ex) 
            { 
                response.StatusCode=Domain.Enums.StatusCode.ERROR;
                response.Message=ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> DeleteMTAccount(Guid accountId)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var accounts = _mtAccountRepository.SelectAll();
                var account = accounts.FirstOrDefault(x => x.Id == accountId);
                if(account == null)
                {
                    response.Data = false;
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account with such Id does not exists";
                }
                else
                {
                    if(await _mtAccountRepository.Delete(account))
                    {
                        response.Data = true;
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Message = "Success";
                        
                    }
                    else
                    {
                        response.Data = false;
                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "DB error";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public BaseResponse<List<MTAccountResponseModel>> GetMTAccountsByUser(Guid userId)
        {
            var response = new BaseResponse<List<MTAccountResponseModel>>();
            try
            {
                var accounts = _mtAccountRepository.SelectAll().Where(x => x.UserId == userId);
                var accountsResponse = new List<MTAccountResponseModel>();
                foreach (var account in accounts)
                {
                    accountsResponse.Add(new MTAccountResponseModel(account));
                }
                if(accountsResponse.Count == 0)
                {
                    response.StatusCode= Domain.Enums.StatusCode.ERROR;
                    response.Message = "User has ho accounts";
                }
                else
                {
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Data = accountsResponse;
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
