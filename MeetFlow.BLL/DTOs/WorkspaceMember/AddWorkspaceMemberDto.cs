namespace MeetFlow.BLL.DTOs.WorkspaceMember
{
    public class AddWorkspaceMemberDto
    {
        // Invite by email — the person must already have a MeetFlow account.
        // New members always join as "Member". Only the Owner can promote someone
        // to Owner afterwards via the change-role endpoint.
        public string Email { get; set; } = null!;
    }
}
