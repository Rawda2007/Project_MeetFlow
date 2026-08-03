using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public interface IWorkspaceMemberRepository
    {
        Task<WorkspaceMember?> GetAsync(int workspaceId, int userId);
        Task<List<WorkspaceMember>> GetMembersAsync(int workspaceId);
        Task<int> CountOwnersAsync(int workspaceId);
        Task AddAsync(WorkspaceMember member);
        void Remove(WorkspaceMember member);
    }
}
