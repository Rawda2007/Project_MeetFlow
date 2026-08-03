using System;

namespace MeetFlow.BLL.DTOs.Task
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? AssignedTo { get; set; } // must be a member of the meeting's workspace
        public DateTime? DueDate { get; set; }
        public string? Priority { get; set; } // "Low" | "Medium" | "High" — defaults to "Medium"
    }
}
