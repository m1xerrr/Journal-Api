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
        private readonly IMTDealRepository _mtDealRepository;

        public MTDataService(IMTDataRepository mtDataRepository, IMTDealRepository mtDealRepository)
        {
            _mtDataRepository = mtDataRepository;
            _mtDealRepository = mtDealRepository;  
        }

        public async Task<BaseResponse<MTAccountData>> GetAccountData(MTAccountJsonModel account)
        {
            var response = new BaseResponse<MTAccountData>();
            try
            {
                var deals = await _mtDataRepository.GetDeals(account);
                if(deals.Count == 0)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "No deals found";
                }
                else
                {
                    response.Data = await DealstoAccount(account.Id, deals);    
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
        
        private async Task<MTAccountData> DealstoAccount(Guid accountID, List<MTDealJsonModel> dealsList)
        {
            var account = new MTAccountData();
            var deposits = dealsList.Where(deal => deal.Comment.Contains("Deposit")).ToList();
            foreach(var deposit in deposits)
            {
                account.Deposit += deposit.Profit;
                dealsList.Remove(deposit);
            }
            var dbDeals = new List<MTDeal>();
            foreach(var deal in dealsList)
            {
                if (dbDeals.FirstOrDefault(x => x.PositionId == deal.PositionId) == null)
                {
                    dbDeals.Add(new MTDeal
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
                    if (deal.Comment.Contains("tp")) { accountDeal.CloseType = CloseType.TakeProfit; }
                    else if (deal.Comment.Contains("sl")) { accountDeal.CloseType = CloseType.StopLoss; }
                    else { accountDeal.CloseType = CloseType.Market; }
                    accountDeal.ProfitPercentage = deal.Profit / account.Deposit;
                }
            }

            foreach (var deal in dbDeals)
            {
                account.Deals.Add(new MTDealResponseModel(deal));
            }
            account.Profit = account.Deals.Sum(x => x.Profit) + account.Deals.Sum(x => x.Comission);
            account.currentBalance = account.Deposit + account.Profit;
            account.TotalDeals = account.Deals.Count;
            account.TPDeals = account.Deals.Where(x => x.CloseType == CloseType.TakeProfit).Count();
            account.SLDeals = account.Deals.Where(x => x.CloseType == CloseType.StopLoss).Count();
            account.MarketDeals = account.Deals.Where(x => x.CloseType == CloseType.Market).Count();
            account.LongDeals = account.Deals.Where(x => x.Direction == Direction.Long).Count();
            account.ShortDeals = account.Deals.Where(x => x.Direction == Direction.Short).Count();
            account.ProfitPercentage = account.Profit / account.Deposit;
            await AddDealsToDb(accountID, dbDeals);
            return account;
        }

        private async Task AddDealsToDb(Guid accountId, List<MTDeal> allDeals)
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
    }
}
