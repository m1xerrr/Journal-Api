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
using Journal.Domain.JsonModels.TradingAccount;
using Journal.Domain.Helpers;

namespace Journal.Service.Implementations
{
    public class CTraderAccountService : ICTraderAccountService
    {
        private readonly ICTraderAccountRepository _cTraderAccountRepository;
        private readonly ICTraderApiRepository _cTraderApiRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDealRepository _dealRepository;
        private readonly IDescriptionRepository _descriptionRepository;
        private readonly ITradeLockerAPIRepository _tradeLockerAPIRepository;

        public CTraderAccountService(ICTraderApiRepository apiRepository, ICTraderAccountRepository accountRepository, IUserRepository userRepository, IDealRepository dealRepository, IDescriptionRepository descriptionRepository, ITradeLockerAPIRepository tradeLockerAPIRepository)
        {
            _cTraderApiRepository = apiRepository;
            _cTraderAccountRepository = accountRepository;
            _userRepository = userRepository;
            _dealRepository = dealRepository;
            _descriptionRepository = descriptionRepository;
            _tradeLockerAPIRepository = tradeLockerAPIRepository;
        }
        public async Task<BaseResponse<AccountResponseModel>> AddAccount(string accessToken, Guid UserId, long accountId)
        {
            var response = new BaseResponse<AccountResponseModel>();
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
                var account = accounts.CtidTraderAccount.FirstOrDefault(x => (long)x.TraderLogin == accountId);
                if (account == null)
                {
                    response.StatusCode=Domain.Enums.StatusCode.ERROR;
                    response.Message = "Account with such login not found";
                }
                else {
                    if (accountsDB.FirstOrDefault(x => (x.AccountId == (long)account.TraderLogin) && x.UserID == UserId) != null)
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Account with such id already added";
                    }
                    var newAccount = new CTraderAccount()
                    {
                        AccessToken = accessToken,
                        UserID = UserId,
                        Id = Guid.NewGuid(),
                        AccountId = (long)account.CtidTraderAccountId,
                        IsLive = account.IsLive,
                        Login = account.TraderLogin
                    };
                    newAccount.Deposit = await GetDeposit(newAccount.AccessToken, newAccount.AccountId, newAccount.IsLive);
                    if (await _cTraderAccountRepository.Create(newAccount))
                    {
                        var responseAccount = new AccountResponseModel(newAccount);
                        responseAccount.Provider = "CTrader";
                        response.Data = responseAccount;
                    }
                    else
                    {
                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "Can not add account to DB";
                        return response;
                    }
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Success";
                    await LoadAccountData(newAccount.Id);
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
                var deals = _dealRepository.SelectAll().Where(x => x.AccountId == id).ToList();
                foreach (var deal in deals)
                {
                    _descriptionRepository.SelectAll();
                    foreach(var description in deal.DescriptionItems)
                    {
                        await _descriptionRepository.Delete(description);
                    }
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

        public async Task<BaseResponse<List<OrderResponseModel>>> GetOrders(Guid accountId)
        {
            var response = new BaseResponse<List<OrderResponseModel>>();
            try
            {
                var account = _cTraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.Message = "Account not found";
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    return response;
                }

                var ordersResponse = await _cTraderApiRepository.GetOrders(account.AccessToken, account.AccountId, account.IsLive);
                if (ordersResponse.Count() == 0)
                {
                    response.Data = new List<OrderResponseModel>();
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "No orders found";
                }
                else { 

                    response.Data = new List<OrderResponseModel>();
                    var symbols = await _cTraderApiRepository.GetSymbols(account.AccessToken, account.AccountId, account.IsLive);
                    foreach(var order in ordersResponse)
                    {
                        var symbol = symbols.FirstOrDefault(x => x.SymbolId == order.TradeData.SymbolId);
                        int multiplier = symbol.SymbolCategoryId == 1 ? 10000000 : 100;
                        if (symbol.SymbolName.Contains("XAU")) multiplier *= 100;
                        response.Data.Add(new OrderResponseModel()
                        {
                            Id = order.PositionId.ToString(),
                            Direction = order.TradeData.TradeSide == ProtoOATradeSide.Buy ? Direction.Long : Direction.Short,
                            Price = order.ExecutionPrice,
                            OrderTime = DateTimeOffset.FromUnixTimeMilliseconds(order.UtcLastUpdateTimestamp).UtcDateTime,
                            Symbol = symbol.SymbolName,
                            Volume = order.TradeData.Volume / multiplier,
                        });
                    }
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Success";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<List<PositionResponseModel>>> GetPositions(Guid accountId)
        {
            var response = new BaseResponse<List<PositionResponseModel>>();
            try
            {
                var account = _cTraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.Message = "Account not found";
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    return response;
                }

                var positionsResponse = await _cTraderApiRepository.GetPositions(account.AccessToken, account.AccountId, account.IsLive);
                if (positionsResponse.Count() == 0)
                {
                    response.Data = new List<PositionResponseModel>();
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "No positions found";
                }
                else {
                    response.Data = new List<PositionResponseModel>();
                    var symbols = await _cTraderApiRepository.GetSymbols(account.AccessToken, account.AccountId, account.IsLive);
                    foreach (var position in positionsResponse)
                    {
                        var symbol = symbols.FirstOrDefault(x => x.SymbolId == position.TradeData.SymbolId);
                        double multiplier = Math.Pow(10, position.MoneyDigits);
                        response.Data.Add(new PositionResponseModel()
                        {
                            Id = position.PositionId.ToString(),
                            Symbol = symbol.SymbolName,
                            Direction = position.TradeData.TradeSide == ProtoOATradeSide.Buy ? Direction.Long : Direction.Short,
                            OpenPrice = position.Price,
                            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(position.TradeData.OpenTimestamp).UtcDateTime,
                            Volume = position.TradeData.Volume / multiplier,
                        });
                    }
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Success";
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> PlaceOrder(OpenPositionJsonModel model)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _cTraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == model.AccountId);
                if (account == null)
                {
                    response.Message = "Account not found";
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    return response;
                }
                double volume = 0;
                if (model.Price == 0 && model.Type < 3)
                {
                    double priceTmp = await _tradeLockerAPIRepository.GetPrice(symbol: model.Symbol);
                    if (priceTmp == 0) throw new Exception("Invalid symbol data");
                    volume = CalculateLotsHelper.CalculateForexLots(model.Risk, priceTmp, model.Stoploss);
                }
                else
                    volume = CalculateLotsHelper.CalculateForexLots(model.Risk, model.Price, model.Stoploss);
                var longVolume = (long)(volume*10000000);
                var newOrder = await _cTraderApiRepository.PlaceOrder(account.AccessToken, account.AccountId, account.IsLive, model.Symbol, model.Type, longVolume, model.Stoploss, model.TakeProfit, model.Price);
                if (newOrder)
                {
                    response.Data = true;
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Order sent on CTrader platform";
                }
                else
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Error during placing order";
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> CloseOrder(Guid accountId, long id)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _cTraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.Message = "Account not found";
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    return response;
                }
                var position = (await _cTraderApiRepository.GetPositions(account.AccessToken, account.AccountId, account.IsLive)).FirstOrDefault(x => x.PositionId == id);
                var order = (await _cTraderApiRepository.GetOrders(account.AccessToken, account.AccountId, account.IsLive)).FirstOrDefault(x => x.OrderId == id);
                if(position != null)
                {
                    await _cTraderApiRepository.DeletePosition(account.AccessToken, account.AccountId, account.IsLive, id, position.TradeData.Volume);
                    response.Data = true;
                    response.Message = "Close position command has been sent";
                    response.StatusCode = StatusCode.OK;
                }
                else if(order != null)
                {
                    await _cTraderApiRepository.DeleteOrder(account.AccessToken, account.AccountId, account.IsLive, id);
                    response.Data = true;
                    response.Message = "Delete order command has been sent";
                    response.StatusCode = StatusCode.OK;
                }
                else
                {
                    response.StatusCode= StatusCode.ERROR;
                    response.Message = "Order or position with such id has not been found";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<List<string>>> GetSymbols(Guid accountId)
        {
            var response = new BaseResponse<List<string>> ();
            try
            {
                var account = _cTraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.Message = "Account not found";
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    return response;
                }

                var symbolsResponse = await _cTraderApiRepository.GetSymbols(account.AccessToken, account.AccountId, account.IsLive);
                if (symbolsResponse.Count() == 0)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Symbols not found";
                    return response;
                }
                response.Data = symbolsResponse.Select(x => x.SymbolName).ToList();
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
            double Deposit = 0;
            if (dealsResponse.Count > 1)
            {
                var protoDeals = dealsResponse.OrderBy(x => x.ExecutionTimestamp).ToList();
                Deposit = (protoDeals[1].ClosePositionDetail.Balance - (protoDeals[1].ClosePositionDetail.Commission + protoDeals[1].ClosePositionDetail.GrossProfit)) / Math.Pow(10, protoDeals[1].MoneyDigits);
            }
            else
            {
                var accountState = await _cTraderApiRepository.AccountStateRequest(accessToken, accountId, isLive);
                Deposit = accountState.Balance /100;
            }
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
                    if (newDeal.ProfitPercentage < 0) newDeal.Result = Domain.Enums.Result.Loss;
                    else  newDeal.Result = Domain.Enums.Result.Win;
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
                var descriptions = _descriptionRepository.SelectAll();
                var deals = _dealRepository.SelectAll().Where(x => x.AccountId == id);
                if (deals.Count() == 0)
                {
                    await LoadAccountData(id);
                    var newDeals = _dealRepository.SelectAll().Where(_x => _x.AccountId == id);
                    if (newDeals.Count() == 0) {
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
                        accountData.LongDeals = accountData.Deals.Where(x => x.Direction == Direction.Long).Count();
                        accountData.ShortDeals = accountData.Deals.Where(x => x.Direction == Direction.Short).Count();
                        accountData.ProfitPercentage = Math.Round(accountData.Profit / accountData.Deposit * 100, 2);
                        accountData.Winrate = Math.Round((double)accountData.WonDeals / (double)accountData.TotalDeals * 100, 2);
                        accountData.Lots = accountData.Deals.Select(x => x.Volume).Sum();
                        accountData.AverageLoss = accountData.Deals.Where(x => x.Result == Result.Loss).Select(x => x.Profit).Average();
                        accountData.AverageWin = accountData.Deals.Where(x => x.Result == Result.Win).Select(x => x.Profit).Average();
                        accountData.DailyProfit = accountData.Deals.Where(x => x.EntryTime.Date == DateTime.Today).Sum(x => x.Profit + x.Comission);
                        accountData.Provider = "CTrader";
                        response.Data = accountData;
                    }
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
                    accountData.LongDeals = accountData.Deals.Where(x => x.Direction == Direction.Long).Count();
                    accountData.ShortDeals = accountData.Deals.Where(x => x.Direction == Direction.Short).Count();
                    accountData.ProfitPercentage = Math.Round(accountData.Profit / accountData.Deposit * 100, 2);
                    accountData.Winrate = Math.Round((double)accountData.WonDeals / (double)accountData.TotalDeals * 100, 2);
                    accountData.Lots = accountData.Deals.Select(x => x.Volume).Sum();
                    accountData.AverageLoss = accountData.Deals.Where(x => x.Result == Result.Loss).Select(x => x.Profit).Average();
                    accountData.AverageWin = accountData.Deals.Where(x => x.Result == Result.Win).Select(x => x.Profit).Average();
                    accountData.DailyProfit = accountData.Deals.Where(x => x.EntryTime.Date == DateTime.Today).Sum(x => x.Profit + x.Comission);
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
