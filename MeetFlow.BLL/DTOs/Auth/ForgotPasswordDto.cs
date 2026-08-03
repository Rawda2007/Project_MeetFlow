using System.ComponentModel.DataAnnotations;

namespace MeetFlow.BLL.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
    }
}
