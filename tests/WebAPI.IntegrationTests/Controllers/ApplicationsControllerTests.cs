using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Applications;
using Application.DTOs.Auth;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.IntegrationTests.Helpers;
using WebAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace WebAPI.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for ApplicationsController endpoints.
/// Tests use in-memory database and WebApplicationFactory for isolated testing.
/// Each test is independent and manages its own test data.
/// </summary>
public class ApplicationsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ApplicationsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Helper Methods

    /// <summary>
    /// Creates an authenticated user and returns the auth token
    /// </summary>
    private async Task<string> CreateAuthenticatedUserAsync()
    {
        var registerRequest = TestDataHelper.ValidRegisterRequest;
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        return result!.Token;
    }

    /// <summary>
    /// Creates an authenticated HttpClient with Bearer token
    /// </summary>
    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var token = await CreateAuthenticatedUserAsync();
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return authenticatedClient;
    }

    private static CreateApplicationRequest CreateValidQuickCapture(string? sourceUrl = null)
    {
        return new CreateApplicationRequest
        {
            SourceUrl = sourceUrl,
            RawPageContent = "Sample job listing content for parsing."
        };
    }

    #endregion

    #region POST /api/applications - Success Tests

    [Fact]
    public async Task CreateApplication_WithValidData_Returns201AndApplication()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = CreateValidQuickCapture("https://indeed.com/job/12345");

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.SourceUrl.Should().Be("https://indeed.com/job/12345");
        result.Status.Should().Be("Applied");
        result.AiParsingStatus.Should().Be("Pending");
        result.Points.Should().Be(1);
        result.TotalPoints.Should().Be(1);
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify Location header is set
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString()
            .Should()
            .ContainEquivalentOf($"/api/applications/{result.Id}");
    }

    [Fact]
    public async Task CreateApplication_WithSourceUrl_Returns201AndIncludesUrl()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = CreateValidQuickCapture("https://indeed.com/job/12345");

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>();
        result.Should().NotBeNull();
        result!.SourceUrl.Should().Be("https://indeed.com/job/12345");
    }

    #endregion

    #region POST /api/applications - Validation Tests

    [Fact]
    public async Task CreateApplication_WithMissingRawPageContent_Returns400()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job/123",
            RawPageContent = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Job page content is required");
    }

    [Fact]
    public async Task CreateApplication_WithInvalidUrl_Returns400()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            SourceUrl = "not-a-valid-url",
            RawPageContent = "Sample content"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Source URL must be a valid URL");
    }

    #endregion

    #region POST /api/applications - Authentication Tests

    [Fact]
    public async Task CreateApplication_WithoutToken_Returns401()
    {
        // Arrange
        var request = CreateValidQuickCapture("https://example.com/job/123");

        // Act (using unauthenticated client)
        var response = await _client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateApplication_WithInvalidToken_Returns401()
    {
        // Arrange
        var invalidClient = _factory.CreateClient();
        invalidClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "invalid.token.here");

        var request = CreateValidQuickCapture("https://example.com/job/123");

        // Act
        var response = await invalidClient.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region GET /api/applications - List Tests

    [Fact]
    public async Task GetApplications_WithoutToken_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/applications");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetApplications_WithNoApplications_ReturnsEmptyList()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync("/api/applications");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApplicationsListResponse>();
        result.Should().NotBeNull();
        result!.Items.Should().BeEmpty();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(25);
        result.HasMore.Should().BeFalse();
    }

    [Fact]
    public async Task GetApplications_ReturnsNewestFirst()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();

        var firstRequest = CreateValidQuickCapture("https://example.com/job/first");
        var firstResponse = await client.PostAsJsonAsync("/api/applications", firstRequest);
        var firstResult = await firstResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        await Task.Delay(50);

        var secondRequest = CreateValidQuickCapture("https://example.com/job/second");
        var secondResponse = await client.PostAsJsonAsync("/api/applications", secondRequest);
        var secondResult = await secondResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        // Act
        var listResponse = await client.GetAsync("/api/applications");
        var listResult = await listResponse.Content.ReadFromJsonAsync<ApplicationsListResponse>();

        // Assert
        listResult.Should().NotBeNull();
        listResult!.Items.Should().HaveCount(2);
        listResult.Items[0].Id.Should().Be(secondResult!.Id);
        listResult.Items[1].Id.Should().Be(firstResult!.Id);
    }

    [Fact]
    public async Task GetApplications_FiltersByCurrentUser()
    {
        // Arrange
        var client1 = await CreateAuthenticatedClientAsync();
        var client2 = await CreateAuthenticatedClientAsync();

        var request1 = CreateValidQuickCapture("https://example.com/job/user-one");
        var response1 = await client1.PostAsJsonAsync("/api/applications", request1);
        var result1 = await response1.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        var request2 = CreateValidQuickCapture("https://example.com/job/user-two");
        await client2.PostAsJsonAsync("/api/applications", request2);

        // Act
        var response = await client1.GetAsync("/api/applications");
        var result = await response.Content.ReadFromJsonAsync<ApplicationsListResponse>();

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().ContainSingle(item => item.Id == result1!.Id);
    }

    [Fact]
    public async Task GetApplications_PaginatesResults()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();

        for (var i = 1; i <= 3; i++)
        {
            var request = CreateValidQuickCapture($"https://example.com/job/{i}");
            await client.PostAsJsonAsync("/api/applications", request);
        }

        // Act
        var page1Response = await client.GetAsync("/api/applications?page=1&pageSize=2");
        var page1 = await page1Response.Content.ReadFromJsonAsync<ApplicationsListResponse>();

        var page2Response = await client.GetAsync("/api/applications?page=2&pageSize=2");
        var page2 = await page2Response.Content.ReadFromJsonAsync<ApplicationsListResponse>();

        // Assert
        page1.Should().NotBeNull();
        page1!.Items.Should().HaveCount(2);
        page1.Page.Should().Be(1);
        page1.PageSize.Should().Be(2);
        page1.HasMore.Should().BeTrue();

        page2.Should().NotBeNull();
        page2!.Items.Should().HaveCount(1);
        page2.Page.Should().Be(2);
        page2.PageSize.Should().Be(2);
        page2.HasMore.Should().BeFalse();
    }

    #endregion

    #region GET/PUT/DELETE /api/applications/{id} - Detail Tests

    [Fact]
    public async Task GetApplication_WithValidId_ReturnsDetail()
    {
        var client = await CreateAuthenticatedClientAsync();
        var createRequest = CreateValidQuickCapture("https://example.com/job/detail");
        var createResponse = await client.PostAsJsonAsync("/api/applications", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        var response = await client.GetAsync($"/api/applications/{createResult!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApplicationDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(createResult.Id);
        result.RawPageContent.Should().Be(createRequest.RawPageContent);
        result.AiParsingStatus.Should().Be("Pending");
        result.Status.Should().Be("Applied");
    }

    [Fact]
    public async Task GetApplication_WithInvalidId_Returns404()
    {
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/api/applications/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateApplication_WithValidData_UpdatesFieldsAndPoints()
    {
        var client = await CreateAuthenticatedClientAsync();
        var createRequest = CreateValidQuickCapture("https://example.com/job/update");
        var createResponse = await client.PostAsJsonAsync("/api/applications", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        var updateRequest = new UpdateApplicationRequest
        {
            CompanyName = "Nimbus Labs",
            RoleTitle = "Backend Engineer",
            SourceName = "LinkedIn",
            SourceUrl = "https://linkedin.com/jobs/123",
            Status = "Interview",
            WorkMode = "Remote",
            Location = "Dallas, TX",
            SalaryMin = 120,
            SalaryMax = 150,
            JobDescription = "Build APIs and pipelines.",
            RequiredSkills = new List<string> { "C#", "SQL Server" },
            NiceToHaveSkills = new List<string> { "Azure" }
        };

        var response = await client.PutAsJsonAsync(
            $"/api/applications/{createResult!.Id}",
            updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApplicationDto>();
        result.Should().NotBeNull();
        result!.CompanyName.Should().Be("Nimbus Labs");
        result.RoleTitle.Should().Be("Backend Engineer");
        result.SourceName.Should().Be("LinkedIn");
        result.Status.Should().Be("Interview");
        result.Points.Should().Be(6); // Applied (1) + Interview (5) = 6 points
        result.WorkMode.Should().Be("Remote");

        var meResponse = await client.GetAsync("/api/auth/me");
        var meResult = await meResponse.Content.ReadFromJsonAsync<GetMeResponse>();
        meResult!.Points.Should().Be(6);
        meResult.TotalPoints.Should().Be(6);
    }

    [Fact]
    public async Task UpdateApplication_WithInvalidStatus_Returns400()
    {
        var client = await CreateAuthenticatedClientAsync();
        var createResponse = await client.PostAsJsonAsync(
            "/api/applications",
            CreateValidQuickCapture("https://example.com/job/invalid"));
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        var updateRequest = new UpdateApplicationRequest
        {
            CompanyName = "Nimbus Labs",
            RoleTitle = "Backend Engineer",
            SourceName = "LinkedIn",
            Status = "NotAStatus"
        };

        var response = await client.PutAsJsonAsync(
            $"/api/applications/{createResult!.Id}",
            updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteApplication_RemovesApplicationAndPoints()
    {
        var client = await CreateAuthenticatedClientAsync();
        var createResponse = await client.PostAsJsonAsync(
            "/api/applications",
            CreateValidQuickCapture("https://example.com/job/delete"));
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        var deleteResponse = await client.DeleteAsync($"/api/applications/{createResult!.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var fetchResponse = await client.GetAsync($"/api/applications/{createResult.Id}");
        fetchResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var meResponse = await client.GetAsync("/api/auth/me");
        var meResult = await meResponse.Content.ReadFromJsonAsync<GetMeResponse>();
        meResult!.Points.Should().Be(0);
        meResult.TotalPoints.Should().Be(0);
    }

    #endregion

    #region Points and Database Tests

    [Fact]
    public async Task CreateApplication_AddsPointsToUser()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request1 = CreateValidQuickCapture("https://example.com/job/one");
        var request2 = CreateValidQuickCapture("https://example.com/job/two");

        // Act - Create two applications
        var response1 = await client.PostAsJsonAsync("/api/applications", request1);
        var result1 = await response1.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        var response2 = await client.PostAsJsonAsync("/api/applications", request2);
        var result2 = await response2.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        // Assert
        result1!.Points.Should().Be(1);
        result1.TotalPoints.Should().Be(1);

        result2!.Points.Should().Be(1);
        result2.TotalPoints.Should().Be(2); // Cumulative total

        // Verify via /me endpoint
        var meResponse = await client.GetAsync("/api/auth/me");
        var meResult = await meResponse.Content.ReadFromJsonAsync<GetMeResponse>();
        meResult!.Points.Should().Be(2);
        meResult.TotalPoints.Should().Be(2);
    }

    [Fact]
    public async Task CreateApplication_StoresApplicationInDatabase()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = CreateValidQuickCapture("https://glassdoor.com/job/99999");

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);
        var result = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        // Assert - Verify in database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var application = await dbContext.Applications.FindAsync(result!.Id);
        application.Should().NotBeNull();
        application!.CompanyName.Should().Be(string.Empty);
        application.RoleTitle.Should().Be(string.Empty);
        application.SourceName.Should().Be("glassdoor.com");
        application.SourceUrl.Should().Be("https://glassdoor.com/job/99999");
        application.RawPageContent.Should().Be(request.RawPageContent);
        application.AiParsingStatus.Should().Be(Domain.Enums.AiParsingStatus.Pending);
        application.Status.Should().Be(Domain.Enums.ApplicationStatus.Applied);
        application.Points.Should().Be(1);
    }

    [Fact]
    public async Task CreateApplication_MultipleUsers_IsolatesPoints()
    {
        // Arrange - Create two different users
        var client1 = await CreateAuthenticatedClientAsync();
        var client2 = await CreateAuthenticatedClientAsync();

        var request = CreateValidQuickCapture("https://example.com/job/shared");

        // Act - Each user creates an application
        var response1 = await client1.PostAsJsonAsync("/api/applications", request);
        var result1 = await response1.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        var response2 = await client2.PostAsJsonAsync("/api/applications", request);
        var result2 = await response2.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        // Assert - Each user should have 1 point independently
        result1!.TotalPoints.Should().Be(1);
        result2!.TotalPoints.Should().Be(1);

        // Verify via /me endpoint for each user
        var me1 = await client1.GetAsync("/api/auth/me");
        var me1Result = await me1.Content.ReadFromJsonAsync<GetMeResponse>();
        me1Result!.TotalPoints.Should().Be(1);

        var me2 = await client2.GetAsync("/api/auth/me");
        var me2Result = await me2.Content.ReadFromJsonAsync<GetMeResponse>();
        me2Result!.TotalPoints.Should().Be(1);
    }

    #endregion
}
