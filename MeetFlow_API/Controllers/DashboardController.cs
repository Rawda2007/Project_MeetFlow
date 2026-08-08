using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetFlow.BLL.DTOs.Dashboard;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET /api/dashboard/summary
        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var summary = await _dashboardService.GetSummaryAsync(GetCurrentUserId());
            return Ok(summary);
        }
    }
}
