using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Configurations;

public class PasswordResetCodeConfiguration : IEntityTypeConfiguration<PasswordResetCode>
{
    public void Configure(EntityTypeBuilder<PasswordResetCode> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__PasswordResetCodes");

        entity.Property(e => e.Code).HasMaxLength(10);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.IsUsed).HasDefaultValue(false);

        entity.HasOne(d => d.User).WithMany(p => p.PasswordResetCodes)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_PasswordResetCodes_Users");
    }
}
