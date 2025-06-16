using ControlApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlApi.Data;

public class ControlApiDbContext : DbContext
{
    public ControlApiDbContext(DbContextOptions<ControlApiDbContext> options) : base(options) { }
    
    public DbSet<User> users => Set<User>();
    public DbSet<Detection> detections => Set<Detection>();
    public DbSet<Prediction> predictions => Set<Prediction>();
    public DbSet<POI> POIs => Set<POI>();
    public DbSet<DetectionPOI> detectionPOIs => Set<DetectionPOI>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControlApiDbContext).Assembly);
}