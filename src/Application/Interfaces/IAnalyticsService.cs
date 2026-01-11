using BigJobHunterPro.Application.DTOs.Analytics;

namespace Application.Interfaces;

/// <summary>
/// Service for calculating meta-analytics on user applications
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Gets top keywords from successful applications (those that reached interview stage or beyond)
    /// </summary>
    /// <param name="userId">The user ID to analyze</param>
    /// <param name="topCount">Number of top keywords to return (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of keywords with their frequencies</returns>
    Task<List<KeywordFrequency>> GetTopKeywordsAsync(string userId, int topCount = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets conversion rates by application source/platform
    /// </summary>
    /// <param name="userId">The user ID to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of sources with their conversion rates</returns>
    Task<List<ConversionBySource>> GetConversionBySourceAsync(string userId, CancellationToken cancellationToken = default);
}
