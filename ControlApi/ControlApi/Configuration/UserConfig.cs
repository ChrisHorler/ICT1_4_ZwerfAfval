using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ControlApi.Entities;

namespace ControlApi.Configuration;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> e)
    {
        e.ToTable("User");
        e.HasKey(u => u.userId);
        e.Property(u => u.email).IsRequired().HasMaxLength(255);
        e.HasIndex(u => u.email).IsUnique();
    }
}
