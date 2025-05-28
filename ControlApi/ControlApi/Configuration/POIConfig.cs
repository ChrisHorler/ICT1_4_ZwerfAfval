using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ControlApi.Entities;

namespace ControlApi.Configuration;

public class POIConfig : IEntityTypeConfiguration<POI>
{
    public void Configure(EntityTypeBuilder<POI> e)
    {
        e.ToTable("POI");
        e.HasKey(p => p.POIID);
        e.HasOne(p => p.category)
            .WithMany(c => c.POIS)
            .HasForeignKey(p => p.categoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}