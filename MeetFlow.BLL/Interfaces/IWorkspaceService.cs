using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Workspace;
using MeetFlow.BLL.DTOs.WorkspaceMember;

namespace MeetFlow.BLL.Interfaces
{
    public interface IWorkspaceService
    {
        Task<WorkspaceDto> CreateAsync(int userId, CreateWorkspaceDto dto);
        Task<List<WorkspaceDto>> GetMyWorkspacesAsync(int userId);
        Task<WorkspaceDto> GetByIdAsync(int userId, int workspaceId);
        Task<WorkspaceDto> UpdateAsync(int userId, int workspaceId, UpdateWorkspaceDto dto);
        Task DeleteAsync(int userId, int workspaceId);

        Task<List<WorkspaceMemberDto>> GetMembersAsync(int userId, int workspaceId);
        Task<WorkspaceMemberDto> AddMemberAsync(int userId, int workspaceId, AddWorkspaceMemberDto dto);
        Task UpdateMemberRoleAsync(int userId, int workspaceId, int targetUserId, UpdateWorkspaceMemberRoleDto dto);
        Task RemoveMemberAsync(int userId, int workspaceId, int targetUserId);
    }
}
