using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public class DecisionRepository : IDecisionRepository
    {
        private readonly MeetFlowDbContext _db;

        public DecisionRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task<Decision?> GetByIdAsync(int id) =>
            _db.Decisions.FirstOrDefaultAsync(d => d.Id == id);

        public Task<List<Decision>> GetForMeetingAsync(int meetingId) =>
            _db.Decisions
                .Where(d => d.MeetingId == meetingId)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();

        public Task AddAsync(Decision decision)
        {
            _db.Decisions.Add(decision);
            return Task.CompletedTask;
        }

        public void Remove(Decision decision)
        {
            _db.Decisions.Remove(decision);
        }
    }
}
