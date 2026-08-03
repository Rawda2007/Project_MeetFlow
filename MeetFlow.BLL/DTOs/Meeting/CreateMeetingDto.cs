using System;

namespace MeetFlow.BLL.DTOs.Meeting
{
    public class CreateMeetingDto
    {
        public int WorkspaceId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime MeetingDate { get; set; }
    }
}
