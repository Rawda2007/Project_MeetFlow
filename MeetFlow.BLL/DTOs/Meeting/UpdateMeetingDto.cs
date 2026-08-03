using System;

namespace MeetFlow.BLL.DTOs.Meeting
{
    public class UpdateMeetingDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime MeetingDate { get; set; }
    }
}
