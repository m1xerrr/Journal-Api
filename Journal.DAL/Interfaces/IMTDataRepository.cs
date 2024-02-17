using Journal.Domain.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Interfaces
{
    public interface IMTDataRepository
    {
        Task<List<MTDealJsonModel>> GetDeals(MTAccountJsonModel account);

        Task<bool> Initialize(MTAccountJsonModel account);
    }
}
