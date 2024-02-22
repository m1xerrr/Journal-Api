using Journal.DAL;
using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Journal.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(User entity)
        {
            await _db.Users.AddAsync(entity);
            _db.SaveChanges();
            return true;
        }

        public async Task<bool> Delete(User entity)
        {
            _db.Users.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Edit(User entity)
        {
            _db.Users.Update(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public List<User> SelectAll()
        {
            return _db.Users.ToList();
        }
    }
}
