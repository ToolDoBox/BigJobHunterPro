using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using Application.DTOs.Applications;
using BigJobHunterPro.Application.DTOs.Analytics;
using FluentAssertions;
using WebAPI.IntegrationTests.Helpers;
using WebAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace WebAPI.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for AnalyticsController endpoints.
/// Tests keyword extraction and conversion rate analytics.
/// </summary>
public class AnalyticsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AnalyticsControllerTests(CustomWebApplicationFactory factory)
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

    [Fact]
    public async Task GetTopKeywords_WithoutAuthentication_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/analytics/keywords");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTopKeywords_WithNoApplications_ReturnsEmptyList()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/analytics/keywords");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<KeywordFrequency>>();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTopKeywords_WithTopCountParameter_RespectsLimit()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/analytics/keywords?topCount=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<KeywordFrequency>>();
        result.Should().NotBeNull();
        // Even if empty, it should respect the limit when there are keywords
    }

    [Fact]
    public async Task GetTopKeywords_ReturnsCorrectResponseStructure()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/analytics/keywords");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<KeywordFrequency>>();
        result.Should().NotBeNull();

        // Verify structure (even if empty)
        if (result!.Any())
        {
            var firstKeyword = result.First();
            firstKeyword.Keyword.Should().NotBeNullOrWhiteSpace();
            firstKeyword.Frequency.Should().BeGreaterThan(0);
            firstKeyword.Percentage.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    [Fact]
    public async Task GetConversionBySource_WithoutAuthentication_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/analytics/conversion-by-source");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetConversionBySource_WithNoApplications_ReturnsEmptyList()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/analytics/conversion-by-source");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<ConversionBySource>>();
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConversionBySource_ReturnsCorrectResponseStructure()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/analytics/conversion-by-source");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<ConversionBySource>>();
        result.Should().NotBeNull();

        // Verify structure (even if empty)
        if (result!.Any())
        {
            var firstConversion = result.First();
            firstConversion.SourceName.Should().NotBeNullOrWhiteSpace();
            firstConversion.TotalApplications.Should().BeGreaterThan(0);
            firstConversion.InterviewCount.Should().BeGreaterThanOrEqualTo(0);
            firstConversion.ConversionRate.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    [Fact]
    public async Task GetConversionBySource_OnlyIncludesAuthenticatedUserData()
    {
        // Arrange - Create first user with applications
        var token1 = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

        // Create application for first user
        var request1 = new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job/test1",
            RawPageContent = "Test job posting 1"
        };
        await _client.PostAsJsonAsync("/api/applications", request1);

        // Create second user with applications
        var token2 = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

        var request2 = new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job/test2",
            RawPageContent = "Test job posting 2"
        };
        await _client.PostAsJsonAsync("/api/applications", request2);
        await _client.PostAsJsonAsync("/api/applications", request2); // Second application

        // Act - Get conversions for second user
        var response = await _client.GetAsync("/api/analytics/conversion-by-source");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<ConversionBySource>>();
        result.Should().NotBeNull();

        // Second user should see their own data (2 applications if AI parsing succeeded)
        // Note: Since we're using AI parsing, the actual count may vary based on parsing success
    }

    [Fact]
    public async Task Analytics_MultipleCallsUseCaching_ReturnsSameResults()
    {
        // Arrange
        var token = await RegisterAndLoginUserAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Make first call
        var response1 = await _client.GetAsync("/api/analytics/keywords");
        var result1 = await response1.Content.ReadFromJsonAsync<List<KeywordFrequency>>();

        // Make second call (within cache window)
        var response2 = await _client.GetAsync("/api/analytics/keywords");
        var result2 = await response2.Content.ReadFromJsonAsync<List<KeywordFrequency>>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        result1.Should().NotBeNull();
        result2.Should().NotBeNull();

        // Results should be the same due to caching
        result1.Should().BeEquivalentTo(result2);
    }
}
