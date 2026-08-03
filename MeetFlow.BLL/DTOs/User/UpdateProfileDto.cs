namespace MeetFlow.BLL.DTOs.User
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = null!;

        // International format WITHOUT a leading '+' (e.g. "201234567890" for Egypt).
        // Required if this user should receive WhatsApp task notifications.
        public string? PhoneNumber { get; set; }
    }
}
