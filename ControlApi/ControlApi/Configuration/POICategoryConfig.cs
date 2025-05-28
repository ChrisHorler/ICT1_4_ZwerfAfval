using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ControlApi.Entities;

namespace ControlApi.Configuration;

public class POICategoryConfig : IEntityTypeConfiguration<POICategory>
{
    public void Configure(EntityTypeBuilder<POICategory> e)
    {
        e.ToTable("POICategory");
        e.HasKey(c => c.categoryId);
        e.Property(c => c.name).IsRequired();
    }
}