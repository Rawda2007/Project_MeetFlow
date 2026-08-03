using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Repositories
{
    public interface IWorkspaceRepository
    {
        Task<Workspace?> GetByIdAsync(int id);
        Task<List<Workspace>> GetForUserAsync(int userId);
        Task AddAsync(Workspace workspace);
        void Remove(Workspace workspace);
    }
}
