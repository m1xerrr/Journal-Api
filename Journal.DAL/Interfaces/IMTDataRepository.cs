using Journal.Domain.Models;
using Journal.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Interfaces
{
    public interface IMTDataRepository
    {
        Task<List<DealJson>> GetDeals(MTAccountViewModel account);

        Task<bool> Initialize(MTAccountViewModel account);
    }
}
