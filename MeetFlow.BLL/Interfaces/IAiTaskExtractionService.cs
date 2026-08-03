using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow.BLL.DTOs.AiExtraction;

namespace MeetFlow.BLL.Interfaces
{
    // Contract only — the rest of the app doesn't care whether this is Gemini, OpenAI,
    // or anything else. Swap the implementation without touching any calling code.
    public interface IAiTaskExtractionService
    {
        // workspaceMembers lets the AI match names directly to real users (including
        // Arabic-to-English name matches, nicknames, etc.) instead of us doing text
        // matching afterward — which breaks on translated names or duplicate first names.
        Task<List<ExtractedTaskDto>> ExtractTasksAsync(string meetingNotesText, List<WorkspaceMemberHintDto> workspaceMembers);
    }
}
