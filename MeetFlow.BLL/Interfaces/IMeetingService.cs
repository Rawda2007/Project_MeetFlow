using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.Meeting;
using MeetFlow.BLL.DTOs.MeetingNote;

namespace MeetFlow.BLL.Interfaces
{
    public interface IMeetingService
    {
        Task<MeetingDto> CreateAsync(int userId, CreateMeetingDto dto);
        Task<List<MeetingDto>> GetForWorkspaceAsync(int userId, int workspaceId);
        Task<MeetingDto> GetByIdAsync(int userId, int meetingId);
        Task<MeetingDto> UpdateAsync(int userId, int meetingId, UpdateMeetingDto dto);
        Task DeleteAsync(int userId, int meetingId);

        Task<List<MeetingNoteDto>> GetNotesAsync(int userId, int meetingId);
        Task<MeetingNoteDto> AddNoteAsync(int userId, int meetingId, CreateMeetingNoteDto dto);
        Task<MeetingNoteDto> UpdateNoteAsync(int userId, int meetingId, int noteId, UpdateMeetingNoteDto dto);
        Task DeleteNoteAsync(int userId, int meetingId, int noteId);
    }
}
