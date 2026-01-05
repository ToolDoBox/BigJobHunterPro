using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using FluentAssertions;
using WebAPI.IntegrationTests.Helpers;
using WebAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace WebAPI.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for AuthController endpoints.
/// Tests use in-memory database and WebApplicationFactory for isolated testing.
/// Each test is independent and manages its own test data.
/// </summary>
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Register Endpoint Tests

    [Fact]
    public async Task Register_WithValidData_Returns201AndToken()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = TestDataHelper.GenerateUniqueEmail(),
            Password = "Test123!",
            DisplayName = "New Hunter"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        result.Should().NotBeNull();
        result!.UserId.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(request.Email);
        result.Token.Should().NotBeNullOrEmpty();

        // Validate JWT format (should have 3 parts: header.payload.signature)
        result.Token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        // Arrange
        var email = TestDataHelper.GenerateUniqueEmail();
        var firstRequest = new RegisterRequest
        {
            Email = email,
            Password = "Test123!",
            DisplayName = "First User"
        };

        // Create first user
        await _client.PostAsJsonAsync("/api/auth/register", firstRequest);

        // Create duplicate request
        var duplicateRequest = new RegisterRequest
        {
            Email = email,
            Password = "DifferentPassword123!",
            DisplayName = "Duplicate User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", duplicateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.Error.Should().Contain("Email already registered");
        error.Details.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_Returns400()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "not-a-valid-email",
            Password = "Test123!",
            DisplayName = "Test User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.Error.Should().Contain("Validation failed");
    }

    [Fact]
    public async Task Register_WithWeakPassword_Returns400()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = TestDataHelper.GenerateUniqueEmail(),
            Password = "weak", // Too short, no uppercase, no digit
            DisplayName = "Test User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.Details.Should().NotBeEmpty();
    }

    #endregion

    #region Login Endpoint Tests

    [Fact]
    public async Task Login_WithValidCredentials_Returns200AndToken()
    {
        // Arrange - Create a user first
        var email = TestDataHelper.GenerateUniqueEmail();
        var password = "Test123!";
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = password,
            DisplayName = "Test User"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.UserId.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(email);
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        // Validate JWT format
        result.Token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns401()
    {
        // Arrange - Create a user first
        var email = TestDataHelper.GenerateUniqueEmail();
        var correctPassword = "Test123!";
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = correctPassword,
            DisplayName = "Test User"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.Error.Should().Contain("Invalid credentials");
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_Returns401()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.Error.Should().Contain("Invalid credentials");
        // Should NOT reveal that user doesn't exist (security best practice)
    }

    #endregion

    #region Protected Endpoint Tests (/me)

    [Fact]
    public async Task GetMe_WithoutToken_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_WithInvalidToken_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "invalid.token.here");

        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_WithValidToken_Returns200AndUserData()
    {
        // Arrange - Register and get token
        var email = TestDataHelper.GenerateUniqueEmail();
        var displayName = "Test Hunter";
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = "Test123!",
            DisplayName = displayName
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();

        // Create new client with auth token
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", registerResult!.Token);

        // Act
        var response = await authenticatedClient.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GetMeResponse>();
        result.Should().NotBeNull();
        result!.UserId.Should().Be(registerResult.UserId);
        result.Email.Should().Be(email);
        result.DisplayName.Should().Be(displayName);
        result.Points.Should().Be(0); // New user starts with 0 points
    }

    #endregion
}
