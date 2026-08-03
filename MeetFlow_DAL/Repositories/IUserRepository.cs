using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        void Remove(User user);
    }
}
