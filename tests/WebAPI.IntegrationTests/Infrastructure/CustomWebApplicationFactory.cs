using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.IntegrationTests.Infrastructure;

/// <summary>
/// Custom WebApplicationFactory for integration testing.
/// Creates an in-memory test server with test database for isolated testing.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real database context
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            // Build service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to get the database context
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Ensure database is created
                db.Database.EnsureCreated();

                // Note: We DON'T seed data here - each test will manage its own data
                // This ensures test isolation
            }
        });

        // Use Test environment
        builder.UseEnvironment("Test");
    }
}
