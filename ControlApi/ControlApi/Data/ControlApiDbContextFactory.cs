using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ControlApi.Data;

public class ControlApiDbContextFactory : IDesignTimeDbContextFactory<ControlApiDbContext>
{
    public ControlApiDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<ControlApiDbContext>()
            .Build();
        
        var connectionString = config.GetConnectionString("Default");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string is missing in user secrets.");
            
        var optionsBuilder = new DbContextOptionsBuilder<ControlApiDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new ControlApiDbContext(optionsBuilder.Options);
    }
}