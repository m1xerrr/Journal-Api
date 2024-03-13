using Journal.DAL.Interfaces;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Journal.Domain.Models;
using System.ComponentModel;
using Google.Protobuf.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Identity.Client;
using System.Security.Principal;
using Journal.DAL.Repositories;
using Journal.Domain.Enums;

namespace Journal.Service.Implementations
{
    public class CTraderAccountService : ICTraderAccountService
    {
        private readonly ICTraderAccountRepository _cTraderAccountRepository;
        private readonly ICTraderApiRepository _cTraderApiRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDealRepository _dealRepository;

        public CTraderAccountService(ICTraderApiRepository apiRepository, ICTraderAccountRepository accountRepository, IUserRepository userRepository, IDealRepository dealRepository)
        {
            _cTraderApiRepository = apiRepository;
            _cTraderAccountRepository = accountRepository;
            _userRepository = userRepository;
            _dealRepository = dealRepository;
        }
        public async Task<BaseResponse<List<AccountResponseModel>>> AddAccounts(string accessToken, Guid UserId)
        {
            var response = new BaseResponse<List<AccountResponseModel>>();
            response.Data = new List<AccountResponseModel>();
            try
            {
                var accountsDB = _cTraderAccountRepository.SelectAll();
                var accounts = await _cTraderApiRepository.AccountListRequest(accessToken);

                var user = _userRepository.SelectAll().Where(x => x.Id == UserId);

                if (user == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "User not found";
                    return response;
                }

                foreach (var account in accounts.CtidTraderAccount)
                {
                    if (accountsDB.FirstOrDefault(x => (x.AccountId == (long)account.CtidTraderAccountId) && x.UserID == UserId) != null) continue;
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
                        var responseAccount = new AccountResponseModel(newAccount);
                        responseAccount.Provider = "CTrader";
                        response.Data.Add(responseAccount);
                        LoadAccountData(newAccount.Id);
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
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account not found";
                    return response;
                }
                var deals = _dealRepository.SelectAll().Where(x => x.AccountId == id);
                foreach (var deal in deals)
                {
                    await _dealRepository.Delete(deal);
                }

                if (!await _cTraderAccountRepository.Delete(account))
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
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
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
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> LoadAccountData(Guid id)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _cTraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if (account == null)
                {
                    response.Message = "Account not found";
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    return response;
                }
                
                var dealsResponse = await _cTraderApiRepository.GetDeals(account.AccessToken, account.AccountId, account.IsLive);
                if(dealsResponse.Count() == 0)
                {
                    response.Data = false;
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account is empty. Please, open at least one deal";
                    return response;
                }
                var symbols = await _cTraderApiRepository.GetSymbols(account.AccessToken, account.AccountId, account.IsLive);
                await GetAccountFromDeals(dealsResponse, symbols, id);
                response.Data = true;
                response.StatusCode = Domain.Enums.StatusCode.OK;
                response.Message = "Success";

            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        private async Task<double> GetDeposit(string accessToken, long accountId, bool isLive)
        {
            var dealsResponse = await _cTraderApiRepository.GetDeals(accessToken, accountId, isLive);
            var protoDeals = dealsResponse.OrderBy(x => x.ExecutionTimestamp).ToList();
            var Deposit = (protoDeals[1].ClosePositionDetail.Balance - (protoDeals[1].ClosePositionDetail.Commission + protoDeals[1].ClosePositionDetail.GrossProfit)) / Math.Pow(10, protoDeals[1].MoneyDigits);
            return Deposit;
        }

        private async Task GetAccountFromDeals(RepeatedField<ProtoOADeal> requestDeals, RepeatedField<ProtoOALightSymbol> symbols, Guid accountId)
        {
            var account = new AccountData();
            var protoDeals =  requestDeals.OrderBy(x => x.ExecutionTimestamp).ToList();
            var accountDeals = new List<Deal>();
            var dealsFromDB = _dealRepository.SelectAll().Where(x => x.AccountId == accountId);
            var Deposit = (protoDeals[1].ClosePositionDetail.Balance - (protoDeals[1].ClosePositionDetail.Commission + protoDeals[1].ClosePositionDetail.GrossProfit)) / Math.Pow(10, protoDeals[1].MoneyDigits);
            account.Deposit = Deposit;
            foreach(var deal in protoDeals)
            {
                if (dealsFromDB.FirstOrDefault(x => (x.AccountId == accountId && x.PositionId == deal.PositionId)) != null) continue;
                var newDeal = accountDeals.FirstOrDefault(x => x.PositionId == deal.PositionId);
                if (newDeal == null)
                {
                    newDeal = new Deal()
                    {
                        PositionId = deal.PositionId,
                        Direction = (deal.TradeSide == ProtoOATradeSide.Sell) ? Domain.Enums.Direction.Short : Domain.Enums.Direction.Long,
                        Volume = deal.Volume / (Math.Pow(10, 5 + deal.MoneyDigits)),
                        EntryTime = DateTimeOffset.FromUnixTimeMilliseconds(deal.ExecutionTimestamp).UtcDateTime,
                        EntryPrice = deal.ExecutionPrice,
                        Symbol = symbols.FirstOrDefault(x => x.SymbolId == deal.SymbolId).SymbolName,
                    };
                    accountDeals.Add(newDeal);
                }
                else
                {
                    newDeal.Profit += deal.ClosePositionDetail.GrossProfit/ Math.Pow(10, deal.MoneyDigits);
                    newDeal.Comission += deal.ClosePositionDetail.Commission / Math.Pow(10, deal.MoneyDigits);
                    newDeal.ExitPrice = deal.ExecutionPrice;
                    newDeal.ExitTime = DateTimeOffset.FromUnixTimeMilliseconds(deal.ExecutionTimestamp).UtcDateTime;
                    newDeal.ProfitPercentage = (newDeal.Profit / Deposit) * 100;
                    if (newDeal.ProfitPercentage < -0.1) newDeal.Result = Domain.Enums.Result.Loss;
                    else if (newDeal.ProfitPercentage > 0.1) newDeal.Result = Domain.Enums.Result.Win;
                    else newDeal.Result = Domain.Enums.Result.Breakeven;
                    newDeal.AccountId = accountId;
                }
            }

            await DealsDB(accountDeals, accountId);
        }

        private async Task DealsDB(List<Deal> newDeals, Guid accountId)
        {

            foreach(var newDeal in newDeals)
            {
                await _dealRepository.Create(newDeal);
            }
            var deals = _dealRepository.SelectAll().Where(x => x.AccountId == accountId);
            var response = new List<DealResponseModel>();
        }

        public async Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId)
        {
            var response = new BaseResponse<List<AccountResponseModel>>();
            try
            {
                var accounts = _cTraderAccountRepository.SelectAll().Where(x => x.UserID == UserId);
                response.Data = new List<AccountResponseModel>();
                foreach (var account in accounts)
                {
                    var accountResponse = new AccountResponseModel(account);
                    accountResponse.Provider = "CTrader";
                    var accountData = await GetAccountData(account.Id);
                    if (accountData.StatusCode == Domain.Enums.StatusCode.ERROR) continue;
                    accountResponse.Profit = accountData.Data.Profit;
                    accountResponse.ProfitPercentage = accountData.Data.ProfitPercentage;
                    accountResponse.Balance = accountData.Data.currentBalance;
                    accountResponse.DealsCount = accountData.Data.TotalDeals;
                    accountResponse.Deposit = accountData.Data.Deposit;
                    response.Data.Add(accountResponse);
                }
                response.StatusCode = Domain.Enums.StatusCode.OK;
                response.Message = $"User has {accounts.Count()} accounts";
            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<AccountData>> GetAccountData(Guid id)
        {
            var response = new BaseResponse<AccountData>();
            try
            {

                var account = _cTraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                    return response;
                }
                var deals = _dealRepository.SelectAll().Where(x => x.AccountId == id);
                if (deals.Count() == 0)
                {
                    response.Message = "Deals not found";
                    response.StatusCode = StatusCode.ERROR;
                }
                else
                {
                    var accountData = new AccountData();
                    foreach (var deal in deals)
                    {
                        accountData.Deals.Add(new DealResponseModel(deal));
                    }
                    accountData.Deposit = await GetDeposit(account.AccessToken, account.AccountId, account.IsLive);
                    accountData.Profit = accountData.Deals.Sum(x => x.Profit) + account.Deals.Sum(x => x.Comission);
                    accountData.currentBalance = accountData.Deposit + accountData.Profit;
                    accountData.TotalDeals = accountData.Deals.Count;
                    accountData.WonDeals = accountData.Deals.Where(x => x.Result == Result.Win).Count();
                    accountData.LostDeals = accountData.Deals.Where(x => x.Result == Result.Loss).Count();
                    accountData.BreakevenDeals = accountData.Deals.Where(x => x.Result == Result.Breakeven).Count();
                    accountData.LongDeals = accountData.Deals.Where(x => x.Direction == Direction.Long).Count();
                    accountData.ShortDeals = accountData.Deals.Where(x => x.Direction == Direction.Short).Count();
                    accountData.ProfitPercentage = Math.Round(accountData.Profit / accountData.Deposit * 100, 2);
                    accountData.Provider = "CTrader";
                    response.Data = accountData;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
            }
            return response;
        }
    }
}
