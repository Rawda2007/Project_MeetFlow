using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Dashboard;

namespace MeetFlow.BLL.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync(int userId);
    }
}
