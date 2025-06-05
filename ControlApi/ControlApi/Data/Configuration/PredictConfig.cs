using ControlApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlApi.Data.Configuration;

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