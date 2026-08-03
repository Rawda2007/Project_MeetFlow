using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Workspac__3214EC07B08DC0DB");

        entity.HasIndex(e => new { e.WorkspaceId, e.UserId }, "UQ_WorkspaceMembers_Workspace_User").IsUnique();

        entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Role)
            .HasMaxLength(50)
            .HasDefaultValue("Member");

        entity.HasOne(d => d.User).WithMany(p => p.WorkspaceMembers)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkspaceMembers_Users");

        entity.HasOne(d => d.Workspace).WithMany(p => p.WorkspaceMembers)
            .HasForeignKey(d => d.WorkspaceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkspaceMembers_Workspaces");
    }
}
