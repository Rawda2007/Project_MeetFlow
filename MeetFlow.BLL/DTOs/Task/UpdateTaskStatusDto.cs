namespace MeetFlow.BLL.DTOs.Task
{
    public class UpdateTaskStatusDto
    {
        public string Status { get; set; } = null!; // "Todo" | "InProgress" | "Done"
    }
}
