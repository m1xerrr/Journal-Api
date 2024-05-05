using Journal.DAL.Interfaces;
using Journal.Domain.Models;


namespace Journal.DAL.Repositories
{
    public class DescriptionRepository : IDescriptionRepository
    {
        private readonly ApplicationDbContext _db;
        public DescriptionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(DescriptionItem entity)
        {
            await _db.DealsDescriptions.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(DescriptionItem entity)
        {
            _db.DealsDescriptions.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Edit(DescriptionItem entity)
        {
            _db.DealsDescriptions.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<DescriptionItem> SelectAll()
        {
            return _db.DealsDescriptions.ToList();
        }
    }
}
