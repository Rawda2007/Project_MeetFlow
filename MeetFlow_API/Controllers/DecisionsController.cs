using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetFlow.BLL.DTOs.Decision;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow_API.Controllers
{
    // Decisions are always scoped to a meeting: /api/meetings/{meetingId}/decisions
    [Route("api/meetings/{meetingId:int}/decisions")]
    [Authorize]
    public class DecisionsController : BaseApiController
    {
        private readonly IDecisionService _decisionService;

        public DecisionsController(IDecisionService decisionService)
        {
            _decisionService = decisionService;
        }

        // GET /api/meetings/{meetingId}/decisions
        [HttpGet]
        public async Task<ActionResult<List<DecisionDto>>> GetForMeeting(int meetingId)
        {
            var decisions = await _decisionService.GetForMeetingAsync(GetCurrentUserId(), meetingId);
            return Ok(decisions);
        }

        // POST /api/meetings/{meetingId}/decisions  — any workspace member
        [HttpPost]
        public async Task<ActionResult<DecisionDto>> Add(int meetingId, CreateDecisionDto dto)
        {
            var decision = await _decisionService.AddAsync(GetCurrentUserId(), meetingId, dto);
            return Ok(decision);
        }

        // PUT /api/meetings/{meetingId}/decisions/{decisionId}  — meeting creator or workspace Owner
        [HttpPut("{decisionId:int}")]
        public async Task<ActionResult<DecisionDto>> Update(int meetingId, int decisionId, UpdateDecisionDto dto)
        {
            var decision = await _decisionService.UpdateAsync(GetCurrentUserId(), meetingId, decisionId, dto);
            return Ok(decision);
        }

        // DELETE /api/meetings/{meetingId}/decisions/{decisionId}  — meeting creator or workspace Owner
        [HttpDelete("{decisionId:int}")]
        public async Task<IActionResult> Delete(int meetingId, int decisionId)
        {
            await _decisionService.DeleteAsync(GetCurrentUserId(), meetingId, decisionId);
            return NoContent();
        }
    }
}
