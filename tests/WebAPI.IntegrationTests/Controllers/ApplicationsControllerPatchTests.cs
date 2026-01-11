using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Applications;
using Application.DTOs.Auth;
using Domain.Enums;
using FluentAssertions;
using WebAPI.IntegrationTests.Helpers;
using WebAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace WebAPI.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for PATCH /api/applications/{id}/status endpoint.
/// Ensures the dedicated status update endpoint works correctly with proper point calculations.
/// </summary>
public class ApplicationsControllerPatchTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ApplicationsControllerPatchTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> CreateAuthenticatedUserAsync()
    {
        var registerRequest = TestDataHelper.ValidRegisterRequest;
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        return result!.Token;
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var token = await CreateAuthenticatedUserAsync();
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return authenticatedClient;
    }

    [Fact]
    public async Task PatchApplicationStatus_WithValidStatus_Returns200AndUpdatesPoints()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();

        // Create an application
        var createRequest = new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job",
            RawPageContent = "Test job content"
        };
        var createResponse = await client.PostAsJsonAsync("/api/applications", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        // Act - Update status to Interview
        var updateRequest = new UpdateStatusRequest
        {
            Status = ApplicationStatus.Interview
        };
        var response = await client.PatchAsJsonAsync(
            $"/api/applications/{created!.Id}/status",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApplicationDto>();
        result.Should().NotBeNull();
        result!.Status.Should().Be("Interview");
        result.Points.Should().Be(6); // Applied (1) + Interview (5) = 6 points
    }

    [Fact]
    public async Task PatchApplicationStatus_ToSameStatus_Returns200WithoutAddingPoints()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();

        // Create an application (Applied = 1 point)
        var createRequest = new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job",
            RawPageContent = "Test job content"
        };
        var createResponse = await client.PostAsJsonAsync("/api/applications", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        // Act - Update to Applied again (same status)
        var updateRequest = new UpdateStatusRequest
        {
            Status = ApplicationStatus.Applied
        };
        var response = await client.PatchAsJsonAsync(
            $"/api/applications/{created!.Id}/status",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApplicationDto>();
        result.Should().NotBeNull();
        result!.Status.Should().Be("Applied");
        result.Points.Should().Be(1); // Should still be 1 point, no duplicate
    }

    [Fact]
    public async Task PatchApplicationStatus_WithInvalidId_Returns404()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();
        var invalidId = Guid.NewGuid();

        // Act
        var updateRequest = new UpdateStatusRequest
        {
            Status = ApplicationStatus.Interview
        };
        var response = await client.PatchAsJsonAsync(
            $"/api/applications/{invalidId}/status",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PatchApplicationStatus_FromInterviewToOffer_AwardsCorrectPoints()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync();

        // Create and update to Interview first
        var createRequest = new CreateApplicationRequest
        {
            SourceUrl = "https://example.com/job",
            RawPageContent = "Test job content"
        };
        var createResponse = await client.PostAsJsonAsync("/api/applications", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateApplicationResponse>();

        await client.PatchAsJsonAsync(
            $"/api/applications/{created!.Id}/status",
            new UpdateStatusRequest { Status = ApplicationStatus.Interview }
        );

        // Act - Update to Offer
        var offerRequest = new UpdateStatusRequest
        {
            Status = ApplicationStatus.Offer
        };
        var response = await client.PatchAsJsonAsync(
            $"/api/applications/{created.Id}/status",
            offerRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApplicationDto>();
        result.Should().NotBeNull();
        result!.Status.Should().Be("Offer");
        result.Points.Should().Be(56); // Applied (1) + Interview (5) + Offer (50) = 56 points
    }

    [Fact]
    public async Task PatchApplicationStatus_Unauthenticated_Returns401()
    {
        // Arrange - Use unauthenticated client
        var updateRequest = new UpdateStatusRequest
        {
            Status = ApplicationStatus.Interview
        };

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/applications/{Guid.NewGuid()}/status",
            updateRequest
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
