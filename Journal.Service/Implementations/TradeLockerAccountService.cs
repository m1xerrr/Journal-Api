using Journal.DAL.Interfaces;
using Journal.DAL.Repositories;
using Journal.Domain.Enums;
using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Implementations
{
    public class TradeLockerAccountService : ITradeLockerAccountService
    {
        private readonly ITradeLockerAccountRepository _tradeLockerAccountRepository;
        private readonly ITradeLockerAPIRepository _tradeLockerAPIRepository;
        private readonly IDealRepository _dealRepository;
        private readonly IDescriptionRepository _descriptionRepository;
        public TradeLockerAccountService(ITradeLockerAPIRepository tradeLockerAPIRepository, ITradeLockerAccountRepository tradeLockerAccountRepository, IDealRepository dealRepository, IDescriptionRepository descriptionRepository) 
        {
           _tradeLockerAccountRepository = tradeLockerAccountRepository;
           _tradeLockerAPIRepository = tradeLockerAPIRepository;
            _dealRepository = dealRepository;
            _descriptionRepository = descriptionRepository;
        }

        public async Task<BaseResponse<AccountResponseModel>> AddAccount(string email, string server, string password, bool isLive, Guid UserId, long accountId)
        {
            var response = new BaseResponse<AccountResponseModel>();
            try
            {
                if (_tradeLockerAccountRepository.SelectAll().FirstOrDefault(x => x.Login == accountId) != null)
                {
                    response.Message = "Account already exists";
                    response.StatusCode = StatusCode.ERROR;
                }
                else
                {
                    if (await _tradeLockerAPIRepository.GetAccount(email, password, server, isLive, accountId) == null)
                    {

                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "Account not found";

                    }
                    else
                    {
                        TradeLockerAccount account = new TradeLockerAccount()
                        {
                            Email = email,
                            Password = password,
                            Server = server,
                            Live = isLive,
                            Login = accountId,
                            UserID = UserId,
                            Id = Guid.NewGuid(),
                        };
                        account.Deposit = await _tradeLockerAPIRepository.GetDeposit(email, password, server, isLive, accountId);
                        if (await _tradeLockerAccountRepository.Create(account))
                        {
                            response.StatusCode = StatusCode.OK;
                            response.Message = "Success";
                            response.Data = new AccountResponseModel(account);
                        }
                        else
                        {
                            response.StatusCode = Domain.Enums.StatusCode.ERROR;
                            response.Message = "Error during saving the account";
                        }
                        await LoadAccountData(_tradeLockerAccountRepository.SelectAll().FirstOrDefault(x => x.Login == accountId).Id);
                    }
                }

            }
            catch (Exception ex) 

            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAccountTradeLockerAccount(Guid id)
        {
            var response = new BaseResponse<bool>();
            try
            {
                foreach(var deal in _dealRepository.SelectAll().Where(x => x.AccountId == id))
                {
                    foreach(var description in _descriptionRepository.SelectAll().Where(x => x.DealId == deal.Id))
                    {
                        await _descriptionRepository.Delete(description);
                    }
                    await _dealRepository.Delete(deal);
                }
                var account = _tradeLockerAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if(await _tradeLockerAccountRepository.Delete(account))
                {
                    response.StatusCode = StatusCode.OK;
                    response.Data = true;
                    response.Message = "Success";
                }
                else
                {
                    response.StatusCode= Domain.Enums.StatusCode.ERROR;
                    response.Message = "DB Error";
                }
            }
            catch (Exception ex) 
            {
                response.Message += ex.Message;
                response.StatusCode = StatusCode.ERROR;
            }
            return response;
        }

        public async Task<BaseResponse<AccountData>> GetAccountData(Guid id)
        {
            var response = new BaseResponse<AccountData>();
            try
            {

                var account = _tradeLockerAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                    return response;
                }
                var descriptions = _descriptionRepository.SelectAll();
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

                    accountData.UserId = account.UserID;
                    accountData.Deposit = account.Deposit;
                    accountData.Profit = accountData.Deals.Sum(x => x.Profit) + account.Deals.Sum(x => x.Comission);
                    accountData.currentBalance = accountData.Deposit + accountData.Profit;
                    accountData.TotalDeals = accountData.Deals.Count;
                    accountData.WonDeals = accountData.Deals.Where(x => x.Result == Result.Win).Count();
                    accountData.LostDeals = accountData.Deals.Where(x => x.Result == Result.Loss).Count();
                    accountData.BreakevenDeals = accountData.Deals.Where(x => x.Result == Result.Breakeven).Count();
                    accountData.LongDeals = accountData.Deals.Where(x => x.Direction == Direction.Long).Count();
                    accountData.ShortDeals = accountData.Deals.Where(x => x.Direction == Direction.Short).Count();
                    accountData.ProfitPercentage = Math.Round(accountData.Profit / accountData.Deposit * 100, 2);
                    accountData.Provider = "DXTrade";
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

        public async Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId)
        {
            var response = new BaseResponse<List<AccountResponseModel>>();
            try
            {
                var accounts = _tradeLockerAccountRepository.SelectAll().Where(x => x.UserID == UserId);
                response.Data = new List<AccountResponseModel>();
                foreach (var account in accounts)
                {
                    var accountResponse = new AccountResponseModel(account);
                    accountResponse.Provider = "TradeLocker";
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

        public async Task<BaseResponse<bool>> LoadAccountData(Guid id)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _tradeLockerAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if(account == null)
                {
                    response.StatusCode= Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var apiDeals = await _tradeLockerAPIRepository.GetDeals(account.Email, account.Password, account.Server, account.Live, account.Login);
                    var deals = new List<Deal>();
                    for (int i = 0; i < apiDeals.Data.Count; i++)
                    {
                        var deal = apiDeals.Data[i];
                        var newDeal = new Deal()
                        {
                            Symbol = deal[0],
                            AccountId = account.Id,
                            Comission = Double.Parse(deal[10]),
                            Direction = deal[3].ToString() == "Buy" ? Direction.Long : Direction.Short,
                            EntryPrice = Double.Parse(deal[5]),
                            ExitPrice = Double.Parse(deal[9]),
                            EntryTime = DateTimeOffset.FromUnixTimeMilliseconds(Int64.Parse(deal[1])).UtcDateTime,
                            ExitTime = DateTimeOffset.FromUnixTimeMilliseconds(Int64.Parse(deal[8])).UtcDateTime,
                            Profit = Double.Parse(deal[12]),
                            ProfitPercentage = Double.Parse(deal[12]) / account.Deposit * 100,
                            Volume = Double.Parse(deal[4]),
                            PositionId = Int64.Parse(deal.Last()),
                        };
                        if (newDeal.ProfitPercentage < -0.1) newDeal.Result = Result.Loss;
                        else if (newDeal.ProfitPercentage > 0.1) newDeal.Result = Result.Win;
                        else newDeal.Result = Result.Breakeven;
                        deals.Add(newDeal);
                    }
                    await DealsToDB(deals);
                    response.StatusCode = StatusCode.OK;
                    response.Message = "Success";
                }
            }
            catch (Exception ex)
            {
                response.Message += ex.Message;
                response.StatusCode= Domain.Enums.StatusCode.ERROR;
            }
            return response;
        }

        private async Task DealsToDB(List<Deal> deals)
        {
            foreach(var deal in deals)
            {
                if (_dealRepository.SelectAll().FirstOrDefault(x => x.PositionId == deal.PositionId) != null) continue;
                else await _dealRepository.Create(deal);
            }
        }
    }
}
