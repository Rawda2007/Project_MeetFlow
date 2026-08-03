using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Configurations;

public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
{
    public void Configure(EntityTypeBuilder<Decision> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Decision__3214EC07A36AB2C4");

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

        entity.HasOne(d => d.Meeting).WithMany(p => p.Decisions)
            .HasForeignKey(d => d.MeetingId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Decisions_Meetings");
    }
}
