using Journal.Domain.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Interfaces
{
    public interface IDXTradeDataRepository
    {
        Task<DXTradeAccountAPIUserJsonModel> GetAccounts(string username, string password, string domain);

        Task<DXTradeAPIDealJsonModel> GetDeals(string username, string password, string domain, string account);

        Task<double> GetDeposit(string username, string password, string domain, string account);
    }
}
