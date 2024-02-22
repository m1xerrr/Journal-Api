using Journal.DAL;
using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _db;
        public SubscriptionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Subscription entity)
        {
            await _db.Subscriptions.AddAsync(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Subscription entity)
        {
            _db.Subscriptions.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Edit(Subscription entity)
        {
            _db.Subscriptions.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<Subscription> SelectAll()
        {
            return _db.Subscriptions.ToList();
        }
    }
}
