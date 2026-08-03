using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskEntity = MeetFlow_DAL.Entities.Task;

namespace MeetFlow_DAL.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Tasks__3214EC07528DAAC3");

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Priority)
            .HasMaxLength(50)
            .HasDefaultValue("Medium");
        entity.Property(e => e.Status)
            .HasMaxLength(50)
            .HasDefaultValue("Todo");
        entity.Property(e => e.Title).HasMaxLength(200);

        entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Tasks)
            .HasForeignKey(d => d.AssignedTo)
            .HasConstraintName("FK_Tasks_Users");

        entity.HasOne(d => d.Meeting).WithMany(p => p.Tasks)
            .HasForeignKey(d => d.MeetingId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Tasks_Meetings");
    }
}
