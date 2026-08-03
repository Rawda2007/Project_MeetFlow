using System.Threading.Tasks;

namespace MeetFlow_DAL.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IPasswordResetCodeRepository PasswordResetCodes { get; }
        IWorkspaceRepository Workspaces { get; }
        IWorkspaceMemberRepository WorkspaceMembers { get; }
        IMeetingRepository Meetings { get; }
        IMeetingNoteRepository MeetingNotes { get; }
        IDecisionRepository Decisions { get; }
        ITaskRepository Tasks { get; }

        Task<int> SaveChangesAsync();
    }
}
