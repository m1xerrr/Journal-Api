using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Journal.Domain.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Journal.Domain.Enums;
using Journal.DAL.Repositories;
using Microsoft.Identity.Client;
using Journal.Domain.JsonModels.MetaTrader;
using Journal.Domain.JsonModels.TradingAccount;
using Journal.Domain.Helpers;

namespace Journal.Service.Implementations
{
    public class MTAccountService : IMTAccountService
    {
        private readonly IMTAccountRepository _mtAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMTDataRepository _mtDataRepository;
        private readonly IDealRepository _mtDealRepository;
        private readonly IDescriptionRepository _descriptionRepository;
        private readonly ITradeLockerAPIRepository _tradeLockerAPIRepository;
        public MTAccountService(IMTAccountRepository mtAccountRepository, IUserRepository userRepository, IMTDataRepository mtDataRepository, IDealRepository mTDealRepository, IDescriptionRepository descriptionRepository, ITradeLockerAPIRepository tradeLockerAPIRepository)
        {
            _mtAccountRepository = mtAccountRepository;
            _userRepository = userRepository;
            _mtDataRepository = mtDataRepository;
            _mtDealRepository = mTDealRepository;
            _descriptionRepository = descriptionRepository;
            _tradeLockerAPIRepository = tradeLockerAPIRepository;
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
                account.Deposit = await GetDeposit(await _mtDataRepository.GetDeals(accountModel));

                

                if (await _mtAccountRepository.Create(account))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Data = new AccountResponseModel(account);
                    await LoadAccountData(account.Id);
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
                    var deals = _mtDealRepository.SelectAll().Where(x => x.AccountId == accountId).ToList();
                    foreach (var deal in deals)
                    {
                        _descriptionRepository.SelectAll();
                        foreach (var description in deal.DescriptionItems)
                        {
                            await _descriptionRepository.Delete(description);
                        }
                        await _mtDealRepository.Delete(deal);
                    }
                    if (await _mtAccountRepository.Delete(account))
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
                    var accountResponse = new AccountResponseModel(account);
                    var accountData = await GetAccountData(account.Id);
                    accountResponse.Profit = accountData.Data.Profit;
                    accountResponse.Balance = accountData.Data.currentBalance;
                    accountResponse.ProfitPercentage = accountData.Data.ProfitPercentage;
                    accountResponse.DealsCount = accountData.Data.TotalDeals;
                    accountResponse.Deposit = accountData.Data.Deposit;
                    accountsResponse.Add(accountResponse);
                }
                response.StatusCode = Domain.Enums.StatusCode.OK;
                response.Data = accountsResponse;
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<BaseResponse<bool>> LoadAccountData(Guid accountId)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                    return response;
                }
                var accountJson = new MTAccountJsonModel();
                accountJson.Password = account.Password;
                accountJson.UserId = account.UserID;
                accountJson.Id = account.Id;
                accountJson.Login = account.Login;
                accountJson.Server = account.Server;
                var deals = await _mtDataRepository.GetDeals(accountJson);
                if (deals.Count == 0)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No deals found";
                }
                else
                {
                    if(await DealstoAccount(account.Id, deals))
                    {
                        response.Data = true;
                        response.StatusCode = StatusCode.OK;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.Data = false;
                        response.StatusCode= StatusCode.ERROR;
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

        public async Task<BaseResponse<bool>> OpenPosition(OpenPositionJsonModel model)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == model.AccountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    double volume = 0;
                    if (model.Price == 0 && model.Type < 3) { 
                        double priceTmp = await _tradeLockerAPIRepository.GetPrice(symbol: model.Symbol);
                        if (priceTmp == 0) throw new Exception("Invalid symbol data");
                        volume = CalculateLotsHelper.CalculateForexLots(model.Risk, (await _tradeLockerAPIRepository.GetPrice(symbol: model.Symbol)), model.Stoploss);
                    }
                    else
                        volume = CalculateLotsHelper.CalculateForexLots(model.Risk, model.Price, model.Stoploss);
                    var order = await _mtDataRepository.PlaceOrder(account.Login, account.Password, account.Server, model.Symbol, volume, model.Type, model.Price, model.Stoploss, model.TakeProfit);
                    if (order)
                    {
                        response.Data = true;
                        response.StatusCode = StatusCode.OK;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Error occured during placing order";
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

        public async Task<BaseResponse<List<OrderResponseModel>>> GetOrders(Guid accountId)
        {
            var response = new BaseResponse<List<OrderResponseModel>> ();
            try
            {
                var account = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var model = new MTAccountJsonModel()
                    {
                        Id = account.Id,
                        Login = account.Login,
                        Password = account.Password,
                        Server = account.Server,
                        UserId = account.UserID
                    };
                    var orders = await _mtDataRepository.GetOrders(model);
                    if (orders != null)
                    {
                        response.Data = new List<OrderResponseModel>();
                        foreach (var order in orders)
                        {
                            response.Data.Add(new OrderResponseModel
                            {
                                Id = order.Ticket,
                                Direction = order.Type == 2 ? Direction.Long : Direction.Short,
                                OrderTime = DateTimeOffset.FromUnixTimeSeconds(order.TimeSetup).UtcDateTime,
                                Price = order.PriceOpen,
                                Symbol = order.Symbol,
                                Volume = order.VolumeInitial
                            });
                        }
                        response.StatusCode = StatusCode.OK;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Message = "User has no orders";
                        response.Data = new List<OrderResponseModel>();
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

        public async Task<BaseResponse<List<string>>> GetSymbols(Guid accountId)
        {
            var response = new BaseResponse<List<string>>();
            try
            {
                var account = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var model = new MTAccountJsonModel()
                    {
                        Id = account.Id,
                        Login = account.Login,
                        Password = account.Password,
                        Server = account.Server,
                        UserId = account.UserID
                    };
                    var symbols = await _mtDataRepository.GetSymbols(model);
                    if (symbols != null)
                    {
                        response.Data = symbols;
                        response.StatusCode = StatusCode.OK;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Symbols not found";
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

        public async Task<BaseResponse<List<PositionResponseModel>>> GetPositions(Guid accountId)
        {
            var response = new BaseResponse<List<PositionResponseModel>>();
            try
            {
                var account = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var model = new MTAccountJsonModel()
                    {
                        Id = account.Id,
                        Login = account.Login,
                        Password = account.Password,
                        Server = account.Server,
                        UserId = account.UserID
                    };
                    var positions = await _mtDataRepository.GetPositions(model);
                    if (positions != null)
                    {
                        response.Data = new List<PositionResponseModel>();
                        foreach (var position in positions)
                        {
                            response.Data.Add(new PositionResponseModel
                            {
                                Id = position.Ticket,
                                Direction = position.Type == 0 ? Direction.Long : Direction.Short,
                                Volume = position.Volume,
                                OpenPrice = position.PriceOpen,
                                OpenTime = DateTimeOffset.FromUnixTimeSeconds(position.Time).UtcDateTime,
                                Symbol = position.Symbol,
                            });
                        }
                        response.StatusCode = StatusCode.OK;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Message = "Positions not found";
                        response.Data = new List<PositionResponseModel>();
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

        public async Task<BaseResponse<bool>> CloseOrder(ClosePositionJsonModel model)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var account = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == model.AccountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var order = await _mtDataRepository.DeleteOrder(account.Login, account.Password, account.Server, model.positionId);
                    if (order)
                    {
                        response.Data = order;
                        response.StatusCode = StatusCode.OK;
                        response.Message = "Success";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Error during closing order";
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

        private async Task<bool> DealstoAccount(Guid accountID, List<MTDealJsonModel> dealsList)
        {
            var account = new AccountData();
            try
            {
                var deposits = dealsList.Where(deal => deal.Comment.ToLower().Contains("deposit")).ToList();
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
                        accountDeal.Comission += deal.Commission;
                        accountDeal.ExitTime = DateTimeOffset.FromUnixTimeSeconds(deal.Time).UtcDateTime;
                        accountDeal.ProfitPercentage = Math.Round(deal.Profit / account.Deposit * 100, 2);
                        if (accountDeal.ProfitPercentage > 0.1) { accountDeal.Result = Result.Win; }
                        else { accountDeal.Result = Result.Loss; }
                    }
                }
                await AddDealsToDb(accountID, dbDeals, account);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<double> GetDeposit(List<MTDealJsonModel> dealsList)
        {
            double Deposit = 0;
            var deposits = dealsList.Where(deal => deal.Comment.ToLower().Contains("deposit")).ToList();
            foreach (var deposit in deposits)
            {
                Deposit += deposit.Profit;
            }
            return Deposit;
        }

        private async Task AddDealsToDb(Guid accountId, List<Deal> allDeals, AccountData account)
        {
            var dealsDB = _mtDealRepository.SelectAll();
            var deals = dealsDB.Where(x => x.AccountId == accountId).ToList();
            var newDeals = allDeals.Where(newDeal => !deals.Any(existingDeal => existingDeal.PositionId == newDeal.PositionId));
            foreach (var deal in newDeals)
            {
                deal.AccountId = accountId;
                await _mtDealRepository.Create(deal);
            }
        }

        public async Task<BaseResponse<AccountData>> GetAccountData(Guid accountId)
        {
            var response = new BaseResponse<AccountData>();
            try
            {  
               
                var account = _mtAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                if (account == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Account not found";
                    return response;
                }

                var descriptions = _descriptionRepository.SelectAll();
                var deals = _mtDealRepository.SelectAll().Where(x => x.AccountId == accountId).ToList();
                if (deals.Count() == 0)
                {
                    response.Message = "Deals not found";
                    response.StatusCode = StatusCode.ERROR;
                }
                else
                {
                    var accountData = new AccountData();
                    var dealsResponse = new List<DealResponseModel>();
                    foreach (var deal in deals)
                    {
                        dealsResponse.Add(new DealResponseModel(deal));
                    }

                    accountData.UserId = account.UserID;
                    accountData.Deposit = account.Deposit;
                    accountData.Deals = dealsResponse;
                    accountData.Profit = accountData.Deals.Sum(x => x.Profit) + account.Deals.Sum(x => x.Comission);
                    accountData.currentBalance = accountData.Deposit + accountData.Profit;
                    accountData.TotalDeals = accountData.Deals.Count;
                    accountData.WonDeals = accountData.Deals.Where(x => x.Result == Result.Win).Count();
                    accountData.LostDeals = accountData.Deals.Where(x => x.Result == Result.Loss).Count();
                    accountData.LongDeals = accountData.Deals.Where(x => x.Direction == Direction.Long).Count();
                    accountData.ShortDeals = accountData.Deals.Where(x => x.Direction == Direction.Short).Count();
                    accountData.ProfitPercentage = Math.Round(accountData.Profit / accountData.Deposit * 100, 2);
                    accountData.Winrate = accountData.WonDeals / accountData.TotalDeals * 100;
                    accountData.Lots = accountData.Deals.Select(x => x.Volume).Sum();
                    accountData.AverageLoss = accountData.Deals.Where(x => x.Result == Result.Loss).Select(x => x.Profit).Average();
                    accountData.AverageWin = accountData.Deals.Where(x => x.Result == Result.Win).Select(x => x.Profit).Average();
                    accountData.Provider = "MetaTrader 5";
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
