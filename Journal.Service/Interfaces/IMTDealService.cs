using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Service.Interfaces
{
    public interface IMTDealService
    {

        Task<BaseResponse<MTDealResponseModel>> GetDeal(int positionid, Guid accountId);

        Task<BaseResponse<MTDealResponseModel>> AddImage(int dealId, Guid accountId, string img);

        Task<BaseResponse<MTDealResponseModel>> AddNotes(int dealId, Guid accountId, string note);
    }
}
