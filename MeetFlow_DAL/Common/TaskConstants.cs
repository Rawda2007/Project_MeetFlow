namespace MeetFlow_DAL.Common
{
    public static class TaskPriorities
    {
        public const string Low = "Low";
        public const string Medium = "Medium";
        public const string High = "High";

        public static readonly string[] All = { Low, Medium, High };
    }

    public static class TaskStatuses
    {
        public const string Todo = "Todo";
        public const string InProgress = "InProgress";
        public const string Done = "Done";

        public static readonly string[] All = { Todo, InProgress, Done };
    }
}
