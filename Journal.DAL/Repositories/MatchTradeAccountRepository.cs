using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Repositories
{
    public class MatchTradeAccountRepository : IMatchTradeAccountRepository
    {
        private readonly ApplicationDbContext _db;
        public MatchTradeAccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(MatchTradeAccount entity)
        {
            await _db.MatchTradeAccounts.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(MatchTradeAccount entity)
        {
            _db.MatchTradeAccounts.Remove(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Edit(MatchTradeAccount entity)
        {
            _db.MatchTradeAccounts.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<MatchTradeAccount> SelectAll()
        {
            return _db.MatchTradeAccounts.ToList();
        }
    }
}
