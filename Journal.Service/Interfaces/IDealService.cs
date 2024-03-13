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

        Task<BaseResponse<DealResponseModel>> GetDeal(int positionid, Guid accountId);

        Task<BaseResponse<DealResponseModel>> AddImage(int dealId, Guid accountId, string img);

        Task<BaseResponse<DealResponseModel>> AddNotes(int dealId, Guid accountId, string note);


    }
}
