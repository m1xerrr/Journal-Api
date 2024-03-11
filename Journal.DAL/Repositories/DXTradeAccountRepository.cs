using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Repositories
{
    public class DXTradeAccountRepository : IDXTradeAccountRepository
    {
        private readonly ApplicationDbContext _db;
        public DXTradeAccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(DXTradeAccount entity)
        {
            await _db.DXTradeAccounts.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(DXTradeAccount entity)
        {
            _db.DXTradeAccounts.Remove(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Edit(DXTradeAccount entity)
        {
            _db.DXTradeAccounts.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<DXTradeAccount> SelectAll()
        {
            return _db.DXTradeAccounts.ToList();
        }
    }
}
