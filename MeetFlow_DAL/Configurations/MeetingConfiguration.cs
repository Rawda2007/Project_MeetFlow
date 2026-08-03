using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Meetings__3214EC079421BB66");

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Title).HasMaxLength(200);

        entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Meetings)
            .HasForeignKey(d => d.CreatedBy)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Meetings_Users");

        entity.HasOne(d => d.Workspace).WithMany(p => p.Meetings)
            .HasForeignKey(d => d.WorkspaceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Meetings_Workspaces");
    }
}
