using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace MeetFlow_API.Controllers
{
    // Shared by every controller that needs to know "who is calling". Centralizing this
    // avoids copy-pasting the same claim-parsing code into every controller.
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing token.");

            return userId;
        }
    }
}
