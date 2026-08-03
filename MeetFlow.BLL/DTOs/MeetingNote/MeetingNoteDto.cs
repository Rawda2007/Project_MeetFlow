using System;

namespace MeetFlow.BLL.DTOs.MeetingNote
{
    public class MeetingNoteDto
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string Content { get; set; } = null!;
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
