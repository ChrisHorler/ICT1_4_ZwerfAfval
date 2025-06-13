using ControlApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlApi.Data.Configuration;

public class DetectionConfig : IEntityTypeConfiguration<Detection>
{
    public void Configure(EntityTypeBuilder<Detection> e)
    {
        e.ToTable("Detection");
        e.HasKey(d => d.detectionId);
        e.Property(d => d.latitude).HasColumnType("float");
        e.Property(d => d.longitude).HasColumnType("float");
        e.Property(d => d.trashType)
            .HasMaxLength(100)
            .IsRequired();
    }
}