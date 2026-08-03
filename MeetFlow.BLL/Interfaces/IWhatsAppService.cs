using System;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace MeetFlow.BLL.Interfaces
{
    public interface IWhatsAppService
    {
        Task SendTaskAssignedMessageAsync(string phoneNumber, string taskTitle, DateTime? dueDate);
    }
}
