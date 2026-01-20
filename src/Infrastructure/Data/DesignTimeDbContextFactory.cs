using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data;

/// <summary>
/// Design-time factory for creating ApplicationDbContext.
/// This ensures EF Core migrations always use PostgreSQL, regardless of environment settings.
/// Used by: dotnet ef migrations add/remove commands
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Always use PostgreSQL for migrations with a dummy connection string.
        // The actual connection is not needed for schema generation - only the provider matters.
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
            ?? "Host=localhost;Database=bigjobhunterpro;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory");
        });

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
