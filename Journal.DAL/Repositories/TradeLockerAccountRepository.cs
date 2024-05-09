using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Repositories
{
    public class TradeLockerAccountRepository : ITradeLockerAccountRepository
    {
        private readonly ApplicationDbContext _db;
        public TradeLockerAccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(TradeLockerAccount entity)
        {
            await _db.TradeLockerAccounts.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(TradeLockerAccount entity)
        {
            _db.TradeLockerAccounts.Remove(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Edit(TradeLockerAccount entity)
        {
            _db.TradeLockerAccounts.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<TradeLockerAccount> SelectAll()
        {
            return _db.TradeLockerAccounts.ToList();
        }
    }
}
