using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public interface IPasswordResetCodeRepository
    {
        Task AddAsync(PasswordResetCode code);
        Task<PasswordResetCode?> GetValidCodeAsync(int userId, string code);
    }
}
