using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using TaskEntity = MeetFlow_DAL.Entities.Task;
using MeetFlow.BLL.DTOs.AiExtraction;
using MeetFlow.BLL.DTOs.Task;
using MeetFlow.BLL.Interfaces;
using MeetFlow_DAL.Common;
using MeetFlow_DAL.Entities;
using MeetFlow_DAL.Repositories;

namespace MeetFlow.BLL.Services
{
    // Access rules:
    // - Any workspace member can view tasks and create new ones.
    // - Full edits (title/description/assignee/due date/priority) are restricted to the
    //   meeting's creator or the workspace Owner.
    // - Status updates (Todo/InProgress/Done) can ALSO be done by whoever the task is
    //   assigned to — they need to be able to mark their own work done.
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAiTaskExtractionService _aiExtractionService;
        private readonly IWhatsAppService _whatsAppService;

        public TaskService(IUnitOfWork unitOfWork, IAiTaskExtractionService aiExtractionService, IWhatsAppService whatsAppService)
        {
            _unitOfWork = unitOfWork;
            _aiExtractionService = aiExtractionService;
            _whatsAppService = whatsAppService;
        }

        public async Task<List<TaskDto>> GetForMeetingAsync(int userId, int meetingId)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var tasks = await _unitOfWork.Tasks.GetForMeetingAsync(meetingId);
            return tasks.Select(ToDto).ToList();
        }

        public async Task<List<TaskDto>> GetMyTasksAsync(int userId)
        {
            var tasks = await _unitOfWork.Tasks.GetAssignedToUserAsync(userId);
            return tasks.Select(ToDto).ToList();
        }

        public async Task<TaskDto> CreateAsync(int userId, int meetingId, CreateTaskDto dto)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var priority = string.IsNullOrWhiteSpace(dto.Priority) ? TaskPriorities.Medium : dto.Priority;
            if (!TaskPriorities.All.Contains(priority))
                throw new InvalidOperationException($"Priority must be one of: {string.Join(", ", TaskPriorities.All)}.");

            if (dto.AssignedTo is not null)
                await RequireMembershipAsync(dto.AssignedTo.Value, meeting.WorkspaceId);

            var task = new TaskEntity
            {
                MeetingId = meetingId,
                Title = dto.Title,
                Description = dto.Description,
                AssignedTo = dto.AssignedTo,
                DueDate = dto.DueDate,
                Priority = priority,
                Status = TaskStatuses.Todo,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            if (task.AssignedTo is not null)
                task.AssignedToNavigation = await _unitOfWork.Users.GetByIdAsync(task.AssignedTo.Value);

            return ToDto(task);
        }

        // This is the core "magic" flow: raw meeting notes -> AI extracts action items ->
        // each one is matched against real workspace members by name -> saved as a Task ->
        // WhatsApp notification sent to the assignee (if they have a phone number on file).
        public async Task<List<TaskDto>> ExtractTasksFromNotesAsync(int userId, int meetingId, string notesText)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var workspaceMembers = await _unitOfWork.WorkspaceMembers.GetMembersAsync(meeting.WorkspaceId);
            var memberHints = workspaceMembers
                .Select(m => new WorkspaceMemberHintDto { UserId = m.UserId, FullName = m.User.FullName })
                .ToList();

            // The AI sees the real member list and returns a matched UserId directly —
            // no local text-matching, so Arabic/English name differences and duplicate
            // first names are handled by the model instead of a naive Contains() check.
            var extractedTasks = await _aiExtractionService.ExtractTasksAsync(notesText, memberHints);
            if (extractedTasks.Count == 0)
                return new List<TaskDto>();

            var membersById = workspaceMembers.ToDictionary(m => m.UserId);
            var createdTasks = new List<TaskEntity>();

            foreach (var extracted in extractedTasks)
            {
                var matchedMember = extracted.AssigneeUserId is not null && membersById.TryGetValue(extracted.AssigneeUserId.Value, out var m)
                    ? m
                    : null;

                var priority = TaskPriorities.All.Contains(extracted.Priority) ? extracted.Priority : TaskPriorities.Medium;

                var task = new TaskEntity
                {
                    MeetingId = meetingId,
                    Title = extracted.Title,
                    AssignedTo = matchedMember?.UserId,
                    DueDate = extracted.DueDate,
                    Priority = priority,
                    Status = TaskStatuses.Todo,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Tasks.AddAsync(task);
                createdTasks.Add(task);

                if (matchedMember is not null)
                    task.AssignedToNavigation = matchedMember.User;
            }

            await _unitOfWork.SaveChangesAsync();

            // Notify assignees over WhatsApp. A failed message for one person shouldn't block the others.
            foreach (var task in createdTasks.Where(t => t.AssignedToNavigation?.PhoneNumber is not null))
            {
                try
                {
                    await _whatsAppService.SendTaskAssignedMessageAsync(
                        task.AssignedToNavigation!.PhoneNumber!, task.Title, task.DueDate);
                }
                catch
                {
                    // Swallow WhatsApp delivery failures — the task is already saved either way.
                    // A production build would log this; kept quiet here to stay dependency-free.
                }
            }

            return createdTasks.Select(ToDto).ToList();
        }

        public async Task<TaskDto> UpdateAsync(int userId, int meetingId, int taskId, UpdateTaskDto dto)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMeetingEditRightsAsync(userId, meeting);

            var task = await GetTaskOrThrowAsync(taskId, meetingId);

            if (!TaskPriorities.All.Contains(dto.Priority))
                throw new InvalidOperationException($"Priority must be one of: {string.Join(", ", TaskPriorities.All)}.");

            if (dto.AssignedTo is not null)
                await RequireMembershipAsync(dto.AssignedTo.Value, meeting.WorkspaceId);

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.AssignedTo = dto.AssignedTo;
            task.DueDate = dto.DueDate;
            task.Priority = dto.Priority;
            await _unitOfWork.SaveChangesAsync();

            return ToDto(task);
        }

        public async Task<TaskDto> UpdateStatusAsync(int userId, int meetingId, int taskId, UpdateTaskStatusDto dto)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            var membership = await RequireMembershipAsync(userId, meeting.WorkspaceId);

            var task = await GetTaskOrThrowAsync(taskId, meetingId);

            var isAssignee = task.AssignedTo == userId;
            var isCreatorOrOwner = meeting.CreatedBy == userId || membership.Role == WorkspaceRoles.Owner;

            if (!isAssignee && !isCreatorOrOwner)
                throw new UnauthorizedAccessException("Only the assignee, the meeting creator, or the workspace owner can update this task's status.");

            if (!TaskStatuses.All.Contains(dto.Status))
                throw new InvalidOperationException($"Status must be one of: {string.Join(", ", TaskStatuses.All)}.");

            task.Status = dto.Status;
            await _unitOfWork.SaveChangesAsync();

            return ToDto(task);
        }

        public async Task DeleteAsync(int userId, int meetingId, int taskId)
        {
            var meeting = await GetMeetingOrThrowAsync(meetingId);
            await RequireMeetingEditRightsAsync(userId, meeting);

            var task = await GetTaskOrThrowAsync(taskId, meetingId);
            _unitOfWork.Tasks.Remove(task);
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

            var membership = await _unitOfWork.WorkspaceMembers.GetAsync(meeting.WorkspaceId, userId);
            if (membership is null || membership.Role != WorkspaceRoles.Owner)
                throw new UnauthorizedAccessException("Only the meeting's creator or the workspace owner can do this.");
        }

        private async Task<Meeting> GetMeetingOrThrowAsync(int meetingId) =>
            await _unitOfWork.Meetings.GetByIdAsync(meetingId)
                ?? throw new KeyNotFoundException("Meeting not found.");

        private async Task<TaskEntity> GetTaskOrThrowAsync(int taskId, int expectedMeetingId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId)
                ?? throw new KeyNotFoundException("Task not found.");

            if (task.MeetingId != expectedMeetingId)
                throw new KeyNotFoundException("Task not found.");

            return task;
        }

        private static TaskDto ToDto(TaskEntity task) => new()
        {
            Id = task.Id,
            MeetingId = task.MeetingId,
            Title = task.Title,
            Description = task.Description,
            AssignedTo = task.AssignedTo,
            AssignedToName = task.AssignedToNavigation?.FullName,
            DueDate = task.DueDate,
            Priority = task.Priority,
            Status = task.Status,
            CreatedAt = task.CreatedAt
        };
    }
}
