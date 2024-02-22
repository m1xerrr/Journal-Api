using Journal.DAL;
using Journal.DAL.Interfaces;
using Journal.Domain.Models;

namespace Journal.DAL.Repositories
{
    public class MTAccountRepository : IMTAccountRepository
    {
        private readonly ApplicationDbContext _db;
        public MTAccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(MTAccount entity)
        {
            await _db.MTAccounts.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(MTAccount entity)
        {
            _db.MTAccounts.Remove(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Edit(MTAccount entity)
        {
            _db.MTAccounts.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<MTAccount> SelectAll()
        {
            return _db.MTAccounts.ToList();
        }
    }
}
