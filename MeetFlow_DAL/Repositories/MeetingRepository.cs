using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly MeetFlowDbContext _db;

        public MeetingRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task<Meeting?> GetByIdAsync(int id) =>
            _db.Meetings
                .Include(m => m.CreatedByNavigation)
                .Include(m => m.MeetingNotes)
                .FirstOrDefaultAsync(m => m.Id == id);

        public Task<List<Meeting>> GetForWorkspaceAsync(int workspaceId) =>
            _db.Meetings
                .Include(m => m.CreatedByNavigation)
                .Include(m => m.MeetingNotes)
                .Where(m => m.WorkspaceId == workspaceId)
                .OrderByDescending(m => m.MeetingDate)
                .ToListAsync();

        public Task AddAsync(Meeting meeting)
        {
            _db.Meetings.Add(meeting);
            return Task.CompletedTask;
        }

        public void Remove(Meeting meeting)
        {
            _db.Meetings.Remove(meeting);
        }
    }
}
