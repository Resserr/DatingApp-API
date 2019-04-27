using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        void Add<T> (T element) where T : class;
        void Delete<T> (T element) where T : class;
        Task<User> GetUser (int id);
        Task<List<User>> GetUsers();
        Task<bool> SaveAll();
        
    }
}