using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ControlApi.Entities;

namespace ControlApi.Configuration;

public class TrashTypeConfig : IEntityTypeConfiguration<TrashType>
{
    public void Configure(EntityTypeBuilder<TrashType> e)
    {
        e.ToTable("TrashType");
        e.HasKey(t => t.trashTypeId);
        e.Property(t => t.name).IsRequired();
    }
}