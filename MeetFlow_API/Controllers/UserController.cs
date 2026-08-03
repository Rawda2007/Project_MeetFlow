using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetFlow.BLL.DTOs.User;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize] // every endpoint here requires a valid JWT access token
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /api/user/me
        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            var profile = await _userService.GetProfileAsync(GetCurrentUserId());
            return Ok(profile);
        }

        // PUT /api/user/me
        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateMyProfile(UpdateProfileDto dto)
        {
            var updated = await _userService.UpdateProfileAsync(GetCurrentUserId(), dto);
            return Ok(updated);
        }

        // PUT /api/user/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                await _userService.ChangePasswordAsync(GetCurrentUserId(), dto);
                return Ok(new { message = "Password changed successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // DELETE /api/user/me
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMyAccount()
        {
            await _userService.DeleteAccountAsync(GetCurrentUserId());
            return NoContent();
        }
    }
}
