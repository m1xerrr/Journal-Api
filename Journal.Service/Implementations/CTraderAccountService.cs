using Journal.DAL.Interfaces;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Journal.Domain.Models;

namespace Journal.Service.Implementations
{
    public class CTraderAccountService : ICTraderAccountService
    {
        private readonly ICTraderAccountRepository _cTraderAccountRepository;
        private readonly ICTraderApiRepository _cTraderApiRepository;
        private readonly IUserRepository _userRepository;

        public CTraderAccountService(ICTraderApiRepository apiRepository, ICTraderAccountRepository accountRepository, IUserRepository userRepository)
        {
            _cTraderApiRepository = apiRepository;
            _cTraderAccountRepository = accountRepository;
            _userRepository = userRepository;
        }
        public async Task<BaseResponse<List<CTraderAccountResponseModel>>> AddAccounts(string accessToken, Guid UserId)
        {
            var response = new BaseResponse<List<CTraderAccountResponseModel>>();
            response.Data = new List<CTraderAccountResponseModel>();
            try
            {
                var accountsDB = _cTraderAccountRepository.SelectAll();
                var accounts = await _cTraderApiRepository.AccountListRequest(accessToken);

                var user = _userRepository.SelectAll().Where(x => x.Id == UserId);

                if(user == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "User not found";
                    return response;
                }

                foreach (var account in accounts.CtidTraderAccount)
                {
                    if (accountsDB.FirstOrDefault(x => (x.AccountId == (long)account.CtidTraderAccountId) &&  x.UserID == UserId) != null) continue;
                    var newAccount = new CTraderAccount()
                    {
                        AccessToken = accessToken,
                        UserID = UserId,
                        Id = Guid.NewGuid(),
                        AccountId = (long)account.CtidTraderAccountId,
                        IsLive = account.IsLive,
                        Login = account.TraderLogin
                    };
                    if (await _cTraderAccountRepository.Create(newAccount))
                    {
                        response.Data.Add(new CTraderAccountResponseModel(newAccount));
                    }
                    else
                    {
                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "Can not add account to DB";
                        return response;
                    }
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Success";
                }
            }
            catch (Exception ex) 
            {
                response.Message = ex.Message;
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteCTraderAccount(Guid id)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var accounts = _cTraderAccountRepository.SelectAll();
                var account = accounts.FirstOrDefault(x => x.Id == id);
                if (account == null)
                {
                    response.StatusCode=Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account not found";
                    return response;
                }

                if(!await _cTraderAccountRepository.Delete(account))
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Can not delete account";
                    return response;
                }
                response.StatusCode = Domain.Enums.StatusCode.OK;
                response.Data = true;
                response.Message = "Account deleted";
                
            }
            catch (Exception ex)
            {
                response.StatusCode= Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<string>> GetAccessToken(string authorizationLink)
        {
            var response = new BaseResponse<string>();

            try
            {
                response.StatusCode = Domain.Enums.StatusCode.OK;
                response.Data = await _cTraderApiRepository.GetAccessToken(authorizationLink.Split('=')[1]);
            }
            catch (Exception ex)
            {
                response.StatusCode= Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
