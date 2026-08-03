using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Meeting;
using MeetFlow.BLL.DTOs.MeetingNote;
using MeetFlow.BLL.Interfaces;
using MeetFlow_DAL.Common;
using MeetFlow_DAL.Entities;
using MeetFlow_DAL.Repositories;

namespace MeetFlow.BLL.Services
{
    // Access rules:
    // - Any workspace member can view/create meetings and notes.
    // - Only the meeting's creator OR the workspace Owner can update/delete that meeting.
    // - Only a note's creator can edit it; the note's creator OR the workspace Owner can delete it.
    public class MeetingService : IMeetingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MeetingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MeetingDto> CreateAsync(int userId, CreateMeetingDto dto)
        {
            await RequireMembershipAsync(userId, dto.WorkspaceId);

            var meeting = new Meeting
            {
                WorkspaceId = dto.WorkspaceId,
                Title = dto.Title,
                Description = dto.Description,
                MeetingDate = dto.MeetingDate,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Meetings.AddAsync(meeting);
            await _unitOfWork.SaveChangesAsync();

            var creator = await _unitOfWork.Users.GetByIdAsync(userId);
            return ToDto(meeting, creator?.FullName ?? string.Empty, notesCount: 0);
        }

        public async Task<List<MeetingDto>> GetForWorkspaceAsync(int userId, int workspaceId)
        {
            await RequireMembershipAsync(userId, workspaceId);

            var meetings = await _unitOfWork.Meetings.GetForWorkspaceAsync(workspaceId);
            return meetings.Select(m => ToDto(m, m.CreatedByNavigation.FullName, m.MeetingNotes.Count)).ToList();
        }

        public async Task<MeetingDto> GetByIdAsync(int userId, int meetingId)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMembershipAsync(userId, meeting.WorkspaceId);

            return ToDto(meeting, meeting.CreatedByNavigation.FullName, meeting.MeetingNotes.Count);
        }

        public async Task<MeetingDto> UpdateAsync(int userId, int meetingId, UpdateMeetingDto dto)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMeetingEditRightsAsync(userId, meeting);

            meeting.Title = dto.Title;
            meeting.Description = dto.Description;
            meeting.MeetingDate = dto.MeetingDate;
            await _unitOfWork.SaveChangesAsync();

            return ToDto(meeting, meeting.CreatedByNavigation.FullName, meeting.MeetingNotes.Count);
        }

        public async Task DeleteAsync(int userId, int meetingId)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMeetingEditRightsAsync(userId, meeting);

            // Note: this fails at the database level if the meeting still has Decisions/Tasks
            // pointing at it (foreign key constraint) — those features aren't built yet.
            _unitOfWork.Meetings.Remove(meeting);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<MeetingNoteDto>> GetNotesAsync(int userId, int meetingId)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var notes = await _unitOfWork.MeetingNotes.GetForMeetingAsync(meetingId);
            return notes.Select(ToNoteDto).ToList();
        }

        public async Task<MeetingNoteDto> AddNoteAsync(int userId, int meetingId, CreateMeetingNoteDto dto)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var note = new MeetingNote
            {
                MeetingId = meetingId,
                Content = dto.Content,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MeetingNotes.AddAsync(note);
            await _unitOfWork.SaveChangesAsync();

            var author = await _unitOfWork.Users.GetByIdAsync(userId);
            note.CreatedByNavigation = author!;
            return ToNoteDto(note);
        }

        public async Task<MeetingNoteDto> UpdateNoteAsync(int userId, int meetingId, int noteId, UpdateMeetingNoteDto dto)
        {
            var note = await GetNoteOrThrowAsync(noteId, meetingId);

            if (note.CreatedBy != userId)
                throw new UnauthorizedAccessException("Only the person who wrote this note can edit it.");

            note.Content = dto.Content;
            await _unitOfWork.SaveChangesAsync();

            return ToNoteDto(note);
        }

        public async Task DeleteNoteAsync(int userId, int meetingId, int noteId)
        {
            var note = await GetNoteOrThrowAsync(noteId, meetingId);
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            var membership = await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var isAuthor = note.CreatedBy == userId;
            var isWorkspaceOwner = membership.Role == WorkspaceRoles.Owner;

            if (!isAuthor && !isWorkspaceOwner)
                throw new UnauthorizedAccessException("Only the note's author or the workspace owner can delete it.");

            _unitOfWork.MeetingNotes.Remove(note);
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

        private async Task RequireMeetingEditRightsAsync(int userId, Meeting meeting)
        {
            if (meeting.CreatedBy == userId)
                return;

            var membership = await RequireMembershipAsync(userId, meeting.WorkspaceId);
            if (membership.Role != WorkspaceRoles.Owner)
                throw new UnauthorizedAccessException("Only the meeting's creator or the workspace owner can do this.");
        }

        private async Task<Meeting> GetMeetingOrThrowAsync(int meetingId) =>
            await _unitOfWork.Meetings.GetByIdAsync(meetingId)
                ?? throw new KeyNotFoundException("Meeting not found.");

        private async Task<MeetingNote> GetNoteOrThrowAsync(int noteId, int expectedMeetingId)
        {
            var note = await _unitOfWork.MeetingNotes.GetByIdAsync(noteId)
                ?? throw new KeyNotFoundException("Note not found.");

            if (note.MeetingId != expectedMeetingId)
                throw new KeyNotFoundException("Note not found.");

            return note;
        }

        private static MeetingDto ToDto(Meeting meeting, string createdByName, int notesCount) => new()
        {
            Id = meeting.Id,
            WorkspaceId = meeting.WorkspaceId,
            Title = meeting.Title,
            Description = meeting.Description,
            MeetingDate = meeting.MeetingDate,
            CreatedBy = meeting.CreatedBy,
            CreatedByName = createdByName,
            CreatedAt = meeting.CreatedAt,
            NotesCount = notesCount
        };

        private static MeetingNoteDto ToNoteDto(MeetingNote note) => new()
        {
            Id = note.Id,
            MeetingId = note.MeetingId,
            Content = note.Content,
            CreatedBy = note.CreatedBy,
            CreatedByName = note.CreatedByNavigation.FullName,
            CreatedAt = note.CreatedAt
        };
    }
}
