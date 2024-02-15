using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Journal.DAL.Repositories;
using Journal.Domain.Enums;
using Journal.Domain.Models;
using Journal.DAL.Interfaces;
using Journal.Domain.ViewModels;

namespace Journal.Service.Implementations
{
    public class MTDataService : IMTDataService
    {
        private readonly IMTDataRepository _mtDataRepository;

        public MTDataService(IMTDataRepository mtDataRepository)
        {
            _mtDataRepository = mtDataRepository;
        }

        public async Task<BaseResponse<MTAccountData>> GetAccountData(MTAccountViewModel account)
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
                    response.Data = DealstoAccount(deals);    
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
        
        private MTAccountData DealstoAccount(List<DealJson> dealsList)
        {
            var account = new MTAccountData();
            var deposits = dealsList.Where(deal => deal.Comment.Contains("Deposit")).ToList();
            foreach(var deposit in deposits)
            {
                account.Deposit += deposit.Profit;
                dealsList.Remove(deposit);
            }
            foreach(var deal in dealsList)
            {
                if (account.Deals.FirstOrDefault(x => x.PositionId == deal.PositionId) == null)
                {
                    account.Deals.Add(new Deal
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
                    var accountDeal = account.Deals.FirstOrDefault(x => x.PositionId == deal.PositionId);
                    accountDeal.ExitPrice = deal.Price;
                    accountDeal.Profit += deal.Profit;
                    accountDeal.Volume += deal.Volume;
                    accountDeal.Comission += deal.Commission;
                    accountDeal.ExitTime = DateTimeOffset.FromUnixTimeSeconds(deal.Time).UtcDateTime;
                    if (deal.Comment.Contains("tp")) { accountDeal.CloseType = CloseType.TakeProfit; }
                    else if (deal.Comment.Contains("sl")) { accountDeal.CloseType = CloseType.StopLoss; }
                    else { accountDeal.CloseType = CloseType.Market; }
                }
            }
            account.Profit = account.Deals.Sum(x => x.Profit) + account.Deals.Sum(x => x.Comission);
            account.currentBalance = account.Deposit + account.Profit;
            account.TotalDeals = account.Deals.Count;
            account.TPDeals = account.Deals.Where(x => x.CloseType == CloseType.TakeProfit).Count();
            account.SLDeals = account.Deals.Where(x => x.CloseType == CloseType.StopLoss).Count();
            account.MarketDeals = account.Deals.Where(x => x.CloseType == CloseType.Market).Count();
            account.LongDeals = account.Deals.Where(x => x.Direction == Direction.Long).Count();
            account.ShortDeals = account.Deals.Where(x => x.Direction == Direction.Short).Count();
            return account;
        }
    }
}
