using ControlApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlApi.Data;

public class ControlApiDbContext : DbContext
{
    public ControlApiDbContext(DbContextOptions<ControlApiDbContext> options) : base(options) { }
    
    public DbSet<User> users => Set<User>();
    public DbSet<Detection> detections => Set<Detection>();
    public DbSet<Prediction> predictions => Set<Prediction>();
    public DbSet<POICategory> POICategories => Set<POICategory>();
    public DbSet<POI> POIs => Set<POI>();
    public DbSet<DetectionPOI> detectionPOIs => Set<DetectionPOI>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControlApiDbContext).Assembly);
        // Detection key
        modelBuilder.Entity<Detection>()
            .HasKey(d => d.detectionId);
        modelBuilder.Entity<Detection>()
            .Property(d => d.detectionId)
            .ValueGeneratedOnAdd();

        // POI key
        modelBuilder.Entity<POI>()
            .HasKey(p => p.POIID);
        modelBuilder.Entity<POI>()
            .Property(p => p.POIID)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<DetectionPOI>(b =>
        {
            // Composite primary key:
            b.HasKey(dp => new { dp.detectionId, dp.POIID });
            // FK → Detection
            b.HasOne(dp => dp.detection)
                .WithMany(d => d.detectionPOIs)
                .HasForeignKey(dp => dp.detectionId)
                .OnDelete(DeleteBehavior.Cascade);
            // FK → POI
            b.HasOne(dp => dp.POI)
                .WithMany(p => p.detectionPOIs)
                .HasForeignKey(dp => dp.POIID)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}