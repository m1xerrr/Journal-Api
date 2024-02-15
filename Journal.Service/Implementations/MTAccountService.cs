using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Domain.ViewModels;
using Journal.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<BaseResponse<MTAccount>> AddAccount(MTAccountViewModel accountModel)
        {
            var response = new BaseResponse<MTAccount>();
            try
            {
                var accounts = await _mtAccountRepository.SelectAll();
                if (accounts.FirstOrDefault(x => x.Login == accountModel.Login) != null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account exists";
                    return response;
                }

                if(!await _mtDataRepository.Initialize(accountModel))
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account initializing error";
                }
                else {
                    var account = new MTAccount();
                    var users = await _userRepository.SelectAll();

                    account.Id = Guid.NewGuid();
                    account.Login = accountModel.Login;
                    account.Password = accountModel.Password;
                    account.Server = accountModel.Server;
                    account.UserId = accountModel.UserId;
                    account.User = users.FirstOrDefault(x => x.Id == accountModel.UserId);

                    if (await _mtAccountRepository.Create(account))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Data = account;
                    }
                    else
                    {
                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "DB Error";
                    }
                }


            }
            catch(Exception ex) 
            { 
                response.StatusCode=Domain.Enums.StatusCode.ERROR;
                response.Message=ex.Message;
            }
            return response;
        }
    }
}
