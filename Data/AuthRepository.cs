using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if(user == null)
                return null;

            if(!CheckPassword(user, password))
                return null;

            return user;
        }

        private bool CheckPassword(User user, string password)
        {
            using(var hash = new HMACSHA512(user.PasswordSalt))
            {
                var generatedHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                if(!generatedHash.SequenceEqual(user.PasswordHash))
                    return false;
            }

            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CorrectPassword(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CorrectPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hash = new HMACSHA512())
            {
                passwordSalt = hash.Key;
                passwordHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync( x => x.Username == username);
            if(user == null)
                return false;

            return true;
        }
    }
}