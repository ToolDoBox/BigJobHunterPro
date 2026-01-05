using Application.DTOs.Auth;

namespace WebAPI.IntegrationTests.Helpers;

/// <summary>
/// Helper class for generating test data.
/// Ensures test isolation by providing unique data for each test.
/// </summary>
public static class TestDataHelper
{
    /// <summary>
    /// Generates a unique email address for each test to prevent conflicts.
    /// </summary>
    public static string GenerateUniqueEmail() =>
        $"testuser{Guid.NewGuid():N}@example.com";

    /// <summary>
    /// Provides valid registration request data.
    /// </summary>
    public static RegisterRequest ValidRegisterRequest => new()
    {
        Email = GenerateUniqueEmail(),
        Password = "Test123!",
        DisplayName = "Test User"
    };

    /// <summary>
    /// Provides valid login request data.
    /// </summary>
    public static LoginRequest ValidLoginRequest(string email, string password) => new()
    {
        Email = email,
        Password = password
    };

    /// <summary>
    /// Provides registration request with invalid email format.
    /// </summary>
    public static RegisterRequest RegisterRequestWithInvalidEmail => new()
    {
        Email = "not-an-email",
        Password = "Test123!",
        DisplayName = "Test User"
    };

    /// <summary>
    /// Provides registration request with password that's too short.
    /// </summary>
    public static RegisterRequest RegisterRequestWithShortPassword => new()
    {
        Email = GenerateUniqueEmail(),
        Password = "short",
        DisplayName = "Test User"
    };

    /// <summary>
    /// Provides registration request with weak password (no uppercase or digits).
    /// </summary>
    public static RegisterRequest RegisterRequestWithWeakPassword => new()
    {
        Email = GenerateUniqueEmail(),
        Password = "nouppercaseordigit",
        DisplayName = "Test User"
    };
}
