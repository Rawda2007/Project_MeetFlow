using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__RefreshTokens");

        entity.HasIndex(e => e.Token, "UQ_RefreshTokens_Token").IsUnique();

        entity.Property(e => e.Token).HasMaxLength(500);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

        entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_RefreshTokens_Users");
    }
}
