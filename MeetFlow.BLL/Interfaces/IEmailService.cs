using System.Threading.Tasks;

namespace MeetFlow.BLL.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetCodeAsync(string toEmail, string recipientName, string code);
    }
}
