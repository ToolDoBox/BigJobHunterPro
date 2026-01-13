using BigJobHunterPro.Application.DTOs.Statistics;

namespace Application.Interfaces;

/// <summary>
/// Service for calculating user statistics and metrics
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Gets weekly statistics comparing this week vs last week
    /// </summary>
    /// <param name="userId">The user ID to get statistics for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weekly statistics with comparison data</returns>
    Task<WeeklyStatsResponse> GetWeeklyStatsAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the distribution of applications across different statuses
    /// </summary>
    /// <param name="userId">The user ID to get statistics for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Status distribution data</returns>
    Task<StatusDistributionResponse> GetStatusDistributionAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the distribution of applications across different sources
    /// </summary>
    /// <param name="userId">The user ID to get statistics for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Source distribution data</returns>
    Task<SourceDistributionResponse> GetSourceDistributionAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets average time (in days) from application submission to various milestones
    /// </summary>
    /// <param name="userId">The user ID to get statistics for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Average time to milestone data</returns>
    Task<AverageTimeResponse> GetAverageTimeToMilestonesAsync(string userId, CancellationToken cancellationToken = default);
}
