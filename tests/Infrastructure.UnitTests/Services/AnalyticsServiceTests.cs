using BigJobHunterPro.Application.DTOs.Analytics;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Infrastructure.UnitTests.Services;

public class AnalyticsServiceTests
{
    private static (AnalyticsService service, ApplicationDbContext context, IMemoryCache cache) CreateService()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        var unitOfWork = new UnitOfWork(context);
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new AnalyticsService(unitOfWork, cache);

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
            Points = 0
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private static async Task<Domain.Entities.Application> CreateApplicationWithEvents(
        ApplicationDbContext context,
        string userId,
        string roleTitle,
        string? jobDescription = null,
        string sourceName = "LinkedIn",
        bool hasInterview = false,
        List<string>? requiredSkills = null)
    {
        var application = new Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = "Test Company",
            RoleTitle = roleTitle,
            JobDescription = jobDescription,
            SourceName = sourceName,
            Status = ApplicationStatus.Applied,
            CreatedDate = DateTime.UtcNow,
            RequiredSkills = requiredSkills ?? new List<string>(),
            NiceToHaveSkills = new List<string>()
        };

        context.Applications.Add(application);
        await context.SaveChangesAsync();

        // Add Applied event
        var appliedEvent = new TimelineEvent
        {
            Id = Guid.NewGuid(),
            ApplicationId = application.Id,
            EventType = EventType.Applied,
            Timestamp = DateTime.UtcNow,
            Points = 1,
            CreatedDate = DateTime.UtcNow
        };

        context.TimelineEvents.Add(appliedEvent);

        if (hasInterview)
        {
            var interviewEvent = new TimelineEvent
            {
                Id = Guid.NewGuid(),
                ApplicationId = application.Id,
                EventType = EventType.Interview,
                Timestamp = DateTime.UtcNow.AddDays(7),
                Points = 5,
                CreatedDate = DateTime.UtcNow.AddDays(7)
            };

            context.TimelineEvents.Add(interviewEvent);
        }

        await context.SaveChangesAsync();

        return application;
    }

    [Fact]
    public async Task GetTopKeywords_NoApplications_ReturnsEmptyList()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        // Act
        var result = await service.GetTopKeywordsAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTopKeywords_NoSuccessfulApplications_ReturnsEmptyList()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        // Create applications without interviews
        await CreateApplicationWithEvents(context, user.Id, "Software Engineer", null, "LinkedIn", false);
        await CreateApplicationWithEvents(context, user.Id, "Backend Developer", null, "Indeed", false);

        // Act
        var result = await service.GetTopKeywordsAsync(user.Id);

        // Assert
        result.Should().BeEmpty(); // No interviews = no keywords
    }

    [Fact]
    public async Task GetTopKeywords_ExtractsFromRoleTitle()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        await CreateApplicationWithEvents(context, user.Id, "Senior Python Developer", null, "LinkedIn", true);
        await CreateApplicationWithEvents(context, user.Id, "Python Engineer", null, "Indeed", true);

        // Act
        var result = await service.GetTopKeywordsAsync(user.Id, 10);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain(k => k.Keyword == "python");
        var pythonKeyword = result.First(k => k.Keyword == "python");
        pythonKeyword.Frequency.Should().Be(2);
        pythonKeyword.Percentage.Should().Be(100m); // 100% of successful apps have "python"
    }

    [Fact]
    public async Task GetTopKeywords_ExtractsFromJobDescription()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        await CreateApplicationWithEvents(context, user.Id,
            "Software Engineer",
            "Looking for a React developer with TypeScript experience",
            "LinkedIn", true);

        // Act
        var result = await service.GetTopKeywordsAsync(user.Id, 10);

        // Assert
        result.Should().Contain(k => k.Keyword == "react");
        result.Should().Contain(k => k.Keyword == "typescript");
    }

    [Fact]
    public async Task GetTopKeywords_FiltersStopwords()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        await CreateApplicationWithEvents(context, user.Id,
            "The Senior Developer",
            "We are looking for the best developer",
            "LinkedIn", true);

        // Act
        var result = await service.GetTopKeywordsAsync(user.Id, 10);

        // Assert
        result.Should().NotContain(k => k.Keyword == "the");
        result.Should().NotContain(k => k.Keyword == "are");
        result.Should().NotContain(k => k.Keyword == "for");
    }

    [Fact]
    public async Task GetTopKeywords_IncludesSkills()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        await CreateApplicationWithEvents(context, user.Id,
            "Software Engineer",
            null,
            "LinkedIn",
            true,
            new List<string> { "Docker", "Kubernetes", "AWS" });

        // Act
        var result = await service.GetTopKeywordsAsync(user.Id, 10);

        // Assert
        result.Should().Contain(k => k.Keyword == "docker");
        result.Should().Contain(k => k.Keyword == "kubernetes");
        result.Should().Contain(k => k.Keyword == "aws");
    }

    [Fact]
    public async Task GetTopKeywords_RespectsTopCountLimit()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        // Create application with many different words
        await CreateApplicationWithEvents(context, user.Id,
            "Senior Software Engineer Backend Developer Frontend Specialist",
            "Python Java JavaScript TypeScript Ruby Go Rust Kotlin",
            "LinkedIn", true);

        // Act
        var result = await service.GetTopKeywordsAsync(user.Id, 5);

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetTopKeywords_UsesCaching()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        await CreateApplicationWithEvents(context, user.Id, "Python Developer", null, "LinkedIn", true);

        // Act
        var result1 = await service.GetTopKeywordsAsync(user.Id, 10);

        // Add another application (should not be reflected due to cache)
        await CreateApplicationWithEvents(context, user.Id, "Java Engineer", null, "Indeed", true);

        var result2 = await service.GetTopKeywordsAsync(user.Id, 10);

        // Assert
        result1.Should().BeEquivalentTo(result2); // Same due to caching
    }

    [Fact]
    public async Task GetConversionBySource_NoApplications_ReturnsEmptyList()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        // Act
        var result = await service.GetConversionBySourceAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConversionBySource_CalculatesCorrectRates()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        // LinkedIn: 2 out of 4 got interviews = 50%
        await CreateApplicationWithEvents(context, user.Id, "Dev 1", null, "LinkedIn", true);
        await CreateApplicationWithEvents(context, user.Id, "Dev 2", null, "LinkedIn", true);
        await CreateApplicationWithEvents(context, user.Id, "Dev 3", null, "LinkedIn", false);
        await CreateApplicationWithEvents(context, user.Id, "Dev 4", null, "LinkedIn", false);

        // Indeed: 1 out of 2 got interviews = 50%
        await CreateApplicationWithEvents(context, user.Id, "Dev 5", null, "Indeed", true);
        await CreateApplicationWithEvents(context, user.Id, "Dev 6", null, "Indeed", false);

        // Act
        var result = await service.GetConversionBySourceAsync(user.Id);

        // Assert
        result.Should().HaveCount(2);

        var linkedin = result.First(r => r.SourceName == "LinkedIn");
        linkedin.TotalApplications.Should().Be(4);
        linkedin.InterviewCount.Should().Be(2);
        linkedin.ConversionRate.Should().Be(50.0m);

        var indeed = result.First(r => r.SourceName == "Indeed");
        indeed.TotalApplications.Should().Be(2);
        indeed.InterviewCount.Should().Be(1);
        indeed.ConversionRate.Should().Be(50.0m);
    }

    [Fact]
    public async Task GetConversionBySource_SortsbyConversionRate()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        // LinkedIn: 1 out of 2 = 50%
        await CreateApplicationWithEvents(context, user.Id, "Dev 1", null, "LinkedIn", true);
        await CreateApplicationWithEvents(context, user.Id, "Dev 2", null, "LinkedIn", false);

        // Indeed: 2 out of 2 = 100%
        await CreateApplicationWithEvents(context, user.Id, "Dev 3", null, "Indeed", true);
        await CreateApplicationWithEvents(context, user.Id, "Dev 4", null, "Indeed", true);

        // Company Website: 0 out of 1 = 0%
        await CreateApplicationWithEvents(context, user.Id, "Dev 5", null, "Company Website", false);

        // Act
        var result = await service.GetConversionBySourceAsync(user.Id);

        // Assert
        result.Should().HaveCount(3);
        result[0].SourceName.Should().Be("Indeed"); // 100% first
        result[1].SourceName.Should().Be("LinkedIn"); // 50% second
        result[2].SourceName.Should().Be("Company Website"); // 0% last
    }

    [Fact]
    public async Task GetConversionBySource_HandlesUnknownSource()
    {
        // Arrange
        var (service, context, _) = CreateService();
        var user = await CreateTestUser(context);

        var app = new Domain.Entities.Application
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CompanyName = "Test Company",
            RoleTitle = "Developer",
            SourceName = "", // Empty source
            Status = ApplicationStatus.Applied,
            CreatedDate = DateTime.UtcNow,
            RequiredSkills = new List<string>(),
            NiceToHaveSkills = new List<string>()
        };

        context.Applications.Add(app);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetConversionBySourceAsync(user.Id);

        // Assert
        result.Should().HaveCount(1);
        result[0].SourceName.Should().Be("Unknown");
    }

    [Fact]
    public async Task GetConversionBySource_OnlyIncludesUserOwnApplications()
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

        // User 1: 1 application from LinkedIn
        await CreateApplicationWithEvents(context, user1.Id, "Dev 1", null, "LinkedIn", true);

        // User 2: 5 applications from LinkedIn
        await CreateApplicationWithEvents(context, user2.Id, "Dev 2", null, "LinkedIn", true);
        await CreateApplicationWithEvents(context, user2.Id, "Dev 3", null, "LinkedIn", false);
        await CreateApplicationWithEvents(context, user2.Id, "Dev 4", null, "LinkedIn", false);

        // Act
        var result = await service.GetConversionBySourceAsync(user1.Id);

        // Assert
        result.Should().HaveCount(1);
        var linkedin = result.First();
        linkedin.TotalApplications.Should().Be(1); // Only user1's application
    }
}
