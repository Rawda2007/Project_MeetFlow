using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetFlow.BLL.DTOs.Task;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow_API.Controllers
{
    // Tasks are always scoped to a meeting: /api/meetings/{meetingId}/tasks
    [Route("api/meetings/{meetingId:int}/tasks")]
    [Authorize]
    public class TasksController : BaseApiController
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET /api/meetings/{meetingId}/tasks
        [HttpGet]
        public async Task<ActionResult<List<TaskDto>>> GetForMeeting(int meetingId)
        {
            var tasks = await _taskService.GetForMeetingAsync(GetCurrentUserId(), meetingId);
            return Ok(tasks);
        }

        // POST /api/meetings/{meetingId}/tasks  — any workspace member
        [HttpPost]
        public async Task<ActionResult<TaskDto>> Create(int meetingId, CreateTaskDto dto)
        {
            var task = await _taskService.CreateAsync(GetCurrentUserId(), meetingId, dto);
            return Ok(task);
        }

        // PUT /api/meetings/{meetingId}/tasks/{taskId}  — meeting creator or workspace Owner
        [HttpPut("{taskId:int}")]
        public async Task<ActionResult<TaskDto>> Update(int meetingId, int taskId, UpdateTaskDto dto)
        {
            var task = await _taskService.UpdateAsync(GetCurrentUserId(), meetingId, taskId, dto);
            return Ok(task);
        }

        // PUT /api/meetings/{meetingId}/tasks/{taskId}/status  — assignee, meeting creator, or workspace Owner
        [HttpPut("{taskId:int}/status")]
        public async Task<ActionResult<TaskDto>> UpdateStatus(int meetingId, int taskId, UpdateTaskStatusDto dto)
        {
            var task = await _taskService.UpdateStatusAsync(GetCurrentUserId(), meetingId, taskId, dto);
            return Ok(task);
        }

        // DELETE /api/meetings/{meetingId}/tasks/{taskId}  — meeting creator or workspace Owner
        [HttpDelete("{taskId:int}")]
        public async Task<IActionResult> Delete(int meetingId, int taskId)
        {
            await _taskService.DeleteAsync(GetCurrentUserId(), meetingId, taskId);
            return NoContent();
        }

        // POST /api/meetings/{meetingId}/tasks/extract-from-notes
        // The core MeetFlow flow: paste raw meeting notes -> AI pulls out action items ->
        // matched to real workspace members -> saved as Tasks -> WhatsApp sent to each assignee.
        [HttpPost("extract-from-notes")]
        public async Task<ActionResult<List<TaskDto>>> ExtractFromNotes(int meetingId, ExtractTasksRequestDto dto)
        {
            var tasks = await _taskService.ExtractTasksFromNotesAsync(GetCurrentUserId(), meetingId, dto.NotesText);
            return Ok(tasks);
        }

        // GET /api/tasks/my  — every task assigned to me, across all my workspaces
        [HttpGet("/api/tasks/my")]
        public async Task<ActionResult<List<TaskDto>>> GetMyTasks()
        {
            var tasks = await _taskService.GetMyTasksAsync(GetCurrentUserId());
            return Ok(tasks);
        }
    }
}
