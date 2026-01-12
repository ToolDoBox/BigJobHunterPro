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
}
