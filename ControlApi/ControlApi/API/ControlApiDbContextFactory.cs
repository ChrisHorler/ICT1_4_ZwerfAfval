using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ControlApi.Data;

public class ControlApiDbContextFactory : IDesignTimeDbContextFactory<ControlApiDbContext>
{
    public ControlApiDbContext CreateDbContext(string[] args)
    {
        // Expect connection string as CLI argument (first argument)
        var connectionString = args.FirstOrDefault();
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string must be passed as a CLI argument.");

        var optionsBuilder = new DbContextOptionsBuilder<ControlApiDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new ControlApiDbContext(optionsBuilder.Options);
    }
}