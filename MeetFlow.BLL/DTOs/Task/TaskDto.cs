using System;

namespace MeetFlow.BLL.DTOs.Task
{
    public class TaskDto
    {
        public int Id { get; set; }
        public int MeetingId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
