using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using MeetFlow_DAL.Common;
using MeetFlow_DAL.Data;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public class WorkspaceMemberRepository : IWorkspaceMemberRepository
    {
        private readonly MeetFlowDbContext _db;

        public WorkspaceMemberRepository(MeetFlowDbContext db)
        {
            _db = db;
        }

        public Task<WorkspaceMember?> GetAsync(int workspaceId, int userId) =>
            _db.WorkspaceMembers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.WorkspaceId == workspaceId && m.UserId == userId);

        public Task<List<WorkspaceMember>> GetMembersAsync(int workspaceId) =>
            _db.WorkspaceMembers
                .Include(m => m.User)
                .Where(m => m.WorkspaceId == workspaceId)
                .ToListAsync();

        public Task<int> CountOwnersAsync(int workspaceId) =>
            _db.WorkspaceMembers
                .CountAsync(m => m.WorkspaceId == workspaceId && m.Role == WorkspaceRoles.Owner);

        public Task AddAsync(WorkspaceMember member)
        {
            _db.WorkspaceMembers.Add(member);
            return Task.CompletedTask;
        }

        public void Remove(WorkspaceMember member)
        {
            _db.WorkspaceMembers.Remove(member);
        }
    }
}
