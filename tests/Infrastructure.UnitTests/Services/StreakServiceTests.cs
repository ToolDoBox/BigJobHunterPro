using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.UnitTests.Services;

public class StreakServiceTests
{
    private static (StreakService service, ApplicationDbContext context) CreateService()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        var unitOfWork = new UnitOfWork(context);
        var service = new StreakService(unitOfWork);

        return (service, context);
    }

    private static async Task<ApplicationUser> CreateTestUser(ApplicationDbContext context)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Test User",
            Points = 0,
            CurrentStreak = 0,
            LongestStreak = 0,
            LastActivityDate = null,
            StreakLastUpdated = null
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    [Fact]
    public async Task UpdateStreak_FirstActivity_SetsStreakToOne()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var activityTime = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = await service.UpdateStreakAsync(user.Id, activityTime);
        await context.SaveChangesAsync();

        // Assert
        result.CurrentStreak.Should().Be(1);
        result.LongestStreak.Should().Be(1);
        result.StreakIncremented.Should().BeTrue();
        result.StreakBroken.Should().BeFalse();

        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.CurrentStreak.Should().Be(1);
        updatedUser.LongestStreak.Should().Be(1);
        updatedUser.LastActivityDate.Should().Be(activityTime);
    }

    [Fact]
    public async Task UpdateStreak_SameDayActivity_DoesNotIncrementStreak()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var firstActivity = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var secondActivity = new DateTime(2024, 1, 15, 14, 0, 0, DateTimeKind.Utc); // 4 hours later

        // Act
        await service.UpdateStreakAsync(user.Id, firstActivity);
        await context.SaveChangesAsync();
        var result = await service.UpdateStreakAsync(user.Id, secondActivity);
        await context.SaveChangesAsync();

        // Assert
        result.CurrentStreak.Should().Be(1);
        result.LongestStreak.Should().Be(1);
        result.StreakIncremented.Should().BeFalse();
        result.StreakBroken.Should().BeFalse();

        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.CurrentStreak.Should().Be(1);
    }

    [Fact]
    public async Task UpdateStreak_NextDayActivity_IncrementsStreak()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var firstActivity = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var secondActivity = new DateTime(2024, 1, 16, 10, 0, 0, DateTimeKind.Utc); // Exactly 24 hours later

        // Act
        await service.UpdateStreakAsync(user.Id, firstActivity);
        await context.SaveChangesAsync();
        var result = await service.UpdateStreakAsync(user.Id, secondActivity);
        await context.SaveChangesAsync();

        // Assert - At exactly 24 hours (boundary), should increment to next day
        result.CurrentStreak.Should().Be(2);
        result.LongestStreak.Should().Be(2);
        result.StreakIncremented.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateStreak_Within48Hours_IncrementsStreak()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var firstActivity = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var secondActivity = new DateTime(2024, 1, 16, 11, 0, 0, DateTimeKind.Utc); // 25 hours later

        // Act
        await service.UpdateStreakAsync(user.Id, firstActivity);
        await context.SaveChangesAsync();
        var result = await service.UpdateStreakAsync(user.Id, secondActivity);
        await context.SaveChangesAsync();

        // Assert
        result.CurrentStreak.Should().Be(2);
        result.LongestStreak.Should().Be(2);
        result.StreakIncremented.Should().BeTrue();
        result.StreakBroken.Should().BeFalse();

        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.CurrentStreak.Should().Be(2);
        updatedUser.LongestStreak.Should().Be(2);
    }

    [Fact]
    public async Task UpdateStreak_After48Hours_ResetsStreakToOne()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var firstActivity = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var secondActivity = new DateTime(2024, 1, 18, 10, 0, 0, DateTimeKind.Utc); // 72 hours later

        // Act
        await service.UpdateStreakAsync(user.Id, firstActivity);
        await context.SaveChangesAsync();
        var result = await service.UpdateStreakAsync(user.Id, secondActivity);
        await context.SaveChangesAsync();

        // Assert
        result.CurrentStreak.Should().Be(1);
        result.LongestStreak.Should().Be(1);
        result.StreakIncremented.Should().BeFalse();
        result.StreakBroken.Should().BeTrue();

        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.CurrentStreak.Should().Be(1);
    }

    [Fact]
    public async Task UpdateStreak_NewLongestStreak_UpdatesLongestStreak()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var day1 = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var day2 = new DateTime(2024, 1, 16, 10, 30, 0, DateTimeKind.Utc);
        var day3 = new DateTime(2024, 1, 17, 11, 0, 0, DateTimeKind.Utc);

        // Act
        await service.UpdateStreakAsync(user.Id, day1); // Streak = 1
        await context.SaveChangesAsync();
        await service.UpdateStreakAsync(user.Id, day2); // Streak = 2
        await context.SaveChangesAsync();
        var result = await service.UpdateStreakAsync(user.Id, day3); // Streak = 3
        await context.SaveChangesAsync();

        // Assert
        result.CurrentStreak.Should().Be(3);
        result.LongestStreak.Should().Be(3);

        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.CurrentStreak.Should().Be(3);
        updatedUser.LongestStreak.Should().Be(3);
    }

    [Fact]
    public async Task UpdateStreak_StreakBroken_PreservesLongestStreak()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var day1 = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var day2 = new DateTime(2024, 1, 16, 10, 30, 0, DateTimeKind.Utc);
        var day3 = new DateTime(2024, 1, 17, 11, 0, 0, DateTimeKind.Utc);
        var day6 = new DateTime(2024, 1, 20, 10, 0, 0, DateTimeKind.Utc); // 72+ hours after day3

        // Act
        await service.UpdateStreakAsync(user.Id, day1); // Streak = 1, Longest = 1
        await context.SaveChangesAsync();
        await service.UpdateStreakAsync(user.Id, day2); // Streak = 2, Longest = 2
        await context.SaveChangesAsync();
        await service.UpdateStreakAsync(user.Id, day3); // Streak = 3, Longest = 3
        await context.SaveChangesAsync();
        var result = await service.UpdateStreakAsync(user.Id, day6); // Streak broken, reset to 1
        await context.SaveChangesAsync();

        // Assert
        result.CurrentStreak.Should().Be(1);
        result.LongestStreak.Should().Be(3); // Preserved from before
        result.StreakBroken.Should().BeTrue();

        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.CurrentStreak.Should().Be(1);
        updatedUser.LongestStreak.Should().Be(3); // Preserved
    }

    [Fact]
    public async Task UpdateStreak_ExactlyAt24Hours_DoesIncrement()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var firstActivity = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var secondActivity = firstActivity.AddHours(24); // Exactly 24 hours

        // Act
        await service.UpdateStreakAsync(user.Id, firstActivity);
        await context.SaveChangesAsync();
        var result = await service.UpdateStreakAsync(user.Id, secondActivity);
        await context.SaveChangesAsync();

        // Assert - 24 hours is the boundary, should increment
        result.CurrentStreak.Should().Be(2);
        result.StreakIncremented.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateStreak_ExactlyAt48Hours_IncrementsStreak()
    {
        // Arrange
        var (service, context) = CreateService();
        var user = await CreateTestUser(context);
        var firstActivity = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var secondActivity = firstActivity.AddHours(47.5); // Just under 48 hours

        // Act
        await service.UpdateStreakAsync(user.Id, firstActivity);
        await context.SaveChangesAsync();
        var result = await service.UpdateStreakAsync(user.Id, secondActivity);
        await context.SaveChangesAsync();

        // Assert - Should still increment within 48 hour grace period
        result.CurrentStreak.Should().Be(2);
        result.StreakIncremented.Should().BeTrue();
    }

    [Fact]
    public void IsStreakActive_WithinGracePeriod_ReturnsTrue()
    {
        // Arrange
        var (service, _) = CreateService();
        var lastActivity = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var currentTime = lastActivity.AddHours(40); // 40 hours later, within 48 hour grace period

        // Act
        var result = service.IsStreakActive(lastActivity, currentTime);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsStreakActive_PastGracePeriod_ReturnsFalse()
    {
        // Arrange
        var (service, _) = CreateService();
        var lastActivity = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var currentTime = lastActivity.AddHours(50); // 50 hours later, past 48 hour grace period

        // Act
        var result = service.IsStreakActive(lastActivity, currentTime);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsStreakActive_NoLastActivity_ReturnsFalse()
    {
        // Arrange
        var (service, _) = CreateService();
        var currentTime = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = service.IsStreakActive(null, currentTime);

        // Assert
        result.Should().BeFalse();
    }
}
