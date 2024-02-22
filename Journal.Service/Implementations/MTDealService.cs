using Journal.Service.Interfaces;
using Journal.DAL.Interfaces;
using Journal.Domain.Responses;
using Journal.Domain.ResponseModels;

namespace Journal.Service.Implementations
{
    public class MTDealService : IMTDealService
    {
        private readonly IMTDealRepository _mtDealRepository;

        public MTDealService(IMTDealRepository mtDealRepository)
        {
            _mtDealRepository = mtDealRepository;
        }

        public async Task<BaseResponse<MTDealResponseModel>> AddImage(int dealId, Guid accountId, string img)
        {
            var response = new BaseResponse<MTDealResponseModel>();
            try
            {
                var deals = _mtDealRepository.SelectAll();
                var deal = deals.FirstOrDefault(x => x.PositionId == dealId && x.AccountId == accountId);
                if (deal == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Did not find a deal with such id";
                }
                else
                {
                    deal.Image = img;
                    if(await _mtDealRepository.Edit(deal))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Data = new MTDealResponseModel(deal);
                    }
                    else
                    {
                        response.StatusCode=Domain.Enums.StatusCode.ERROR;
                        response.Message = "Error on updating deal object";
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

        public async Task<BaseResponse<MTDealResponseModel>> AddNotes(int dealId, Guid accountId, string note)
        {
            var response = new BaseResponse<MTDealResponseModel>();
            try
            {
                var deals = _mtDealRepository.SelectAll();
                var deal = deals.FirstOrDefault(x => x.PositionId == dealId && x.AccountId == accountId);
                if (deal == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Did not find a deal with such id";
                }
                else
                {
                    deal.Notes = note;
                    if (await _mtDealRepository.Edit(deal))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Data = new MTDealResponseModel(deal);
                    }
                    else
                    {
                        response.StatusCode = Domain.Enums.StatusCode.ERROR;
                        response.Message = "Error on updating deal object";
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

        public async Task<BaseResponse<MTDealResponseModel>> GetDeal(int positionid, Guid accountId)
        {
            var response = new BaseResponse<MTDealResponseModel>();
            try
            {
                var deals = _mtDealRepository.SelectAll();
                var deal = deals.FirstOrDefault(x => x.PositionId == positionid && x.AccountId == accountId);
                if (deal == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Did not find a deal with such id";
                }
                else
                {
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Data = new MTDealResponseModel(deal);
                }
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
