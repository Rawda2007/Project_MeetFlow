using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MeetFlowDbContext _db;

        public UserRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task<User?> GetByEmailAsync(string email) =>
            _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        public Task<User?> GetByIdAsync(int id) =>
            _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        public Task<bool> EmailExistsAsync(string email) =>
            _db.Users.AnyAsync(u => u.Email == email);

        public Task AddAsync(User user)
        {
            _db.Users.Add(user);
            return Task.CompletedTask; // actual insert happens on SaveChangesAsync via IUnitOfWork
        }

        public void Remove(User user)
        {
            _db.Users.Remove(user);
        }
    }
}
