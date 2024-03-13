﻿using Journal.Service.Interfaces;
using Journal.DAL.Interfaces;
using Journal.Domain.Responses;
using Journal.Domain.ResponseModels;
using Journal.Domain.Models;

namespace Journal.Service.Implementations
{
    public class DealService : IDealService
    {
        private readonly IDealRepository _dealRepository;

        public DealService(IDealRepository mtDealRepository)
        {
            _dealRepository = mtDealRepository;
        }

        public async Task<BaseResponse<DealResponseModel>> AddImage(int dealId, Guid accountId, string img)
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
        }

        public async Task<BaseResponse<DealResponseModel>> GetDeal(int positionid, Guid accountId)
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
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Data = new DealResponseModel(deal);
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
