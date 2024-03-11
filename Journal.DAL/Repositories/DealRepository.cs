using Journal.DAL;
using Journal.DAL.Interfaces;
using Journal.Domain.Models;

namespace Journal.DAL.Repositories
{
    public class DealRepository : IDealRepository
    {
        private readonly ApplicationDbContext _db;

        public DealRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Deal entity)
        {
            await _db.Deals.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(Deal entity)
        {
            _db.Deals.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Edit(Deal entity)
        {
            _db.Deals.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<Deal> SelectAll()
        {
            return _db.Deals.ToList();
        }
    }
}
