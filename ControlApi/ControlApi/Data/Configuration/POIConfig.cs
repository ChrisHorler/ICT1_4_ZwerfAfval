using ControlApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlApi.Data.Configuration;

public class POIConfig : IEntityTypeConfiguration<POI>
{
    public void Configure(EntityTypeBuilder<POI> e)
    {
        e.ToTable("POI");
        e.HasKey(p => p.POIID);
    }
}