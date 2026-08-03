using System;

namespace MeetFlow.BLL.DTOs.Meeting
{
    public class MeetingDto
    {
        public int Id { get; set; }
        public int WorkspaceId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime MeetingDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int NotesCount { get; set; }
    }
}
