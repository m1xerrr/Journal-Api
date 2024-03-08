using Journal.DAL.Interfaces;
using Journal.Domain.Models;

namespace Journal.DAL.Repositories
{
    public class CTraderAccountRepository : ICTraderAccountRepository
    {
        private readonly ApplicationDbContext _db;
        public CTraderAccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(CTraderAccount entity)
        {
            await _db.CTraderAccounts.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(CTraderAccount entity)
        {
            _db.CTraderAccounts.Remove(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Edit(CTraderAccount entity)
        {
            _db.CTraderAccounts.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<CTraderAccount> SelectAll()
        {
            return _db.CTraderAccounts.ToList();
        }
    }
}
