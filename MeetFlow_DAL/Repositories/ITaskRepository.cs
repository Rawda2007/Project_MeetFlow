using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using TaskEntity = MeetFlow_DAL.Entities.Task;

namespace MeetFlow_DAL.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetByIdAsync(int id);
        Task<List<TaskEntity>> GetForMeetingAsync(int meetingId);
        Task<List<TaskEntity>> GetAssignedToUserAsync(int userId);
        Task AddAsync(TaskEntity task);
        void Remove(TaskEntity task);
    }
}
