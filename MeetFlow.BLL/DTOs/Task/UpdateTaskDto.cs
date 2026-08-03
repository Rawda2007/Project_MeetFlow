using System;

namespace MeetFlow.BLL.DTOs.Task
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = null!;
    }
}
