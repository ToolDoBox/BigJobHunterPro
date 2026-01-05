using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the given user with configured expiration
    /// </summary>
    /// <param name="user">The ApplicationUser to generate a token for</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(ApplicationUser user);

    /// <summary>
    /// Gets the configured token expiration date
    /// </summary>
    /// <returns>DateTime representing when the token expires</returns>
    DateTime GetTokenExpiration();
}
