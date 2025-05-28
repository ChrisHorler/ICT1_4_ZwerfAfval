using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ControlApi.Entities;

namespace ControlApi.Configuration;

public class PredictConfig : IEntityTypeConfiguration<Prediction>
{
    public void Configure(EntityTypeBuilder<Prediction> e)
    {
        e.ToTable("Prediction");
        e.HasKey(p => p.predictionId);
        e.HasOne(p => p.detection)
            .WithMany(d => d.predictions)
            .HasForeignKey(p => p.detectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}