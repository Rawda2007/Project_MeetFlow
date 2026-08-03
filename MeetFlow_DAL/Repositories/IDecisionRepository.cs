using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public interface IDecisionRepository
    {
        Task<Decision?> GetByIdAsync(int id);
        Task<List<Decision>> GetForMeetingAsync(int meetingId);
        Task AddAsync(Decision decision);
        void Remove(Decision decision);
    }
}
