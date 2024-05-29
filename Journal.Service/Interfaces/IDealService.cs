using Journal.Domain.Enums;
using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Interfaces
{
    public interface IDealService
    {

        Task<BaseResponse<DealResponseModel>> GetDeal(long positionid, Guid accountId);

        Task<BaseResponse<DealResponseModel>> AddDescriptionItem(long positionId, Guid accountId, string field, DescriptionType type);

        Task<BaseResponse<DealResponseModel>> EditDescriptionItem(Guid itemId, string field);

        Task<BaseResponse<bool>> DeleteDescriptionItem(Guid itemId);

        Task<BaseResponse<bool>> FixDeals();

        //Task<BaseResponse<DealResponseModel>> AddImage(int dealId, Guid accountId, string img);

        //Task<BaseResponse<DealResponseModel>> AddNotes(int dealId, Guid accountId, string note);
    }
}
