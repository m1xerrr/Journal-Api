using Journal.DAL.Interfaces;
using Journal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.DAL.Repositories
{
    public class NoteRepository : INoteRepository
    {
            private readonly ApplicationDbContext _db;

            public NoteRepository(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<bool> Create(Note entity)
            {
                await _db.Notes.AddAsync(entity);
                _db.SaveChanges();
                return true;
            }

            public async Task<bool> Delete(Note entity)
            {
                _db.Notes.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }

            public async Task<bool> Edit(Note entity)
            {
                _db.Notes.Update(entity);
                await _db.SaveChangesAsync();
                return true;
            }

            public List<Note> SelectAll()
            {
                return _db.Notes.ToList();
            }
    }
}
