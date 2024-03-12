using Journal.DAL.Interfaces;
using Journal.DAL.Repositories;
using Journal.Domain.JsonModels;
using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Implementations
{
    public class DXTradeAccountService : IDXTradeAccountService
    {
        private readonly IDXTradeAccountRepository _dxTradeAccountRepository;
        private readonly IDXTradeDataRepository _dxTradeDataRepository;
        private readonly IDealRepository _dealRepository;

        public DXTradeAccountService(IDXTradeAccountRepository dxTradeAccountRepository, IDXTradeDataRepository dxTradeDataRepository, IDealRepository dealRepository)
        {
            _dxTradeAccountRepository = dxTradeAccountRepository;
            _dxTradeDataRepository = dxTradeDataRepository;
            _dealRepository = dealRepository;
        }

        public async Task<BaseResponse<List<AccountResponseModel>>> AddAccounts(string username, string password, string domain, Guid UserId)
        {
            var response = new BaseResponse<List<AccountResponseModel>>();
            response.Data = new List<AccountResponseModel>();

            try
            {
                var accounts = _dxTradeAccountRepository.SelectAll();
                var accountsResponse = await _dxTradeDataRepository.GetAccounts(username, password, domain);
                foreach (var account in accountsResponse.Accounts) 
                {
                    if (accounts.FirstOrDefault(x => x.Login == long.Parse(account.Account.Split(":").Last())) != null) continue;
                    var accountDB = new DXTradeAccount {
                        Id = Guid.NewGuid(),
                        Domain = domain,
                        Username = username,
                        Password = password,
                        UserID = UserId,
                        Login = long.Parse(account.Account.Split(":").Last())
                    };
                    if(!await _dxTradeAccountRepository.Create(accountDB))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "Can not add account to DB";
                        return response;
                    }
                    response.Data.Add(new AccountResponseModel(accountDB));
                }
                response.StatusCode = Domain.Enums.StatusCode.OK;
                response.Message = $"Added {response.Data.Count} accounts";
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
                var account = _dxTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if(account == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "User not found";
                }
                else
                {
                    if(await _dxTradeAccountRepository.Delete(account))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Message = "Success";
                        response.Data = true;
                    }
                    else
                    {
                        response.StatusCode= Domain.Enums.StatusCode.ERROR;
                        response.Message = "Can not delete the object";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message= ex.Message;
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
            }
            return response;
        }

        public async Task<BaseResponse<AccountData>> GetAccountData(Guid id)
        {
            var response = new BaseResponse<AccountData>();

            try
            {
                var account = _dxTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if (account == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var dealsResponse = await _dxTradeDataRepository.GetDeals(account.Username, account.Password, account.Domain, account.Login.ToString());
                    if (dealsResponse.Orders.Count() == 0)
                    {
                        response.Data = new AccountData();
                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "Account is empty. Please, open at least one deal";
                        return response;
                    }
                    var deposit = await _dxTradeDataRepository.GetDeposit(account.Username, account.Password, account.Domain, account.Login.ToString());
                    var accountData = await GetAccountFromDeals(dealsResponse.Orders, id, deposit);
                    accountData.UserId = account.UserID;
                    response.Data = accountData;
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Success";
                }

            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
            }
            return response;
        }

        private async Task<AccountData> GetAccountFromDeals(List<DXTradeAPIDealOrderJsonModel> orders, Guid accountId, double deposit)
        {
            var account = new AccountData();
            var apiDeals = orders.Where(x => x.Status == "COMPLETED");
            var accountDeals = new List<Deal>();
            var dealsFromDB = _dealRepository.SelectAll().Where(x => x.AccountId == accountId);
            account.Deposit = deposit;
            account.currentBalance = deposit;
            foreach (var deal in apiDeals.Where(x => x.Status == "COMPLETED"))
            {
                if (dealsFromDB.FirstOrDefault(x => (x.AccountId == accountId && x.PositionId == long.Parse(deal.OrderCode.Split(":").First()))) != null) continue;
                var newDeal = accountDeals.FirstOrDefault(x => x.PositionId == long.Parse(deal.OrderCode.Split(":").First()));
                if (newDeal == null)
                {
                    newDeal = new Deal()
                    {
                        PositionId = long.Parse(deal.OrderCode.Split(":").First()),
                        Direction = (deal.Side == "BUY") ? Domain.Enums.Direction.Long : Domain.Enums.Direction.Short,
                        Volume = (double)deal.Legs.First().Quantity / 100000,
                        EntryTime = deal.TransactionTime,
                        EntryPrice = deal.Legs.First().Price,
                        Symbol = deal.Instrument,
                    };
                    accountDeals.Add(newDeal);
                }
                else
                {
                    newDeal.Profit += deal.CashTransactions.Where(x => x.Type == "SETTLEMENT").Sum(x => x.Value); 
                    newDeal.Comission += deal.CashTransactions.Where(x => x.Type == "COMMISSION").Sum(x => x.Value);
                    newDeal.ExitPrice = deal.Legs.First().AveragePrice;
                    newDeal.ExitTime = deal.TransactionTime;
                    newDeal.ProfitPercentage = (newDeal.Profit / account.Deposit) * 100;
                    if (newDeal.ProfitPercentage < -0.1) newDeal.Result = Domain.Enums.Result.Loss;
                    else if (newDeal.ProfitPercentage > 0.1) newDeal.Result = Domain.Enums.Result.Win;
                    else newDeal.Result = Domain.Enums.Result.Breakeven;
                    newDeal.AccountId = accountId;
                    account.currentBalance += (newDeal.Comission + newDeal.Profit);
                }
            }
            account.Profit = account.currentBalance - account.Deposit;
            account.ProfitPercentage = (account.Profit / account.Deposit) * 100;
            var deals = await DealsDB(accountDeals, accountId);
            account.Deals = deals;
            account.BreakevenDeals = account.Deals.Where(x => x.Result == Domain.Enums.Result.Breakeven).Count();
            account.WonDeals = account.Deals.Where(x => x.Result == Domain.Enums.Result.Win).Count();
            account.LostDeals = account.Deals.Where(x => x.Result == Domain.Enums.Result.Loss).Count();
            account.LongDeals = account.Deals.Where(x => x.Direction == Domain.Enums.Direction.Long).Count();
            account.ShortDeals = account.Deals.Where(x => x.Direction == Domain.Enums.Direction.Short).Count();
            account.TotalDeals = account.Deals.Count();
            return account;
        }

        private async Task<List<DealResponseModel>> DealsDB(List<Deal> newDeals, Guid accountId)
        {

            foreach (var newDeal in newDeals)
            {
                await _dealRepository.Create(newDeal);
            }
            var deals = _dealRepository.SelectAll().Where(x => x.AccountId == accountId);
            var response = new List<DealResponseModel>();
            foreach (var deal in deals)
            {
                response.Add(new DealResponseModel(deal));
            }
            return response;
        }

        public async Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId)
        {
            var response = new BaseResponse<List<AccountResponseModel>>();
            try
            {
                var accounts = _dxTradeAccountRepository.SelectAll().Where(x => x.UserID == UserId);
                response.Data = new List<AccountResponseModel>();
                foreach (var account in accounts)
                {
                    var accountResponse = new AccountResponseModel(account);
                    accountResponse.Provider = "DXTrade";
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
    }
}
