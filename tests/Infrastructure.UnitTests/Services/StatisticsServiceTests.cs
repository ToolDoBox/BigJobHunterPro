using BigJobHunterPro.Application.DTOs.Statistics;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Infrastructure.UnitTests.Services;

public class StatisticsServiceTests
{
    private static (StatisticsService service, ApplicationDbContext context, IMemoryCache cache) CreateService()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new StatisticsService(context, cache);

        return (service, context, cache);
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
            LongestStreak = 0
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private static async Task<Domain.Entities.Application> CreateTestApplication(
        ApplicationDbContext context,
        string userId,
        DateTime createdDate,
        int initialPoints = 1)
    {
        var application = new Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = "Test Company",
            RoleTitle = "Test Role",
            Status = ApplicationStatus.Applied,
            CreatedDate = createdDate
        };

        context.Applications.Add(application);
        await context.SaveChangesAsync();

        // Add initial "Applied" timeline event
        var timelineEvent = new TimelineEvent
        {
            Id = Guid.NewGuid(),
            ApplicationId = application.Id,
            EventType = EventType.Applied,
            Timestamp = createdDate,
            Points = initialPoints,
            CreatedDate = createdDate
        };

        context.TimelineEvents.Add(timelineEvent);
        await context.SaveChangesAsync();

        return application;
    }

    [Fact]
    public async Task GetWeeklyStats_NoApplications_ReturnsZeroStats()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        // Act
        var result = await service.GetWeeklyStatsAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.ApplicationsThisWeek.Should().Be(0);
        result.ApplicationsLastWeek.Should().Be(0);
        result.PercentageChange.Should().Be(0);
        result.PointsThisWeek.Should().Be(0);
        result.PointsLastWeek.Should().Be(0);
    }

    [Fact]
    public async Task GetWeeklyStats_ApplicationsThisWeekOnly_ReturnsCorrectStats()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);
        var now = DateTime.UtcNow;

        // Create 3 applications in the last 7 days
        await CreateTestApplication(context, user.Id, now.AddDays(-1), 1);
        await CreateTestApplication(context, user.Id, now.AddDays(-3), 1);
        await CreateTestApplication(context, user.Id, now.AddDays(-5), 1);

        // Act
        var result = await service.GetWeeklyStatsAsync(user.Id);

        // Assert
        result.ApplicationsThisWeek.Should().Be(3);
        result.ApplicationsLastWeek.Should().Be(0);
        result.PercentageChange.Should().Be(100m); // 100% when last week is 0
        result.PointsThisWeek.Should().Be(3); // 1 point per application
        result.PointsLastWeek.Should().Be(0);
    }

    [Fact]
    public async Task GetWeeklyStats_ApplicationsBothWeeks_CalculatesPercentageChange()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);
        var now = DateTime.UtcNow;

        // Create 6 applications this week (last 7 days)
        for (int i = 1; i <= 6; i++)
        {
            await CreateTestApplication(context, user.Id, now.AddDays(-i), 1);
        }

        // Create 4 applications last week (7-14 days ago)
        for (int i = 8; i <= 11; i++)
        {
            await CreateTestApplication(context, user.Id, now.AddDays(-i), 1);
        }

        // Act
        var result = await service.GetWeeklyStatsAsync(user.Id);

        // Assert
        result.ApplicationsThisWeek.Should().Be(6);
        result.ApplicationsLastWeek.Should().Be(4);
        result.PercentageChange.Should().Be(50m); // (6-4)/4 * 100 = 50%
        result.PointsThisWeek.Should().Be(6);
        result.PointsLastWeek.Should().Be(4);
    }

    [Fact]
    public async Task GetWeeklyStats_NegativePercentageChange_ReturnsCorrectValue()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);
        var now = DateTime.UtcNow;

        // Create 2 applications this week
        await CreateTestApplication(context, user.Id, now.AddDays(-1), 1);
        await CreateTestApplication(context, user.Id, now.AddDays(-3), 1);

        // Create 4 applications last week
        for (int i = 8; i <= 11; i++)
        {
            await CreateTestApplication(context, user.Id, now.AddDays(-i), 1);
        }

        // Act
        var result = await service.GetWeeklyStatsAsync(user.Id);

        // Assert
        result.ApplicationsThisWeek.Should().Be(2);
        result.ApplicationsLastWeek.Should().Be(4);
        result.PercentageChange.Should().Be(-50m); // (2-4)/4 * 100 = -50%
    }

    [Fact]
    public async Task GetWeeklyStats_IncludesPointsFromTimelineEvents()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);
        var now = DateTime.UtcNow;

        // Create application this week
        var app = await CreateTestApplication(context, user.Id, now.AddDays(-2), 1);

        // Add additional timeline events (screening + interview)
        var screeningEvent = new TimelineEvent
        {
            Id = Guid.NewGuid(),
            ApplicationId = app.Id,
            EventType = EventType.Screening,
            Timestamp = now.AddDays(-1),
            Points = 2,
            CreatedDate = now.AddDays(-1)
        };

        var interviewEvent = new TimelineEvent
        {
            Id = Guid.NewGuid(),
            ApplicationId = app.Id,
            EventType = EventType.Interview,
            Timestamp = now,
            Points = 5,
            CreatedDate = now
        };

        context.TimelineEvents.AddRange(screeningEvent, interviewEvent);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetWeeklyStatsAsync(user.Id);

        // Assert
        result.ApplicationsThisWeek.Should().Be(1);
        result.PointsThisWeek.Should().Be(8); // 1 (applied) + 2 (screening) + 5 (interview)
    }

    [Fact]
    public async Task GetWeeklyStats_ExcludesOldApplications()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);
        var now = DateTime.UtcNow;

        // Create applications within windows
        await CreateTestApplication(context, user.Id, now.AddDays(-2), 1);
        await CreateTestApplication(context, user.Id, now.AddDays(-10), 1);

        // Create applications outside windows (should be excluded)
        await CreateTestApplication(context, user.Id, now.AddDays(-15), 1);
        await CreateTestApplication(context, user.Id, now.AddDays(-30), 1);

        // Act
        var result = await service.GetWeeklyStatsAsync(user.Id);

        // Assert
        result.ApplicationsThisWeek.Should().Be(1); // Only the -2 days one
        result.ApplicationsLastWeek.Should().Be(1); // Only the -10 days one
        result.PointsThisWeek.Should().Be(1);
        result.PointsLastWeek.Should().Be(1);
    }

    [Fact]
    public async Task GetWeeklyStats_UsesCaching_ReturnsSameInstanceOnSecondCall()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);
        var now = DateTime.UtcNow;

        await CreateTestApplication(context, user.Id, now.AddDays(-1), 1);

        // Act
        var result1 = await service.GetWeeklyStatsAsync(user.Id);

        // Add another application (should not be reflected due to cache)
        await CreateTestApplication(context, user.Id, now.AddDays(-2), 1);

        var result2 = await service.GetWeeklyStatsAsync(user.Id);

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1.ApplicationsThisWeek.Should().Be(result2.ApplicationsThisWeek);
        result1.ApplicationsThisWeek.Should().Be(1); // Cached value, not 2
    }

    [Fact]
    public async Task GetWeeklyStats_OnlyIncludesUserOwnApplications()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user1 = await CreateTestUser(context);

        var user2 = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "user2@example.com",
            UserName = "user2@example.com",
            DisplayName = "User 2",
            Points = 0
        };
        context.Users.Add(user2);
        await context.SaveChangesAsync();

        var now = DateTime.UtcNow;

        // Create applications for both users
        await CreateTestApplication(context, user1.Id, now.AddDays(-1), 1);
        await CreateTestApplication(context, user2.Id, now.AddDays(-2), 1);
        await CreateTestApplication(context, user2.Id, now.AddDays(-3), 1);

        // Act
        var result = await service.GetWeeklyStatsAsync(user1.Id);

        // Assert
        result.ApplicationsThisWeek.Should().Be(1); // Only user1's application
        result.PointsThisWeek.Should().Be(1);
    }
}
