using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Workspace;
using MeetFlow.BLL.DTOs.WorkspaceMember;
using MeetFlow.BLL.Interfaces;
using MeetFlow_DAL.Common;
using MeetFlow_DAL.Entities;
using MeetFlow_DAL.Repositories;

namespace MeetFlow.BLL.Services
{
    // Simplified permission model: every workspace member is either "Owner" or "Member".
    // Owners manage the workspace and its members; Members just use it.
    public class WorkspaceService : IWorkspaceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkspaceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkspaceDto> CreateAsync(int userId, CreateWorkspaceDto dto)
        {
            var workspace = new Workspace
            {
                Name = dto.Name,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Workspaces.AddAsync(workspace);
            await _unitOfWork.SaveChangesAsync(); // generates workspace.Id

            // The creator automatically becomes the Owner.
            await _unitOfWork.WorkspaceMembers.AddAsync(new WorkspaceMember
            {
                WorkspaceId = workspace.Id,
                UserId = userId,
                Role = WorkspaceRoles.Owner,
                JoinedAt = DateTime.UtcNow
            });
            await _unitOfWork.SaveChangesAsync();

            var creator = await _unitOfWork.Users.GetByIdAsync(userId);
            return new WorkspaceDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                CreatedBy = workspace.CreatedBy,
                CreatedByName = creator?.FullName ?? string.Empty,
                CreatedAt = workspace.CreatedAt,
                MemberCount = 1,
                MyRole = WorkspaceRoles.Owner
            };
        }

        public async Task<List<WorkspaceDto>> GetMyWorkspacesAsync(int userId)
        {
            var workspaces = await _unitOfWork.Workspaces.GetForUserAsync(userId);

            return workspaces.Select(w => new WorkspaceDto
            {
                Id = w.Id,
                Name = w.Name,
                CreatedBy = w.CreatedBy,
                CreatedByName = w.CreatedByNavigation.FullName,
                CreatedAt = w.CreatedAt,
                MemberCount = w.WorkspaceMembers.Count,
                MyRole = w.WorkspaceMembers.First(m => m.UserId == userId).Role
            }).ToList();
        }

        public async Task<WorkspaceDto> GetByIdAsync(int userId, int workspaceId)
        {
            var membership = await RequireMembershipAsync(userId, workspaceId);
            var workspace = await _unitOfWork.Workspaces.GetByIdAsync(workspaceId)
                ?? throw new KeyNotFoundException("Workspace not found.");

            return new WorkspaceDto
            {
                Id = workspace.Id,
                Name = workspace.Name,
                CreatedBy = workspace.CreatedBy,
                CreatedByName = workspace.CreatedByNavigation.FullName,
                CreatedAt = workspace.CreatedAt,
                MemberCount = workspace.WorkspaceMembers.Count,
                MyRole = membership.Role
            };
        }

        public async Task<WorkspaceDto> UpdateAsync(int userId, int workspaceId, UpdateWorkspaceDto dto)
        {
            var membership = await RequireMembershipAsync(userId, workspaceId);
            RequireOwner(membership);

            var workspace = await _unitOfWork.Workspaces.GetByIdAsync(workspaceId)
                ?? throw new KeyNotFoundException("Workspace not found.");

            workspace.Name = dto.Name;
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(userId, workspaceId);
        }

        public async Task DeleteAsync(int userId, int workspaceId)
        {
            var membership = await RequireMembershipAsync(userId, workspaceId);
            RequireOwner(membership);

            var workspace = await _unitOfWork.Workspaces.GetByIdAsync(workspaceId)
                ?? throw new KeyNotFoundException("Workspace not found.");

            // Note: this will fail at the database level if the workspace still has
            // meetings pointing at it (foreign key constraint) — remove/reassign those first.
            _unitOfWork.Workspaces.Remove(workspace);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<WorkspaceMemberDto>> GetMembersAsync(int userId, int workspaceId)
        {
            await RequireMembershipAsync(userId, workspaceId);

            var members = await _unitOfWork.WorkspaceMembers.GetMembersAsync(workspaceId);
            return members.Select(ToMemberDto).ToList();
        }

        public async Task<WorkspaceMemberDto> AddMemberAsync(int userId, int workspaceId, AddWorkspaceMemberDto dto)
        {
            var membership = await RequireMembershipAsync(userId, workspaceId);
            RequireOwner(membership);

            var targetUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email)
                ?? throw new KeyNotFoundException("No user is registered with this email.");

            var existing = await _unitOfWork.WorkspaceMembers.GetAsync(workspaceId, targetUser.Id);
            if (existing is not null)
                throw new InvalidOperationException("This user is already a member of the workspace.");

            var member = new WorkspaceMember
            {
                WorkspaceId = workspaceId,
                UserId = targetUser.Id,
                Role = WorkspaceRoles.Member, // invited members always join as Member
                JoinedAt = DateTime.UtcNow
            };

            await _unitOfWork.WorkspaceMembers.AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            member.User = targetUser;
            return ToMemberDto(member);
        }

        // Lets an Owner promote a Member to Owner (co-ownership), or demote an Owner back to Member.
        public async Task UpdateMemberRoleAsync(int userId, int workspaceId, int targetUserId, UpdateWorkspaceMemberRoleDto dto)
        {
            var membership = await RequireMembershipAsync(userId, workspaceId);
            RequireOwner(membership);

            var target = await _unitOfWork.WorkspaceMembers.GetAsync(workspaceId, targetUserId)
                ?? throw new KeyNotFoundException("This user is not a member of the workspace.");

            if (dto.Role != WorkspaceRoles.Owner && dto.Role != WorkspaceRoles.Member)
                throw new InvalidOperationException("Role must be either 'Owner' or 'Member'.");

            // Don't allow demoting the last remaining Owner — the workspace would be left without one.
            if (target.Role == WorkspaceRoles.Owner && dto.Role == WorkspaceRoles.Member)
            {
                var ownerCount = await _unitOfWork.WorkspaceMembers.CountOwnersAsync(workspaceId);
                if (ownerCount <= 1)
                    throw new InvalidOperationException("Cannot remove the last remaining owner of the workspace.");
            }

            target.Role = dto.Role;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(int userId, int workspaceId, int targetUserId)
        {
            var membership = await RequireMembershipAsync(userId, workspaceId);
            RequireOwner(membership);

            var target = await _unitOfWork.WorkspaceMembers.GetAsync(workspaceId, targetUserId)
                ?? throw new KeyNotFoundException("This user is not a member of the workspace.");

            if (target.Role == WorkspaceRoles.Owner)
                throw new InvalidOperationException("The workspace owner cannot be removed. Demote them to Member first.");

            _unitOfWork.WorkspaceMembers.Remove(target);
            await _unitOfWork.SaveChangesAsync();
        }

        // ---------- helpers ----------

        private async Task<WorkspaceMember> RequireMembershipAsync(int userId, int workspaceId)
        {
            var membership = await _unitOfWork.WorkspaceMembers.GetAsync(workspaceId, userId);
            if (membership is null)
                throw new UnauthorizedAccessException("You are not a member of this workspace.");

            return membership;
        }

        private static void RequireOwner(WorkspaceMember membership)
        {
            if (membership.Role != WorkspaceRoles.Owner)
                throw new UnauthorizedAccessException("Only the workspace owner can perform this action.");
        }

        private static WorkspaceMemberDto ToMemberDto(WorkspaceMember member) => new()
        {
            Id = member.Id,
            UserId = member.UserId,
            FullName = member.User.FullName,
            Email = member.User.Email,
            Role = member.Role,
            JoinedAt = member.JoinedAt
        };
    }
}
