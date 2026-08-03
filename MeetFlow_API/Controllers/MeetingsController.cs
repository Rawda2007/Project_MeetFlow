using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetFlow.BLL.DTOs.Meeting;
using MeetFlow.BLL.DTOs.MeetingNote;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MeetingsController : BaseApiController
    {
        private readonly IMeetingService _meetingService;

        public MeetingsController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        // GET /api/meetings/workspace/{workspaceId}
        [HttpGet("workspace/{workspaceId:int}")]
        public async Task<ActionResult<List<MeetingDto>>> GetForWorkspace(int workspaceId)
        {
            var meetings = await _meetingService.GetForWorkspaceAsync(GetCurrentUserId(), workspaceId);
            return Ok(meetings);
        }

        // POST /api/meetings
        [HttpPost]
        public async Task<ActionResult<MeetingDto>> Create(CreateMeetingDto dto)
        {
            var meeting = await _meetingService.CreateAsync(GetCurrentUserId(), dto);
            return CreatedAtAction(nameof(GetById), new { id = meeting.Id }, meeting);
        }

        // GET /api/meetings/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MeetingDto>> GetById(int id)
        {
            var meeting = await _meetingService.GetByIdAsync(GetCurrentUserId(), id);
            return Ok(meeting);
        }

        // PUT /api/meetings/{id}  — meeting creator or workspace Owner only
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MeetingDto>> Update(int id, UpdateMeetingDto dto)
        {
            var meeting = await _meetingService.UpdateAsync(GetCurrentUserId(), id, dto);
            return Ok(meeting);
        }

        // DELETE /api/meetings/{id}  — meeting creator or workspace Owner only
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _meetingService.DeleteAsync(GetCurrentUserId(), id);
            return NoContent();
        }

        // ---------- Meeting Notes (sub-resource) ----------

        // GET /api/meetings/{id}/notes
        [HttpGet("{id:int}/notes")]
        public async Task<ActionResult<List<MeetingNoteDto>>> GetNotes(int id)
        {
            var notes = await _meetingService.GetNotesAsync(GetCurrentUserId(), id);
            return Ok(notes);
        }

        // POST /api/meetings/{id}/notes  — any workspace member
        [HttpPost("{id:int}/notes")]
        public async Task<ActionResult<MeetingNoteDto>> AddNote(int id, CreateMeetingNoteDto dto)
        {
            var note = await _meetingService.AddNoteAsync(GetCurrentUserId(), id, dto);
            return Ok(note);
        }

        // PUT /api/meetings/{id}/notes/{noteId}  — note author only
        [HttpPut("{id:int}/notes/{noteId:int}")]
        public async Task<ActionResult<MeetingNoteDto>> UpdateNote(int id, int noteId, UpdateMeetingNoteDto dto)
        {
            var note = await _meetingService.UpdateNoteAsync(GetCurrentUserId(), id, noteId, dto);
            return Ok(note);
        }

        // DELETE /api/meetings/{id}/notes/{noteId}  — note author or workspace Owner
        [HttpDelete("{id:int}/notes/{noteId:int}")]
        public async Task<IActionResult> DeleteNote(int id, int noteId)
        {
            await _meetingService.DeleteNoteAsync(GetCurrentUserId(), id, noteId);
            return NoContent();
        }
    }
}
