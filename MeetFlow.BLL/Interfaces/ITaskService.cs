using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Task;

namespace MeetFlow.BLL.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetForMeetingAsync(int userId, int meetingId);
        Task<TaskDto> CreateAsync(int userId, int meetingId, CreateTaskDto dto);
        Task<TaskDto> UpdateAsync(int userId, int meetingId, int taskId, UpdateTaskDto dto);
        Task<TaskDto> UpdateStatusAsync(int userId, int meetingId, int taskId, UpdateTaskStatusDto dto);
        Task DeleteAsync(int userId, int meetingId, int taskId);
        Task<List<TaskDto>> GetMyTasksAsync(int userId);

        // The "magic" flow: raw meeting-notes text in -> AI extracts action items ->
        // matched against real workspace members -> saved as Tasks -> WhatsApp sent.
        Task<List<TaskDto>> ExtractTasksFromNotesAsync(int userId, int meetingId, string notesText);
    }
}
