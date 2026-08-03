using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetFlow.BLL.DTOs.Workspace;
using MeetFlow.BLL.DTOs.WorkspaceMember;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class WorkspacesController : BaseApiController
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspacesController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        // GET /api/workspaces
        [HttpGet]
        public async Task<ActionResult<List<WorkspaceDto>>> GetMyWorkspaces()
        {
            var workspaces = await _workspaceService.GetMyWorkspacesAsync(GetCurrentUserId());
            return Ok(workspaces);
        }

        // POST /api/workspaces
        [HttpPost]
        public async Task<ActionResult<WorkspaceDto>> Create(CreateWorkspaceDto dto)
        {
            var workspace = await _workspaceService.CreateAsync(GetCurrentUserId(), dto);
            return CreatedAtAction(nameof(GetById), new { id = workspace.Id }, workspace);
        }

        // GET /api/workspaces/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<WorkspaceDto>> GetById(int id)
        {
            var workspace = await _workspaceService.GetByIdAsync(GetCurrentUserId(), id);
            return Ok(workspace);
        }

        // PUT /api/workspaces/{id}  — Owner only
        [HttpPut("{id:int}")]
        public async Task<ActionResult<WorkspaceDto>> Update(int id, UpdateWorkspaceDto dto)
        {
            var workspace = await _workspaceService.UpdateAsync(GetCurrentUserId(), id, dto);
            return Ok(workspace);
        }

        // DELETE /api/workspaces/{id}  — Owner only
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _workspaceService.DeleteAsync(GetCurrentUserId(), id);
            return NoContent();
        }

        // ---------- Members (sub-resource) ----------

        // GET /api/workspaces/{id}/members
        [HttpGet("{id:int}/members")]
        public async Task<ActionResult<List<WorkspaceMemberDto>>> GetMembers(int id)
        {
            var members = await _workspaceService.GetMembersAsync(GetCurrentUserId(), id);
            return Ok(members);
        }

        // POST /api/workspaces/{id}/members  — Owner only; invites an existing user by email as a Member
        [HttpPost("{id:int}/members")]
        public async Task<ActionResult<WorkspaceMemberDto>> AddMember(int id, AddWorkspaceMemberDto dto)
        {
            var member = await _workspaceService.AddMemberAsync(GetCurrentUserId(), id, dto);
            return Ok(member);
        }

        // PUT /api/workspaces/{id}/members/{userId}/role  — Owner only; promote to Owner or demote to Member
        [HttpPut("{id:int}/members/{userId:int}/role")]
        public async Task<IActionResult> UpdateMemberRole(int id, int userId, UpdateWorkspaceMemberRoleDto dto)
        {
            await _workspaceService.UpdateMemberRoleAsync(GetCurrentUserId(), id, userId, dto);
            return Ok(new { message = "Role updated." });
        }

        // DELETE /api/workspaces/{id}/members/{userId}  — Owner only (the Owner itself can't be removed this way)
        [HttpDelete("{id:int}/members/{userId:int}")]
        public async Task<IActionResult> RemoveMember(int id, int userId)
        {
            await _workspaceService.RemoveMemberAsync(GetCurrentUserId(), id, userId);
            return NoContent();
        }
    }
}
