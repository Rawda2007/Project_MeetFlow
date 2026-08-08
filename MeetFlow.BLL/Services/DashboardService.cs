using System;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Dashboard;
using MeetFlow.BLL.Interfaces;
using MeetFlow_DAL.Common;
using MeetFlow_DAL.Repositories;

namespace MeetFlow.BLL.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;

            // One query for "my tasks" (already includes Meeting, needed to know each
            // task's WorkspaceId for the "at risk" calculation below).
            var myTasks = await _unitOfWork.Tasks.GetAssignedToUserAsync(userId);

            var pendingTasks = myTasks.Where(t => t.Status != TaskStatuses.Done).ToList();
            var overdueTasks = pendingTasks.Where(t => t.DueDate is not null && t.DueDate < now).ToList();

            var completionPercent = myTasks.Count == 0
                ? 0
                : Math.Round(100.0 * myTasks.Count(t => t.Status == TaskStatuses.Done) / myTasks.Count, 1);

            // One query for every meeting across every workspace this user belongs to.
            var myMeetings = await _unitOfWork.Meetings.GetForUserAsync(userId);
            var todaysMeetings = myMeetings.Where(m => m.MeetingDate.Date == today).ToList();
            var nextMeeting = todaysMeetings
                .Where(m => m.MeetingDate >= now)
                .OrderBy(m => m.MeetingDate)
                .FirstOrDefault();

            var myWorkspaces = await _unitOfWork.Workspaces.GetForUserAsync(userId);
            var atRiskWorkspaceIds = overdueTasks.Select(t => t.Meeting.WorkspaceId).Distinct().Count();

            return new DashboardSummaryDto
            {
                TodaysMeetingsCount = todaysMeetings.Count,
                NextMeetingTitle = nextMeeting?.Title,
                NextMeetingInMinutes = nextMeeting is null ? null : (int)(nextMeeting.MeetingDate - now).TotalMinutes,

                TasksPendingCount = pendingTasks.Count,
                TasksOverdueCount = overdueTasks.Count,

                CompletionProgressPercent = completionPercent,

                ActiveWorkspacesCount = myWorkspaces.Count,
                WorkspacesAtRiskCount = atRiskWorkspaceIds
            };
        }
    }
}
