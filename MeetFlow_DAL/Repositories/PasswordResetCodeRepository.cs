using System;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public class PasswordResetCodeRepository : IPasswordResetCodeRepository
    {
        private readonly MeetFlowDbContext _db;

        public PasswordResetCodeRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(PasswordResetCode code)
        {
            _db.PasswordResetCodes.Add(code);
            return Task.CompletedTask;
        }

        public Task<PasswordResetCode?> GetValidCodeAsync(int userId, string code) =>
            _db.PasswordResetCodes
                .Where(c => c.UserId == userId && c.Code == code && !c.IsUsed && c.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();
    }
}
