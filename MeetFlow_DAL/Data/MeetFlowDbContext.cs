using System;
using System.Collections.Generic;
using MeetFlow_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using TaskEntity = MeetFlow_DAL.Entities.Task;
namespace MeetFlow_DAL.Data;

public partial class MeetFlowDbContext : DbContext
{
    public MeetFlowDbContext(DbContextOptions<MeetFlowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Decision> Decisions { get; set; }

    public virtual DbSet<Meeting> Meetings { get; set; }

    public virtual DbSet<MeetingNote> MeetingNotes { get; set; }

    public virtual DbSet<TaskEntity> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Workspace> Workspaces { get; set; }

    public virtual DbSet<WorkspaceMember> WorkspaceMembers { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

    // Connection string now lives in appsettings.json (ConnectionStrings:DefaultConnection)
    // and is wired up via AddDbContext(...) in Program.cs — no OnConfiguring needed here.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Each table now has its own configuration class under MeetFlow_DAL/Configurations/
        // instead of being defined inline here.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeetFlowDbContext).Assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
