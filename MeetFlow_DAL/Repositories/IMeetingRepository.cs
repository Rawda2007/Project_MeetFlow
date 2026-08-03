using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public interface IMeetingRepository
    {
        Task<Meeting?> GetByIdAsync(int id);
        Task<List<Meeting>> GetForWorkspaceAsync(int workspaceId);
        Task AddAsync(Meeting meeting);
        void Remove(Meeting meeting);
    }
}
