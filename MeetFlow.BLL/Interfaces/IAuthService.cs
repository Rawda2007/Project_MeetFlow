using System.Threading.Tasks;
using MeetFlow.BLL.DTOs.Auth;

namespace MeetFlow.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
        Task LogoutAsync(int userId, RefreshTokenRequestDto dto);
        Task LogoutAllAsync(int userId);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}
