using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Application.DTOs.Applications;
using BigJobHunterPro.Application.DTOs.Statistics;
using Domain.Enums;
using FluentAssertions;
using WebAPI.IntegrationTests.Helpers;
using WebAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace WebAPI.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for StatisticsController endpoints.
/// Tests use in-memory database and WebApplicationFactory for isolated testing.
/// </summary>
public class StatisticsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public StatisticsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginUserAsync()
    {
        var registerRequest = new RegisterRequest
        {
            Email = TestDataHelper.GenerateUniqueEmail(),
            Password = "Test123!",
            DisplayName = "Test Hunter"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();

        return registerResult!.Token;
    }

    private async Task CreateApplicationAsync(string token, DateTime? createdDate = null)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job/test",
            RawPageContent = "Test Company is hiring for Software Engineer position. This is a test job posting content."
        };

        var response = await _client.PostAsJsonAsync("/api/applications", request);
        response.EnsureSuccessStatusCode();

        // If a specific created date is needed, we would need to manipulate it at the database level
        // For now, we'll use the current timestamp
    }

    [Fact]
    public async Task GetWeeklyStats_WithoutAuthentication_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/statistics/weekly");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetWeeklyStats_WithNoApplications_ReturnsZeroStats()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/statistics/weekly");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<WeeklyStatsResponse>();
        result.Should().NotBeNull();
        result!.ApplicationsThisWeek.Should().Be(0);
        result.ApplicationsLastWeek.Should().Be(0);
        result.PercentageChange.Should().Be(0);
        result.PointsThisWeek.Should().Be(0);
        result.PointsLastWeek.Should().Be(0);
    }

    [Fact]
    public async Task GetWeeklyStats_WithApplications_ReturnsCorrectCounts()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create 3 applications
        await CreateApplicationAsync(token);
        await CreateApplicationAsync(token);
        await CreateApplicationAsync(token);

        // Act
        var response = await _client.GetAsync("/api/statistics/weekly");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<WeeklyStatsResponse>();
        result.Should().NotBeNull();
        result!.ApplicationsThisWeek.Should().Be(3);
        result.PointsThisWeek.Should().Be(3); // 1 point per "Applied" event
    }

    [Fact]
    public async Task GetWeeklyStats_WithApplicationsThisWeekOnly_ShowsPercentageIncrease()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create applications
        await CreateApplicationAsync(token);
        await CreateApplicationAsync(token);

        // Act
        var response = await _client.GetAsync("/api/statistics/weekly");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<WeeklyStatsResponse>();
        result.Should().NotBeNull();
        result!.ApplicationsThisWeek.Should().Be(2);
        result.ApplicationsLastWeek.Should().Be(0);
        result.PercentageChange.Should().Be(100m); // 100% when coming from 0
    }

    [Fact]
    public async Task GetWeeklyStats_OnlyIncludesAuthenticatedUserData()
    {
        // Arrange - Create first user with applications
        var token1 = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);
        await CreateApplicationAsync(token1);
        await CreateApplicationAsync(token1);

        // Create second user with applications
        var token2 = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);
        await CreateApplicationAsync(token2);

        // Act - Get stats for second user
        var response = await _client.GetAsync("/api/statistics/weekly");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<WeeklyStatsResponse>();
        result.Should().NotBeNull();
        result!.ApplicationsThisWeek.Should().Be(1); // Only second user's application
        result.PointsThisWeek.Should().Be(1); // Only second user's points
    }

    [Fact]
    public async Task GetWeeklyStats_MultipleCallsUseCaching_ReturnsSameResults()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await CreateApplicationAsync(token);

        // Act - Make first call
        var response1 = await _client.GetAsync("/api/statistics/weekly");
        var result1 = await response1.Content.ReadFromJsonAsync<WeeklyStatsResponse>();

        // Create another application (should be cached and not reflected immediately)
        await CreateApplicationAsync(token);

        // Make second call (within cache window)
        var response2 = await _client.GetAsync("/api/statistics/weekly");
        var result2 = await response2.Content.ReadFromJsonAsync<WeeklyStatsResponse>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        result1.Should().NotBeNull();
        result2.Should().NotBeNull();

        // Results should be the same due to caching
        result1!.ApplicationsThisWeek.Should().Be(result2!.ApplicationsThisWeek);
    }

    [Fact]
    public async Task GetWeeklyStats_ReturnsCorrectResponseStructure()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/statistics/weekly");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var result = await response.Content.ReadFromJsonAsync<WeeklyStatsResponse>();
        result.Should().NotBeNull();

        // Verify all expected properties are present
        result!.ApplicationsThisWeek.Should().BeGreaterThanOrEqualTo(0);
        result.ApplicationsLastWeek.Should().BeGreaterThanOrEqualTo(0);
        result.PercentageChange.Should().BeInRange(-100, decimal.MaxValue);
        result.PointsThisWeek.Should().BeGreaterThanOrEqualTo(0);
        result.PointsLastWeek.Should().BeGreaterThanOrEqualTo(0);
    }
}
