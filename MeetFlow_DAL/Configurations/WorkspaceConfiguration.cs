using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Workspac__3214EC0756B2F27F");

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Name).HasMaxLength(100);

        entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Workspaces)
            .HasForeignKey(d => d.CreatedBy)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Workspaces_Users");
    }
}
