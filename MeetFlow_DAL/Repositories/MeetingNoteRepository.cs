using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public class MeetingNoteRepository : IMeetingNoteRepository
    {
        private readonly MeetFlowDbContext _db;

        public MeetingNoteRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task<MeetingNote?> GetByIdAsync(int id) =>
            _db.MeetingNotes
                .Include(n => n.CreatedByNavigation)
                .FirstOrDefaultAsync(n => n.Id == id);

        public Task<List<MeetingNote>> GetForMeetingAsync(int meetingId) =>
            _db.MeetingNotes
                .Include(n => n.CreatedByNavigation)
                .Where(n => n.MeetingId == meetingId)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();

        public Task AddAsync(MeetingNote note)
        {
            _db.MeetingNotes.Add(note);
            return Task.CompletedTask;
        }

        public void Remove(MeetingNote note)
        {
            _db.MeetingNotes.Remove(note);
        }
    }
}
