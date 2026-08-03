using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetFlow_DAL.Entities;

namespace MeetFlow_DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07A955C17A");

        entity.HasIndex(e => e.Email, "UQ__Users__A9D10534E6D78B43").IsUnique();

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Email).HasMaxLength(150);
        entity.Property(e => e.FullName).HasMaxLength(100);
        entity.Property(e => e.PhoneNumber).HasMaxLength(20);
    }
}
