using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data;

public static class SeedData
{
    /// <summary>
    /// Seeds the database with test data for development environment.
    /// Call this from Program.cs in Development environment only.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure database is created and migrations are applied
        await context.Database.MigrateAsync();

        // Seed test users if they don't exist
        await SeedTestUsersAsync(userManager);
    }

    private static async Task SeedTestUsersAsync(UserManager<ApplicationUser> userManager)
    {
        // Test User 1: Active Hunter
        if (await userManager.FindByEmailAsync("hunter@test.com") == null)
        {
            var testUser1 = new ApplicationUser
            {
                Email = "hunter@test.com",
                UserName = "hunter@test.com",
                DisplayName = "Test Hunter",
                Points = 150,
                TotalPoints = 150,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow.AddDays(-30)
            };

            await userManager.CreateAsync(testUser1, "Hunter123!");
        }

        // Test User 2: New Hunter
        if (await userManager.FindByEmailAsync("newbie@test.com") == null)
        {
            var testUser2 = new ApplicationUser
            {
                Email = "newbie@test.com",
                UserName = "newbie@test.com",
                DisplayName = "Newbie Hunter",
                Points = 0,
                TotalPoints = 0,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow
            };

            await userManager.CreateAsync(testUser2, "Newbie123!");
        }

        // Test User 3: Pro Hunter
        if (await userManager.FindByEmailAsync("pro@test.com") == null)
        {
            var testUser3 = new ApplicationUser
            {
                Email = "pro@test.com",
                UserName = "pro@test.com",
                DisplayName = "Pro Hunter",
                Points = 500,
                TotalPoints = 500,
                EmailConfirmed = true,
                CreatedDate = DateTime.UtcNow.AddDays(-90)
            };

            await userManager.CreateAsync(testUser3, "ProHunter123!");
        }
    }
}
