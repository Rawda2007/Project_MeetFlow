namespace MeetFlow.BLL.DTOs.AiExtraction
{
    // A lightweight member reference we pass to the AI so it can match directly
    // against real workspace members instead of guessing from free text.
    public class WorkspaceMemberHintDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
    }
}
