using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Decision;
using MeetFlow.BLL.Interfaces;
using MeetFlow_DAL.Common;
using MeetFlow_DAL.Entities;
using MeetFlow_DAL.Repositories;

namespace MeetFlow.BLL.Services
{
    // Access rules:
    // - Any workspace member can view/add decisions for a meeting.
    // - Decisions have no per-row author (matches the DB schema), so editing/deleting
    //   is restricted to the meeting's creator or the workspace Owner.
    public class DecisionService : IDecisionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DecisionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DecisionDto>> GetForMeetingAsync(int userId, int meetingId)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var decisions = await _unitOfWork.Decisions.GetForMeetingAsync(meetingId);
            return decisions.Select(ToDto).ToList();
        }

        public async Task<DecisionDto> AddAsync(int userId, int meetingId, CreateDecisionDto dto)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var decision = new Decision
            {
                MeetingId = meetingId,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Decisions.AddAsync(decision);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(decision);
        }

        public async Task<DecisionDto> UpdateAsync(int userId, int meetingId, int decisionId, UpdateDecisionDto dto)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMeetingEditRightsAsync(userId, meeting);

            var decision = await GetDecisionOrThrowAsync(decisionId, meetingId);
            decision.Description = dto.Description;
            await _unitOfWork.SaveChangesAsync();

            return ToDto(decision);
        }

        public async Task DeleteAsync(int userId, int meetingId, int decisionId)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMeetingEditRightsAsync(userId, meeting);

            var decision = await GetDecisionOrThrowAsync(decisionId, meetingId);
            _unitOfWork.Decisions.Remove(decision);
            await _unitOfWork.SaveChangesAsync();
        }

        // ---------- helpers ----------

        private async Task RequireMembershipAsync(int userId, int workspaceId)
        {
            var membership = await _unitOfWork.WorkspaceMembers.GetAsync(workspaceId, userId);
            if (membership is null)
                throw new UnauthorizedAccessException("You are not a member of this workspace.");
        }

        private async Task RequireMeetingEditRightsAsync(int userId, Meeting meeting)
        {
            if (meeting.CreatedBy == userId)
                return;

            var membership = await _unitOfWork.WorkspaceMembers.GetAsync(meeting.WorkspaceId, userId);
            if (membership is null || membership.Role != WorkspaceRoles.Owner)
                throw new UnauthorizedAccessException("Only the meeting's creator or the workspace owner can do this.");
        }

        private async Task<Meeting> GetMeetingOrThrowAsync(int meetingId) =>
            await _unitOfWork.Meetings.GetByIdAsync(meetingId)
                ?? throw new KeyNotFoundException("Meeting not found.");

        private async Task<Decision> GetDecisionOrThrowAsync(int decisionId, int expectedMeetingId)
        {
            var decision = await _unitOfWork.Decisions.GetByIdAsync(decisionId)
                ?? throw new KeyNotFoundException("Decision not found.");

            if (decision.MeetingId != expectedMeetingId)
                throw new KeyNotFoundException("Decision not found.");

            return decision;
        }

        private static DecisionDto ToDto(Decision decision) => new()
        {
            Id = decision.Id,
            MeetingId = decision.MeetingId,
            Description = decision.Description,
            CreatedAt = decision.CreatedAt
        };
    }
}
