using Microsoft.EntityFrameworkCore;
using ControlApi.Entities;

namespace ControlApi;

public class ControlApiDbContext : DbContext
{
    public ControlApiDbContext(DbContextOptions<ControlApiDbContext> options) : base(options) { }
    
    public DbSet<User> users => Set<User>();
    public DbSet<TrashType> trashTypes => Set<TrashType>();
    public DbSet<Detection> detections => Set<Detection>();
    public DbSet<Prediction> predictions => Set<Prediction>();
    public DbSet<POICategory> POICategories => Set<POICategory>();
    public DbSet<POI> POIs => Set<POI>();
    public DbSet<DetectionPOI> detectionPOIs => Set<DetectionPOI>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControlApiDbContext).Assembly);
}