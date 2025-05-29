using ControlApi.ControlApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlApi.ControlApi.Data.Configuration;

public class POICategoryConfig : IEntityTypeConfiguration<POICategory>
{
    public void Configure(EntityTypeBuilder<POICategory> e)
    {
        e.ToTable("POICategory");
        e.HasKey(c => c.categoryId);
        e.Property(c => c.name).IsRequired();
    }
}