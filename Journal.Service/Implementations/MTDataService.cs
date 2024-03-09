using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Journal.Domain.ResponseModels;
using Journal.Domain.Enums;
using Journal.Domain.Models;
using Journal.DAL.Interfaces;
using Journal.Domain.JsonModels;

namespace Journal.Service.Implementations
{
    public class MTDataService : IMTDataService
    {
        private readonly IMTDataRepository _mtDataRepository;
        private readonly IDealRepository _mtDealRepository;
        private readonly IMTAccountRepository _mtAccountRepository;

        public MTDataService(IMTDataRepository mtDataRepository, IDealRepository mtDealRepository, IMTAccountRepository mtAccountRepository)
        {
            _mtDataRepository = mtDataRepository;
            _mtDealRepository = mtDealRepository; 
            _mtAccountRepository = mtAccountRepository;
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
                var accountJson = new MTAccountJsonModel();
                accountJson.Password = account.Password;
                accountJson.UserId = account.UserID;
                accountJson.Id = account.Id;
                accountJson.Login = account.Login;
                accountJson.Server = account.Server;
                var deals = await _mtDataRepository.GetDeals(accountJson);
                if(deals.Count == 0)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No deals found";
                }
                else
                {
                    response.Data = await DealstoAccount(account.Id, deals);
                    response.Data.UserId = account.UserID;
                    response.StatusCode = StatusCode.OK;
                    response.Message = "Success";

                }
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }
        
        private async Task<AccountData> DealstoAccount(Guid accountID, List<MTDealJsonModel> dealsList)
        {
            var account = new AccountData();
            var deposits = dealsList.Where(deal => deal.Comment.Contains("Deposit")).ToList();
            foreach(var deposit in deposits)
            {
                account.Deposit += deposit.Profit;
                dealsList.Remove(deposit);
            }
            var dbDeals = new List<Deal>();
            foreach(var deal in dealsList)
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
                    accountDeal.ProfitPercentage = Math.Round(deal.Profit / account.Deposit * 100, 2);
                    if (accountDeal.ProfitPercentage > 0.1) { accountDeal.Result = Result.Win; }
                    else if (accountDeal.ProfitPercentage < -0.1) { accountDeal.Result = Result.Loss; }
                    else { accountDeal.Result = Result.Breakeven; }
                }
            }
            account.Deals = await AddDealsToDb(accountID, dbDeals, account);
            account.Profit = account.Deals.Sum(x => x.Profit) + account.Deals.Sum(x => x.Comission);
            account.currentBalance = account.Deposit + account.Profit;
            account.TotalDeals = account.Deals.Count;
            account.WonDeals = account.Deals.Where(x => x.Result == Result.Win).Count();
            account.LostDeals = account.Deals.Where(x => x.Result == Result.Loss).Count();
            account.BreakevenDeals = account.Deals.Where(x => x.Result == Result.Breakeven).Count();
            account.LongDeals = account.Deals.Where(x => x.Direction == Direction.Long).Count();
            account.ShortDeals = account.Deals.Where(x => x.Direction == Direction.Short).Count();
            account.ProfitPercentage = Math.Round(account.Profit / account.Deposit * 100, 2);
            return account;
        }

        private async Task<List<DealResponseModel>> AddDealsToDb(Guid accountId, List<Deal> allDeals, AccountData account)
        {
            var dealsDB = _mtDealRepository.SelectAll();
            var deals = dealsDB.Where(x => x.AccountId == accountId).ToList();
            var newDeals = allDeals.Where(newDeal => !deals.Any(existingDeal => existingDeal.PositionId == newDeal.PositionId));
            foreach (var deal in newDeals)
            {
                deal.AccountId = accountId;
                await _mtDealRepository.Create(deal);
            }
            var response = new List<DealResponseModel>();
            foreach (var deal in _mtDealRepository.SelectAll().ToList())
            {
                response.Add(new DealResponseModel(deal));
            }
            return response;
        }
    }
}
