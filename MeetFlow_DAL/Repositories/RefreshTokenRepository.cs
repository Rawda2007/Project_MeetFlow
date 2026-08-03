using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly MeetFlowDbContext _db;

        public RefreshTokenRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task<RefreshToken?> GetByTokenAsync(string token) =>
            _db.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);

        public Task<List<RefreshToken>> GetActiveByUserIdAsync(int userId) =>
            _db.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

        public Task AddAsync(RefreshToken refreshToken)
        {
            _db.RefreshTokens.Add(refreshToken);
            return Task.CompletedTask;
        }
    }
}
