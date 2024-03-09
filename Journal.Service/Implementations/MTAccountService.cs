using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;
using Journal.Domain.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Journal.Domain.Enums;

namespace Journal.Service.Implementations
{
    public class MTAccountService : IMTAccountService
    {
        private readonly IMTAccountRepository _mtAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMTDataRepository _mtDataRepository;
        private readonly IDealRepository _mtDealRepository;

        public MTAccountService(IMTAccountRepository mtAccountRepository, IUserRepository userRepository, IMTDataRepository mtDataRepository, IDealRepository mTDealRepository)
        {
            _mtAccountRepository = mtAccountRepository;
            _userRepository = userRepository;
            _mtDataRepository = mtDataRepository;
            _mtDealRepository = mTDealRepository;
        }

        public async Task<BaseResponse<AccountResponseModel>> AddAccount(MTAccountJsonModel accountModel)
        {
            var response = new BaseResponse<AccountResponseModel>();
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
                account.UserID = accountModel.UserId;
                var users = _userRepository.SelectAll();
                account.User = users.FirstOrDefault(x => x.Id == accountModel.UserId);

                if (await _mtAccountRepository.Create(account))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Data = new AccountResponseModel(account);
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
                var deals = _mtDealRepository.SelectAll();
                foreach(var deal in deals)
                {
                    if(deal.AccountId == accountId)
                    {
                        await _mtDealRepository.Delete(deal);
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

        public async Task<BaseResponse<List<AccountResponseModel>>> GetMTAccountsByUser(Guid userId)
        {
            var response = new BaseResponse<List<AccountResponseModel>>();
            try
            {
                var accounts = _mtAccountRepository.SelectAll().Where(x => x.UserID == userId);
                var accountsResponse = new List<AccountResponseModel>();
                foreach (var account in accounts)
                {
                    var accountJson = new MTAccountJsonModel();
                    accountJson.UserId = account.UserID;
                    accountJson.Server = account.Server;
                    accountJson.Password = account.Password;
                    accountJson.Login = account.Login;
                    accountJson.Id = account.Id;
                    var deals = await _mtDataRepository.GetDeals(accountJson);
                    var data = await DealstoAccount(account.Id, deals);
                    var accountResponse = new AccountResponseModel(account);
                    accountResponse.Profit = data.Profit;
                    accountResponse.ProfitPercentage = data.ProfitPercentage;
                    accountResponse.Balance = data.currentBalance;
                    accountResponse.DealsCount = data.TotalDeals;
                    accountResponse.Deposit = data.Deposit;
                    accountsResponse.Add(accountResponse);
                }
                response.StatusCode = Domain.Enums.StatusCode.OK;
                response.Data = accountsResponse;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        private async Task<AccountData> DealstoAccount(Guid accountID, List<MTDealJsonModel> dealsList)
        {
            var account = new AccountData();
            var deposits = dealsList.Where(deal => deal.Comment.Contains("Deposit")).ToList();
            foreach (var deposit in deposits)
            {
                account.Deposit += deposit.Profit;
                dealsList.Remove(deposit);
            }
            var dbDeals = new List<Deal>();
            foreach (var deal in dealsList)
            {
                if (dbDeals.FirstOrDefault(x => x.PositionId == deal.PositionId) == null)
                {
                    dbDeals.Add(new Deal
                    {
                        PositionId = deal.PositionId,
                        Direction = (deal.Type == 0) ? Direction.Long : Direction.Short,
                        EntryPrice = deal.Price,
                        Profit = deal.Profit,
                        Volume = deal.Volume,
                        Comission = deal.Commission,
                        EntryTime = DateTimeOffset.FromUnixTimeSeconds(deal.Time).UtcDateTime,
                        Symbol = deal.Symbol,
                    });
                }
                else
                {
                    var accountDeal = dbDeals.FirstOrDefault(x => x.PositionId == deal.PositionId);
                    accountDeal.ExitPrice = deal.Price;
                    accountDeal.Profit += deal.Profit;
                    accountDeal.Volume += deal.Volume;
                    accountDeal.Comission += deal.Commission;
                    accountDeal.ExitTime = DateTimeOffset.FromUnixTimeSeconds(deal.Time).UtcDateTime;
                    if (deal.Comment.Contains("tp")) { accountDeal.Result = Result.Win; }
                    else if (deal.Comment.Contains("sl")) { accountDeal.Result = Result.Loss; }
                    else { accountDeal.Result = Result.Breakeven; }
                    accountDeal.ProfitPercentage = Math.Round(deal.Profit / account.Deposit * 100, 2);
                }
            }
            account.Profit = dbDeals.Sum(x => x.Profit) + dbDeals.Sum(x => x.Comission);
            account.currentBalance = account.Deposit + account.Profit;
            account.TotalDeals = dbDeals.Count;
            account.ProfitPercentage = Math.Round(account.Profit / account.Deposit * 100, 2);
            return account;
        }
    }
}
