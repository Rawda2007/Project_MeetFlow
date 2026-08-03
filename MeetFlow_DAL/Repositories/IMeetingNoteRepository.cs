using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public interface IMeetingNoteRepository
    {
        Task<MeetingNote?> GetByIdAsync(int id);
        Task<List<MeetingNote>> GetForMeetingAsync(int meetingId);
        Task AddAsync(MeetingNote note);
        void Remove(MeetingNote note);
    }
}
