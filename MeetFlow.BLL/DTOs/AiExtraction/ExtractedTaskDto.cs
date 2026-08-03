using System;

namespace MeetFlow.BLL.DTOs.AiExtraction
{
    // What the AI extracts from raw meeting-note text, already matched to a real user.
    public class ExtractedTaskDto
    {
        public string Title { get; set; } = null!;
        public int? AssigneeUserId { get; set; } // matched against the member list we sent the AI, or null if unsure
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Medium"; // "Low" | "Medium" | "High"
    }
}
