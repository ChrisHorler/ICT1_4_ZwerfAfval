using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ControlApi.Entities;

namespace ControlApi.Configuration;

public class DetectionPOIConfig : IEntityTypeConfiguration<DetectionPOI>
{
    public void Configure(EntityTypeBuilder<DetectionPOI> e)
    {
        e.ToTable("DetectionPOI");
        e.HasKey(dp => new { dp.detectionId, dp.POIID });

        e.HasOne(dp => dp.detection)
            .WithMany(d => d.detectionPOIs)
            .HasForeignKey(dp => dp.detectionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        e.HasOne(dp => dp.POI)
            .WithMany(p => p.detectionPOIs)
            .HasForeignKey(dp => dp.POIID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}