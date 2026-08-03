using System.ComponentModel.DataAnnotations;

namespace MeetFlow.BLL.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Code { get; set; } = null!;

        [Required, MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string NewPassword { get; set; } = null!;
    }
}
