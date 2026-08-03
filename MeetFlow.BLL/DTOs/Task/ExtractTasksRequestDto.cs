namespace MeetFlow.BLL.DTOs.Task
{
    public class ExtractTasksRequestDto
    {
        // Raw meeting notes text — the same thing you'd type into the Notes box.
        // You can also pass an existing note's Content here after fetching it.
        public string NotesText { get; set; } = null!;
    }
}
