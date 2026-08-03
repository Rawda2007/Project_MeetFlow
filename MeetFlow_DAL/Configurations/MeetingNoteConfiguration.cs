using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Configurations;

public class MeetingNoteConfiguration : IEntityTypeConfiguration<MeetingNote>
{
    public void Configure(EntityTypeBuilder<MeetingNote> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__MeetingN__3214EC07E1246726");

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

        entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MeetingNotes)
            .HasForeignKey(d => d.CreatedBy)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MeetingNotes_Users");

        entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingNotes)
            .HasForeignKey(d => d.MeetingId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MeetingNotes_Meetings");
    }
}
