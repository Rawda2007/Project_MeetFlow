using System.ComponentModel.DataAnnotations;

namespace MeetFlow.BLL.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = null!;

        [Required, MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = null!;
    }
}
