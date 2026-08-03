using System;

namespace MeetFlow.BLL.DTOs.WorkspaceMember
{
    public class WorkspaceMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime JoinedAt { get; set; }
    }
}
