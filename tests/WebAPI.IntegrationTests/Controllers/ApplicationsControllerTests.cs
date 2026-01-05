using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Applications;
using Application.DTOs.Auth;
using Domain.Entities;
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

    #endregion

    #region POST /api/applications - Success Tests

    [Fact]
    public async Task CreateApplication_WithValidData_Returns201AndApplication()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            CompanyName = "Test Company",
            RoleTitle = "Software Engineer",
            SourceName = "LinkedIn"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.CompanyName.Should().Be("Test Company");
        result.RoleTitle.Should().Be("Software Engineer");
        result.SourceName.Should().Be("LinkedIn");
        result.Status.Should().Be("Applied");
        result.Points.Should().Be(1);
        result.TotalPoints.Should().Be(1);
        result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify Location header is set
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/applications/{result.Id}");
    }

    [Fact]
    public async Task CreateApplication_WithSourceUrl_Returns201AndIncludesUrl()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            CompanyName = "Tech Corp",
            RoleTitle = "Backend Developer",
            SourceName = "Indeed",
            SourceUrl = "https://indeed.com/job/12345"
        };

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
    public async Task CreateApplication_WithMissingCompanyName_Returns400()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            CompanyName = "", // Missing required field
            RoleTitle = "Software Engineer",
            SourceName = "LinkedIn"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Company name is required");
    }

    [Fact]
    public async Task CreateApplication_WithMissingRoleTitle_Returns400()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            CompanyName = "Test Company",
            RoleTitle = "", // Missing required field
            SourceName = "LinkedIn"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Role title is required");
    }

    [Fact]
    public async Task CreateApplication_WithMissingSourceName_Returns400()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            CompanyName = "Test Company",
            RoleTitle = "Software Engineer",
            SourceName = "" // Missing required field
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Source name is required");
    }

    [Fact]
    public async Task CreateApplication_WithInvalidUrl_Returns400()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            CompanyName = "Test Company",
            RoleTitle = "Software Engineer",
            SourceName = "LinkedIn",
            SourceUrl = "not-a-valid-url"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Source URL must be a valid URL");
    }

    [Fact]
    public async Task CreateApplication_WithTooLongCompanyName_Returns400()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            CompanyName = new string('A', 201), // Exceeds 200 char limit
            RoleTitle = "Software Engineer",
            SourceName = "LinkedIn"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Company name cannot exceed 200 characters");
    }

    #endregion

    #region POST /api/applications - Authentication Tests

    [Fact]
    public async Task CreateApplication_WithoutToken_Returns401()
    {
        // Arrange
        var request = new CreateApplicationRequest
        {
            CompanyName = "Test Company",
            RoleTitle = "Software Engineer",
            SourceName = "LinkedIn"
        };

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

        var request = new CreateApplicationRequest
        {
            CompanyName = "Test Company",
            RoleTitle = "Software Engineer",
            SourceName = "LinkedIn"
        };

        // Act
        var response = await invalidClient.PostAsJsonAsync("/api/applications", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Points and Database Tests

    [Fact]
    public async Task CreateApplication_AddsPointsToUser()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request1 = new CreateApplicationRequest
        {
            CompanyName = "Company 1",
            RoleTitle = "Role 1",
            SourceName = "LinkedIn"
        };
        var request2 = new CreateApplicationRequest
        {
            CompanyName = "Company 2",
            RoleTitle = "Role 2",
            SourceName = "Indeed"
        };

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
        meResult!.Points.Should().Be(0); // Old Points field
        meResult.TotalPoints.Should().Be(2); // New TotalPoints field
    }

    [Fact]
    public async Task CreateApplication_StoresApplicationInDatabase()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var request = new CreateApplicationRequest
        {
            CompanyName = "Database Test Company",
            RoleTitle = "QA Engineer",
            SourceName = "Glassdoor",
            SourceUrl = "https://glassdoor.com/job/99999"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/applications", request);
        var result = await response.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        // Assert - Verify in database
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var application = await dbContext.Applications.FindAsync(result!.Id);
        application.Should().NotBeNull();
        application!.CompanyName.Should().Be("Database Test Company");
        application.RoleTitle.Should().Be("QA Engineer");
        application.SourceName.Should().Be("Glassdoor");
        application.SourceUrl.Should().Be("https://glassdoor.com/job/99999");
        application.Status.Should().Be(Domain.Enums.ApplicationStatus.Applied);
        application.Points.Should().Be(1);
    }

    [Fact]
    public async Task CreateApplication_MultipleUsers_IsolatesPoints()
    {
        // Arrange - Create two different users
        var client1 = await CreateAuthenticatedClientAsync();
        var client2 = await CreateAuthenticatedClientAsync();

        var request = new CreateApplicationRequest
        {
            CompanyName = "Shared Company",
            RoleTitle = "Engineer",
            SourceName = "LinkedIn"
        };

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
