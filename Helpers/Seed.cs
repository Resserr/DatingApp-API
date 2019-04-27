using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Helpers
{
    public class Seed
    {
        private DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        }

        public void SeedUsers()
        {
            var jsonFileContent = File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(jsonFileContent);

            foreach (User user in users)
            {
                user.Username = user.Username.ToLower();

                byte[] passwordSalt = null, passwordHash = null;
                CorrectPassword("password", out passwordHash, out passwordSalt);
                if (passwordSalt != null && passwordHash != null)
                {
                    user.PasswordSalt = passwordSalt;
                    user.PasswordHash = passwordHash;

                    _context.Add(user);
                }
            }
            _context.SaveChanges();
        }
        private void CorrectPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hash = new HMACSHA512())
            {
                passwordSalt = hash.Key;
                passwordHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}