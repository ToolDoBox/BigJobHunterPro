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

    /// <summary>
    /// Gets comprehensive analysis of ALL user applications including role keywords and skill frequencies
    /// Unlike GetTopKeywordsAsync which only analyzes successful applications, this analyzes all applications
    /// </summary>
    /// <param name="userId">The user ID to analyze</param>
    /// <param name="topRoleKeywords">Number of top role keywords to return (default: 10)</param>
    /// <param name="topSkills">Number of top skills to return (default: 15)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis containing role keywords, top skills, and total applications analyzed</returns>
    Task<ApplicationsAnalysis> GetApplicationsAnalysisAsync(string userId, int topRoleKeywords = 10, int topSkills = 15, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all cached analytics data for a user.
    /// Should be called when applications are created, updated, or deleted.
    /// </summary>
    /// <param name="userId">The user ID whose cache should be invalidated</param>
    void InvalidateUserCache(string userId);
}
