using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ControlApi.Entities;

namespace ControlApi.Configuration;

public class DetectionConfig : IEntityTypeConfiguration<Detection>
{
    public void Configure(EntityTypeBuilder<Detection> e)
    {
        e.ToTable("Detection");
        e.HasKey(d => d.detectionId);
        e.Property(d => d.latitude).HasColumnType("float");
        e.Property(d => d.longitude).HasColumnType("float");
        e.HasOne(d => d.trashType)
            .WithMany(t => t.detections)
            .HasForeignKey(d => d.trashTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}