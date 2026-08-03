using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MeetFlow.BLL.DTOs.Auth;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow_API.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting("AuthPolicy")] // brute-force protection: 10 req/min per IP on all auth endpoints
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResultDto>> Register(RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // POST /api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResultDto>> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // POST /api/auth/refresh-token
        // Anonymous by necessity — the access token has usually already expired by the time
        // this is called, so there's no valid JWT to attach. The refresh token itself is the
        // credential here, which is why it's a long random value stored server-side (see RefreshTokens table).
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResultDto>> RefreshToken(RefreshTokenRequestDto dto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // POST /api/auth/logout
        // Requires a valid access token — logout is tied to a real authenticated session,
        // and the service double-checks the refresh token actually belongs to the caller.
        // The access token itself stays valid until it naturally expires (stateless JWT);
        // that's an accepted trade-off — keep AccessTokenExpiryMinutes short in appsettings.json.
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequestDto dto)
        {
            await _authService.LogoutAsync(GetCurrentUserId(), dto);
            return Ok(new { message = "Logged out." });
        }

        // POST /api/auth/logout-all
        // Revokes every active refresh token for this user — e.g. "sign out of all devices"
        // after a suspected compromise.
        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            await _authService.LogoutAllAsync(GetCurrentUserId());
            return Ok(new { message = "Logged out from all devices." });
        }

        // POST /api/auth/forgot-password
        // Always returns 200 regardless of whether the email exists, to avoid leaking which emails are registered.
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok(new { message = "If this email is registered, a reset code has been sent." });
        }

        // POST /api/auth/reset-password
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            try
            {
                await _authService.ResetPasswordAsync(dto);
                return Ok(new { message = "Password has been reset successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
