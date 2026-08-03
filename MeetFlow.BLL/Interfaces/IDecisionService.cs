using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Decision;

namespace MeetFlow.BLL.Interfaces
{
    public interface IDecisionService
    {
        Task<List<DecisionDto>> GetForMeetingAsync(int userId, int meetingId);
        Task<DecisionDto> AddAsync(int userId, int meetingId, CreateDecisionDto dto);
        Task<DecisionDto> UpdateAsync(int userId, int meetingId, int decisionId, UpdateDecisionDto dto);
        Task DeleteAsync(int userId, int meetingId, int decisionId);
    }
}
