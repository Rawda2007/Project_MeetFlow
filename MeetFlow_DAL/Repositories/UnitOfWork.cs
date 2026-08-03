using System.Threading.Tasks;
using MeetFlow_DAL.Data;

namespace MeetFlow_DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeetFlowDbContext _db;

        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IPasswordResetCodeRepository PasswordResetCodes { get; }
        public IWorkspaceRepository Workspaces { get; }
        public IWorkspaceMemberRepository WorkspaceMembers { get; }
        public IMeetingRepository Meetings { get; }
        public IMeetingNoteRepository MeetingNotes { get; }
        public IDecisionRepository Decisions { get; }
        public ITaskRepository Tasks { get; }

        public UnitOfWork(MeetFlowDbContext db)
        {
            _db = db;
            Users = new UserRepository(db);
            RefreshTokens = new RefreshTokenRepository(db);
            PasswordResetCodes = new PasswordResetCodeRepository(db);
            Workspaces = new WorkspaceRepository(db);
            WorkspaceMembers = new WorkspaceMemberRepository(db);
            Meetings = new MeetingRepository(db);
            MeetingNotes = new MeetingNoteRepository(db);
            Decisions = new DecisionRepository(db);
            Tasks = new TaskRepository(db);
        }

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
