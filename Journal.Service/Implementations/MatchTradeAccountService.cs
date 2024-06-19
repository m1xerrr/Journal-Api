using Journal.DAL.Interfaces;
using Journal.DAL.Repositories;
using Journal.Domain.Enums;
using Journal.Domain.Helpers;
using Journal.Domain.JsonModels.TradingAccount;
using Journal.Domain.Models;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;

namespace Journal.Service.Implementations
{
    public class MatchTradeAccountService : IMatchTradeAccountService
    {
        private readonly IMatchTradeApiRepository _matchTradeApiRepository;
        private readonly IMatchTradeAccountRepository _matchTradeAccountRepository;
        private readonly ITradeLockerAPIRepository _tradeLockerApiRepository;
        private readonly IDescriptionRepository _descriptionRepository;
        private readonly IDealRepository _dealRepository;

        public MatchTradeAccountService(IMatchTradeApiRepository matchTradeApiRepository, IMatchTradeAccountRepository matchTradeAccountRepository, ITradeLockerAPIRepository tradeLockerApiRepository, IDescriptionRepository descriptionRepository, IDealRepository dealRepository)
        {
            _matchTradeApiRepository = matchTradeApiRepository;
            _matchTradeAccountRepository = matchTradeAccountRepository;
            _tradeLockerApiRepository = tradeLockerApiRepository;
            _descriptionRepository = descriptionRepository;
            _dealRepository = dealRepository;
        }

        public async Task<BaseResponse<AccountResponseModel>> AddAccount(string email, int brokerId, string password, bool isLive, Guid UserId, long accountId)
        {
            var response = new BaseResponse<AccountResponseModel>();
            try
            {
                if (_matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Login == accountId) != null)
                {
                    response.Message = "Account already exists";
                    response.StatusCode = StatusCode.ERROR;
                }
                else
                {
                    var accountResponse = await _matchTradeApiRepository.InitializeSession(isLive, email, password, brokerId, accountId);
                    if (accountResponse == null)
                    {

                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "Account not found";

                    }
                    else
                    {
                        MatchTradeAccount account = new MatchTradeAccount()
                        {
                            Email = email,
                            Password = password,
                            BrokerId = brokerId,
                            UUID = accountResponse.UUID,
                            LiveStatus = isLive,
                            Login = accountId,
                            UserID = UserId,
                            Id = Guid.NewGuid(),
                        };
                        account.Deposit = await _matchTradeApiRepository.GetDeposit(isLive, email, password, brokerId, accountId);
                        if(account.Deposit == 0)
                        {
                            response.StatusCode = StatusCode.ERROR;
                            response.Message = "Error during retrieving account data";
                        }
                        else { 
                        if (await _matchTradeAccountRepository.Create(account))
                        {
                            response.StatusCode = StatusCode.OK;
                            response.Message = "Success";
                            await LoadAccountData(account.Id);
                            response.Data = new AccountResponseModel(account);
                        }
                        else
                        {
                            response.StatusCode = Domain.Enums.StatusCode.ERROR;
                            response.Message = "Error during saving the account";
                        }
                        }
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

        public async Task<BaseResponse<bool>> DeleteAccount(Guid id)
        {
            var response = new BaseResponse<bool>();
            try
            {
                foreach (var deal in _dealRepository.SelectAll().Where(x => x.AccountId == id))
                {
                    foreach (var description in _descriptionRepository.SelectAll().Where(x => x.DealId == deal.Id))
                    {
                        await _descriptionRepository.Delete(description);
                    }
                    await _dealRepository.Delete(deal);
                }
                var account = _matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if(account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Data = false;
                    response.Message = "Account not found";
                }
                else
                {
                    if(!(await _matchTradeAccountRepository.Delete(account)))
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Data = false;
                        response.Message = "Can not delete account";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Data = true;
                        response.Message = "Success";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message += ex.Message;
                response.StatusCode = StatusCode.ERROR;
                response.Data = false;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> DeleteOrder(ClosePositionJsonModel model)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == model.AccountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var order = (await _matchTradeApiRepository.GetOrders(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login)).Orders.FirstOrDefault(x => x.Id == model.positionId);
                    var position = (await _matchTradeApiRepository.GetPositions(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login)).Positions.FirstOrDefault(x => x.Id == model.positionId);
                    if(order != null)
                    {
                        if (await _matchTradeApiRepository.ClosePosition(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login, model.positionId, order.Symbol, order.Side, "LIMIT", double.Parse(order.Volume)))
                        {
                            response.StatusCode = StatusCode.OK;
                            response.Message = "Success";
                        }
                        else
                        {
                            response.StatusCode = StatusCode.ERROR;
                            response.Message = "Deleting order error";
                        }
                    } 
                    else if(position != null)
                    {
                        if (await _matchTradeApiRepository.ClosePosition(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login, model.positionId, position.Symbol, position.Side, "MARKET", position.Volume))
                        {
                            response.StatusCode = StatusCode.OK;
                            response.Message = "Success";
                        }
                        else
                        {
                            response.StatusCode = StatusCode.ERROR;
                            response.Message = "Deleting order error";
                        }
                    }
                    else
                    {
                        response.StatusCode= StatusCode.ERROR;
                        response.Message = "Position with such id not found";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = StatusCode.ERROR;
            }
            return response;
        }

        public async Task<BaseResponse<AccountData>> GetAccountData(Guid id)
        {
            var response = new BaseResponse<AccountData>();
            try
            {

                var account = _matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
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
                    if (newDeals.Count() == 0)
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
                        accountData.LongDeals = accountData.Deals.Where(x => x.Direction == Direction.Long).Count();
                        accountData.ShortDeals = accountData.Deals.Where(x => x.Direction == Direction.Short).Count();
                        accountData.ProfitPercentage = Math.Round(accountData.Profit / accountData.Deposit * 100, 2);
                        accountData.Winrate = Math.Round((double)accountData.WonDeals / (double)accountData.TotalDeals * 100, 2);
                        accountData.Lots = accountData.Deals.Select(x => x.Volume).Sum();
                        accountData.AverageLoss = accountData.Deals.Where(x => x.Result == Result.Loss).Select(x => x.Profit).Average();
                        accountData.AverageWin = accountData.Deals.Where(x => x.Result == Result.Win).Select(x => x.Profit).Average();
                        accountData.DailyProfit = accountData.Deals.Where(x => x.EntryTime.Date == DateTime.Today).Sum(x => x.Profit + x.Comission);
                        accountData.Provider = "MatchTrade";
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
                    if(accountData.LostDeals == 0) accountData.AverageLoss = 0;
                    else accountData.AverageLoss = accountData.Deals.Where(x => x.Result == Result.Loss).Select(x => x.Profit).Average();
                    if (accountData.WonDeals == 0) accountData.AverageWin = 0;
                    else accountData.AverageWin = accountData.Deals.Where(x => x.Result == Result.Win).Select(x => x.Profit).Average();
                    accountData.DailyProfit = accountData.Deals.Where(x => x.EntryTime.Date == DateTime.Today).Sum(x => x.Profit + x.Comission);
                    accountData.Provider = "MatchTrade";
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


        public async Task<BaseResponse<List<PositionResponseModel>>> GetPositions(Guid id)
        {
            var response = new BaseResponse<List<PositionResponseModel>>();
            try
            {
                var account = _matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var positions = await _matchTradeApiRepository.GetPositions(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login);
                    response.Data = new List<PositionResponseModel>();
                    foreach (var position in positions.Positions)
                    {
                        response.Data.Add(new PositionResponseModel
                        {
                            Id = position.Id,
                            Symbol = position.Symbol,
                            Direction = position.Side == "SELL" ? Direction.Short : Direction.Long,
                            OpenPrice = position.OpenPrice,
                            OpenTime = position.OpenTime,
                            Volume = position.Volume,
                            Profit = position.NetProfit
                        });
                    }
                    response.StatusCode = StatusCode.OK;
                    response.Message = "Success";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode
                    .ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<List<OrderResponseModel>>> GetOrders(Guid id)
        {
            var response = new BaseResponse<List<OrderResponseModel>>();
            try
            {
                var account = _matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var orders = await _matchTradeApiRepository.GetOrders(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login);
                    response.Data = new List<OrderResponseModel>();
                    foreach (var order in orders.Orders)
                    {
                        response.Data.Add(new OrderResponseModel
                        {
                            Id = order.Id,
                            Direction = order.Side == "SELL" ? Direction.Short : Direction.Long,
                            OrderTime = order.CreationTime,
                            Price = double.Parse(order.ActivationPrice),
                            Symbol = order.Symbol,
                            Volume = double.Parse(order.Volume),
                        });
                    }
                    response.StatusCode = StatusCode.OK;
                    response.Message = "Success";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode
                    .ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<List<AccountResponseModel>>> GetUserAccounts(Guid UserId)
        {
            var response = new BaseResponse<List<AccountResponseModel>>();
            response.Data = new List<AccountResponseModel>();
            try
            {
                var accounts = _matchTradeAccountRepository.SelectAll().Where(x => x.UserID == UserId);
                
                foreach (var account in accounts)
                {
                    response.Data.Add(new AccountResponseModel(account));
                    
                }
                response.StatusCode = StatusCode.OK;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response
                    .Message = ex.Message;
                response .StatusCode = StatusCode.ERROR;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> LoadAccountData(Guid id)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                var sessionData = await _matchTradeApiRepository.InitializeSession(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login);
                var symbols = (await _tradeLockerApiRepository.GetSymbols()).Data.Instruments.Select(x => x.Name).ToList();
                var deals = await _matchTradeApiRepository.GetDeals(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login, sessionData.TradingApiToken, sessionData.CoAuthToken, sessionData.UUID, symbols);
                var dbDeals = _dealRepository.SelectAll().Where(x => x.AccountId == id);
                var newDeals = new List<Deal>();
                foreach(var deal in deals)
                {
                    var dealId = long.Parse(deal.Id.Where(char.IsDigit).ToArray());
                    if (dbDeals.FirstOrDefault(x => x.PositionId == dealId) == null)
                    {
                        newDeals.Add(new Deal()
                        {
                            Direction = deal.Side == "SELL" ? Direction.Short : Direction.Long,
                            AccountId = id,
                            Comission = deal.Commission,
                            EntryPrice = deal.OpenPrice,
                            EntryTime = deal.OpenTime,
                            ExitPrice = deal.ClosePrice,
                            ExitTime = deal.Time,
                            PositionId = dealId,
                            Profit = deal.Profit,
                            Symbol = deal.Symbol,
                            Volume = deal.Volume,
                            ProfitPercentage = deal.Profit / account.Deposit * 100,
                            Result = (deal.Profit / account.Deposit * 100 < 0) ? Result.Loss : Result.Win
                        });
                    }
                    
                }
                await DealsToDB(newDeals);
                response.StatusCode = StatusCode.OK;
                response.Message = "Success";
            }
            catch(Exception ex)
            {
                response .Message = ex.Message;
                response.StatusCode = StatusCode.ERROR;
            }
            return response;
        }

        private async Task DealsToDB(List<Deal> deals)
        {
            foreach (var deal in deals)
            {
                if (_dealRepository.SelectAll().FirstOrDefault(x => x.PositionId == deal.PositionId) != null) continue;
                else await _dealRepository.Create(deal);
            }
        }

        public async Task<BaseResponse<bool>> PlaceOrder(OpenPositionJsonModel model)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == model.AccountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    double volume = 0;
                    if (model.Price == 0 && model.Type < 3)
                    {
                        double priceTmp = await _tradeLockerApiRepository.GetPrice(symbol: model.Symbol);
                        if (priceTmp == 0) throw new Exception("Invalid symbol data");
                        volume = CalculateLotsHelper.CalculateForexLots(model.Risk, priceTmp, model.Stoploss);
                    }
                    else
                        volume = CalculateLotsHelper.CalculateForexLots(model.Risk, model.Price, model.Stoploss);
                    if (await _matchTradeApiRepository.OpenPosition(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login, model.Price, model.Stoploss, model.TakeProfit, volume, model.Type, model.Symbol))
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Placing order error";
                    }
                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode= StatusCode.ERROR;
            }
            return response;
        }
    }
}
