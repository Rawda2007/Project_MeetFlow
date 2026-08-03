using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly MeetFlowDbContext _db;

        public WorkspaceRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task<Workspace?> GetByIdAsync(int id) =>
            _db.Workspaces
                .Include(w => w.CreatedByNavigation)
                .Include(w => w.WorkspaceMembers)
                .FirstOrDefaultAsync(w => w.Id == id);

        public Task<List<Workspace>> GetForUserAsync(int userId) =>
            _db.Workspaces
                .Include(w => w.CreatedByNavigation)
                .Include(w => w.WorkspaceMembers)
                .Where(w => w.WorkspaceMembers.Any(m => m.UserId == userId))
                .ToListAsync();

        public Task AddAsync(Workspace workspace)
        {
            _db.Workspaces.Add(workspace);
            return Task.CompletedTask;
        }

        public void Remove(Workspace workspace)
        {
            _db.Workspaces.Remove(workspace);
        }
    }
}
