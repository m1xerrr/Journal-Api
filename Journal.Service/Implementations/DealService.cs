using Journal.Service.Interfaces;
using Journal.DAL.Interfaces;
using Journal.Domain.Responses;
using Journal.Domain.ResponseModels;
using Journal.Domain.Models;
using Journal.Domain.Enums;

namespace Journal.Service.Implementations
{
    public class DealService : IDealService
    {
        private readonly IDealRepository _dealRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMTAccountRepository _mtcRepository;
        private readonly ICTraderAccountRepository _ctraderAccountRepository;
        private readonly IDXTradeAccountRepository _dxtradeAccountRepository;
        private readonly IDescriptionRepository _descriptionRepository;

        public DealService(IDealRepository mtDealRepository, IUserRepository userRepository, IMTAccountRepository mtcRepository, ICTraderAccountRepository ctraderAccountRepository, IDXTradeAccountRepository tradeAccountRepository, IDescriptionRepository descriptionRepository)
        {
            _dealRepository = mtDealRepository;
            _userRepository = userRepository;
            _mtcRepository = mtcRepository;
            _ctraderAccountRepository = ctraderAccountRepository;
            _dxtradeAccountRepository = tradeAccountRepository;
            _descriptionRepository = descriptionRepository;
        }

        public async Task<BaseResponse<DealResponseModel>> AddDescriptionItem(long positionId, Guid accountId, string field, DescriptionType type)
        {
            var response = new BaseResponse<DealResponseModel>();
            try
            {
                var deals = _dealRepository.SelectAll();
                var deal = deals.FirstOrDefault(x => x.PositionId == positionId && x.AccountId == accountId);
                if (deal == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Did not find a deal with such id";
                }
                else
                {
                    var description = _descriptionRepository.SelectAll().Where(x => x.DealId == deal.Id).ToList();
                    var item = new DescriptionItem()
                    {
                        Id = Guid.NewGuid(),
                        DealId = deal.Id,
                        Field = field,
                        Type = type,
                    };
                    if (description.Count == 0) item.Number = 1;
                    else item.Number = description.Select(x => x.Number).Max() + 1;
                    if (await _descriptionRepository.Create(item))
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Data = new DealResponseModel(deal);
                        response.Message = "Success";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Error";
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

        public async Task<BaseResponse<bool>> DeleteDescriptionItem(Guid itemId)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var item = _descriptionRepository.SelectAll().FirstOrDefault(x => x.Id == itemId);
                if(item == null)
                {
                    response.StatusCode=StatusCode.ERROR;
                    response.Message = "Description with such ID not found";
                }
                else { 
                if(await _descriptionRepository.Delete(item))
                {
                    response.StatusCode = StatusCode.OK;
                    response.Data = true;
                    response.Message = "Success";
                }
                else
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Error";
                }
                }

            }
            catch (Exception ex)
            {

                response.StatusCode= Domain.Enums.StatusCode.ERROR; 
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<DealResponseModel>> EditDescriptionItem(Guid itemId, string field)
        {
            var response = new BaseResponse<DealResponseModel>();

            try
            {
                var item = _descriptionRepository.SelectAll().FirstOrDefault(x => x.Id == itemId);
                if (item == null)
                {
                    response.StatusCode = StatusCode.ERROR;
                    response.Message = "Description with such ID not found";
                }
                else
                {
                    item.Field = field;
                    if (await _descriptionRepository.Edit(item))
                    {
                        response.StatusCode = StatusCode.OK;
                        response.Data = new DealResponseModel(_dealRepository.SelectAll().FirstOrDefault(x => x.Id == item.DealId));
                        response.Message = "Success";
                    }
                    else
                    {
                        response.StatusCode = StatusCode.ERROR;
                        response.Message = "Error";
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

        /*public async Task<BaseResponse<DealResponseModel>> AddImage(int dealId, Guid accountId, string img)
        {
            var response = new BaseResponse<DealResponseModel>();
            try
            {
                var deals = _dealRepository.SelectAll();
                var deal = deals.FirstOrDefault(x => x.PositionId == dealId && x.AccountId == accountId);
                if (deal == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Did not find a deal with such id";
                }
                else
                {
                    deal.Image = img;
                    if(await _dealRepository.Edit(deal))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Data = new DealResponseModel(deal);
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

        public async Task<BaseResponse<DealResponseModel>> AddNotes(int dealId, Guid accountId, string note)
        {
            var response = new BaseResponse<DealResponseModel>();
            try
            {
                var deals = _dealRepository.SelectAll();
                var deal = deals.FirstOrDefault(x => x.PositionId == dealId && x.AccountId == accountId);
                if (deal == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Did not find a deal with such id";
                }
                else
                {
                    deal.Notes = note;
                    if (await _dealRepository.Edit(deal))
                    {
                        response.StatusCode = Domain.Enums.StatusCode.OK;
                        response.Data = new DealResponseModel(deal);
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
        }*/

        public async Task<BaseResponse<DealResponseModel>> GetDeal(long positionid, Guid accountId)
        {
            var response = new BaseResponse<DealResponseModel>();
            try
            {
                var deals = _dealRepository.SelectAll();
                var deal = deals.FirstOrDefault(x => x.PositionId == positionid && x.AccountId == accountId);
                if (deal == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Did not find a deal with such id";
                }
                else
                {
                    var description = _descriptionRepository.SelectAll().Where(x => x.DealId == deal.Id).ToList();
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Data = new DealResponseModel(deal);
                    Account account = _mtcRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                    if (account == null) account = _ctraderAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                    if (account == null) account = _dxtradeAccountRepository.SelectAll().FirstOrDefault(x => x.Id == accountId);
                    response.Data.Username = _userRepository.SelectAll().FirstOrDefault(x => x.Id == account.UserID).Name;
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
