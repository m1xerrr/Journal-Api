using Journal.DAL;
using Journal.DAL.Interfaces;
using Journal.Domain.Models;

namespace Journal.DAL.Repositories
{
    public class MTDealRepository : IMTDealRepository
    {
        private readonly ApplicationDbContext _db;

        public MTDealRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(MTDeal entity)
        {
            await _db.MTDeals.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(MTDeal entity)
        {
            _db.MTDeals.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Edit(MTDeal entity)
        {
            _db.MTDeals.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<MTDeal> SelectAll()
        {
            return _db.MTDeals.ToList();
        }
    }
}
