namespace MeetFlow.BLL.DTOs.Dashboard
{
    // Powers the 4 stat cards on the frontend dashboard.
    // Everything here is scoped to the CURRENT USER (their meetings, their tasks) —
    // not the whole system — so it stays correct regardless of who's logged in.
    public class DashboardSummaryDto
    {
        public int TodaysMeetingsCount { get; set; }
        public string? NextMeetingTitle { get; set; }
        public int? NextMeetingInMinutes { get; set; } // null if no upcoming meeting today

        public int TasksPendingCount { get; set; }     // status != Done
        public int TasksOverdueCount { get; set; }      // pending AND past due date

        public double CompletionProgressPercent { get; set; } // Done / total, 0 if no tasks at all

        public int ActiveWorkspacesCount { get; set; }
        public int WorkspacesAtRiskCount { get; set; }   // workspaces with at least one overdue task for this user
    }
}
