using System;

namespace MeetFlow.BLL.DTOs.Workspace
{
    public class WorkspaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public string MyRole { get; set; } = null!; // the caller's own role in this workspace
    }
}
