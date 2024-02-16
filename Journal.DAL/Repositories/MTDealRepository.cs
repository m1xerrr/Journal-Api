using Automarket.DAL;
using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IQueryable<MTDeal>> SelectAll()
        {
            return await Task.FromResult(_db.MTDeals);
        }
    }
}
