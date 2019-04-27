using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T element) where T : class
        {
            _context.Add(element);
        }

        public void Delete<T>(T element) where T : class
        {
            _context.Remove(element);
        }

        public Task<User> GetUser(int id)
        {
            return _context.Users.Include( x => x.Photos).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<User>> GetUsers()
        {
            return _context.Users.Include( x => x.Photos).ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}