using System;

namespace MeetFlow.BLL.DTOs.Decision
{
    public class DecisionDto
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
