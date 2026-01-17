using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Data;

/// <summary>
/// Repository interface for TimelineEvent entity.
/// </summary>
public interface ITimelineEventRepository : IRepository<TimelineEvent>
{
    /// <summary>
    /// Gets all timeline events for a specific application.
    /// </summary>
    Task<IEnumerable<TimelineEvent>> GetByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets timeline events filtered by event type.
    /// </summary>
    Task<IEnumerable<TimelineEvent>> GetByEventTypeAsync(
        Guid applicationId,
        EventType eventType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent timeline event for an application.
    /// </summary>
    Task<TimelineEvent?> GetMostRecentAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts timeline events for an application.
    /// </summary>
    Task<int> CountByApplicationIdAsync(
        Guid applicationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sums points for timeline events within a date range for a user.
    /// </summary>
    Task<int> SumPointsByUserIdInRangeAsync(
        string userId,
        DateTime startInclusive,
        DateTime endExclusive,
        CancellationToken cancellationToken = default);
}
