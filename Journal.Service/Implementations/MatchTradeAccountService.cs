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

        public MatchTradeAccountService(IMatchTradeApiRepository matchTradeApiRepository, IMatchTradeAccountRepository matchTradeAccountRepository, ITradeLockerAPIRepository tradeLockerApiRepository)
        {
            _matchTradeApiRepository = matchTradeApiRepository;
            _matchTradeAccountRepository = matchTradeAccountRepository;
            _tradeLockerApiRepository = tradeLockerApiRepository;
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
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<List<PositionResponseModel>>> GetPositions(Guid id)
        {
            var response = new BaseResponse<List<PositionResponseModel>>();
            try
            {
                var account = _matchTradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if(account == null)
                {
                    response.StatusCode=StatusCode.ERROR;
                    response.Message = "Account not found";
                }
                else
                {
                    var positions = await _matchTradeApiRepository.GetPositions(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login);
                    response.Data = new List<PositionResponseModel>();
                    foreach(var position in positions.Positions)
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
                    response.StatusCode =StatusCode.OK;
                    response.Message = "Success";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode
                    .ERROR;
                response.Message= ex.Message;
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
                var deals = await _matchTradeApiRepository.GetDeals(account.LiveStatus, account.Email, account.Password, account.BrokerId, account.Login, sessionData.TradingApiToken, sessionData.CoAuthToken);
            }
            catch(Exception ex)
            {
                response .Message = ex.Message;
                response.StatusCode = StatusCode.ERROR;
            }
            return response;
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
