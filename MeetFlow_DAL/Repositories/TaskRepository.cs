using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using TaskEntity = MeetFlow_DAL.Entities.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Data;

namespace MeetFlow_DAL.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly MeetFlowDbContext _db;

        public TaskRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task<TaskEntity?> GetByIdAsync(int id) =>
            _db.Tasks
                .Include(t => t.AssignedToNavigation)
                .FirstOrDefaultAsync(t => t.Id == id);

        public Task<List<TaskEntity>> GetForMeetingAsync(int meetingId) =>
            _db.Tasks
                .Include(t => t.AssignedToNavigation)
                .Where(t => t.MeetingId == meetingId)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

        public Task<List<TaskEntity>> GetAssignedToUserAsync(int userId) =>
            _db.Tasks
                .Include(t => t.AssignedToNavigation)
                .Include(t => t.Meeting)
                .Where(t => t.AssignedTo == userId)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

        public Task AddAsync(TaskEntity task)
        {
            _db.Tasks.Add(task);
            return Task.CompletedTask;
        }

        public void Remove(TaskEntity task)
        {
            _db.Tasks.Remove(task);
        }
    }
}
